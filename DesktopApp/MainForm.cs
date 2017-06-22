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
                btnSearch.Enabled = false;
                //Search our old results first
                var keyword = txtKeyword.Text.ToLower();
                BusinessLogic.Search exist = BusinessLogic.DatabaseService.IsKeywordExist(keyword);
                if (!string.IsNullOrEmpty(keyword))
                {
                    if (exist != null)
                    {
                        DisplyDataInChart(exist.Positive, exist.Negative, exist.Total);

                    }
                    else
                    {
                        exist =  BusinessLogic.SearchService.GetResul(keyword);
                        DisplyDataInChart(exist.Positive, exist.Negative, exist.Total);
                    }
                }
                else

                {
                    MessageBox.Show("Pease enter the keyword");
                }
                btnSearch.Enabled = true;
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
            chart1.Series["Positive"].Color = System.Drawing.Color.Blue;
            chart1.Series.Add("Negative");
            chart1.Series["Negative"].ChartType = SeriesChartType.Column;
            chart1.Series["Negative"].Points.AddXY(1, negativeresp);
            chart1.Series["Negative"].Color = System.Drawing.Color.Red;
            lblPositive.Text = positiveresp.ToString();
            lblNegative.Text = negativeresp.ToString();
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
       

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //circularProgressBar1.AnimationFunction = WinFormAnimation.KnownAnimationFunctions.
        }
       

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
               
               // //HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
               // HtmlAgilityPack.HtmlDocument docHtml = new HtmlWeb().Load(@"http://www.bbc.com/");
               // //document.Load(new MemoryStream(File.ReadAllBytes(@"https://lotsacode.wordpress.com/2011/02/11/extract-only-viewable-text-from-html-with-c/")));
               //MessageBox.Show(ExtractViewableTextCleaned(docHtml.DocumentNode));
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
    }
