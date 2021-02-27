using HtmlAgilityPack;
using System.Threading.Tasks;
using WikEpubLib.Records;

namespace WikEpubLib.Interfaces
{
    public interface IEpubHtmlCreator
    {
        Task<IDocument> CreateHtmlDoc(WikiPageRecord wikiPageRecord, string filePath);
        
    }
}