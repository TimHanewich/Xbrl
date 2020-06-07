using System;
using Xbrl.Helpers;

namespace Xbrl.FinancialStatement
{
    public static class FinancialStatementHelper
    {
        public static FinancialStatement CreateFinancialStatement(this XbrlInstanceDocument doc)
        {
            FinancialStatement ToReturn = new FinancialStatement();
            ToReturn.FinancialStatementGeneratedAtUtc = DateTime.UtcNow;

            //Document Type (Header)
            ToReturn.Header.DocumentType = doc.DocumentType;

            //Period End Date (Header)
            if (doc.PrimaryPeriodContextId != "")
            {
                try
                {
                    XbrlContext con = doc.GetContextById(doc.PrimaryPeriodContextId);
                    ToReturn.Header.PeriodEndDate = con.EndDate;
                }
                catch
                {
                    ToReturn.Header.PeriodEndDate = DateTime.Parse("1/1/1900");
                }
            }

            //Symbol (header)
            ToReturn.Header.Symbol = doc.TradingSymbol;

            //Period start
            try
            {
                XbrlContext con = doc.GetContextById(doc.PrimaryPeriodContextId);
                ToReturn.PeriodStart = con.StartDate;
                ToReturn.PeriodEnd = con.EndDate;
            }
            catch
            {

            }

            //Revenue
            try
            {
                ToReturn.Revenue = doc.GetValueFromPriorities("Revenue", "Revenues", "RevenueFromContractWithCustomerExcludingAssessedTax", "RevenueFromContractWithCustomerIncludingAssessedTax", "RevenueFromContractWithCustomerBeforeReimbursementsExcludingAssessedTax", "SalesRevenueNet").Value;
            }
            catch
            {
                ToReturn.Revenue = null;
            }

            

            //Net income
            try
            {
                ToReturn.NetIncome = doc.GetValueFromPriorities("NetIncomeLoss","IncomeLossFromContinuingOperationsIncludingPortionAttributableToNoncontrollingInterest","ProfitLoss").Value;
            }
            catch
            {
                ToReturn.NetIncome = null;
            }

            //Assets
            try
            {
                ToReturn.Assets = doc.GetValueFromPriorities("Assets").Value;
            }
            catch
            {
                ToReturn.Assets = null;
            }

            //Liabilities
            try
            {
                ToReturn.Liabilities = doc.GetValueFromPriorities("Liabilities").Value;
            }
            catch
            {
                ToReturn.Liabilities = null;
            }

            //Equity
            try
            {
                ToReturn.Equity = doc.GetValueFromPriorities("Equity","StockholdersEquity","StockholdersEquityIncludingPortionAttributableToNoncontrollingInterest").Value;
            }
            catch
            {
                ToReturn.Equity = null;
            }

            //If only liabilities or equity were able to be found, fill in the rest
            if (ToReturn.Assets != null)
            {
                if (ToReturn.Liabilities == null && ToReturn.Equity != null)
                {
                    ToReturn.Liabilities = ToReturn.Assets - ToReturn.Equity;
                }
                else if (ToReturn.Equity == null && ToReturn.Liabilities != null)
                {
                    ToReturn.Equity = ToReturn.Assets - ToReturn.Liabilities;
                }
            }
            else
            {
                if (ToReturn.Liabilities != null && ToReturn.Equity != null)
                {
                    ToReturn.Assets = ToReturn.Liabilities + ToReturn.Equity;
                }
            }

            //Cash
            try
            {
                ToReturn.Cash = doc.GetValueFromPriorities("CashAndCashEquivalents","CashAndCashEquivalentsAtCarryingValue").Value;
            }
            catch
            {
                ToReturn.Cash = null;
            }

            //Current Assets
            try
            {
                ToReturn.CurrentAssets = doc.GetValueFromPriorities("AssetsCurrent").Value;
            }
            catch
            {
                ToReturn.CurrentAssets = null;
            }

            //Current Libilities
            try
            {
                ToReturn.CurrentLiabilities = doc.GetValueFromPriorities("LiabilitiesCurrent").Value;
            }
            catch
            {
                ToReturn.CurrentLiabilities = null;
            }

            //Retained Earnings
            try
            {
                ToReturn.RetainedEarnings = doc.GetValueFromPriorities("RetainedEarningsAccumulatedDeficit").Value;
            }
            catch
            {
                ToReturn.RetainedEarnings = null;
            }


            //Operating Cash Flows
            try
            {
                ToReturn.OperatingCashFlows = doc.GetValueFromPriorities("NetCashProvidedByUsedInOperatingActivities").Value;
            }
            catch
            {
                ToReturn.OperatingCashFlows = null;
            }

            //Investing Cash Flows
            try
            {
                ToReturn.InvestingCashFlows = doc.GetValueFromPriorities("NetCashProvidedByUsedInInvestingActivities").Value;
            }
            catch
            {
                ToReturn.InvestingCashFlows = null;
            }

            //Finance cash flows
            try
            {
                ToReturn.FinancingCashFlows = doc.GetValueFromPriorities("NetCashProvidedByUsedInFinancingActivities").Value;
            }
            catch
            {
                ToReturn.FinancingCashFlows = null;
            }

            return ToReturn;
        }
    }
}