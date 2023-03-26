using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDwarf.Utils
{
    public enum Branch { Left, Right }

    public class PathfindingNode
    {
        public PathfindingNode parent;
        public Vector3 position;

        public int g;
        public int h;
        public int f;

        public PathfindingNode(Vector3 position, PathfindingNode parent = null)
        {
            this.position = position;
            this.parent = parent;

            g = h = f = 0;
        }

        public override bool Equals(object o)
        {
            return position == ((PathfindingNode)o).position;
        }
    }
    /*
    public class PathfindingTree
    {
        public PathfindingTree Left = null;
        public PathfindingTree Right = null;

        public PathfindingNode Value;

        public PathfindingTree(PathfindingNode value)
        {
            Value = value;
        }




        public override string ToString()
        {
            if (Left == null && Right == null)
                return $"{Value}";
            else if (Left == null && Right != null)
                return $"{Value} ({Right})";
            else if (Left != null && Right == null)
                return $"{Value} ({Left})";
            else
                return $"{Value} ({Left} {Right})";
        }

        public void RemoveBranch(Branch branch)
        {
            if (branch == Branch.Left)
            {
                Left = null;
            } else
            {
                Right = null;
            }

        }

        public void Insert(PathfindingNode value)
        {
            PathfindingTree current = this;
            PathfindingTree parent = null;

            while (current != null)
            {
                parent = current;

                if (value < current.Value)
                    current = current.Left;
                else
                    current = current.Right;
            }

            if (value < parent.Value)
                parent.Left = new PathfindingTree(value);

        }
    }*/
}
