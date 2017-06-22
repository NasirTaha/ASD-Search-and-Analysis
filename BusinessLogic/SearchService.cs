using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BusinessLogic
{
public  static  class SearchService
    {
        static string text = null;
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
        //public DataSet searchResult{ get; set; }
        //public List<string> parameters { get; set; }

        //public string keyword { get; set; }
        public  static Search GetResul(string keyword)
        {
            Search s = null;
            try
            {
                List<SearchResult> results = new List<SearchResult>();

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
                        contents.Append(text);// reader.ReadToEnd();
                    }
                    catch (Exception ex)
                    {
                        //To do: write exception to log message
                    }
                }

                //Wrire to text file
                string path = Environment.CurrentDirectory + "../../../Results.txt";
                File.WriteAllText(path, contents.ToString());
                int positiveresp = getPositiveParameters();
                int negativeresp = getNegativeParameters();
                IEnumerable<string> words = contents.ToString().Split(' ');
                int total = words.Count();
                //// DisplyDataInChart(positiveresp, negativeresp, total);
                s = new Search();
                s.Keyword = keyword;
                s.Negative = negativeresp;
                s.Positive = positiveresp;

              
                //Save search to DB
                BusinessLogic.DatabaseService.AddSearch(new Search
                {
                    Keyword = keyword,
                    Negative = negativeresp,
                    Positive = positiveresp,
                    SearchDate = DateTime.Now,
                    Total = total
                });
                //Delete old records
                BusinessLogic.DatabaseService.deleteOldResults(DateTime.Now.AddDays(-7));

            }
            catch(Exception ex)
            {
                s = null;
            }
            return s;

        }
        private static int getPositiveParameters()
        {
            int positive = 0;
            text = clearTextGenrator();
            List<string> positiveList = createPositiveWordList();
            String word = "";
            foreach (string positiveElement in positiveList)
            {
                if (positiveElement.StartsWith("\r\n"))
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
        private static int getNegativeParameters()
        {
           int negative = 0;
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

        private static List<string> createPositiveWordList()
        {
            string path = Environment.CurrentDirectory + "../../../positive.txt";
            string positiveText = System.IO.File.ReadAllText(path);

            string[] positiveArray = positiveText.Split(',');//<string1/string2/string3/--->     
            List<string> positiveList = new List<string>(); //make a new string list    
            positiveList.AddRange(positiveArray);
            return positiveList;
        }
        private static List<string> createNegativeWordList()
        {
            string path = Environment.CurrentDirectory + "../../../negative.txt";
            string negativeText = System.IO.File.ReadAllText(path);

            string[] negativeArray = negativeText.Split(',');//<string1/string2/string3/--->     
            List<string> negativeList = new List<string>(); //make a new string list    
            negativeList.AddRange(negativeArray);
            return negativeList;

        }

        private static string clearTextGenrator()
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
        static List<string> getListToClearText()
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
