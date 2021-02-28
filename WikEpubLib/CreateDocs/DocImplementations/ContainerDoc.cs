using System.Threading.Tasks;
using System.Xml.Linq;

namespace WikEpubLib.CreateDocs
{
    public class ContainerDoc : AbstractXDoc
    {
        public ContainerDoc(XDocument document, string directory) : base(document, directory)
        {
        }

        public async override Task SaveAsync()
        {
            await SaveTaskAsync(_document, "container.xml");
        }
    }
}