using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Models
{
    public static class TreeEx
    {
        public static int Width<TKey, TNode>(this ITreeNode<TKey, TNode> treeNode)
            where TNode : ITreeNode<TKey, TNode> => treeNode.ChildNodes.Any() ? treeNode.ChildNodes.Sum(x => x.Width()) : 1;

        public static int Deep<TKey, TNode>(this ITreeNode<TKey, TNode> treeNode)
           where TNode : ITreeNode<TKey, TNode> => treeNode.ChildNodes.Any() ? treeNode.ChildNodes.Max(x => x.Deep()) + 1 : 1;

        public static IEnumerable<TNode> GetLeafs<TKey, TValue, TNode>(this ITreeNode<TKey,  TNode> treeNode)
            where TNode : ITreeNode<TKey, TNode> => treeNode.Find(x => !x.ChildNodes.Any());
    }
    public interface ITreeNode<TKey, TNode> where TNode : ITreeNode<TKey, TNode>
    {
        TKey Name { get; set; }
        TNode Parent { get; set; }
        List<TNode> ChildNodes { get; }
        List<string> GetPath();
        IEnumerable<TNode> ToFlat();
        IEnumerable<TNode> Find(Func<ITreeNode<TKey, TNode>, bool> expr);
    }

    public abstract class TreeNode<TKey, TNode> : ITreeNode<TKey, TNode>
        where TNode : class, ITreeNode<TKey, TNode>
    {
        protected TreeNode()
        {
            ChildNodes = new List<TNode>();
        }
        protected TreeNode(TKey name, TNode parent) : this()
        {
            Name = name;
            Parent = parent;
        }

        public TKey Name { get; set; }

        [JsonIgnore]
        public TNode Parent { get; set; }
        public List<TNode> ChildNodes { get; protected set; }

        public bool TryFindOne(Func<ITreeNode<TKey, TNode>, bool> expr, out TNode node)
        {
            node = Find(expr).FirstOrDefault();
            return node != default;
        }
        public IEnumerable<TNode> ToFlat()
        {
            yield return this as TNode;

            foreach (var child in ChildNodes.SelectMany(x => x.ToFlat()))
                yield return child;
        }
        public List<string> GetPath()
        {
            var path = new List<string>() { Name.ToString() };

            if (Parent != null)
                path.AddRange(Parent.GetPath());

            return path;
        }
        public IEnumerable<TNode> Find(Func<ITreeNode<TKey, TNode>, bool> expr)
        {
            if (expr(this))
                yield return this as TNode;
            foreach (var node in ChildNodes.SelectMany(x => x.Find(expr)))
                yield return node;
        }

        public override string ToString() => Name.ToString();
    }
}