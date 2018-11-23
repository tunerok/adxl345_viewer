using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using System.IO.Ports;



namespace Com_port

{
    public partial class Inp_f : Form
    {

        String ID_mk;

        public Inp_f()
        {
            InitializeComponent();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ID_mk = textBox1.Text;
            try
            {



                PortEr._port_finded = ID_mk;
                PortEr.Run_port();

                progressBar1.Value = 100;
            }
            catch { }

            if (PortEr.MkPortFound == false)
            {
                textBox1.Text = "COM";
                progressBar1.Value = 44;
                return;
            }
            else
            {

                this.Hide();
                Form1 frm1 = new Form1();
                frm1.ShowDialog();
                Application.Exit();
                //this.Close();
                // Application.Run(new Form1());

            }

        }
       
        private void Window_Closing(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
