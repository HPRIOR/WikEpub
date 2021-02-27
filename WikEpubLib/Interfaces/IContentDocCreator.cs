using System.Collections.Generic;
using System.Threading.Tasks;
using WikEpubLib.Records;

namespace WikEpubLib.Interfaces
{
    public interface IContentDocCreator
    {
        Task<IDocument> CreateContentDoc(IEnumerable<WikiPageRecord> wikiPageRecords, string bookTitle, string filePath);
    }
}