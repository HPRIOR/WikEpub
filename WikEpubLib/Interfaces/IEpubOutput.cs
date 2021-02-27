using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WikEpubLib.Enums;
using WikEpubLib.Records;

namespace WikEpubLib
{
    public interface IEpubOutput
    {
        Task CreateDirectoriesAsync(Dictionary<Directories, string> directories);

        IEnumerable<Task> DownloadImages(WikiPageRecord pageRecord, Dictionary<Directories, string> directories);

        Task ZipFiles(Dictionary<Directories, string> directories, Guid guid);

        Task CreateMimeFile(Dictionary<Directories, string> directories);
    }
}