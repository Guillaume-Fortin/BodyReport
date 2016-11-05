using BodyReport.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.ServiceLayers.Interfaces
{
    public interface ICountriesService
    {
        Country GetCountry(CountryKey key);
        List<Country> FindCountries(CountryCriteria countryCriteria = null);
    }
}
