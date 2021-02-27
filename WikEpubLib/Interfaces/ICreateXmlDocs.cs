using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using WikEpubLib.Enums;
using WikEpubLib.Records;

namespace WikEpubLib
{
    public interface ICreateXmlDocs
    {
        IEnumerable<Task<(XmlType type, XDocument doc)>> From(IEnumerable<WikiPageRecord> pageRecords, string bookTitle);
    }
}