using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;


namespace Com_port
{
    public sealed class My_txt_Writer
    {
        private static volatile My_txt_Writer instance;
        private static object syncRoot = new Object();
        private static StreamWriter sw;


        private My_txt_Writer() { }

        public static void Append_to_file(String x, String y, String z)
        {
            try
            {
                
                sw.Write(x + " " + y + " " + z + "\n");
            }
            catch (Exception ex)
            {
               // MessageBox.Show(ex.ToString());
            }
        }

        public static void Close_file()
        {
            try
            {
                sw.Close();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }
        }



        public static void Ini_file_writer()
        {
            string fileName = Path.Combine(Environment.CurrentDirectory, DateTime.Now.ToString("yyyyMMddhhmmss"));//, ".txt");
            try
            {
                sw = new StreamWriter(fileName);
                sw.WriteLine("x, y, z");

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }

        }


        public static My_txt_Writer getInstance(string name)//функция, необходимая, чтобы не породилось несколько экземпляров данного класса
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                        instance = new My_txt_Writer();
                }
            }
            return instance;
        }
    }
}
