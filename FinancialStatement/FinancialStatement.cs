using System;

namespace Xbrl.FinancialStatement
{
    public class FinancialStatement
        {
            public FinancialStatementHeader Header { get; set; }
            public DateTime PeriodStart { get; set; }
            public DateTime PeriodEnd { get; set; }

            //Income Statement Items
            public float? Revenue { get; set; }
            public float? NetIncome { get; set; }

            //Balance Sheet Items
            public float? Assets { get; set; }
            public float? Liabilities { get; set; }
            public float? Equity { get; set; }
            public float? Cash { get; set; }
            public float? CurrentAssets { get; set; }
            public float? CurrentLiabilities { get; set; }
            public float? RetainedEarnings { get; set; }
            public DateTime FinancialStatementGeneratedAtUtc { get; set; }

            public FinancialStatement()
            {
                Header = new FinancialStatementHeader();
            }
        }
}