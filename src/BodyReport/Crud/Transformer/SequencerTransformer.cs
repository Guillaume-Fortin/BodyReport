using BodyReport.Models;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Crud.Transformer
{
    public class SequencerTransformer
    {
        public static void ToRow(Sequencer bean, SequencerRow row)
        {
            if (bean == null)
                return;

            row.Id = bean.Id;
            row.Name = bean.Name;
            row.Value = bean.Value;
        }

        internal static Sequencer ToBean(SequencerRow row)
        {
            if (row == null)
                return null;

            var bean = new Sequencer();
            bean.Id = row.Id;
            bean.Name = row.Name;
            bean.Value = row.Value;
            return bean;
        }
    }
}
