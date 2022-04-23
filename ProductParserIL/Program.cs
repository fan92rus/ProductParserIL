using System;
using System.Collections.Generic;

namespace ProductParserIL
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }

    public interface ICategorySource
    {
        public IEnumerable<Category> Read();
    }

    public interface IProductSource
    {
        public IEnumerable<Product> Read();
    }
}
