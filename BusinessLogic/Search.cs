using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
   public class Search
    {
       private int _Id;
       private DateTime _SearchDate;
       private int _Positive;
       private int _Negative;
       private String _Keyword;
        public int Id
        {
            get { return _Id; }
            set { _Id = value; }
        }
        public DateTime SearchDate
        {
            get { return _SearchDate; }
            set { _SearchDate = value; }
        }
        public int Positive
        {
            get { return _Positive; }
            set { _Positive = value; }
        }
        public int Negative
        {
            get { return _Negative; }
            set { _Negative = value; }
        }
        public String Keyword
        {
            get { return _Keyword; }
            set { _Keyword = value; }
        }
        public Search()
        {
            this._Id = 0;
            this._SearchDate = DateTime.MinValue;
            this._Positive = int.MinValue;
            this._Negative = int.MinValue;
            this._Keyword = String.Empty;
        }
        public Search(int Id, DateTime SearchDate, int Positive, int Negative, String Keyword)
        {
            this._Id = Id;
            this._SearchDate = SearchDate;
            this._Positive = Positive;
            this._Negative = Negative;
            this._Keyword = Keyword;
        }
    }
}
