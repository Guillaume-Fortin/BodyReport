using BodyReport.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

// Don't work
namespace BodyReport.Framework.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class CustomStringLengthAttribute : StringLengthAttribute
    {
        public string TranslationKey { get; set; }

        public CustomStringLengthAttribute(int maximumLength, string translationKey) : base(maximumLength)
        {
            TranslationKey = translationKey;
            ErrorMessageResourceName = translationKey;
            ErrorMessageResourceType = typeof(Translation);
        }
    }
}
