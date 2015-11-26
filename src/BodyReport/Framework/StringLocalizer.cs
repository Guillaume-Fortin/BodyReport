using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.AspNet.Localization;
using Framework;
using Message;

namespace BodyReport.Framework
{
    public class StringLocalizer : IStringLocalizer
    {
        private static Dictionary<string, Dictionary<string, string>> _translationDictionary = new Dictionary<string, Dictionary<string, string>>();

        private string _cultureName;
        private string _resourcePath;
        private string _resourceName;
        public StringLocalizer(string resourcePath, string resourceName)
        {
            _resourcePath = IoUtils.CompleteDirectoryPath(resourcePath);
            _resourceName = resourceName;
            _cultureName = CultureInfo.CurrentCulture.Name;
            LoadResources();
        }

        private void LoadResources()
        {
            try
            {
                string fileName = string.Format("{0}-{1}.json", _resourceName, _cultureName);
                if (!_translationDictionary.ContainsKey(_resourcePath + fileName))
                {
                    Dictionary<string, string> translationList;
                    if (TranslationManager.Instance.LoadTranslation(_resourcePath + fileName, out translationList) && translationList != null)
                    {
                        _translationDictionary.Add(_resourcePath + fileName, translationList);
                    }
                }
            }
            catch//(Exception exception)
            {
                //TODO LOG
            }
        }

        private Dictionary<string, string> CurrentTranslationList
        {
            get
            {
                string fileName = string.Format("{0}-{1}.json", _resourceName, _cultureName);
                if (_translationDictionary.ContainsKey(_resourcePath + fileName))
                    return _translationDictionary[_resourcePath + fileName];
                else
                    return null;
            }
        }

        public LocalizedString this[string name]
        {
            get
            {
                bool found = false;
                string value = null;
                string culture = CultureInfo.CurrentCulture.Name;

                Dictionary<string, string> translationList = CurrentTranslationList;
                if (translationList != null)
                {
                    KeyValuePair<string, string> trKeyVal = translationList.Where(t => t.Key.ToLower() == name.ToLower()).FirstOrDefault();
                    //if (trKeyVal != null)
                    {
                        found = true;
                        value = trKeyVal.Value;
                    }
                }
                if (value == null)
                    value = "[NF]" + name;
                return new LocalizedString(name, value, found);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                return new LocalizedString(name, name + " Test - " + CultureInfo.CurrentCulture.Name, false);
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeAncestorCultures)
        {
            throw new NotImplementedException();
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
