using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Lexer.ViewModel
{
	class SourceViewModel
	{
		private static int offset = -1;
		private static IDocument document;
		private static TextEditor textEditor;
		private static Encoding encoder = Encoding.Default;
		private static byte[] sourceData;

		public static Encoding Encoder
		{
			get
			{
				return encoder;
			}

			set
			{
				encoder = value;
				if(sourceData != null)
				{
					textEditor.Text = encoder.GetString(sourceData);
				}
			}
		}

		public static byte[] SourceData
		{
			get
			{
				return sourceData;
			}

			set
			{
				sourceData = value;
				if (sourceData != null)
				{
					textEditor.Text = encoder.GetString(sourceData);
				}
			}
		}

		public static FontFamily Font
		{
			get
			{
				return textEditor.FontFamily;
			}

			set
			{
				textEditor.FontFamily = value;
			}
		}

		public static double FontSize
		{
			get
			{
				return textEditor.FontSize;
			}

			set
			{
				textEditor.FontSize = value;
			}
		}

		public static void Init(TextEditor tEditor)
		{
			textEditor = tEditor;
			document = tEditor.Document;
			textEditor.TextChanged += TextChanged;
		}



		private static void TextChanged(object sender, EventArgs e)
		{
			Reset();
		}

		public static void KeepOnlyRead()
		{
			textEditor.IsReadOnly = true;
		}

		public static void UnkeepOnlyRead()
		{
			textEditor.IsReadOnly = false;
			Reset();
		}

		public static void Reset()
		{
			offset = -1;
		}

		public static int GetOffset(int line, int column)
		{
			return document.GetOffset(line, column);
		}

		public static int GetOffset()
		{
			return offset;
		}

		public static int GetLine(int offset)
		{
			return document.GetLocation(offset).Line;
		}

		public static int GetLineOffset(int offset)
		{
			return document.GetLocation(offset).Column;
		}
		
		public static void putBack()
		{
			if(offset >= 0)
			{
				offset--;
			}
		}

		public static char? NextChar()
		{
			if(offset + 1 < document.TextLength)
			{
				offset++;
				return document.GetCharAt(offset);
			}
			else
			{
				return null;
			}
		}
	}
}
