using HtmlAgilityPack;
using System.IO;
using System.Threading.Tasks;
using WikEpubLib.Interfaces;

namespace WikEpubLib.CreateDocs
{
    public abstract class AbstractHtmlDoc : IDocument
    {
        protected HtmlDocument _document;
        protected string _directory;

        public AbstractHtmlDoc(HtmlDocument document, string directory)
        {
            _document = document;
            _directory = directory;
        }

        public abstract Task SaveAsync();

        protected async Task SaveTaskAsync(HtmlDocument file, string toDirectory, string withFileName)
        {
            await using Stream stream = File.Create($"{toDirectory}/{withFileName}");
            await Task.Run(() => file.Save(stream));
        }
    }
}