using Lexer.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Lexer.ViewModel
{
	class TokenViewModel
	{
		private static TokenViewModel _singleton = new TokenViewModel();
		private ObservableCollection<Token> tokens = new ObservableCollection<Token>();
		private DataGrid tokenDataGrid;

		private TokenViewModel() { }

		static public TokenViewModel getInstance()
		{
			return _singleton;
		}

		public void Save(string path)
		{
			var result = new List<string>();
			foreach(var t in tokens)
			{
				result.Add("< " + t.kind.ToString() + " , " + t.value + " >");
			}
			File.WriteAllLines(path, result.ToList());
		}

		public void Init(DataGrid dGrid)
		{
			tokenDataGrid = dGrid;
			tokenDataGrid.ItemsSource = tokens;
		}

		public void addToken(Token token)
		{
			tokens.Add(token);
		}

		public void clear()
		{
			tokens.Clear();
		}

		public Token getToken(int index)
		{
			if (tokens.Count > index)
			{
				return tokens[index];
			}
			else
			{
				return null;
			}
		}

		public bool isEmpty()
		{
			return tokens.Count == 0 || tokens == null;
		}
	}
}
