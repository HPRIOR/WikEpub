using WikEpubLib.Interfaces; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikEpubLib;
using HtmlAgilityPack;

namespace WikEpubLibTests.GetEpubTest
{
    public class MockHtmlInput : IHtmlInput
    {
        private string _htmlString;

        public MockHtmlInput(string htmlString) 
        {
            _htmlString = htmlString;
        }

        public Task<HtmlDocument[]> GetHtmlDocumentsFromAsync(IEnumerable<string> urls, HtmlWeb htmlWeb)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(_htmlString);
            var htmlDocuments = new HtmlDocument[1];
            htmlDocuments[0] = htmlDoc;
            return Task.Run(() => htmlDocuments);
        }
    }
}
