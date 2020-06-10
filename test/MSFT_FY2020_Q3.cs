using System;
using Xbrl;
using Xunit;
using System.IO;
using Xunit.Abstractions;
using Xunit.Extensions;
using Xunit.Sdk;
using System.Linq;

namespace test
{
    public class MSFT_FY2020_Q3
    {
        private readonly ITestOutputHelper output;

        public MSFT_FY2020_Q3(ITestOutputHelper _helper)
        {
            output = _helper;
        }
      
        [Fact]
        public void TestOpen()
        {
            XbrlInstanceDocument doc = XbrlInstanceDocument.Create(GetMSFT_FY2020_Q3());

            output.WriteLine("Primary Instance Context ID: " + doc.PrimaryInstantContextId);
            output.WriteLine("Primary Period Context ID: " + doc.PrimaryPeriodContextId);

            Assert.True(false);
        }


        //Link: https://www.sec.gov/Archives/edgar/data/789019/000156459020019706/0001564590-20-019706-index.htm
        private Stream GetMSFT_FY2020_Q3()
        {
            string path = GetTestProjectPath() + "\\MSFT_FY2020_Q3.xml";
            Stream s = System.IO.File.Open(path, FileMode.Open);
            return s;
        }

        private string GetTestProjectPath()
        {
            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string parent1 = System.IO.Directory.GetParent(exePath).FullName;
            string parent2 = System.IO.Directory.GetParent(parent1).FullName;
            string parent3 = System.IO.Directory.GetParent(parent2).FullName;
            string parent4 = System.IO.Directory.GetParent(parent3).FullName;
            return parent4;
        }
    }
}
