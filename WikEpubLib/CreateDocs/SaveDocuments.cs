﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikEpubLib.Interfaces;

namespace WikEpubLib.CreateDocs
{
    public class SaveDocuments : ISaveDocuments
    {
        public Task Save(IEnumerable<IDocument> documents)
        {
            return Task.WhenAll(documents.Select(document => document.SaveAsync()));
        }
    }
}