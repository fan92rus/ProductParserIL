using System.Collections.Generic;
using ProductParserIL.Models;

namespace ProductParserIL.Domain
{
    public interface IProductSource
    {
        public IEnumerable<Product> Read();
    }
}