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
        public float Value { get; set; }
    }
}