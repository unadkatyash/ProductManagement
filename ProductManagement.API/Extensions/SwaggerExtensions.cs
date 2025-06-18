//using Microsoft.OpenApi.Models;
//using Swashbuckle.AspNetCore.SwaggerGen;
//using System.Reflection;

//namespace ProductManagement.API.Extensions
//{
//    public static class SwaggerExtensions
//    {
//        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
//        {
//            services.AddEndpointsApiExplorer();
//            services.AddSwaggerGen(c =>
//            {
//                c.SwaggerDoc("v1", new OpenApiInfo
//                {
//                    Title = "Real-Time Products API",
//                    Version = "v1",
//                    Description = @"
//                        A comprehensive real-time API for managing products with the following features:
                        
//                        ## Features
//                        - **CRUD Operations**: Create, Read, Update, Delete products
//                        - **Real-time Updates**: SignalR integration for live data synchronization  
//                        - **Entity Framework**: SQL Server integration with automatic migrations
//                        - **Input Validation**: Comprehensive data validation with error responses
//                        - **RESTful Design**: Following REST API best practices
                        
//                        ## SignalR Integration
//                        Connect to `/productHub` to receive real-time notifications:
//                        - `ProductAdded`: Fired when a new product is created
//                        - `ProductUpdated`: Fired when a product is modified  
//                        - `ProductDeleted`: Fired when a product is removed
                        
//                        ## Getting Started
//                        1. Use GET `/api/products` to retrieve all products
//                        2. Connect to SignalR hub at `/productHub` for live updates
//                        3. Join the 'ProductUpdates' group to receive notifications
//                        4. Use POST, PUT, DELETE endpoints to modify data
//                    ",
//                    Contact = new OpenApiContact
//                    {
//                        Name = "API Support Team",
//                        Email = "api-support@example.com",
//                        Url = new Uri("https://example.com/support")
//                    },
//                    License = new OpenApiLicense
//                    {
//                        Name = "MIT License",
//                        Url = new Uri("https://opensource.org/licenses/MIT")
//                    }
//                });

//                // Add XML comments for better documentation
//                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
//                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
//                if (File.Exists(xmlPath))
//                {
//                    c.IncludeXmlComments(xmlPath);
//                }

//                // Add security definitions if needed in future
//                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//                {
//                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
//                    Name = "Authorization",
//                    In = ParameterLocation.Header,
//                    Type = SecuritySchemeType.ApiKey,
//                    Scheme = "Bearer"
//                });

//                // Group endpoints by tags
//                c.TagActionsBy(api => new[] { api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] });
//                c.DocInclusionPredicate((name, api) => true);

//                // Add response examples
//                //c.EnableAnnotations();

//                // Custom operation filters
//                c.OperationFilter<SwaggerDefaultValues>();
//            });

//            return services;
//        }

//        public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app, IWebHostEnvironment env)
//        {
//            if (env.IsDevelopment())
//            {
//                app.UseSwagger(c =>
//                {
//                    c.RouteTemplate = "swagger/{documentName}/swagger.json";
//                });

//                app.UseSwaggerUI(c =>
//                {
//                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Real-Time Products API v1");
//                    c.RoutePrefix = "swagger";
//                    c.DocumentTitle = "Real-Time Products API - Documentation";

//                    // UI customization
//                    c.DisplayRequestDuration();
//                    c.EnableDeepLinking();
//                    c.EnableFilter();
//                    c.ShowExtensions();
//                    c.EnableValidator();
//                    c.SupportedSubmitMethods(Swashbuckle.AspNetCore.SwaggerUI.SubmitMethod.Get,
//                                           Swashbuckle.AspNetCore.SwaggerUI.SubmitMethod.Post,
//                                           Swashbuckle.AspNetCore.SwaggerUI.SubmitMethod.Put,
//                                           Swashbuckle.AspNetCore.SwaggerUI.SubmitMethod.Delete);

//                    // Custom CSS for better appearance
//                    c.InjectStylesheet("/swagger-ui/custom.css");

//                    // Add custom JavaScript
//                    c.InjectJavascript("/swagger-ui/custom.js");
//                });
//            }

//            return app;
//        }
//    }

//    // Custom operation filter for default values
//    //public class SwaggerDefaultValues : IOperationFilter
//    //{
//    //    public void Apply(OpenApiOperation operation, OperationFilterContext context)
//    //    {
//    //        var apiDescription = context.ApiDescription;

//    //        operation.Deprecated |= apiDescription.IsDeprecated();

//    //        foreach (var responseType in context.ApiDescription.SupportedResponseTypes)
//    //        {
//    //            var responseKey = responseType.IsDefaultResponse ? "default" : responseType.StatusCode.ToString();
//    //            var response = operation.Responses[responseKey];

//    //            foreach (var contentType in response.Content.Keys)
//    //            {
//    //                if (responseType.ModelMetadata?.ModelType != null)
//    //                {
//    //                    var schema = context.SchemaGenerator.GenerateSchema(responseType.ModelMetadata.ModelType, context.SchemaRepository);
//    //                    response.Content[contentType].Schema = schema;
//    //                }
//    //            }
//    //        }
//    //    }
//    //}
//}
