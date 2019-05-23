using System;
using FEM;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleFEM
{
    class Program
    {
        static public void Main(string[] args)
        {
            TriangularMesh triangularMesh = new TriangularMesh(0, 0, 1, 1);
            triangularMesh.CreateMesh(10, 10);
            var nodesCount = triangularMesh.Nodes.Count;
            //triangularMesh.Nodes.ForEach(item => Console.WriteLine($"{item.X} -- {item.Y}"));
            triangularMesh.CalculateMatrixForEachElement();

             double[,] globalMatrix = triangularMesh.CreateGlobalMatrix();
             double[] rightPart = triangularMesh.CreateRightPart();

            Console.WriteLine("\n Matrix");

            //for (int i = 0; i < nodesCount; ++i)
            //{
            //    for (int j = 0; j < nodesCount; ++j)
            //    {
            //        Console.Write(globalMatrix[i, j] + "    ");
            //    }

            //    Console.WriteLine();
            //}
            ////////////////////Console.WriteLine("\n\n\n\n");
            ////////////////////for (int j = 0; j < nodesCount; ++j)
            ////////////////////{
            ////////////////////    Console.WriteLine(rightPart[j]);
            ////////////////////}
            //Console.WriteLine("\n Right part");

            //for (int i = 0; i < nodesCount; ++i)
            //{
            //    Console.WriteLine(rightPart[i]);
            //}
            //////triangularMesh.CalculateMatrixForEachElement();
            //////for (int i = 0; i < triangularMesh.Matrixes.Count; ++i)
            //////{
            //////    for (int j = 0; j < 3; j++)
            //////    {
            //////        for (int k = 0; k < 3; ++k)
            //////        {
            //////            Console.Write(triangularMesh.Matrixes[i].Data[j][k] + "\t");
            //////        }

            //////        Console.WriteLine();
            //////    }

            //////    Console.WriteLine("\n\n");
            //////}

            var q = triangularMesh.GetQArray();

            //double[][] nodes;
            //double[] weights;

            //triangularMesh.InitTriangleNodes(5, out nodes, out weights);

            //for (int i = 0; i < nodes.Length; ++i)
            //{
            //    for (int j = 0; j < 2; ++j)
            //    {
            //        Console.Write(nodes[i][j] + "    ");
            //    }

            //    Console.WriteLine();
            //}

            //Console.WriteLine("\n\n\n");
            //for (int i = 0; i < weights.Length; ++i)
            //{
            //    Console.WriteLine(weights[i]);
            //}

            Console.WriteLine("\n\n\n");
            Console.WriteLine("MYMYMYMYMYMYMYMYMYMYMY");
            q.ToList();//.ForEach(item => Console.WriteLine(item));
            Console.WriteLine("\n\n");


            double[] result = new double[triangularMesh.Nodes.Count];
            for (int i = 0; i < triangularMesh.Nodes.Count; ++i)
            {
                result[i] = TriangularMesh.ext(triangularMesh.Nodes[i].X, triangularMesh.Nodes[i].Y);
            }

            Console.WriteLine("NOT MY");
            result.ToList();//.ForEach(item => Console.WriteLine(item));
            for (int i = 0; i < q.Length; ++i)
            {
                Console.WriteLine(q[i] + " ------ " + result[i]);
            }

            Console.Read();
        }
    }
}
