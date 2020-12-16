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

        public async Task FromAsync(IEnumerable<string> urls, string rootDirectory, string bookTitle, Guid folderID)
        {
            Task<HtmlDocument[]> initialHtmlDocs = _htmlInput.GetHtmlDocumentsFromAsync(urls, new HtmlWeb());

            var directoryPaths = GetDirectoryContext(rootDirectory, folderID);
            Task createDirectories = _epubOutput.CreateDirectoriesAsync(directoryPaths);

            // Associate html document with its records.
            // The records contain information from the html needed to create the various files which 
            // constitute the epub format. A decision was made not to include the the actual html class in the 
            // record, and use a tuple instead, becuase only one of the three document producing classes uses it:
            // see getParsedDocument below. (Possible refactor: include HtmlDocument to simplify the calling code)
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
            await _epubOutput.ZipFiles(directoryPaths, folderID);
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