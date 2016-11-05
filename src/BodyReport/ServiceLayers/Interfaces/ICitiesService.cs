using BodyReport.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.ServiceLayers.Interfaces
{
    public interface ICitiesService
    {
        City GetCity(CityKey key);

        List<City> FindCities(CityCriteria cityCriteria = null);
    }
}
