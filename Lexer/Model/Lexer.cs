using Lexer.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexer.Model
{
	enum TokenKind
	{
		IF,
		THEN,
		ELSE,
		END,
		REPEAT,
		UNTIL,
		READ,
		WRITE,
		GT,
		ADD,
		SUB,
		MULT,
		DIV,
		SLASH,
		EQU,
		LT,
		LBRA,
		RBRA,
		SEMI,
		ID,
		NUM,
		ANNO,
		ERROR
	};

	enum ErrorKind
	{
		UNNEEDBRA,
		INVALIDCHAR,
		FLOATNUM,
		NESTBRA,
		BRANOTCLOSED,
		DEFAULT
	};

	class Lexer
	{
		static private bool keepAnno = false;
		static private bool keepError = false;

		static Dictionary<string, TokenKind> KeyWords = new Dictionary<string, TokenKind> {
			{ "if", TokenKind.IF } ,
			{ "then", TokenKind.THEN } ,
			{ "else", TokenKind.ELSE } ,
			{ "end", TokenKind.END } ,
			{ "repeat", TokenKind.REPEAT } ,
			{ "until", TokenKind.UNTIL } ,
			{ "read", TokenKind.READ } ,
			{ "write", TokenKind.WRITE } };

		static HashSet<char> symbols = new HashSet<char>
		{ '+', '-', '*', '/', '=', '<', '>', '{', '}', ';' };

		static public Token LexNext()
		{
			int state = 0;
			string value = string.Empty;
			int startOffset = 0;
			ErrorKind errorKind = ErrorKind.DEFAULT;
			char? cQuestion;
			char c;
			while (true)
			{
				cQuestion = SourceViewModel.NextChar();
				if(cQuestion.HasValue)
				{
					c = cQuestion.Value;
					switch (state)
					{
						case 0:
							startOffset = SourceViewModel.GetOffset();
							if (char.IsLetter(c))
							{
								state = 1;
								value += c;
								break;
							}

							if (char.IsDigit(c))
							{
								state = 2;
								value += c;
								break;
							}

							if (char.IsWhiteSpace(c))
							{
								break;
							}

							if (c == '>')
							{
								value = c.ToString();
								return new Token(startOffset, value, TokenKind.GT);
							}

							if (c == '+')
							{
								value = c.ToString();
								return new Token(startOffset, value, TokenKind.ADD); ;
							}

							if (c == '-')
							{
								value = c.ToString();
								return new Token(startOffset, value, TokenKind.SUB);
							}

							if (c == '*')
							{
								value = c.ToString();
								return new Token(startOffset, value, TokenKind.MULT);
							}

							if (c == '/')
							{
								value = c.ToString();
								return new Token(startOffset, value, TokenKind.DIV);
							}

							if (c == '=')
							{
								value = c.ToString();
								return new Token(startOffset, value, TokenKind.EQU);
							}

							if (c == '<')
							{
								value = c.ToString();
								return new Token(startOffset, value, TokenKind.LT);
							}

							if (c == '{')
							{
								value += c;
								state = 3;
								break;
							}

							if (c == '}')
							{
								state = 4;
								value = c.ToString();
								errorKind = ErrorKind.UNNEEDBRA;
								break;
							}

							if (c == ';')
							{
								value = c.ToString();
								return new Token(startOffset, value, TokenKind.SEMI);
							}

							//ERROR
							state = 4;
							value += c;
							errorKind = ErrorKind.INVALIDCHAR;
							break;
						case 1:
							if (char.IsLetter(c))
							{
								value += c;
								break;
							}

							SourceViewModel.putBack();

							if (KeyWords.ContainsKey(value))
							{
								return new Token(startOffset, value, KeyWords[value]);
							}
							else
							{
								return new Token(startOffset, value, TokenKind.ID);
							}
						case 2:
							if (char.IsDigit(c))
							{
								value += c;
								break;
							}

							if(c == '.')
							{
								state = 4;
								value += c;
								errorKind = ErrorKind.FLOATNUM;
								break;
							}

							SourceViewModel.putBack();

							return new Token(startOffset, value, TokenKind.NUM);
						case 3:
							if (c == '{')
							{
								state = 3;
								value += c;
								errorKind = ErrorKind.NESTBRA;
								break;
							}
							if (c == '}')
							{
								value += c;
								if (errorKind == ErrorKind.NESTBRA)
								{
									HandleError(startOffset, value, ErrorKind.NESTBRA);
								}
								return new Token(startOffset, value, TokenKind.ANNO);
							}
							value += c;
							break;
						case 4:
							switch (errorKind)
							{
								case ErrorKind.FLOATNUM:
									if (char.IsDigit(c) || c == '.')
									{
										value += c;
									}
									else
									{
										SourceViewModel.putBack();
										HandleError(startOffset, value, errorKind);
										return new Token(startOffset, value, TokenKind.ERROR);
									}
									break;
								case ErrorKind.INVALIDCHAR:
									if(char.IsLetterOrDigit(c) || symbols.Contains(c) || char.IsWhiteSpace(c))
									{
										SourceViewModel.putBack();
										HandleError(startOffset, value, errorKind);
										return new Token(startOffset, value, TokenKind.ERROR);
									}
									else
									{
										value += c;
									}
									break;
								default:
									if (char.IsWhiteSpace(c))
									{
										HandleError(startOffset, value, errorKind);
										return new Token(startOffset, value, TokenKind.ERROR);
									}
									else
									{
										value += c;
									}
									break;
							}
							break;
					}
				}
				else
				{
					switch (state)
					{
						case 0:
							return null;
						case 1:
							if (KeyWords.ContainsKey(value))
							{
								return new Token(startOffset, value, KeyWords[value]);
							}
							else
							{
								return new Token(startOffset, value, TokenKind.ID);
							}
						case 2:
							return new Token(startOffset, value, TokenKind.NUM);
						case 3:
							HandleError(startOffset, value, ErrorKind.BRANOTCLOSED);
							return new Token(startOffset, value, TokenKind.ERROR);
						case 4:
							HandleError(startOffset, value, errorKind);
							return new Token(startOffset, value, TokenKind.ERROR);
					}
				}
			}
		}

		static private void HandleError(int eOffset, string eValue, ErrorKind ekind)
		{
			Error e = new Error();
			e.line = SourceViewModel.GetLine(eOffset);
			e.lineOffset = SourceViewModel.GetLineOffset(eOffset);
			e.length = eValue.Length;
			e.kind = ekind;
			switch (ekind)
			{
				case ErrorKind.BRANOTCLOSED:
					e.infomation = "注释未闭合";
					break;
				case ErrorKind.FLOATNUM:
					e.infomation = string.Format("数字 {0} 格式错误", eValue);
					break;
				case ErrorKind.INVALIDCHAR:
					e.infomation = string.Format("无法识别的字符: {0}", eValue);
					break;
				case ErrorKind.NESTBRA:
					e.infomation = "出现嵌套的注释";
					break;
				default:
					e.infomation = string.Format("未知类型错误: {0}", eValue);
					break;
			}
			ErrorViewModel.getInstance().addError(e);
		}

		static public void LexAll()
		{
			SourceViewModel.Reset();
			SourceViewModel.KeepOnlyRead();
			TokenViewModel.getInstance().clear();
			ErrorViewModel.getInstance().clear();
			Token token;
			while(true)
			{
				token = LexNext();
				if(token != null)
				{
					switch (token.kind)
					{
						case TokenKind.ANNO:
							if (keepAnno)
							{
								TokenViewModel.getInstance().addToken(token);
							}
							break;
						case TokenKind.ERROR:
							if (keepError)
							{
								TokenViewModel.getInstance().addToken(token);
							}
							break;
						default:
							TokenViewModel.getInstance().addToken(token);
							break;
					}

				}
				else
				{
					break;
				}
			}
			SourceViewModel.UnkeepOnlyRead();
		}
	}
}
