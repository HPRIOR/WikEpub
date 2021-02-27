using Microsoft.VisualStudio.TestTools.UnitTesting;
using WikEpubLib.IO;
using WikEpubLib.CreateDocs;
using WikEpubLib.Records;
using WikEpubLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using WikEpubLibTests.GetEpubTest;
using HtmlAgilityPack;

namespace WikEpubLibTests
{
    [TestClass]
    public class GetEpubTests
    {
        private GetEpub _getEpub;
        [TestInitialize]
        public void Init()
        {
            string htmlString = 
                File.ReadAllText(@"C:\Users\User\Documents\Code\WebDev\WikEpub\WikEpubLibTests\GetEpubTest\Resources\htmlString.txt");
            var mockHtmlInput = new MockHtmlInput(htmlString);

            var htmlParser = new HtmlParser();
            var getRecords = new GetWikiPageRecords();
            var getXmlDocument = new GetXmlDocs(new GetTocXml(), new GetContentXml(), new GetContainerXml());
            var epubOut = new EpubOutput(new System.Net.Http.HttpClient());
            _getEpub = new GetEpub(htmlParser, getRecords, getXmlDocument, mockHtmlInput, epubOut);

        }

        [TestCleanup]
        public void TearDown()
        {
            DirectoryInfo directoryInfo =
                new DirectoryInfo(@"C:\Users\User\Documents\Code\WebDev\WikEpub\WikEpubLibTests\GetEpubTest\Resources\TestOutput\");

            directoryInfo.GetFiles().ToList().ForEach(file => file.Delete());
            directoryInfo.GetDirectories().ToList().ForEach(dir => dir.Delete(true));
        }

        async private Task createTestBook()
        {
            string rootDirPath = @"C:\Users\User\Documents\Code\WebDev\WikEpub\WikEpubLibTests\GetEpubTest\Resources\TestOutput\";
            await _getEpub.FromAsync(new List<string>(), rootDirPath, "testbook", Guid.NewGuid());
        }

        [TestMethod]
        async public Task Does_Not_Throw_Error()
        {
            await createTestBook();
        }

        private DirectoryInfo GetTestOutputDirInfo() => new DirectoryInfo(@"C:\Users\User\Documents\Code\WebDev\WikEpub\WikEpubLibTests\GetEpubTest\Resources\TestOutput\");

        [TestMethod]
        async public Task Creates_File_In_Directory()
        {
            await createTestBook();
            var directoryInfo = GetTestOutputDirInfo();
            var directoryFiles = directoryInfo.GetFiles().ToList();
            Assert.AreEqual(1, directoryFiles.Count);
        }

        [TestMethod]
        async public Task Creates_Epub_File_in_Directory()
        {
            await createTestBook();
            var directoryInfo = GetTestOutputDirInfo();
            var directoryFiles = directoryInfo.GetFiles().ToList();
            var fileExt = directoryFiles[0].Extension;
            Assert.AreEqual(".epub", fileExt);
        }
    
        async private Task<DirectoryInfo> GetUnzippedEpubDirectory()
        {
            await createTestBook();
            var directoryInfo = GetTestOutputDirInfo();
            var epubFile = directoryInfo.GetFiles().ToList().First();
            File.Move(epubFile.FullName, Path.ChangeExtension(epubFile.FullName, ".zip"));
            File.Delete(epubFile.FullName);
            var zipFile = GetTestOutputDirInfo().GetFiles().ToList().First();
            var unzippedDir =
                @"C:\Users\User\Documents\Code\WebDev\WikEpub\WikEpubLibTests\GetEpubTest\Resources\TestOutput\UnzipFolder";
            
            ZipFile.ExtractToDirectory(
                zipFile.FullName, 
                unzippedDir
            );
            return new DirectoryInfo(unzippedDir); 
        }

        [TestMethod]
        async public Task Epub_File_Contains_Mime_Type()
        {
            var dirInfo = await GetUnzippedEpubDirectory();
            var mimetypeFile = dirInfo.GetFiles().ToList().First();
            Assert.AreEqual("mimetype", mimetypeFile.Name);
        }

        [TestMethod]
        async public Task Epub_File_Contains_ContainerXML()
        {
            var dirInfo = await GetUnzippedEpubDirectory();
            var metaInfDir = dirInfo.GetDirectories().First(dir => dir.Name == "META-INF");
            var containerXML = metaInfDir.GetFiles().First();
            Assert.AreEqual("container.xml", containerXML.Name);
        }

        [TestMethod]
        async public Task Epub_File_Contains_Correct_Num_Img_Files()
        {
            var dirInfo = await GetUnzippedEpubDirectory();
            var imgRepoDir = dirInfo
                .GetDirectories()
                .First(dir => dir.Name == "OEBPS")
                .GetDirectories()
                .First(dir => dir.Name == "image_repo");

            var imageFiles = imgRepoDir.GetFiles().ToList();

            Assert.AreEqual(11, imageFiles.Count);
        }

        [TestMethod]
        async public Task Epub_File_Contains_ContentOPF()
        {
            //var dirInfo = await GetUnzippedEpubDirectory();
            //var OEBPSDir = dirInfo
            //    .GetDirectories()
            //    .First(dir => dir.Name == "OEBPS");
            //OEBPSDir.

            //Assert.AreEqual(11, imageFiles.Count);
        }


       

        
    }
}
