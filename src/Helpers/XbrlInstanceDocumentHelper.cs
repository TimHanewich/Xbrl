using System;
using System.Collections.Generic;

namespace Xbrl.Helpers
{
    public static class XbrlInstanceDocumentHelper
    {
        public static XbrlFact GetValueFromPriorities(this XbrlInstanceDocument doc, params string[] priorities)
        {
            foreach (string word in priorities)
            {
                foreach (XbrlFact fact in doc.Facts)
                {
                    if (fact.ContextId.ToLower().Trim() == doc.PrimaryInstantContextId.ToLower().Trim() || fact.ContextId.Trim().ToLower() == doc.PrimaryPeriodContextId.Trim().ToLower())
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