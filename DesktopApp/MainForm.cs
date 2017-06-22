using BusinessLogic;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
            
            //            circularProgressBar1.Update();
            //         backgroundWorker1.RunWorkerAsync();
            try
            {
                //Search our old results first
                var keyword = txtKeyword.Text.ToLower();
                BusinessLogic.Search exist = BusinessLogic.DatabaseService.IsKeywordExist(keyword);
                if (exist != null)
                {
                    DisplyDataInChart(exist.Positive, exist.Negative, 0);

                }
                else
                {



                    List<SearchResult> results = new List<SearchResult>();
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        results = BingWebSearcher.Search(keyword);
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                        StringBuilder contents = new StringBuilder();
                        //Get text from each result
                        foreach (SearchResult r in results)
                        {
                            try
                            {
                                HtmlAgilityPack.HtmlDocument docHtml = new HtmlWeb().Load(r.Link);
                                var text = ExtractViewableTextCleaned(docHtml.DocumentNode);
                                // MessageBox.Show(text);
                                contents.Append(text);// reader.ReadToEnd();
                                                      //WebRequest request = WebRequest.Create(r.Link);
                                                      //request.Method = "GET";
                                                      //WebResponse response = request.GetResponse();
                                                      //Stream stream = response.GetResponseStream();
                                                      //StreamReader reader = new StreamReader(stream);
                                                      //contents += reader.ReadToEnd();
                                                      //reader.Close();
                                                      //response.Close();
                            }
                            catch (Exception ex)
                            {
                                //To do: write exception to log message
                            }
                        }

                        //Wrire to text file
                        // Create a file to write to. 
                        string path = Environment.CurrentDirectory + "../../../Results.txt";
                        //Path.Combine(Environment.CurrentDirectory, "..\\..\\", fileName);
                        //string createText = "Hello and Welcome" + Environment.NewLine;
                        File.WriteAllText(path, contents.ToString());
                        //MessageBox.Show("Results saved in text file");
                        int positiveresp = getPositiveParameters();
                        int negativeresp = getNegativeParameters();
                        var result = from word in contents.ToString().Split(' ')
                                     group word by word into g
                                     select new { Word = g.Key, Count = g.Count() };
                        IEnumerable<string> words = contents.ToString().Split(' ');
                        int total = words.Count();
                        DisplyDataInChart(positiveresp, negativeresp, total);
                        //Save search to DB
                        BusinessLogic.DatabaseService.AddSearch(new Search
                        {
                            Keyword = keyword,
                            Negative = negativeresp,
                            Positive = positiveresp,
                            SearchDate = DateTime.Now
                        });
                        //Delete old records
                        BusinessLogic.DatabaseService.deleteOldResults(DateTime.Now.AddDays(-7));

                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        private void DisplyDataInChart(int positiveresp, int negativeresp, int total)
        {
            this.chart1.Series.Clear();
           
            chart1.Series.Add("Positive");
            chart1.Series["Positive"].ChartType = SeriesChartType.Column;
            chart1.Series["Positive"].Points.AddXY(0, positiveresp);
            //chart1.Series["Positive"].ChartArea = "ChartArea1";
            chart1.Series["Positive"].Color = System.Drawing.Color.Blue;
            chart1.Series.Add("Negative");
            chart1.Series["Negative"].ChartType = SeriesChartType.Column;
            chart1.Series["Negative"].Points.AddXY(1, negativeresp);
            //chart1.Series["Negative"].ChartArea = "ChartArea1";
            chart1.Series["Negative"].Color = System.Drawing.Color.Red;


            // this.chart1.Series["Positive"].Points.AddXY("Positive", positiveresp);
            //this.chart1.Series["Negative"].Points.AddXY("Negative", negativeresp);

            lblPositive.Text = positive.ToString();
            lblNegative.Text = negative.ToString();
            lblTotal.Text = total.ToString();
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
            String word = "";
            foreach (string positiveElement in positiveList)
            {
                if(positiveElement.StartsWith("\r\n"))
                {
                    word = positiveElement.Substring(2);
                }
                else
                {
                    word = positiveElement;
                }
                if (text.Contains(word))
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
            String word = "";
            foreach (string negativeElement in negativeList)
            {
                if (negativeElement.StartsWith("\r\n"))
                {
                    word = negativeElement.Substring(2);
                }
                else
                {
                    word = negativeElement;
                }


                if (text.Contains(word))
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
            //foreach (string element in clearTextList)
            //{
            //    if (element == "&")
            //    {
            //        text = text.Replace(element, "and");
            //    }
            //    if (element == ",")
            //    {
            //        text = text.Replace(element, " ");
            //    }
            //    else if (element == "  ")
            //    {
            //        text = text.Replace(element, " ");
            //    }
            //    else if (element == " an ")
            //    {
            //        text = text.Replace(element, " ");
            //    }
            //    else if (element == " from ")
            //    {
            //        text = text.Replace(element, " ");
            //    }
            //    else if (element == " the ")
            //    {
            //        text = text.Replace(element, " ");
            //    }
            //    else if (element == " for ")
            //    {
            //        text = text.Replace(element, " ");
            //    }
            //    else
            //    {
            //        text = text.Replace(element, "");
            //    }

            //}

           // text = text.Replace("\"", "");
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

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //circularProgressBar1.AnimationFunction = WinFormAnimation.KnownAnimationFunctions.
        }
        private static Regex _removeRepeatedWhitespaceRegex = new Regex(@"(\s|\n|\r){2,}", RegexOptions.Singleline | RegexOptions.IgnoreCase);

        public static string ExtractViewableTextCleaned(HtmlNode node)
        {
            string textWithLotsOfWhiteSpaces = ExtractViewableText(node);
            return _removeRepeatedWhitespaceRegex.Replace(textWithLotsOfWhiteSpaces, " ");
        }
        public static string ExtractViewableText(HtmlNode node)
        {
            StringBuilder sb = new StringBuilder();
            ExtractViewableTextHelper(sb, node);
            return sb.ToString();
        }
        private static void ExtractViewableTextHelper(StringBuilder sb, HtmlNode node)
        {
            if (node.Name != "script" && node.Name != "style")
            {
                if (node.NodeType == HtmlNodeType.Text)
                {
                    AppendNodeText(sb, node);
                }

                foreach (HtmlNode child in node.ChildNodes)
                {
                    ExtractViewableTextHelper(sb, child);
                }
            }
        }
        private static void AppendNodeText(StringBuilder sb, HtmlNode node)
        {
            string text = ((HtmlTextNode)node).Text;
            if (string.IsNullOrWhiteSpace(text) == false)
            {
                sb.Append(text);

                // If the last char isn't a white-space, add a white space
                // otherwise words will be added ontop of each other when they're only separated by
                // tags
                if (text.EndsWith("\t") || text.EndsWith("\n") || text.EndsWith(" ") || text.EndsWith("\r"))
                {
                    // We're good!
                }
                else
                {
                    sb.Append(" ");
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
               
                //HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
                HtmlAgilityPack.HtmlDocument docHtml = new HtmlWeb().Load(@"http://www.bbc.com/");
                //document.Load(new MemoryStream(File.ReadAllBytes(@"https://lotsacode.wordpress.com/2011/02/11/extract-only-viewable-text-from-html-with-c/")));
               MessageBox.Show(ExtractViewableTextCleaned(docHtml.DocumentNode));
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
    }
