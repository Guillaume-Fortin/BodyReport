using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using BodyReport.Framework;
using Microsoft.Extensions.Logging;
using BodyReport.Resources;

namespace BodyReport.Framework
{
    public class StringLocalizer : IStringLocalizer
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = WebAppConfiguration.CreateLogger(typeof(Translation));

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

        /// <summary>
        /// Load resources
        /// </summary>
        private void LoadResources()
        {
            try
            {
                string fileName = string.Format("{0}-{1}.json", _resourceName, _cultureName);
                if (!_translationDictionary.ContainsKey(_resourcePath + fileName))
                {
                    if (_resourcePath != null && _resourcePath.ToLower().StartsWith("resources"))
                    {
                        Dictionary<string, string> translationList;
                        if (TranslationManager.Instance.LoadTranslation(_resourcePath + fileName, out translationList) && translationList != null)
                        {
                            _translationDictionary.Add(_resourcePath + fileName, translationList);
                        }
                    }
                }
            }
            catch(Exception exception)
            {
                _logger.LogError("Load resources error", exception);
            }
        }

        /// <summary>
        /// Get in memory current file or Database translation
        /// </summary>
        /// <param name="isDatabaseTranslation">check for database translation</param>
        /// <returns>translation dictionary</returns>
        private Dictionary<string, string> GetCurrentTranslationList()
        {
            string fileName = string.Format("{0}-{1}.json", _resourceName, _cultureName);
            if (_translationDictionary.ContainsKey(_resourcePath + fileName))
                return _translationDictionary[_resourcePath + fileName];
            else
                return null;
        }

        /// <summary>
        /// Get translation value
        /// </summary>
        /// <param name="name">name</param>
        /// <returns>LocalizedString</returns>
        public LocalizedString this[string name]
        {
            get
            {
                bool found = false;
                string value = null;
                string culture = CultureInfo.CurrentCulture.Name;

                Dictionary<string, string> translationList = GetCurrentTranslationList();
                if (translationList != null)
                {
                    lock (translationList)
                    {
                        if (translationList != null)
                        {
                            if (translationList.ContainsKey(name))
                            {
                                found = true;
                                value = translationList[name];
                            }
                        }
                    }
                }
                if (value == null)
                    value = "[NF]" + name;
                return new LocalizedString(name, value, found);
            }
        }

        /// <summary>
        /// Get translation value
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="arguments">???</param>
        /// <returns>LocalizedString</returns>
        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                return this[name];
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

        /// <summary>
        /// Check if translation exist
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="isDatabaseTranslation">in db translation</param>
        /// <returns></returns>
        public bool IsTranslationInDictionnaryExist(string key)
        {
            Dictionary<string, string> translationList = GetCurrentTranslationList();
            lock (translationList)
            {
                return translationList.ContainsKey(key);
            }
        }

        /// <summary>
        /// Add translation value in dictionary
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <param name="isDatabaseTranslation">in db translation</param>
        public void AddTranslationInDictionnary(string key, string value)
        {
            Dictionary<string, string> translationList = GetCurrentTranslationList();
            lock (translationList)
            {
                if (translationList.ContainsKey(key))
                    _logger.LogError("Programming error, multiple adding same value in dictionry");
                else
                    translationList.Add(key, value);
            }
        }
        
        internal void RemoveTranslationInDictionnary(string key)
        {
            Dictionary<string, string> translationList = GetCurrentTranslationList();
            lock (translationList)
            {
                if (translationList.ContainsKey(key))
                {
                    translationList.Remove(key);
                }
            }
        }
    }
}
