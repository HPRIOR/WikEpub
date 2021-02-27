using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WikEpubLib.Interfaces;
using WikEpubLib.Records;

namespace WikEpubLib.CreateDocs
{
    /// <summary>
    /// Removes irrelevant parts of the HTML, and updates src of images to local directory specified in WikiPageRecords.
    /// </summary>
    /// <remarks>
    /// Changes to HTML which are presented by the e-reader occur here. This should be extended to increase compatablity
    /// with a variety of different Wikipedia pages.
    /// </remarks>
    public class CreateEpubHtml : IEpubHtmlCreator
    {
        public async Task<IDocument> CreateHtmlDoc(WikiPageRecord wikiPageRecord, string filePath)
        {
            return await Task.Run(() =>
            {
                return new HtmlDoc(CreateHtml(wikiPageRecord), filePath, wikiPageRecord);
            });
        }

        public HtmlDocument CreateHtml(WikiPageRecord wikiPageRecord)
        {
            HtmlDocument newDocument = new HtmlDocument();
            var initNode =
                HtmlNode.CreateNode($"<html><head><meta charset=\"utf-8\"><title>{wikiPageRecord.Id.Replace('_', ' ')}</title></head><body></body></html>");
            newDocument.DocumentNode.AppendChild(initNode);
            var bodyNode = newDocument.DocumentNode.SelectSingleNode("/html/body");

            bool nodePredicate(HtmlNode node) => node.Name != "style"
                && !(node.Name == "style" ||
                node.Descendants().Any(d => d.Attributes.Any(a => a.Value == "navigation" | a.Value == "vertical-navbox nowraplinks hlist")));

            var childNodes = wikiPageRecord.htmlDoc
                .DocumentNode
                .SelectSingleNode("//html/body")
                .ChildNodes;

            childNodes.AsParallel().AsOrdered().ToList().ForEach(node =>
            {
                if (nodePredicate(node))
                {
                    ChangeHyperLinks(node);
                    ChangeDownloadLinks(node, wikiPageRecord.SrcMap);
                    bodyNode.AppendChild(node);
                }
            });

            return newDocument;
        }

        private void ChangeHyperLinks(HtmlNode node)
        {
            if (node.Name == "a" && !node.ParentNode.HasClass("mw-ref"))
                ReplaceNode(node);
            node.Descendants("a")
                .AsParallel()
                .ToList()
                .ForEach(n =>
                {
                    if (!n.ParentNode.HasClass("mw-ref") & n.HasChildNodes && n.FirstChild.Name != "img") ReplaceNode(n);
                    ChangeHrefLink(n);
                });
        }

        private void ChangeHrefLink(HtmlNode node)
        {
            var oldHref = node.GetAttributeValue("href", "null");
            if (oldHref != "null")
                node.SetAttributeValue("href", $"#{oldHref.Split("#").Last()}");
        }

        private void ReplaceNode(HtmlNode node)
        {
            HtmlNode newNode = HtmlNode.CreateNode($"<span>{node.InnerText}</span>");
            node.ParentNode.ReplaceChild(newNode, node);
        }

        /// <summary>
        ///  Uses the src map provided by a WikiPageRecord to change the old img src to new relative local directory.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="srcMap">old src url --> local relative directory</param>
        private void ChangeDownloadLinks(HtmlNode node, Dictionary<string, string> srcMap)
        {
            var imgNodes = node.Descendants("img");
            if (node.Name == "img") imgNodes.Append(node);
            if (!imgNodes.Any()) return;
            imgNodes.Distinct().AsParallel().ToList().ForEach(imgNode =>
            {
                var oldSrcValue = imgNode.GetAttributeValue("src", "null");
                if (srcMap.ContainsKey(oldSrcValue))
                    imgNode.SetAttributeValue("src", srcMap[oldSrcValue]);
            });
        }
    }
}