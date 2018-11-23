

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.IO.Ports;
using System.Windows.Forms;
using System.Threading;

namespace Com_port
{

    public sealed class PortEr
    {
        private static volatile PortEr instance;
        private static object syncRoot = new Object();
        public static SerialPort _currentPort;
        public static String _ID_mk, _port_finded, strFromPort;
        public static bool MkPortFound;
        public static string[] ports;
        public static System.Timers.Timer aTimer;
        private static Thread readThread  = new Thread(Read);
        public static int check_current_port;
		public static bool isChanged;
        //public static string port;
        private PortEr() { }
        public static void Ini()
        

        {
            strFromPort = "";
            Get_ports();
        }

        public static void Close_port()
        {
            try
            {
                readThread.Join(500);
               // readThread.Interrupt();
               
                _currentPort.DiscardInBuffer();
                _currentPort.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }



        public static void Find_port(string ID_mk)
        {
            _ID_mk = ID_mk;
            MkPortFound = false;
            check_current_port = 0;


            try
            {
                foreach (string port in ports)//просматриваем все порты
                {
                    
                    check_current_port++;
                    _currentPort = new SerialPort(port, 9600);//каждый открываем
                    _currentPort.DtrEnable = true;
                    _currentPort.ReadTimeout = 2000;
                   // _currentPort.WriteTimeout = 500;
                    bool det = MkDetected();
                    if (det)// и слушаем
                    {
                        MkPortFound = true;
                        _port_finded = port;
                        System.Threading.Thread.Sleep(500);//ждем, не спешим
                        break;
                    }
                    else
                    {
                        MkPortFound = false;
                    }

                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        public static void Run_port()
        {
            _currentPort = new SerialPort(_port_finded, 9600); // new
            PortEr.MkPortFound = true;//new
            isChanged = false;
            _currentPort.DtrEnable = true;
            _currentPort.BaudRate = 9600;
            //_currentPort.DtrEnable = true;
            _currentPort.ReadTimeout = 2000;
            

            try
            {
                _currentPort.Open();
                System.Threading.Thread.Sleep(1000);
                _currentPort.DiscardInBuffer();
                readThread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            

            //aTimer = new System.Timers.Timer(200);
            //aTimer.Elapsed += OnTimedEvent;
            //aTimer.AutoReset = true;
            //aTimer.Enabled = true;
            


        }

       



        //private static void OnTimedEvent(object sender, ElapsedEventArgs e)
        public static void Read()
        {

            
            while (_currentPort.IsOpen)
            {

                try // так как после закрытия окна таймер еще может выполнится или предел ожидания может быть превышен
                {
                    // удалим накопившееся в буфере
                    //_currentPort.DiscardInBuffer();

                    // считаем последнее значение 
                    //if (_currentPort.BytesToRead > 0)
                    //{
                    if (_currentPort.IsOpen)
                    {
                        string strFromPort_temp = _currentPort.ReadLine();
                        if ((!strFromPort_temp.Contains("E")) || (!strFromPort_temp.Contains(_ID_mk)))
                        {
							isChanged = true;
                            strFromPort = strFromPort_temp;
                        }
                    }
                    else return;
                    //}

                }
                catch (Exception ex)
                {
                   // MessageBox.Show(ex.ToString());
                }
            }
        }
        

        private static void Get_ports()
        {
            ports = SerialPort.GetPortNames();
        }

        public static int Nums_ports()
        {
            //получаем все порты на данном пк
            int i = ports.Length;//для отображения статус бара

            return i;
        }

        private static bool MkDetected()
        {
            try
            {
                _currentPort.Open();
                string returnMessage;
              // Run_port();
                System.Threading.Thread.Sleep(500);
               // returnMessage = _currentPort.ReadLine();
                
                 
                // _currentPort.Write(_ID_mk);
                _currentPort.WriteLine(_ID_mk);
                System.Threading.Thread.Sleep(10);
                returnMessage = _currentPort.ReadLine();
                strFromPort = returnMessage;
                
                // необходимо чтобы void loop() в скетче содержал код c ID;
                if (returnMessage.Contains(_ID_mk))
                {
                    
                    _currentPort.Close();
                    return true;

                }
                else
                {
                    _currentPort.Close();
                    return false;
                }
            }
            catch (Exception ex)
            {
              //  MessageBox.Show(ex.ToString());
                _currentPort.Close();
                return false;

            }
        }

        public static PortEr getInstance(string name)//функция, необходимая, чтобы не породилось несколько экземпляров данного класса
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                        instance = new PortEr();
                }
            }
            return instance;
        }

        public static string Get_port()
        {

            return _port_finded;
        }

        ~PortEr()
        {
            aTimer.Enabled = false;
            _currentPort.Close();
        }

         

    //public static bool Check_connection()
    //    {
    //        _currentPort.DiscardInBuffer();
    //        _currentPort.Write("S");
    //        System.Threading.Thread.Sleep(100);
    //        string returnMessage = _currentPort.ReadLine();

    //        if (returnMessage.Contains("E"))
    //        {
    //           // _currentPort.Write("R");
    //           // _currentPort.Close();
    //            return true;

    //        }
    //        else
    //        {
    //            _currentPort.Close();
    //            return false;
    //        }
    //    }




}

 }
