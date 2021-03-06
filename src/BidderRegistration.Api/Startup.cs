using BidderRegistration.Api.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json.Serialization;
using BidderRegistration.Api.Domain.Shared;
using BidderRegistration.Services.Config;
using BidderRegistration.Api.Application.Filters;
using BidderRegistration.Api.Application.Middleware;
using BidderRegistration.Domain.Exceptions;
using BidderRegistration.Api.Controllers.V1;
using BidderRegistration.Domain;
using BidderRegistration.Api.Validators;
using BidderRegistration.CorrelationId;
using RequestResponseLogger.Configuration;


namespace BidderRegistration.Api
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        protected bool UseAppConfiguration { get; set; } = true;
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        private IConfiguration Configuration { get; }

        private IWebHostEnvironment Environment { get; }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.MapConfigurationToTypedClass(Configuration);
            services.AddExternalServices(Configuration);
            services.AddInternalServices(Configuration, Environment);
            services.AddAzureServices(Configuration);
            services.AddApiVersioning(1);

            services.AddControllers().AddJsonOptions(options =>
                 options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            services.AddHeaderPropagation();
            services.AddSwaggerGen(swaggerOptions =>
            {

                swaggerOptions.CustomSchemaIds(x => x.FullName);
                swaggerOptions.SwaggerDoc("v1", new OpenApiInfo { Title = "Bidder Registration", Version = "v1" });
                swaggerOptions.OperationFilter<AddRequiredHeaderParameter>();
            });

            var correlationIdConfiguration = new CorrelationIdConfiguration("BidderRegistration", "INLB");
            services.AddCorrelationIdServices(correlationIdConfiguration);

            var loggingConfiguration = new LoggingConfiguration(applicationName: "BidderRegistration", applicationVersion: "1.0", isLoggingOn: true, excludeLogRoutes: new string[] { });
            services.AddLoggingProvider(loggingConfiguration);

            services.AddPipelineBehaviors();
            services.AddGzipCompression();

            var cosmosConfiguration = new CosmosDbSettings();
            Configuration.Bind("CosmosDbSettings", cosmosConfiguration);
            services.AddSingleton(cosmosConfiguration);

            services.Configure<ApiBehaviorOptions>(o =>
            {
                o.InvalidModelStateResponseFactory = actionContext =>
                    ProcessModelStateErrors(actionContext);
            });

            var regionSettings = new RegionSettings
            {
                ApplicationRegion = Configuration["ApplicationRegion"]
            };
            services.AddSingleton(regionSettings);

            //services.AddSingleton(new BiddingClient(Environment));

            services.AddSingleton<IBidderRepository, BidderRepository>();

        }

        private IActionResult ProcessModelStateErrors(ActionContext actionContext)
        {
            var response = new ModelBindingValidationError
            {
                ValidationResults = new List<ValidationError>()
            };
            foreach (var key in actionContext.ModelState.Keys)
            {
                if (!actionContext.ModelState[key].Errors.Any()) continue;
                var errorMessages = actionContext.ModelState[key].Errors.Select(x => x.ErrorMessage);
                var description = string.Join(".", errorMessages);
                response.ValidationResults.Add(new ValidationError((int)ErrorCode.ERROR_MISSING_DATA, ErrorCode.ERROR_MISSING_DATA.ToString() , description, key));
            }

            return new BadRequestObjectResult(response);
        }

        public virtual void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<GzipRequestMiddleware>();
            // Enable response compression (This must be called before any middleware that compresses responses).
            app.UseResponseCompression();
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseRouting();
            app.RegisterSwagger();
            app.RegisterMiddleware();
            if (UseAppConfiguration)
                app.UseAzureAppConfiguration();

            app.UseHttpsRedirection();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

        }

        /// <summary>
        /// Swagger filter that enables the required parameter option on headers.
        /// These headers are specified in the configuration.
        /// </summary>
        private class AddRequiredHeaderParameter : IOperationFilter
        {
            private readonly IOptions<Headers> _options;

            public AddRequiredHeaderParameter(IOptions<Headers> options)
            {
                _options = options;
            }

            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                operation.Parameters ??= new List<OpenApiParameter>();

                foreach (var key in _options.Value.Request)
                {
                    operation.Parameters.Add(new OpenApiParameter
                    {
                        Name = key,
                        In = ParameterLocation.Header,
                        Schema = new OpenApiSchema { Type = "string" },
                        Required = true,
                        Example = new OpenApiString("1")
                    });
                }
            }
        }
    }
}