﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEM
{
    public class Element
    {
        public List<Node> Nodes { get; set; }

        public int Index { get; set; }

        public Node this[int index]
        {
            get
            {
                return Nodes.Where(item => item.LocalIndex == index).ToList().First();
            }
        }

        public Element()
        {
            Nodes = new List<Node>();
        }

        public Element(int index)
        {
            this.Index = index;
            Nodes = new List<Node>();
        }

    }
}
