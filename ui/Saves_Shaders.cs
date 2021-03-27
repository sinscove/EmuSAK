using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Ryu_Yuzu_Downloader
{
    public partial class Saves_Shaders : Form
    {
        HttpClient client = new HttpClient();
        Dictionary<String, String> shaders;
        Dictionary<String, String> saves;
        string m_platform;
        public Saves_Shaders(string platform)
        {
            m_platform = platform;
            InitializeComponent();
        }

        private void Saves_Shaderscs_Load(object sender, EventArgs e)
        {
            this.Text = m_platform.ToUpperInvariant() + " Shaders/Saves";
            InitClient();
            getData();
        }
        private void getData()
        {
            string result = GetStringData("shaders/"+m_platform).Result;
            shaders = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
            foreach(KeyValuePair<string,string> entry in shaders) { 
                var item = new ListViewItem();
                item.Text = entry.Key;
                
                listView1.Items.Add(item);
            }
            result = GetStringData("saves").Result;
            saves = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
            foreach (KeyValuePair<string, string> entry in saves)
            {
                var item = new ListViewItem();
                item.Text = entry.Value;

                listView2.Items.Add(item);
            }
        }
        public void InitClient()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("Your Backend IP Adress here");
        #if DEBUG
            client.BaseAddress = new Uri("http://127.0.0.1:5000/api/");
        #endif  

        }
        public async Task<string> GetStringData(string location)
        {
            try
            {
                HttpResponseMessage message = client.GetAsync(location).Result;
                string v = await message.Content.ReadAsStringAsync();
                return v;
            }
            catch
            {
                return "";
            }
        }
        private async void listView1_DoubleClicked(object sender, EventArgs e)
        {
            string titleid = shaders[listView1.SelectedItems[0].Text];
            string titlename = listView1.SelectedItems[0].Text;
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (m_platform == "ryujinx") { 
                string location = appDataFolder + @"\Ryujinx\games\" + titleid + @"\cache\shader\guest\program\";
                string location2 = appDataFolder + @"\Ryujinx\games\" + titleid + @"\cache\shader\opengl\glsl\host\";
                Directory.CreateDirectory(location); 
                Directory.CreateDirectory(location2);
                FirmwareDownload window = new FirmwareDownload("shaders/" + m_platform + "?type=zip&id=" + titleid, location+"cache.zip","Download of Shaders of "+titlename+" finished","Shader Download of "+titlename);
                FirmwareDownload windowInfo = new FirmwareDownload("shaders/" + m_platform + "?type=info&id=" + titleid, location + "cache.info", "Download of Info of " + titlename + " finished", "Shader Download of " + titlename);
                window.Show();
                windowInfo.Show();
                while(IsFileLocked(new FileInfo(location+"cache.zip")) || IsFileLocked(new FileInfo(location + "cache.info")))
                {
                     await WaitAsync(1000);
                }
                copyFiles(location, location2);
            }
            else
            {
                if (!Directory.Exists(appDataFolder + @"\yuzu\shader\opengl\transferable"))
                {
                    Directory.CreateDirectory(appDataFolder + @"\yuzu\shader\opengl\transferable");
                }
                FirmwareDownload download = new FirmwareDownload("shaders/" + m_platform + "?id=" + titleid, appDataFolder + @"\yuzu\shader\opengl\transferable\" + titleid + ".bin", "Download of Shaders of " + titlename + " finished!", "Shader Download of " + titlename);
                download.Show();
            }
        }
        private async void listView2_DoubleClicked(object sender, EventArgs e)
        {
            string titlename = listView2.SelectedItems[0].Text;
            var titleid = "";
            foreach(var save in saves)
            {
                if (save.Value == titlename)
                {
                    titleid = save.Key;
                }
            }
            if (Directory.Exists("saves") == false)
            {
                Directory.CreateDirectory("saves");
            }
            string text;
            if (m_platform == "yuzu")
            {
                text = "Download of " + titlename + " finished, it's placed in this directory, install it in Yuzu with right-click on the game > Open Save Data Location and then place the content of the zip in the folder";
            }
            else
            {
                text = "Download of " + titlename + " finished, it's placed in this directory, install it in Ryujinx with right-click on the game > Open User Save Directory and then place the content of the zip in the folder";
            }
            FirmwareDownload window = new FirmwareDownload("saves?id=" + titleid, @"saves/"+titlename , text, "Save Download of " + titlename);
            window.Show();
            while(IsFileLocked(new FileInfo(titlename)))
            {
                await WaitAsync(500);
            }
            
        }
        async Task WaitAsync(int msTime)
        {
            await Task.Delay(msTime);
        }


        protected virtual bool IsFileLocked(FileInfo file)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }

            //file is not locked
            return false;
        }
        private async void copyFiles(string location,string location2)
        {
            
            FileStream fr = new FileStream(location2 + "cache.zip", FileMode.Create);
            await fr.WriteAsync(File.ReadAllBytes(location + "cache.zip"));
            fr.Close();
            fr = new FileStream(location2 + "cache.info", FileMode.Create);
            await fr.WriteAsync(File.ReadAllBytes(location + "cache.info"));
            fr.Close();

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
