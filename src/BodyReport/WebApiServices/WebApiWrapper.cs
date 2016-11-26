using BodyReport.Framework;
using BodyReport.Message;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace BodyReport.WebApiServices
{
   

    public class WebApiWrapper
    {
        //private IServiceProvider _serviceProvider = null;
        private HttpClientPoolManager _httpClientPoolManager = null;
        public WebApiWrapper(Uri webApiBaseAdress, int webServicesMaxPoolSize)
        {
            _httpClientPoolManager = new HttpClientPoolManager(webApiBaseAdress, webServicesMaxPoolSize);
        }

        public HttpClientPoolManager HttpClientPoolManager
        {
            get
            {
                return _httpClientPoolManager;
            }
        }
    }

    /// <summary>
    /// Web service Muscles
    /// </summary>
    public static class MusclesWS
    {
        private const string _baseUrl = "Api/Muscles/";
        public static async Task<List<Muscle>> FindAsync(WebApiWrapper webApiWrapper, string userId, Cookie cookie)
        {
            return await webApiWrapper.HttpClientPoolManager.GetAsync<List<Muscle>>(userId, cookie, _baseUrl + "Find");
        }
    }

    /// <summary>
    /// Web service BodyExercises
    /// </summary>
    public static class BodyExercisesWS
    {
        private const string _baseUrl = "Api/BodyExercises/";
        public static async Task<BodyExercise> GetAsync(WebApiWrapper webApiWrapper, string userId, Cookie cookie, BodyExerciseKey key)
        {
            Dictionary<string, string> datas = new Dictionary<string, string>();
            datas.Add("Id", key.Id.ToString());
            return await webApiWrapper.HttpClientPoolManager.GetAsync<BodyExercise>(userId, cookie, _baseUrl + "Get", datas);
        }
        public static async Task<BodyExercise> CreateAsync(WebApiWrapper webApiWrapper, string userId, Cookie cookie, BodyExercise bodyExercice)
        {
            return await webApiWrapper.HttpClientPoolManager.PostAsync<BodyExercise, BodyExercise>(userId, cookie, _baseUrl + "Create", bodyExercice);
        }
        public static async Task<List<BodyExercise>> FindAsync(WebApiWrapper webApiWrapper, string userId, Cookie cookie)
        {
            return await webApiWrapper.HttpClientPoolManager.GetAsync<List<BodyExercise>>(userId, cookie, _baseUrl + "Find");
        }
        public static async Task<BodyExercise> UpdateAsync(WebApiWrapper webApiWrapper, string userId, Cookie cookie, BodyExercise bodyExercice)
        {
            return await webApiWrapper.HttpClientPoolManager.PostAsync<BodyExercise, BodyExercise>(userId, cookie, _baseUrl + "Update", bodyExercice);
        }
        public static async Task<List<BodyExercise>> UpdateListAsync(WebApiWrapper webApiWrapper, string userId, Cookie cookie, List<BodyExercise> bodyExercices)
        {
            return await webApiWrapper.HttpClientPoolManager.PostAsync<List<BodyExercise>, List<BodyExercise>>(userId, cookie, _baseUrl + "UpdateList", bodyExercices);
        }
        public static async Task<bool> DeleteAsync(WebApiWrapper webApiWrapper, string userId, Cookie cookie, BodyExerciseKey key)
        {
            return await webApiWrapper.HttpClientPoolManager.PostAsync<BodyExerciseKey, bool>(userId, cookie, _baseUrl + "Delete", key);
        }
    }
}
