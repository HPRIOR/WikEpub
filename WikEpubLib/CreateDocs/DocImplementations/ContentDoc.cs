using System.Threading.Tasks;
using System.Xml.Linq;

namespace WikEpubLib.CreateDocs
{
    public class ContentDoc : AbstractXDoc
    {
        public ContentDoc(XDocument document, string directory) : base(document, directory)
        {
        }

        public async override Task SaveAsync()
        {
            await SaveTaskAsync(_document, _directory, "content.opf");
        }
    }
}