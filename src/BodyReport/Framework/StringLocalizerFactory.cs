using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Framework
{
    public class StringLocalizerFactory : IStringLocalizerFactory
    {
        public IStringLocalizer Create(Type resourceSource)
        {
            string[] namespaces = resourceSource.FullName.Split('.');
            string resourcePath = string.Empty;
            if(namespaces != null && namespaces.Length>2)
            {
                if (namespaces[namespaces.Length - 2] == "Resources")
                {
                    for (int i = 1; i < namespaces.Length; i++)
                    {
                        if (i == namespaces.Length - 1)
                            break;
                        resourcePath += namespaces[i] + '/';
                    }
                    string resourceName = resourceSource.Name;
                    return new StringLocalizer(resourcePath, resourceName);
                }
            }
            return null;
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return new StringLocalizer(location, baseName);
        }
    }
}
