using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WikEpubLib.CreateDocs
{
    public class TocDoc : AbstractXDoc
    {
        public TocDoc(XDocument document, string directory) : base(document, directory)
        {
        }

        public async override Task SaveAsync()
        {
            await SaveTaskAsync(_document, _directory, "toc.ncx");
        }
    }
}
