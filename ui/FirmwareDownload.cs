using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Threading;
namespace Ryu_Yuzu_Downloader
{
    public partial class FirmwareDownload : Form
    {
        FileDownloader fileDownloader = new FileDownloader();
        string m_url, m_messageBoxText,m_saveLocation,m_windowText;
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        public FirmwareDownload(string url, string saveLocation, string messageBoxText, string windowText)
        {
            m_windowText = windowText;
            m_saveLocation = saveLocation;
            m_url = url;
            m_messageBoxText = messageBoxText;
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.FormClosing += FirmwareDownloader_Closing;
        }
        private void FirmwareDownloader_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void downloadFirmware()
        {
            string baseurl= "Your Server IP Here";
            try {
#if DEBUG
                baseurl="http://127.0.0.1:5000/api/";
#endif  
                //betterFileDownloader.DownloadFile(baseurl + m_url, m_saveLocation);
                fileDownloader.DownloadFileAsync(baseurl+m_url, m_saveLocation, cancellationTokenSource.Token);
            }
            catch(Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void FirmwareDownload_Load(object sender, EventArgs e)
        {
            Dow.Text = m_windowText;
            this.Text = m_windowText;
            fileDownloader.DownloadProgressChanged += (sender, e) => progressChanged(e.ProgressPercentage, e.BytesReceived, e.TotalBytesToReceive);
            // This callback is triggered for both DownloadFile and DownloadFileAsync
            fileDownloader.DownloadFileCompleted += (sender, e) => downloadFinished();
            downloadFirmware();
        }

        private void progressChanged(int value,long bytesReceived, long bytesTotal)
        {
            try { 
            progressBar1.Invoke((MethodInvoker)delegate
            {

                progressBar1.Value = value;
                

            });
            label1.Invoke((MethodInvoker)delegate
            {
                label1.Text = (bytesReceived / 1024).ToString() + "kb / " + (bytesTotal / 1024) + "kb";
            });
            }
            catch
            {
                //Do Nothing
            }
        }
        private void downloadFinished()
        {
            MessageBox.Show(m_messageBoxText);
            try
            {
            this.Invoke((MethodInvoker)delegate
            {
                this.Close();
          
            });

            }
            catch
            {
                //Do Nothing
            }
        }
    }
}
