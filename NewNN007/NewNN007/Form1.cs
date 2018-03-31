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
        
        public ArrayList TS = new ArrayList();
        public double[] decision = new double[3] { 1.0, 2.0, 3.0 };
        public ArrayList atributes = new ArrayList();

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
        }

        
    }
}
