using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WikEpubLib.Enums;
using WikEpubLib.Interfaces;
using WikEpubLib.Records;

namespace WikEpubLib.CreateDocs
{
    /// <summary>
    /// Wrapper class which gets all the xml files for the epub format (content, contianer, toc).
    /// </summary>
    public class DocumentCreator : IDocumentCreator
    {
        private readonly ITocDocCreator _tocDocCreator;
        private readonly IContentDocCreator _contentDocCreator;
        private readonly IContainerDocCreator _containerDocCreator;
        private readonly IEpubHtmlCreator _epubHtmlCreator;

        public DocumentCreator(
            ITocDocCreator tocDocCreator,
            IContentDocCreator contentDocCreator,
            IContainerDocCreator containerDocCreator,
            IEpubHtmlCreator epubHtmlCreator
            )
        {
            _tocDocCreator = tocDocCreator;
            _contentDocCreator = contentDocCreator;
            _containerDocCreator = containerDocCreator;
            _epubHtmlCreator = epubHtmlCreator;
        }

        /// <summary>
        /// Returns an IEnumerable of IDocuments which can be saved in specified directories
        /// </summary>
        /// <param name="wikiRecords"></param>
        /// <param name="bookTitle"></param>
        /// <returns></returns>
        public IEnumerable<Task<IDocument>> From(
            IEnumerable<WikiPageRecord> wikiRecords,
            string bookTitle,
            IDictionary<Directories, string> directories
            )
        {
            return new List<Task<IDocument>>()
            {
                _containerDocCreator.CreateContainerDoc(directories[Directories.METAINF]),
                _contentDocCreator.CreateContentDoc(wikiRecords, bookTitle, directories[Directories.OEBPS]),
                _tocDocCreator.CreateTocDoc(wikiRecords, bookTitle, directories[Directories.OEBPS]),
            }.Concat(wikiRecords.Select(record => _epubHtmlCreator.CreateHtmlDoc(record, directories[Directories.OEBPS])));
        }
    }
}