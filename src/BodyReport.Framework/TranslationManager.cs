using BodyReport.Message;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System;
using System.Reflection;

namespace BodyReport.Framework
{
    /// <summary>
    /// Translation Object
    /// </summary>
    public class TranslationManager
    {
        private static TranslationManager _instance = null;

        public static TranslationManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TranslationManager();
                return _instance;
            }
        }

        public TranslationManager()
        {
        }

        /// <summary>
        /// Load Translation inside Json file
        /// </summary>
        /// <returns></returns>
        public bool LoadTranslation(string filePath, out Dictionary<string, string> translationList)
        {
            bool result = false;
            translationList = null;

            lock (this)
            {
                try
                { //Read Json file
                    CreateFile(filePath);
                    using (var fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                    using (StreamReader sr = new StreamReader(fileStream))
                    using (JsonTextReader reader = new JsonTextReader(sr))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        translationList = serializer.Deserialize<Dictionary<string, string>>(reader);
                        result = true;
                    }
                }
                catch
                {

                }
            }

            return result;
        }

        public void CreateFile(string filePath, bool overwriteFileIfExists = false, Dictionary<string, string> translationList=null)
        {
            lock (this)
            try
            { //Write Json file
                if (!overwriteFileIfExists && File.Exists(filePath))
                    return;

                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    
                using (var fileStream = File.Open(filePath, FileMode.Create, FileAccess.Write))
                using (StreamWriter sr = new StreamWriter(fileStream))
                using (JsonTextWriter jsonWriter = new JsonTextWriter(sr))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Formatting = Formatting.Indented;
                    serializer.Serialize(jsonWriter, translationList);
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// Create or Update JSON translation file
        /// </summary>
        /// <typeparam name="T">Class who contains translation key</typeparam>
        /// <param name="filePath">Translation file path</param>
        /// <param name="isDevelopmentCurrentTranslation">true if it's the current development developer language</param>
        public void CreateOrUpdateTranslationFile<T>(string filePath, bool isDevelopmentCurrentTranslation)
        {
            bool newValueFound = false;
            string trValue;
            Dictionary<string, string> translationList;
            LoadTranslation(filePath, out translationList);
            if (translationList == null)
                translationList = new Dictionary<string, string>();
            
            var fieldInfos = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                if(fieldInfo.FieldType == typeof(string))
                {
                    if (isDevelopmentCurrentTranslation)
                    {
                        trValue = string.Empty;
                        var trAttr = fieldInfo.GetCustomAttribute<TranslationAttribute>();
                        if (trAttr != null)
                            trValue = trAttr.Value;
                        //Put automatic all traduction on default language file
                        if (!translationList.ContainsKey(fieldInfo.Name))
                        {
                            newValueFound = true;
                            translationList.Add(fieldInfo.Name, trValue);
                        }
                        else
                            translationList[fieldInfo.Name] = trValue;
                    }
                    else
                    {  // Put only new Key
                        if (!translationList.ContainsKey(fieldInfo.Name))
                        {
                            newValueFound = true;
                            translationList.Add(fieldInfo.Name, string.Empty);
                        }
                    }
                }
            }

            if (newValueFound)
            {
                CreateFile(filePath, true, translationList);
            }
        }
    }
}
