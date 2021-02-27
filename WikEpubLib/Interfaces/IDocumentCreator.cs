using System.Collections.Generic;
using System.Threading.Tasks;
using WikEpubLib.Enums;
using WikEpubLib.Interfaces;
using WikEpubLib.Records;

namespace WikEpubLib
{
    public interface IDocumentCreator
    {
        IEnumerable<Task<IDocument>> From(IEnumerable<WikiPageRecord> pageRecords, string bookTitle, IDictionary<Directories, string> directories);
    }
}