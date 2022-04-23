using System;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    public class Product
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Price Price { get; set; }
        public IQueryable<Category> Categories { get; set; }
        public IQueryable<string> Tags { get; set; }
        public Localization Localization { get; set; }
        public Uri Link { get; set; }
        public IQueryable<Uri> Images { get; set; }
        public IQueryable<string> Features { get; set; }
        public IQueryable<Characteristic> Variables { get; set; }
    }
}