//using FMUtils.KeyboardHook;
using FMUtils.KeyboardHook;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WindowsInput;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            this.Text = RandomString(8);
            InitializeComponent();

            if(!License.Status.Licensed)
            {
                MessageBox.Show(this,"A hardware ID file called hwid.txt has been created in the appliaction directory.\r\n Please send it to license@clickz0r.online together with your registration mail address to get a valid license file. ");
                File.WriteAllText("hwid.txt",License.Status.HardwareID);    
                Process.GetCurrentProcess().Kill(); 
            }
            
                if(Process.GetProcessesByName("WindowsFormsApp1").Length == 0) 
                    //Process.GetProcessesByName("WindowsFormsApp1")[0].Kill();
                File.Delete(".\\WindowsFormsApp1.exe");
            
            if (File.Exists(".\\WindowsFormsApp1.exe"))
            {
                byte[] file = File.ReadAllBytes(".\\WindowsFormsApp1.exe");
                int pumpedAdd = new Random().Next(0, 1000000);
                byte[] newFile=new byte[file.Length + pumpedAdd];
                for (int i = 0; i < file.Length;i++) 
                {
                    newFile[i] = file[i];
                }

                for(int i = file.Length-1; i < file.Length+pumpedAdd;i++)
                {
                    newFile[i] = (byte)new Random().Next(0, 255);
                }
                String rnd = RandomString(8);
                File.WriteAllBytes(".\\" + rnd + ".exe",newFile);
                
                Process.Start(".\\" + rnd + ".exe");
                Process.GetCurrentProcess().Kill(); 
            }
            
            
        }
        
        #region Declarations
        Hook KeyboardHook = null;
        private bool isRunning;

        public Dictionary<String, itemData> items = new Dictionary<string, itemData>();
        Point pixelPoint = new Point();
        Color pixelColor = new Color();
        public static Dictionary<String, String> values = new Dictionary<string, string>();
        public static Dictionary<String, Dictionary<String, String>> macros = new Dictionary<String, Dictionary<String, String>>();
        bool doHook = false;

        private static Random random = new Random();


        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        const int GWL_EXSTYLE = -20;
        const int WS_EX_TOOLWINDOW = 0x00000080;

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
        //Mouse actions
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;



        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator Point(POINT point)
            {
                return new Point(point.X, point.Y);
            }
        }
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        public class itemData
        {
            public String text = "";
            public WindowsInput.Native.VirtualKeyCode key = WindowsInput.Native.VirtualKeyCode.VK_0;
            public override string ToString()
            {
                return text;
            }
        }
        #endregion

        #region Events

        private void Form1_Load(object sender, EventArgs e)
        {

            items.Add("0", new itemData() { text = "0", key = WindowsInput.Native.VirtualKeyCode.VK_0 });
            comboBox1.Items.Add(items["0"]);
            items.Add("1", new itemData() { text = "1", key = WindowsInput.Native.VirtualKeyCode.VK_1 });
            comboBox1.Items.Add(items["1"]);
            items.Add("2", new itemData() { text = "2", key = WindowsInput.Native.VirtualKeyCode.VK_2 });
            comboBox1.Items.Add(items["2"]);
            items.Add("3", new itemData() { text = "3", key = WindowsInput.Native.VirtualKeyCode.VK_3 });
            comboBox1.Items.Add(items["3"]);
            items.Add("4", new itemData() { text = "4", key = WindowsInput.Native.VirtualKeyCode.VK_4 });
            comboBox1.Items.Add(items["4"]);
            items.Add("5", new itemData() { text = "5", key = WindowsInput.Native.VirtualKeyCode.VK_5 });
            comboBox1.Items.Add(items["5"]);
            items.Add("6", new itemData() { text = "6", key = WindowsInput.Native.VirtualKeyCode.VK_6 });
            comboBox1.Items.Add(items["6"]);
            items.Add("7", new itemData() { text = "7", key = WindowsInput.Native.VirtualKeyCode.VK_7 });
            comboBox1.Items.Add(items["7"]);
            items.Add("8", new itemData() { text = "8", key = WindowsInput.Native.VirtualKeyCode.VK_8 });
            comboBox1.Items.Add(items["8"]);
            items.Add("9", new itemData() { text = "9", key = WindowsInput.Native.VirtualKeyCode.VK_9 });
            comboBox1.Items.Add(items["9"]);

            KeyboardHook = new Hook(RandomString(3));
            KeyboardHook.KeyUpEvent += KeyDown;

            Width += new Random().Next(0, 50);
            Height += new Random().Next(0, 50);

            SetWindowLong(this.Handle, GWL_EXSTYLE, WS_EX_TOOLWINDOW);
        }


        private void bnAdd_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && !macros.ContainsKey(textBox1.Text))
            {
                int idx = listBox1.Items.Add(textBox1.Text);
                Dictionary<String, String> values = new Dictionary<string, string>();
                values.Add("1", comboBox1.SelectedItem.ToString());

                values.Add("2", pixelColor.R.ToString());
                values.Add("3", pixelColor.G.ToString());
                values.Add("4", pixelColor.B.ToString());
                values.Add("5", pixelPoint.X.ToString());
                values.Add("6", pixelPoint.Y.ToString());

                macros.Add(textBox1.Text, values);

                button3.Enabled = false;
                textBox1.Text = "";
                comboBox1.SelectedIndex = -1;
            }
            else
            {
                MessageBox.Show(this, "Please fill all fields.", "Error");
            }
        }


        private void bnStart_Click(object sender, EventArgs e)
        {
            if (button4.Text == "Start")
            {
                isRunning = true;
                backgroundWorker1.RunWorkerAsync();
                button4.Text = "Stop";
            }
            else
            {
                button4.Text = "Start";
                isRunning = false;
            }
        }


        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (isRunning)
            {
                for (int i = 0; i < listBox1.Items.Count; i++)
                {
                    Dictionary<String, String> d = macros[listBox1.Items[i].ToString()];
                    Point p = new Point(int.Parse(d["5"]), int.Parse(d["6"]));
                    int R = int.Parse(d["2"]);
                    int G = int.Parse(d["3"]);
                    int B = int.Parse(d["4"]);

                    String macro = d["1"];
                    if (checkPixel(p, R, G, B))
                    {
                        System.Threading.Thread.Sleep(RandomNumber(300, 700));
                        sendString(macro);
                        break;
                    }
                }
                System.Threading.Thread.Sleep(100);
            }
        }

        private void bnDelete_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                macros.Remove(listBox1.SelectedItem.ToString());
                listBox1.Items.RemoveAt(listBox1.SelectedIndex);
            }
        }
        

        private void bnPixel_Click(object sender, EventArgs e)
        {
            doHook = true;
            button2.Enabled = false;
        }
        private void KeyDown(KeyboardHookEventArgs e)
        {
            if (e.Key == Keys.F11 && doHook)
            {
                POINT lpPoint;
                GetCursorPos(out lpPoint);

                pixelPoint = new Point(lpPoint.X, lpPoint.Y);
                System.Threading.Thread.Sleep(3000);
                do
                {
                    pixelColor = GetColorAt(lpPoint.X, lpPoint.Y);
                } while ((int)pixelColor.R == 255 && (int)pixelColor.B == 255 && (int)pixelColor.G == 255);
                button2.Enabled = true;
                doHook = false;
                button3.Enabled = true;
            }

            if (e.Key == Keys.F)
            {
                if (button4.Text == "Start")
                {
                    isRunning = true;
                    backgroundWorker1.RunWorkerAsync();
                    button4.Text = "Stop";
                }
                else
                {
                    button4.Text = "Start";
                    isRunning = false;
                }
            }
        }
        #endregion

        #region Helper Functions
        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        private void sendString(String text)
        {
            try
            {
                WindowsInput.Native.VirtualKeyCode c = items[text].key;
                InputSimulator i = new InputSimulator();
                i.Keyboard.KeyPress(c);
                //SendKeys.Send(text);
            }
            catch (Exception)
            {
            }
        }

        private int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        public static Color GetColorAt(int x, int y)
        {
            Bitmap b = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(b);
            g.CopyFromScreen(0, 0, 0, 0, b.Size);
            Color c = b.GetPixel(x, y);
            return c;
        }
        private bool checkPixel(Point p, int R, int G, int B)
        {
            if (GetColorAt(p.X, p.Y).R == R && GetColorAt(p.X, p.Y).G == G && GetColorAt(p.X, p.Y).B == B)
                return true;
            else
                return false;
        }

        #endregion

        #region Menu Icons
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            macros = new Dictionary<string, Dictionary<string, string>>();
            listBox1.Items.Clear();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                macros = SaveLoad.ReadFromBinaryFile<Dictionary<String, Dictionary<String, String>>>(openFileDialog1.FileName);
                foreach (String d in macros.Keys)
                {
                    listBox1.Items.Add(d);
                }
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                SaveLoad.WriteToBinaryFile<Dictionary<String, Dictionary<String, String>>>(saveFileDialog1.FileName, macros);
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "Usage:\r\n\r\nPress F11 to set the pixel of a button after pressing 'Set'.\r\nTo Start/Stop, press the 'F' key.\r\n\r\nAttention! Change the filename of the executable to prevent detection.", "Help");
        }
        #endregion

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }


        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
;