using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;

namespace NewNN007
{
    public static class Functional
    {
        public static double[][] getTS(String path)
        {
            String text;
            double[][] TS;
            int count = 0;
            using (StreamReader sr = new StreamReader(path, Encoding.UTF8))
            {
                while ((text = sr.ReadLine()) != null)
                {
                    count++;
                }
            }
            TS = new double[count][];

            using (StreamReader sr = new StreamReader(path, Encoding.UTF8))
            {
                int count2 = 0, i = 0;

                while ((text = sr.ReadLine()) != null)
                {
                    string[] tmp = text.Split(';');
                    TS[count2] = new double[tmp.Length];
                    for (int j = 0; j < tmp.Length; j++)
                    {
                        TS[count2][j] = Double.Parse(tmp[j]);
                    }
                    count2++;
                }
            }

            
            return TS;
        }

        public static ArrayList getAtributes(ArrayList TS)
        {
            ArrayList result = new ArrayList();

            for (int i = 0; i < 3; i++)
            {
                double[] tmp = new double[TS.Count];
                int j = 0;
                foreach (double[] vector in TS)
                {
                    tmp[j] = vector[i];
                    j++;
                }
                result.Add(tmp);
                
            }
            
            return result;
        }

        public static double[][] getInput(ArrayList TS)
        {
            int xp = 0;
            double[][] list = new double[TS.Count][];
            foreach (double[] vector in TS)
            {
                
                list[xp] = new double[vector.Length];
                for (int i = 0; i < list.Length; i++)
                {
                    list[xp][i] = vector[i];
                }
                xp++;
            }
            return list;
        }

        public static double[][] getOutput(ArrayList TS)
        {
            int xp = 0;
            double[][] list = new double[TS.Count][];
            foreach (double[] vector in TS)
            {

                list[xp] = new double[1];
                
                list[xp][0] = vector[vector.Length-1];
                
                xp++;
            }
            return list;
        }

        public static double[][] convertListToDouble(ArrayList list)
        {
            double[][] result = new double[list.Count][];
            int x = 0;
            foreach (double[] vector in list)
            {
                result[x] = new double[vector.Length];
                for (int i = 0; i < result[x].Length; i++)
                {
                    result[x][i] = vector[i];
                }
            }
            return result;

            

        }
    }
}
