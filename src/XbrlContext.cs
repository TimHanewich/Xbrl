using System;
using System.Collections.Generic;

namespace Xbrl
{
public class XbrlContext
    {
        public string Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime InstantDate { get; set; }
        public XbrlTimeType TimeType { get; set; }

        /// <summary>
        /// Gets a list of facts that use this context.
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public XbrlFact[] GetRelevantFacts(XbrlFact[] search)
        {
            List<XbrlFact> ToReturn = new List<XbrlFact>();
            foreach (XbrlFact f in search)
            {
                if (f.ContextId == Id)
                {
                    ToReturn.Add(f);
                }
            }
            return ToReturn.ToArray();
        }
    }
}
