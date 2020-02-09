using CShapDM;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Picachu
{
    public partial class Form1 : Form
    {
        CDmSoft dm = new CDmSoft();

       
        

        public Form1()
        {
            InitializeComponent();

        }

        public void button1_Click(object sender, EventArgs e)
        {
          
            int handle = gethandle();
            dm.ForceUnBindWindow(handle);
         
            dm.BindWindowEx(handle, "gdi", "windows", "windows", "", 0);
            // MessageBox.Show(handle.ToString());
            // dm.SetMouseDelay("windows", 180);
            // Đọc hình ảnh thành tọa độ , có 9 dòng và 16 cột , Ta cho thêm rìa ngoài 1  cột để tìm đường đi thành 11 dòng và 18 cột
            // ví dụ rắn ở tọa đô 1 1 , vậy là tọa dộ [1,1]  = 1 ( ta gán ở func dưới ) quét 1 lượt 
            int[,] toado = new int[18, 11];
            do
            {
                for (int x = 0; x <= 17; x++)
                {
                    for (int y = 0; y <= 10; y++)
                    {

                      // toa do ngoài thì giá trị = 0 khỏi quét
                        if (x == 0 || y == 0 || x == 17 || y == 10)
                        {
                            toado[x, y] = 0;

                        }
                        else
                        {
                            //quét tọa độ của pikachu
                            toado[x, y] = findpikachu(x, y);
                            
                        }
                      
                    }

                }
                int[] arrayx = new int[18];
                int[] arrayy = new int[11];
               
                int count = 0;


                for (int z = 1; z <= 36; z++)
                {
                    for (int i = 1; i <= 16; i++)
                    {
                        for (int j = 1; j <= 9; j++)
                        {

                            if (toado[i, j] == z)
                            {
                               //gán tọa độ vào arrayx và y
                                arrayx[count] = i;
                                arrayy[count] = j;
                                
                                count++;
                                if (count >= 4) count = 0;
                            }

                        }

                    }

                    //check có bị dính click ko
  

                   // sau khi gán ta có 2 hoặc 4 array , ta cho vào vòng lập check link và click lần lượt với nhau
                        bool isbool = true;
                        while (isbool)
                        {
                       
                            isbool = false;
                            for (int i = 0; i <= 2; i++)
                            {
                               
                                for (int j = i + 1; j <= 3; j++)
                                {
                                
                                    if (CheckLink(toado, arrayx[i], arrayy[i], arrayx[j], arrayy[j]) == true)
                                    {
                                    //tìm toàn màn hình có bị dính màu đo đỏ khi lần trước click còn lưu lại, có thì click vào con đó
                                        dm.FindMultiColor(0, 0, 960, 540, "d24d57", "6|5|d24d57", 1, 0, out object x, out object y);
                                       int  intX = Convert.ToInt32(x);
                                        int intY = Convert.ToInt32(y);
                                        if (intX != -1)
                                        {
                                            dm.MoveTo(intX, intY);
                                            dm.LeftClick();
                                           
                                        }
                                        Thread.Sleep(500);
                                    //ghi tọa độ ra để dễ nhìn
                                        Console.WriteLine((arrayx[i] + " " + arrayy[i] + " ," + arrayx[j] + " " + arrayy[j]));
                                    // move chuột lại và click vào tọa độ pikachu
                                        dm.MoveTo(43 + 55 * (arrayx[i] - 1), 68 + 55 * (arrayy[i] - 1));
                                   
                                        dm.LeftClick();
                                        Thread.Sleep(300);
                                    //do mình hay bị không click được con đầu nên mình làm thêm cái check có click được không, không được thì click tiếp .
                                    dm.FindMultiColor(0, 0, 960, 540, "d24d57", "6|5|d24d57", 1, 0, out  x, out  y);
                                    intX = Convert.ToInt32(x);
                                    intY = Convert.ToInt32(y);
                                    if (intX == -1)
                                    {
                                        dm.MoveTo(43 + 55 * (arrayx[i] - 1), 68 + 55 * (arrayy[i] - 1));
                                       
                                        dm.LeftClick();

                                    }

                                    Thread.Sleep(500);
                                    //move và click con thứ 2
                                        dm.MoveTo(43 + 55 * (arrayx[j] - 1), 68 + 55 * (arrayy[j] - 1));
                                        dm.delay(50);
                                        dm.LeftClick();

                                        Thread.Sleep(1000);
                                    // xóa phần từ để không check lại .
                                        Array.Clear(arrayx, i, j);
                                        Array.Clear(arrayy, i, j);
                                        isbool = true;
                                        break ;

                                    }

                                }
                            }
                        }
                   

                      
                        Thread.Sleep(10);
                  Array.Clear(arrayx, 0, arrayx.Length);
                  Array.Clear(arrayy, 0, arrayy.Length);
                }
           
            } while (true) ;
        }

        public void button2_Click(object sender, EventArgs e)
        {

            //button này để mình test, các bạn bỏ qua
           
        }

        public int findpikachu(int x, int y)
        {
       
            //ran
            if (dm.CmpColor(38 + 55 * (x-1), 68 + 55 * (y-1), "bac3c1", 1) == 0) return 1;
            //rua
            if (dm.CmpColor(43 + 55 * (x-1), 68 + 55 * (y-1), "3d3c6c", 1) == 0) return 2;
            //Nhim
            if (dm.CmpColor(43 + 55 * (x-1), 68 + 55 * (y-1), "3f1d1a", 1) == 0) return 3;
            //Khi
            if (dm.CmpColor(43 + 55 * (x-1), 68 + 55 * (y-1), "ffd6d0", 1) == 0) return 4;
            //TeGiac
            if (dm.CmpColor(43 + 55 * (x-1), 68 + 55 * (y-1), "bafc9b", 1) == 0) return 5;
            //CaMap
            if (dm.CmpColor(43 + 55 * (x-1), 68 + 55 * (y-1), "b6ffff", 1) == 0) return 6;
            //Ngua
            if (dm.CmpColor(43 + 55 * (x-1), 68 + 55 * (y-1), "352329", 1) == 0) return 7;
            //DaDieu
            if (dm.CmpColor(43 + 55 * (x-1), 68 + 55 * (y-1), "9e5031", 1) == 0) return 8;
            //Cop
            if (dm.CmpColor(43 + 55 * (x-1), 68 + 55 * (y-1), "fdffff", 1) == 0) return 9;
            //Nai
            if (dm.CmpColor(43 + 55 * (x-1), 68 + 55 * (y-1), "867b5f", 1) == 0) return 10;
            //Tho
            if (dm.CmpColor(43 + 55 * (x-1), 68 + 55 * (y-1), "9d9292", 1) == 0) return 11;
            //OcSen
            if (dm.CmpColor(43 + 55 * (x-1), 68 + 55 * (y-1), "ec5860", 1) == 0) return 12;
            //CaKiem
            if (dm.CmpColor(40 + 55 * (x-1), 68 + 55 * (y-1), "c4521d", 1) == 0) return 13;
            //Heo
            if (dm.CmpColor(43 + 55 * (x-1), 68 + 55 * (y-1), "ffd4d0", 1) == 0) return 14;
            //Gacon
            if (dm.CmpColor(43 + 55 * (x-1), 68 + 55 * (y-1), "ffffe1", 1) == 0) return 15;
            //Bo
            if (dm.CmpColor(43 + 55 * (x-1), 68 + 55 * (y-1), "432b1e", 1) == 0) return 16;
            //Cao
            if (dm.CmpColor(43 + 55 * (x-1), 68 + 55 * (y-1), "56367b", 1) == 0) return 17;
            //HaiCau
            if (dm.CmpColor(43 + 55 * (x-1), 68 + 55 * (y-1), "999694", 1) == 0) return 18;
            //Sâu
            if (dm.CmpColor(43 + 55 * (x-1), 68 + 55 * (y-1), "6a6c51", 1) == 0) return 19;
            //ConCo
            if (dm.CmpColor(43 + 55 * (x-1), 68 + 55 * (y-1), "767883", 1) == 0) return 20;
            //Gau
            if (dm.CmpColor(43 + 55 * (x-1), 68 + 55 * (y-1), "fefeff", 1) == 0) return 21;
            //Huou
            if (dm.CmpColor(43 + 55 * (x-1), 68 + 55 * (y-1), "8a9194", 1) == 0) return 22;
            //HaiMa
            if (dm.CmpColor(43 + 55 * (x-1), 68 + 55 * (y-1), "49668d", 1) == 0) return 23;
            //Cua
            if (dm.CmpColor(43 + 55 * (x-1), 68 + 55 * (y-1), "e51004", 1) == 0) return 24;
            //Voi
            if (dm.CmpColor(43 + 55 * (x-1), 68 + 55 * (y-1), "6f79bd", 1) == 0) return 25;
            //Kien
            if (dm.CmpColor(43 + 55 * (x-1), 68 + 55 * (y-1), "888e96", 1) == 0) return 26;
            //Meo
            if (dm.CmpColor(43 + 55 * (x-1), 68 + 55 * (y-1), "f2d7e2", 1) == 0) return 27;
            //HeoRung
            if (dm.CmpColor(43 + 55 * (x-1), 68 + 55 * (y-1), "a36645", 1) == 0) return 28;
            //GauBong
            if (dm.CmpColor(43 + 55 * (x-1), 68 + 55 * (y-1), "9d8a66", 1) == 0) return 29;
            //CanhCam
            if (dm.CmpColor(43 + 55 * (x-1), 68 + 55 * (y-1), "eeefef", 1) == 0) return 30;
            //CaVoi
            if (dm.CmpColor(43 + 55 * (x-1), 68 + 55 * (y-1), "d6effe", 1) == 0) return 31;
            //ConChim
            if (dm.CmpColor(43 + 55 * (x-1), 68 + 55 * (y-1), "2a7850", 1) == 0) return 32;
            //BachTuot
            if (dm.CmpColor(43 + 55 * (x-1), 68 + 55 * (y-1), "f406ff", 1) == 0) return 33;
            //ConCuu
            if (dm.CmpColor(43 + 55 * (x-1), 68 + 55 * (y-1), "fff1ec", 1) == 0) return 34;
            //Ca
            if (dm.CmpColor(43 + 55 * (x-1), 68 + 55 * (y-1), "4b9ca6", 1) == 0) return 35;
            //Ech
            if (dm.CmpColor(43 + 55 * (x-1), 68 + 55 * (y-1), "5fff6a", 1) == 0) return 36;
            return 0;
        }

       
        //Check xem có kết nối được không
       public bool CheckLink(int [,] toado,int x1,int y1 , int x2, int y2)
        {
          if (x1==0 || y1==0 || x2 ==0 || y2==0)
            {
                return false;
            }
            if (x1 ==x2)
            {
                if (y1 - y2 == 1 || y2 - y1 == 1)
                {
                    return true;
                }
            }
            if (y1 == y2)
            {
                if (x1 -x2 == 1 ||x2 - x1 == 1)
                {
                    return true;
                }
            }
            for (int i = 0; i < 18; i++)
            {
                bool key = true;
                for (int j = (y1 > y2 ? y2 : y1) + 1; j < (y1 < y2 ? y2 : y1); j++)
                {
                    if (toado[i, j] != 0)
                    {
                        key = false;
                        break;
                    }
                }
                if (!key)
                {
                    continue;
                }
                else
                {
                    bool key1 = true, key2 = true;
                    for (int k = (x1 < i ? x1 : i); k <= (x1 > i ? x1 : i); k++)
                    {
                        if (toado[k,y1] != 0)
                        {
                            if (k != x1)
                            {
                                key1 = false;
                                break;
                            }
                        }
                    }
                    if (!key1)
                    {
                        //break;
                        continue;
                    }
                    for (int k = (x2 < i ?x2 : i); k <= (x2 > i ?x2 : i); k++)
                    {
                        if (toado[k,y2] != 0)
                        {
                            if (k !=x2)
                            {
                                key2 = false;
                                break;
                            }
                        }
                    }
                    if (key2)
                    {
                        return true;
                    }
                }
            }
            for (int j = 0; j < 11; j++)
            {
                bool key = true;
                for (int i = (x1 >x2 ?x2 : x1) + 1; i < (x1 <x2 ?x2 : x1); i++)
                {
                    if (toado[i,j] != 0)
                    {
                        key = false;
                        break;
                    }
                }
                if (!key)
                {
                    continue;
                }
                else
                {
                    bool key1 = true, key2 = true;
                    for (int k = (j > y1 ? y1 : j); k <= (j < y1 ? y1 : j); k++)
                    {
                        if (toado[x1,k] != 0)
                        {
                            if (k != y1)
                            {
                                key1 = false;
                                break;
                            }
                        }
                    }
                    if (!key1)
                    {
                        //break;
                        continue;
                    }
                    for (int k = (j > y2 ? y2 : j); k <= (j < y2 ? y2 : j); k++)
                    {
                        if (toado[x2,k] != 0)
                        {
                            if (k != y2)
                            {
                                key2 = false;
                                break;
                            }
                        }
                    }
                    if (key2)
                    {
                        return true;
                    }
                }
            }
            return false;

        }
        public int gethandle()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\ChangZhi\LDPlayer");

            if (key == null)
            {
                MessageBox.Show("Không tìm thấy đường dẫn ldplayer 4 , vui lòng cài đặt nếu bạn chưa cài");
            }
            object objRegisteredValue = key.GetValue("InstallDir");
            string SimulatorPath = objRegisteredValue.ToString();

            /// <summary>
            /// View the status of the created simulator PS: In turn: index, title, top-level window handle, bound window handle, whether to enter android, process PID, VBox process PID
            /// </summary>
            /// <returns>Returns a List </returns>

            List<string> list = new List<string>();
            string[] str = Regex.Split(ImplementCmd(
                string.Format("{0}dnconsole list2",
                SimulatorPath)), "\r\n", RegexOptions.IgnoreCase);
            for (int i = 4; i < (str.Length - 3); i++)
            {
                list.Add(str[i]);
            }


            var line = list[0];

            String[] strlist = line.Split(',');
            var hand = strlist[3];


            return int.Parse(hand.ToString());

        }
        public string ImplementCmd(string value)
        {
            Process p = new Process();
            
            p.StartInfo.FileName = "cmd.exe";
          
            p.StartInfo.UseShellExecute = false;
            
            p.StartInfo.RedirectStandardInput = true;
           
            p.StartInfo.RedirectStandardOutput = true;
            
            p.StartInfo.RedirectStandardError = true;

            p.StartInfo.CreateNoWindow = true;


          
            p.Start();
           
            p.StandardInput.WriteLine(value);
            p.StandardInput.WriteLine("exit");
            p.StandardInput.AutoFlush = true;
            
            string strOuput = p.StandardOutput.ReadToEnd();
            
            if (p.WaitForExit(10000))
                p.Close();
            else
                p.Kill();

            return strOuput;
        }
    }

    }