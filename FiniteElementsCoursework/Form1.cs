using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FEM;


namespace FiniteElementsCoursework
{
    public class NodesComparer : IComparer<Node>
    {
        public int Compare(Node first, Node second)
        {
            if (first.GlobalIndex > second.GlobalIndex) return 1;
            else if (first.GlobalIndex < second.GlobalIndex) return -1;
            else return 0;
        }
    }
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            TriangularMesh triangularMesh = new TriangularMesh(0,0,4,3);
            triangularMesh.CreateMesh(4, 3);
            triangularMesh.Nodes.Sort(new NodesComparer());
            foreach (var item in triangularMesh.Nodes)
            {
                chart1.Series["Series1"].Points.AddXY(item.X, item.Y);
            }
        }
    }
}
