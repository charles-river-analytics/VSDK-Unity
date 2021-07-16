using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.Utilities
{
    public class Tree<T>
    {
        public T value;
        public HashSet<Tree<T>> children;

        public Tree(T value)
        {
            this.value = value;
            children = new HashSet<Tree<T>>();
        }

        public Tree<T> AddChild(T data)
        {
            var newTree = new Tree<T>(data);

            if(children.Add(newTree))
            {
                return newTree;
            }
            else
            {
                return newTree;
            }
        }

        public Tree<T> InChildNode(T data)
        {
            foreach(var t in children)
            {
                if(t.value.Equals(data))
                {
                    return t;
                }
            }

            return null;
        }
        
        public void Traverse(Action<T> visitAction)
        {
            Traverse(this, visitAction);
        }

        private void Traverse(Tree<T> node, Action<T> visitAction)
        {
            visitAction(node.value);

            foreach (Tree<T> child in node.children)
            {
                Traverse(child, visitAction);
            }
        }
    }
}