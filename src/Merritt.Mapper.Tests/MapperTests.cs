using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Merritt.Mapper.Tests
{
    [TestClass]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public class MapperTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException),"source")]
        public void MapFromDataReaderThrowsArgumentNullException()
        {
            (new Mapper()).MapFromDataReader<TestDestination>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MapFromDataReaderThrowsExceptionWhenConvertingInvalidStringToInteger()
        {
            var dataSet = new DataSet();
            dataSet.Tables.Add(new DataTable());
            dataSet.Tables[0].Columns.Add(new DataColumn("test_value_3", System.Type.GetType("System.Int32")));
            var dataRow = dataSet.Tables[0].NewRow();
            dataRow["test_value_3"] = "2014-12-05T19:30:00-06:00";
            dataSet.Tables[0].Rows.Add(dataRow);
            var dateTime = DateTime.Parse("2014-12-05T19:30:00-06:00");

            var results = (new Mapper()).MapFromDataReader<TestDestination>(dataSet.Tables[0].CreateDataReader());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MapFromDataReaderThrowsExceptionWhenConvertingInvalidStringToDateTime()
        {
            var dataSet = new DataSet();
            dataSet.Tables.Add(new DataTable());
            dataSet.Tables[0].Columns.Add(new DataColumn("test_value_3", System.Type.GetType("System.Int32")));
            var dataRow = dataSet.Tables[0].NewRow();
            dataRow["test_value_3"] = "1";
            dataSet.Tables[0].Rows.Add(dataRow);

            var results = (new Mapper()).MapFromDataReader<TestDestination>(dataSet.Tables[0].CreateDataReader());
        }

        [TestMethod]
        public void MapFromDataReaderAssignsStringToDateTimeValue()
        {
            var dataSet = new DataSet();
            dataSet.Tables.Add(new DataTable());
            dataSet.Tables[0].Columns.Add(new DataColumn("test_value_3", System.Type.GetType("System.String")));
            var dataRow = dataSet.Tables[0].NewRow();
            dataRow["test_value_3"] = "2014-12-05T19:30:00-06:00";
            dataSet.Tables[0].Rows.Add(dataRow);
            var dateTime = DateTime.Parse("2014-12-05T19:30:00-06:00");

            var results = (new Mapper()).MapFromDataReader<TestDestination>(dataSet.Tables[0].CreateDataReader());

            Assert.AreEqual(dateTime, results[0].TestValue3);
        }

        [TestMethod]
        public void MapFromDataReaderAssignsStringToIntegerValue()
        {
            var dataSet = new DataSet();
            dataSet.Tables.Add(new DataTable());
            dataSet.Tables[0].Columns.Add(new DataColumn("test_value_2", System.Type.GetType("System.String")));
            var dataRow = dataSet.Tables[0].NewRow();
            dataRow["test_value_2"] = "1";
            dataSet.Tables[0].Rows.Add(dataRow);

            var results = (new Mapper()).MapFromDataReader<TestDestination>(dataSet.Tables[0].CreateDataReader());

            Assert.AreEqual(1, results[0].TestValue2);
        }

        [TestMethod]
        public void MapFromDataReaderDefaultScenario()
        {
            var dataSet = new DataSet();
            dataSet.Tables.Add(new DataTable());
            dataSet.Tables[0].Columns.Add(new DataColumn("test_value_1", System.Type.GetType("System.String")));
            dataSet.Tables[0].Columns.Add(new DataColumn("test_value_2", System.Type.GetType("System.Int32")));
            dataSet.Tables[0].Columns.Add(new DataColumn("test_value_3", System.Type.GetType("System.DateTime")));
            var dataRow = dataSet.Tables[0].NewRow();
            dataRow["test_value_1"] = "test value";
            dataRow["test_value_2"] = 2;
            var datetime1 = DateTime.Now;
            dataRow["test_value_3"] = datetime1;
            dataSet.Tables[0].Rows.Add(dataRow);
            dataRow = dataSet.Tables[0].NewRow();
            dataRow["test_value_1"] = "test value2";
            dataRow["test_value_2"] = 3;
            var datetime2 = DateTime.Now;
            dataRow["test_value_3"] = datetime2;
            dataSet.Tables[0].Rows.Add(dataRow);

            var results = (new Mapper()).MapFromDataReader<TestDestination>(dataSet.Tables[0].CreateDataReader());
            
            Assert.AreEqual(results[0].TestValue1, "test value");
            Assert.AreEqual(results[0].TestValue2, 2);
            Assert.AreEqual(results[0].TestValue3, datetime1);
            Assert.AreEqual(results[1].TestValue1, "test value2");
            Assert.AreEqual(results[1].TestValue2, 3);
            Assert.AreEqual(results[1].TestValue3, datetime2);
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class TestDestination
        {
            public string TestValue1 { get; set; }
            public int TestValue2 { get; set; }
            public DateTime TestValue3 { get; set; }
        }
    }
}