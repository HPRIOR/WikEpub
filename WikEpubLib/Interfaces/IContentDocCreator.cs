using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using WikEpubLib.CreateDocs;
using WikEpubLib.Enums;
using WikEpubLib.Records;

namespace WikEpubLib.Interfaces
{
    public interface IContentDocCreator
    {
        Task<IDocument> CreateContentDoc(IEnumerable<WikiPageRecord> wikiPageRecords, string bookTitle, string filePath);
    }
}