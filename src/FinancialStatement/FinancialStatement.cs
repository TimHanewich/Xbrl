using System;

namespace Xbrl.FinancialStatement
{
    public class FinancialStatement
        {
            //General data
            public DateTime? PeriodStart { get; set; }
            public DateTime? PeriodEnd { get; set; }

            //Income Statement Items
            public float? Revenue { get; set; }
            public float? SellingGeneralAndAdministrativeExpense {get; set;}
            public float? ResearchAndDevelopmentExpense {get; set;}
            public float? OperatingIncome {get; set;}
            public float? NetIncome { get; set; }

            //Balance Sheet Items
            public float? Assets { get; set; }
            public float? Liabilities { get; set; }
            public float? Equity { get; set; }
            public float? Cash { get; set; }
            public float? CurrentAssets { get; set; }
            public float? CurrentLiabilities { get; set; }
            public float? RetainedEarnings { get; set; }
            public long? CommonStockSharesOutstanding {get; set;}
            

            //Cash Flow Statement Items
            public float? OperatingCashFlows {get; set;}
            public float? InvestingCashFlows {get; set;}
            public float? FinancingCashFlows {get; set;}
            public float? ProceedsFromIssuanceOfDebt {get; set;}
            public float? PaymentsOfDebt {get; set;}
            public float? DividendsPaid {get; set;}
        }
}