using System.Threading.Tasks;
using System.Xml.Linq;
using WikEpubLib.CreateDocs;
using WikEpubLib.Enums;

namespace WikEpubLib.Interfaces
{
    public interface IContainerDocCreator
    {
        Task<IDocument> CreateContainerDoc(string filePath);
    }
}