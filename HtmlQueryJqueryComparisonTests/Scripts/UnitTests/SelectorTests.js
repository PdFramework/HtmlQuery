/// <reference path="../jquery-2.0.3.js"/>

//#region TagNameSelectTests
//public void FirstLevelTagNameSelect() { Assert.AreEqual(0, domTree.SelectNodes("dom").Count()); }
test("FirstLevelTagNameSelect.js", function () {
    var selectedNodes = $('dom');
    equal(selectedNodes.length, 0);
});

//public void SecondLevelTagNameSelect() { Assert.AreEqual(1, domTree.SelectNodes("HTML").Count()); }
test("SecondLevelTagNameSelect.js", function () {
    var selectedNodes = $('HTML');
    equal(selectedNodes.length, 1);
});

//public void ThirdLevelTagNameSelect() { Assert.AreEqual(1, domTree.SelectNodes("head").Count()); }
test("ThirdLevelTagNameSelect.js", function () {
    var selectedNodes = $('head');
    equal(selectedNodes.length, 1);
});

//public void MultipleTagNameMatchesInOneNodeSelect() { Assert.AreEqual(5, domTree.SelectNodes("link").Count()); }
test("MultipleTagNameMatchesInOneNodeSelect.js", function () {
    var selectedNodes = $('link');
    equal(selectedNodes.length, 5);
});

//public void MultipleTagNameMatchesInMultipleNodesSelect() { Assert.AreEqual(7, domTree.SelectNodes("script").Count()); }
test("MultipleTagNameMatchesInMultipleNodesSelect.js", function () {
    var selectedNodes = $('script');
    equal(selectedNodes.length, 7);
});

//public void MultipleTagNameMatchesInMultipleNodesAtMultipleDepthsSelect() { Assert.AreEqual(11, domTree.SelectNodes("div").Count()); }
test("MultipleTagNameMatchesInOneNodeSelectMultipleTagNameMatchesInMultipleNodesAtMultipleDepthsSelect.js", function () {
    var selectedNodes = $('div').not('#qunit-testrunner-toolbar'); // this div gets added after tests run
    equal(selectedNodes.length, 11);
});
//#endregion TagNameSelectTests

//#region AttributeKeyEqualTests
//public void FirstLevelAttributeKeySelect() { Assert.AreEqual(1, domTree.SelectNodes("[html]").Count()); }
test("FirstLevelAttributeKeySelect.js", function () {
    var selectedNodes = $('[xmlns]');
    equal(selectedNodes.length, 1);
});

//public void SecondLevelAttributeKeySelect() { Assert.AreEqual(1, domTree.SelectNodes("[data-type]").Count()); }
test("SecondLevelAttributeKeySelect.js", function () {
    var selectedNodes = $('[data-type]');
    equal(selectedNodes.length, 1);
});

//public void ThirdLevelAttributeKeySelect() { Assert.AreEqual(1, domTree.SelectNodes("[http-equiv]").Count()); }
test("ThirdLevelAttributeKeySelect.js", function () {
    var selectedNodes = $('[http-equiv]');
    equal(selectedNodes.length, 1);
});

//public void MultipleAttributeKeyMatchesInOneNodeSelect() { Assert.AreEqual(5, domTree.SelectNodes("[rel]").Count()); }
test("MultipleAttributeKeyMatchesInOneNodeSelect.js", function () {
    var selectedNodes = $('[rel]');
    equal(selectedNodes.length, 5);
});

//public void MultipleAttributeKeyMatchesInMultipleNodesSelect() { Assert.AreEqual(6, domTree.SelectNodes("[src]").Count()); }
test("MultipleAttributeKeyMatchesInMultipleNodesSelect.js", function () {
    var selectedNodes = $('[src]');
    equal(selectedNodes.length, 6);
});

//public void MultipleAttributeKeyMatchesInMultipleNodesAtMultipleDepthsSelect() { Assert.AreEqual(9, domTree.SelectNodes("[href]").Count()); }
test("MultipleAttributeKeyMatchesInMultipleNodesAtMultipleDepthsSelect.js", function () {
    var selectedNodes = $('[href]').not('[href] #qunit');
    var nodesToExclude = $('#qunit a'); // these are <a href=''></a> elements that get added by qunit while running the tests
    equal(selectedNodes.length - nodesToExclude.length, 9);
});
//#endregion

//#region AttributeValueEqualTests "[key='value']"
//public void FirstLevelAttributeKeyValueSelect() { Assert.AreEqual(1, domTree.SelectNodes("[lang='en']").Count()); }
test("FirstLevelAttributeKeyValueSelect.js", function () {
    var selectedNodes = $('[lang="en"]');
    equal(selectedNodes.length, 1);
});

//public void SecondLevelAttributeKeyValueSelect() { Assert.AreEqual(1, domTree.SelectNodes("[data-type='testattributematch']").Count()); }
test("SecondLevelAttributeKeyValueSelect.js", function () {
    var selectedNodes = $('[data-type="testattributematch"]');
    equal(selectedNodes.length, 1);
});

//public void ThirdLevelAttributeKeyValueSelect() { Assert.AreEqual(1, domTree.SelectNodes("[target='_blank']").Count()); }
test("ThirdLevelAttributeKeyValueSelect.js", function () {
    var selectedNodes = $('[target="_blank"]');
    equal(selectedNodes.length, 1);
});

//public void MultipleAttributeKeyValueMatchesInMultipleNodesSelect() { Assert.AreEqual(5, domTree.SelectNodes("[type='text/javascript']").Count()); }
test("MultipleAttributeKeyValueMatchesInMultipleNodesSelect.js", function () {
    var selectedNodes = $('[type="text/javascript"]');
    equal(selectedNodes.length, 5);
});

//public void MultipleAttributeKeyValueMatchesInMultipleNodesAtMultipleDepthsSelect() { Assert.AreEqual(3, domTree.SelectNodes("[type='testattributematch']").Count()); }
test("MultipleAttributeKeyValueMatchesInMultipleNodesAtMultipleDepthsSelect.js", function () {
    var selectedNodes = $('[type="testattributematch"]');
    equal(selectedNodes.length, 3);
});
//#endregion

//#region AttributeNotEqualTests "[key!='value']"
// TODO: rework selector in accordance with jquery spec: Select elements that either don't have the specified attribute, or do have the specified attribute but not with a certain value.
//public void FirstLevelAttributeKeyAndValueNotEqualSelect() { Assert.AreEqual(1, domTree.SelectNodes("[lang!='gb']").Count()); }
test("FirstLevelAttributeKeyAndValueNotEqualSelect.js", function () {
    var selectedNodes = $('[lang!="gb"]');
    var nodesToExclude = $('#qunit *');
    equal(selectedNodes.length - nodesToExclude.length, 1);
});

//public void SecondLevelAttributeKeyAndValueNotEqualSelect() { Assert.AreEqual(0, domTree.SelectNodes("[data-type!='testattributematch']").Count()); }
test("SecondLevelAttributeKeyAndValueNotEqualSelect.js", function () {
    var selectedNodes = $('[data-type!="testattributematch"]');
    equal(selectedNodes.length, 0);
});

//public void ThirdLevelAttributeKeyAndValueNotEqualSelect() { Assert.AreEqual(0, domTree.SelectNodes("[target!='_blank']").Count()); }
test("ThirdLevelAttributeKeyAndValueNotEqualSelect.js", function () {
    var selectedNodes = $('[target!="_blank"]');
    equal(selectedNodes.length, 0);
});

//public void MultipleAttributeKeyAndValueNotEqualInMultipleNodesSelect() { Assert.AreEqual(8, domTree.SelectNodes("[type!='text/javascript']").Count()); }
test("MultipleAttributeKeyAndValueNotEqualInMultipleNodesSelect.js", function () {
    var selectedNodes = $('[type!="text/javascript"]');
    equal(selectedNodes.length, 8);
});

//public void MultipleAttributeKeyAndValueNotEqualInMultipleNodesAtMultipleDepthsSelect() { Assert.AreEqual(10, domTree.SelectNodes("[type!='testattributematch']").Count()); }
test("MultipleAttributeKeyAndValueNotEqualInMultipleNodesAtMultipleDepthsSelect.js", function () {
    var selectedNodes = $('[type!="testattributematch"]');
    equal(selectedNodes.length, 3);
});
//#endregion

//#region AttributeEndsWithTests "[key$='value']"
//public void FirstLevelAttributeKeyAndValueEndsWithSelect() { Assert.AreEqual(1, domTree.SelectNodes("[xmlns$='html']").Count()); }
test("FirstLevelAttributeKeyAndValueEndsWithSelect.js", function () {
    var selectedNodes = $('[xmlns$="html"]');
    equal(selectedNodes.length, 1);
});

//public void SecondLevelAttributeKeyAndValueEndsWithSelect() { Assert.AreEqual(1, domTree.SelectNodes("[data-type$='match']").Count()); }
test("SecondLevelAttributeKeyAndValueEndsWithSelect.js", function () {
    var selectedNodes = $('[data-type$="match"]');
    equal(selectedNodes.length, 1);
});

//public void ThirdLevelAttributeKeyAndValueEndsWithSelect() { Assert.AreEqual(3, domTree.SelectNodes("[type$='\\css']").Count()); }
test("ThirdLevelAttributeKeyAndValueEndsWithSelect.js", function () {
    var selectedNodes = $('[type$="/css"]');
    equal(selectedNodes.length, 3);
});

//public void MultipleAttributeKeyAndValueEndsWithInMultipleNodesSelect() { Assert.AreEqual(5, domTree.SelectNodes("[type$='/javascript']").Count()); }
test("MultipleAttributeKeyAndValueEndsWithInMultipleNodesSelect.js", function () {
    var selectedNodes = $('[type$="/javascript"]');
    equal(selectedNodes.length, 5);
});

//public void MultipleAttributeKeyAndValueEndsWithInMultipleNodesAtMultipleDepthsSelect() { Assert.AreEqual(3, domTree.SelectNodes("[type$='match']").Count()); }
test("MultipleAttributeKeyAndValueEndsWithInMultipleNodesAtMultipleDepthsSelect.js", function () {
    var selectedNodes = $('[type$="match"]');
    equal(selectedNodes.length, 3);
});
//#endregion

//#region AttributeStartsWithTests "[key^='value']"
//public void FirstLevelAttributeKeyAndValueStartsWithSelect() { Assert.AreEqual(1, domTree.SelectNodes("[xmlns^='http']").Count()); }
test("FirstLevelAttributeKeyAndValueStartsWithSelect.js", function () {
    var selectedNodes = $('[xmlns^="http"]');
    equal(selectedNodes.length, 1);
});

//public void SecondLevelAttributeKeyAndValueStartsWithSelect() { Assert.AreEqual(1, domTree.SelectNodes("[data-type^='test']").Count()); }
test("SecondLevelAttributeKeyAndValueStartsWithSelect.js", function () {
    var selectedNodes = $('[data-type^="test"]');
    equal(selectedNodes.length, 1);
});

//public void ThirdLevelAttributeKeyAndValueStartsWithSelect() { Assert.AreEqual(1, domTree.SelectNodes("[rel^=''']").Count()); }
test("ThirdLevelAttributeKeyAndValueStartsWithSelect.js", function () {
    var selectedNodes = $('[rel^="\'"]');
    equal(selectedNodes.length, 1);
});

//public void MultipleAttributeKeyAndValueStartsWithInMultipleNodesSelect() { Assert.AreEqual(8, domTree.SelectNodes("[type^='text']").Count()); }
test("MultipleAttributeKeyAndValueStartsWithInMultipleNodesSelect.js", function () {
    var selectedNodes = $('[type^="text"]');
    equal(selectedNodes.length, 8);
});

//public void MultipleAttributeKeyAndValueStartsWithInMultipleNodesAtMultipleDepthsSelect() { Assert.AreEqual(3, domTree.SelectNodes("[type^='test']").Count()); }
test("MultipleAttributeKeyAndValueStartsWithInMultipleNodesAtMultipleDepthsSelect.js", function () {
    var selectedNodes = $('[type^="test"]');
    equal(selectedNodes.length, 4);
});
//#endregion

//#region AttributeContainsTests "[key*='value']"
//public void FirstLevelAttributeKeyAndValueContainsSelect() { Assert.AreEqual(1, domTree.SelectNodes("[xmlns*='.org']").Count()); }
test("FirstLevelAttributeKeyAndValueContainsSelect.js", function () {
    var selectedNodes = $('[xmlns*=".org"]');
    equal(selectedNodes.length, 1);
});

//public void SecondLevelAttributeKeyAndValueContainsSelect() { Assert.AreEqual(1, domTree.SelectNodes("[data-type*='attribute']").Count()); }
test("SecondLevelAttributeKeyAndValueContainsSelect.js", function () {
    var selectedNodes = $('[data-type*="attribute"]');
    equal(selectedNodes.length, 1);
});

//public void ThirdLevelAttributeKeyAndValueContainsSelect() { Assert.AreEqual(2, domTree.SelectNodes("[href*='static/borrow']").Count()); }
test("ThirdLevelAttributeKeyAndValueContainsSelect.js", function () {
    var selectedNodes = $('[href*="static/borrow"]');
    equal(selectedNodes.length, 2);
});

//public void MultipleAttributeKeyAndValueContainsInMultipleNodesSelect() { Assert.AreEqual(2, domTree.SelectNodes("[href*='sheets']").Count()); }
test("MultipleAttributeKeyAndValueContainsInMultipleNodesSelect.js", function () {
    var selectedNodes = $('[href*="sheets"]');
    equal(selectedNodes.length, 2);
});

//public void MultipleAttributeKeyAndValueContainsInMultipleNodesAtMultipleDepthsSelect() { Assert.AreEqual(2, domTree.SelectNodes("[type*='soft']").Count()); }
test("MultipleAttributeKeyAndValueContainsInMultipleNodesAtMultipleDepthsSelect.js", function () {
    var selectedNodes = $('[type*="soft"]');
    equal(selectedNodes.length, 2);
});
//#endregion

//#region Attribute<>Tests "[key~='value']"
//public void FirstLevelAttributeKeyAndValueContainsExactWordSelect() { Assert.AreEqual(1, domTree.SelectNodes("[lang~='en']").Count()); }
test("FirstLevelAttributeKeyAndValueContainsExactWordSelect.js", function () {
    var selectedNodes = $('[lang~="en"]');
    equal(selectedNodes.length, 1);
});

//public void SecondLevelAttributeKeyAndValueContainsExactWordSelect() { Assert.AreEqual(1, domTree.SelectNodes("[data-type~='testattributematch']").Count()); }
test("SecondLevelAttributeKeyAndValueContainsExactWordSelect.js", function () {
    var selectedNodes = $('[data-type~="testattributematch"]');
    equal(selectedNodes.length, 1);
});

//public void ThirdLevelAttributeKeyAndValueContainsExactWordSelect() { Assert.AreEqual(1, domTree.SelectNodes("[id~='qunit']").Count()); }
test("ThirdLevelAttributeKeyAndValueContainsExactWordSelect.js", function () {
    var selectedNodes = $('[id~="qunit"]');
    equal(selectedNodes.length, 1);
});

//public void MultipleAttributeKeyAndValueContainsExactWordInOneNodeSelect() { Assert.AreEqual(1, domTree.SelectNodes("[class~='unique']").Count()); }
test("MultipleAttributeKeyAndValueContainsExactWordInOneNodeSelect.js", function () {
    var selectedNodes = $('[class~="unique"]');
    equal(selectedNodes.length, 1);
});

//public void MultipleAttributeKeyAndValueContainsExactWordInMultipleNodesSelect() { Assert.AreEqual(5, domTree.SelectNodes("[type~='text/javascript']").Count()); }
test("MultipleAttributeKeyAndValueContainsExactWordInMultipleNodesSelect.js", function () {
    var selectedNodes = $('[type~="text/javascript"]');
    equal(selectedNodes.length, 5);
});

//public void MultipleAttributeKeyAndValueContainsExactWordInMultipleNodesAtMultipleDepthsSelect() { Assert.AreEqual(3, domTree.SelectNodes("[type='testattributematch']").Count()); }
test("MultipleAttributeKeyAndValueContainsExactWordInMultipleNodesAtMultipleDepthsSelect.js", function () {
    var selectedNodes = $('[type="testattributematch"]');
    equal(selectedNodes.length, 3);
});
//#endregion