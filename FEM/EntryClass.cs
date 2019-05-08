using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEM
{
    static class EntryClass
    {
        static public void Main(string[] args)
        {
            TriangularMesh triangularMesh = new TriangularMesh(0, 0, 1, 1);
            triangularMesh.CreateMesh(4, 3);
            var q = triangularMesh.GetQArray();
            Console.WriteLine("MYMYMYMYMYMYMYMYMYMYMY");
            q.ToList().ForEach(item=>Console.WriteLine(item));
            Console.WriteLine("\n\n");


            double[] result = new double[triangularMesh.Nodes.Count];
            for (int i = 0; i < triangularMesh.Nodes.Count; ++i)
            {
                result[i] = TriangularMesh.ext(triangularMesh.Nodes[i].X, triangularMesh.Nodes[i].Y);
            }

            Console.WriteLine("NOT MY");
            result.ToList().ForEach(item => Console.WriteLine(item));
            Console.Read();
        }
    }
}
