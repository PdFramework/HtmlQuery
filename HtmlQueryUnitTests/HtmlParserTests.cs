namespace PeinearyDevelopment.Web.HtmlQueryUnitTests
{
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using PeinearyDevelopment.Web.HtmlQuery;

    [TestClass]
    public class HtmlParserTests
    {
        #region LocalVariablesAndTestInitialize
        private StreamReader reader;

        [TestInitialize]
        public void Initialize()
        {
            reader = new StreamReader("HtmlTestData.txt");
        }
        #endregion

        #region CommentTests
        [TestMethod]
        public void ContinuousComment()
        {
            reader.AdvanceReaderToTestLine(2);
            reader.AdvanceReaderToTestCharStart("\t<!--".Length);
            var comment = reader.GetComment();
            Assert.AreEqual("---------------------------------------------------------------------------------/", comment.Text);
        }

        [TestMethod]
        public void SmallestComment()
        {
            reader.AdvanceReaderToTestLine(60);
            reader.AdvanceReaderToTestCharStart("<p class=\"tablenote\"><small>The 'x<!--".Length);

            var comment = reader.GetComment();
            Assert.IsNull(comment.Text);
        }
        #endregion

        #region HtmlAttributeTests
        [TestMethod]
        public void GetAttributeNameNoValue()
        {
            reader.AdvanceReaderToTestCharStart("<!DOCTYPE".Length);

            var attributeName = reader.GetAttributeName();
            Assert.AreEqual("html", attributeName);
        }

        [TestMethod]
        public void GetAttributeNameNoValueExtraWhiteSpace()
        {
            reader.AdvanceReaderToTestCharStart("<!DOCTYPE ".Length);

            var attributeName = reader.GetAttributeName();
            Assert.AreEqual("html", attributeName);
        }

        [TestMethod]
        public void GetAttributeName()
        {
            reader.AdvanceReaderToTestLine(1);
            reader.AdvanceReaderToTestCharStart("<html".Length);

            var attributeName = reader.GetAttributeName();
            Assert.AreEqual("xmlns", attributeName);
        }

        [TestMethod]
        public void GetAttributeNameExtraWhiteSpace()
        {
            reader.AdvanceReaderToTestLine(1);
            reader.AdvanceReaderToTestCharStart("<html ".Length);

            var attributeName = reader.GetAttributeName();
            Assert.AreEqual("xmlns", attributeName);
        }

        [TestMethod]
        public void GetAttributeValueNoValue()
        {
            reader.AdvanceReaderToTestCharStart("<!DOCTYPE html".Length);

            var attributeValue = reader.GetAttributeValue("html");
            Assert.AreEqual(attributeValue.Count(), 0);
        }

        [TestMethod]
        public void GetAttributeValue()
        {
            reader.AdvanceReaderToTestLine(1);
            reader.AdvanceReaderToTestCharStart("<html xmlns=".Length);

            var attributeValue = reader.GetAttributeValue("xmlns");
            Assert.AreEqual(attributeValue.First(), "http://www.w3.org/1999/xhtml");
        }

        [TestMethod]
        public void GetAttributeValueExtraWhiteSpace()
        {
            reader.AdvanceReaderToTestLine(1);
            reader.AdvanceReaderToTestCharStart("<html xmlns=\"http://www.w3.org/1999/xhtml\" lang=".Length);

            var attributeValue = reader.GetAttributeValue("lang");
            Assert.AreEqual(attributeValue.First(), "en");
        }

        [TestMethod]
        public void GetAttributeValueNestedSingleQuotes()
        {
            reader.AdvanceReaderToTestLine(7);
            reader.AdvanceReaderToTestCharStart("		<link rel=".Length);

            var attributeValue = reader.GetAttributeValue("rel");
            Assert.AreEqual(attributeValue.First(), "'shortcut icon'");
        }

        [TestMethod]
        public void GetAttributeValueNestedDoubleQuotes()
        {
            reader.AdvanceReaderToTestLine(8);
            reader.AdvanceReaderToTestCharStart("		<link rel=".Length);

            var attributeValue = reader.GetAttributeValue("rel");
            Assert.AreEqual(attributeValue.First(), "\"icon\"");
        }

        [TestMethod]
        public void GetAttributes()
        {
            reader.AdvanceReaderToTestLine(33);
            reader.AdvanceReaderToTestCharStart("		<div ".Length);

            var attributes = reader.GetElementAttributes();
            Assert.IsTrue(attributes.ContainsKey("id"));
            Assert.IsTrue(attributes["id"].First() == "header-wrap");
            Assert.IsTrue(attributes.Count == 1);
        }

        [TestMethod]
        public void GetAttributesExtraWhiteSpace()
        {
            reader.AdvanceReaderToTestLine(33);
            reader.AdvanceReaderToTestCharStart("		<div".Length);

            var attributes = reader.GetElementAttributes();
            Assert.IsTrue(attributes.ContainsKey("id"));
            Assert.IsTrue(attributes["id"].First() == "header-wrap");
            Assert.IsTrue(attributes.Count == 1);
        }

        [TestMethod]
        public void GetAttributesNestedSingleQuotes()
        {
            reader.AdvanceReaderToTestLine(7);
            reader.AdvanceReaderToTestCharStart("		<link ".Length);

            var attributes = reader.GetElementAttributes();
            Assert.IsTrue(attributes.ContainsKey("rel"));
            Assert.IsTrue(attributes["rel"].First() == "'shortcut icon'");
            Assert.IsTrue(attributes.Count == 3);
        }

        [TestMethod]
        public void GetLastAttributeValueTrailingWhiteSpace()
        {
            reader.AdvanceReaderToTestLine(7);
            reader.AdvanceReaderToTestCharStart("		<link rel=\"'shortcut icon'\" type=\"image/vnd.microsoft.icon\" href=".Length);

            var attributeValue = reader.GetAttributeValue("href");
            Assert.AreEqual(attributeValue.First(), "/mglstatic/sharedcontent/images/home/favicon.ico");
        }

        [TestMethod]
        public void GetAttributesNestedDoubleQuotes()
        {
            reader.AdvanceReaderToTestLine(8);
            reader.AdvanceReaderToTestCharStart("		<link ".Length);

            var attributes = reader.GetElementAttributes();
            Assert.IsTrue(attributes.ContainsKey("rel"));
            Assert.IsTrue(attributes["rel"].First() == "\"icon\"");
            Assert.IsTrue(attributes.Count == 3);
        }

        [TestMethod]
        public void GetAttributesOpenCloseTagExtraWhiteSpace()
        {
            reader.AdvanceReaderToTestLine(6);

            var attributes = reader.GetElementAttributes();
            Assert.IsTrue(attributes.ContainsKey("content"));
            Assert.IsTrue(attributes["content"].First() == "text/html; charset=utf-8");
            Assert.IsTrue(attributes.Count == 2);
        }

        [TestMethod]
        public void GetMultipleAttributes()
        {
            reader.AdvanceReaderToTestLine(7);
            reader.AdvanceReaderToTestCharStart("		<link ".Length);

            var attributes = reader.GetElementAttributes();
            Assert.IsTrue(attributes.ContainsKey("rel"));
            Assert.IsTrue(attributes.ContainsKey("type"));
            Assert.IsTrue(attributes.ContainsKey("href"));
            Assert.IsTrue(attributes["rel"].First() == "'shortcut icon'");
            Assert.IsTrue(attributes["type"].First() == "image/vnd.microsoft.icon");
            Assert.IsTrue(attributes["href"].First() == "/mglstatic/sharedcontent/images/home/favicon.ico");
            Assert.IsTrue(attributes.Count == 3);
        }
        #endregion

        #region HtmlElementNameTests
        [TestMethod]
        public void GetElementNameOpenTag()
        {
            var elementName = reader.GetElementName();
            Assert.AreEqual(elementName, "!DOCTYPE");
        }

        [TestMethod]
        public void GetElementNameSingleTag()
        {
            reader.AdvanceReaderToTestLine(6);

            var elementName = reader.GetElementName();
            Assert.AreEqual(elementName, "meta");
        }

        [TestMethod]
        public void GetElementNameOpenCloseTag()
        {
            reader.AdvanceReaderToTestLine(4);

            var elementName = reader.GetElementName();
            Assert.AreEqual(elementName, "title");
        }
        #endregion

        #region HtmlElementTests
        [TestMethod]
        public void EndOfFile()
        {
            reader.AdvanceReaderToTestLine(78);
            reader.AdvanceReaderToTestCharStart("</html>".Length);

            var element = reader.GetElement();
            Assert.IsNull(element);
        }

        [TestMethod]
        public void TextOnlyElement()
        {
            reader.AdvanceReaderToTestLine(64);
            reader.AdvanceReaderToTestCharStart("	<div>".Length);

            var element = reader.GetElement();
            Assert.IsNull(element.Attributes);
            Assert.IsNull(element.Descendants);
            Assert.IsFalse(element.HasEndingTag);
            Assert.AreEqual("TEXT", element.TagName);
            Assert.AreEqual("Test1", element.Text);
        }

        [TestMethod]
        public void CommentOnlyElement()
        {
            reader.AdvanceReaderToTestLine(60);
            reader.AdvanceReaderToTestCharStart("<p class=\"tablenote\"><small>The 'x".Length);

            var element = reader.GetElement();
            Assert.IsNull(element.Attributes);
            Assert.IsNull(element.Descendants);
            Assert.IsFalse(element.HasEndingTag);
            Assert.AreEqual("COMMENT", element.TagName);
            Assert.IsNull(element.Text);
        }

        [TestMethod]
        public void SpecialTagOnlyElement()
        {
            reader.AdvanceReaderToTestLine(45);
            reader.AdvanceReaderToTestCharStart("		".Length);

            var element = reader.GetElement();
            Assert.AreEqual(2, element.Attributes.Count);
            Assert.IsNull(element.Descendants);
            Assert.IsTrue(element.HasEndingTag);
            Assert.AreEqual("script", element.TagName);
            Assert.IsNull(element.Text);
        }

        [TestMethod]
        public void GetElement()
        {
            reader.AdvanceReaderToTestLine(60);

            var element = reader.GetElement();
            Assert.AreEqual(1, element.Attributes.Count);
            Assert.IsNull(element.Descendants);
            Assert.IsFalse(element.HasEndingTag);
            Assert.AreEqual("p", element.TagName);
            Assert.IsNull(element.Text);
        }
        #endregion

        #region DomElementTests
        [TestMethod]
        public void ParseHtml()
        {
            var html = reader.ParseDomElement();
            Assert.IsTrue(html.Id.Item1 == 0 && html.Id.Item2 == 0);
            Assert.AreEqual(2, html.Descendants.Count());
            Assert.AreEqual("!DOCTYPE", html.Descendants.First().TagName);
            Assert.IsTrue(html.Descendants.First().Id.Item1 == 1 && html.Descendants.First().Id.Item2 == 0);
            //Assert.IsTrue(html.Descendants.First().Id == 2);
            Assert.AreEqual("html", html.Descendants.Last().TagName);
            Assert.IsNull(html.Descendants.First().Descendants);
            Assert.AreEqual(3, html.Descendants.Last().Descendants.Count());
            var head = html.Descendants.Last().Descendants.First(n => n.TagName == "head");
            var body = html.Descendants.Last().Descendants.Last();
            Assert.AreEqual(11, head.Descendants.Count());
            Assert.AreEqual(6, body.Descendants.Count());
            Assert.AreEqual(2, body.Descendants.Count(d => d.TagName == "div"));
            Assert.AreEqual(2, body.Descendants.Count(d => d.TagName == "script"));
            Assert.AreEqual(1, body.Descendants.Count(d => d.TagName == "style"));
            Assert.AreEqual(1, body.Descendants.Count(d => d.TagName == "p"));
            Assert.AreEqual(3, body.Descendants.Last(d => d.TagName == "div").Descendants.Count(d => d.TagName == "div"));
            var div = body.Descendants.Last(d => d.TagName == "div");
            Assert.AreEqual(1, div.Descendants.Count(d => d.Descendants != null && d.Descendants.Any(d2 => d2.TagName == "div")));
            Assert.AreEqual(html.GetType(), typeof(DomElement));
        }
        #endregion

        #region HtmlElementSpecialNameTests
        [TestMethod]
        public void GetSpecialTag()
        {
            reader.AdvanceReaderToTestLine(16);
            reader.AdvanceReaderToTestCharStart("		<script".Length);

            var scriptElement = reader.GetSpecialTag("script");
            Assert.AreEqual(2, scriptElement.Attributes.Count);
            Assert.IsTrue(scriptElement.Attributes.ContainsKey("type"));
            Assert.IsTrue(scriptElement.Attributes.ContainsKey("src"));
            Assert.AreEqual("text/javascript", scriptElement.Attributes["type"].First());
            Assert.AreEqual("Scripts/jquery-2.0.3.js", scriptElement.Attributes["src"].First());
            Assert.IsNull(scriptElement.Descendants);
            Assert.IsTrue(scriptElement.HasEndingTag);
            Assert.IsNull(scriptElement.Text);
        }

        [TestMethod]
        public void GetSpecialTagUnmatchedCaseName()
        {
            reader.AdvanceReaderToTestLine(16);
            reader.AdvanceReaderToTestCharStart("		<script".Length);

            var scriptElement = reader.GetSpecialTag("ScrIpT");
            Assert.AreEqual(2, scriptElement.Attributes.Count);
            Assert.IsTrue(scriptElement.Attributes.ContainsKey("type"));
            Assert.IsTrue(scriptElement.Attributes.ContainsKey("src"));
            Assert.AreEqual("text/javascript", scriptElement.Attributes["type"].First());
            Assert.AreEqual("Scripts/jquery-2.0.3.js", scriptElement.Attributes["src"].First());
            Assert.IsNull(scriptElement.Descendants);
            Assert.IsTrue(scriptElement.HasEndingTag);
            Assert.IsNull(scriptElement.Text);
        }

        [TestMethod]
        public void GetSpecialTagScriptWithScript()
        {
            reader.AdvanceReaderToTestLine(17);
            reader.AdvanceReaderToTestCharStart("		<script".Length);

            var scriptElement = reader.GetSpecialTag("script");
            Assert.AreEqual(1, scriptElement.Attributes.Count);
            Assert.AreEqual(5, Regex.Matches(scriptElement.Text, "jquery", RegexOptions.IgnoreCase).Count);
        }

        [TestMethod]
        public void GetSpecialTagStyle()
        {
            reader.AdvanceReaderToTestLine(47);
            reader.AdvanceReaderToTestCharStart("		<style".Length);

            var scriptElement = reader.GetSpecialTag("style");
            Assert.AreEqual(1, scriptElement.Attributes.Count);
            Assert.AreEqual(2, Regex.Matches(scriptElement.Text, "-", RegexOptions.IgnoreCase).Count);
            Assert.IsTrue(scriptElement.Text.EndsWith("}"));
            Assert.IsNull(scriptElement.Descendants);
            Assert.IsTrue(scriptElement.HasEndingTag);
            Assert.AreEqual("style", scriptElement.TagName);
        }
        #endregion

        #region HtmlTextElementTests
        [TestMethod]
        public void GetText()
        {
            reader.AdvanceReaderToTestLine(60);
            reader.AdvanceReaderToTestCharStart("<p class=\"tablenote\"><small>".Length);

            var text = reader.GetText();
            Assert.AreEqual("The 'x", text);
        }

        [TestMethod]
        public void GetTextWithWhiteSpace()
        {
            reader.AdvanceReaderToTestLine(61);

            var text = reader.GetText();
            Assert.AreEqual("indicate a font size 50% larger than 'xx-large'.", text);
        }

        [TestMethod]
        public void GetTextLineWrap()
        {
            reader.AdvanceReaderToTestLine(60);
            reader.AdvanceReaderToTestCharStart("<p class=\"tablenote\"><small>The 'x<!---->".Length);

            var text = reader.GetText();
            Assert.AreEqual(@"xx-large' value is a non-CSS value used here to
    indicate a font size 50% larger than 'xx-large'.", text);
        }

        [TestMethod]
        public void GetTextNoText()
        {
            reader.AdvanceReaderToTestLine(16);
            reader.AdvanceReaderToTestCharStart("		<script type=\"text/javascript\" src=\"Scripts/jquery-2.0.3.js\">".Length);

            var text = reader.GetText();
            Assert.IsNull(text);
        }
        #endregion

        #region TestCleanup
        [TestCleanup]
        public void Cleanup()
        {
            reader.Dispose();
        }
        #endregion
    }
}
