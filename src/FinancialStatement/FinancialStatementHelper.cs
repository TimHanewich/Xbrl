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


            #region "Contextual (misc) info"
            //Inaccuracy Flag
            if (doc.PrimaryPeriodContextIdInaccuracyRiskFlag == true)
            {
                ToReturn.InaccuracyRiskFlag = true;
            }


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

            //Common Stock Shares Outstanding
            try
            {
                ToReturn.CommonStockSharesOutstanding = Convert.ToInt64(doc.GetValueFromPriorities("CommonStockSharesOutstanding", "EntityCommonStockSharesOutstanding").Value);
            }
            catch
            {
                ToReturn.CommonStockSharesOutstanding = null;
            }

            #endregion

            #region "Income Statement"
            //Revenue
            try
            {
                ToReturn.Revenue = doc.GetValueFromPriorities("Revenue", "Revenues", "RevenueFromContractWithCustomerExcludingAssessedTax", "RevenueFromContractWithCustomerIncludingAssessedTax", "RevenueFromContractWithCustomerBeforeReimbursementsExcludingAssessedTax", "SalesRevenueNet", "SalesRevenueGoodsNet", "TotalRevenuesAndOtherIncome").Value;
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

            //Operating Income
            try
            {
                ToReturn.OperatingIncome = doc.GetValueFromPriorities("OperatingIncomeLoss").Value;
            }
            catch
            {
                ToReturn.OperatingIncome = null;
            }

            //Selling general and administrative expense
            try
            {
                ToReturn.SellingGeneralAndAdministrativeExpense = doc.GetValueFromPriorities("SellingGeneralAndAdministrativeExpense").Value;
            }
            catch
            {
                ToReturn.SellingGeneralAndAdministrativeExpense = null;
            }

            //Research and development expense
            try
            {
                ToReturn.ResearchAndDevelopmentExpense = doc.GetValueFromPriorities("ResearchAndDevelopmentExpense", "ResearchAndDevelopmentExpenseExcludingAcquiredInProcessCost").Value;
            }
            catch
            {
                ToReturn.ResearchAndDevelopmentExpense = null;
            }


            #endregion
         
            #region "Balance Sheet"
         
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
                ToReturn.Cash = doc.GetValueFromPriorities("CashAndCashEquivalents","CashAndCashEquivalentsAtCarryingValue", "CashCashEquivalentsRestrictedCashAndRestrictedCashEquivalents", "CashCashEquivalentsRestrictedCashAndRestrictedCashEquivalents").Value;
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

            

#endregion
            
            #region "Cash Flows"
            
            //Operating Cash Flows
            try
            {
                ToReturn.OperatingCashFlows = doc.GetValueFromPriorities("NetCashProvidedByUsedInOperatingActivities", "NetCashProvidedByUsedInOperatingActivitiesContinuingOperations").Value;
            }
            catch
            {
                ToReturn.OperatingCashFlows = null;
            }

            //Investing Cash Flows
            try
            {
                ToReturn.InvestingCashFlows = doc.GetValueFromPriorities("NetCashProvidedByUsedInInvestingActivities", "NetCashProvidedByUsedInInvestingActivitiesContinuingOperations").Value;
            }
            catch
            {
                ToReturn.InvestingCashFlows = null;
            }

            //Finance cash flows
            try
            {
                ToReturn.FinancingCashFlows = doc.GetValueFromPriorities("NetCashProvidedByUsedInFinancingActivities", "NetCashProvidedByUsedInFinancingActivitiesContinuingOperations").Value;
            }
            catch
            {
                ToReturn.FinancingCashFlows = null;
            }

            //ProceedsFromIssuanceOfDebt
            try
            {
                ToReturn.ProceedsFromIssuanceOfDebt = doc.GetValueFromPriorities("ProceedsFromIssuanceOfDebt").Value;
            }
            catch
            {
                ToReturn.ProceedsFromIssuanceOfDebt = null;
            }
            
            
            
            
            #endregion
            
            
            return ToReturn;
        }
    }
}