using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WikEpubLib.Enums;
using WikEpubLib.Interfaces;
using WikEpubLib.Records;

namespace WikEpubLib
{
    /// <summary>
    /// Main class of the library. It coordinates the various classes of the library and creates an epub 
    /// file in a specified directory.
    /// </summary>
    public class GetEpub : IHtmlsToEpub
    {
        private readonly IHtmlInput _htmlInput;
        private readonly IParseHtml _parseHtml;
        private readonly IGetWikiPageRecords _getRecords;
        private readonly IGetXmlDocs _getXmlDocs;
        private readonly IEpubOutput _epubOutput;

        public GetEpub(IParseHtml parseHtml, IGetWikiPageRecords getRecords,
            IGetXmlDocs getXmlDocs, IHtmlInput htmlInput, IEpubOutput epubOutput)
        {
            _parseHtml = parseHtml;
            _getRecords = getRecords;
            _getXmlDocs = getXmlDocs;
            _htmlInput = htmlInput;
            _epubOutput = epubOutput;
        }
        
        /// <summary>
        /// Creates an epub from the given urls. 
        /// </summary>
        /// <remarks>
        /// This method makes use of a record created for each html that is given by a url.
        /// The records contain information from the html needed to create the various files which 
        /// constitute the epub format. An immutable record is created initially so the data collected can be used 
        /// accross various parts of the program. For example, the record contains a mapping (src map) for each image.
        /// This maps the current src of the image to a new local one - this is then used to change the sources of the
        /// html images to a local directory and also to download the images to the relevant local directory.
        ///
        /// A decision was made not to include the the actual html class in the record, and use a tuple instead, 
        /// because only one of the three document producing classes uses it: when parsing the html, the actual html 
        /// is needed along with its associated record - see _parseHtml.ParseAsync(doc, record) below. 
        /// (Possible refactor: include HtmlDocument in record to simplify the calling code)
        /// </remarks>
        /// <param name="urls"></param>
        /// <param name="rootDirectory">Path in which the book will be created</param>
        /// <param name="bookTitle">Name of the book</param>
        /// <param name="guid">Unique identifier for the folder</param>
        /// <returns></returns>
        public async Task FromAsync(IEnumerable<string> urls, string rootDirectory, string bookTitle, Guid guid)
        {
            Task<HtmlDocument[]> initialHtmlDocs = _htmlInput.GetHtmlDocumentsFromAsync(urls, new HtmlWeb());

            var directoryPaths = GetDirectoryContext(rootDirectory, guid);
            Task createDirectories = _epubOutput.CreateDirectoriesAsync(directoryPaths);

            //Associate html document with its records.
            List<(HtmlDocument doc, WikiPageRecord record)> htmlRecordTuple =
               (await initialHtmlDocs).Select(doc => (doc, _getRecords.From(doc, "image_repo"))).ToList();
            var pageRecords = htmlRecordTuple.Select(t => t.record);

            // Use the tuples to produce the files needed to create the epub document: images + xml
            Task downloadImages = Task.WhenAll(pageRecords.SelectMany(record => _epubOutput.DownLoadImages(record, directoryPaths)));
            var xmlDocs = Task.WhenAll(_getXmlDocs.From(pageRecords, bookTitle));
            var parsedDocuments = Task.WhenAll(htmlRecordTuple.Select(t => (_parseHtml.ParseAsync(t.doc, t.record))));

            // Save the produced files to relevant directory and compress them
            await createDirectories;
            Task createMime = _epubOutput.CreateMimeFile(directoryPaths);
            Task saveXml = _epubOutput.SaveDocumentsAsync(directoryPaths, (await xmlDocs));
            Task saveHtml = _epubOutput.SaveDocumentsAsync(directoryPaths, (await parsedDocuments));
            Task.WaitAll(saveXml, saveHtml, createMime, downloadImages);
            await _epubOutput.ZipFiles(directoryPaths, guid);
        }

        private Dictionary<Directories, string> GetDirectoryContext(string rootDir, Guid folderId) => new Dictionary<Directories, string> {
            {Directories.ROOT, rootDir},
            {Directories.OEBPS, @$"{rootDir}\{folderId}\OEBPS" },
            {Directories.METAINF, @$"{rootDir}\{folderId}\META-INF" },
            {Directories.BOOKDIR,  @$"{rootDir}\{folderId}" },
            {Directories.IMAGES, @$"{rootDir}\{folderId}\OEBPS\image_repo" }
        };
    }
}