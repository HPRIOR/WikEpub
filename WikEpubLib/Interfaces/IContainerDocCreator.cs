using System.Threading.Tasks;

namespace WikEpubLib.Interfaces
{
    public interface IContainerDocCreator
    {
        Task<IDocument> CreateContainerDoc(string filePath);
    }
}