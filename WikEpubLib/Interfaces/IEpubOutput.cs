﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using WikEpubLib.Enums;
using WikEpubLib.Records;

namespace WikEpubLib
{
    public interface IEpubOutput
    {
        Task CreateDirectoriesAsync(Dictionary<Directories, string> directories);

        Task SaveDocumentsAsync(Dictionary<Directories, string> directories, IEnumerable<(XmlType type, XDocument doc)> xmlDocs);

        Task SaveDocumentsAsync(Dictionary<Directories, string> directories, IEnumerable<WikiPageRecord> updatedWikiRecords);

        IEnumerable<Task> DownloadImages(WikiPageRecord pageRecord, Dictionary<Directories, string> directories);

        Task ZipFiles(Dictionary<Directories, string> directories, Guid guid);

        Task CreateMimeFile(Dictionary<Directories, string> directories);
    }
}