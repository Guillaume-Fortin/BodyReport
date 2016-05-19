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
    public class TranslationManager : ServiceManager
    {
        TranslationModule _module = null;

        public TranslationManager(ApplicationDbContext dbContext) : base(dbContext)
        {
            _module = new TranslationModule(_dbContext);
        }

        internal TranslationVal GetTranslation(TranslationValKey key)
        {
            return _module.Get(key);
        }

        public List<TranslationVal> FindTranslation()
        {
            return _module.Find();
        }

        internal TranslationVal UpdateTranslation(TranslationVal translation)
        {
            return _module.Update(translation);
        }
    }
}
