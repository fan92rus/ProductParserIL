using System;
using System.Linq;
using System.Threading.Tasks;
using ProductParserIL.Domain.Parsers.Asos;

namespace ProductParserIL
{
    internal static class Program
    {
        static async Task Main()
        {
            try
            {
                var catParser = new AsosCategoryParser();
                var cats = await catParser.Read().ToListAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            Console.WriteLine("Hello World!");
        }
    }
}
