using HtmlAgilityPack;
using System.Threading.Tasks;
using WikEpubLib.Records;

namespace WikEpubLib.Interfaces
{
    public interface IParseHtml
    {
        Task<WikiPageRecord> ParseAsync(WikiPageRecord wikiPageRecord);
        
    }
}