using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Html.Dom;
using ProductParserIL.Helpers;
using ProductParserIL.Models;
using RestSharp;

namespace ProductParserIL.Domain.Parsers.Asos
{
    public class AsosCategoryParser : ICategorySource
    {
        private readonly RestClient _client;
        private readonly HtmlDomTools _domTools;
        public AsosCategoryParser()
        {
            _domTools = new HtmlDomTools(null);
            _client = new RestClientFactory(null).CreateRc("https://www.asos.com/", false);
        }

        public async IAsyncEnumerable<Category> Read()
        {
            var content = await _client.ExecuteWitHeadersAsync(new RestRequest("/women/"));
            var doc = await _domTools.GetDocument(content.Content);
            var items = doc.QuerySelectorAll("[data-testid = text-link]").Cast<IHtmlAnchorElement>();

            foreach (var cat in items)
                yield return new Category(cat.TextContent, new Uri(cat.Href));
        }
    }
}