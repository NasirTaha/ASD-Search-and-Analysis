using BusinessLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DesktopApp
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                var query = txtKeyword.Text;

                List<SearchResult> results = new List<SearchResult>();
                if (!string.IsNullOrEmpty(query))
                {
                    results = BingWebSearcher.Search(query);

                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    string contents = string.Empty;
                    //Get text from each result
                    foreach (SearchResult r in results)
                    {
                        WebRequest request = WebRequest.Create(r.Link);
                        request.Method = "GET";
                        WebResponse response = request.GetResponse();
                        Stream stream = response.GetResponseStream();
                        StreamReader reader = new StreamReader(stream);
                        contents += reader.ReadToEnd();
                        reader.Close();
                        response.Close();
                    }

                    //Wrire to text file
                    // Create a file to write to. 
                    string path = Environment.CurrentDirectory + "../../../Results.txt";
                    //Path.Combine(Environment.CurrentDirectory, "..\\..\\", fileName);
                    //string createText = "Hello and Welcome" + Environment.NewLine;
                    File.WriteAllText(path, contents);
                    MessageBox.Show("Results saved in text file");

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
