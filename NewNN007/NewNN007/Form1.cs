using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Threading;
using System.Diagnostics;

namespace NewNN007
{
    public partial class Form1 : Form
    {
        
        NeuralNW NET;
        String path, testPath;
        bool pathNW = false, start, pathTS = false;
        double[][] TS;
        double[] testTS;

        private void method2()
        {
            if (pathNW && pathTS) startButton.Enabled = true;
        }
       

        public Form1()
        {
            InitializeComponent();
            //this.Size = new Size(560, 560);
            ////this.Width = 745;
            ////this.Height = 560;
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Method1(true);
            startButton.Enabled = false;
            
           

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
            pathTS = true;          
            
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

                        Stopwatch time = new Stopwatch();
                        time.Start();
                        double[] err = NET.Learn(TS);
                        time.Stop();
                        for (int j = 0; j < err.Length; j++)
                        {
                            string text = string.Format("Выборка № {0}. Error: {1} \r\n", (j + 1), err[j].ToString());
                            textBox1.AppendText(text);
                        }
                        double timer = time.ElapsedMilliseconds * 0.001;
                        textBox1.AppendText("Цикл обучения завершен. \r\n");
                        textBox1.AppendText("Времени затрачено: " + timer + " секунд \r\n");
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

        private void Method1(bool flag)
        {
            textBox4.ReadOnly = flag;
            textBox5.ReadOnly = flag;
            textBox6.ReadOnly = flag;
            textBox7.ReadOnly = flag;
            textBox8.ReadOnly = flag;
            textBox9.ReadOnly = flag;
            textBox10.ReadOnly = flag;
            textBox11.ReadOnly = flag;
            textBox12.ReadOnly = flag;
            
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

            
            using (StreamReader sr = new StreamReader(testPath))
            {
                string[] tmp = sr.ReadLine().Split(';');
                testTS = new double[tmp.Length];
                for (int i = 0; i < tmp.Length; i++) { testTS[i] = Double.Parse(tmp[i]); }
            }

            textBox4.Text = testTS[0].ToString();
            textBox5.Text = testTS[1].ToString();
            textBox6.Text = testTS[2].ToString();
            textBox7.Text = testTS[3].ToString();
            textBox8.Text = testTS[4].ToString();
            textBox9.Text = testTS[5].ToString();
            textBox10.Text = testTS[6].ToString();
            textBox11.Text = testTS[7].ToString();
            textBox12.Text = testTS[8].ToString();

            Method1(false);
               
        }

        private void button4_Click(object sender, EventArgs e)
        {

            double[] answer = NET.TEST(this.testTS);
            for (int i = 0; i < answer.Length; i++)
            {
                textBox3.AppendText(String.Format("Выходной нейрон: {0} --- {1} \n", i+1 , Math.Round(answer[i], 2)));
            }
        }

        private void countFOR_TextChanged(object sender, EventArgs e)
        {
            method2();
        }

        

        
    }
}
