using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEM
{

    public class Node
    {
        public bool IsBoundary { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        public int LocalIndex { get; set; }

        public int GlobalIndex { get; set; }

        public List<Node> LinkedNodes { get; set; }

        public Node()
        {

        }

        public Node(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Node(double x, double y, int globalIndex)
        {
            X = x;
            Y = y;
            GlobalIndex = globalIndex;
        }

        public Node(double x, double y, int localIndex, int globalIndex)
        {
            X = x;
            Y = y;
            LocalIndex = localIndex;
            GlobalIndex = globalIndex;
        }
    }
}
