using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Services
{
    /// <summary>
    /// Helper for generate hml result after call view with view model
    /// </summary>
    public class ViewHelper
    {
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly HttpContext _httpContext;

        public ViewHelper(
            IRazorViewEngine razorViewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider,
            IHttpContextAccessor httpContextAccessor)
        {
            _razorViewEngine = razorViewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
            _httpContext = httpContextAccessor.HttpContext;
    }

        /// <summary>
        /// renderView to html string with viewmodel
        /// </summary>
        /// <param name="viewName">view name</param>
        /// <param name="viewModel">view model</param>
        /// <returns></returns>
        public async Task<string> RenderViewToStringAsync(string viewName, object viewModel)
        {
            var actionContext = new ActionContext(_httpContext, new RouteData(), new ActionDescriptor());
            var viewEngineResult = _razorViewEngine.GetView(viewName, viewName, false);

            if (viewEngineResult.View == null || (!viewEngineResult.Success))
            {
                throw new ArgumentNullException($"Unable to find view '{viewName}'");
            }

            var view = viewEngineResult.View;


            using (var sw = new StringWriter())
            {
                var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
                viewDictionary.Model = viewModel;

                var tempData = new TempDataDictionary(_httpContext, _tempDataProvider);

                var viewContext = new ViewContext(actionContext, view, viewDictionary, tempData, sw, new HtmlHelperOptions());

                viewContext.RouteData = _httpContext.GetRouteData();   //set route data here

                await view.RenderAsync(viewContext);

                return sw.ToString();
            }
        }
    }
}
