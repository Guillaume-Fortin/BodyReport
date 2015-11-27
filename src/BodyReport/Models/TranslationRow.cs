using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Models
{
    [Table("Translation")]
    public class TranslationRow
    {
        /*[Key, DatabaseGenerated(DatabaseGeneratedOption.None), Column(Order = 1)]
        [MaxLength(8)]*/
        public string Culture
        {
            get;
            set;
        }

        /*[Key, DatabaseGenerated(DatabaseGeneratedOption.None), Column(Order = 2)]
        [MaxLength(256)]*/
        public string Key
        {
            get;
            set;
        }

        /*[MaxLength(2000)]*/
        public string Value
        {
            get;
            set;
        }
    }
}
