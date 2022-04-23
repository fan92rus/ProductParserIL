using System.Collections.Generic;
using System.Linq;
using Models;

namespace ProductParserIL.Helpers
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
}