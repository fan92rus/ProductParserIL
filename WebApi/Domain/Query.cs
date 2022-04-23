using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Formatters;
using Models;

namespace WebApi.Domain
{
    public class Query
    {
        public IQueryable<Product> GetProduct() =>
            new EnumerableQuery<Product>(new List<Product>()
            {
                new Product()
                {
                    //Categories = new List<Category>(){new Category("обувь", null)},
                    Description = "Description",
                    Name = "Кросовки"
                }
            });
        //public Book GetBook() =>
        //    new Book
        //    {
        //        Title = "C# in depth.",
        //        Author = new Author
        //        {
        //            Name = "Jon Skeet"
        //        }
        //    };
    }

    public class Book
    {
        public string Title { get; set; }

        public Author Author { get; set; }
    }

    public class Author
    {
        public string Name { get; set; }
    }

}
