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
using System.Threading;

namespace Com_port
{
    public partial class Form1 : Form
    {
        private delegate void InvokeDelegate();
        public delegate void DisplayHandler();

        System.Timers.Timer aTimer;
        PointF[] points = new PointF[717];
        PointF[] points2 = new PointF[717];
        PointF[] points3 = new PointF[717];

        PointF[] lpf = new PointF[717];
        PointF[] lpf2 = new PointF[717];
        PointF[] lpf3 = new PointF[717];

        String S = "";
        double Max_voltage;
        double current_inp = 0;
        double current_inp2 = 0;
        double current_inp3 = 0;
        Graphics g;
        Bitmap b;
        int conn_counter = 0;
        bool connect = true;
        bool paused = false;
        Pen pen = new Pen(Color.Black);
        Pen pen2 = new Pen(Color.Red);
        Pen pen3 = new Pen(Color.Blue);
        Pen pen_lvl = new Pen(Color.Red);
        Pen pen_lvl2 = new Pen(Color.DarkMagenta);
        Pen pen_lvl3 = new Pen(Color.DeepPink);

        Pen pen_lf = new Pen(Color.DeepPink);
        Pen pen_lf2 = new Pen(Color.DarkGreen); 
         Pen pen_lf3 = new Pen(Color.DarkRed);

        SolidBrush fig = new SolidBrush(Color.White);
        float graph_maxy, graph_miny, graph_maxy2, graph_miny2, graph_maxy3, graph_miny3;
        DisplayHandler handler;
        int writer_counter = 5000;
        int cycle_counter = 0;
        double res = 0.0;
        double r = 0.051;
        PointF[] bezie_x = new PointF[178];
        PointF[] bezie_x2 = new PointF[178];
        PointF[] bezie_x3 = new PointF[178];
        int counter_b_x = 0;
        int counter_b_y = 0;
        int counter_b_z = 0;
        int n_mean = 5;
        int lpf_win = 5;


        int x_in, y_in, z_in;
        string[] num = new string[3];

        int nx = 300;
        int ny = 300;
        int nz = 300;

        public Form1()
        {

            InitializeComponent();
            textBox2.Text = PortEr._ID_mk;
            label2.Text = PortEr._port_finded;//выводим номер порта
            S = "0";
           // PortEr.Run_port();
            b = new Bitmap(pictureBox1.Width, pictureBox1.Height);//сразу объявим картинку как графику ,чтобы упростить с ней взаимодействие
            g = Graphics.FromImage(b);
            
            Max_voltage = Convert.ToDouble(textBox1.Text);
            for (int i = 0; i < 717; i++)
            {//инициализируем нужные нам точки 
                points[i].X = i;
                points[i].Y = (float)Max_voltage/2;
                points2[i].X = i;
                points2[i].Y = (float)Max_voltage / 2;
                points3[i].X = i;
                points3[i].Y = (float)Max_voltage / 2;

                lpf[i].X = i;
                lpf[i].Y = (float)Max_voltage / 2;
                lpf2[i].X = i;
                lpf2[i].Y = (float)Max_voltage / 2;
                lpf3[i].X = i;
                lpf3[i].Y = (float)Max_voltage / 2;

            }
            g.FillRectangle(fig, 0, 0, pictureBox1.Width, pictureBox1.Height);
            
            handler = new DisplayHandler(updateImageBox);
            My_txt_Writer.Ini_file_writer();
            Main();
        }

        private void tbAux_SelectionChanged(object sender, EventArgs e)//метод для объединения потоков(пока хз насколько в таком виде это заработает, но я думаю все будет более-менее)
        {
            
            Main();
        }

        void Main()
        {
           
                aTimer = new System.Timers.Timer(50);//тут у нас второй таймер, для отрисовки и чека порта
                aTimer.Elapsed += OnTimedEvent;//вызываем событие по "прерыванию"
                aTimer.AutoReset = true;//авторестарт
                aTimer.Enabled = true;
            
            
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)//Само событие просто считывает значение переменной, которая была забита из класса управления портом
        {
            if (checkBox5.Checked)//inversion
            {
                pen.Color = Color.Green;
                //pen_lvl.Color = Color.Blue;
                fig.Color = Color.Black; 
            }
            else
            {
                pen.Color = Color.Black;
               //pen_lvl.Color = Color.Red;
                fig.Color = Color.White;
            }
            conn_counter = conn_counter + 1;
            if (conn_counter > 20)
            {

               // connect = PortEr.Check_connection();
              //  System.Threading.Thread.Sleep(2000);
                conn_counter = 0;
            }
            try
            {
                if (connect)
                {

                    //BeginInvoke(new InvokeDelegate(updateImageBox));
                    

                    
					
					
                    handler.Invoke();
					if (PortEr.isChanged){
						PortEr.isChanged = false;
                        cycle_counter++;
                        if (cycle_counter > writer_counter)
                        {
                            My_txt_Writer.Close_file();
                            My_txt_Writer.Ini_file_writer();
                            cycle_counter = 0;
                        }

                        S = PortEr.strFromPort;
                        num = S.Split(' ');
                        x_in = Int32.Parse(num[0]);
                        y_in = Int32.Parse(num[1]);
                        z_in = Int32.Parse(num[2]);
                        My_txt_Writer.Append_to_file(x_in.ToString(), y_in.ToString(), z_in.ToString());
						//button4.BeginInvoke((Action)delegate () { button4.Enabled = false; });
						
						ThreadHelperClass.SetText(this, label6, x_in.ToString()); //PortEr._port_finded);
					}
                }
                else
                {
                    ThreadHelperClass.SetText(this, label6, "Не найдено");
                    //button4.BeginInvoke((Action)delegate () { button4.Enabled = true; });

                }
            }
            catch(Exception ex) {
             //   MessageBox.Show(ex.ToString());
            }

            

            //throw new NotImplementedException();
        }

        private void updateImageBox()
        {
            try
            {
                current_inp = Convert.ToDouble(x_in + nx);
                current_inp2 = Convert.ToDouble(y_in + ny);
                current_inp3 = Convert.ToDouble(z_in + nz);
                graph_miny = (float)Max_voltage;
                graph_maxy = 1;

                graph_miny2 = (float)Max_voltage;
                graph_maxy2 = 1;

                graph_miny3 = (float)Max_voltage;
                graph_maxy3 = 1;

                Drawer_my(current_inp, current_inp2, current_inp3, Max_voltage);
            }
            catch { }

            //textBox1.Text = S;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            nx = Int32.Parse(textBox3.Text);
            ny = Int32.Parse(textBox4.Text);
            nz = Int32.Parse(textBox5.Text);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            n_mean = Int32.Parse(textBox6.Text);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            lpf_win = Int32.Parse(textBox7.Text);
            if (lpf_win > 716)
                lpf_win = 716;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Max_voltage = Convert.ToDouble(textBox1.Text);//считываем опорное напряжение
        }


        private void speed_changed(object sender, EventArgs e)
        {
            aTimer.Interval = 50d / (double)numericUpDown1.Value;
        }

        private void Drawer_ini()
        {

        }

        //
        //Функция отрисовки. каждую итерацию смещает все изображение на один писель влево и записывает новое значение  
        //в правую часть. Принимает на вход две переменных - полученное значение с АЦП МК и заданное пользователем или дефолтное опорное наряжение
        private void Drawer_my(double r, double r2, double r3, double v)
        {

            g.Clear(fig.Color);
            double t_inp, t_inp2, t_inp3;
            int i = 0;

            //приводим к текушей канве
            t_inp = r / v;
            t_inp *= b.Height;

            t_inp2 = r2 / v;
            t_inp2 *= b.Height;

            t_inp3 = r3 / v;
            t_inp3 *= b.Height;

            t_inp = b.Height - t_inp + 1;
            t_inp2 = b.Height - t_inp2 + 1;
            t_inp3 = b.Height - t_inp3 + 1;

            if (t_inp > b.Height - 1)
                t_inp = b.Height - 1;

            if (t_inp2 > b.Height - 1)
                t_inp2 = b.Height - 1;

            if (t_inp3 > b.Height - 1)
                t_inp3 = b.Height - 1;

            t_inp = Math.Round(t_inp);
            t_inp2 = Math.Round(t_inp2);
            t_inp3 = Math.Round(t_inp3);


            for (; i < 716; i++)
            {//перебираем все элементы, кроме самого последнего 
                points[i].Y = points[i + 1].Y; // оперируем только  с элементами оординат, т.к. абсцисса "виртуальная и не едет"
                if (points[i].Y > graph_maxy)
                    graph_maxy = points[i].Y;
                if (points[i].Y < graph_miny)
                    graph_miny = points[i].Y;

                points2[i].Y = points2[i + 1].Y; // оперируем только  с элементами оординат, т.к. абсцисса "виртуальная и не едет"
                if (points2[i].Y > graph_maxy2)
                    graph_maxy2 = points2[i].Y;
                if (points2[i].Y < graph_miny2)
                    graph_miny2 = points2[i].Y;

                points3[i].Y = points3[i + 1].Y; // оперируем только  с элементами оординат, т.к. абсцисса "виртуальная и не едет"
                if (points3[i].Y > graph_maxy3)
                    graph_maxy3 = points3[i].Y;
                if (points3[i].Y < graph_miny3)
                    graph_miny3 = points3[i].Y;


                lpf[i].Y = lpf[i + 1].Y;
                lpf2[i].Y = lpf2[i + 1].Y;
                lpf3[i].Y = lpf3[i + 1].Y;


            }

            points[716].Y = (float)t_inp;
            points2[716].Y = (float)t_inp2;
            points3[716].Y = (float)t_inp3;

            
            

            if (checkBox15.Checked)
            {
                float temp = 0;
                lpf[716].Y = (float)t_inp;
                for (i = 716 - lpf_win; i <  717; i++)
                {
                    temp = temp + lpf[i].Y;
                }
                lpf[715].Y = temp / (lpf_win+1);
                g.DrawLines(pen_lf, lpf);

            }

            if (checkBox16.Checked)
            {
                float temp = 0;
                lpf2[716].Y = (float)t_inp2;
                for (i = 716 - lpf_win; i < 717; i++)
                {
                    temp = temp + lpf2[i].Y;
                }
                lpf2[715].Y = temp / (lpf_win+1);
                g.DrawLines(pen_lf2, lpf2);

            }

            if (checkBox17.Checked)
            {
                float temp = 0;
                lpf3[716].Y = (float)t_inp3;
                for (i = 716 - lpf_win; i < 717; i++)
                {
                    temp = temp + lpf3[i].Y;
                }
                lpf3[715].Y = temp / (lpf_win+1);
                g.DrawLines(pen_lf3, lpf3);

            }





            if (checkBox12.Checked)
                g.DrawLines(pen, points);
            if (checkBox13.Checked)
                g.DrawLines(pen2, points2);
            if (checkBox14.Checked)
                g.DrawLines(pen3, points3);
            //g.DrawCurve(pen, points);

            if (checkBox10.Checked)
            {
                counter_b_x++;
                if (counter_b_x > n_mean)
                {
                    
                    i = 0;
                    while (i < 178)
                    {
                        bezie_x[i].X = points[i * 4].X;
                        bezie_x[i].Y = points[i * 4].Y;
                        i++;
                    }
                    counter_b_x = 0;
                }
                g.DrawBeziers(pen_lvl, bezie_x);
            }

            if (checkBox7.Checked)
            {
                counter_b_y++;
                if (counter_b_y > n_mean)
                {

                    i = 0;
                    while (i < 178)
                    {
                        bezie_x2[i].X = points2[i * 4].X;
                        bezie_x2[i].Y = points2[i * 4].Y;
                        i++;
                    }
                    counter_b_y = 0;
                }
                g.DrawBeziers(pen_lvl2, bezie_x2);
            }

            if (checkBox2.Checked)
            {
                counter_b_z++;
                if (counter_b_z > n_mean)
                {

                    i = 0;
                    while (i < 178)
                    {
                        bezie_x3[i].X = points3[i * 4].X;
                        bezie_x3[i].Y = points3[i * 4].Y;
                        i++;
                    }
                    counter_b_z = 0;
                }
                g.DrawBeziers(pen_lvl3, bezie_x3);
            }



            if (checkBox9.Checked)//min
                g.DrawLine(pen_lvl, 1, graph_maxy, 716, graph_maxy);
            if (checkBox6.Checked)//min
                g.DrawLine(pen_lvl, 1, graph_maxy2, 716, graph_maxy2);
            if (checkBox3.Checked)//min
                g.DrawLine(pen_lvl, 1, graph_maxy3, 716, graph_maxy3);

            if (checkBox11.Checked)//max
                g.DrawLine(pen_lvl, 1, graph_miny, 716, graph_miny);
            if (checkBox8.Checked)//max
                g.DrawLine(pen_lvl, 1, graph_miny2, 716, graph_miny2);
            if (checkBox1.Checked)//max
                g.DrawLine(pen_lvl, 1, graph_miny3, 716, graph_miny3);
            pictureBox1.Image = b;//выбрать между функциями
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                string ID_mk = textBox2.Text;
                PortEr.Ini();
                PortEr.Find_port(ID_mk);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            if (PortEr.MkPortFound == false)
            {
                label6.Text = "Не найдено";
                return;
            }
            else
            {

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!paused)
            {
                paused = !paused;
                aTimer.Enabled = false;
                button2.Text = "Продолжить";
            }
            else
            {
                aTimer.Enabled = true;
                button2.Text = "Пауза";
                paused = !paused;
            }
        }

        

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            aTimer.Stop();
            My_txt_Writer.Close_file();
            PortEr.Close_port();
            
            //Application.Exit();
            this.Close();
        }

        
    }
}
