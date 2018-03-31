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
        String path = "Network.nw";
        
        public ArrayList TS = new ArrayList();
        public double[] decision = new double[3] { 1.0, 2.0, 3.0 };
        public ArrayList atributes = new ArrayList();
        public double[][] input, output;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            

        }

        private void LoadTs_Click(object sender, EventArgs e)
        {
            TS = Functional.getTS("TS.txt");
            atributes = Functional.getAtributes(TS);
            input = Functional.getInput(TS);
            output = Functional.getOutput(TS);
           
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                NET = new NeuralNW(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "aaaaa",MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            string s;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int currPos = 0;
            double kErr = 1E256;
            double kErrNorm = 0.01;
            double kLern = 0.1;

            while (kErr > kErrNorm)
            {
                kErr = 0;
                for (currPos = 0; currPos < NET.getHidenCount; currPos++)
                {
                    string s;
                    NET.StartLayes(NET);
                    
                }
            }
        }

        
    }
}
