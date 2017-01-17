using NUnit.Framework;

namespace HQF.Tutorials.Dapper.UnitTest
{
    [TestFixture]
    public class TestClass
    {
        public TestClass()
        {
        }

        [Test]
        public void TestMethod()
        {
            DapperForSQLite.CreateAndOpenDb();

            DapperForSQLite.SeedDatabase();
          var  count=DapperForSQLite.GetUsersCount();
            Assert.AreEqual(1,count);

            DapperForSQLite.CreateSecondUser();

            Assert.AreEqual(2,count);

            DapperForSQLite.ModifyAdminUser();

            DapperForSQLite.AddMoreUsers(15);

            Assert.AreEqual(17,count);

            DapperForSQLite.RemoveLastNonAdminUser();

            // TODO: Add your test code here
            Assert.Pass("Your first passing test");
        }
    }
}