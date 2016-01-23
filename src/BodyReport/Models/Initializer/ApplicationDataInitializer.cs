using BodyReport.Framework;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyReport.Models.Initializer
{
    /// <summary>
    /// Source code view first time on http://wildermuth.com/2015/3/17/A_Look_at_ASP_NET_5_Part_3_-_EF7
    /// </summary>
    public class ApplicationDataInitializer
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static ILogger _logger = WebAppConfiguration.CreateLogger(typeof(ApplicationDataInitializer));
        private ApplicationDbContext _context;
        private IHostingEnvironment _env;

        public ApplicationDataInitializer(ApplicationDbContext context, IHostingEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public void InitializeData()
        {
            CreateCountries();
            CreateCities();
        }

        /*
        /// <summary>
        /// Poutpulate data with http://www.geonames.org/export/ open datas
        /// </summary>
        private void CreateCities()
        {
            try
            {
                int rowCount = 0;
                Dictionary<string, int> duplicateCityZipCodes = new Dictionary<string, int>();
                string line;
                string[] splitLine;
                string dataFilePath = Path.Combine(_env.WebRootPath, "databasedata", "frenchCities.csv");
                if (_context.Cities.Count() == 0 && File.Exists(dataFilePath))
                {
                    _logger.LogInformation("Begin of Populate cities in database");

                    using (var fileStream = File.OpenRead(dataFilePath))
                    {
                        using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                        {
                            while ((line = streamReader.ReadLine()) != null)
                            {
                                if (line.StartsWith("#"))
                                    continue;
                                
                                splitLine = line.Split(';');
                                if (splitLine != null && splitLine.Length > 2)
                                {
                                    var city = new CityRow();
                                    city.Name = splitLine[3];
                                    city.ZipCode = splitLine[2];
                                    if (!string.IsNullOrWhiteSpace(city.Name) && !string.IsNullOrWhiteSpace(city.ZipCode))
                                    { //Manage duplicate name with different zip code
                                        if(duplicateCityZipCodes.ContainsKey(city.ZipCode))
                                        {
                                            city.Id = duplicateCityZipCodes[city.ZipCode] + 1;
                                            duplicateCityZipCodes[city.ZipCode] = city.Id;
                                        }
                                        else
                                        {
                                            city.Id = 0;
                                            duplicateCityZipCodes.Add(city.ZipCode, city.Id);
                                        }

                                        _context.Cities.Add(city);
                                        rowCount++;
                                        if (rowCount % 5000 == 0) //Commit all 500 row
                                            _context.SaveChanges();
                                    }
                                }
                            }
                            //Security save change
                            _context.SaveChanges();
                        }
                    }
                    _logger.LogInformation(string.Format("End of Populate cities in database : number row={0}", rowCount));
                }
            }
            catch(Exception exception)
            {
                _logger.LogCritical("Unable populate cities in database", exception);
            }
        }
        */

        private void CreateCountries()
        {
            string rootPath = Path.Combine(_env.WebRootPath, "databasedata");
            if (!Directory.Exists(rootPath))
                return;

            try
            {
                int countryId;
                string[] splitLine;
                string[] countryLines = File.ReadAllLines(Path.Combine(rootPath, "countries.txt"));
                foreach (string countryLine in countryLines)
                {
                    splitLine = countryLine.Split(';');
                    countryId = int.Parse(splitLine[0]);
                    var countryRow = _context.Countries.Where(c => c.Id == countryId).FirstOrDefault();
                    if (countryRow == null)
                    {
                        countryRow = new CountryRow()
                        {
                            Id = countryId,
                            ShortName = splitLine[1],
                            Name = splitLine[2]
                        };
                        _context.Countries.Add(countryRow);
                    }
                }
                _context.SaveChanges();
            }
            catch (Exception exception)
            {
                _logger.LogCritical("Unable populate countries in database", exception);
            }
        }

        /// <summary>
        /// Poutpulate data with http://www.geonames.org/export/ open datas
        /// </summary>
        private void CreateCities()
        {
            int rowCount;
            CountryRow country;
            Dictionary<string, int> duplicateCityZipCodes = new Dictionary<string, int>();
            string line, cityShortName;
            string[] splitLine;

            try
            {
                string rootPath = Path.Combine(_env.WebRootPath, "databasedata", "zipcode");
                if (!Directory.Exists(rootPath))
                    return;

                string[] cityFiles = Directory.GetFiles(rootPath, "*.txt");
                foreach (string dataFilePath in cityFiles)
                {
                    rowCount = 0;
                    duplicateCityZipCodes.Clear();
                    cityShortName = Path.GetFileNameWithoutExtension(dataFilePath);
                    country = _context.Countries.Where(c => c.ShortName == cityShortName).FirstOrDefault();
                    if (country == null)
                    {
                        _logger.LogInformation(string.Format("country not found : {0}", cityShortName));
                        continue;
                    }
                    if (_context.Cities.Where(c => c.CountryId == country.Id).Count() == 0)
                    {
                        _logger.LogInformation(string.Format("Begin of Populate {0} cities in database", cityShortName));
                        using (var fileStream = File.OpenRead(dataFilePath))
                        {
                            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                            {
                                var cities = new List<CityRow>();
                                while ((line = streamReader.ReadLine()) != null)
                                {
                                    if (line.StartsWith("#"))
                                        continue;

                                    splitLine = line.Split('\t');
                                    if (splitLine != null && splitLine.Length > 2)
                                    {
                                        var city = new CityRow();
                                        city.CountryId = country.Id;
                                        city.Name = splitLine[2];
                                        city.ZipCode = splitLine[1];
                                        if (!string.IsNullOrWhiteSpace(city.Name) && !string.IsNullOrWhiteSpace(city.ZipCode) &&
                                            !city.ZipCode.Contains("CEDEX"))
                                        { //Manage duplicate name with different zip code
                                            if (duplicateCityZipCodes.ContainsKey(city.ZipCode))
                                            {
                                                city.Id = duplicateCityZipCodes[city.ZipCode] + 1;
                                                duplicateCityZipCodes[city.ZipCode] = city.Id;
                                            }
                                            else
                                            {
                                                city.Id = 0;
                                                duplicateCityZipCodes.Add(city.ZipCode, city.Id);
                                            }

                                            cities.Add(city);
                                            
                                            rowCount++;
                                            if (rowCount % 5000 == 0) //Commit all 500 row
                                            {
                                                _context.Cities.AddRange(cities);
                                                _context.SaveChanges();
                                                cities.Clear();
                                            }   
                                        }
                                    }
                                }
                                //Security save change
                                if (cities.Count > 0)
                                {
                                    _context.Cities.AddRange(cities);
                                    _context.SaveChanges();
                                    cities.Clear();
                                }
                            }
                        }
                        _logger.LogInformation(string.Format("End of Populate {0} cities in database: number row={1}", cityShortName, rowCount));
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.LogCritical("Unable populate cities in database", exception);
            }
        }
    }
}
