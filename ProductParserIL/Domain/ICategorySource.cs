using System.Collections.Generic;
using Models;

namespace ProductParserIL.Domain
{
    public interface ICategorySource
    {
        public IAsyncEnumerable<Category> Read();
    }
}