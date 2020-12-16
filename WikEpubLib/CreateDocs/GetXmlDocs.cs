using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using WikEpubLib.Enums;
using WikEpubLib.Interfaces;
using WikEpubLib.Records;

namespace WikEpubLib.CreateDocs
{
    /// <summary>
    /// Wrapper class which gets all the xml files for the epub format (content, contianer, toc).
    /// </summary>
    public class GetXmlDocs : IGetXmlDocs
    {
        private readonly IGetTocXml _getTocXml;
        private readonly IGetContentXml _getContentXml;
        private readonly IGetContainerXml _getContainerXml;

        public GetXmlDocs(IGetTocXml getTocXml, IGetContentXml getContentXml, IGetContainerXml getContainerXml)
        {
            _getTocXml = getTocXml;
            _getContentXml = getContentXml;
            _getContainerXml = getContainerXml;
        }

        /// <summary>
        /// Returns tuple of xml type and document
        /// </summary>
        /// <remarks>
        /// The type is used elsewhere to determine the directory of the XML path 
        /// </remarks>
        /// <param name="pageRecords"></param>
        /// <param name="bookTitle"></param>
        /// <returns>IEnumberable of Task<tuples>: type of xml file which is used by saving functons elsewhere to determine the directory of the file, and the actual document </returns>
        public IEnumerable<Task<(XmlType type, XDocument doc)>> From(IEnumerable<WikiPageRecord> pageRecords, string bookTitle) =>
            new List<Task<(XmlType type, XDocument doc)>>()
            {
                _getContainerXml.GetContainer(),
                _getContentXml.From(pageRecords, bookTitle),
                _getTocXml.From(pageRecords, bookTitle)
            };
    }
}