using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEM
{
    public class LocalMatrix
    {
        public int Index { get; set; }

        public int InRow { get; set; }

        public int InCol { get; set; }

        public List<List<double>> Data { get; set; }

        public LocalMatrix()
        {

        }

        public LocalMatrix(int r, int c)
        {
            this.InCol = c;
            this.InRow = r;
            Data = new List<List<double>>();
            for (int i = 0; i < InCol; ++i)
            {
                var list = new List<double>();
                for(int j = 0; j < this.InRow; ++j)
                {
                    list.Add(0);
                }
                Data.Add(list);
            }
        }
    }
}
