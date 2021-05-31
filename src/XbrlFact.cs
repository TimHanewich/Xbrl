using System;

namespace Xbrl
{
    public class XbrlFact
    {
        public string NamespaceId { get; set; }
        public string Id { get; set; }
        public string Label { get; set; }
        public string ContextId { get; set; }
        public string UnitId { get; set; }
        public int Decimals { get; set; }
        public string Value { get; set; }

        public float ValueAsFloat()
        {
            try
            {
                return Convert.ToSingle(Value);
            }
            catch
            {
                throw new Exception("Unable to convert value '" + Value + "' to a float.");
            }
        }
    
        public DateTime ValueAsDateTime()
        {
            try
            {
                return DateTime.Parse(Value);
            }
            catch
            {
                throw new Exception("Unable to convert value '" + Value + "' into a DateTime.");
            }
        }
    }
}