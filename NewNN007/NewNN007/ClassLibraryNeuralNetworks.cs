﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;


namespace ClassLibraryNeuralNetworks
{

    // Структура дря разбиения переменных типа int и double на байты
    [StructLayout(LayoutKind.Explicit)]
    internal class DataToByte
    {
        [FieldOffset(0)]
        public double vDouble;

        [FieldOffset(0)]
        public int vInt;

        [FieldOffset(0)]
        public byte b1;
        [FieldOffset(1)]
        public byte b2;
        [FieldOffset(2)]
        public byte b3;
        [FieldOffset(3)]
        public byte b4;
        [FieldOffset(4)]
        public byte b5;
        [FieldOffset(5)]
        public byte b6;
        [FieldOffset(6)]
        public byte b7;
        [FieldOffset(7)]
        public byte b8;
    }

    // Класс - слой нейросети
    public class LayerNW
    {
        double[,] Weights;
        int cX, cY;

        // Заполняем веса случайными числами
        public void GenerateWeights()
        {
            Random rnd = new Random();
            for (int i = 0; i < cX; i++)
            {
                for (int j = 0; j < cY; j++)
                {
                    Weights[i, j] = rnd.NextDouble() - 0.5;
                }
            }
        }

        // Выделяет память под веса
        protected void GiveMemory()
        {
            Weights = new double[cX, cY];
        }

        // Конструктор с параметрами. передается количество входных и выходных нейронов
        public LayerNW(int countX, int countY)
        {
            cX = countX;
            cY = countY;
            GiveMemory();
        }

        public int countX
        {
            get { return cX; }
        }

        public int countY
        {
            get { return cY; }
        }

        public double this[int row, int col]
        {
            get { return Weights[row, col]; }
            set { Weights[row, col] = value; }
        }

    }

    /// <summary> 
    /// Структура классов принятия решений
    /// </summary>
    public class ClassesNW
    {
        /// <summary>
        /// Массив средних значений атрибутов для класса принятия решений
        /// </summary>
        public double[] MeansAtributes;

        public int CountOfSamples;

        public int GetCountOfAtributes
        {
            get
            {
                return MeansAtributes.Length;
            }
        }

        /// <summary>
        /// Инициализирует массив средних значений атрибутов
        /// </summary>
        /// <param name="countOfAtributes">Количество атрибутов</param>
        public ClassesNW(int countOfAtributes)
        {
            MeansAtributes = new double[countOfAtributes];
            CountOfSamples = 0;
        }      
    }

    public class Gamma
    {
        
        public List<double[]> GammaValues;

        double gammaOFerrors = 0.0;

        public double GammaOfErrors
        {
            get { return gammaOFerrors; }
            set { gammaOFerrors = value; }
        }        

        public Gamma(LayerNW[] Layers)
        {
            GammaValues = new List<double[]>();
            for (int i = 0; i < Layers.Length; i++)
			{
                GammaValues.Add(new double[Layers[i].countX]);
			}
        }
    }

   
  
   
    

    // Класс - нейронная сеть
    public class NeuralNW
    {
        LayerNW[] Layers;
        ClassesNW[] Classes;
        Gamma[] GammaNeuron;
        
        int countLayers = 0, countX, countY;
        double[][] NETOUT;  // NETOUT[countLayers + 1][]
        double[][] DELTA;   // NETOUT[countLayers    ][]       
        double[] ERRORS;
        double[] DISCRIMINANT;
        double[] ACTIVATE_FUNCTIONS;
        Dictionary<double, double[]> PROBABILITY;
        
        ///Ниже представлены наши методы

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
            ActivateClasses(numberOfDecisionClasses, CountOfAtributes);
            ActivateProbability(numberOfDecisionClasses);
            DISCRIMINANT = new double[Layers[0].countY];
            InitializeZeroWeights();
            IntializeACTIVATE_FUNCTIONS();
        }

        private void IntializeACTIVATE_FUNCTIONS()
        {
            ACTIVATE_FUNCTIONS = new double[Layers[0].countY];
        }

        /// <summary>
        /// Инициализирует структуру Gamma
        /// </summary>
        /// <param name="CountOfTs">Количество строк обучающей выборки</param>
        public void ActivateGammaStruct(int CountOfTs)
        {
            GammaNeuron = new Gamma[CountOfTs];
            for (int i = 0; i < CountOfTs; i++)
            {
                GammaNeuron[i] = new Gamma(Layers);
            }
        }

        /// <summary>
        /// Происходит заполнение словаря вероятностей PROBABILITY
        /// </summary>
        /// <param name="numberOfDecisionClasses"></param>
        private void ActivateProbability(int numberOfDecisionClasses)
        {
            PROBABILITY = new Dictionary<double,double[]>();
            double maxProbability = (double)(1 - (numberOfDecisionClasses * 0.01) + 0.01);

            //Цикл по кол-ву классов принятия решений
            for (int i = 0; i < numberOfDecisionClasses; i++)
            {                
                //массив вероятностей для класса, заполняется автоматически
                double[] prob = new double[numberOfDecisionClasses];
                for (int j = 0; j < prob.Length; j++) { prob[j] = 0.01; }
                prob[i] = maxProbability;
                double key = (double) (i + 1);
                PROBABILITY.Add(key, prob);
            }
        }

        /// <summary>
        /// Инициализирует структуру Classes
        /// </summary>
        /// <param name="numberOfDecisionClasses">Количество классов принятие решений</param>
        /// <param name="GetCountOfAtributes">Количество атрибутов</param>
        private void ActivateClasses(int numberOfDecisionClasses, int CountOfAtributes)
        {
            Classes = new ClassesNW[numberOfDecisionClasses];
            for (int i = 0; i < numberOfDecisionClasses; i++)
            {
                Classes[i] = new ClassesNW(CountOfAtributes);
            }
        }

        

        /// <summary>
        /// Заполняем структуру Classes средними значениями атрибутов
        /// </summary>
        /// <param name="TS">Обучающая выборка</param>
        public void GetMeanAtributes(double[][] TS)
        {
            //Инициализируем массив ошибок
            ActivateErrors(TS.Length);

            for (int i = 0; i < TS.Length; i++)
            {
                ///Берем последнее значение(выход) из strTS-ой строчки выборки,
                ///конвентируем в intЮ вычитаем 1
                ///и получаем индекс структуры Classes =)
                int numberOfClass = Convert.ToInt32(TS[i][TS[i].Length - 1]) - 1;

                //Увеличиваем счетчик выборки, принадлежащей к классу
                Classes[numberOfClass].CountOfSamples++;

                int CountOfAtributes = Classes[0].GetCountOfAtributes;
                for (int x = 0; x < CountOfAtributes; x++)
                {
                    Classes[numberOfClass].MeansAtributes[x] += TS[i][x];
                }                
            }

            //Проходимся по структуре Classes, находим среднее атрубутов
            for (int i = 0; i < Classes.Length; i++)
            {
                for (int j = 0; j < Classes[i].GetCountOfAtributes; j++)
                {
                    Classes[i].MeansAtributes[j] /= Classes[i].CountOfSamples;
                }
            }
        }

        /// <summary>
        /// Инициализируется массив ошибок
        /// </summary>
        /// <param name="length"></param>
        private void ActivateErrors(int length)
        {
            ERRORS = new double[length];
        }
    
        
        /// <summary>
        /// Нахождение корреляции по методу Л.Гудмена и Е.Краскала
        ///
        /// </summary>
        /// <param name="pairs">Массив пар</param>
        /// <returns></returns>
        public double CalcGamma(double[][] pairs)
        {
            //счетчики согласованных и несогласованных пар соответственно.
            int countS = 0;
            int countD = 0;

            int count = 0;
            for (int i = 0; i < pairs.Length - 1; i++)
            {
                
                for (int j = 0 + count; j < pairs.Length - 1; j++)
                {
                    if (pairs[i][0] > pairs[j + 1][0] && pairs[i][1] > pairs[j + 1][1]) { countS++; }
                    else if (pairs[i][0] < pairs[j + 1][0] && pairs[i][1] < pairs[j + 1][1]) { countS++; }
                    else countD++;
                } 
                count++;
                
            }
            ////мера Гамма Л.Гудмена и Е.Краскала
            double a = countS - countD;
            double b = countS + countD;
            double gamma = a / b;
            return gamma;            

        }

        private void InitializeZeroWeights()
        {
            for (int k = 0; k < Layers.Length; k++)
            {
                for (int i = 0; i < Layers[k].countX; i++)
                {
                    for (int j = 0; j < Layers[k].countY; j++)
                    {
                        Layers[k][i, j] = 0.0;
                    }
                }
            }
        }

        public void InitializeWeights(double[][] TS)
        {
            for (int k = 0; k < Layers.Length; k++)
            {
                for (int i = 0; i < Layers[k].countX; i++)
                {
                    double[] currentAttributes = new double[TS.Length];

                    for (int a = 0; a < TS.Length; a++)
                    {
                        currentAttributes[a] = TS[a][i];
                    }

                    for (int j = 0; j < Layers[k].countY; j++)
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
                            double[] temp = new double[Layers[k].countY];
                            foreach (KeyValuePair<double, double[]> entry in PROBABILITY)
                            {
                                if (entry.Key == classLabel)
                                {
                                    temp = entry.Value;
                                }
                            }
                            massForGamma[a][0] = atribute_minus_avg[a];
                            massForGamma[a][1] = temp[j];

                        }

                        Layers[k][i, j] = CalcGamma(massForGamma);
                    }
                }
            }
        }

        private double Max(double[] mass)
        {
            Array.Sort(mass);
            double Max = mass[mass.Length - 1];
            return Max;
            
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

                for (int l = 0; l < Layers[0].countX; l++)
                {
                    NETOUT[0][l] = ts[l];
                }

                for (int i = 0; i < Layers[0].countY; i++)
                {
                    double U = 0.0;
                    for (int j = 0; j < Layers[0].countX; j++)
                    {
                        double num1 = NETOUT[0][j] * Layers[0][j, i];
                        int label = Convert.ToInt32(TS[t][TS[t].Length - 1] - 1);
                        double num2 = Math.Pow(NETOUT[0][j] - Classes[label].MeansAtributes[i], 2);
                        U += (num1 / num2);
                    }
                    DISCRIMINANT[i] = U;
                }

                for (int l = 0; l < DISCRIMINANT.Length; l++)
                {
                    //double s = 1.0 / (1 + Math.Exp(-DISCRIMINANT[l]));
                    
                    double W = 1.7159 * Math.Tanh(DISCRIMINANT[l] * 2 / 3);
                    ACTIVATE_FUNCTIONS[l] = W;
                    NETOUT[1][l] = W;
                }

                double[] temp = new double[Layers[0].countY];
                foreach (KeyValuePair<double, double[]> entry in PROBABILITY)
                {
                    if (entry.Key == ts[ts.Length - 1])
                    {
                        temp = entry.Value;
                    }
                }

                for (int l = 0; l < NETOUT[1].Length; l++)
                {
                    double G = 0.0;

                    double num1 = Math.Exp(ACTIVATE_FUNCTIONS[l]) - Max(ACTIVATE_FUNCTIONS);
                    double num2 = 0.0;
                    for (int b = 0; b < ACTIVATE_FUNCTIONS.Length; b++)
                    {
                        num2 += ACTIVATE_FUNCTIONS[b] - Max(ACTIVATE_FUNCTIONS);
                    }
                    G = num1 / num2;

                    ERRORS[t] += temp[l] - G;

                }
            }

            //change weights
            for (int i = 0; i < Layers[0].countX; i++)
            {
                double[] currentAttributes = new double[TS.Length];

                for (int a = 0; a < TS.Length; a++)
                {
                    currentAttributes[a] = TS[a][i];
                }
                
                for (int j = 0; j < Layers[0].countY; j++)
                {                    

                    double[] atribute_minus_avg = new double[currentAttributes.Length];

                    for (int a = 0; a < currentAttributes.Length; a++)
                    {
                        atribute_minus_avg[a] = currentAttributes[a] - Classes[j].MeansAtributes[i];
                    }

                    double[][] massForGamma = new double[currentAttributes.Length][];
                    double[][] massForErrors = new double[currentAttributes.Length][];

                    for (int a = 0; a < TS.Length; a++)
                    {
                        massForGamma[a] = new double[2];
                        double classLabel = TS[a][TS[a].Length - 1];
                        double[] temp = new double[Layers[0].countY];
                        foreach (KeyValuePair<double, double[]> entry in PROBABILITY)
                        {
                            if (entry.Key == classLabel)
                            {
                                temp = entry.Value;
                            }
                        }
                        massForGamma[a][0] = atribute_minus_avg[a];
                        massForGamma[a][1] = temp[j];

                        massForErrors[a] = new double[2];
                        massForErrors[a][0] = atribute_minus_avg[a];
                        massForErrors[a][1] = ERRORS[a];
                    }

                    double chislitel = CalcGamma(massForErrors) * CalcGamma(massForGamma);
                    double znameenatel = 0.0;
                    //////////////////////////////////
                    for (int a = 0; a < Classes[0].GetCountOfAtributes; a++)
                    {
                        double[] current = new double[TS.Length];

                        for (int z = 0; z < TS.Length; z++)
                        {
                            current[z] = TS[z][a];
                        }

                        double[] atribute_minus_AVG = new double[currentAttributes.Length];

                        for (int m = 0; m < current.Length; m++)
                        {
                            atribute_minus_AVG[m] = current[m] - Classes[j].MeansAtributes[a];
                        }

                        double[][] massCurrentForGamma = new double[current.Length][];
                        double[][] massCurrentForErrors = new double[current.Length][];

                        for (int v = 0; v < TS.Length; v++)
                        {
                            massCurrentForGamma[v] = new double[2];
                            double classLabel = TS[v][TS[v].Length - 1];
                            double[] temp = new double[Layers[0].countY];
                            foreach (KeyValuePair<double, double[]> entry in PROBABILITY)
                            {
                                if (entry.Key == classLabel)
                                {
                                    temp = entry.Value;
                                }
                            }
                            massCurrentForGamma[v][0] = atribute_minus_AVG[v];
                            massCurrentForGamma[v][1] = temp[j];

                            massCurrentForErrors[v] = new double[2];
                            massCurrentForErrors[v][0] = atribute_minus_AVG[v];
                            massCurrentForErrors[v][1] = ERRORS[v];
                        }
                        znameenatel += CalcGamma(massCurrentForErrors) * CalcGamma(massCurrentForGamma);
                    }

                    Layers[0][i, j] = chislitel / znameenatel;  
                }
                        
            }

            return ERRORS;
            
        }

        public void EraseErrors()
        {
            for (int i = 0; i < ERRORS.Length; i++)
            {
                ERRORS[i] = 0.0;
            }
        }
        public double AverageArray(double[] mass)
        {
            double result = 0.0;
            foreach (double item in mass) { result += item; }
            return result;

        }

        public void ActivateErrorMass(double[][] TS)
        {
            ERRORS = new double[TS.Length];
        }

        public double CalcError(double[] TS)
        {
            double kErr = 0;
            double[] Y = new double[Classes.Length];
            foreach (KeyValuePair<double, double[]> item in PROBABILITY)
            {
                if (item.Key.Equals(TS[TS.Length - 1]))
                {
                    Y = item.Value;
                    break;
                }
            }

            for (int i = 0; i < Y.Length; i++)
            {
                kErr += Math.Pow(Y[i] - NETOUT[countLayers][i], 2);
            }
            
            return kErr * 0.5;

        }

        ///Ниже методы, уже присутствующие в библиотеке!!!

        // Конструкторы
        /* Создает полносвязанную сеть из 1 слоя. 
           sizeX - размерность вектора входных параметров
           sizeY - размерность вектора выходных параметров */
        public NeuralNW(int sizeX, int sizeY)
        {
            countLayers = 1;
            Layers = new LayerNW[countLayers];
            Layers[0] = new LayerNW(sizeX, sizeY);
            Layers[0].GenerateWeights();
        }

        /* Создает полносвязанную сеть из n слоев. 
           sizeX - размерность вектора входных параметров
           layers - массив слоев. Значение элементов массива - количество нейронов в слое               
         */
        public NeuralNW(int sizeX, params int[] layers)
        {
            countLayers = layers.Length;
            countX = sizeX;
            countY = layers[layers.Length - 1];
            // Размерность выходов нейронов и Дельты
            NETOUT = new double[countLayers + 1][];
            NETOUT[0] = new double[sizeX];
            DELTA = new double[countLayers][];

            this.Layers = new LayerNW[countLayers];

            int countY1, countX1 = sizeX;
            // Устанавливаем размерность слоям и заполняем слоя случайнымичислами
            for (int i = 0; i < countLayers; i++)
            {
                countY1 = layers[i];

                NETOUT[i + 1] = new double[countY1];
                DELTA[i] = new double[countY1];

                this.Layers[i] = new LayerNW(countX1, countY1);
                this.Layers[i].GenerateWeights();
                countX1 = countY1;
            }
        }

        // Открывает НС
        public NeuralNW(String FileName)
        {
            OpenNW(FileName);
        }  
       
        // Открывает НС
        public void OpenNW(String FileName)
        {
            byte[] binNW = File.ReadAllBytes(FileName);

            int k = 0;
            // Извлекаем количество слоев из массива
            countLayers = ReadFromArrayInt(binNW, ref k);
            Layers = new LayerNW[countLayers];

            // Извлекаем размерность слоев
            int CountY1=0, CountX1 = ReadFromArrayInt(binNW, ref k);
            // Размерность входа
            countX = CountX1;
            // Выделяемпамять под выходы нейронов и дельта
            NETOUT = new double[countLayers + 1][];
            NETOUT[0] = new double[CountX1];
            DELTA = new double[countLayers][];

            for (int i = 0; i < countLayers; i++)
            {
                CountY1 = ReadFromArrayInt(binNW, ref k);
                Layers[i] = new LayerNW(CountX1, CountY1);
                CountX1 = CountY1;

                // Выделяем память
                NETOUT[i + 1] = new double[CountY1];
                DELTA[i] = new double[CountY1];
            }
            // Размерность выхода
            countY = CountY1;
            // Извлекаем и записываем сами веса
            for (int r = 0; r < countLayers; r++)
                for (int p = 0; p < Layers[r].countX; p++)
                    for (int q = 0; q < Layers[r].countY; q++)
                    {
                        Layers[r][p, q] = ReadFromArrayDouble(binNW, ref k);
                    }
        }

        // Сохраняет НС
        public void SaveNW(String FileName)
        {
            // размер сети в байтах
            int sizeNW = GetSizeNW();
            byte[] binNW = new byte[sizeNW];

            int k = 0;
            // Записываем размерности слоев в массив байтов
            WriteInArray(binNW, ref k, countLayers);
            if (countLayers <= 0)
                return;

            WriteInArray(binNW, ref k, Layers[0].countX);
            for (int i = 0; i < countLayers; i++)
                WriteInArray(binNW, ref k, Layers[i].countY);

            // Зпаисвыаем сами веса
            for (int r = 0; r < countLayers; r++)
                for (int p = 0; p < Layers[r].countX; p++)
                    for (int q = 0; q < Layers[r].countY; q++)
                    {
                        WriteInArray(binNW, ref k, Layers[r][p, q]);
                    }


            File.WriteAllBytes(FileName, binNW);
        }

        // Возвращает значение j-го слоя НС
        public void NetOUT(double[] inX, out double[] outY, int jLayer)
        {
            GetOUT(inX, jLayer);
            int N = NETOUT[jLayer].Length;

            outY = new double[N];

            for (int i = 0; i < N; i++)
            {
                outY[i] = NETOUT[jLayer][i];
            }

        }

        // Возвращает значение НС
        public void NetOUT(double[] inX, out double[] outY)
        {
            int j = countLayers;
            NetOUT(inX, out outY, j);
        }

        // Возвращает ошибку (метод наименьших квадратов)
        public double CalcError(double[] X, double[] Y)
        {
            double kErr = 0;
            for (int i = 0; i < Y.Length; i++)
            {
                kErr += Math.Pow(Y[i] - NETOUT[countLayers][i], 2);
            }

            return 0.5 * kErr;
        }

        

        /* Обучает сеть, изменяя ее весовые коэффициэнты. 
           X, Y - обучающая пара. kLern - скорость обучаемости
           В качестве результата метод возвращает ошибку 0.5(Y-outY)^2 */
        public double LernNW(double[] X, double[] Y, double kLern)
        {
            double O;  // Вход нейрона
            double s;

            // Вычисляем выход сети
            GetOUT(X);

            // Заполняем дельта последнего слоя
            for (int j = 0; j < Layers[countLayers - 1].countY; j++)
            {
                O = NETOUT[countLayers][j];
                DELTA[countLayers - 1][j] = (Y[j] - O) * O * (1 - O);
            }

            

            // Перебираем все слои начиная споследнего 
            // изменяя веса и вычисляя дельта для скрытого слоя
            for (int k = countLayers - 1; k >= 0; k--)
            {
                // Изменяем веса выходного слоя
                for (int j = 0; j < Layers[k].countY; j++)
                {
                    for (int i = 0; i < Layers[k].countX; i++)
                    {
                        Layers[k][i, j] += kLern * DELTA[k][j] * NETOUT[k][i];
                    }
                }
                if (k > 0)
                {

                    // Вычисляем дельта слоя к-1
                    for (int j = 0; j < Layers[k - 1].countY; j++)
                    {

                        s = 0;
                        for (int i = 0; i < Layers[k].countY; i++)
                        {
                            s += Layers[k][j, i] * DELTA[k][i];
                        }

                        DELTA[k - 1][j] = NETOUT[k][j] * (1 - NETOUT[k][j]) * s;
                    }
                }
            }

            return CalcError(X, Y);
        }


        //Я НАЧИНАЮ
        //МОЙ АЛГОРИТСМ
        //public double ByesLernNW(double[] X, double[] Y, double mean)
        //{
        //    //Layers - это веса, меняющиеся в предыдущем методе
        //    double O;  // Вход нейрона
        //    double[] g = new double[Y.Length]; //ДИСКРЕТНАЯ ПЕРЕМЕННАЯ СКРЫТОГО НЕЙРОНА П.6
        //    double[] fun; //МАССИВ ЗНАЧЕНИЙ ФУНКЦИИ АКТИВАЦИИ НА КАЖДОМ ШАГЕ
        //    double sum = 0; 
        //    double[] U; //линейный дискриминант для каждого скрытого слоя
        //    int[][] y = new int[Y.Length][]; //МЕТКА КЛАССА ПРИНЯТИЯ РЕШЕНИЙ
        //    y[0] = new int[Y.Length];

        //    for (int k = countLayers - 1; k >= 0; k--) //цикл по скрытым слоям
        //    {
        //        U = new double[Layers[k].countY];
        //        fun = new double[Layers[k].countY];

        //        for (int j = 0; j < Layers[k].countY; j++) //цикл по нейронам скрытого слоя
        //        {

        //            for (int strTS = 0; strTS < X.Length; strTS++)                    
        //            {   //3 пункт. линейный дискриминант
        //                U[j] += X[strTS] * (Layers[k][strTS, j]) / ((X[k] - mean) * (X[k] - mean));
        //            }

        //            fun[j] = func(U[j]);
        //            for (int strTS = 0; strTS < X.Length; strTS++)
        //                sum += fun[strTS] - max(fun, strTS);
                        
        //            //5п. метка уj
        //            for (int strTS = Y.Length; strTS > 0; strTS--) //цикл по классам принятия решений
        //            {
        //                for (int p = 0; p < Y.Length; j++)
        //                {
        //                    if ((strTS + p) == Y.Length) y[strTS][p] = 1;
        //                    else y[strTS][j] = 0;
        //                }
        //                //6п.
        //                g[strTS] = (Math.Exp(fun[j])-max(fun, strTS))/sum;
        //            }
        //        }

        //    }

        //    // Вычисляем выход сети
        //    GetOUT(X);

        //    for (int j = 0; j < Layers[countLayers - 1].countY; j++)
        //    {
        //        O = NETOUT[countLayers][j];
                
        //    }

        //    return CalcError(X, Y);
        //}

        //public void StartLayes(NeuralNW NET, double[][] input, double[][] output, double[] decision, double[][] atributes)
        //{
            
        //    for (int l = 0; l < NET.Layers.Length; l++)
        //    {
        //        for (int strTS = 0; strTS < NET.Layers[l].countX; strTS++)
        //        {
        //            for (int j = 0; j < NET.Layers[l].countY; j++)
        //            {
        //                //NET.Layers[l][strTS,j] = Gamma()
        //            }
        //        }
        //        //NET.Layers[strTS]
        //    }

        //}


        //нелинейная активационная функция СИГМОИД
        //public double func(double Uj)
        //{
        //    double result = 1/(1 + Math.Exp(Uj));
        //    return result;
        //}
        //maximum
        //public double max(double[] mu, int strTS)
        //{
        //    System.Array.Sort(mu);
        //    double max = mu[mu.Length - 1];
        //    return max;
        //}
        //ЗДЕСЬ ДОЛЖНЫ ВЕСА ПОМЕНЯТЬСЯ
        //public void weight(double[] X, double[] Y, double[] E, double mean)
        //{
        //    double znam = 0;
        //    for (int k = countLayers - 1; k >= 0; k--)
        //    {
        //        for (int j = 0; j < Layers[k].countY; j++)
        //        {
        //            for (int strTS = 0; strTS < Layers[k].countX; strTS++)
        //            {
        //                //Вызов Гамма-Корреляции

        //             //   znam += Gamma((X[strTS] - mean), E[k]) * Gamma((X[strTS] - mean), Y[j]);
        //             //  Layers[k][strTS, j] += (Gamma((X[strTS] - mean), E[k]) * Gamma((X[strTS] - mean), Y[j]))/znam;
        //            }
        //        }
        //    }

        //}

        //гамма-корреляция 
        //посылаем два нейрона с двумя значениями - на вход и на выход для каждого нейрона
       
        //Я ЗАКАНЧИВАЮ




        // Свойства. Возвращает число входов и выходов сети

        

        public int GetX
        {
            get { return countX; }
        }

        public int GetY
        {
            get { return countY; }
        }

        public int CountLayers
        {
            get { return countLayers; }
        }
        /* Вспомогательные закрытые функции */

        // Возвращает все значения нейронов до lastLayer слоя
        void GetOUT(double[] inX, int lastLayer)
        {
            double s;

            for (int j = 0; j < Layers[0].countX; j++)
                NETOUT[0][j] = inX[j];

            for (int i = 0; i < lastLayer; i++)
            {
                // размерность столбца проходящего через strTS-й слой
                for (int j = 0; j < Layers[i].countY; j++)
                {
                    s = 0;
                    for (int k = 0; k < Layers[i].countX; k++)
                    {
                        s += Layers[i][k, j] * NETOUT[i][k];
                    }

                    // Вычисляем значение активационной функции
                    s = 1.0 / (1 + Math.Exp(-s));
                    NETOUT[i + 1][j] = 0.998 * s + 0.001;

                }
            }

        }

        // Возвращает все значения нейронов всех слоев
        void GetOUT(double[] inX)
        {
            GetOUT(inX, countLayers);
        }

        // Возвращает размер НС в байтах
        int GetSizeNW()
        {
            int sizeNW = sizeof(int) * (countLayers + 2);
            for (int i = 0; i < countLayers; i++)
            {
                sizeNW += sizeof(double) * Layers[i].countX * Layers[i].countY;
            }
            return sizeNW;
        }

        // Возвращает classLabel-й слой Нейронной сети
        public LayerNW Layer(int num)
        {
            return Layers[num]; 
        }

        // Разбивает переменную типа int на байты и записывает в массив
        void WriteInArray(byte[] mas, ref int pos, int value)
        {
            DataToByte DTB = new DataToByte();
            DTB.vInt = value;
            mas[pos++] = DTB.b1;
            mas[pos++] = DTB.b2;
            mas[pos++] = DTB.b3;
            mas[pos++] = DTB.b4;
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
