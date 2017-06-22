using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
namespace BusinessLogic
{
   public static class DatabaseService
    {
        public static bool connected  { get; set; }
        public static DataSet queryReasult { get; set; }
       static localDatabaseEntities db = new localDatabaseEntities();

        public static Search IsKeywordExist(String keyword)
        {
            Search result = null; 
            
            if (db.Searches.Where(s => s.Keyword == keyword).Any())
            {
                AutoMapper.Mapper.Initialize(cfg => {
                    cfg.CreateMap<DataAccess.Search, BusinessLogic.Search>();
                });
                var search = db.Searches.Where(s => s.Keyword == keyword).SingleOrDefault();
                result = AutoMapper.Mapper.Map<BusinessLogic.Search>(search);
            }
            return result;
        }
        public static bool AddSearch(Search search)
        {
            bool result;
            try
            {
                AutoMapper.Mapper.Initialize(cfg => {
                    cfg.CreateMap<BusinessLogic.Search, DataAccess.Search>();
                });
                var newSearch = AutoMapper.Mapper.Map<DataAccess.Search>(search);
                db.Searches.Add(newSearch);
                if (db.SaveChanges() > 0)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }

            }
            catch(Exception ex)
            {
                result = false;
            }

            return result;
        }

        public static int deleteOldResults(DateTime from)
        {
            int result = 0;
            try
            {

                var list = db.Searches.Where(s => s.SearchDate < from).ToList();
                foreach (var item in list)
                {
                    db.Searches.Remove(item);
                }
                result = db.SaveChanges();
            }
            catch (Exception ex)
            {
                result = 0;
            }

            return result;
        }


    }
}
