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

namespace NewNN007
{
    public partial class Form1 : Form
    {
        
        NeuralNW NET;
        String path = "Network2.nw";
        bool g;
        double[][] TS;    
       

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            

        }

        private void LoadTs_Click(object sender, EventArgs e)
        {
            TS = Functional.getTS("TS3.txt");                               
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            g = true;
            try
            {
                NET = new NeuralNW(path, 2, 9);
                NET.GetMeanAtributes(TS);
                NET.ActivateGammaStruct(TS.Length);
                NET.InitializeWeights(TS);
                int count = 10;
                for (int i = 0; i < count; i++)
                {
                    NET.EraseErrors();
                    double[] err = NET.Learn(TS);
                    for (int j = 0; j < err.Length; j++)
                    {
                        textBox1.AppendText("ошибка = " + err[j].ToString() + "\r\n");
                        
                    }
                    textBox1.AppendText("!!!\r\n");                    
                }
                    
                    
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Все сломалось!!!!",MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            g = false;
        }

        
    }
}
