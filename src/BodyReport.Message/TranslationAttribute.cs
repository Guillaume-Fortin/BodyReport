using System;

namespace BodyReport.Message
{
    public class TranslationAttribute : Attribute
    {
        public string Value
        {
            get; set;
        }

        public TranslationAttribute(string value) : base()
        {
            Value = value;
        }
    }
}
