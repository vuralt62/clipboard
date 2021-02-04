using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Runtime.InteropServices;


namespace clipboard
{
    public partial class Form1 : Form
    {
        List<KopyaBilgi> clipboard = null;
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetClipboardViewer(IntPtr hWnd);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        IntPtr SonrakiClipboardOgesi;
        public Form1()
        {
            InitializeComponent();
            clipboard = new List<KopyaBilgi>();
        }
        //NotifyIcon trayIcon = new NotifyIcon();
        private void Form1_Load(object sender, EventArgs e)
        {
            SonrakiClipboardOgesi = SetClipboardViewer(this.Handle);
            // nesneyi oluştur
            

            // sağ alt köşede görünecek olan simgeyi belirle
            trayIcon.Icon = new Icon("Graphicloads-100-Flat-Email-2.ico");

            // mouse ile simgenin üzerine gelindiğince belirecek metin
            trayIcon.Text = "Yeni mesaj bildirici";

            // simgeye tıklandığında olup bitecekler
            trayIcon.MouseClick += delegate {
            };

            // simgeyi görünür hale getir
            trayIcon.Visible = true;

            // yeni bir baloncuk göstermek için
            //trayIcon.ShowBalloonTip(1, "Bilgi", "Yeni bir mesaj var!", ToolTipIcon.Info);
            /*resim yapıştırma
            IDataObject dObjpic = Clipboard.GetDataObject();
            if (dObjpic.GetDataPresent(DataFormats.Bitmap))
            {
                pictureBox1.Image = (Bitmap)dObj.GetData(DataFormats.Bitmap);
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            }*/
        }

        protected void LB_Click(object label, EventArgs e)
        {
            string icerik;
            icerik = label.ToString();
            foreach (var item in clipboard)
            {
                if(icerik== item.Context)
                {
                    Clipboard.SetText(item.Context);
                }
            }           
        }
        protected override void WndProc(ref Message m)
        {
            int WM_DRAWCLIPBOARD = 0x0308;
            int WM_CHANGECBCHAIN = 0x030D;

            if (m.Msg == WM_DRAWCLIPBOARD)
            {
                IDataObject dObj = Clipboard.GetDataObject();
                IDataObject dObjpic = Clipboard.GetDataObject();
                KopyaBilgi k = new KopyaBilgi();
                if (dObj.GetDataPresent(DataFormats.Text))
                {                   
                    
                    if (Clipboard.GetText() != "")
                    {
                        Label label = new Label();
                        label.Height = 50;
                        label.Width = flowLayoutPanel1.Width - 10;
                        label.BorderStyle = BorderStyle.FixedSingle;
                        label.Click += new EventHandler(LB_Click);
                        if (flowLayoutPanel1.Controls.Count < 10)
                        {
                            k.Context= (dObj.GetData(DataFormats.Text).ToString());
                            k.Time = DateTime.Now;
                        }
                        else
                        {
                            for (int i = 0; i < 9; i++)
                            {
                                clipboard[i] = clipboard[i + 1];
                            }
                            clipboard.RemoveAt(9);
                            k.Context = dObj.GetData(DataFormats.Text).ToString();
                            k.Time = DateTime.Now;
                            flowLayoutPanel1.Controls.RemoveAt(0);
                            
                        }
                        clipboard.Add(k);
                        foreach (var item in clipboard)
                        {
                            label.Text = item.Context;
                            flowLayoutPanel1.Controls.Add(label);
                        }                       
                    }
                }
                SendMessage(SonrakiClipboardOgesi, m.Msg, m.WParam, m.LParam);
            }
            else if (m.Msg == WM_CHANGECBCHAIN)
            {
                if (m.WParam == SonrakiClipboardOgesi)
                {
                    SonrakiClipboardOgesi = m.LParam;
                }
                else
                {
                    SendMessage(SonrakiClipboardOgesi, m.Msg, m.WParam, m.LParam);
                }
            }
            base.WndProc(ref m);
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            ChangeClipboardChain(this.Handle, SonrakiClipboardOgesi);
        }

        private void FlowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form1_Resize(object sender, EventArgs e)
        {

        }

        private void GösterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
        }
    }
}