using System;
using System.Collections.Generic;

namespace Xbrl.FinancialStatement
{
    public class FinancialStatementHeader
        {
            public string Symbol { get; set; }
            public string DocumentType { get; set; }
            public DateTime PeriodEndDate { get; set; }

            public string AsString()
            {
                if (Symbol == null)
                {
                    throw new Exception("Unable to serialize Financial Statement Header. Symbol was null.");
                }
                else
                {
                    if (Symbol == "")
                    {
                        throw new Exception("Unable to serialize Financial Statement Header.  Symbol was blank.");
                    }
                }

                if (DocumentType == null)
                {
                    throw new Exception("Unable to serialize Financial Statement Header. Document Type was null.");
                }
                else
                {
                    if (DocumentType == "")
                    {
                        throw new Exception("Unable to serialize Financial Statement Header.  Document Type was blank.");
                    }
                }

                string ToReturn = "";
                ToReturn = Symbol.ToLower() + "." + DocumentType.ToLower().Replace("\"", "") + "." + PeriodEndDate.Month.ToString("00") + PeriodEndDate.Day.ToString("00") + PeriodEndDate.Year.ToString("0000");
                return ToReturn;


            }

            public static FinancialStatementHeader Parse(string representation)
            {
                FinancialStatementHeader fst = new FinancialStatementHeader();

                List<string> Splitter = new List<string>();
                Splitter.Add(".");
                string[] parts = representation.Split(Splitter.ToArray(), StringSplitOptions.None);

                if (parts.Length != 3)
                {
                    throw new Exception("Unable to parse string into Financial Statement Header.  There were more or less than 3 parts in the string (separated by period).");
                }

                fst.Symbol = parts[0];
                fst.DocumentType = parts[1];
                fst.PeriodEndDate = new DateTime(Convert.ToInt32(parts[2].Substring(4, 4)), Convert.ToInt32(parts[2].Substring(0, 2)), Convert.ToInt32(parts[2].Substring(2, 2)));

                return fst;
            }

        }
}