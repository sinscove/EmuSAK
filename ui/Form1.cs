using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ryu_Yuzu_Downloader
{
    public partial class Form1 : Form
    {
        string version = "1.1";
        HttpClient client;
        public Form1()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.Text = this.Text + " v" + version;
        }
        public void GetVersion()
        {
             client = new HttpClient();
            client.BaseAddress = new Uri("Your backend ip here");
            #if DEBUG
                client.BaseAddress = new Uri("http://127.0.0.1:5000/api/");
#endif  
            string result = GetStringData("version").Result;
            string firmwareVersion = GetStringData("firmware/version").Result;
            button2.Text = "Download Firmware Version: " + firmwareVersion;
            if (result == "")
            {
                MessageBox.Show("The servers are currently unreachable");
                this.Close();
            }
            else if (result != version)
            {
                MessageBox.Show("There is a newer Version available");
            }
            //http://127.0.0.1:5000/api/version
        }
        public async Task<string> GetStringData(string location)
        {
            try { 
            HttpResponseMessage message = client.GetAsync(location).Result;
            string v = await message.Content.ReadAsStringAsync();
            return v;
            }
            catch
            {
                return "";
            }
        }
        
        private async void button1_Click(object sender, EventArgs e)
        {
            string v =  GetStringData("keys").Result;
            string appDataFolder=Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            bool yuzu = false;
            bool    ryu = false;
            if (Directory.Exists(appDataFolder + @"\yuzu"))
            {
                Directory.CreateDirectory(appDataFolder + @"\yuzu\keys");
                StreamWriter file = File.CreateText(appDataFolder +@"\yuzu\keys\prod.keys");
                file.Write(v);
                file.Close();
                yuzu = true;
            }
            if (Directory.Exists(appDataFolder + @"\Ryujinx"))
            {
                Directory.CreateDirectory(appDataFolder + @"\Ryujinx\system");
                StreamWriter file = File.CreateText(appDataFolder + @"\Ryujinx\system\prod.keys");
                file.Write(v);
                file.Close();
                ryu = true;
            }
            string text;
            if (yuzu && ryu)
            {
                text = "prod.keys succesfully installed in Yuzu & Ryujinx";
            }
            else if (yuzu)
            {
                text = "prod.keys succesfully installed in Yuzu";
            }
            else if (ryu)
            {
                text = "prod.keys succesfully installed in Ryujinx";
            }
            else
            {
                text = "Couldn't install prod.keys (most likely you haven't run any of them yet)";
            }
            MessageBox.Show(text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FirmwareDownload firmwareDownload = new FirmwareDownload("firmware","firmware.zip","Firmware download finished, the firmware is saved in this application folder","Firmware Download");
            firmwareDownload.Show();
        }

        private void button3_Click(object sender, EventArgs e)//Ryu
        {
            Saves_Shaders window = new Saves_Shaders("ryujinx");
            window.Show();
        }

        private void button4_Click(object sender, EventArgs e)//yuzu
        {
            Saves_Shaders window = new Saves_Shaders("yuzu");
            window.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            GetVersion();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", Directory.GetCurrentDirectory());
        }
    }
}
