using Lexer.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Lexer.ViewModel
{
	class ErrorViewModel
	{
		private static ErrorViewModel _singleton = new ErrorViewModel();
		private ObservableCollection<Error> errors = new ObservableCollection<Error>();
		private DataGrid errorDataGrid;

		private ErrorViewModel() { }

		static public ErrorViewModel getInstance()
		{
			return _singleton;
		}

		public void Init(DataGrid dGrid)
		{
			errorDataGrid = dGrid;
			errorDataGrid.ItemsSource = errors;
		}

		public void addError(Error error)
		{
			errors.Add(error);
		}

		public void clear()
		{
			errors.Clear();
		}

		public Error getError(int index)
		{
			if(errors.Count > index)
			{
				return errors[index];
			}
			else
			{
				return null;
			}
		}
	}
}
