﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LM.RichComments.Domain;
using RichCommentsTests.Properties;
using RichCommentsTests.DataSchema;

namespace RichCommentsTests.Integration
{
    [TestClass]
    public class ParserTests
    {
        private TestContext _testContext;
        private string _contentTypeName;
        ImageItemParser _imageItemParser;

        public TestContext TestContext
        {
            get { return _testContext; }
            set { _testContext = value; }
        }

        [TestInitialize]
        public void Initialize()
        {
            _contentTypeName = "CSharp";
            _imageItemParser = new ImageItemParser();
        }

        [DataSource("System.Data.SqlServerCe.3.5", "data source=|DataDirectory|\\TestData.sdf", TestData.ParserData.NAME, DataAccessMethod.Sequential), DeploymentItem("RichCommentsTests\\TestData.sdf"), TestMethod]
        public void ImageItemParserTest()
        {
            // ARRANGE
            string inputLineText = _testContext.DataRow[TestData.ParserData.LineText].ToString();

            bool expectedToParse = bool.Parse(_testContext.DataRow[TestData.ParserData.ShouldBeParsable].ToString());

            ImageItem.Parameters expectedParameters = new ImageItem.Parameters(
                _testContext.DataRow[TestData.ParserData.ExpectedParameter1].ToString(),
                Convert.ToDouble(_testContext.DataRow[TestData.ParserData.ExpectedParameter2]));
            
            int expectedXmlStartPosition = Convert.ToInt32(_testContext.DataRow[TestData.ParserData.ExpectedXmlPosition]);

            string expectedExceptionMessage = _testContext.DataRow[TestData.ParserData.ExpectedException].ToString();

            IRichCommentItemParameters parsedParameters;
            Exception parseException;
            int? xmlStartPosition;

            // ACT
            bool parsedSuccessfully = _imageItemParser.TryParse(_contentTypeName, inputLineText, out parsedParameters, out parseException, out xmlStartPosition);

            var actualImageItemParameters = (ImageItem.Parameters)parsedParameters;

            // ASSERT
            Assert.AreEqual(expectedToParse, parsedSuccessfully);
            Assert.AreEqual(expectedParameters.Url, actualImageItemParameters.Url);
            Assert.AreEqual(expectedParameters.Scale, actualImageItemParameters.Scale);
            Assert.AreEqual(expectedExceptionMessage, (parseException ?? new Exception("")).Message);
            Assert.AreEqual(expectedXmlStartPosition, xmlStartPosition);
        }
    }
}
