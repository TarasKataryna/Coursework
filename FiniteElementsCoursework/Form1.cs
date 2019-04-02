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
            TriangularMesh triangularMesh = new TriangularMesh(0,0,3,4);
            triangularMesh.CreateMesh(3,4);
            foreach (var item in triangularMesh.Nodes)
            {
                chart1.Series["Series1"].Points.AddXY(item.X, item.Y);
            }
        }
    }
}
