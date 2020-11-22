﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikEpubLib.Interfaces;
using WikEpubLib.Records;

namespace WikEpubLib.CreateDocs
{
    public class HtmlParser : IParseHtml
    {

        public async Task<(HtmlDocument doc, WikiPageRecord record)> ParseAsync(HtmlDocument htmlDocument, WikiPageRecord wikiPageRecord)
        {
            return await Task.Run(() =>
            {
                return (CreateHtml(htmlDocument, wikiPageRecord), wikiPageRecord);
            });
        }

        public HtmlDocument CreateHtml(HtmlDocument inputDocument, WikiPageRecord wikiPageRecord)
        {
            HtmlDocument newDocument = new HtmlDocument();
            var initNode =
                HtmlNode.CreateNode($"<html><head><meta charset=\"utf-8\"><title>{wikiPageRecord.Id.Replace('_', ' ')}</title></head><body></body></html>");
            newDocument.DocumentNode.AppendChild(initNode);
            var bodyNode = newDocument.DocumentNode.SelectSingleNode("/html/body");
            
            // this exludes images
            bool nodePredicate(HtmlNode node) => node.Name != "style"
                && !(node.Name == "style" || node.Descendants().Any(d => d.Attributes.Any(a => a.Name == "role")));

            var childNodes = inputDocument
                .DocumentNode
                .SelectSingleNode("//*[@id='mw-content-text']/div[1]")
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
            if(node.Name == "a" && !(node.ParentNode.HasClass("reference")))
                ReplaceNode(node);
            node.Descendants("a").AsParallel().ToList().ForEach(n => { if (!n.ParentNode.HasClass("reference")) ReplaceNode(n); });
        }

        private void ReplaceNode(HtmlNode node)
        {
            HtmlNode newNode = HtmlNode.CreateNode($"<span>{node.InnerText}</span>");
            node.ParentNode.ReplaceChild(newNode, node);
        }
        
        private void ChangeDownloadLinks(HtmlNode node, Dictionary<string, string> srcMap)
        {
            var imgNodes = node.Descendants("img");
            if (node.Name == "img") imgNodes.Append(node);
            if (!imgNodes.Any()) return;
            imgNodes.AsParallel().ToList().ForEach(imgNode =>
            {
                var oldSrcValue = node.GetAttributeValue("src", "null");
                if (srcMap.ContainsKey(oldSrcValue))
                    imgNode.SetAttributeValue("src", srcMap[oldSrcValue]);
            });
        }

    }
}
