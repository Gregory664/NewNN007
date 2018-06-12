using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NewNN007
{
    // Класс - нейронная сеть
    public class NeuralNW
    {
        //Поля
        private int countLayers = 0;
        private int countX;
        private int countY;

        //Структуры - в виде массивов
        double[][] NETOUT;
        double[] ERRORS;
        double[] DISCRIMINANT;
        double[] ACTIVATE_FUNCTIONS;

        //Структуры - в виде классов
        LayerNW[] Layers;
        ClassesNW[] Classes;
        Dictionary<double, double[]> PROBABILITY;

        /*............................................................*/
        /*
         * геттеры, сеттеры
         */
        public int getX
        {
            get { return countX; }
        }

        public int getY
        {
            get { return countY; }
        }

        public int getCountLayers
        {
            get { return countLayers; }
        }
        /*............................................................*/

        /*............................................................*/
        /* 
         * конструкторы
         */

        /// <summary>
        /// Открывает НС, а так же инициализирует структуру Classes, 
        /// которая равна количеству классов принятия решений
        /// </summary>
        /// <param name="FileName">Путь к НС</param>
        /// <param name="numberOfDecisionClasses">Количество классов принятие решений</param>
        /// <param name="GetCountOfAtributes">Количество атрибутов</param>
        public NeuralNW(String FileName, int numberOfDecisionClasses, int CountOfAtributes)
        {
            OpenNW(FileName);
            InitializationClasses(numberOfDecisionClasses, CountOfAtributes);
            InitializationProbabilities(numberOfDecisionClasses);
            DISCRIMINANT = new double[Layers[0].getCountY];
            InitializationZeroWeights();
            InitializationActivateFunction();
        }

        /*............................................................*/


        /*............................................................*/
        /*
         * инициализация структур
         */

        /// <summary>
        /// Инициализирует структуру Classes
        /// </summary>
        /// <param name="numberOfDecisionClasses">Количество классов принятие решений</param>
        /// <param name="GetCountOfAtributes">Количество атрибутов</param>
        private void InitializationClasses(int numberOfDecisionClasses, int countOfAtributes)
        {
            Classes = new ClassesNW[numberOfDecisionClasses];
            for (int i = 0; i < numberOfDecisionClasses; i++)
            {
                Classes[i] = new ClassesNW(countOfAtributes);
            }
        }

        /// <summary>
        /// Происходит заполнение словаря вероятностей PROBABILITY
        /// </summary>
        /// <param name="numberOfDecisionClasses"></param>
        private void InitializationProbabilities(int numberOfDecisionClasses)
        {
            PROBABILITY = new Dictionary<double, double[]>();
            double maxProbability = (double)(1 - (numberOfDecisionClasses * 0.01) + 0.01);

            //Цикл по кол-ву классов принятия решений
            for (int i = 0; i < numberOfDecisionClasses; i++)
            {
                //массив вероятностей для класса, заполняется автоматически
                double[] prob = new double[numberOfDecisionClasses];
                for (int j = 0; j < prob.Length; j++)
                {
                    prob[j] = 0.01;
                }
                prob[i] = maxProbability;
                double key = (double)(i + 1);
                PROBABILITY.Add(key, prob);
            }
        }

        private void InitializationActivateFunction()
        {
            ACTIVATE_FUNCTIONS = new double[Layers[0].getCountY];
        }

        /// <summary>
        /// Инициализируется массив ошибок
        /// </summary>
        /// <param name="length"></param>
        private void InitializationErrors(int length)
        {
            ERRORS = new double[length];
        }

        /*............................................................*/


        /// <summary>
        /// Заполняем структуру Classes средними значениями атрибутов
        /// </summary>
        /// <param name="TS">Обучающая выборка</param>
        public void ComputeMeanAttributes(double[][] TS)
        {
            //Инициализируем массив ошибок
            InitializationErrors(TS.Length);

            for (int i = 0; i < TS.Length; i++)
            {
                ///Берем последнее значение(выход) из strTS-ой строчки выборки,
                ///конвентируем в intЮ вычитаем 1
                ///и получаем индекс структуры Classes =)
                int numberOfClass = Convert.ToInt32(TS[i][TS[i].Length - 1]) - 1;

                //Увеличиваем счетчик выборки, принадлежащей к классу
                Classes[numberOfClass].setCountOfSamples++;

                int CountOfAtributes = Classes[0].getCountOfAtributes;
                for (int x = 0; x < CountOfAtributes; x++)
                {
                    Classes[numberOfClass].MeansAtributes[x] += TS[i][x];
                }
            }

            //Проходимся по структуре Classes, находим среднее атрубутов
            for (int i = 0; i < Classes.Length; i++)
            {
                for (int j = 0; j < Classes[i].getCountOfAtributes; j++)
                {
                    Classes[i].MeansAtributes[j] /= Classes[i].setCountOfSamples;
                }
            }
        }

        /// <summary>
        /// Нахождение корреляции по методу Л.Гудмена и Е.Краскала
        /// </summary>
        /// <param name="pairs">Массив пар</param>
        /// <returns></returns>
        private double ComputeGamma(double[][] pairs)
        {
            //счетчики согласованных и несогласованных пар соответственно.
            int countS = 0;
            int countD = 0;

            int count = 0;
            for (int i = 0; i < pairs.Length - 1; i++)
            {

                for (int j = 0 + count; j < pairs.Length - 1; j++)
                {
                    if (pairs[i][0] > pairs[j + 1][0] && pairs[i][1] > pairs[j + 1][1])
                    {
                        countS++;
                    }
                    else if (pairs[i][0] < pairs[j + 1][0] && pairs[i][1] < pairs[j + 1][1])
                    {
                        countS++;
                    }
                    else countD++;
                }
                count++;

            }
            ////мера Гамма Л.Гудмена и Е.Краскала
            double a = countS - countD;
            double b = countS + countD;
            double gamma = Math.Abs(a / b);
            return gamma;
        }

        /// <summary>
        /// Нахождение дискриминанта для каждого нейрона
        /// </summary>
        private void ComputeDiscriminantes()
        {
            for (int i = 0; i < Layers[0].getCountY; i++)
            {
                double number = 0.0;
                for (int j = 0; j < Layers[0].getCountX; j++)
                {
                    double num1 = NETOUT[0][j] * Layers[0][j, i];
                    number += num1;
                }
                DISCRIMINANT[i] = number;
            }
        }

        /// <summary>
        /// Нахождение ошибки обучения, а так же заполнение структуры Errors[t], где
        /// t - индекс текущей обучающей выборки
        /// </summary>
        /// <param name="t">Индекс текущей обучающей выборки</param>
        /// <param name="probability">Массив вероятностей</param>
        private void ComputeCurrentError(int t, double[] probability)
        {
            for (int l = 0; l < NETOUT[NETOUT.Length - 1].Length; l++)
            {
                ERRORS[t] += Math.Pow(probability[l] - ACTIVATE_FUNCTIONS[l], 2) / 2;
            }
        }

        /// <summary>
        /// Расчет активационной функции
        /// </summary>
        private void ComputeActivateFunction()
        {
            for (int l = 0; l < DISCRIMINANT.Length; l++)
            {
                double W = Math.Tanh(DISCRIMINANT[l] * 2.0 / 3.0);
                ACTIVATE_FUNCTIONS[l] = W;
                NETOUT[1][l] = W;
            }
        }



        private void InitializationZeroWeights()
        {
            for (int k = 0; k < Layers.Length; k++)
            {
                for (int i = 0; i < Layers[k].getCountX; i++)
                {
                    for (int j = 0; j < Layers[k].getCountY; j++)
                    {
                        Layers[k][i, j] = 0.0;
                    }
                }
            }
        }

        public void InitializationWeights(double[][] TS)
        {
            for (int k = 0; k < Layers.Length; k++)
            {
                for (int i = 0; i < Layers[k].getCountX; i++)
                {
                    double[] currentAttributes = new double[TS.Length];

                    for (int a = 0; a < TS.Length; a++)
                    {
                        currentAttributes[a] = TS[a][i];
                    }

                    for (int j = 0; j < Layers[k].getCountY; j++)
                    {
                        double[] atribute_minus_avg = new double[currentAttributes.Length];

                        for (int a = 0; a < currentAttributes.Length; a++)
                        {
                            atribute_minus_avg[a] = currentAttributes[a] - Classes[j].MeansAtributes[i];
                        }

                        double[][] massForGamma = new double[currentAttributes.Length][];

                        for (int a = 0; a < TS.Length; a++)
                        {
                            massForGamma[a] = new double[2];

                            double classLabel = TS[a][TS[a].Length - 1];
                            double[] temp = getPropability(classLabel);
                            massForGamma[a][0] = atribute_minus_avg[a];
                            massForGamma[a][1] = temp[j];

                        }

                        Layers[k][i, j] = ComputeGamma(massForGamma);
                    }
                }
            }
        }



        /// <summary>
        /// Возвращает массив вероятностей, соответсвтующий метке класса
        /// </summary>
        /// <param name="label">Метка класса</param>
        /// <returns></returns>
        public double[] getPropability(double label)
        {
            double[] probability = new double[Layers[0].getCountY];
            foreach (KeyValuePair<double, double[]> entry in PROBABILITY)
            {
                if (entry.Key == label) { probability = entry.Value; }
            }
            return probability;
        }

        /// <summary>
        /// Наш метод обучения
        /// </summary>
        /// <param name="TS">Обучающая выборка</param>
        public double[] Learn(double[][] TS)
        {
            //Расчет линейного дискриминанта
            for (int t = 0; t < TS.Length; t++)
            {
                double[] ts = new double[TS[t].Length];
                ts = TS[t];

                for (int l = 0; l < Layers[0].getCountX; l++)
                {
                    NETOUT[0][l] = ts[l];
                }

                //Расчитываем дискриминант для выходного слоя   
                ComputeDiscriminantes();

                //Расчитываем активационную функцию
                ComputeActivateFunction();

                //Заполняем массив вероятностей для текущих классов
                double[] probabilities = getPropability(ts[ts.Length - 1]);

                //Расчет текущей ошибки для данной выборки данных
                ComputeCurrentError(t, probabilities);
            }

            //Меняем веса
            ChangeWeight(TS);

            return ERRORS;

        }

        public double[] TEST(double[] ts)
        {
            ///???
            double[] answer = new double[NETOUT[1].Length];

            for (int l = 0; l < Layers[0].getCountX; l++)
            {
                NETOUT[0][l] = ts[l];
            }

            for (int j = 0; j < Layers[0].getCountY; j++)
            {
                double summ = 0.0;
                for (int i = 0; i < Layers[0].getCountX; i++)
                {
                    summ += NETOUT[0][i] * Layers[0][i, j];
                }
                NETOUT[1][j] = Math.Tanh(summ * 2.0 / 3.0);

            }

            for (int i = 0; i < answer.Length; i++)
            {
                answer[i] = NETOUT[1][i];
            }


            return answer;
        }

        private void ChangeWeight(double[][] TS)
        {
            for (int i = 0; i < Layers[0].getCountX; i++)
            {
                for (int j = 0; j < Layers[0].getCountY; j++)
                {
                    //Массив всех атрибутов для текущего нейрона
                    double[] currentAttributes = new double[TS.Length];
                    for (int a = 0; a < TS.Length; a++)
                    {
                        currentAttributes[a] = TS[a][i];
                    }

                    //Массив значение (текущего атрибута - среднее текущего атрибута)
                    double[] atribute_minus_avg = new double[currentAttributes.Length];
                    for (int a = 0; a < currentAttributes.Length; a++)
                    {
                        atribute_minus_avg[a] = currentAttributes[a] - Classes[j].MeansAtributes[i];
                    }

                    double[][] massForGamma = new double[currentAttributes.Length][];
                    double[][] massForErrors = new double[currentAttributes.Length][];

                    //Заполняем массив пар для нохождения корреляции
                    for (int a = 0; a < TS.Length; a++)
                    {
                        massForGamma[a] = new double[2];

                        double classLabel = TS[a][TS[a].Length - 1];
                        double[] temp = getPropability(classLabel);

                        massForGamma[a][0] = atribute_minus_avg[a];
                        massForGamma[a][1] = temp[j];

                        massForErrors[a] = new double[2];
                        massForErrors[a][0] = atribute_minus_avg[a];
                        massForErrors[a][1] = ERRORS[a];
                    }

                    double chislitel = ComputeGamma(massForErrors) * ComputeGamma(massForGamma);
                    double znameenatel = 0.0;
                    //////////////////////////////////
                    for (int a = 0; a < Classes[0].getCountOfAtributes; a++)
                    {

                        massForGamma = new double[currentAttributes.Length][];
                        massForErrors = new double[currentAttributes.Length][];

                        for (int v = 0; v < TS.Length; v++)
                        {
                            massForGamma[v] = new double[2];

                            double classLabel = TS[v][TS[v].Length - 1];
                            double[] temp = getPropability(classLabel);

                            massForGamma[v][0] = atribute_minus_avg[v];
                            massForGamma[v][1] = temp[j];

                            massForErrors[v] = new double[2];
                            massForErrors[v][0] = atribute_minus_avg[v];
                            massForErrors[v][1] = ERRORS[v];
                        }
                        znameenatel += ComputeGamma(massForErrors) * ComputeGamma(massForGamma);
                    }

                    Layers[0][i, j] = chislitel;// / znameenatel;
                }

            }
        }

        /// <summary>
        /// Расчет дискриминанта
        /// </summary>
        /// <param name="TS"></param>
        /// <param name="t"></param>


        public void EraseErrors()
        {
            for (int i = 0; i < ERRORS.Length; i++)
            {
                ERRORS[i] = 0.0;
            }
        }







        /*
         * Методы для сохранения НС
         */

        // Сохраняет НС
        public void SaveNW(String FileName)
        {
            // размер сети в байтах
            int sizeNW = returnSizeNW();
            byte[] binNW = new byte[sizeNW];

            int k = 0;
            // Записываем размерности слоев в массив байтов
            WriteInArray(binNW, ref k, countLayers);
            if (countLayers <= 0)
                return;

            WriteInArray(binNW, ref k, Layers[0].getCountX);
            for (int i = 0; i < countLayers; i++)
                WriteInArray(binNW, ref k, Layers[i].getCountY);

            // Зпаисвыаем сами веса
            for (int r = 0; r < countLayers; r++)
                for (int p = 0; p < Layers[r].getCountX; p++)
                    for (int q = 0; q < Layers[r].getCountY; q++)
                    {
                        WriteInArray(binNW, ref k, Layers[r][p, q]);
                    }


            File.WriteAllBytes(FileName, binNW);
        }

        // Разбивает переменную типа int на байты и записывает в массив
        void WriteInArray(byte[] mas, ref int pos, double value)
        {
            DataToByte DTB = new DataToByte();
            DTB.vDouble = value;
            mas[pos++] = DTB.b1;
            mas[pos++] = DTB.b2;
            mas[pos++] = DTB.b3;
            mas[pos++] = DTB.b4;
            mas[pos++] = DTB.b5;
            mas[pos++] = DTB.b6;
            mas[pos++] = DTB.b7;
            mas[pos++] = DTB.b8;
        }

        // Извлекает переменную типа int из 4-х байтов массива
        int ReadFromArrayInt(byte[] mas, ref int pos)
        {
            DataToByte DTB = new DataToByte();
            DTB.b1 = mas[pos++];
            DTB.b2 = mas[pos++];
            DTB.b3 = mas[pos++];
            DTB.b4 = mas[pos++];

            return DTB.vInt;
        }

        // Возвращает размер НС в байтах
        private int returnSizeNW()
        {
            int sizeNW = sizeof(int) * (countLayers + 2);
            for (int i = 0; i < countLayers; i++)
            {
                sizeNW += sizeof(double) * Layers[i].getCountX * Layers[i].getCountY;
            }
            return sizeNW;
        }


        /*
         * Вспомогательные методы для открытия НС
         */

        // Открывает НС
        public NeuralNW(String FileName)
        {
            OpenNW(FileName);
        }

        // Открывает НС
        private void OpenNW(String FileName)
        {
            byte[] binNW = File.ReadAllBytes(FileName);

            int k = 0;
            // Извлекаем количество слоев из массива
            countLayers = ReadFromArrayInt(binNW, ref k);
            Layers = new LayerNW[countLayers];

            // Извлекаем размерность слоев
            int CountY1 = 0, CountX1 = ReadFromArrayInt(binNW, ref k);
            // Размерность входа
            countX = CountX1;
            // Выделяемпамять под выходы нейронов и дельта
            NETOUT = new double[countLayers + 1][];
            NETOUT[0] = new double[CountX1];


            for (int i = 0; i < countLayers; i++)
            {
                CountY1 = ReadFromArrayInt(binNW, ref k);
                Layers[i] = new LayerNW(CountX1, CountY1);
                CountX1 = CountY1;

                // Выделяем память
                NETOUT[i + 1] = new double[CountY1];
            }
            // Размерность выхода
            countY = CountY1;
            // Извлекаем и записываем сами веса
            for (int r = 0; r < countLayers; r++)
                for (int p = 0; p < Layers[r].getCountX; p++)
                    for (int q = 0; q < Layers[r].getCountY; q++)
                    {
                        Layers[r][p, q] = ReadFromArrayDouble(binNW, ref k);
                    }
        }

        // Извлекает переменную типа double из 8-ми байтов массива
        double ReadFromArrayDouble(byte[] mas, ref int pos)
        {
            DataToByte DTB = new DataToByte();
            DTB.b1 = mas[pos++];
            DTB.b2 = mas[pos++];
            DTB.b3 = mas[pos++];
            DTB.b4 = mas[pos++];
            DTB.b5 = mas[pos++];
            DTB.b6 = mas[pos++];
            DTB.b7 = mas[pos++];
            DTB.b8 = mas[pos++];

            return DTB.vDouble;
        }
    }
}
