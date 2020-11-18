﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WikEpubLib.Interfaces;

namespace WikEpubLib
{
    public class GetXmlDocs : IGetXmlDocs
    {
        readonly IGetTocXml _getTocXml;
        readonly IGetContentXml _getContentXml;
        readonly IGetContainerXml _getContainerXml;
        public GetXmlDocs(IGetTocXml getTocXml, IGetContentXml getContentXml, IGetContainerXml getContainerXml)
        {
            _getTocXml = getTocXml;
            _getContentXml = getContentXml;
            _getContainerXml = getContainerXml;
        }

        public async Task<Dictionary<XmlType, XDocument>> From(IEnumerable<WikiPageRecord> pageRecords, string bookTitle) =>
            await Task.Run(() => new Dictionary<XmlType, XDocument>()
            {
                { XmlType.Container, _getContainerXml.GetContainer()},
                { XmlType.Content, _getContentXml.From(pageRecords, bookTitle) },
                { XmlType.Toc, _getTocXml.From(pageRecords, bookTitle) }

            });

            
    }
}