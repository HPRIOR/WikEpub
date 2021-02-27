using System.Collections.Generic;
using System.Threading.Tasks;
using WikEpubLib.Records;

namespace WikEpubLib.Interfaces
{
    public interface ITocDocCreator
    {
        Task<IDocument> CreateTocDoc(IEnumerable<WikiPageRecord> pageRecords, string bookTitle, string filePath);
    }
}