using System.Threading.Tasks;

namespace WikEpubLib.Interfaces
{
    public interface IDocument
    {
        Task SaveAsync();
    }
}