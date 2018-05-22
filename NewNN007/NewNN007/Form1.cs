using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClassLibraryNeuralNetworks;
using System.IO;
using System.Collections;
using System.Threading;

namespace NewNN007
{
    public partial class Form1 : Form
    {
        
        NeuralNW NET;
        String path, testPath;
        bool pathNW = false, start;
        double[][] TS;    
       

        public Form1()
        {
            InitializeComponent();
            //this.Size = new Size(560, 560);
            ////this.Width = 745;
            ////this.Height = 560;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            
           

        }

        private void LoadTs_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    tsPath.Text = openFileDialog.FileName;
                    TS = Functional.getTS(openFileDialog.FileName);   
                }
            }                                       
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        path = openFileDialog.FileName;
                    }
                }
                NET = new NeuralNW(path, 2, 9);
                NET.GetMeanAtributes(TS);
                NET.ActivateGammaStruct(TS.Length);
                NET.InitializeWeights(TS);
                pathNW = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Все сломалось!!!!",MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            openNWLabel.Text = chech();
        
        }

        private string chech()
        {
            return pathNW == true ? "Сеть загружена" : "Сеть не загружена";
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            
            if (!pathNW)
            {
                openNWLabel.Text = chech();
            }
            else
            {
                if(countFOR.Text == "") textBox1.Text = "Введите количество циклов обучения!";
                else
                {
                    int count = Convert.ToInt32(countFOR.Text);

                    for (int i = 0; i < count; i++)
                    {
                        textBox1.Text = "";
                        
                        NET.EraseErrors();

                        double[] err = NET.Learn(TS);

                        for (int j = 0; j < err.Length; j++)
                        {
                            string text = string.Format("Выборка № {0}. Error: {1} \r\n", (j + 1), err[j].ToString());
                            textBox1.AppendText(text);
                        }

                        textBox1.AppendText("Цикл обучения завершен. \r\n");
                        textBox1.AppendText("Количество итераций: " + (i + 1));
                    }
                    
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            pathNW = false;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            using (OpenFileDialog open = new OpenFileDialog())
            {
                if (open.ShowDialog() == DialogResult.OK)
                {
                    testPath = open.FileName;
                    textBox2.Text = testPath;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            double[] ts;
            using (StreamReader sr = new StreamReader(testPath))
            {
                string[] tmp = sr.ReadLine().Split(';');
                ts = new double[tmp.Length];
                for (int i = 0; i < tmp.Length; i++) { ts[i] = Double.Parse(tmp[i]); }
            }
            double[] test = NET.TEST(ts);////????????
            

            //Проверка работы сети
           // NET.NetOUT
        }

        

        
    }
}
