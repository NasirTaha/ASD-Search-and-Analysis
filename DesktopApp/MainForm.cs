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
using System.Windows.Forms.DataVisualization.Charting;

namespace DesktopApp
{
    public partial class MainForm : Form
    {
        string text = null;
        int positive = 0;
        int negative = 0;
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
                    this.chart1.Series.Clear();
                    int positiveresp = getPositiveParameters();
                    int negativeresp = getNegativeParameters();
                    chart1.Series.Add("Positive");
                    chart1.Series["Positive"].ChartType = SeriesChartType.Column;
                    chart1.Series["Positive"].Points.AddXY(0,positiveresp);
                    //chart1.Series["Positive"].ChartArea = "ChartArea1";
                    chart1.Series["Positive"].Color = System.Drawing.Color.Blue;
                    chart1.Series.Add("Negative");
                    chart1.Series["Negative"].ChartType = SeriesChartType.Column;
                    chart1.Series["Negative"].Points.AddXY(1,negativeresp);
                    //chart1.Series["Negative"].ChartArea = "ChartArea1";
                    chart1.Series["Negative"].Color = System.Drawing.Color.Red;


                    // this.chart1.Series["Positive"].Points.AddXY("Positive", positiveresp);
                    //this.chart1.Series["Negative"].Points.AddXY("Negative", negativeresp);

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void chart1_MouseMove(object sender, MouseEventArgs e)
        {
            Point mousePoint = new Point(e.X, e.Y);
            chart1.ChartAreas[0].CursorX.Interval = 0;
            chart1.ChartAreas[0].CursorY.Interval = 0;

            chart1.ChartAreas[0].CursorX.SetCursorPixelPosition(mousePoint, true);
            chart1.ChartAreas[0].CursorY.SetCursorPixelPosition(mousePoint, true);

            label2.Text = "Pixel X position : " + chart1.ChartAreas[0].AxisX.PixelPositionToValue(e.X).ToString();
            label3.Text = "Pixel Y position : " + chart1.ChartAreas[0].AxisY.PixelPositionToValue(e.Y).ToString();

            HitTestResult result = chart1.HitTest(e.X, e.Y);
            if (result.PointIndex > -1 && result.ChartArea != null)
            {
                label4.Text = "Y - value : " + result.Series.Points[result.PointIndex].YValues[0].ToString();


            }
        }
        private int getPositiveParameters()
        {
            positive = 0;
            text = clearTextGenrator();
            List<string> positiveList = createPositiveWordList();

            foreach (string positiveElement in positiveList)
            {
                if (text.Contains(positiveElement))
                {
                    positive++;
                }
            }
            return positive;
        }
        private int getNegativeParameters()
        {
            negative = 0;
            text = clearTextGenrator();
            List<string> negativeList = createNegativeWordList();

            foreach (string negativeElement in negativeList)
            {
                if (text.Contains(negativeElement))
                {
                    negative++;
                }
            }
            return negative;
        }

        private List<string> createPositiveWordList()
        {
            string path = Environment.CurrentDirectory + "../../../positive.txt";
            string positiveText = System.IO.File.ReadAllText(path);

            string[] positiveArray = positiveText.Split(',');//<string1/string2/string3/--->     
            List<string> positiveList = new List<string>(); //make a new string list    
            positiveList.AddRange(positiveArray);
            return positiveList;
        }
        private List<string> createNegativeWordList()
        {
            string path = Environment.CurrentDirectory + "../../../negative.txt";
            string negativeText = System.IO.File.ReadAllText(path);

            string[] negativeArray = negativeText.Split(',');//<string1/string2/string3/--->     
            List<string> negativeList = new List<string>(); //make a new string list    
            negativeList.AddRange(negativeArray);
            return negativeList;

        }

        private string clearTextGenrator()
        {
            string path = Environment.CurrentDirectory + "../../../Results.txt";
            text = System.IO.File.ReadAllText(path);
            text = text.ToLower();
            List<string> clearTextList = getListToClearText();
            foreach (string element in clearTextList)
            {
                if (element == "&")
                {
                    text = text.Replace(element, "and");
                }
                if (element == ",")
                {
                    text = text.Replace(element, " ");
                }
                else if (element == "  ")
                {
                    text = text.Replace(element, " ");
                }
                else if (element == " an ")
                {
                    text = text.Replace(element, " ");
                }
                else if (element == " from ")
                {
                    text = text.Replace(element, " ");
                }
                else if (element == " the ")
                {
                    text = text.Replace(element, " ");
                }
                else if (element == " for ")
                {
                    text = text.Replace(element, " ");
                }
                else
                {
                    text = text.Replace(element, "");
                }

            }

            text = text.Replace("\"", "");
            return text;
        }
        private List<string> getListToClearText()
        {
            List<string> list = new List<string>();

            list.Add("!");
            list.Add("`");
            list.Add("~");
            list.Add("@");
            list.Add("#");
            list.Add("$");
            list.Add("%");
            list.Add("^");
            list.Add("&");//extra
            list.Add("*");
            list.Add("(");
            list.Add(")");
            list.Add("-");
            list.Add("_");
            list.Add("+");
            list.Add("=");
            list.Add("|");
            list.Add(@"\");
            list.Add("}");
            list.Add("]");
            list.Add("{");
            list.Add("[");
            list.Add("'");
            list.Add(";");
            list.Add(":");
            list.Add("/");
            list.Add("?");
            list.Add(">");
            list.Add("<");
            list.Add(".");
            list.Add(",");
            list.Add("  "); //extra
            list.Add("‎€");
            list.Add("‎₹");
            list.Add(" an ");
            list.Add(" the ");
            list.Add(" for ");
            list.Add(" from ");

            return list;

        }

        
    }
    }
