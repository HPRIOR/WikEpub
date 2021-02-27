using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikEpubLib.Records;

namespace WikEpubLib.CreateDocs
{
    public class HtmlDoc : AbstractHtmlDoc
    {
        private WikiPageRecord _wikiRecord;
        public HtmlDoc(HtmlDocument document, string directory, WikiPageRecord wikiRecord) : base(document, directory)
        {
        }

        public async override Task SaveAsync()
        {
            await SaveTaskAsync(_document, _directory, $"{_wikiRecord.Id}.html");
        }
    }
}
