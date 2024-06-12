using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Stephen.JsonSerializer
{
	public class JsonToken
	{
		public JsonToken() { }
	}

	public class StringToken : JsonToken
	{
		public string Value { get; set; }
		public StringToken(string value)
		{
			Value = value;
		}

		public override string ToString()
		{
			return $"str:{Value}";
		}
	}
	public class NumberToken : JsonToken
	{
		public string Value { get; set; }
		public NumberToken(string value)
		{
			Value = value;
		}

		public override string ToString()
		{
			return $"num:{Value}";
		}
	}

	public class UnquotedConstantToken(string value) : JsonToken
	{
		public string Value { get; set; } = value;
	}

	public class ObjectStartToken : JsonToken
	{
	}

	public class ObjectEndToken : JsonToken
	{
	}

	public class ListStartToken : JsonToken
	{
	}

	public class ListEndToken : JsonToken
	{
	}

	public class MemberSeparatorToken : JsonToken
	{
	}

	public class CommaToken : JsonToken
	{
	}


	public class JsonParser
	{
		private StringReader _reader;
		public JsonParser() { }

		public List<JsonToken> Parse(string json)
		{
			var tokens = new List<JsonToken>();
			_reader = new StringReader(json);
			while (_reader.Peek() != -1)
			{
				var c = (char)_reader.Peek();
				if (char.IsWhiteSpace(c))
				{
					_reader.Read();
					continue;
				}

				if (c == '"' || c == '\'')
				{
					var str = ParseString();
					tokens.Add(new StringToken(str));
				}
				else if (char.IsDigit(c) || c == '-')
				{
					var str = ParseNumber();
					tokens.Add(new NumberToken(str));
					//_reader.Read();
				}
				else if (c == 't' || c == 'T' || c == 'f' || c == 'F' || c == 'n' || c == 'N')
				{
					var value = ParseUnquotedConstant();
					tokens.Add(new UnquotedConstantToken(value));
					//_reader.Read();
				}
				else if (c == '{')
				{
					tokens.Add(new ObjectStartToken());
					_reader.Read();
				}
				else if (c == '}')
				{
					tokens.Add(new ObjectEndToken());
					_reader.Read();
				}
				else if (c == '[')
				{
					tokens.Add(new ListStartToken());
					_reader.Read();
				}
				else if (c == ']')
				{
					tokens.Add(new ListEndToken());
					_reader.Read();
				}
				else if (c == ':')
				{
					tokens.Add(new MemberSeparatorToken());
					_reader.Read();
				}
				else if (c == ',')
				{
					tokens.Add(new CommaToken());
					_reader.Read();
				}
				else
					throw new Exception("Unknown character in expression: " + c);
			}
			return tokens;
		}

		private string ParseString()
		{
			_reader.Read(); //skip first quote
			var sb = new StringBuilder(1024);
			while (_reader.Peek() != '"' && _reader.Peek() != '\'')
			{
				sb.Append((char)_reader.Read());
			}
			_reader.Read();     //skip last quote
			return sb.ToString();
		}

		private string ParseNumber()
		{
			var sb = new StringBuilder(1024);
			while (char.IsDigit((char)_reader.Peek()) || _reader.Peek() == '.' || _reader.Peek() == '-')
			{
				sb.Append((char)_reader.Read());
			}
			return sb.ToString();
		}

		private string ParseUnquotedConstant()
		{
			var sb = new StringBuilder(1024);
			char c = char.ToLower((char)_reader.Peek());
			while ("true".Any(s => s == c) || "false".Any(s => s == c) || "null".Any(s => s == c))
			{
				sb.Append((char)_reader.Read());
				c = char.ToLower((char)_reader.Peek());
			}

			var test = sb.ToString().ToLower();
			if (test != "true" && test != "false" && test != "null")
				throw new Exception("Invalid - expected true false or null");

			return test;
		}

		public JsonObject ProcessTokens(List<JsonToken> tokens)
		{
			var index = 0;
			return ProcessTokens(tokens.ToArray(), ref index);
		}

		private JsonObject ProcessTokens(JsonToken[] tokens, ref int index)
		{
			var token = tokens[index];
			if (token is StringToken stringToken)
				return new JsonObjectValue { Value = stringToken.Value };
			if (token is NumberToken numberToken)
				return new JsonObjectValue { Value = numberToken.Value };
			if (token is UnquotedConstantToken unquotedConstantToken)
				return new JsonObjectValue { Value = unquotedConstantToken.Value };
			if (token is ObjectStartToken)
				return ProcessObject(tokens, ref index);
			if (token is ListStartToken)
				return ProcessList(tokens, ref index);
			return null;
		}

		private JsonObject ProcessList(JsonToken[] tokens, ref int index)
		{
			//current token should be a start list token
			Debug.Assert(tokens[index] is ListStartToken);
			
			var list = new JsonObjectList();
			var token = tokens[++index];
			while (token is not ListEndToken && index < tokens.Length)
			{
				if (token is StringToken stringToken)
					list.Array.Add(new JsonObjectValue{Value = stringToken.Value});
				else if (token is ObjectStartToken)
					list.Array.Add(ProcessObject(tokens, ref index));
				else if (token is ListStartToken)
					list.Array.Add(ProcessList(tokens, ref index));
				token = tokens[++index];
			}
			
			//current token should be end list
			Debug.Assert(token is ListEndToken);
			return list;
		}

		private JsonObject ProcessObject(JsonToken[] tokens, ref int index)
		{
			//current token should be objectstart
			Debug.Assert(tokens[index] is ObjectStartToken);
			
			var obj = new JsonObjectComplex();
			var token = tokens[++index];
			while (token is not ObjectEndToken && index < tokens.Length)
			{
				if (token is StringToken nameToken)
				{
					token = tokens[++index];
					if (token is MemberSeparatorToken)
					{
						//next token is either constant or complex or list
						index++;
						var value = ProcessTokens(tokens, ref index);
						obj.Complex[nameToken.Value] = value;
					}					
					else
						throw new Exception("Invalid token - expected separator");
				}
				else if (token is CommaToken)
					; //do nothing
				else if (token is ObjectEndToken)
					; //do nothing
				else
					throw new Exception("Invalid token - expected name");
				
				token = tokens[++index];
			}
			
			//the current token should be an end token
			Debug.Assert(token is ObjectEndToken);
			return obj;
		}
	}
}
