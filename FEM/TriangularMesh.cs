﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MLApp;



namespace FEM
{
    public class TriangularMesh
    {
        public delegate double BaseLocalFunction(double[] alpha);

        public double X1 { get; set; }

        public double Y1 { get; set; }

        public double X2 { get; set; }

        public double Y2 { get; set; }

        public int InRow { get; set; }

        public int InCol { get; set; }

        public List<Node> Nodes { get; set; }

        public List<Element> FiniteElements { get; set; }

        public double Jakobian { get; set; }

        public List<LocalMatrix> Matrixes { get; set; }

        public List<double[]> LocalRightParts { get; set; }

        public BaseLocalFunction[] baseFunctions { get; set; }

        public BaseLocalFunction[][] baseFunctionsDerivative { get; set; }

        public TriangularMesh()

        {
            baseFunctions = new BaseLocalFunction[3];
            baseFunctions[0] = delegate (double[] alpha) { return 1 - alpha[0] - alpha[1]; };
            baseFunctions[1] = delegate (double[] alpha) { return alpha[0]; };
            baseFunctions[2] = delegate (double[] alpha) { return alpha[1]; };


            baseFunctionsDerivative = new BaseLocalFunction[3][];
            baseFunctionsDerivative[0] = new BaseLocalFunction[2];
            baseFunctionsDerivative[0][0] = delegate (double[] alpha) { return -1; };
            baseFunctionsDerivative[0][1] = delegate (double[] alpha) { return -1; };

            baseFunctionsDerivative[1] = new BaseLocalFunction[2];
            baseFunctionsDerivative[1][0] = delegate (double[] alpha) { return 1; };
            baseFunctionsDerivative[1][1] = delegate (double[] alpha) { return 0; };

            baseFunctionsDerivative[2] = new BaseLocalFunction[2];
            baseFunctionsDerivative[2][0] = delegate (double[] alpha) { return 0; };
            baseFunctionsDerivative[2][1] = delegate (double[] alpha) { return 1; };

        }

        public TriangularMesh(double x1, double y1, double x2, double y2)
        {
            this.X1 = x1;
            this.X2 = x2;
            this.Y1 = y1;
            this.Y2 = y2;

            baseFunctions = new BaseLocalFunction[3];
            baseFunctions[0] = delegate (double[] alpha) { return 1 - alpha[0] - alpha[1]; };
            baseFunctions[1] = delegate (double[] alpha) { return alpha[0]; };
            baseFunctions[2] = delegate (double[] alpha) { return alpha[1]; };


            baseFunctionsDerivative = new BaseLocalFunction[3][];
            baseFunctionsDerivative[0] = new BaseLocalFunction[2];
            baseFunctionsDerivative[0][0] = delegate (double[] alpha) { return -1; };
            baseFunctionsDerivative[0][1] = delegate (double[] alpha) { return -1; };

            baseFunctionsDerivative[1] = new BaseLocalFunction[2];
            baseFunctionsDerivative[1][0] = delegate (double[] alpha) { return 1; };
            baseFunctionsDerivative[1][1] = delegate (double[] alpha) { return 0; };

            baseFunctionsDerivative[2] = new BaseLocalFunction[2];
            baseFunctionsDerivative[2][0] = delegate (double[] alpha) { return 0; };
            baseFunctionsDerivative[2][1] = delegate (double[] alpha) { return 1; };

        }

        public void CreateMesh(int inRow, int inCol)
        {
            Nodes = new List<Node>();
            FiniteElements = new List<Element>();

            this.InRow = inRow;
            this.InCol = inCol;

            double elementHeight = (this.Y2 - this.Y1) / this.InCol;
            double elementWidth = (this.X2 - this.X1) / this.InRow;
            double elementHalfHeight = elementHeight / 2;
            double elementHalfWidth = elementWidth / 2;

            double x, y;
            int index = 0;

            //nodes
            for (int i = 0; i < this.InRow + 1; ++i)
            {
                x = this.X1 + (i * elementWidth);
                y = this.Y1;
                index = i * (this.InCol + 1) + i * this.InCol;
                for (int j = 0; j < this.InCol + 1; ++j)
                {
                    Node node = new Node(x, y, index);
                    y += elementHeight;
                    ++index;
                    if (i == 0 || i == (this.InRow)) node.IsBoundary = true;
                    if (j == 0 || j == (this.InCol)) node.IsBoundary = true;
                    Nodes.Add(node);
                }
            }

            for (int i = 0; i < this.InRow; ++i)
            {
                x = this.X1 + elementHalfWidth + (i * elementWidth);
                y = this.Y1 + elementHalfHeight;
                index = (i + 1) * (this.InCol + 1) + i * this.InCol;
                for (int j = 0; j < this.InCol; ++j)
                {
                    Node node = new Node(x, y, index);
                    Nodes.Add(node);
                    y += elementHeight;
                    ++index;
                }
            }


            //finite elements
            //  |\
            //  | \
            //  | /
            //  |/
            index = 0;
            for (int i = 0; i < this.InRow; ++i)
            {
                index = i * this.InCol * 4;
                for (int j = 0; j < this.InCol; ++j)
                {
                    Element element = new Element();
                    element.Index = index;
                    int nodeIndex = i * (this.InCol + 1) + i * this.InCol + j;
                    element.Nodes.Add(Nodes.Where(el => el.GlobalIndex == nodeIndex).First());
                    element.Nodes.Add(Nodes.Where(el => el.GlobalIndex == (nodeIndex + 1)).First());
                    element.Nodes.Add(Nodes.Where(el => el.GlobalIndex == (nodeIndex + 1 + this.InCol)).First());
                    FiniteElements.Add(element);
                    ++index;
                }
            }

            //     /|
            //    / | 
            //    \ |
            //     \|
            for (int i = 0; i < this.InRow; ++i)
            {
                index = i * this.InCol * 4 + this.InCol * 3;
                for (int j = 0; j < this.InCol; ++j)
                {
                    Element element = new Element();
                    element.Index = index;
                    int nodeIndex = (i + 1) * (this.InCol + 1) + (i + 1) * this.InCol + j;
                    element.Nodes.Add(Nodes.Where(el => el.GlobalIndex == nodeIndex).First());
                    element.Nodes.Add(Nodes.Where(el => el.GlobalIndex == nodeIndex + 1).First());
                    element.Nodes.Add(Nodes.Where(el => el.GlobalIndex == nodeIndex - this.InCol).First());
                    FiniteElements.Add(element);
                    ++index;
                }
            }

            //    /\
            //   /  \
            //  /____\
            for (int i = 0; i < this.InRow; ++i)
            {
                index = i * this.InCol * 4 + this.InCol;
                for (int j = 0; j < this.InCol; ++j)
                {
                    Element element = new Element();
                    element.Index = index;
                    int nodeIndex = (i + 1) * (this.InCol + 1) + (i + 1) * this.InCol + j;
                    element.Nodes.Add(Nodes.Where(el => el.GlobalIndex == nodeIndex).First());
                    element.Nodes.Add(Nodes.Where(el => el.GlobalIndex == nodeIndex - this.InCol).First());
                    element.Nodes.Add(Nodes.Where(el => el.GlobalIndex == nodeIndex - 2 * this.InCol - 1).First());
                    FiniteElements.Add(element);
                    index += 2;
                }
            }

            for (int i = 0; i < this.InRow; ++i)
            {
                index = i * this.InCol * 4 + this.InCol + 1;
                for (int j = 0; j < this.InCol; ++j)
                {
                    Element element = new Element();
                    element.Index = index;
                    int nodeIndex = (i + 1) * (this.InCol + 1) + (i + 1) * this.InCol + j - this.InCol;
                    element.Nodes.Add(Nodes.Where(el => el.GlobalIndex == nodeIndex).First());
                    element.Nodes.Add(Nodes.Where(el => el.GlobalIndex == nodeIndex - this.InCol).First());
                    element.Nodes.Add(Nodes.Where(el => el.GlobalIndex == nodeIndex + this.InCol + 1).First());
                    FiniteElements.Add(element);
                    index += 2;
                }
            }


            //find linked nodes
            /*int maxNodes = Nodes.Count;
            int nodesInCol = this.InCol + 1 + this.InCol;

            for (int i = 0; i < this.InRow + 1; ++i)
            {
                x = this.X1 + (i * elementWidth);
                y = this.Y1;
                index = i * (this.InCol + 1) + i * this.InCol;
                for (int j = 0; j < this.InCol + 1; ++j)
                {
                    Node node = Nodes.Where(n => n.GlobalIndex == index).First();
                    if (node.IsBoundary)
                    {

                    }
                    else
                    {
                        node.LinkedNodes.Add(Nodes.Where(n => n.GlobalIndex == (index + 1)).First());
                        node.LinkedNodes.Add(Nodes.Where(n => n.GlobalIndex == (index - 1)).First());
                        node.LinkedNodes.Add(Nodes.Where(n => n.GlobalIndex == (index + nodesInCol)).First());
                        node.LinkedNodes.Add(Nodes.Where(n => n.GlobalIndex == (index - nodesInCol)).First());
                    }
                    ++index;
                }
            }*/

        }

        public double CalculateJakobian(Node first, Node second, Node third)
        {
            return 0.0;

        }

        public double CalculateElementOfMatrix(int PhiUIndex, int PhiVindex, double[][] nodes, double[] weights, Element element)
        {
            double result = 0;

            double[] getArray = CalculateDePhiDeXDeFiDeY(PhiUIndex, element);
            double[] getArrayV = CalculateDePhiDeXDeFiDeY(PhiVindex, element);

            for (int i = 0; i < nodes.Length; ++i)
            {
                result += (M1(nodes[i][0], nodes[i][1]) * getArray[0] * getArrayV[0]
                    + M2(nodes[i][0], nodes[i][1]) * getArray[1] * getArrayV[1]
                    + B1(nodes[i][0], nodes[i][1]) * baseFunctions[PhiVindex](nodes[i]) * getArray[0]
                    + B2(nodes[i][0], nodes[i][1]) * baseFunctions[PhiVindex](nodes[i]) * getArray[1]
                    + L(nodes[i][0], nodes[i][1]) * baseFunctions[PhiUIndex](nodes[i]) * baseFunctions[PhiVindex](nodes[i])) * weights[i];
            }

            //Console.WriteLine($"Element --- {result}\n");
            return result;
        }

        public double CalculateElementOfRightPart(int PhiVIndex, double[][] nodes, double[] weights)
        {
            double result = 0;
            for (int i = 0; i < nodes.Length; ++i)
            {
                result += F(nodes[i][0], nodes[i][1]) * baseFunctions[PhiVIndex](nodes[i]) * weights[i];
            }
            return result;
        }

        public void CalculateMatrixForEachElement()
        {
            int localInRow = 3;
            int localInCol = 3;

            Matrixes = new List<LocalMatrix>();

            double[][] nodes;
            double[] weights;

            InitTriangleNodes(5, out nodes, out weights);

            LocalRightParts = new List<double[]>();

            for (int i = 0; i < FiniteElements.Count; ++i)
            {
                LocalMatrix localMatrix = new LocalMatrix(r: localInRow, c: localInCol);

                /*double jakobian = (FiniteElements[i].Nodes
                    .Where(n => n.LocalIndex == 0).First().X - FiniteElements[i].Nodes
                    .Where(n => n.LocalIndex == 1).First().X) * (FiniteElements[i].Nodes.Where(n => n.LocalIndex == 0).First().Y - FiniteElements[i].Nodes.Where(n => n.LocalIndex == 2).First().Y);
                jakobian -= (FiniteElements[i].Nodes
                    .Where(n => n.LocalIndex == 0).First().X - FiniteElements[i].Nodes
                    .Where(n => n.LocalIndex == 2).First().X) * (FiniteElements[i].Nodes.Where(n => n.LocalIndex == 0).First().Y - FiniteElements[i].Nodes.Where(n => n.LocalIndex == 1).First().Y);
                    */
                double jakobian = (FiniteElements[i].Nodes[0].X - FiniteElements[i].Nodes[1].X) * (FiniteElements[i].Nodes[0].Y - FiniteElements[i].Nodes[2].Y);
                jakobian -= (FiniteElements[i].Nodes[0].X - FiniteElements[i].Nodes[2].X) * (FiniteElements[i].Nodes[0].Y - FiniteElements[i].Nodes[1].Y);

                localMatrix.Index = FiniteElements[i].Index;

                double[] a = new double[3];

                for (int j = 0; j < localInCol; ++j)
                {
                    for (int k = 0; k < localInRow; ++k)
                    {
                        localMatrix.Data[j][k] = CalculateElementOfMatrix(k, j, nodes, weights, FiniteElements[i]) * jakobian;
                    }
                    a[j] = CalculateElementOfRightPart(j, nodes, weights) * jakobian;
                }

                Matrixes.Add(localMatrix);

                LocalRightParts.Add(a);
            }
        }

        public double[][] CreateGlobalMatrix1()
        {
            double[][] globalMatrix = new double[Nodes.Count][];
            for (int i = 0; i < Nodes.Count; ++i)
            {
                globalMatrix[i] = new double[Nodes.Count];
            }
            for (int i = 0; i < Nodes.Count; ++i)
            {
                for (int j = 0; j < Nodes.Count; ++j)
                {
                    globalMatrix[i][j] = 0;
                }
            }

            for (int i = 0; i < FiniteElements.Count; ++i)
            {
                for (int row = 0; row < 3; ++row)
                {
                    for (int col = 0; col < 3; ++col)
                    {
                        globalMatrix[FiniteElements[i].Nodes[row].GlobalIndex][FiniteElements[i].Nodes[col].GlobalIndex] += Matrixes[i].Data[row][col];
                    }
                }
            }

            for (int i = 0; i < Nodes.Count; ++i)
            {
                if (Nodes.Where(node => node.GlobalIndex == i).FirstOrDefault().IsBoundary)
                    for (int j = 0; j < Nodes.Count; ++j)
                    {
                        if (j == i)
                            globalMatrix[i][j] = 1;
                        else
                            globalMatrix[i][j] = 0;
                    }
            }

            return globalMatrix;
        }

        public double[,] CreateGlobalMatrix()
        {
            double[,] globalMatrix = new double[Nodes.Count, Nodes.Count];
            for (int i = 0; i < Nodes.Count; ++i)
            {
                for (int j = 0; j < Nodes.Count; ++j)
                {
                    globalMatrix[i, j] = 0;
                }
            }
            double a = 0;
            for (int i = 0; i < FiniteElements.Count; ++i)
            {
                for (int row = 0; row < 3; ++row)
                {
                    for (int col = 0; col < 3; ++col)
                    {
                        globalMatrix[FiniteElements[i].Nodes[row].GlobalIndex, FiniteElements[i].Nodes[col].GlobalIndex] += Matrixes[i].Data[row][col];
                    }
                }
            }

            for (int i = 0; i < Nodes.Count; ++i)
            {
                if (Nodes.Where(node => node.GlobalIndex == i).FirstOrDefault().IsBoundary)
                    for (int j = 0; j < Nodes.Count; ++j)
                    {
                        if (j == i)
                            globalMatrix[i, j] = 1000000000000000;
                    }
            }

            return globalMatrix;
        }

        public double[] CreateRightPart()
        {
            double[] rightPart = new double[Nodes.Count];

            for (int i = 0; i < rightPart.Length; ++i)
            {
                rightPart[0] = 0;
            }

            for (int i = 0; i < FiniteElements.Count; ++i)
            {
                for (int row = 0; row < 3; ++row)
                {
                    rightPart[FiniteElements[i].Nodes[row].GlobalIndex] += LocalRightParts[i][row];
                }
            }

            return rightPart;
        }

        public double M1(double x, double y)
        {
            return 0.1;
        }

        public double M2(double x, double y)
        {
            return 0.1;
        }

        public double B1(double x, double y)
        {
            return 2;
        }

        public double B2(double x, double y)
        {
            return 3;
        }

        public double L(double x, double y)
        {
            return 0;
        }

        public double F(double x, double y)
        {
            return 2 * ((0.02 - 3 * y) * (GGG(2, x) - x) - GGG(3, y) + y * y);
        }

        public double[] GetQArray()
        {
            double[] q = new double[Nodes.Count];

            CalculateMatrixForEachElement();

            double[][] globalMatrix1 = CreateGlobalMatrix1();
            double[,] globalMatrix = CreateGlobalMatrix();

            double[] rightPart = CreateRightPart();

            //GausMethod gau = new GausMethod((uint)Nodes.Count, (uint)Nodes.Count);
            //gau.Matrix = globalMatrix1;
            //gau.RightPart = rightPart;
            //Console.WriteLine($"\n\n{gau.SolveMatrix()}");
            //q = gau.Answer;

            //alglib.densesolverreport report = new alglib.densesolverreport();
            //int info;


            //alglib.rmatrixsolve(globalMatrix, Nodes.Count, rightPart, out info, out report, out q);

            LinearSystem system = new LinearSystem(globalMatrix, rightPart, 0.0000000000000000001);
            system.GaussSolve();
            q = system.XVector;
            return q;
        }

        static public double ext(double x, double y)
        {
            var a = x * y * y - y * y * GGG(2, x) + GGG(3, y) * (GGG(2, x) - x);
            return a;
        }
        static public double GGG(double x, double y)
        {
            return Math.Exp(180.3 * x * (y - 1));
        }

        public void InitTriangleNodes(int order, out double[][] gtn, out double[] gtw)
        {
            gtn = new double[0][];
            gtw = new double[0];
            int order_num = dunavant_order_num(order);
            if (order_num == -1) return;

            gtn = new double[order_num][];
            for (int ni = 0; ni < order_num; ni++)
                gtn[ni] = new double[2];
            gtw = new double[order_num];

            dunavant_rule(order, order_num, ref gtn, ref gtw);
            for (int ni = 0; ni < order_num; ni++)
                gtw[ni] /= 2;

        }

        #region dunavant

        /// <summary>
        ///  Purpose:
        ///
        ///    DUNAVANT_ORDER_NUM returns the order of a Dunavant rule for the triangle.
        ///
        ///  Licensing:
        ///
        ///    This code is distributed under the GNU LGPL license.
        ///
        ///  Modified:
        ///
        ///    11 December 2006
        ///
        ///  Author:
        ///
        ///    John Burkardt
        ///
        ///  Reference:
        ///
        ///    David Dunavant,
        ///    High Degree Efficient Symmetrical Gaussian Quadrature Rules
        ///    for the Triangle,
        ///    International Journal for Numerical Methods in Engineering,
        ///    Volume 21, 1985, pages 1129-1148.
        ///
        ///    James Lyness, Dennis Jespersen,
        ///    Moderate Degree Symmetric Quadrature Rules for the Triangle,
        ///    Journal of the Institute of Mathematics and its Applications,
        ///    Volume 15, Number 1, February 1975, pages 19-32.
        /// </summary>
        /// <param name="rule">int RULE, the index of the rule.</param>
        /// <returns>int DUNAVANT_ORDER_NUM, the order (number of points) of the rule</returns>
        private int dunavant_order_num(int rule)
        {
            int order;
            int order_num;
            int[] suborder;
            int suborder_num;

            suborder_num = dunavant_suborder_num(rule);
            if (suborder_num == -1) return -1;

            suborder = dunavant_suborder(rule, suborder_num);

            order_num = 0;
            for (order = 0; order < suborder_num; order++)
            {
                order_num = order_num + suborder[order];
            }
            return order_num;
        }

        /// <summary>
        ///  Purpose:
        ///
        ///    DUNAVANT_SUBORDER_NUM returns the number of suborders for a Dunavant rule.
        ///
        ///  Licensing:
        ///
        ///    This code is distributed under the GNU LGPL license.
        ///
        ///  Modified:
        ///
        ///    11 December 2006
        ///
        ///  Author:
        ///
        ///    John Burkardt
        ///
        ///  Reference:
        ///
        ///    David Dunavant,
        ///    High Degree Efficient Symmetrical Gaussian Quadrature Rules
        ///    for the Triangle,
        ///    International Journal for Numerical Methods in Engineering,
        ///    Volume 21, 1985, pages 1129-1148.
        ///
        ///    James Lyness, Dennis Jespersen,
        ///    Moderate Degree Symmetric Quadrature Rules for the Triangle,
        ///    Journal of the Institute of Mathematics and its Applications,
        ///    Volume 15, Number 1, February 1975, pages 19-32.
        /// </summary>
        /// <param name="rule">int RULE, the index of the rule.</param>
        /// <returns>int DUNAVANT_SUBORDER_NUM, the number of suborders of the rule.</returns>
        private int dunavant_suborder_num(int rule)
        {
            int suborder_num;

            switch (rule)
            {
                case 1:
                    suborder_num = 1;
                    break;
                case 2:
                    suborder_num = 1;
                    break;
                case 3:
                    suborder_num = 2;
                    break;
                case 4:
                    suborder_num = 2;
                    break;
                case 5:
                    suborder_num = 3;
                    break;
                case 6:
                    suborder_num = 3;
                    break;
                case 7:
                    suborder_num = 4;
                    break;
                case 8:
                    suborder_num = 5;
                    break;
                case 9:
                    suborder_num = 6;
                    break;
                case 10:
                    suborder_num = 6;
                    break;
                case 11:
                    suborder_num = 7;
                    break;
                case 12:
                    suborder_num = 8;
                    break;
                case 13:
                    suborder_num = 10;
                    break;
                case 14:
                    suborder_num = 10;
                    break;
                case 15:
                    suborder_num = 11;
                    break;
                case 16:
                    suborder_num = 13;
                    break;
                case 17:
                    suborder_num = 15;
                    break;
                case 18:
                    suborder_num = 17;
                    break;
                case 19:
                    suborder_num = 17;
                    break;
                case 20:
                    suborder_num = 19;
                    break;
                default:
                    suborder_num = -1;
                    //throw new Exception("DUNAVANT_SUBORDER_NUM - Fatal error!\n" +
                    //"  Illegal RULE = " + rule + "\n");
                    break;
            }

            return suborder_num;
        }

        /// <summary>
        ///  Purpose:
        ///
        ///    DUNAVANT_SUBORDER returns the suborders for a Dunavant rule.
        ///
        ///  Licensing:
        ///
        ///    This code is distributed under the GNU LGPL license.
        ///
        ///  Modified:
        ///
        ///    11 December 2006
        ///
        ///  Author:
        ///
        ///    John Burkardt
        ///
        ///  Reference:
        ///
        ///    David Dunavant,
        ///    High Degree Efficient Symmetrical Gaussian Quadrature Rules
        ///    for the Triangle,
        ///    International Journal for Numerical Methods in Engineering,
        ///    Volume 21, 1985, pages 1129-1148.
        ///
        ///    James Lyness, Dennis Jespersen,
        ///    Moderate Degree Symmetric Quadrature Rules for the Triangle,
        ///    Journal of the Institute of Mathematics and its Applications,
        ///    Volume 15, Number 1, February 1975, pages 19-32.
        /// </summary>
        /// <param name="rule">int RULE, the index of the rule.</param>
        /// <param name="suborder_num">int SUBORDER_NUM, the number of suborders of the rule.</param>
        /// <returns>int DUNAVANT_SUBORDER[SUBORDER_NUM], the suborders of the rule.</returns>
        private int[] dunavant_suborder(int rule, int suborder_num)
        {
            int[] suborder;

            suborder = new int[suborder_num];

            switch (rule)
            {
                case 1:
                    suborder[0] = 1;
                    break;
                case 2:
                    suborder[0] = 3;
                    break;
                case 3:
                    suborder[0] = 1;
                    suborder[1] = 3;
                    break;
                case 4:
                    suborder[0] = 3;
                    suborder[1] = 3;
                    break;
                case 5:
                    suborder[0] = 1;
                    suborder[1] = 3;
                    suborder[2] = 3;
                    break;
                case 6:
                    suborder[0] = 3;
                    suborder[1] = 3;
                    suborder[2] = 6;
                    break;
                case 7:
                    suborder[0] = 1;
                    suborder[1] = 3;
                    suborder[2] = 3;
                    suborder[3] = 6;
                    break;
                case 8:
                    suborder[0] = 1;
                    suborder[1] = 3;
                    suborder[2] = 3;
                    suborder[3] = 3;
                    suborder[4] = 6;
                    break;
                case 9:
                    suborder[0] = 1;
                    suborder[1] = 3;
                    suborder[2] = 3;
                    suborder[3] = 3;
                    suborder[4] = 3;
                    suborder[5] = 6;
                    break;
                case 10:
                    suborder[0] = 1;
                    suborder[1] = 3;
                    suborder[2] = 3;
                    suborder[3] = 6;
                    suborder[4] = 6;
                    suborder[5] = 6;
                    break;
                case 11:
                    suborder[0] = 3;
                    suborder[1] = 3;
                    suborder[2] = 3;
                    suborder[3] = 3;
                    suborder[4] = 3;
                    suborder[5] = 6;
                    suborder[6] = 6;
                    break;
                case 12:
                    suborder[0] = 3;
                    suborder[1] = 3;
                    suborder[2] = 3;
                    suborder[3] = 3;
                    suborder[4] = 3;
                    suborder[5] = 6;
                    suborder[6] = 6;
                    suborder[7] = 6;
                    break;
                case 13:
                    suborder[0] = 1;
                    suborder[1] = 3;
                    suborder[2] = 3;
                    suborder[3] = 3;
                    suborder[4] = 3;
                    suborder[5] = 3;
                    suborder[6] = 3;
                    suborder[7] = 6;
                    suborder[8] = 6;
                    suborder[9] = 6;
                    break;
                case 14:
                    suborder[0] = 3;
                    suborder[1] = 3;
                    suborder[2] = 3;
                    suborder[3] = 3;
                    suborder[4] = 3;
                    suborder[5] = 3;
                    suborder[6] = 6;
                    suborder[7] = 6;
                    suborder[8] = 6;
                    suborder[9] = 6;
                    break;
                case 15:
                    suborder[0] = 3;
                    suborder[1] = 3;
                    suborder[2] = 3;
                    suborder[3] = 3;
                    suborder[4] = 3;
                    suborder[5] = 3;
                    suborder[6] = 6;
                    suborder[7] = 6;
                    suborder[8] = 6;
                    suborder[9] = 6;
                    suborder[10] = 6;
                    break;
                case 16:
                    suborder[0] = 1;
                    suborder[1] = 3;
                    suborder[2] = 3;
                    suborder[3] = 3;
                    suborder[4] = 3;
                    suborder[5] = 3;
                    suborder[6] = 3;
                    suborder[7] = 3;
                    suborder[8] = 6;
                    suborder[9] = 6;
                    suborder[10] = 6;
                    suborder[11] = 6;
                    suborder[12] = 6;
                    break;
                case 17:
                    suborder[0] = 1;
                    suborder[1] = 3;
                    suborder[2] = 3;
                    suborder[3] = 3;
                    suborder[4] = 3;
                    suborder[5] = 3;
                    suborder[6] = 3;
                    suborder[7] = 3;
                    suborder[8] = 3;
                    suborder[9] = 6;
                    suborder[10] = 6;
                    suborder[11] = 6;
                    suborder[12] = 6;
                    suborder[13] = 6;
                    suborder[14] = 6;
                    break;
                case 18:
                    suborder[0] = 1;
                    suborder[1] = 3;
                    suborder[2] = 3;
                    suborder[3] = 3;
                    suborder[4] = 3;
                    suborder[5] = 3;
                    suborder[6] = 3;
                    suborder[7] = 3;
                    suborder[8] = 3;
                    suborder[9] = 3;
                    suborder[10] = 6;
                    suborder[11] = 6;
                    suborder[12] = 6;
                    suborder[13] = 6;
                    suborder[14] = 6;
                    suborder[15] = 6;
                    suborder[16] = 6;
                    break;
                case 19:
                    suborder[0] = 1;
                    suborder[1] = 3;
                    suborder[2] = 3;
                    suborder[3] = 3;
                    suborder[4] = 3;
                    suborder[5] = 3;
                    suborder[6] = 3;
                    suborder[7] = 3;
                    suborder[8] = 3;
                    suborder[9] = 6;
                    suborder[10] = 6;
                    suborder[11] = 6;
                    suborder[12] = 6;
                    suborder[13] = 6;
                    suborder[14] = 6;
                    suborder[15] = 6;
                    suborder[16] = 6;
                    break;
                case 20:
                    suborder[0] = 1;
                    suborder[1] = 3;
                    suborder[2] = 3;
                    suborder[3] = 3;
                    suborder[4] = 3;
                    suborder[5] = 3;
                    suborder[6] = 3;
                    suborder[7] = 3;
                    suborder[8] = 3;
                    suborder[9] = 3;
                    suborder[10] = 3;
                    suborder[11] = 6;
                    suborder[12] = 6;
                    suborder[13] = 6;
                    suborder[14] = 6;
                    suborder[15] = 6;
                    suborder[16] = 6;
                    suborder[17] = 6;
                    suborder[18] = 6;
                    break;
                default:
                    throw new Exception("DUNAVANT_SUBORDER - Fatal error!\n" +
                        "  Illegal RULE = " + rule + "\n");
            }

            return suborder;
        }

        /// <summary>
        ///  Purpose:
        ///
        ///    DUNAVANT_RULE returns the points and weights of a Dunavant rule.
        ///
        ///  Licensing:
        ///
        ///    This code is distributed under the GNU LGPL license.
        ///
        ///  Modified:
        ///
        ///    11 December 2006
        ///
        ///  Author:
        ///
        ///    John Burkardt
        ///
        ///  Reference:
        ///
        ///    David Dunavant,
        ///    High Degree Efficient Symmetrical Gaussian Quadrature Rules
        ///    for the Triangle,
        ///    International Journal for Numerical Methods in Engineering,
        ///    Volume 21, 1985, pages 1129-1148.
        ///
        ///    James Lyness, Dennis Jespersen,
        ///    Moderate Degree Symmetric Quadrature Rules for the Triangle,
        ///    Journal of the Institute of Mathematics and its Applications,
        ///    Volume 15, Number 1, February 1975, pages 19-32.
        /// </summary>
        /// <param name="rule">int RULE, the index of the rule.</param>
        /// <param name="order_num">int ORDER_NUM, the order (number of points) of the rule.</param>
        /// <param name="xy">double XY[2*ORDER_NUM], the points of the rule.</param>
        /// <param name="w">double W[ORDER_NUM], the weights of the rule.</param>
        private void dunavant_rule(int rule, int order_num, ref double[][] n, ref double[] w)
        {
            int k;
            int o;
            int s;
            int[] suborder;
            int suborder_num;
            double[] suborder_w;
            double[] suborder_xyz;
            //
            //  Get the suborder information.
            //
            suborder_num = dunavant_suborder_num(rule);

            suborder_xyz = new double[3 * suborder_num];
            suborder_w = new double[suborder_num];

            suborder = dunavant_suborder(rule, suborder_num);

            dunavant_subrule(rule, suborder_num, ref suborder_xyz, ref suborder_w);
            //
            //  Expand the suborder information to a full order rule.
            //
            o = 0;

            for (s = 0; s < suborder_num; s++)
            {
                switch (suborder[s])
                {
                    case 1:
                        n[o][0] = suborder_xyz[0 + s * 3];
                        n[o][1] = suborder_xyz[1 + s * 3];
                        w[o] = suborder_w[s];
                        o++;
                        break;
                    case 3:
                        for (k = 0; k < 3; k++)
                        {
                            n[o][0] = suborder_xyz[i4_wrap(k, 0, 2) + s * 3];
                            n[o][1] = suborder_xyz[i4_wrap(k + 1, 0, 2) + s * 3];
                            w[o] = suborder_w[s];
                            o++;
                        }
                        break;
                    case 6:
                        for (k = 0; k < 3; k++)
                        {
                            n[o][0] = suborder_xyz[i4_wrap(k, 0, 2) + s * 3];
                            n[o][1] = suborder_xyz[i4_wrap(k + 1, 0, 2) + s * 3];
                            w[o] = suborder_w[s];
                            o++;
                        }
                        for (k = 0; k < 3; k++)
                        {
                            n[o][0] = suborder_xyz[i4_wrap(k + 1, 0, 2) + s * 3];
                            n[o][1] = suborder_xyz[i4_wrap(k, 0, 2) + s * 3];
                            w[o] = suborder_w[s];
                            o++;
                        }
                        break;
                    default:
                        throw new Exception("DUNAVANT_RULE - Fatal error!\n;" +
                            "  Illegal SUBORDER(" + s + ") = " + suborder[s] + "\n");
                }
            }
        }

        /// <summary>
        /// Purpose:
        ///
        ///    DUNAVANT_SUBRULE returns a compressed Dunavant rule.
        ///
        ///  Licensing:
        ///
        ///    This code is distributed under the GNU LGPL license.
        ///
        ///  Modified:
        ///
        ///    11 December 2006
        ///
        ///  Author:
        ///
        ///    John Burkardt
        ///
        ///  Reference:
        ///
        ///    David Dunavant,
        ///    High Degree Efficient Symmetrical Gaussian Quadrature Rules
        ///    for the Triangle,
        ///    International Journal for Numerical Methods in Engineering,
        ///    Volume 21, 1985, pages 1129-1148.
        ///
        ///    James Lyness, Dennis Jespersen,
        ///    Moderate Degree Symmetric Quadrature Rules for the Triangle,
        ///    Journal of the Institute of Mathematics and its Applications,
        ///    Volume 15, Number 1, February 1975, pages 19-32.
        /// </summary>
        /// <param name="rule">int RULE, the index of the rule.</param>
        /// <param name="suborder_num">int SUBORDER_NUM, the number of suborders of the rule.</param>
        /// <param name="suborder_xyz">double SUBORDER_XYZ[3*SUBORDER_NUM], the barycentric coordinates of the abscissas.</param>
        /// <param name="suborder_w">double SUBORDER_W[SUBORDER_NUM], the suborder weights.</param>
        private void dunavant_subrule(int rule, int suborder_num, ref double[] suborder_xyz, ref double[] suborder_w)
        {
            switch (rule)
            {
                case 1:
                    dunavant_subrule_01(suborder_num, ref suborder_xyz, ref suborder_w);
                    break;
                case 2:
                    dunavant_subrule_02(suborder_num, ref suborder_xyz, ref suborder_w);
                    break;
                case 3:
                    dunavant_subrule_03(suborder_num, ref suborder_xyz, ref suborder_w);
                    break;
                case 4:
                    dunavant_subrule_04(suborder_num, ref suborder_xyz, ref suborder_w);
                    break;
                case 5:
                    dunavant_subrule_05(suborder_num, ref suborder_xyz, ref suborder_w);
                    break;
                case 6:
                    dunavant_subrule_06(suborder_num, ref suborder_xyz, ref suborder_w);
                    break;
                case 7:
                    dunavant_subrule_07(suborder_num, ref suborder_xyz, ref suborder_w);
                    break;
                case 8:
                    dunavant_subrule_08(suborder_num, ref suborder_xyz, ref suborder_w);
                    break;
                case 9:
                    dunavant_subrule_09(suborder_num, ref suborder_xyz, ref suborder_w);
                    break;
                case 10:
                    dunavant_subrule_10(suborder_num, ref suborder_xyz, ref suborder_w);
                    break;
                case 11:
                    dunavant_subrule_11(suborder_num, ref suborder_xyz, ref suborder_w);
                    break;
                case 12:
                    dunavant_subrule_12(suborder_num, ref suborder_xyz, ref suborder_w);
                    break;
                case 13:
                    dunavant_subrule_13(suborder_num, ref suborder_xyz, ref suborder_w);
                    break;
                case 14:
                    dunavant_subrule_14(suborder_num, ref suborder_xyz, ref suborder_w);
                    break;
                case 15:
                    dunavant_subrule_15(suborder_num, ref suborder_xyz, ref suborder_w);
                    break;
                case 16:
                    dunavant_subrule_16(suborder_num, ref suborder_xyz, ref suborder_w);
                    break;
                case 17:
                    dunavant_subrule_17(suborder_num, ref suborder_xyz, ref suborder_w);
                    break;
                case 18:
                    dunavant_subrule_18(suborder_num, ref suborder_xyz, ref suborder_w);
                    break;
                case 19:
                    dunavant_subrule_19(suborder_num, ref suborder_xyz, ref suborder_w);
                    break;
                case 20:
                    dunavant_subrule_20(suborder_num, ref suborder_xyz, ref suborder_w);
                    break;
                default:
                    throw new Exception("DUNAVANT_SUBRULE - Fatal error!\n" +
                   "  Illegal RULE = " + rule);
            }
        }
        private void dunavant_subrule_01(int suborder_num, ref double[] suborder_xyz, ref double[] suborder_w)
        {
            int s;
            double[] suborder_xy_rule_01 = {
      0.333333333333333,  0.333333333333333, 0.333333333333333
  };
            double[] suborder_w_rule_01 = {
      1.000000000000000
  };

            for (s = 0; s < suborder_num; s++)
            {
                suborder_xyz[0 + s * 3] = suborder_xy_rule_01[0 + s * 3];
                suborder_xyz[1 + s * 3] = suborder_xy_rule_01[1 + s * 3];
                suborder_xyz[2 + s * 3] = suborder_xy_rule_01[2 + s * 3];
            }

            for (s = 0; s < suborder_num; s++)
            {
                suborder_w[s] = suborder_w_rule_01[s];
            }

            return;
        }

        private void dunavant_subrule_02(int suborder_num, ref double[] suborder_xyz,
          ref double[] suborder_w)
        {
            int s;
            double[] suborder_xy_rule_02 = {
      0.666666666666667, 0.166666666666667, 0.166666666666667
  };
            double[] suborder_w_rule_02 = {
      0.333333333333333
  };

            for (s = 0; s < suborder_num; s++)
            {
                suborder_xyz[0 + s * 3] = suborder_xy_rule_02[0 + s * 3];
                suborder_xyz[1 + s * 3] = suborder_xy_rule_02[1 + s * 3];
                suborder_xyz[2 + s * 3] = suborder_xy_rule_02[2 + s * 3];
            }

            for (s = 0; s < suborder_num; s++)
            {
                suborder_w[s] = suborder_w_rule_02[s];
            }

            return;
        }

        private void dunavant_subrule_03(int suborder_num, ref double[] suborder_xyz,
          ref double[] suborder_w)
        {
            int s;
            double[] suborder_xy_rule_03 = {
      0.333333333333333, 0.333333333333333, 0.333333333333333,
      0.600000000000000, 0.200000000000000, 0.200000000000000
  };
            double[] suborder_w_rule_03 = {
      -0.562500000000000,
       0.520833333333333
  };

            for (s = 0; s < suborder_num; s++)
            {
                suborder_xyz[0 + s * 3] = suborder_xy_rule_03[0 + s * 3];
                suborder_xyz[1 + s * 3] = suborder_xy_rule_03[1 + s * 3];
                suborder_xyz[2 + s * 3] = suborder_xy_rule_03[2 + s * 3];
            }

            for (s = 0; s < suborder_num; s++)
            {
                suborder_w[s] = suborder_w_rule_03[s];
            }

            return;
        }

        private void dunavant_subrule_04(int suborder_num, ref double[] suborder_xyz,
          ref double[] suborder_w)
        {
            int s;
            double[] suborder_xy_rule_04 = {
      0.108103018168070, 0.445948490915965, 0.445948490915965,
      0.816847572980459, 0.091576213509771, 0.091576213509771
  };
            double[] suborder_w_rule_04 = {
      0.223381589678011,
      0.109951743655322
  };

            for (s = 0; s < suborder_num; s++)
            {
                suborder_xyz[0 + s * 3] = suborder_xy_rule_04[0 + s * 3];
                suborder_xyz[1 + s * 3] = suborder_xy_rule_04[1 + s * 3];
                suborder_xyz[2 + s * 3] = suborder_xy_rule_04[2 + s * 3];
            }

            for (s = 0; s < suborder_num; s++)
            {
                suborder_w[s] = suborder_w_rule_04[s];
            }

            return;
        }

        private void dunavant_subrule_05(int suborder_num, ref double[] suborder_xyz,
          ref double[] suborder_w)
        {
            int s;
            double[] suborder_xy_rule_05 = {
      0.333333333333333, 0.333333333333333, 0.333333333333333,
      0.059715871789770, 0.470142064105115, 0.470142064105115,
      0.797426985353087, 0.101286507323456, 0.101286507323456
  };
            double[] suborder_w_rule_05 = {
      0.225000000000000,
      0.132394152788506,
      0.125939180544827
  };

            for (s = 0; s < suborder_num; s++)
            {
                suborder_xyz[0 + s * 3] = suborder_xy_rule_05[0 + s * 3];
                suborder_xyz[1 + s * 3] = suborder_xy_rule_05[1 + s * 3];
                suborder_xyz[2 + s * 3] = suborder_xy_rule_05[2 + s * 3];
            }

            for (s = 0; s < suborder_num; s++)
            {
                suborder_w[s] = suborder_w_rule_05[s];
            }

            return;
        }

        private void dunavant_subrule_06(int suborder_num, ref double[] suborder_xyz,
          ref double[] suborder_w)
        {
            int s;
            double[] suborder_xy_rule_06 = {
      0.501426509658179, 0.249286745170910, 0.249286745170910,
      0.873821971016996, 0.063089014491502, 0.063089014491502,
      0.053145049844817, 0.310352451033784, 0.636502499121399
  };
            double[] suborder_w_rule_06 = {
      0.116786275726379,
      0.050844906370207,
      0.082851075618374
  };

            for (s = 0; s < suborder_num; s++)
            {
                suborder_xyz[0 + s * 3] = suborder_xy_rule_06[0 + s * 3];
                suborder_xyz[1 + s * 3] = suborder_xy_rule_06[1 + s * 3];
                suborder_xyz[2 + s * 3] = suborder_xy_rule_06[2 + s * 3];
            }

            for (s = 0; s < suborder_num; s++)
            {
                suborder_w[s] = suborder_w_rule_06[s];
            }

            return;
        }

        private void dunavant_subrule_07(int suborder_num, ref double[] suborder_xyz,
          ref double[] suborder_w)
        {
            int s;
            double[] suborder_xy_rule_07 = {
      0.333333333333333, 0.333333333333333, 0.333333333333333,
      0.479308067841920, 0.260345966079040, 0.260345966079040,
      0.869739794195568, 0.065130102902216, 0.065130102902216,
      0.048690315425316, 0.312865496004874, 0.638444188569810
  };
            double[] suborder_w_rule_07 = {
     -0.149570044467682,
      0.175615257433208,
      0.053347235608838,
      0.077113760890257
  };

            for (s = 0; s < suborder_num; s++)
            {
                suborder_xyz[0 + s * 3] = suborder_xy_rule_07[0 + s * 3];
                suborder_xyz[1 + s * 3] = suborder_xy_rule_07[1 + s * 3];
                suborder_xyz[2 + s * 3] = suborder_xy_rule_07[2 + s * 3];
            }

            for (s = 0; s < suborder_num; s++)
            {
                suborder_w[s] = suborder_w_rule_07[s];
            }

            return;
        }

        private void dunavant_subrule_08(int suborder_num, ref double[] suborder_xyz,
          ref double[] suborder_w)
        {
            int s;
            double[] suborder_xy_rule_08 = {
      0.333333333333333, 0.333333333333333, 0.333333333333333,
      0.081414823414554, 0.459292588292723, 0.459292588292723,
      0.658861384496480, 0.170569307751760, 0.170569307751760,
      0.898905543365938, 0.050547228317031, 0.050547228317031,
      0.008394777409958, 0.263112829634638, 0.728492392955404
  };
            double[] suborder_w_rule_08 = {
      0.144315607677787,
      0.095091634267285,
      0.103217370534718,
      0.032458497623198,
      0.027230314174435
  };

            for (s = 0; s < suborder_num; s++)
            {
                suborder_xyz[0 + s * 3] = suborder_xy_rule_08[0 + s * 3];
                suborder_xyz[1 + s * 3] = suborder_xy_rule_08[1 + s * 3];
                suborder_xyz[2 + s * 3] = suborder_xy_rule_08[2 + s * 3];
            }

            for (s = 0; s < suborder_num; s++)
            {
                suborder_w[s] = suborder_w_rule_08[s];
            }

            return;
        }

        private void dunavant_subrule_09(int suborder_num, ref double[] suborder_xyz,
          ref double[] suborder_w)
        {
            int s;
            double[] suborder_xy_rule_09 = {
      0.333333333333333, 0.333333333333333, 0.333333333333333,
      0.020634961602525, 0.489682519198738, 0.489682519198738,
      0.125820817014127, 0.437089591492937, 0.437089591492937,
      0.623592928761935, 0.188203535619033, 0.188203535619033,
      0.910540973211095, 0.044729513394453, 0.044729513394453,
      0.036838412054736, 0.221962989160766, 0.741198598784498
  };
            double[] suborder_w_rule_09 = {
      0.097135796282799,
      0.031334700227139,
      0.077827541004774,
      0.079647738927210,
      0.025577675658698,
      0.043283539377289
  };

            for (s = 0; s < suborder_num; s++)
            {
                suborder_xyz[0 + s * 3] = suborder_xy_rule_09[0 + s * 3];
                suborder_xyz[1 + s * 3] = suborder_xy_rule_09[1 + s * 3];
                suborder_xyz[2 + s * 3] = suborder_xy_rule_09[2 + s * 3];
            }

            for (s = 0; s < suborder_num; s++)
            {
                suborder_w[s] = suborder_w_rule_09[s];
            }

            return;
        }

        private void dunavant_subrule_10(int suborder_num, ref double[] suborder_xyz,
          ref double[] suborder_w)
        {
            int s;
            double[] suborder_xy_rule_10 = {
      0.333333333333333, 0.333333333333333, 0.333333333333333,
      0.028844733232685, 0.485577633383657, 0.485577633383657,
      0.781036849029926, 0.109481575485037, 0.109481575485037,
      0.141707219414880, 0.307939838764121, 0.550352941820999,
      0.025003534762686, 0.246672560639903, 0.728323904597411,
      0.009540815400299, 0.066803251012200, 0.923655933587500
  };
            double[] suborder_w_rule_10 = {
      0.090817990382754,
      0.036725957756467,
      0.045321059435528,
      0.072757916845420,
      0.028327242531057,
      0.009421666963733
  };

            for (s = 0; s < suborder_num; s++)
            {
                suborder_xyz[0 + s * 3] = suborder_xy_rule_10[0 + s * 3];
                suborder_xyz[1 + s * 3] = suborder_xy_rule_10[1 + s * 3];
                suborder_xyz[2 + s * 3] = suborder_xy_rule_10[2 + s * 3];
            }

            for (s = 0; s < suborder_num; s++)
            {
                suborder_w[s] = suborder_w_rule_10[s];
            }

            return;
        }

        private void dunavant_subrule_11(int suborder_num, ref double[] suborder_xyz,
          ref double[] suborder_w)
        {
            int s;
            double[] suborder_xy_rule_11 = {
     -0.069222096541517, 0.534611048270758, 0.534611048270758,
      0.202061394068290, 0.398969302965855, 0.398969302965855,
      0.593380199137435, 0.203309900431282, 0.203309900431282,
      0.761298175434837, 0.119350912282581, 0.119350912282581,
      0.935270103777448, 0.032364948111276, 0.032364948111276,
      0.050178138310495, 0.356620648261293, 0.593201213428213,
      0.021022016536166, 0.171488980304042, 0.807489003159792
  };
            double[] suborder_w_rule_11 = {
      0.000927006328961,
      0.077149534914813,
      0.059322977380774,
      0.036184540503418,
      0.013659731002678,
      0.052337111962204,
      0.020707659639141
  };

            for (s = 0; s < suborder_num; s++)
            {
                suborder_xyz[0 + s * 3] = suborder_xy_rule_11[0 + s * 3];
                suborder_xyz[1 + s * 3] = suborder_xy_rule_11[1 + s * 3];
                suborder_xyz[2 + s * 3] = suborder_xy_rule_11[2 + s * 3];
            }

            for (s = 0; s < suborder_num; s++)
            {
                suborder_w[s] = suborder_w_rule_11[s];
            }

            return;
        }

        private void dunavant_subrule_12(int suborder_num, ref double[] suborder_xyz,
          ref double[] suborder_w)
        {
            int s;
            double[] suborder_xy_rule_12 = {
      0.023565220452390, 0.488217389773805, 0.488217389773805,
      0.120551215411079, 0.439724392294460, 0.439724392294460,
      0.457579229975768, 0.271210385012116, 0.271210385012116,
      0.744847708916828, 0.127576145541586, 0.127576145541586,
      0.957365299093579, 0.021317350453210, 0.021317350453210,
      0.115343494534698, 0.275713269685514, 0.608943235779788,
      0.022838332222257, 0.281325580989940, 0.695836086787803,
      0.025734050548330, 0.116251915907597, 0.858014033544073
  };
            double[] suborder_w_rule_12 = {
      0.025731066440455,
      0.043692544538038,
      0.062858224217885,
      0.034796112930709,
      0.006166261051559,
      0.040371557766381,
      0.022356773202303,
      0.017316231108659
  };

            for (s = 0; s < suborder_num; s++)
            {
                suborder_xyz[0 + s * 3] = suborder_xy_rule_12[0 + s * 3];
                suborder_xyz[1 + s * 3] = suborder_xy_rule_12[1 + s * 3];
                suborder_xyz[2 + s * 3] = suborder_xy_rule_12[2 + s * 3];
            }

            for (s = 0; s < suborder_num; s++)
            {
                suborder_w[s] = suborder_w_rule_12[s];
            }

            return;
        }

        private void dunavant_subrule_13(int suborder_num, ref double[] suborder_xyz,
          ref double[] suborder_w)
        {
            int s;
            double[] suborder_xy_rule_13 = {
      0.333333333333333, 0.333333333333333, 0.333333333333333,
      0.009903630120591, 0.495048184939705, 0.495048184939705,
      0.062566729780852, 0.468716635109574, 0.468716635109574,
      0.170957326397447, 0.414521336801277, 0.414521336801277,
      0.541200855914337, 0.229399572042831, 0.229399572042831,
      0.771151009607340, 0.114424495196330, 0.114424495196330,
      0.950377217273082, 0.024811391363459, 0.024811391363459,
      0.094853828379579, 0.268794997058761, 0.636351174561660,
      0.018100773278807, 0.291730066734288, 0.690169159986905,
      0.022233076674090, 0.126357385491669, 0.851409537834241
  };
            double[] suborder_w_rule_13 = {
      0.052520923400802,
      0.011280145209330,
      0.031423518362454,
      0.047072502504194,
      0.047363586536355,
      0.031167529045794,
      0.007975771465074,
      0.036848402728732,
      0.017401463303822,
      0.015521786839045
  };

            for (s = 0; s < suborder_num; s++)
            {
                suborder_xyz[0 + s * 3] = suborder_xy_rule_13[0 + s * 3];
                suborder_xyz[1 + s * 3] = suborder_xy_rule_13[1 + s * 3];
                suborder_xyz[2 + s * 3] = suborder_xy_rule_13[2 + s * 3];
            }

            for (s = 0; s < suborder_num; s++)
            {
                suborder_w[s] = suborder_w_rule_13[s];
            }

            return;
        }

        private void dunavant_subrule_14(int suborder_num, ref double[] suborder_xyz,
          ref double[] suborder_w)
        {
            int s;
            double[] suborder_xy_rule_14 = {
      0.022072179275643, 0.488963910362179, 0.488963910362179,
      0.164710561319092, 0.417644719340454, 0.417644719340454,
      0.453044943382323, 0.273477528308839, 0.273477528308839,
      0.645588935174913, 0.177205532412543, 0.177205532412543,
      0.876400233818255, 0.061799883090873, 0.061799883090873,
      0.961218077502598, 0.019390961248701, 0.019390961248701,
      0.057124757403648, 0.172266687821356, 0.770608554774996,
      0.092916249356972, 0.336861459796345, 0.570222290846683,
      0.014646950055654, 0.298372882136258, 0.686980167808088,
      0.001268330932872, 0.118974497696957, 0.879757171370171
  };
            double[] suborder_w_rule_14 = {
      0.021883581369429,
      0.032788353544125,
      0.051774104507292,
      0.042162588736993,
      0.014433699669777,
      0.004923403602400,
      0.024665753212564,
      0.038571510787061,
      0.014436308113534,
      0.005010228838501
  };

            for (s = 0; s < suborder_num; s++)
            {
                suborder_xyz[0 + s * 3] = suborder_xy_rule_14[0 + s * 3];
                suborder_xyz[1 + s * 3] = suborder_xy_rule_14[1 + s * 3];
                suborder_xyz[2 + s * 3] = suborder_xy_rule_14[2 + s * 3];
            }

            for (s = 0; s < suborder_num; s++)
            {
                suborder_w[s] = suborder_w_rule_14[s];
            }

            return;
        }

        private void dunavant_subrule_15(int suborder_num, ref double[] suborder_xyz,
          ref double[] suborder_w)
        {
            int s;
            double[] suborder_xy_rule_15 = {
     -0.013945833716486, 0.506972916858243, 0.506972916858243,
      0.137187291433955, 0.431406354283023, 0.431406354283023,
      0.444612710305711, 0.277693644847144, 0.277693644847144,
      0.747070217917492, 0.126464891041254, 0.126464891041254,
      0.858383228050628, 0.070808385974686, 0.070808385974686,
      0.962069659517853, 0.018965170241073, 0.018965170241073,
      0.133734161966621, 0.261311371140087, 0.604954466893291,
      0.036366677396917, 0.388046767090269, 0.575586555512814,
     -0.010174883126571, 0.285712220049916, 0.724462663076655,
      0.036843869875878, 0.215599664072284, 0.747556466051838,
      0.012459809331199, 0.103575616576386, 0.883964574092416
  };
            double[] suborder_w_rule_15 = {
      0.001916875642849,
      0.044249027271145,
      0.051186548718852,
      0.023687735870688,
      0.013289775690021,
      0.004748916608192,
      0.038550072599593,
      0.027215814320624,
      0.002182077366797,
      0.021505319847731,
      0.007673942631049
  };

            for (s = 0; s < suborder_num; s++)
            {
                suborder_xyz[0 + s * 3] = suborder_xy_rule_15[0 + s * 3];
                suborder_xyz[1 + s * 3] = suborder_xy_rule_15[1 + s * 3];
                suborder_xyz[2 + s * 3] = suborder_xy_rule_15[2 + s * 3];
            }

            for (s = 0; s < suborder_num; s++)
            {
                suborder_w[s] = suborder_w_rule_15[s];
            }

            return;
        }

        private void dunavant_subrule_16(int suborder_num, ref double[] suborder_xyz,
          ref double[] suborder_w)
        {
            int s;
            double[] suborder_xy_rule_16 = {
      0.333333333333333, 0.333333333333333, 0.333333333333333,
      0.005238916103123, 0.497380541948438, 0.497380541948438,
      0.173061122901295, 0.413469438549352, 0.413469438549352,
      0.059082801866017, 0.470458599066991, 0.470458599066991,
      0.518892500060958, 0.240553749969521, 0.240553749969521,
      0.704068411554854, 0.147965794222573, 0.147965794222573,
      0.849069624685052, 0.075465187657474, 0.075465187657474,
      0.966807194753950, 0.016596402623025, 0.016596402623025,
      0.103575692245252, 0.296555596579887, 0.599868711174861,
      0.020083411655416, 0.337723063403079, 0.642193524941505,
     -0.004341002614139, 0.204748281642812, 0.799592720971327,
      0.041941786468010, 0.189358492130623, 0.768699721401368,
      0.014317320230681, 0.085283615682657, 0.900399064086661
  };
            double[] suborder_w_rule_16 = {
      0.046875697427642,
      0.006405878578585,
      0.041710296739387,
      0.026891484250064,
      0.042132522761650,
      0.030000266842773,
      0.014200098925024,
      0.003582462351273,
      0.032773147460627,
      0.015298306248441,
      0.002386244192839,
      0.019084792755899,
      0.006850054546542
  };

            for (s = 0; s < suborder_num; s++)
            {
                suborder_xyz[0 + s * 3] = suborder_xy_rule_16[0 + s * 3];
                suborder_xyz[1 + s * 3] = suborder_xy_rule_16[1 + s * 3];
                suborder_xyz[2 + s * 3] = suborder_xy_rule_16[2 + s * 3];
            }

            for (s = 0; s < suborder_num; s++)
            {
                suborder_w[s] = suborder_w_rule_16[s];
            }

            return;
        }

        private void dunavant_subrule_17(int suborder_num, ref double[] suborder_xyz,
          ref double[] suborder_w)
        {
            int s;
            double[] suborder_xy_rule_17 = {
      0.333333333333333, 0.333333333333333, 0.333333333333333,
      0.005658918886452, 0.497170540556774, 0.497170540556774,
      0.035647354750751, 0.482176322624625, 0.482176322624625,
      0.099520061958437, 0.450239969020782, 0.450239969020782,
      0.199467521245206, 0.400266239377397, 0.400266239377397,
      0.495717464058095, 0.252141267970953, 0.252141267970953,
      0.675905990683077, 0.162047004658461, 0.162047004658461,
      0.848248235478508, 0.075875882260746, 0.075875882260746,
      0.968690546064356, 0.015654726967822, 0.015654726967822,
      0.010186928826919, 0.334319867363658, 0.655493203809423,
      0.135440871671036, 0.292221537796944, 0.572337590532020,
      0.054423924290583, 0.319574885423190, 0.626001190286228,
      0.012868560833637, 0.190704224192292, 0.796427214974071,
      0.067165782413524, 0.180483211648746, 0.752351005937729,
      0.014663182224828, 0.080711313679564, 0.904625504095608
  };
            double[] suborder_w_rule_17 = {
      0.033437199290803,
      0.005093415440507,
      0.014670864527638,
      0.024350878353672,
      0.031107550868969,
      0.031257111218620,
      0.024815654339665,
      0.014056073070557,
      0.003194676173779,
      0.008119655318993,
      0.026805742283163,
      0.018459993210822,
      0.008476868534328,
      0.018292796770025,
      0.006665632004165
  };

            for (s = 0; s < suborder_num; s++)
            {
                suborder_xyz[0 + s * 3] = suborder_xy_rule_17[0 + s * 3];
                suborder_xyz[1 + s * 3] = suborder_xy_rule_17[1 + s * 3];
                suborder_xyz[2 + s * 3] = suborder_xy_rule_17[2 + s * 3];
            }

            for (s = 0; s < suborder_num; s++)
            {
                suborder_w[s] = suborder_w_rule_17[s];
            }

            return;
        }

        private void dunavant_subrule_18(int suborder_num, ref double[] suborder_xyz,
          ref double[] suborder_w)
        {
            int s;
            double[] suborder_xy_rule_18 = {
      0.333333333333333, 0.333333333333333, 0.333333333333333,
      0.013310382738157, 0.493344808630921, 0.493344808630921,
      0.061578811516086, 0.469210594241957, 0.469210594241957,
      0.127437208225989, 0.436281395887006, 0.436281395887006,
      0.210307658653168, 0.394846170673416, 0.394846170673416,
      0.500410862393686, 0.249794568803157, 0.249794568803157,
      0.677135612512315, 0.161432193743843, 0.161432193743843,
      0.846803545029257, 0.076598227485371, 0.076598227485371,
      0.951495121293100, 0.024252439353450, 0.024252439353450,
      0.913707265566071, 0.043146367216965, 0.043146367216965,
      0.008430536202420, 0.358911494940944, 0.632657968856636,
      0.131186551737188, 0.294402476751957, 0.574410971510855,
      0.050203151565675, 0.325017801641814, 0.624779046792512,
      0.066329263810916, 0.184737559666046, 0.748933176523037,
      0.011996194566236, 0.218796800013321, 0.769207005420443,
      0.014858100590125, 0.101179597136408, 0.883962302273467,
     -0.035222015287949, 0.020874755282586, 1.014347260005363
  };
            double[] suborder_w_rule_18 = {
      0.030809939937647,
      0.009072436679404,
      0.018761316939594,
      0.019441097985477,
      0.027753948610810,
      0.032256225351457,
      0.025074032616922,
      0.015271927971832,
      0.006793922022963,
     -0.002223098729920,
      0.006331914076406,
      0.027257538049138,
      0.017676785649465,
      0.018379484638070,
      0.008104732808192,
      0.007634129070725,
      0.000046187660794
  };

            for (s = 0; s < suborder_num; s++)
            {
                suborder_xyz[0 + s * 3] = suborder_xy_rule_18[0 + s * 3];
                suborder_xyz[1 + s * 3] = suborder_xy_rule_18[1 + s * 3];
                suborder_xyz[2 + s * 3] = suborder_xy_rule_18[2 + s * 3];
            }

            for (s = 0; s < suborder_num; s++)
            {
                suborder_w[s] = suborder_w_rule_18[s];
            }

            return;
        }

        private void dunavant_subrule_19(int suborder_num, ref double[] suborder_xyz,
          ref double[] suborder_w)
        {
            int s;
            double[] suborder_xy_rule_19 = {
      0.333333333333333, 0.333333333333333, 0.333333333333333,
      0.020780025853987, 0.489609987073006, 0.489609987073006,
      0.090926214604215, 0.454536892697893, 0.454536892697893,
      0.197166638701138, 0.401416680649431, 0.401416680649431,
      0.488896691193805, 0.255551654403098, 0.255551654403098,
      0.645844115695741, 0.177077942152130, 0.177077942152130,
      0.779877893544096, 0.110061053227952, 0.110061053227952,
      0.888942751496321, 0.055528624251840, 0.055528624251840,
      0.974756272445543, 0.012621863777229, 0.012621863777229,
      0.003611417848412, 0.395754787356943, 0.600633794794645,
      0.134466754530780, 0.307929983880436, 0.557603261588784,
      0.014446025776115, 0.264566948406520, 0.720987025817365,
      0.046933578838178, 0.358539352205951, 0.594527068955871,
      0.002861120350567, 0.157807405968595, 0.839331473680839,
      0.223861424097916, 0.075050596975911, 0.701087978926173,
      0.034647074816760, 0.142421601113383, 0.822931324069857,
      0.010161119296278, 0.065494628082938, 0.924344252620784
  };
            double[] suborder_w_rule_19 = {
      0.032906331388919,
      0.010330731891272,
      0.022387247263016,
      0.030266125869468,
      0.030490967802198,
      0.024159212741641,
      0.016050803586801,
      0.008084580261784,
      0.002079362027485,
      0.003884876904981,
      0.025574160612022,
      0.008880903573338,
      0.016124546761731,
      0.002491941817491,
      0.018242840118951,
      0.010258563736199,
      0.003799928855302
  };

            for (s = 0; s < suborder_num; s++)
            {
                suborder_xyz[0 + s * 3] = suborder_xy_rule_19[0 + s * 3];
                suborder_xyz[1 + s * 3] = suborder_xy_rule_19[1 + s * 3];
                suborder_xyz[2 + s * 3] = suborder_xy_rule_19[2 + s * 3];
            }

            for (s = 0; s < suborder_num; s++)
            {
                suborder_w[s] = suborder_w_rule_19[s];
            }

            return;
        }

        private void dunavant_subrule_20(int suborder_num, ref double[] suborder_xyz,
          ref double[] suborder_w)
        {
            int s;
            double[] suborder_xy_rule_20 = {
      0.333333333333333, 0.333333333333333, 0.333333333333333,
     -0.001900928704400, 0.500950464352200, 0.500950464352200,
      0.023574084130543, 0.488212957934729, 0.488212957934729,
      0.089726636099435, 0.455136681950283, 0.455136681950283,
      0.196007481363421, 0.401996259318289, 0.401996259318289,
      0.488214180481157, 0.255892909759421, 0.255892909759421,
      0.647023488009788, 0.176488255995106, 0.176488255995106,
      0.791658289326483, 0.104170855336758, 0.104170855336758,
      0.893862072318140, 0.053068963840930, 0.053068963840930,
      0.916762569607942, 0.041618715196029, 0.041618715196029,
      0.976836157186356, 0.011581921406822, 0.011581921406822,
      0.048741583664839, 0.344855770229001, 0.606402646106160,
      0.006314115948605, 0.377843269594854, 0.615842614456541,
      0.134316520547348, 0.306635479062357, 0.559048000390295,
      0.013973893962392, 0.249419362774742, 0.736606743262866,
      0.075549132909764, 0.212775724802802, 0.711675142287434,
     -0.008368153208227, 0.146965436053239, 0.861402717154987,
      0.026686063258714, 0.137726978828923, 0.835586957912363,
      0.010547719294141, 0.059696109149007, 0.929756171556853
  };
            double[] suborder_w_rule_20 = {
      0.033057055541624,
      0.000867019185663,
      0.011660052716448,
      0.022876936356421,
      0.030448982673938,
      0.030624891725355,
      0.024368057676800,
      0.015997432032024,
      0.007698301815602,
     -0.000632060497488,
      0.001751134301193,
      0.016465839189576,
      0.004839033540485,
      0.025804906534650,
      0.008471091054441,
      0.018354914106280,
      0.000704404677908,
      0.010112684927462,
      0.003573909385950
  };

            for (s = 0; s < suborder_num; s++)
            {
                suborder_xyz[0 + s * 3] = suborder_xy_rule_20[0 + s * 3];
                suborder_xyz[1 + s * 3] = suborder_xy_rule_20[1 + s * 3];
                suborder_xyz[2 + s * 3] = suborder_xy_rule_20[2 + s * 3];
            }

            for (s = 0; s < suborder_num; s++)
            {
                suborder_w[s] = suborder_w_rule_20[s];
            }

            return;
        }
        #region i4

        /// <summary>
        ///  Purpose:
        ///
        ///    I4_MODP returns the nonnegative remainder of I4 division.
        ///
        ///  Formula:
        ///
        ///    If
        ///      NREM = I4_MODP ( I, J )
        ///      NMULT = ( I - NREM ) / J
        ///    then
        ///      I = J * NMULT + NREM
        ///    where NREM is always nonnegative.
        ///
        ///  Discussion:
        ///
        ///    The MOD function computes a result with the same sign as the
        ///    quantity being divided.  Thus, suppose you had an angle A,
        ///    and you wanted to ensure that it was between 0 and 360.
        ///    Then mod(A,360) would do, if A was positive, but if A
        ///    was negative, your result would be between -360 and 0.
        ///
        ///    On the other hand, I4_MODP(A,360) is between 0 and 360, always.
        ///
        ///  Example:
        ///
        ///        I         J     MOD  I4_MODP   I4_MODP Factorization
        ///
        ///      107        50       7       7    107 =  2 *  50 + 7
        ///      107       -50       7       7    107 = -2 * -50 + 7
        ///     -107        50      -7      43   -107 = -3 *  50 + 43
        ///     -107       -50      -7      43   -107 =  3 * -50 + 43
        ///
        ///  Licensing:
        ///
        ///    This code is distributed under the GNU LGPL license.
        ///
        ///  Modified:
        ///
        ///    26 May 1999
        ///
        ///  Author:
        ///
        ///    John Burkardt
        ///
        /// </summary>
        /// <param name="i">int I, the number to be divided.</param>
        /// <param name="j">int J, the number that divides I.</param>
        /// <returns>nt I4_MODP, the nonnegative remainder when I is divided by J.</returns>
        private int i4_modp(int i, int j)
        {
            int value;

            if (j == 0)
            {
                throw new Exception("I4_MODP - Fatal error!\n I4_MODP ( I, J ) called with J = " + j);
            }

            value = i % j;

            if (value < 0)
            {
                value = value + Math.Abs(j);
            }

            return value;
        }

        /// <summary>
        ///  Purpose:
        ///
        ///    I4_WRAP forces an integer to lie between given limits by wrapping.
        ///
        ///  Example:
        ///
        ///    ILO = 4, IHI = 8
        ///
        ///    I   Value
        ///
        ///    -2     8
        ///    -1     4
        ///     0     5
        ///     1     6
        ///     2     7
        ///     3     8
        ///     4     4
        ///     5     5
        ///     6     6
        ///     7     7
        ///     8     8
        ///     9     4
        ///    10     5
        ///    11     6
        ///    12     7
        ///    13     8
        ///    14     4
        ///
        ///  Licensing:
        ///
        ///    This code is distributed under the GNU LGPL license.
        ///
        ///  Modified:
        ///
        ///    19 August 2003
        ///
        ///  Author:
        ///
        ///    John Burkardt
        ///
        /// </summary>
        /// <param name="ival">int IVAL, an integer value.</param>
        /// <param name="ilo">int ILO, the desired bounds for the integer value.</param>
        /// <param name="ihi">int IHI, the desired bounds for the integer value.</param>
        /// <returns>int I4_WRAP, a "wrapped" version of IVAL.</returns>
        private int i4_wrap(int ival, int ilo, int ihi)
        {
            int jhi;
            int jlo;
            int wide;

            jlo = Math.Min(ilo, ihi);
            jhi = Math.Max(ilo, ihi);

            wide = jhi + 1 - jlo;

            if (wide == 1)
                return jlo;
            else
                return jlo + i4_modp(ival - jlo, wide);
        }
        #endregion

        #endregion


        public double[] CalculateDePhiDeXDeFiDeY(int deFiIndex, Element element)
        {
            double deiksdealpha = element.Nodes[0].X * baseFunctionsDerivative[0][0](new double[] { 1, 1 }) +
                                  element.Nodes[1].X * baseFunctionsDerivative[1][0](new double[] { 1, 1 }) +
                                  element.Nodes[2].X * baseFunctionsDerivative[2][0](new double[] { 1, 1 });

            double deigrekdealpha = element.Nodes[0].Y * baseFunctionsDerivative[0][0](new double[] { 1, 1 }) +
                                    element.Nodes[1].Y * baseFunctionsDerivative[1][0](new double[] { 1, 1 }) +
                                    element.Nodes[2].Y * baseFunctionsDerivative[2][0](new double[] { 1, 1 });

            double deiksdebetta = element.Nodes[0].X * baseFunctionsDerivative[0][1](new double[] { 1, 1 }) +
                                  element.Nodes[1].X * baseFunctionsDerivative[1][1](new double[] { 1, 1 }) +
                                  element.Nodes[2].X * baseFunctionsDerivative[2][1](new double[] { 1, 1 });

            double deigrekdebetta = element.Nodes[0].Y * baseFunctionsDerivative[0][1](new double[] { 1, 1 }) +
                                  element.Nodes[1].Y * baseFunctionsDerivative[1][1](new double[] { 1, 1 }) +
                                  element.Nodes[2].Y * baseFunctionsDerivative[2][1](new double[] { 1, 1 });

            double reverseJE = 1 / (deiksdealpha * deigrekdebetta - deigrekdealpha * deiksdebetta);

            double jakobian = deiksdealpha * deigrekdebetta - deiksdebetta * deigrekdealpha;

            double[] rightPart = new double[] { baseFunctionsDerivative[deFiIndex][0](new double[] { 1, 1 }), baseFunctionsDerivative[deFiIndex][1](new double[] { 1, 1 }) };

            double[,] reverseMatrix = new double[,]
            {
                {deigrekdebetta / jakobian, -1 * deigrekdealpha / jakobian },
                {-1 * deiksdebetta / jakobian, deiksdealpha / jakobian}
            };

            double[] result = new double[2] { 0, 0 };
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; ++j)
                {
                    result[i] += reverseMatrix[i, j] * rightPart[j];
                }
            }

            return result;
        }

        public string GetAllLinkedNodesForMatlab()
        {
            Dictionary<int, List<int>> toReturn = new Dictionary<int, List<int>>();
            foreach (var element in this.FiniteElements)
            {
                if (!toReturn.ContainsKey(element.Nodes[0].GlobalIndex))
                    toReturn.Add(element.Nodes[0].GlobalIndex, new List<int>());
                if (!toReturn[element.Nodes[0].GlobalIndex].Contains(element.Nodes[1].GlobalIndex))
                    toReturn[element.Nodes[0].GlobalIndex].Add(element.Nodes[1].GlobalIndex);
                if (!toReturn[element.Nodes[0].GlobalIndex].Contains(element.Nodes[2].GlobalIndex))
                    toReturn[element.Nodes[0].GlobalIndex].Add(element.Nodes[2].GlobalIndex);

                if (!toReturn.ContainsKey(element.Nodes[1].GlobalIndex))
                    toReturn.Add(element.Nodes[1].GlobalIndex, new List<int>());
                if (!toReturn[element.Nodes[1].GlobalIndex].Contains(element.Nodes[0].GlobalIndex))
                    toReturn[element.Nodes[1].GlobalIndex].Add(element.Nodes[0].GlobalIndex);
                if (!toReturn[element.Nodes[1].GlobalIndex].Contains(element.Nodes[2].GlobalIndex))
                    toReturn[element.Nodes[1].GlobalIndex].Add(element.Nodes[2].GlobalIndex);

                if (!toReturn.ContainsKey(element.Nodes[2].GlobalIndex))
                    toReturn.Add(element.Nodes[2].GlobalIndex, new List<int>());
                if (!toReturn[element.Nodes[2].GlobalIndex].Contains(element.Nodes[1].GlobalIndex))
                    toReturn[element.Nodes[2].GlobalIndex].Add(element.Nodes[1].GlobalIndex);
                if (!toReturn[element.Nodes[2].GlobalIndex].Contains(element.Nodes[0].GlobalIndex))
                    toReturn[element.Nodes[2].GlobalIndex].Add(element.Nodes[0].GlobalIndex);
            }

            List<int> cells = toReturn.Select(item => item.Value.Count).ToList();
            int max = cells.ToArray().Max();

            foreach (var item in toReturn)
            {
                int count = max - item.Value.Count;
                for (int i = 0; i < count; ++i)
                {
                    item.Value.Add(item.Value[0]);
                }
            }

            string str = "[";
            foreach (var item in toReturn)
            {
                str += " " + (item.Key +1).ToString();
                for (int i = 0; i < item.Value.Count;i++)
                {
                    str += " " + (item.Value[i]+ 1).ToString();
                   
                }

                str += ";";
            }

            str += "];";

            return str;
        }

        public string GetXVectorForMatlab()
        {
            var a = new NodesComparer();
            this.Nodes.Sort(a);
            string str = "[";
            foreach (var item in this.Nodes)
            {
                str += " " + item.X + ";";
            }

            str += "];";
            return str;
        }
        public string GetYVectorForMatlab()
        {
            var a = new NodesComparer();
            this.Nodes.Sort(a);
            string str = "[";
            foreach (var item in this.Nodes)
            {
                str += " " + item.Y + ";";
            }

            str += "];";
            return str;
        }

        public string GetZVectorForMatlab(double[] q)
        {
            string str = "[";
            foreach (var item in q)
            {
                str += " " + item + ";";
            }

            str += "];";
            return str;
        }
    }

    public class NodesComparer : IComparer<Node>
    {
        public int Compare(Node a, Node b)
        {
            if (a.GlobalIndex > b.GlobalIndex)
            {
                return 1;
            }
            else
            {
                if (a.GlobalIndex < b.GlobalIndex)
                {
                    return -1;
                }
                else return 0;
            }
        }
    }

    public class GaussSolutionNotFound : Exception
    {
        public GaussSolutionNotFound(string msg)
            : base("Решение не может быть найдено: \r\n" + msg)
        {
        }
    }

    public class LinearSystem
    {
        private double[,] initial_a_matrix;
        private double[,] a_matrix;  // матрица A
        private double[] x_vector;   // вектор неизвестных x
        private double[] initial_b_vector;
        private double[] b_vector;   // вектор b
        private double[] u_vector;   // вектор невязки U
        private double eps;          // порядок точности для сравнения вещественных чисел 
        private int size;            // размерность задачи


        public LinearSystem(double[,] a_matrix, double[] b_vector)
            : this(a_matrix, b_vector, 0.0001)
        {
        }
        public LinearSystem(double[,] a_matrix, double[] b_vector, double eps)
        {
            if (a_matrix == null || b_vector == null)
                throw new ArgumentNullException("Один из параметров равен null.");

            int b_length = b_vector.Length;
            int a_length = a_matrix.Length;
            if (a_length != b_length * b_length)
                throw new ArgumentException(@"Количество строк и столбцов в матрице A должно совпадать с количеством элементров в векторе B.");

            this.initial_a_matrix = a_matrix;  // запоминаем исходную матрицу
            this.a_matrix = (double[,])a_matrix.Clone(); // с её копией будем производить вычисления
            this.initial_b_vector = b_vector;  // запоминаем исходный вектор
            this.b_vector = (double[])b_vector.Clone();  // с его копией будем производить вычисления
            this.x_vector = new double[b_length];
            this.u_vector = new double[b_length];
            this.size = b_length;
            this.eps = eps;

            GaussSolve();
        }

        public double[] XVector
        {
            get
            {
                return x_vector;
            }
        }

        public double[] UVector
        {
            get
            {
                return u_vector;
            }
        }

        // инициализация массива индексов столбцов
        private int[] InitIndex()
        {
            int[] index = new int[size];
            for (int i = 0; i < index.Length; ++i)
                index[i] = i;
            return index;
        }

        // поиск главного элемента в матрице
        private double FindR(int row, int[] index)
        {
            int max_index = row;
            double max = a_matrix[row, index[max_index]];
            double max_abs = Math.Abs(max);
            //if(row < size - 1)
            for (int cur_index = row + 1; cur_index < size; ++cur_index)
            {
                double cur = a_matrix[row, index[cur_index]];
                double cur_abs = Math.Abs(cur);
                if (cur_abs > max_abs)
                {
                    max_index = cur_index;
                    max = cur;
                    max_abs = cur_abs;
                }
            }

            if (max_abs < eps)
            {
                if (Math.Abs(b_vector[row]) > eps)
                    throw new GaussSolutionNotFound("Система уравнений несовместна.");
                else
                    throw new GaussSolutionNotFound("Система уравнений имеет множество решений.");
            }

            // меняем местами индексы столбцов
            int temp = index[row];
            index[row] = index[max_index];
            index[max_index] = temp;

            return max;
        }

        // Нахождение решения СЛУ методом Гаусса
        public void GaussSolve()
        {
            int[] index = InitIndex();
            GaussForwardStroke(index);
            GaussBackwardStroke(index);
            GaussDiscrepancy();
        }

        // Прямой ход метода Гаусса
        private void GaussForwardStroke(int[] index)
        {
            // перемещаемся по каждой строке сверху вниз
            for (int i = 0; i < size; ++i)
            {
                // 1) выбор главного элемента
                double r = FindR(i, index);

                // 2) преобразование текущей строки матрицы A
                for (int j = 0; j < size; ++j)
                    a_matrix[i, j] /= r;

                // 3) преобразование i-го элемента вектора b
                b_vector[i] /= r;

                // 4) Вычитание текущей строки из всех нижерасположенных строк
                for (int k = i + 1; k < size; ++k)
                {
                    double p = a_matrix[k, index[i]];
                    for (int j = i; j < size; ++j)
                        a_matrix[k, index[j]] -= a_matrix[i, index[j]] * p;
                    b_vector[k] -= b_vector[i] * p;
                    a_matrix[k, index[i]] = 0.0;
                }
            }
        }

        // Обратный ход метода Гаусса
        private void GaussBackwardStroke(int[] index)
        {
            // перемещаемся по каждой строке снизу вверх
            for (int i = size - 1; i >= 0; --i)
            {
                // 1) задаётся начальное значение элемента x
                double x_i = b_vector[i];

                // 2) корректировка этого значения
                for (int j = i + 1; j < size; ++j)
                    x_i -= x_vector[index[j]] * a_matrix[i, index[j]];
                x_vector[index[i]] = x_i;
            }
        }

        // Вычисление невязки решения
        // U = b - x * A
        // x - решение уравнения, полученное методом Гаусса
        private void GaussDiscrepancy()
        {
            for (int i = 0; i < size; ++i)
            {
                double actual_b_i = 0.0;   // результат перемножения i-строки 
                                           // исходной матрицы на вектор x
                for (int j = 0; j < size; ++j)
                    actual_b_i += initial_a_matrix[i, j] * x_vector[j];
                // i-й элемент вектора невязки
                u_vector[i] = initial_b_vector[i] - actual_b_i;
            }
        }

    }

}
