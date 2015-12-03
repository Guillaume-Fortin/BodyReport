using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Models
{
    public class SequencerRow
    {
        /// <summary>
        /// Sequencer Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Sequencer Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Sequencer value
        /// </summary>
        public int Value { get; set; }
    }
}
