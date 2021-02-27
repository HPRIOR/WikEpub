using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using WikEpubLib.Interfaces;

namespace WikEpubLib.CreateDocs
{
    public abstract class AbstractXDoc : IDocument
    {
        protected XDocument _document;
        protected string _directory;

        public AbstractXDoc(XDocument document, string directory)
        {
            _document = document;
            _directory = directory;
        }

        abstract public Task SaveAsync();

        protected async Task SaveTaskAsync(XDocument file, string toDirectory, string withFileName)
        {
            await using Stream stream = File.Create($"{toDirectory}/{withFileName}");
            await file.SaveAsync(stream, SaveOptions.None, CancellationToken.None);
        }
    }
}