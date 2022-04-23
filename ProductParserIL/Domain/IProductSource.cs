using System.Collections.Generic;
using Models;

namespace ProductParserIL.Domain
{
    public interface IProductSource
    {
        public IEnumerable<Product> Read();
    }
}