using System;

namespace Models
{
    public class Category : TreeNode<string, Category>
    {
        public Category(string name, Uri link, Category category) : this(name, link)
        {
            Parent = category;
        }
        public Category(string name, Uri link)
        {
            this.Name = name;
            Url = link;
        }

        public Uri Url { get; set; }
    }
}