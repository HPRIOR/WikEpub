﻿using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WikEpubLib.Exceptions;

namespace WikEpubLib.IO
{
    /// <summary>
    /// Retrieves html from the given urls.
    /// </summary>
    /// <remarks>
    /// Converts urls into API calls and is used to validate the incoming urls (for matches in the Regex and uniqueness)
    /// Unique urls is an arbitrary constaint caused by clashes (and errors thrown) in the file system if two of the same urls are processed. (TODO)
    /// </remarks>
    public class HtmlInput : IHtmlInput
    {
        public async Task<HtmlDocument[]> GetHtmlDocumentsFromAsync(IEnumerable<string> urls, HtmlWeb htmlWeb)
        {
            if (UrlsAreValid(urls) & UrlsAreUnique(urls))
                return await Task.WhenAll(urls.Select(url => htmlWeb.LoadFromWebAsync(TranslateToApiCall(url))));
            throw new InvalidWikiUrlException(urls);
        }

        private string TranslateToApiCall(string url) =>
            $@"https://en.wikipedia.org/api/rest_v1/page/html/{url.Split('/').Last()}";

        private bool UrlsAreValid(IEnumerable<string> urls)
        {
            Regex regex = new Regex("(https:\\/\\/)?(en\\.)?wikipedia\\.org\\/(wiki\\/\\b(([-a-zA-Z0-9()@:%_\\+.~#?&\\/\\/=,]*){1}))");
            foreach (var url in urls)
                if (!regex.IsMatch(url))
                    return false;
            return true;
        }

        private bool UrlsAreUnique(IEnumerable<string> urls) => urls.ToHashSet().Count == urls.Count();
    }
}