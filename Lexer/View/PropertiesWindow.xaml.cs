using Lexer.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Lexer.View
{
	/// <summary>
	/// PropertiesWindow.xaml 的交互逻辑
	/// </summary>
	/// 

	public partial class PropertiesWindow : Window
	{
		public static EncodingInfo[] encodings = Encoding.GetEncodings();
		public static ICollection<FontFamily> fonts = Fonts.SystemFontFamilies;
		public static double fontSize;

		public PropertiesWindow()
		{
			InitializeComponent();
			Array.Sort(encodings, (a, b) => { return a.DisplayName.CompareTo(b.DisplayName); });
			comboBox.ItemsSource = encodings;
			comboBox.SelectedItem = Array.Find(encodings, (a) => { return a.CodePage == SourceViewModel.Encoder.CodePage; });
			FontcomboBox.ItemsSource = fonts;
			FontcomboBox.SelectedItem = SourceViewModel.Font;
			FontSizeComboBox.ItemsSource = new List<double> { 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24 };
			FontSizeComboBox.Text = SourceViewModel.FontSize.ToString();
		}

		private void save_Click(object sender, RoutedEventArgs e)
		{
			if (comboBox.SelectedItem != null)
			{
				SourceViewModel.Encoder = Encoding.GetEncoding(encodings[comboBox.SelectedIndex].CodePage);
			}
			if (FontcomboBox.SelectedItem is FontFamily)
			{
				SourceViewModel.Font = (FontFamily)FontcomboBox.SelectedItem;
			}
			double s;
			if(double.TryParse(FontSizeComboBox.Text,out s))
			{
				SourceViewModel.FontSize = s;
			}
			Close();
		}
	}
}
