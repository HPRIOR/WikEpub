using HtmlAgilityPack;
using System.Collections.Generic;

namespace WikEpubLib.Records
{
    public record WikiPageRecord
    {
        public HtmlDocument htmlDoc;
        public string Id;
        public Dictionary<string, string> SrcMap;
        public List<(string id, string sectionName)> SectionHeadings;
    }
}