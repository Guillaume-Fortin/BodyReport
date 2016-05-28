using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Extensions.FileProviders;
using Message;

namespace Framework
{
    public static class TimeZoneMapper
    {
        private static List<TimeZoneMap> _timeZoneNames = new List<TimeZoneMap>();

        static TimeZoneMapper()
        {
            Init();
        }

        public static List<string> WindowsTimeZoneNames
        {
            get
            {
                return _timeZoneNames.Select(t => t.WindowsName).OrderBy(t => t).ToList();
            }
        }

        public static List<string> OlsonTimeZoneNames
        {
            get
            {
                return _timeZoneNames.Select(t => t.OlsonName).OrderBy( t => t).ToList();
            }
        }

        public static string GetWindowsTimeZoneName(string olsonTimeZoneName)
        {
            if (String.IsNullOrWhiteSpace(olsonTimeZoneName))
                return null;

            var timezoneMap = _timeZoneNames.Where(t => t.OlsonName.ToLowerInvariant() == olsonTimeZoneName.ToLowerInvariant()).FirstOrDefault();
            return timezoneMap == null ? null : timezoneMap.WindowsName;
        }

        public static TimeZoneInfo GetTimeZoneByOlsonName(string olsonTimeZoneName)
        {
            string windowsTimeZoneName = GetWindowsTimeZoneName(olsonTimeZoneName);
            if (string.IsNullOrWhiteSpace(windowsTimeZoneName))
                return null;

            return TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneName);
        }

        public static string GetOlsonTimeZoneName(string windowsTimeZoneName)
        {
            if (String.IsNullOrWhiteSpace(windowsTimeZoneName))
                return null;

            var timezoneMap = _timeZoneNames.Where(t => t.WindowsName.ToLowerInvariant() == windowsTimeZoneName.ToLowerInvariant()).FirstOrDefault();
            return timezoneMap == null ? null : timezoneMap.OlsonName;
        }

        private static void Init()
        {
            _timeZoneNames.Clear();

            var assembly = typeof(TimeZoneMapper).GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream("Framework.windowsZones.xml");
            if(stream != null)
            {
                XAttribute attrWin, attrOlson;
                XDocument xDoc = XDocument.Load(stream);
                var rootTimeZoneElement = xDoc.Root.Element("windowsZones").Element("mapTimezones");
                if(rootTimeZoneElement != null)
                {
                    foreach(var mapZoneElement in rootTimeZoneElement.Elements("mapZone"))
                    {
                        attrWin = mapZoneElement.Attribute("other");
                        attrOlson = mapZoneElement.Attribute("type");
                        if (attrWin != null && attrOlson != null &&
                           !string.IsNullOrWhiteSpace(attrWin.Value) && !string.IsNullOrWhiteSpace(attrOlson.Value))
                        {
                            _timeZoneNames.Add(new TimeZoneMap() { WindowsName = attrWin.Value, OlsonName = attrOlson.Value });
                        }
                    }
                    _timeZoneNames = _timeZoneNames.OrderBy(t => t.OlsonName).ToList();
                }
            }
        }
    }
}
