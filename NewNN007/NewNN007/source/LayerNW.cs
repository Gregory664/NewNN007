using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewNN007
{
    public class LayerNW
    {
        private double[,] weights;
        private int countX, countY;

        public LayerNW(int countX, int countY)
        {
            this.countX = countX;
            this.countY = countY;
            GiveMemory();
        }

        public int getCountX
        {
            get { return countX; }
        }

        public int getCountY
        {
            get { return countY; }
        }

        public double this[int row, int col]
        {
            get { return weights[row, col]; }
            set { weights[row, col] = value; }
        }

        public void GenerateWeights()
        {
            Random rnd = new Random();
            for (int i = 0; i < countX; i++)
            {
                for (int j = 0; j < countY; j++)
                {
                    weights[i, j] = rnd.NextDouble() - 0.5;
                }
            }
        }

        protected void GiveMemory()
        {
            weights = new double[countX, countY];
        }
    }
}
