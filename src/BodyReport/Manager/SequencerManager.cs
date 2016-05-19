using BodyReport.Crud.Module;
using BodyReport.Data;
using BodyReport.Models;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Manager
{
    /// <summary>
    /// Manage All Sequencer
    /// </summary>
    public class SequencerManager
    {
        private static object _locker = new object();

        public int GetNextValue(ApplicationDbContext dbContext, int sequencerId, string sequencerName)
        {
            int result = 0;
            lock (_locker)
            {
                var sequencerModule = new SequencerModule(new ApplicationDbContext());
                var key = new SequencerKey() { Id = sequencerId, Name = sequencerName };
                var sequencer = sequencerModule.Get(key);
                if (sequencer == null)
                {
                    sequencer = new Sequencer() { Id = sequencerId, Name = sequencerName, Value = 1 };
                    sequencer = sequencerModule.Create(sequencer);
                }
                else
                {
                    sequencer.Value++;
                    sequencer = sequencerModule.Update(sequencer);
                }
                result = sequencer.Value;
            }
            return result;
        } 
    }
}
