using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WikEpubLib.CreateDocs;
using WikEpubLib.Enums;
using WikEpubLib.Interfaces;

namespace WikEpubLib
{
    /// <summary>
    /// Main class of the library. It coordinates the various classes of the library and creates an epub
    /// file in a specified directory.
    /// </summary>
    public class GetEpub : IHtmlsToEpub
    {
        private readonly IHtmlInput _htmlInput;
        private readonly IGetWikiPageRecords _getRecords;
        private readonly IDocumentCreator _getEpubDocs;
        private readonly IEpubOutput _epubOutput;

        public GetEpub(
            IGetWikiPageRecords getRecords,
            IDocumentCreator getEpubDocs, 
            IHtmlInput htmlInput, 
            IEpubOutput epubOutput)
        {
            _getRecords = getRecords;
            _getEpubDocs = getEpubDocs;
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
        /// </remarks>
        /// <param name="urls"></param>
        /// <param name="rootDirectory">Path in which the book will be created</param>
        /// <param name="bookTitle">Name of the book</param>
        /// <param name="guid">Unique identifier for the folder</param>
        /// <returns></returns>
        public async Task FromAsync(IEnumerable<string> urls, string rootDirectory, string bookTitle, Guid guid)
        {
            var htmlDocs = _htmlInput.GetHtmlDocumentsFromAsync(urls, new HtmlWeb());

            var directoryPaths = GetDirectoryContext(rootDirectory, guid);
            var createDirectories = _epubOutput.CreateDirectoriesAsync(directoryPaths);

            var wikiPageRecords =
               (await htmlDocs).Select(doc => _getRecords.From(doc, "image_repo")).ToList();
            var downloadImages =
                Task.WhenAll(wikiPageRecords.SelectMany(record => _epubOutput.DownloadImages(record, directoryPaths)));
            
            var createEpubDocuments = Task.WhenAll(_getEpubDocs.From(wikiPageRecords, bookTitle, directoryPaths));

            // Save the produced files to relevant directory and compress them
            await createDirectories;
            var saveDocuments = DocumentSaver.Save(await createEpubDocuments);
            var createMimeType = _epubOutput.CreateMimeFile(directoryPaths);
            Task.WaitAll(saveDocuments, createMimeType, downloadImages);
            await _epubOutput.ZipFiles(directoryPaths, guid);
        }

        private Dictionary<Directories, string> GetDirectoryContext(string rootDir, Guid folderId) => 
            new Dictionary<Directories, string> {
            {Directories.ROOT, rootDir},
            {Directories.OEBPS, @$"{rootDir}\{folderId}\OEBPS" },
            {Directories.METAINF, @$"{rootDir}\{folderId}\META-INF" },
            {Directories.BOOKDIR,  @$"{rootDir}\{folderId}" },
            {Directories.IMAGES, @$"{rootDir}\{folderId}\OEBPS\image_repo" }
        };
    }
}