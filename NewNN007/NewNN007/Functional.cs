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
    }
}
