using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using WikEpubLib.Enums;
using WikEpubLib.Interfaces;
using WikEpubLib.Records;

namespace WikEpubLib.CreateDocs
{
    /// <summary>
    /// Creates the content file required by the epub format. 
    /// </summary>
    /// <remarks>
    /// The content file uniquly identifies the contents of the epub file, such as the html page, and the images, 
    /// and contains information regarding their relative directories, and media-type. 
    /// </remarks>
    public class GetContentXml : IGetContentXml
    {
        public async Task<(XmlType, XDocument)> From(IEnumerable<WikiPageRecord> wikiPageRecords, string bookTitle)
        {
            XNamespace defaultNs = "http://www.idpf.org/2007/opf";
            return await Task.Run(() =>
            {
                XElement package =
                   new XElement(
                       defaultNs + "package",
                       new XAttribute("version", "2.0"),
                       new XAttribute("unique-identifier", "bookid")
                   );

                var xmlns = XNamespace.Xmlns + "dc";
                XNamespace purlNs = "http://purl.org/dc/elements/1.1/";

                XElement metadata =
                    new XElement(defaultNs + "metadata",
                        new XElement(purlNs + "title", bookTitle, new XAttribute(xmlns, purlNs)),
                        new XElement(purlNs + "publisher", "Wikipedia", new XAttribute(xmlns, purlNs)),
                        new XElement(purlNs + "date",
                            $"{DateTime.Now.Day}-{DateTime.Now.Month}-{DateTime.Now.Year}", new XAttribute(xmlns, purlNs)),
                        new XElement(purlNs + "creator", "Harry Prior", new XAttribute(xmlns, purlNs)),
                        new XElement(defaultNs + "meta", new XAttribute("name", "cover"), new XAttribute("content", "cover-image"))
                    );

                XElement manifest =
                            new XElement(defaultNs + "manifest");
                manifest.Add(itemElement(defaultNs, "cover", "cover.html", "application/xhtml+xml"));
                manifest.Add(itemElement(defaultNs, "ncxtoc", "toc.ncx", "application/dtbncx+xml"));

                // id of image is arbitrary. href is given by record src map, which also provides the file type
                int imageId = 1;
                string GetImageId() => $"image_{imageId++}";
                foreach (var record in wikiPageRecords)
                {
                    manifest.Add(itemElement(defaultNs, record.Id, $"{record.Id}.html", "application/xhtml+xml"));
                    if (record.SrcMap is not null)
                        foreach (var dictItem in record.SrcMap)
                            manifest.Add(itemElement(defaultNs, GetImageId(), dictItem.Value, $"image/{dictItem.Value.Split('.').Last().ToLower()}"));
                }

                XElement spine = new XElement(defaultNs + "spine", new XAttribute("toc", "ncxtoc"));
                spine.Add(new XElement(defaultNs + "itemref", new XAttribute("idref", "cover")));
                foreach (var record in wikiPageRecords)
                {
                    spine.Add(new XElement(defaultNs + "itemref", new XAttribute("idref", record.Id)));
                }
                package.Add(metadata);
                package.Add(manifest);
                package.Add(spine);

                return (XmlType.Content, new XDocument(new XDeclaration("1,0", "utf-8", "no"), package));
            });
        }

        private XElement itemElement(XNamespace defaultNs, string id, string href, string mediaType) =>
           new XElement(
               defaultNs + "item",
               new XAttribute("id", id),
               new XAttribute("href", href),
               new XAttribute("media-type", mediaType)
               );
    }
}