﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WikEpubLib.Enums;
using WikEpubLib.Records;

namespace WikEpubLib.IO
{
    /// <summary>
    /// Handles output functions of the library: creating directories, saving files to directories
    /// </summary>
    public class EpubOutput : IEpubOutput
    {
        private HttpClient _httpClient;

        public EpubOutput(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Uses context (Dictionary: enum -> directory path) shared accross program to create directories
        /// </summary>
        public async Task CreateDirectoriesAsync(Dictionary<Directories, string> directories) =>
            await Task.Run(() =>
            {
                Directory.CreateDirectory(directories[Directories.OEBPS]);
                Directory.CreateDirectory(directories[Directories.METAINF]);
                Directory.CreateDirectory(directories[Directories.IMAGES]);
            });

        public async Task CreateMimeFile(Dictionary<Directories, string> directories) =>
           await File.WriteAllTextAsync($@"{directories[Directories.BOOKDIR]}\mimetype", "application/epub+zip");

        /// <summary>
        /// Downloads each image from the html file, and saves it to the source specified in the records src mapping.
        /// </summary>
        /// <remarks>
        /// Src mapping from record includes: old src (download url) -> new src (local file directory).
        /// Switch statement handles the various image sources and returns an appropriate url.
        /// If an unknown src is encountered, no image is downloaded and it is written out to the console.
        /// </remarks>
        /// <returns>Task which represents a completed download for each image in a record</returns>
        public IEnumerable<Task> DownloadImages(WikiPageRecord pageRecord, Dictionary<Directories, string> directories) =>
            pageRecord
            .SrcMap
            .AsParallel()
            .WithDegreeOfParallelism(10)
            .Select(async imgSrc =>
            {
                var srcKey = imgSrc.Key switch
                {
                    _ when imgSrc.Key.StartsWith("https://") => imgSrc.Key,
                    _ when imgSrc.Key.StartsWith(@"/api") => $"https://en.wikipedia.org{imgSrc.Key}",
                    _ when imgSrc.Key.StartsWith(@"//") => @$"https:{imgSrc.Key}",
                    _ => "unknown"
                };
                if (srcKey == "unknown")
                {
                    Debug.WriteLine($"Unknown image href encountered in {pageRecord.Id} wiki: \n" + imgSrc.Key);
                    return;
                }
                HttpResponseMessage response = await _httpClient.GetAsync(srcKey);
                using var memoryStream = await response.Content.ReadAsStreamAsync();
                await using var fileStream = File.Create($@"{directories[Directories.OEBPS]}\{imgSrc.Value}");
                await memoryStream.CopyToAsync(fileStream);
            });

        public async Task ZipFiles(Dictionary<Directories, string> directories, Guid bookId) =>
            await Task.Run(() =>
            {
                ZipFile.CreateFromDirectory(
                    directories[Directories.BOOKDIR],
                    @$"{directories[Directories.BOOKDIR]}.epub");
                Directory.Delete(directories[Directories.BOOKDIR], true);
            });
    }
}