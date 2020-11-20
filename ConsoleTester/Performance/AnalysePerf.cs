﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WikEpubLib;
using WikEpubLib.CreateDocs;
using WikEpubLib.IO;
using WikEpubLib.Records;

namespace CSharpConsoleDebugger.Performance
{
    public static class AnalysePerf
    {
        public static void GetRunTime()
        {
            Console.WriteLine("Enter a log message: ");
            var userLogMessage = Console.ReadLine();

            List<string> urls = new() { "https://en.wikipedia.org/wiki/Sean_Connery", "https://en.wikipedia.org/wiki/Physiology", "https://en.wikipedia.org/wiki/YouTube" };
            string rootDirectory = @"C:\Users\User\Documents\Code\WikEpub\ConsoleTester\Performance\EpubRepo";
            string bookTitle = "TestBook1";

            int num_iterations = 5;
            var avg_time = Enumerable.Range(0, num_iterations).Sum(x => 
            {
                GetEpub getEpub = GetEpubClass();
                return TimeCreateBook(getEpub, urls, rootDirectory, bookTitle).Result;
            })/num_iterations;

            string logMessage = $"{DateTime.Now} \n \n " +
                $"Most recent change: {userLogMessage} \n \n " +
                $"average run-time over {num_iterations} iterations: {Math.Round(avg_time, 3)} seconds \n \n" +
                $"---------------------------------------------- " +
                $"\n \n";
            File.AppendAllText(@"C:\Users\User\Documents\Code\WikEpub\ConsoleTester\Performance\RunTimeLog.txt", logMessage);
        }

        private static GetEpub GetEpubClass()
        {
            HtmlInput htmlInput = new HtmlInput();
            ParseHtml parseHtml = new ParseHtml();
            GetWikiPageRecords getWikiPageRecords = new GetWikiPageRecords();
            GetXmlDocs getXmlDocs = new GetXmlDocs(new GetTocXml(), new GetContentXml(), new GetContainerXml());
            EpubOutput epubOutput = new EpubOutput(new HttpClient());

            GetEpub getEpub = new GetEpub(parseHtml, getWikiPageRecords, getXmlDocs, htmlInput, epubOutput);
            return getEpub;
        }

        async static Task<double> TimeCreateBook(GetEpub getEpub, List<string>urls, string directory, string bookTitle)
        {
            Guid guid = Guid.NewGuid();
            Stopwatch stopwatch = Stopwatch.StartNew();
            await getEpub.FromAsync(urls, directory, bookTitle, guid);
            stopwatch.Stop();
            File.Delete(@$"{directory}\{guid}.epub"); 
            return stopwatch.Elapsed.TotalSeconds;

        }
    }
}