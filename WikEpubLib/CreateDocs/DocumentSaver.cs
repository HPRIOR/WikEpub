using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WikEpubLib.Interfaces;

namespace WikEpubLib.CreateDocs
{
    public static class DocumentSaver
    {
        public static Task Save(IEnumerable<IDocument> documents)
        {
            return Task.WhenAll(documents.Select(document => document.SaveAsync()));
        }
    }
}