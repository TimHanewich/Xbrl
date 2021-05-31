using System;
using System.IO;
using System.Collections.Generic;

namespace Xbrl
{
    public class XbrlInstanceDocument
    {
        //Facts
        public string TradingSymbol { get; set; }
        public string DocumentType { get; set; }
        public XbrlContext[] Contexts { get; set; }
        public XbrlFact[] Facts { get; set; }
        public string PrimaryPeriodContextId { get; set; }
        public string PrimaryInstantContextId { get; set; }

        public static XbrlInstanceDocument Create(Stream s)
        {
            XbrlInstanceDocument ToReturn = new XbrlInstanceDocument();

            StreamReader sr = new StreamReader(s);
            int loc1 = 0;
            int loc2 = 0;
            

            #region "Get Contexts"
            s.Position = 0;
            List<XbrlContext> Contexts = new List<XbrlContext>();
            do
            {
                string line = sr.ReadLine();

                if (line.Contains("<context") || line.Contains("<xbrli:context"))
                {
                    XbrlContext con = new XbrlContext();
                    loc1 = line.IndexOf("id");
                    loc1 = line.IndexOf("\"", loc1 + 1);
                    loc2 = line.IndexOf("\"", loc1 + 1);
                    con.Id = line.Substring(loc1 + 1, loc2 - loc1 - 1).Trim();

                    do
                    {
                        string nl = sr.ReadLine().Trim();


                        if (nl.Contains("startDate"))
                        {
                            loc1 = nl.IndexOf("startDate");
                            loc1 = nl.IndexOf(">", loc1 + 1);
                            loc2 = nl.IndexOf("<", loc1 + 1);
                            con.StartDate = DateTime.Parse(nl.Substring(loc1 + 1, loc2 - loc1 - 1));
                            con.TimeType = XbrlTimeType.Period;
                        }

                        if (nl.Contains("endDate"))
                        {
                            loc1 = nl.IndexOf("endDate");
                            loc1 = nl.IndexOf(">", loc1 + 1);
                            loc2 = nl.IndexOf("<", loc1 + 1);
                            con.EndDate = DateTime.Parse(nl.Substring(loc1 + 1, loc2 - loc1 - 1));
                            con.TimeType = XbrlTimeType.Period;
                        }

                        if (nl.Contains("<instant>") || nl.Contains("<xbrli:instant"))
                        {
                            loc1 = nl.IndexOf("instant");
                            loc1 = nl.IndexOf(">", loc1 + 1);
                            loc2 = nl.IndexOf("<", loc1 + 1);
                            con.InstantDate = DateTime.Parse(nl.Substring(loc1 + 1, loc2 - loc1 - 1));
                            con.TimeType = XbrlTimeType.Instant;
                        }

                        if (nl.Contains("</context") || nl.Contains("</xbrli:context"))
                        {
                            break;
                        }
                    } while (true);

                    Contexts.Add(con);
                }


            } while (sr.EndOfStream == false);

            ToReturn.Contexts = Contexts.ToArray();

            #endregion

            #region "Get Facts"
            s.Position = 0;
            List<XbrlFact> Facts = new List<XbrlFact>();
            do
            {

                string line = sr.ReadLine().Trim();


                if (line.Contains("<") && line.Contains(":") && line.Contains("<context") == false && line.Contains("<xbrli:context") == false && line.Contains("DocumentType") == false && line.Contains("<dei:TradingSymbol") == false && line.Contains("<!--") == false)
                {


                    string FactData = line;

                    //Get the next few lines for the remainder of the fact if it is not all on one line
                    if (FactData.Contains("</") == false && FactData.Contains("/>") == false)
                    {
                        do
                        {
                            string nl = sr.ReadLine();
                            FactData = FactData + " " + nl;
                        } while (FactData.Contains("</") == false && FactData.Contains("/>") == false);
                    }

                    //If this is indeed a fact (it appears to have all of the parts), get data from it
                    if (FactData.Contains("contextRef") && FactData.Contains("unitRef"))
                    {
                        XbrlFact fact = new XbrlFact();

                        //get the namspace
                        loc1 = FactData.IndexOf("<");
                        loc2 = FactData.IndexOf(":");
                        fact.NamespaceId = FactData.Substring(loc1 + 1, loc2 - loc1 - 1);

                        //Get the label
                        loc1 = FactData.IndexOf(":");
                        loc2 = FactData.IndexOf(" ");
                        fact.Label = FactData.Substring(loc1 + 1, loc2 - loc1 - 1);

                        //Get decimals
                        loc1 = FactData.IndexOf("decimals=");
                        if (loc1 != -1)
                        {
                            loc1 = FactData.IndexOf("\"", loc1 + 1);
                            loc2 = FactData.IndexOf("\"", loc1 + 1);
                            if (loc1 != -1 && loc2 != -1)
                            {
                                string decstr = FactData.Substring(loc1 + 1, loc2 - loc1 - 1);
                                if (decstr.ToLower() != "inf")
                                {
                                    fact.Decimals = Convert.ToInt32(decstr);
                                }
                                else
                                {
                                    fact.Decimals = 0;
                                }
                            }
                        }


                        //Get id
                        loc1 = FactData.IndexOf("id=");
                        loc1 = FactData.IndexOf("\"", loc1 + 1);
                        loc2 = FactData.IndexOf("\"", loc1 + 1);
                        if (loc1 != -1 && loc2 != -1)
                        {
                            fact.Id = FactData.Substring(loc1 + 1, loc2 - loc1 - 1);
                        }

                        //Get unit ref
                        loc1 = FactData.IndexOf("unitRef=");
                        loc1 = FactData.IndexOf("\"", loc1 + 1);
                        loc2 = FactData.IndexOf("\"", loc1 + 1);
                        if (loc1 != -1 && loc2 != -1)
                        {
                            fact.UnitId = FactData.Substring(loc1 + 1, loc2 - loc1 - 1);
                        }

                        //Get context ref
                        loc1 = FactData.IndexOf("contextRef=");
                        loc1 = FactData.IndexOf("\"", loc1 + 1);
                        loc2 = FactData.IndexOf("\"", loc1 + 1);
                        if (loc1 != -1 && loc2 != -1)
                        {
                            fact.ContextId = FactData.Substring(loc1 + 1, loc2 - loc1 - 1);
                        }

                        //Get the value
                        if (FactData.Contains("/>") == false)
                        {
                            loc1 = FactData.IndexOf(">");
                            loc2 = FactData.IndexOf("<", loc1 + 1);
                            string valstr = FactData.Substring(loc1 + 1, loc2 - loc1 - 1);
                            if (valstr != "")
                            {
                                //This try bracket is in here because some XBRL files have more than just values in them (this is an error)... for example, XOM's 2017 filing
                                try
                                {
                                    fact.Value = valstr;
                                }
                                catch
                                {
                                    fact.Value = null;
                                }
                            }                            
                        }



                        Facts.Add(fact);
                    }
                }

            } while (sr.EndOfStream == false);
            ToReturn.Facts = Facts.ToArray();
            #endregion

            
            #region "Get Trading Symbol"
            bool TradingSymbolAlreadySet = false; //This is here
            s.Position = 0;
            do
            {
                string line = sr.ReadLine();
                //Is it the Trading Symbol?
                if (line.Contains("<dei:TradingSymbol"))
                {
                    string focus = line;
                    if (focus.Contains("</dei:TradingSymbol") == false)
                    {
                        do
                        {
                            string nl = sr.ReadLine();
                            focus = focus + " " + nl;
                        } while (focus.Contains("</dei:TradingSymbol") == false);
                    }
                    loc1 = focus.IndexOf("TradingSymbol");
                    loc1 = focus.IndexOf(">", loc1 + 1);
                    loc2 = focus.IndexOf("<", loc1 + 1);
                    if (loc1 > 0 && loc2 > loc1)
                    {
                        if (TradingSymbolAlreadySet == false)
                        {
                            ToReturn.TradingSymbol = focus.Substring(loc1 + 1, loc2 - loc1 - 1);
                            ToReturn.TradingSymbol = ToReturn.TradingSymbol.Replace("&#160;", "").Trim(); //I put this in because one of the 10-K's that I found (AMT, 2020 10-K) had this in it.  It is basically some HTML "Space" sign but I need to pull it out
                            TradingSymbolAlreadySet = true;
                        }
                    }
                }
            } while (sr.EndOfStream == false && TradingSymbolAlreadySet == false);
            #endregion

            #region "Get Document Type"

            sr = new StreamReader(s); //Refresh the stream reader
            s.Position = 0;
            do
            {
                string line = sr.ReadLine();

                //Is it the document type
                if (line.Contains("<dei:DocumentType"))
                {
                    string Focus = line;
                    if (Focus.Contains("</dei:DocumentType") == false)
                    {
                        do
                        {
                            string nl = sr.ReadLine();
                            Focus = Focus + " " + nl;
                        } while (Focus.Contains("</dei:DocumentType") == false);
                    }
                    loc1 = Focus.IndexOf(">");
                    loc2 = Focus.IndexOf("<", loc1 + 1);
                    if (loc1 > 0 && loc2 > loc1)
                    {
                        ToReturn.DocumentType = Focus.Substring(loc1 + 1, loc2 - loc1 - 1);
                    }
                }
            } while (sr.EndOfStream == false);

            #endregion

            //This must appear BEOFRE finding the primary context instance
            #region "Get Primary Context Period"

            s.Position = 0;

            do
            {
                string line = sr.ReadLine();

                if (line.Contains("<dei:DocumentFiscalPeriodFocus") || line.Contains("<dei:CurrentFiscalYearEndDate"))
                {
                    string focus = line;
                    do
                    {
                        focus = focus + " " + sr.ReadLine();
                    } while (focus.Contains("contextRef=") == false);
                    loc1 = focus.IndexOf("contextRef=");
                    loc1 = focus.IndexOf("\"", loc1 + 1);
                    loc2 = focus.IndexOf("\"", loc1 + 1);
                    ToReturn.PrimaryPeriodContextId = focus.Substring(loc1 + 1, loc2 - loc1 - 1);
                }


            } while (sr.EndOfStream == false);

            #endregion

            //This can only occur AFTER finding the primary context period.
            #region "Get the Primary Context Instant"
            //Wrap back around and try to find the primary instant ref
            if (ToReturn.PrimaryPeriodContextId != "")
            {
                try
                {
                    XbrlContext PrimaryPeriodContext = ToReturn.GetContextById(ToReturn.PrimaryPeriodContextId);
                    string doc_end_date = PrimaryPeriodContext.EndDate.Month.ToString() + "-" + PrimaryPeriodContext.EndDate.Day.ToString() + "-" + PrimaryPeriodContext.EndDate.Year.ToString();
                    List<XbrlContext> EligibileInstantContexts = new List<XbrlContext>();
                    foreach (XbrlContext con in ToReturn.Contexts)
                    {
                        if (con.TimeType == XbrlTimeType.Instant)
                        {
                            string this_con_date = con.InstantDate.Month.ToString() + "-" + con.InstantDate.Day.ToString() + "-" + con.InstantDate.Year.ToString();
                            if (doc_end_date == this_con_date)
                            {
                                EligibileInstantContexts.Add(con);
                            }
                        }
                    }
                    if (EligibileInstantContexts.Count == 1)
                    {
                        ToReturn.PrimaryInstantContextId = EligibileInstantContexts[0].Id;
                    }
                    else if (EligibileInstantContexts.Count > 1)
                    {
                        XbrlContext WinningContext = null;
                        int WinningCount = -1;
                        foreach (XbrlContext con in EligibileInstantContexts)
                        {
                            int thiscount = con.GetRelevantFacts(ToReturn.Facts).Length;
                            if (thiscount > WinningCount)
                            {
                                WinningCount = thiscount;
                                WinningContext = con;
                            }
                        }
                        ToReturn.PrimaryInstantContextId = WinningContext.Id;
                    }
            
                }
                catch
                {
                    ToReturn.PrimaryInstantContextId = "";
                }
                
            }
            #endregion

            return ToReturn;
        }

        public XbrlContext GetContextById(string id)
        {
            foreach (XbrlContext con in Contexts)
            {
                if (con.Id == id)
                {
                    return con;
                }
            }
            throw new Exception("Unable to find context with ID '" + id + "'.");
        }

        public XbrlFact GetFactById(string id)
        {
            foreach (XbrlFact fact in Facts)
            {
                if (fact.Id == id)
                {
                    return fact;
                }
            }
            throw new Exception("Unable to find fact with ID '" + id + "'.");
        }

        /// <summary>
        /// Finds the primary period context for the specified document type. For example, if the document is a 10-Q, finds the primary ~90 day context.
        /// </summary>
        public XbrlContext FindNormalPeriodPrimaryContext()
        {
            //First, check if the primary period timespan already matches the document period type (i.e. if this document is a 10-Q and indeed the primary period they specified in the document is 90 days, or 3 months)
            bool AlreadyMatches = false;
            XbrlContext pricontext = GetContextById(PrimaryPeriodContextId);
            TimeSpan prispan = pricontext.EndDate - pricontext.StartDate;
            string strippeddoctype = DocumentType.ToLower().Replace("-", "");
            if (strippeddoctype == "10q")
            {
                if (prispan.TotalDays > 80 && prispan.TotalDays < 100) //Roughly 3 months
                {
                    AlreadyMatches = true;
                }
            }
            else if (strippeddoctype == "10k")
            {
                if (prispan.TotalDays > 350 && prispan.TotalDays < 380)
                {
                    AlreadyMatches = true;
                }
            }
            else
            {
                throw new Exception("Unable to recognize document type (not identified as a 10-K or a 10-Q).");
            }
            if (AlreadyMatches)
            {
                return pricontext;
            }
        
            //Now that it does not match, we need to find the most popular one that DOES match.
            ContextUseCount[] usecounts = CountContextUses();
            int lowerbound = 0;
            int upperbound = 0;
            if (strippeddoctype == "10q")
            {
                lowerbound = 80;
                upperbound = 100;
            }
            else if (strippeddoctype == "10k")
            {
                lowerbound = 360;
                upperbound = 380;
            }
            else
            {
                throw new Exception("Unable to recognize document type (not identified as a 10-K or a 10-Q).");
            }

            //Find it
            XbrlContext ToReturn = null;
            foreach (ContextUseCount cuc in usecounts)
            {
                if (ToReturn == null) //Only replace if it is null (this is here to prevent this being replaced later down)
                {
                    TimeSpan thistimespan = cuc.Context.EndDate - cuc.Context.StartDate;
                    if (thistimespan.TotalDays > lowerbound && thistimespan.TotalDays < upperbound)
                    {
                        ToReturn = cuc.Context;
                    }
                }
            }

            //Check for error
            if (ToReturn == null)
            {
                throw new Exception("Unable to find normal period Context appropriate for this document.");
            }

            return ToReturn;
        }

        #region "Utility"

        private ContextUseCount[] CountContextUses()
        {
           List<ContextUseCount> ToReturn = new List<ContextUseCount>();

           //Count them up
           foreach (XbrlContext context in Contexts)
           {
               ContextUseCount cuc = new ContextUseCount();
               cuc.Context = context;
               cuc.Count = 0;

               foreach (XbrlFact fact in Facts)
               {
                   if (fact.ContextId == context.Id)
                   {
                       cuc.Count = cuc.Count + 1;
                   }
               }

               ToReturn.Add(cuc);
           }

           //Arrange from highest to lowest
           List<ContextUseCount> Arranged = new List<ContextUseCount>();
           while (ToReturn.Count > 0)
           {
               ContextUseCount winner = ToReturn[0];
               foreach (ContextUseCount cuc in ToReturn)
               {
                   if (cuc.Count > winner.Count)
                   {
                       winner = cuc;
                   }
               }
               Arranged.Add(winner);
               ToReturn.Remove(winner);
           }
           ToReturn = Arranged;

           return ToReturn.ToArray();
        }
        
        private class ContextUseCount
        {
            public XbrlContext Context {get; set;}
            public int Count {get; set;}
        }


        #endregion
        

    }
}