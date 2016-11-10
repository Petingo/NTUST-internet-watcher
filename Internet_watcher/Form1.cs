using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace Internet_watcher
{
    public partial class Form1 : Form
    {
        private string GetWebContent(string Url)
        {
            string strResult = "";
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Timeout = 30000;
                request.Headers.Set("Pragma", "no-cache");
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream streamReceive = response.GetResponseStream();
                //Encoding encoding = Encoding.GetEncoding("");
                StreamReader streamReader = new StreamReader(streamReceive);
                strResult = streamReader.ReadToEnd();
            }
            catch
            {
                MessageBox.Show("Something Wrong");
            }
            return strResult;
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = GetWebContent("http://network.ntust.edu.tw/");
        }
    }
}
