namespace PeinearyDevelopment.Web.HtmlQuery
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Text;

	internal static class HtmlParser
	{
		internal static DomElement ParseDomElement(this TextReader reader)
		{
			var htmlPageStack = new Stack<HtmlElement>();
			//var tree = new List<KeyValuePair<string, List<Html>>>();

			while (!reader.EndOfStream())
			{
				var element = reader.GetElement();

				if (element != null && element.TagName != null && element.TagName.StartsWith("/"))
				{
					var descendants = new List<HtmlElement>();
					while (htmlPageStack.Peek().TagName == null || htmlPageStack.Peek().HasEndingTag || !string.Equals(htmlPageStack.Peek().TagName, element.TagName.Substring(1), StringComparison.InvariantCultureIgnoreCase))
					{
						descendants.Add(htmlPageStack.Pop());
						if (htmlPageStack.Count == 0)
						{
							foreach (var descendant in descendants)
							{
								htmlPageStack.Push(descendant);
							}
							descendants = new List<HtmlElement>();
							break;
						}
					}

					var currentElement = htmlPageStack.Pop();
					descendants.Reverse();

					//currentElement.Descendants = descendants; //TODO: research how to handle this correctly. It was erasing descendents with malformed html. Now they are remaining, but order of nodes seems to be incorrect
					currentElement.Descendants = currentElement.Descendants.Concat(descendants);
					currentElement.HasEndingTag = true;
					htmlPageStack.Push(currentElement);

					//tree.Add(new KeyValuePair<string, List<Html>>(htmlPageStack.Peek().TagName, descendants));
				}
				else if (element != null)
				{
					htmlPageStack.Push(element);
				}

				if (reader.Peek() == '/')
				{
					while (reader.Peek() != '>')
					{
						reader.Read();
					}
				}

				if (reader.Peek() == '>')
				{
					reader.Read();
				}
			}

			var domElements = htmlPageStack.ToList();
			domElements.Reverse();

			return new DomElement { TagName = "DOM", Descendants = domElements };
		}

		internal static DomElement ParseDomElement(this string htmlString)
		{
			using (var stream = new MemoryStream(Encoding.Default.GetBytes(htmlString ?? string.Empty)))
			{
				return stream.ParseDomElement();
			}
		}

		internal static DomElement ParseDomElement(this Stream stream)
		{
			using (var reader = new StreamReader(stream))
			{
				return reader.ParseDomElement();
			}
		}

		// ***** TODO: figure out how to do this right. currently using a hack *****
		internal static DomElement ParseDomElement(this WebClient client, string url)
		{
			using (client)
			{
				DomElement htmlElementHtmlElement;
				client.DownloadFile(url, AppDomain.CurrentDomain.BaseDirectory + "temp");
				using (var reader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "temp"))
				{
					htmlElementHtmlElement = reader.ParseDomElement();
				}

				File.Delete(AppDomain.CurrentDomain.BaseDirectory + "temp");
				return htmlElementHtmlElement;
			}
		}

		internal static DomElement ParseDomElement(this WebClient client, Uri url)
		{
			using (client)
			{
				DomElement htmlElementHtmlElement;
				client.DownloadFile(url, AppDomain.CurrentDomain.BaseDirectory + "temp");
				using (var reader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "temp"))
				{
					htmlElementHtmlElement = reader.ParseDomElement();
				}

				File.Delete(AppDomain.CurrentDomain.BaseDirectory + "temp");
				return htmlElementHtmlElement;
			}
		}
		// ***** END TODO *****

		internal static HtmlElement GetElement(this TextReader reader)
		{
			reader.AdvanceReaderThroughWhiteSpace();

			if (reader.EndOfStream())
			{
				return null;
			}

			if (reader.Peek() != '<')
			{
				return new HtmlElement { Text = reader.GetText(), TagName = "TEXT" };
			}

			var elementName = reader.GetElementName();

			if (elementName.StartsWith("!--"))
			{
				return reader.GetComment();
			}

			if (elementName.ToLower() == "script" || elementName.ToLower() == "style")
			{
				return reader.GetSpecialTag(elementName);
			}

			return new HtmlElement { Attributes = reader.GetElementAttributes(), TagName = elementName };
		}

		internal static void AdvanceReaderThroughWhiteSpace(this TextReader reader)
		{
			while (reader.Peek() != -1 && char.IsWhiteSpace((char)reader.Peek()))
			{
				reader.Read();
			}
		}

		internal static bool EndOfStream(this TextReader reader)
		{
			return reader.Peek() == -1;
		}

		internal static string GetElementName(this TextReader reader)
		{
			reader.AdvanceReaderThroughWhiteSpace();
			if (reader.Peek() == '<')
			{
				reader.Read();
			}

			var elementName = new StringBuilder();
			while (!elementName.StartsWith("!--") && reader.Peek() != '>' && !char.IsWhiteSpace((char)reader.Peek()))
			{
				elementName.Append((char)reader.Read());
			}

			return elementName.ToString().Trim();
		}

		internal static HtmlElement GetComment(this TextReader reader)
		{
			reader.AdvanceReaderThroughWhiteSpace();

			var comment = new StringBuilder();
			while (!comment.EndsWith("-->"))
			{
				comment.Append((char)reader.Read());
			}

			var strComment = comment.ToString();
			strComment = strComment.Trim();
			strComment = strComment.Substring(0, strComment.Length - "-->".Length);
			return new HtmlElement { TagName = "COMMENT", Text = !string.IsNullOrWhiteSpace(strComment) ? strComment : null, Descendants = new List<HtmlElement>() };
		}

		internal static HtmlElement GetSpecialTag(this TextReader reader, string name)
		{
			reader.AdvanceReaderThroughWhiteSpace();

			var scriptElement = new HtmlElement
			{
				Attributes = reader.GetElementAttributes(),
				TagName = name,
				HasEndingTag = true
			};

			var specialTag = new StringBuilder();
			while (true)
			{
				specialTag.Append((char)reader.Read());
				if (specialTag.EndsWith("</" + name + ">"))
				{
					break;
				}
			}

			var strSpecialTag = specialTag.ToString();
			strSpecialTag = strSpecialTag.Substring(0, strSpecialTag.Length - ("</" + name + ">").Length).Trim();
			scriptElement.Text = !string.IsNullOrWhiteSpace(strSpecialTag) ? strSpecialTag : null;
			return scriptElement;
		}

		internal static IDictionary<string, IEnumerable<string>> GetElementAttributes(this TextReader reader)
		{
			var attributes = new Dictionary<string, IEnumerable<string>>();

			while (reader.Peek() != '>' && reader.Peek() != '/')
			{
				reader.AdvanceReaderThroughWhiteSpace();

				if (reader.Peek() != '>' && reader.Peek() != '/')
				{
					var attributeName = reader.GetAttributeName();
					if (!attributes.ContainsKey(attributeName))
					{
						attributes.Add(attributeName, reader.GetAttributeValue(attributeName));
					}
					else
					{
						attributes[attributeName] = attributes[attributeName].Concat(reader.GetAttributeValue(attributeName));
					}
				}
			}

			if (reader.Peek() == '/')
			{
				while (reader.Peek() != '>')
				{
					reader.Read();
				}
			}

			if (reader.Peek() == '>')
			{
				reader.Read();
			}

			return attributes;
		}

		internal static string GetAttributeName(this TextReader reader)
		{
			reader.AdvanceReaderThroughWhiteSpace();

			var name = new StringBuilder();

			while (reader.Peek() != '=' && reader.Peek() != '>')
			{
				name.Append((char)reader.Read());
			}

			if (reader.Peek() != '>')
			{
				reader.Read();
			}

			return name.ToString().Trim();
		}

		internal static IEnumerable<string> GetAttributeValue(this TextReader reader, string attributeName)
		{
			reader.AdvanceReaderThroughWhiteSpace();

			var value = new StringBuilder();
			var quoteChar = '\0';

			if (reader.Peek() != '>')
			{
				quoteChar = (char)reader.Read();
			}

			while (reader.Peek() != quoteChar && reader.Peek() != '>')
			{
				value.Append((char)reader.Read());
			}

			if (reader.Peek() == quoteChar)
			{
				reader.Read();
			}

			if (reader.Peek() == '/')
			{
				while (reader.Peek() != '>')
				{
					reader.Read();
				}
			}

			var valueString = value.ToString().Trim();

			if (attributeName.ToLowerInvariant() == "class")
			{
				return valueString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			}

			return valueString == string.Empty ? new string[] { } : new[] { valueString };
		}

		internal static string GetText(this TextReader reader)
		{
			var textBuilder = new StringBuilder();

			AdvanceReaderThroughWhiteSpace(reader);
			while (reader.Peek() != -1 && reader.Peek() != '<')
			{
				textBuilder.Append((char)reader.Read());
			}

			var tempText = textBuilder.ToString().Trim();
			return !string.IsNullOrWhiteSpace(tempText) ? tempText : null;
		}
	}
}
