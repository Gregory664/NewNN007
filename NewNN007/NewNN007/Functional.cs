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
        public static ArrayList getTS(String path)
        {
            String text;
            ArrayList list = new ArrayList();
            StreamReader sr = new StreamReader(path, Encoding.UTF8);
            while ((text = sr.ReadLine()) != null)
            {
                string[] oneLine = text.Split(';');
                double[] tmp = new double[oneLine.Length];
                for (int i = 0; i < tmp.Length; i++)
                {
                    tmp[i] = Convert.ToDouble(oneLine[i]);
                }
                list.Add(tmp);
            }

            return list;
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
    }
}
