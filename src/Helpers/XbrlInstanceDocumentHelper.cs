using System;
using System.Collections.Generic;

namespace Xbrl.Helpers
{
    public static class XbrlInstanceDocumentHelper
    {
        public static XbrlFact GetValueFromPriorities(this XbrlInstanceDocument doc, params string[] priorities)
        {
            //First get the normal period context
            string normal_period_context = doc.FindNormalPeriodPrimaryContext().Id;

            foreach (string word in priorities)
            {
                foreach (XbrlFact fact in doc.Facts)
                {
                    if (fact.ContextId.ToLower().Trim() == doc.PrimaryInstantContextId.ToLower().Trim() || fact.ContextId.Trim().ToLower() == normal_period_context.Trim().ToLower())
                    {
                        if (fact.Label.Trim().ToLower() == word.Trim().ToLower())
                        {
                            return fact;
                        }
                    }
                }
            }

            throw new Exception("Unable to find XBRL fact for value labeled with any of the following: " + priorities);
        }
    }
}