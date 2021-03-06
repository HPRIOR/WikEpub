﻿using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;
using WikEpubLib.Exceptions;
using WikEpubLib.IO;

namespace WikEpubLibTests
{
    [TestClass]
    public class HtmlInputTest
    {
        private HtmlWeb htmlWeb;
        private HtmlInput htmlInput;

        [TestInitialize]
        public void Init()
        {
            htmlInput = new HtmlInput();
            htmlWeb = new HtmlWeb();
        }

        [TestMethod]
        public async Task Valid_Url_Does_Not_Throw_Exception()
        {
            List<string> validUrls = new()
            {
                "wikipedia.org/wiki/Thomas_Kuhn",
                "en.wikipedia.org/wiki/Thomas_Kuhn",
                "https://en.wikipedia.org/wiki/Thomas_Kuhn"
            };
            await htmlInput.GetHtmlDocumentsFromAsync(validUrls, htmlWeb);
        }

        [TestMethod]
        public async Task Url_Throws_Exception_No_Title()
        {
            List<string> validUrls = new()
            {
                "https://en.wikipedia.org/wiki/"
            };
            await Assert.ThrowsExceptionAsync<InvalidWikiUrlException>(
                () => htmlInput.GetHtmlDocumentsFromAsync(validUrls, htmlWeb)
                );
        }

        [TestMethod]
        public async Task Empty_Url_Throws()
        {
            List<string> validUrls = new()
            {
                ""
            };
            await Assert.ThrowsExceptionAsync<InvalidWikiUrlException>(
                () => htmlInput.GetHtmlDocumentsFromAsync(validUrls, htmlWeb)
                );
        }
    }
}