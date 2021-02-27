﻿using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using WikEpubLib.CreateDocs;
using WikEpubLib.Enums;
using WikEpubLib.Records;

namespace WikEpubLib.Interfaces
{
    public interface ITocDocCreator
    {
        Task<IDocument> CreateTocDoc(IEnumerable<WikiPageRecord> pageRecords, string bookTitle, string filePath);
    }
}