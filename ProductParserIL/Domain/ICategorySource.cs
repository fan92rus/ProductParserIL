using System.Collections.Generic;
using ProductParserIL.Models;

namespace ProductParserIL.Domain
{
    public interface ICategorySource
    {
        public IAsyncEnumerable<Category> Read();
    }
}