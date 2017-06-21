using Microsoft.VisualStudio.TestTools.UnitTesting;
using BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests
{
    [TestClass()]
    public class DatabaseServiceTests
    {
        [TestMethod()]
        public void deleteOldResultsTest()
        {

            Assert.Fail();
        }

        [TestMethod()]
        public void AddSearchTest()
        {
            var result = BusinessLogic.DatabaseService.AddSearch(new Search
            {
                Keyword = "test key word",
                Negative = 1,
                Positive = 1,
                SearchDate = DateTime.Now
            });
            Assert.IsTrue(result);
        }
    }
}