using System;
using System.Collections.Generic;

namespace Models
{
    public class Product
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Price Price { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public Localization Localization { get; set; }
        public IEnumerable<Uri> Images { get; set; }
        public IEnumerable<Characteristic> Features { get; set; }
    }
}