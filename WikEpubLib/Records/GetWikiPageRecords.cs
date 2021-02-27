using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using WikEpubLib.Interfaces;

namespace WikEpubLib.Records
{
    /// <summary>
    /// Creates a record for an HTML page.
    /// </summary>
    /// <remarks>
    /// Several relevant bits of information are retrieved from the HTML which are used throughtout the program:
    ///     - An id for the file is given by the title of the article
    ///     - For each image in the document a map is created between the its current src value and a new src value
    ///       a relative directory in the new epub file
    ///     - Section headings for each <h2> tag in the document
    /// </remarks>
    public class GetWikiPageRecords : IGetWikiPageRecords
    {
        public WikiPageRecord From(HtmlDocument html, string imageDirectory)
        {
            IEnumerable<HtmlNode> allNodes = html.DocumentNode.Descendants();
            IEnumerable<HtmlNode> contentNodes = allNodes.First(n => n.Name == "body").Descendants().Distinct();
            IEnumerable<HtmlNode> imgNodes = GetImageNodesFrom(contentNodes);
            return new WikiPageRecord
            {
                htmlDoc = html,
                Id = GetIdFrom(allNodes),
                SrcMap = imgNodes.Any() ? GetSrcMapFrom(imgNodes, imageDirectory) : null,
                SectionHeadings = GetSectionHeadingsFrom(contentNodes)
            };
        }

        private string GetIdFrom(IEnumerable<HtmlNode> nodes) =>
            nodes
            .First(n => n.Name == "title")
            .InnerHtml.Split('-').First()
            .Trim().Replace(' ', '_').Replace(")", "").Replace("(", "");

        private IEnumerable<HtmlNode> GetImageNodesFrom(IEnumerable<HtmlNode> nodes) => nodes.Where(n => n.Name == "img");

        private int _imageId = 1;

        private string GetImageId(string originalSrc) => $"image_{_imageId++}.{GetExtension(originalSrc)}";

        private string GetExtension(string originalSrc) => originalSrc switch
        {
            // this can be more efficient instead of checking svg every time
            // maybe create hashset of expected formats, then check
            string when originalSrc.Contains("svg") => "svg.png",
            _ => originalSrc.Split('.')[^1] + ".png"
        };

        private Dictionary<string, string> GetSrcMapFrom(IEnumerable<HtmlNode> imageNodes, string imageDirectory) =>
            imageNodes
            .Select(n => n.GetAttributeValue("src", "null"))
            .Distinct().ToDictionary(src => src, src => @$"{imageDirectory}\{GetImageId(src)}");

        private List<(string id, string sectionName)> GetSectionHeadingsFrom(IEnumerable<HtmlNode> nodes) =>
            nodes
            .Where(n => n.Name == "h2")
            .Select(n => ($"#{n.GetAttributeValue("id", "null")}", n.InnerText)).ToList();
    }
}