using ICSharpCode.AvalonEdit;
using Lexer.Model;
using Lexer.View;
using Lexer.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lexer
{
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window
	{
		public Brush defaultBrush;

		public MainWindow()
		{
			InitializeComponent();
			ErrorViewModel.getInstance().Init(ErrorDataGrid);
			TokenViewModel.getInstance().Init(TokenDataGrid);
			SourceViewModel.Init(textEditor);
			CommandBinding binding = new CommandBinding(ApplicationCommands.Open);
			binding.Executed += Binding_Open_Executed;
			CommandBindings.Add(binding);
			binding = new CommandBinding(ApplicationCommands.Properties);
			binding.Executed += Binding_Properties_Executed;
			CommandBindings.Add(binding);
		}

		private void Binding_Properties_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			(new PropertiesWindow()).ShowDialog();
		}

		private void Binding_Open_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			if (ofd.ShowDialog().Value == true)
			{
				OpenFile(ofd.FileName);
			}
		}

		private void OpenFile(string path)
		{
			SourceViewModel.SourceData = File.ReadAllBytes(path);
		}

		private void button_Click(object sender, RoutedEventArgs e)
		{
			Model.Lexer.LexAll();
		}

		private void ErrorDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (defaultBrush == null)
			{
				defaultBrush = textEditor.TextArea.SelectionBrush;
			}
			var item = ((sender as DataGrid).SelectedItem as Error);
			if (item != null)
			{
				textEditor.Select(textEditor.Document.GetOffset(item.line, item.lineOffset), item.length);
				textEditor.TextArea.SelectionBrush = Brushes.Red;
				textEditor.ScrollTo(item.line, item.lineOffset);
			}
		}

		private void TokenDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (defaultBrush == null)
			{
				defaultBrush = textEditor.TextArea.SelectionBrush;
			}
			var item = ((sender as DataGrid).SelectedItem as Token);
			if (item != null)
			{
				textEditor.Select(item.offset, item.value.Length);
				textEditor.TextArea.SelectionBrush = Brushes.CadetBlue;
				var loc = textEditor.Document.GetLocation(item.offset);
				textEditor.ScrollTo(loc.Line, loc.Column);
			}
		}

		private void ErrorDataGrid_GotFocus(object sender, RoutedEventArgs e)
		{
			ErrorDataGrid_SelectionChanged((sender as DataGrid), null);
		}

		private void TokenDataGrid_GotFocus(object sender, RoutedEventArgs e)
		{
			TokenDataGrid_SelectionChanged((sender as DataGrid), null);
		}

		private void TokenDataGrid_LostFocus(object sender, RoutedEventArgs e)
		{
			textEditor.TextArea.SelectionBrush = defaultBrush;
			textEditor.Select(0, 0);
		}

		private void ErrorDataGrid_LostFocus(object sender, RoutedEventArgs e)
		{
			textEditor.TextArea.SelectionBrush = defaultBrush;
			textEditor.Select(0, 0);
		}

		private void textEditor_Drop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				OpenFile((e.Data.GetData(DataFormats.FileDrop) as string[])[0]);
			}
		}

		private void SaveCommands_Executed(object sender, ExecutedRoutedEventArgs e)
		{

			if (e.Parameter as string == "result")
			{
				if(!TokenViewModel.getInstance().isEmpty())
				{
					SaveFileDialog sfd = new SaveFileDialog();
					sfd.Filter = "词法分析结果 (*.lexres) | *.lexres";
					sfd.DefaultExt = "lexres";
					sfd.AddExtension = true;
					if (sfd.ShowDialog().Value == true)
					{
						TokenViewModel.getInstance().Save(sfd.FileName);
					}
				}
			}
			else
			{
				SaveFileDialog sfd = new SaveFileDialog();
				if (sfd.ShowDialog().Value == true)
				{
					textEditor.Save(sfd.FileName);
				}
			}
		}
	}
}
