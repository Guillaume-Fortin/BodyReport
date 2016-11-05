using BodyReport.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.ServiceLayers.Interfaces
{
    public interface ITranslationsService
    {
        List<TranslationVal> FindTranslation();
        TranslationVal UpdateTranslation(TranslationVal translation);
        List<TranslationVal> UpdateTranslationList(List<TranslationVal> translations);
    }
}
