using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using ProductParserIL.Models;
using RestSharp;

namespace ProductParserIL.Domain.Parsers.Asos
{
    public class AsosCategoryParser : ICategorySource
    {
        private readonly RestClient _client;

        public AsosCategoryParser()
        {
            _client = new RestClient("https://www.asos.com/");
        }

        public async IAsyncEnumerable<Category> Read()
        {
            var content = await _client.ExecuteAsync(new RestRequest("/women/"));
            var jsonText = Regex.Match(content.Content, "category\":.*(?=,\n.+savedItems)").Value;
            var catsDto = JsonConvert.DeserializeObject<CategoriesDto>(jsonText)?.Items;

            foreach (var cat in catsDto)
                yield return new Category(cat.Name, cat.Link);
        }

        private class CategoriesDto
        {
            [JsonPropertyName("childLinks")]
            public IEnumerable<CategoryDto> Items { get; set; }
        }
        private class CategoryDto
        {
            [JsonPropertyName("friendlyName")]
            public string Name { get; set; }
            [JsonPropertyName("linkUrl")]
            public Uri Link { get; set; }
        }
    }
}