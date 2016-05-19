using BodyReport.Crud.Transformer;
using BodyReport.Data;
using BodyReport.Models;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace BodyReport.Crud.Module
{
    public class SequencerModule : Crud
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbContext">database context</param>
        public SequencerModule(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// Create data in database
        /// </summary>
        /// <param name="sequencer">Data</param>
        /// <returns>insert data</returns>
        public Sequencer Create(Sequencer sequencer)
        {
            if (sequencer == null)
                return null;
            
            var sequencerRow = new SequencerRow();
            SequencerTransformer.ToRow(sequencer, sequencerRow);
            _dbContext.Sequencer.Add(sequencerRow);
            _dbContext.SaveChanges();
            return SequencerTransformer.ToBean(sequencerRow);
        }

        /// <summary>
        /// Get data in database
        /// </summary>
        /// <param name="key">Primary Key</param>
        /// <returns>read data</returns>
        public Sequencer Get(SequencerKey key)
        {
            if (key == null || key.Id == 0)
                return null;

            var sequencerRow = _dbContext.Sequencer.Where(m => m.Id == key.Id && m.Name == key.Name).FirstOrDefault();
            if (sequencerRow != null)
            {
                return SequencerTransformer.ToBean(sequencerRow);
            }
            return null;
        }

        /// <summary>
        /// Update data in database
        /// </summary>
        /// <param name="sequencer">data</param>
        /// <returns>updated data</returns>
        public Sequencer Update(Sequencer sequencer)
        {
            if (sequencer == null || sequencer.Id == 0)
                return null;

            var sequencerRow = _dbContext.Sequencer.Where(m => m.Id == sequencer.Id && m.Name == sequencer.Name).FirstOrDefault();
            if (sequencerRow == null)
            { // No data in database
                return Create(sequencer);
            }
            else
            { //Modify Data in database
                SequencerTransformer.ToRow(sequencer, sequencerRow);
                _dbContext.SaveChanges();
                return SequencerTransformer.ToBean(sequencerRow);
            }
        }

        /// <summary>
        /// Delete data in database
        /// </summary>
        /// <param name="key">Primary Key</param>
        public void Delete(SequencerKey key)
        {
            if (key == null || key.Id == 0)
                return;

            var sequencerRow = _dbContext.Sequencer.Where(m => m.Id == key.Id && m.Name == key.Name).FirstOrDefault();
            if (sequencerRow != null)
            {
                _dbContext.Sequencer.Remove(sequencerRow);
                _dbContext.SaveChanges();
            }
        }
    }
}
