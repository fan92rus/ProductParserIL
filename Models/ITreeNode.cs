using System;
using System.Collections.Generic;

namespace Models
{
    public interface ITreeNode<TKey, TNode> where TNode : ITreeNode<TKey, TNode>
    {
        TKey Name { get; set; }
        TNode Parent { get; set; }
        List<TNode> ChildNodes { get; }
        List<string> GetPath();
        IEnumerable<TNode> ToFlat();
        IEnumerable<TNode> Find(Func<ITreeNode<TKey, TNode>, bool> expr);
    }
}