using System;
using Xbrl.Helpers;
using TimHanewich.Csv;

namespace Xbrl.FinancialStatement
{
    public static class FinancialStatementHelper
    {
        public static FinancialStatement CreateFinancialStatement(this XbrlInstanceDocument doc)
        {
            FinancialStatement ToReturn = new FinancialStatement();      

            #region "Contextual (misc) info"

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
                ToReturn.ProceedsFromIssuanceOfDebt = doc.GetValueFromPriorities("ProceedsFromIssuanceOfDebt", "ProceedsFromDebtMaturingInMoreThanThreeMonths", "ProceedsFromIssuanceOfLongTermDebt", "ProceedsFromIssuanceOfLongTermDebtAndCapitalSecuritiesNet").Value;
            }
            catch
            {
                ToReturn.ProceedsFromIssuanceOfDebt = null;
            }
            
            //Payments of debt
            try
            {
                ToReturn.PaymentsOfDebt = doc.GetValueFromPriorities("RepaymentsOfDebt", "RepaymentsOfDebtMaturingInMoreThanThreeMonths", "RepaymentsOfLongTermDebt", "RepaymentsOfLongTermDebtAndCapitalSecurities").Value;
            }
            catch
            {
                ToReturn.PaymentsOfDebt = null;
            }
            
            
            
            #endregion
            
            
            return ToReturn;
        }
    
        public static string PrintFinancialStatements(FinancialStatement[] statements)
        {
            #region  "Error checking"

            if (statements == null)
            {
                throw new Exception("Unable to print financial statements - the supplied array of statements was null.");
            }

            #endregion
        
            CsvFile csv = new CsvFile();

            //Headers
            DataRow dr_header = csv.AddNewRow();
            dr_header.Values.Add("Period Start");
            dr_header.Values.Add("Period End");
            dr_header.Values.Add("Revenue");
            dr_header.Values.Add("SGA Expenses");
            dr_header.Values.Add("R&D Expenses");
            dr_header.Values.Add("Operating Income");
            dr_header.Values.Add("Net Income");
            dr_header.Values.Add("Assets");
            dr_header.Values.Add("Liabilities");
            dr_header.Values.Add("Equity");
            dr_header.Values.Add("Cash");
            dr_header.Values.Add("Current Assets");
            dr_header.Values.Add("Current Liabilities");
            dr_header.Values.Add("Retained Earnings");
            dr_header.Values.Add("Common Stock Shares Outstanding");
            dr_header.Values.Add("Operating Cash Flows");
            dr_header.Values.Add("Investing Cash Flows");
            dr_header.Values.Add("Financing Cash Flows");
            dr_header.Values.Add("Proceeds from Issuance of Debt");
            dr_header.Values.Add("Payments of Debt");

            //Add each value
            foreach (FinancialStatement fs in statements)
            {
                DataRow dr = csv.AddNewRow();

                //Start date
                if (fs.PeriodStart.HasValue)
                {
                    dr.Values.Add(fs.PeriodStart.Value.ToShortDateString());
                }
                else
                {
                    dr.Values.Add("-");
                }

                //End date
                if (fs.PeriodEnd.HasValue)
                {
                    dr.Values.Add(fs.PeriodEnd.Value.ToShortDateString());
                }
                else
                {
                    dr.Values.Add("-");
                }

                //Revenue
                if (fs.Revenue.HasValue)
                {
                    dr.Values.Add(fs.Revenue.Value.ToString("#,##0"));
                }
                else
                {
                    dr.Values.Add("-");
                }

                //SGA expenses
                if (fs.SellingGeneralAndAdministrativeExpense.HasValue)
                {
                    dr.Values.Add(fs.SellingGeneralAndAdministrativeExpense.Value.ToString("#,##0"));
                }
                else
                {
                    dr.Values.Add("-");
                }

                //Research and Development costs
                if (fs.ResearchAndDevelopmentExpense.HasValue)
                {
                    dr.Values.Add(fs.ResearchAndDevelopmentExpense.Value.ToString("#,##0"));
                }
                else
                {
                    dr.Values.Add("-");
                }

                //Operating income
                if (fs.OperatingIncome.HasValue)
                {
                    dr.Values.Add(fs.OperatingIncome.Value.ToString("#,##0"));
                }
                else
                {
                    dr.Values.Add("-");
                }

                //Net Income
                if (fs.NetIncome.HasValue)
                {
                    dr.Values.Add(fs.NetIncome.Value.ToString("#,##0"));
                }
                else
                {
                    dr.Values.Add("-");
                }


                //Assets
                if (fs.Assets.HasValue)
                {
                    dr.Values.Add(fs.Assets.Value.ToString("#,##0"));
                }
                else
                {
                    dr.Values.Add("-");
                }

                //Liabilities
                if (fs.Liabilities.HasValue)
                {
                    dr.Values.Add(fs.Liabilities.Value.ToString("#,##0"));
                }
                else
                {
                    dr.Values.Add("-");
                }

                //Euity
                if (fs.Equity.HasValue)
                {
                    dr.Values.Add(fs.Equity.Value.ToString("#,##0"));
                }
                else
                {
                    dr.Values.Add("-");
                }

                //Casg
                if (fs.Cash.HasValue)
                {
                    dr.Values.Add(fs.Cash.Value.ToString("#,##0"));
                }
                else
                {
                    dr.Values.Add("-");
                }

                //Current Assets
                if (fs.CurrentAssets.HasValue)
                {
                    dr.Values.Add(fs.CurrentAssets.Value.ToString("#,##0"));
                }
                else
                {
                    dr.Values.Add("-");
                }

                //Current Liabilities
                if (fs.CurrentLiabilities.HasValue)
                {
                    dr.Values.Add(fs.CurrentLiabilities.Value.ToString("#,##0"));
                }
                else
                {
                    dr.Values.Add("-");
                }

                //Retained Earnings
                if (fs.RetainedEarnings.HasValue)
                {
                    dr.Values.Add(fs.RetainedEarnings.Value.ToString("#,##0"));
                }
                else
                {
                    dr.Values.Add("-");
                }

                //Common stock shares outstanding
                if (fs.CommonStockSharesOutstanding.HasValue)
                {
                    dr.Values.Add(fs.CommonStockSharesOutstanding.Value.ToString("#,##0"));
                }
                else
                {
                    dr.Values.Add("-");
                }

                //Operating cash flows
                if (fs.OperatingCashFlows.HasValue)
                {
                    dr.Values.Add(fs.OperatingCashFlows.Value.ToString("#,##0"));
                }
                else
                {
                    dr.Values.Add("-");
                }

                //Investing cash flows
                if (fs.InvestingCashFlows.HasValue)
                {
                    dr.Values.Add(fs.InvestingCashFlows.Value.ToString("#,##0"));
                }
                else
                {
                    dr.Values.Add("-");
                }

                //Financing Cash flows
                if (fs.FinancingCashFlows.HasValue)
                {
                    dr.Values.Add(fs.FinancingCashFlows.Value.ToString("#,##0"));
                }
                else
                {
                    dr.Values.Add("-");
                }

                //Proceeds from issuance of debt
                if (fs.ProceedsFromIssuanceOfDebt.HasValue)
                {
                    dr.Values.Add(fs.ProceedsFromIssuanceOfDebt.Value.ToString("#,##0"));
                }
                else
                {
                    dr.Values.Add("-");
                }

                //Payments of debt
                if (fs.PaymentsOfDebt.HasValue)
                {
                    dr.Values.Add(fs.PaymentsOfDebt.Value.ToString("#,##0"));
                }
                else
                {
                    dr.Values.Add("-");
                }
            }

            return csv.GenerateAsCsvFileContent();
        }
    }
}