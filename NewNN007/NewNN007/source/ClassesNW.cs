using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewNN007
{
    /// <summary> 
    /// Структура классов принятия решений
    /// </summary>
    public class ClassesNW
    {
        /// <summary>
        /// Массив средних значений атрибутов для класса принятия решений
        /// </summary>
        public double[] MeansAtributes;
        private int countOfSamples = 0;

        public int setCountOfSamples
        {
            get { return countOfSamples; }
            set { countOfSamples = value; }
        }

        public int getCountOfAtributes
        {
            get { return MeansAtributes.Length; }
        }

        /// <summary>
        /// Инициализирует массив средних значений атрибутов
        /// </summary>
        /// <param name="countOfAtributes">Количество атрибутов</param>
        public ClassesNW(int countOfAtributes)
        {
            MeansAtributes = new double[countOfAtributes];
        }
    }
}
