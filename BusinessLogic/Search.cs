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
       private float _Positive;
       private float _Negative;
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
        public float Positive
        {
            get { return _Positive; }
            set { _Positive = value; }
        }
        public float Negative
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
            this._Positive = float.MinValue;
            this._Negative = float.MinValue;
            this._Keyword = String.Empty;
        }
        public Search(int Id, DateTime SearchDate, float Positive, float Negative, String Keyword)
        {
            this._Id = Id;
            this._SearchDate = SearchDate;
            this._Positive = Positive;
            this._Negative = Negative;
            this._Keyword = Keyword;
        }
    }
}
