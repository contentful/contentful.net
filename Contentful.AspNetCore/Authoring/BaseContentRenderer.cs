using Contentful.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Contentful.AspNetCore.Authoring
{
    /// <summary>
    /// Base class used for creating renderers for a specific razor view.
    /// </summary>
    public abstract class RazorContentRenderer : IContentRenderer
    {
        /// <summary>
        /// The razor view engine used to locate the view.
        /// </summary>
        protected readonly IRazorViewEngine _razorViewEngine;
        /// <summary>
        /// The tempdata provider used in the view context of the view.
        /// </summary>
        protected readonly ITempDataProvider _tempDataProvider;
        /// <summary>
        /// The service provider used to create an http context.
        /// </summary>
        protected readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new RazorContentRenderer.
        /// </summary>
        /// <param name="razorViewEngine">The razor view engine used to locate the view.</param>
        /// <param name="tempDataProvider">Tempdata provider used in the view context of the view.</param>
        /// <param name="serviceProvider">The service provider used to create an http context.</param>
        public RazorContentRenderer(IRazorViewEngine razorViewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider)
        {
            _razorViewEngine = razorViewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// The order of this renderer in the rendering collection.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Whether or not this renderer supports a certain IContent.
        /// </summary>
        /// <param name="content">The content to evaluate.</param>
        /// <returns>True if it supports rendering the content, otherwise false.</returns>
        public abstract bool SupportsContent(IContent content);

        /// <summary>
        /// Renders the content to a string representation.
        /// </summary>
        /// <param name="content">The content to render.</param>
        /// <returns>The content as a string representation.</returns>
        public abstract string Render(IContent content);

        /// <summary>
        /// Renders the content to a string representation asynchronously.
        /// </summary>
        /// <param name="content">The content to render.</param>
        /// <returns>The content as a string representation.</returns>
        public abstract Task<string> RenderAsync(IContent content);

        /// <summary>
        /// Renders a view from a model.
        /// </summary>
        /// <param name="viewName">The name of the view to render.</param>
        /// <param name="model">The model to pass to the view.</param>
        /// <returns>The view as a string.</returns>
        public async Task<string> RenderToString(string viewName, object model)
        {
            var httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

            using (var sw = new StringWriter())
            {
                var viewResult = _razorViewEngine.FindView(actionContext, viewName, false);

                if (viewResult.View == null)
                {
                    
                    throw new ArgumentNullException($"The view {viewName} could not be found." +
                                                    $"Searched the following locations: { viewResult.SearchedLocations }");
                }

                var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                };

                var viewContext = new ViewContext(
                    actionContext,
                    viewResult.View,
                    viewDictionary,
                    new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                    sw,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);
                return sw.ToString();
            }
        }

    }
}
