namespace PeinearyDevelopment.Web.HtmlQueryUnitTests
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Runtime.CompilerServices;

	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using PeinearyDevelopment.Web.HtmlQuery;

	[TestClass]
	public class SelectNodesTests
	{
		#region LocalVariablesAndTestInitialize
		private HtmlElement domTree;

		[TestInitialize]
		public void Initialize()
		{
			using (var reader = new StreamReader(@"D:\github\HtmlQuery\HtmlQueryUnitTests\HtmlTestData.txt"))
			{
				domTree = reader.ParseDomElement();
			}
		}
		#endregion

		#region TagNameSelectTests
		[TestMethod]
		public void FirstLevelTagNameSelect()
		{
			var selectedNodes = domTree.SelectNodes("dom");
			Assert.AreEqual(0, selectedNodes.Count());
		}

		[TestMethod]
		public void SecondLevelTagNameSelect()
		{
			var selectedNodes = domTree.SelectNodes("HTML");
			Assert.AreEqual(1, selectedNodes.Count());
		}

		[TestMethod]
		public void ThirdLevelTagNameSelect()
		{
			var selectedNodes = domTree.SelectNodes("head");
			Assert.AreEqual(1, selectedNodes.Count());
		}

		[TestMethod]
		public void MultipleTagNameMatchesInOneNodeSelect()
		{
			var selectedNodes = domTree.SelectNodes("link");
			Assert.AreEqual(5, selectedNodes.Count());
		}

		[TestMethod]
		public void MultipleTagNameMatchesInMultipleNodesSelect()
		{
			var selectedNodes = domTree.SelectNodes("script");
			Assert.AreEqual(7, selectedNodes.Count());
		}

		[TestMethod]
		public void MultipleTagNameMatchesInMultipleNodesAtMultipleDepthsSelect()
		{
			var selectedNodes = domTree.SelectNodes("div");
			Assert.AreEqual(11, selectedNodes.Count());
			Assert.IsTrue(selectedNodes.Any(n => n.Attributes.ContainsKey("id") && n.Attributes["id"].Any(a => a == "header-wrap")));
			Assert.IsTrue(selectedNodes.Any(n => n.SelectNodes("h1").Any()));
		}
		#endregion

		#region AttributeKeyEqualTests
		[TestMethod]
		public void FirstLevelAttributeKeySelect()
		{
			var selectedNodes = domTree.SelectNodes("[xmlns]");
			Assert.AreEqual(1, selectedNodes.Count());
		}

		[TestMethod]
		public void SecondLevelAttributeKeySelect()
		{
			var selectedNodes = domTree.SelectNodes("[data-type]");
			Assert.AreEqual(1, selectedNodes.Count());
		}

		[TestMethod]
		public void ThirdLevelAttributeKeySelect()
		{
			var selectedNodes = domTree.SelectNodes("[http-equiv]");
			Assert.AreEqual(1, selectedNodes.Count());
		}

		[TestMethod]
		public void MultipleAttributeKeyMatchesInOneNodeSelect()
		{
			var selectedNodes = domTree.SelectNodes("[rel]");
			Assert.AreEqual(5, selectedNodes.Count());
		}

		[TestMethod]
		public void MultipleAttributeKeyMatchesInMultipleNodesSelect()
		{
			var selectedNodes = domTree.SelectNodes("[src]");
			Assert.AreEqual(6, selectedNodes.Count());
		}

		[TestMethod]
		public void MultipleAttributeKeyMatchesInMultipleNodesAtMultipleDepthsSelect()
		{
			var selectedNodes = domTree.SelectNodes("[href]");
			Assert.AreEqual(9, selectedNodes.Count());
			// doesn't include: <link rel="stylesheet" type="text/css" media="screen" href="/mglstatic/borrower/stylesheets/ie6.css "></link>
			// because its in a comment, is this a bug?
		}
		#endregion

		#region AttributeValueEqualTests "[key='value']"
		[TestMethod]
		public void FirstLevelAttributeKeyValueSelect()
		{
			var selectedNodes = domTree.SelectNodes("[lang='en']");
			Assert.AreEqual(1, selectedNodes.Count());
		}

		[TestMethod]
		public void SecondLevelAttributeKeyValueSelect()
		{
			var selectedNodes = domTree.SelectNodes("[data-type='testattributematch']");
			Assert.AreEqual(1, selectedNodes.Count());
		}

		[TestMethod]
		public void ThirdLevelAttributeKeyValueSelect()
		{
			var selectedNodes = domTree.SelectNodes("[target='_blank']");
			Assert.AreEqual(1, selectedNodes.Count());
		}

		//[TestMethod]
		//public void MultipleAttributeKeyValueMatchesInOneNodeSelect()
		//{
		//    var selectedNodes = domTree.SelectNodes("[rel='stylesheet']");
		//    Assert.AreEqual(2, selectedNodes.Count());
		//}

		[TestMethod]
		public void MultipleAttributeKeyValueMatchesInMultipleNodesSelect()
		{
			var selectedNodes = domTree.SelectNodes("[type='text/javascript']");
			Assert.AreEqual(5, selectedNodes.Count());
		}

		[TestMethod]
		public void MultipleAttributeKeyValueMatchesInMultipleNodesAtMultipleDepthsSelect()
		{
			var selectedNodes = domTree.SelectNodes("[type='testattributematch']");
			Assert.AreEqual(3, selectedNodes.Count());
		}
		#endregion

		#region AttributeNotEqualTests "[key!='value']"
		// TODO: rework selector in accordance with jquery spec: Select elements that either don't have the specified attribute, or do have the specified attribute but not with a certain value.
		[TestMethod]
		public void FirstLevelAttributeKeyAndValueNotEqualSelect()
		{
			var selectedNodes = domTree.SelectNodes("[lang!='gb']"); //44 nodes?
			using (var writer = new StreamWriter(string.Format("{0}/{1}", Directory.GetCurrentDirectory(), "test.txt")))
			{
				foreach (var selectedNode in selectedNodes)
				{
					writer.WriteLine("Name:" + selectedNode.TagName + " Text:" + selectedNode.Text + " Attributes:" + string.Join(",", selectedNode.Attributes.Select(attribute => attribute.Key + ":" + attribute.Value)));
				}
			}
			Assert.AreEqual(1, selectedNodes.Count());
		}

		//[TestMethod]
		//public void SecondLevelAttributeKeyAndValueNotEqualSelect()
		//{
		//    var selectedNodes = domTree.SelectNodes("[data-type!='testattributematch']");
		//    Assert.AreEqual(0, selectedNodes.Count());
		//}

		//[TestMethod]
		//public void ThirdLevelAttributeKeyAndValueNotEqualSelect()
		//{
		//    var selectedNodes = domTree.SelectNodes("[target!='_blank']");
		//    Assert.AreEqual(0, selectedNodes.Count());
		//}

		////[TestMethod]
		////public void MultipleAttributeKeyAndValueNotEqualInOneNodeSelect()
		////{
		////    var selectedNodes = domTree.SelectNodes("[rel!='stylesheet']");
		////    Assert.AreEqual(2, selectedNodes.Count());
		////}

		//[TestMethod]
		//public void MultipleAttributeKeyAndValueNotEqualInMultipleNodesSelect()
		//{
		//    var selectedNodes = domTree.SelectNodes("[type!='text/javascript']");
		//    Assert.AreEqual(8, selectedNodes.Count());
		//}

		//[TestMethod]
		//public void MultipleAttributeKeyAndValueNotEqualInMultipleNodesAtMultipleDepthsSelect()
		//{
		//    var selectedNodes = domTree.SelectNodes("[type!='testattributematch']");
		//    Assert.AreEqual(10, selectedNodes.Count());
		//}
		#endregion

		#region AttributeEndsWithTests "[key$='value']"
		[TestMethod]
		public void FirstLevelAttributeKeyAndValueEndsWithSelect()
		{
			var selectedNodes = domTree.SelectNodes("[xmlns$='html']");
			Assert.AreEqual(1, selectedNodes.Count());
		}

		[TestMethod]
		public void SecondLevelAttributeKeyAndValueEndsWithSelect()
		{
			var selectedNodes = domTree.SelectNodes("[data-type$='match']");
			Assert.AreEqual(1, selectedNodes.Count());
		}

		[TestMethod]
		public void ThirdLevelAttributeKeyAndValueEndsWithSelect()
		{
			var selectedNodes = domTree.SelectNodes("[type$='\\css']");
			Assert.AreEqual(3, selectedNodes.Count());
		}

		//[TestMethod]
		//public void MultipleAttributeKeyAndValueEndsWithInOneNodeSelect()
		//{
		//    var selectedNodes = domTree.SelectNodes("[rel$='sheet']");
		//    Assert.AreEqual(2, selectedNodes.Count());
		//}

		[TestMethod]
		public void MultipleAttributeKeyAndValueEndsWithInMultipleNodesSelect()
		{
			var selectedNodes = domTree.SelectNodes("[type$='/javascript']");
			Assert.AreEqual(5, selectedNodes.Count());
		}

		[TestMethod]
		public void MultipleAttributeKeyAndValueEndsWithInMultipleNodesAtMultipleDepthsSelect()
		{
			var selectedNodes = domTree.SelectNodes("[type$='match']");
			Assert.AreEqual(3, selectedNodes.Count());
		}
		#endregion

		#region AttributeStartsWithTests "[key^='value']"
		[TestMethod]
		public void FirstLevelAttributeKeyAndValueStartsWithSelect()
		{
			var selectedNodes = domTree.SelectNodes("[xmlns^='http']");
			Assert.AreEqual(1, selectedNodes.Count());
		}

		[TestMethod]
		public void SecondLevelAttributeKeyAndValueStartsWithSelect()
		{
			var selectedNodes = domTree.SelectNodes("[data-type^='test']");
			Assert.AreEqual(1, selectedNodes.Count());
		}

		[TestMethod]
		public void ThirdLevelAttributeKeyAndValueStartsWithSelect()
		{
			var selectedNodes = domTree.SelectNodes("[rel^=''']");
			Assert.AreEqual(1, selectedNodes.Count());
		}

		//[TestMethod]
		//public void MultipleAttributeKeyAndValueStartsWithInOneNodeSelect()
		//{
		//    var selectedNodes = domTree.SelectNodes("[rel^='sheet']");
		//    Assert.AreEqual(2, selectedNodes.Count());
		//}

		[TestMethod]
		public void MultipleAttributeKeyAndValueStartsWithInMultipleNodesSelect()
		{
			var selectedNodes = domTree.SelectNodes("[type^='text']");
			Assert.AreEqual(8, selectedNodes.Count());
		}

		[TestMethod]
		public void MultipleAttributeKeyAndValueStartsWithInMultipleNodesAtMultipleDepthsSelect()
		{
			var selectedNodes = domTree.SelectNodes("[type^='test']");
			Assert.AreEqual(4, selectedNodes.Count());
		}
		#endregion

		#region AttributeContainsTests "[key*='value']"
		[TestMethod]
		public void FirstLevelAttributeKeyAndValueContainsSelect()
		{
			var selectedNodes = domTree.SelectNodes("[xmlns*='.org']");
			Assert.AreEqual(1, selectedNodes.Count());
		}

		[TestMethod]
		public void SecondLevelAttributeKeyAndValueContainsSelect()
		{
			var selectedNodes = domTree.SelectNodes("[data-type*='attribute']");
			Assert.AreEqual(1, selectedNodes.Count());
		}

		[TestMethod]
		public void ThirdLevelAttributeKeyAndValueContainsSelect()
		{
			var selectedNodes = domTree.SelectNodes("[href*='static/borrow']");
			Assert.AreEqual(2, selectedNodes.Count());
		}

		//[TestMethod]
		//public void MultipleAttributeKeyAndValueContainsInOneNodeSelect()
		//{
		//    var selectedNodes = domTree.SelectNodes("[rel*='sheet']");
		//    Assert.AreEqual(2, selectedNodes.Count());
		//}

		[TestMethod]
		public void MultipleAttributeKeyAndValueContainsInMultipleNodesSelect()
		{
			var selectedNodes = domTree.SelectNodes("[href*='sheets']");
			Assert.AreEqual(2, selectedNodes.Count());
		}

		[TestMethod]
		public void MultipleAttributeKeyAndValueContainsInMultipleNodesAtMultipleDepthsSelect()
		{
			var selectedNodes = domTree.SelectNodes("[type*='soft']");
			Assert.AreEqual(2, selectedNodes.Count());
		}
		#endregion

		#region Attribute<>Tests "[key~='value']"
		[TestMethod]
		public void FirstLevelAttributeKeyAndValueContainsExactWordSelect()
		{
			var selectedNodes = domTree.SelectNodes("[lang~='en']");
			Assert.AreEqual(1, selectedNodes.Count());
		}

		[TestMethod]
		public void SecondLevelAttributeKeyAndValueContainsExactWordSelect()
		{
			var selectedNodes = domTree.SelectNodes("[data-type~='testattributematch']");
			Assert.AreEqual(1, selectedNodes.Count());
		}

		[TestMethod]
		public void ThirdLevelAttributeKeyAndValueContainsExactWordSelect()
		{
			var selectedNodes = domTree.SelectNodes("[id~='qunit']");
			Assert.AreEqual(1, selectedNodes.Count());
		}

		[TestMethod]
		public void MultipleAttributeKeyAndValueContainsExactWordInOneNodeSelect()
		{
			var selectedNodes = domTree.SelectNodes("[class~='unique']");
			Assert.AreEqual(1, selectedNodes.Count());
		}

		[TestMethod]
		public void MultipleAttributeKeyAndValueContainsExactWordInMultipleNodesSelect()
		{
			var selectedNodes = domTree.SelectNodes("[type~='text/javascript']");
			Assert.AreEqual(5, selectedNodes.Count());
		}

		[TestMethod]
		public void MultipleAttributeKeyAndValueContainsExactWordInMultipleNodesAtMultipleDepthsSelect()
		{
			var selectedNodes = domTree.SelectNodes("[type='testattributematch']");
			Assert.AreEqual(3, selectedNodes.Count());
		}
		#endregion

		#region to be deleted
		[TestMethod]
		public void TestMethod1()
		{
			//Expression<Func<IDictionary<string, IEnumerable<string>>, int>> test = attribute => attribute.Count;
			//Expression<Func<KeyValuePair<string, IEnumerable<string>>, int>> asdf = attribute => attribute.;
			//Expression<Func<IEnumerable<string>, int>> lfksjd = att => att.Count();
			//var eParmkeyValueParm2 = Expression.Parameter(typeof(KeyValuePair<string, IEnumerable<string>>));

			var eParm = Expression.Parameter(typeof(HtmlElement));
			var a = eParm.CreateAnyExpression();

			var elem = new HtmlElement
			{
				Descendants = new List<HtmlElement>
                                        {
                                            new HtmlElement
                                            {
                                                Attributes = new Dictionary<string, IEnumerable<string>>
                                                            {
                                                                { "id", new[] { "ID" } },
                                                                { "class", new[] { "CLASS" } }
                                                            }
                                            }
                                        }
			};

			var elem2 = new HtmlElement
			{
				Descendants = new List<HtmlElement>
                                        {
                                            new HtmlElement
                                            {
                                                Attributes = new Dictionary<string, IEnumerable<string>>()
                                            }
                                        }
			};

			var elem3 = new HtmlElement
			{
				Descendants = new List<HtmlElement>
                                        {
                                            new HtmlElement()
                                        }
			};

			var elem4 = new HtmlElement
			{
				Descendants = new List<HtmlElement>()
			};

			var elem5 = new HtmlElement
			{
				Descendants = new List<HtmlElement>
                                        {
                                            new HtmlElement
                                            {
                                                Attributes = new Dictionary<string, IEnumerable<string>>
                                                            {
                                                                { "id", new[] { "class" } },
                                                                { "class", new[] { "id" } }
                                                            }
                                            }
                                        }
			};

			var keyValueParm = Expression.Parameter(typeof(KeyValuePair<string, IEnumerable<string>>));

			var keyCompareExpression = keyValueParm.CreateStringPropertyCompareExpression(
					"class",
					"Key",
					PredicateBuilder.StringTransform.ToLowerInvariant,
					PredicateBuilder.StringComparison.Equals);

			var valueParm = Expression.Parameter(typeof(string));

			var valueCompareExpression = keyValueParm.CreateStringPropertyCompareExpression(
					"class",
					valueParm,
					PredicateBuilder.StringTransform.ToLowerInvariant,
					PredicateBuilder.StringComparison.Equals);

			var valuesCompareExpression = keyValueParm.CreateKeyValueBoolExpression<string>(
					valueParm,
					valueCompareExpression,
					PredicateBuilder.EnumerableMethodName.Any);

			var attributesCompareExpression = Expression.AndAlso(keyCompareExpression, valuesCompareExpression);

			var parm = Expression.Parameter(typeof(HtmlElement));

			var attributesAnyExpression = parm.CreateKeyValueAnyExpression(
					PredicateBuilder.EnumerableMethodName.Any,
					"Attributes",
					attributesCompareExpression,
					keyValueParm);

			var ee = parm.CreateElementsWhereExpression<HtmlElement>(
					PredicateBuilder.EnumerableMethodName.Where,
					"Descendants",
					attributesAnyExpression);

			var l = Expression.Lambda<Func<HtmlElement, IEnumerable<HtmlElement>>>(ee, new[] { parm }).Compile(DebugInfoGenerator.CreatePdbGenerator());

			Assert.IsTrue(l(elem).Any());
			Assert.IsFalse(l(elem2).Any());
			//Assert.IsFalse(l(elem3).Any());
			//Assert.IsFalse(l(elem4).Any());
			Assert.IsFalse(l(elem5).Any());
		}

		[TestMethod]
		public void TestMethod2()
		{
			//var elem = new HtmlElement
			//{
			//    Descendants = new List<HtmlElement>
			//                            {
			//                                new HtmlElement
			//                                {
			//                                    TagName = "div",
			//                                    Attributes = new Dictionary<string, IEnumerable<string>>
			//                                                {
			//                                                    { "id", new[] { "class" } },
			//                                                    { "class", new[] { "id" } }
			//                                                }
			//                                },
			//                                new HtmlElement
			//                                {
			//                                    TagName = "p"//,
			//                                    //Attributes = new Dictionary<string, IEnumerable<string>>()
			//                                },
			//                                new HtmlElement
			//                                {
			//                                    TagName = "Zack"//,
			//                                    //Attributes = new Dictionary<string, IEnumerable<string>>()
			//                                }
			//                            }
			//};
			var elem = new HtmlElement
			{

			};

			//var elementParm = Expression.Parameter(typeof(HtmlElement));
			//var keyValueParm = Expression.Parameter(typeof(KeyValuePair<string, IEnumerable<string>>));
			//var valueParm = Expression.Parameter(typeof(string));
			//var d = ParseHtmlQuery(elementParm, keyValueParm, valueParm, "div");
			//var e = ParseHtmlQuery(elementParm, keyValueParm, valueParm, "#id");
			//var f = ParseHtmlQuery(elementParm, keyValueParm, valueParm, ".class1,.id");
			//var g = ParseHtmlQuery(elementParm, keyValueParm, valueParm, ".class1, .class2");
			//var h = ParseHtmlQuery(elementParm, keyValueParm, valueParm, "div p");
			//var i = f.First().ToString();
			//Expression<Func<KeyValuePair<string, IEnumerable<string>>, bool>> test = attribute => attribute.Key.ToLowerInvariant().Equals("class".ToLowerInvariant()) && attribute.Value.Any(value => value.ToLowerInvariant().Equals("class1".ToLowerInvariant()) || value.ToLowerInvariant().Equals("class2".ToLowerInvariant()));
			//Assert.AreNotEqual(f.First().ToString(), g.First().ToString());

			//var expressions = CreateHtmlFuncs(new[] { f.First(), g.First() }, elementParm, keyValueParm).ToList();
			//Assert.AreEqual(expressions.First()(elem).Count(), 1);
			//Assert.AreEqual(expressions[1](elem).Count(), 0);
		}
		#endregion
	}
}
