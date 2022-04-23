using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Models
{
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