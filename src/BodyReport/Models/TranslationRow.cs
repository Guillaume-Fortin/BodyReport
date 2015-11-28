using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Models
{
    /// <summary>
    /// Database table Translation
    /// </summary>
    public class TranslationRow
    {
        /// <summary>
        /// Regionn Culture id
        /// </summary>
        public int CultureId
        {
            get;
            set;
        }
        
        /// <summary>
        /// Translation key
        /// </summary>
        public string Key
        {
            get;
            set;
        }
        
        /// <summary>
        /// Translation value
        /// </summary>
        public string Value
        {
            get;
            set;
        }
    }
}
