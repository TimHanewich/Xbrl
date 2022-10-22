using System;
using System.IO;
using System.Xml;
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

            XmlDocument doc = new XmlDocument();
            //Handle namespace URI
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            
            //Store expected XBRL xml string and check if stream text is empty and return if null
            string srXml = sr.ReadToEnd();
            if (srXml == "")
                return null;

            doc.LoadXml(sr.ReadToEnd());
            //Gather document namespace elements for resolution in xPath search
            foreach (XmlAttribute a in doc.DocumentElement.Attributes)
            {
                //replace xmlns with xbrl as xmlns is reserved
                if (a.Name == "xmlns")
                    nsmgr.AddNamespace("xbrl", a.Value);
                else
                    nsmgr.AddNamespace(a.Name.Split(':')[1], a.Value);


            }
            XmlNode root = doc.DocumentElement;

            #region "Get Contexts"

            List<XbrlContext> Contexts = new List<XbrlContext>();
            XmlNodeList contexts = root.SelectNodes("descendant::xbrl:context", nsmgr);

            foreach (XmlNode context in contexts)
            {
                XbrlContext xbrlContext = new XbrlContext();
                foreach (XmlNode contextEntityPeriod in context.ChildNodes)
                {
                    if (contextEntityPeriod.Name == "period")
                    {
                        if (contextEntityPeriod.FirstChild.Name == "startDate")
                        {
                            xbrlContext.StartDate = DateTime.Parse(contextEntityPeriod.FirstChild.InnerText);
                            xbrlContext.EndDate = DateTime.Parse(contextEntityPeriod.LastChild.InnerText);
                        }
                        else if (contextEntityPeriod.FirstChild.Name == "instant")
                        {
                            xbrlContext.TimeType = XbrlTimeType.Instant;
                            xbrlContext.InstantDate = DateTime.Parse(contextEntityPeriod.FirstChild.InnerText);
                        }
                    }
                    else if (contextEntityPeriod.Name == "entity")
                    {

                        xbrlContext.Id = context.Attributes[0].Value;
                    }
                }
                Contexts.Add(xbrlContext);
            }


            ToReturn.Contexts = Contexts.ToArray();

            #endregion

            #region "Get Facts"

            List<XbrlFact> Facts = new List<XbrlFact>();
            //Collect all facts provided in document
            foreach (XmlNode node in root.ChildNodes)
            {
                //Context isn't needed again and facts should have at least two attributes for fact ID and context
                if (node.Name != "context" && node.Attributes.Count > 0)
                {
                    XbrlFact fact = new XbrlFact();
                    fact.Value = node.InnerText;
                    fact.NamespaceId = nsmgr.LookupPrefix(node.NamespaceURI);
                    fact.Label = node.Name.Contains(":") ? node.Name.Split(':')[1] : node.Name;
                    //Populate instance document trading symbol and Type
                    if (fact.Label == "TradingSymbol")
                        ToReturn.TradingSymbol = fact.Value;
                    else if (fact.Label == "DocumentType")
                        ToReturn.DocumentType = fact.Value;

                    foreach (XmlAttribute attr in node.Attributes)
                    {
                        //Populate fact attributes
                        if (attr.Name == "contextRef")
                        {
                            fact.ContextId = attr.Value;
                            if (fact.Label == "DocumentFiscalPeriodFocus" || fact.Label == "CurrentFiscalYearEndDate")
                                ToReturn.PrimaryPeriodContextId = fact.ContextId;
                        }
                        //Decimals can be INF sometimes, those are defaulted to null
                        else if (attr.Name == "decimals")
                        {
                            int factDec;
                            int.TryParse(attr.Value, out factDec);
                            if (int.TryParse(attr.Value, out factDec))
                            {
                                fact.Decimals = factDec;
                            }
                        }
                        else if (attr.Name == "id")
                        {
                            fact.Id = attr.Value;
                        }
                        else if (attr.Name == "unitRef")
                        {
                            fact.UnitId = attr.Value;
                        }
                    }
                    if (fact.ContextId != null)
                        Facts.Add(fact);
                }
            }

            ToReturn.Facts = Facts.ToArray();
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
            if (strippeddoctype.StartsWith("10q"))
            {
                if (prispan.TotalDays > 80 && prispan.TotalDays < 100) //Roughly 3 months
                {
                    AlreadyMatches = true;
                }
            }
            else if (strippeddoctype.StartsWith("10k"))
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

            //Count
            ContextUseCount[] usecounts = CountContextUses();

            //Establish period bounds (i.e. quarterly is ~90, annual is ~360)
            int lowerbound = 0;
            int upperbound = 0;
            if (strippeddoctype.StartsWith("10q"))
            {
                lowerbound = 80;
                upperbound = 100;
            }
            else if (strippeddoctype.StartsWith("10k"))
            {
                lowerbound = 350;
                upperbound = 380;
            }
            else
            {
                throw new Exception("Unable to recognize document type (not identified as a 10-K or a 10-Q).");
            }


            //If a DocumentPeriodEndDate property is available for this doc (it should be), try to get
            //The proper period context that (i.e. 90 days, 360 days)
            //ends on that date
            //and is the most popular to meet the above criteria
            if (DocumentPeriodEndDate.HasValue)
            {
                foreach (ContextUseCount cuc in usecounts)
                {
                    if (cuc.Context.EndDate.ToShortDateString() == DocumentPeriodEndDate.Value.ToShortDateString()) //The end date of this period falls on the document's specified end date (what we want)
                    {
                        TimeSpan period_duration = cuc.Context.EndDate - cuc.Context.StartDate;
                        int period_days = period_duration.Days;
                        if (period_days > lowerbound && period_days < upperbound) //If it is in the range that is correct for this period
                        {
                            return cuc.Context;
                        }
                    }
                }
            }

            //Now that it does not match, we need to find the most popular one that DOES match.
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

        public DateTime? DocumentPeriodEndDate
        {
            get
            {
                foreach (XbrlFact fact in Facts)
                {
                    if (fact.NamespaceId.ToLower() == "dei" && fact.Label.ToLower() == "documentperiodenddate")
                    {
                        try
                        {
                            return fact.ValueAsDateTime();
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Failure while returning value: " + ex.Message);
                        }
                    }
                }

                //If it gets this far, it didnt find it, so return null;
                return null;
            }
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
            public XbrlContext Context { get; set; }
            public int Count { get; set; }
        }

        #endregion


    }
}
