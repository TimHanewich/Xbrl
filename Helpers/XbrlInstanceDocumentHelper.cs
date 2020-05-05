using System;
using System.Collections.Generic;

namespace Xbrl.Helpers
{
    public static class XbrlInstanceDocumentHelper
    {
        public static XbrlFact GetValueFromPriorities(this XbrlInstanceDocument doc, string priorities)
            {
                List<string> Splitter = new List<string>();
                Splitter.Add(",");
                string[] prioritiy_words = priorities.Split(Splitter.ToArray(), StringSplitOptions.None);

                foreach (string word in prioritiy_words)
                {
                    foreach (XbrlFact fact in doc.Facts)
                    {
                        if (fact.ContextId == doc.PrimaryInstantContextId || fact.ContextId == doc.PrimaryPeriodContextId)
                        {
                            if (fact.Label.ToLower() == word.ToLower())
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