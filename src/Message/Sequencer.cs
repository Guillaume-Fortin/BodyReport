using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Message
{
    public class SequencerKey
    {
        /// <summary>
        /// Sequencer Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Sequencer Name
        /// </summary>
        public string Name { get; set; }
    }

    public class Sequencer : SequencerKey
    {
        /// <summary>
        /// Sequencer value
        /// </summary>
        public int Value { get; set; }
    }
}
