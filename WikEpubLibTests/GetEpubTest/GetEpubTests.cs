using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using WikEpubLib;
using WikEpubLib.CreateDocs;
using WikEpubLib.IO;
using WikEpubLib.Records;
using WikEpubLibTests.GetEpubTest;

namespace WikEpubLibTests
{
    [TestClass]
    public class GetEpubTests
    {
        private GetEpub _getEpub;

        [TestInitialize]
        public async Task Init()
        {
            string htmlString =
                File.ReadAllText(@"C:\Users\User\Documents\Code\WebDev\WikEpub\WikEpubLibTests\GetEpubTest\Resources\htmlString.txt");
            var mockHtmlInput = new MockHtmlInput(htmlString);

            var getRecords = new GetWikiPageRecords();
            var getXmlDocument = new DocumentCreator(new GetTocXml(), new GetContentXml(), new GetContainerXml(), new CreateEpubHtml());
            var epubOut = new EpubOutput(new System.Net.Http.HttpClient());
            _getEpub = new GetEpub(getRecords, getXmlDocument, mockHtmlInput, epubOut);
            await createTestBook();
        }

        async private Task createTestBook()
        {
            string rootDirPath = @"C:\Users\User\Documents\Code\WebDev\WikEpub\WikEpubLibTests\GetEpubTest\Resources\TestOutput\";
            await _getEpub.FromAsync(new List<string>(), rootDirPath, "testbook", Guid.NewGuid());
        }

        [TestCleanup]
        public void TearDown()
        {
            DirectoryInfo directoryInfo =
                new DirectoryInfo(@"C:\Users\User\Documents\Code\WebDev\WikEpub\WikEpubLibTests\GetEpubTest\Resources\TestOutput\");

            directoryInfo.GetFiles().ToList().ForEach(file => file.Delete());
            directoryInfo.GetDirectories().ToList().ForEach(dir => dir.Delete(true));
        }

        private DirectoryInfo GetTestOutputDirInfo() => new DirectoryInfo(@"C:\Users\User\Documents\Code\WebDev\WikEpub\WikEpubLibTests\GetEpubTest\Resources\TestOutput\");

        [TestMethod]
        public void Creates_File_In_Directory()
        {
            var directoryInfo = GetTestOutputDirInfo();
            var directoryFiles = directoryInfo.GetFiles().ToList();
            Assert.AreEqual(1, directoryFiles.Count);
        }

        [TestMethod]
        public void Creates_Epub_File_in_Directory()
        {
            var directoryInfo = GetTestOutputDirInfo();
            var directoryFiles = directoryInfo.GetFiles().ToList();
            var fileExt = directoryFiles[0].Extension;
            Assert.AreEqual(".epub", fileExt);
        }

        private DirectoryInfo GetUnzippedEpubDirectory()
        {
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
        public void Epub_File_Contains_Mime_Type()
        {
            var dirInfo = GetUnzippedEpubDirectory();
            var mimetypeFile = dirInfo.GetFiles().ToList().First();
            Assert.AreEqual("mimetype", mimetypeFile.Name);
        }

        [TestMethod]
        public void MimeType_File_Is_Correct()
        {
            var dirInfo = GetUnzippedEpubDirectory();
            var mimetypeFile = dirInfo.GetFiles().ToList().First();
            var mimeTypeFileText = File.ReadAllText(mimetypeFile.FullName);
            var expectedMimeTypeFileText =
                File.ReadAllText(@"C:\Users\User\Documents\Code\WebDev\WikEpub\WikEpubLibTests\GetEpubTest\Resources\mimetype.txt");
            Assert.AreEqual(expectedMimeTypeFileText, mimeTypeFileText);
        }

        [TestMethod]
        public void Epub_File_Contains_ContainerXML()
        {
            var dirInfo = GetUnzippedEpubDirectory();
            var metaInfDir = dirInfo.GetDirectories().First(dir => dir.Name == "META-INF");
            var containerXML = metaInfDir.GetFiles().First();
            Assert.AreEqual("container.xml", containerXML.Name);
        }

        [TestMethod]
        public void ContainerXml_Is_Correct()
        {
            var dirInfo = GetUnzippedEpubDirectory();
            var metaInfDir = dirInfo.GetDirectories().First(dir => dir.Name == "META-INF");
            var containerXML = metaInfDir.GetFiles().First();
            var containerXmlText = File.ReadAllText(containerXML.FullName);
            var expectedContainerXmlText =
                File.ReadAllText(@"C:\Users\User\Documents\Code\WebDev\WikEpub\WikEpubLibTests\GetEpubTest\Resources\containerXml.txt");
            Assert.AreEqual(expectedContainerXmlText, containerXmlText);
        }

        [TestMethod]
        public void Epub_File_Contains_Correct_Num_Img_Files()
        {
            var dirInfo = GetUnzippedEpubDirectory();
            var imgRepoDir = dirInfo
                .GetDirectories()
                .First(dir => dir.Name == "OEBPS")
                .GetDirectories()
                .First(dir => dir.Name == "image_repo");

            var imageFiles = imgRepoDir.GetFiles().ToList();

            Assert.AreEqual(11, imageFiles.Count);
        }

        [TestMethod]
        public void ImgFiles_Are_Named_Correctly()
        {
            var dirInfo = GetUnzippedEpubDirectory();
            var imgRepoDir = dirInfo
                .GetDirectories()
                .First(dir => dir.Name == "OEBPS")
                .GetDirectories()
                .First(dir => dir.Name == "image_repo");

            var imageFiles = imgRepoDir
                .GetFiles()
                .ToList()
                .OrderBy(file => int.Parse(String.Concat(file.Name.Split('.')[0].Where(c => char.IsDigit(c)))))
                .ToList();

            int counter = 1;
            imageFiles.ForEach(imgfile =>
            {
                Assert.AreEqual($"image_{counter}", imgfile.Name.Split('.')[0]);
                counter++;
            });
        }

        [TestMethod]
        public void Epub_File_Contains_ContentOPF()
        {
            var dirInfo = GetUnzippedEpubDirectory();
            var OEBPSDir = dirInfo
                .GetDirectories()
                .First(dir => dir.Name == "OEBPS");
            var contentOPFFile = OEBPSDir.GetFiles().First(file => file.Name == "content.opf");
            Assert.AreEqual("content.opf", contentOPFFile.Name);
        }

        [TestMethod]
        public void ContentOPF_is_Correct()
        {
            var dirInfo = GetUnzippedEpubDirectory();
            var OEBPSDir = dirInfo
                .GetDirectories()
                .First(dir => dir.Name == "OEBPS");
            var contentOPFFile = OEBPSDir.GetFiles().First(file => file.Name == "content.opf");
            var contentOPFText = File.ReadAllText(contentOPFFile.FullName);
            var expectedContentOPFText =
                File.ReadAllText(@"C:\Users\User\Documents\Code\WebDev\WikEpub\WikEpubLibTests\GetEpubTest\Resources\ContentOPF.txt");
            Assert.AreEqual(expectedContentOPFText, contentOPFText);
        }

        [TestMethod]
        public void Epub_File_Contains_HTML()
        {
            var dirInfo = GetUnzippedEpubDirectory();
            var OEBPSDir = dirInfo
                .GetDirectories()
                .First(dir => dir.Name == "OEBPS");
            var htmlFile = OEBPSDir.GetFiles().First(file => file.Name == "Benzocfluorene.html");
            Assert.AreEqual("Benzocfluorene.html", htmlFile.Name);
        }

        [TestMethod]
        public void Html_File_Is_Correct()
        {
            var dirInfo = GetUnzippedEpubDirectory();
            var OEBPSDir = dirInfo
                .GetDirectories()
                .First(dir => dir.Name == "OEBPS");
            var htmlFile = OEBPSDir.GetFiles().First(file => file.Name == "Benzocfluorene.html");
            var htmlFileText = File.ReadAllText(htmlFile.FullName);
            var expectedHtmlText =
                File.ReadAllText(@"C:\Users\User\Documents\Code\WebDev\WikEpub\WikEpubLibTests\GetEpubTest\Resources\epubHtml.txt");
            Assert.AreEqual(expectedHtmlText, htmlFileText);
        }

        [TestMethod]
        public void Epub_File_Contains_Toc()
        {
            var dirInfo = GetUnzippedEpubDirectory();
            var OEBPSDir = dirInfo
                .GetDirectories()
                .First(dir => dir.Name == "OEBPS");
            var tocFile = OEBPSDir.GetFiles().First(file => file.Name == "toc.ncx");
            Assert.AreEqual("toc.ncx", tocFile.Name);
        }

        [TestMethod]
        public void Toc_File_Is_Correct()
        {
            var dirInfo = GetUnzippedEpubDirectory();
            var OEBPSDir = dirInfo
                .GetDirectories()
                .First(dir => dir.Name == "OEBPS");
            var tocFile = OEBPSDir.GetFiles().First(file => file.Name == "toc.ncx");
            var tocText = File.ReadAllText(tocFile.FullName);
            var expectedTocText = File.ReadAllText(@"C:\Users\User\Documents\Code\WebDev\WikEpub\WikEpubLibTests\GetEpubTest\Resources\toc.txt");
            Assert.AreEqual(expectedTocText, tocText);
        }
    }
}