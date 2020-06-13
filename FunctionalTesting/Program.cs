using System;
using Xbrl;
using Xbrl.FinancialStatement;
using System.IO;
using Newtonsoft.Json;

namespace FunctionalTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = "C:\\Users\\tihanewi\\Downloads\\hd-20100131.xml";
            Stream s = System.IO.File.OpenRead(path);
            XbrlInstanceDocument doc = XbrlInstanceDocument.Create(s);
            Console.WriteLine("Period context ref: '" + doc.PrimaryPeriodContextId + "'");
            Console.WriteLine("Instant context ref: '" + doc.PrimaryInstantContextId + "'");
            FinancialStatement fs = doc.CreateFinancialStatement();
            string json = JsonConvert.SerializeObject(fs);
            Console.WriteLine(json);
        }
    }
}
