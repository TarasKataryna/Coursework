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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            TriangularMesh triangularMesh = new TriangularMesh(0,0,1,1);
            triangularMesh.CreateMesh(4, 3);
            var a = triangularMesh.GetQArray();
            double[] result = new double[triangularMesh.Nodes.Count];
            for(int i = 0; i < triangularMesh.Nodes.Count; ++i)
            {
                result[i] = TriangularMesh.ext(triangularMesh.Nodes[i].X, triangularMesh.Nodes[i].Y);
            }
            double[] result1 = new double[triangularMesh.Nodes.Count];
            for (int i = 0; i < triangularMesh.Nodes.Count; ++i)
            {
                result1[i] = Math.Abs(result[i]-a[i]);
            }
            foreach (var item in triangularMesh.Nodes)
            {
                chart1.Series["Series1"].Points.AddXY(item.X, item.Y);
            }
        }
    }

    public class NodesComparer : IComparer<Node>
    {
        public int Compare(Node first, Node second)
        {
            if (first.GlobalIndex > second.GlobalIndex) return 1;
            else if (first.GlobalIndex < second.GlobalIndex) return -1;
            else return 0;
        }
    }
}
