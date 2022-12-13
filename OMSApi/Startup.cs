using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OMSServices;
using OMSServices.Hubs;
using System;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.SignalR;
using OMSServices.Utils;
using OMSApi.Converters;
using OMSServices.Implementation;
using IdentityModel.AspNetCore.OAuth2Introspection;
using Microsoft.AspNetCore.Http;
using LS.WebSocketServer;
using OMSApi.Middlewares;
using OMSApi.EventListeners;
using OMSApi.Configurations;

namespace OMSApi
{
    public class Startup
    {
        private readonly IConfiguration Configuration;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ApiConfiguration.PreStartup(Configuration);

            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.Converters.Add(new JsonDecimalConverter());
                    options.JsonSerializerOptions.Converters.Add(new JsonLongConverter());
                    options.JsonSerializerOptions.Converters.Add(new JsonDateTimeConverter());
                });

            services.AddApiVersioning(x =>
            {
                x.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
                x.AssumeDefaultVersionWhenUnspecified = true;
                x.ReportApiVersions = true;
            });

            services.AddServices();
            services.AddMemoryCache();

            // configure redis
            string redisConnectionString = Configuration["Redis:ConnectionString"];
            services.AddStackExchangeRedisCache(x =>
            {
                x.Configuration = redisConnectionString;
            });


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "1.0",
                    Title = "API",
                    Description = "",
                    TermsOfService = new Uri("https://logicielservice.com"),
                    License = new OpenApiLicense
                    {
                        Name = "Use under logiciel service licence",
                        Url = new Uri("https://logicielservice.com"),
                    }
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer exyjkl...')",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(x =>
                {
                    var authority = Configuration["Identity:Authority"];
                    x.Authority = authority;
                    x.RequireHttpsMetadata = authority.StartsWith("https");
                    x.ApiName = Configuration["Identity:ApiName"];
                    x.JwtValidationClockSkew = TimeSpan.Zero;
                    x.TokenRetriever = new Func<HttpRequest, string>(req =>
                    {
                        var fromHeader = TokenRetrieval.FromAuthorizationHeader();
                        var fromQuery = TokenRetrieval.FromQueryString();
                        return fromHeader(req) ?? fromQuery(req);
                    });
                });

            services.AddAuthorization(x =>
            {
                x.AddPolicy(AuthorizationPolicies.DirectUiExternal, x =>
                {
                    x.RequireAuthenticatedUser();
                    x.RequireClaim("sub");
                });
                x.AddPolicy(AuthorizationPolicies.DirectUiInternal, x =>
                {
                    x.RequireAuthenticatedUser();
                    x.RequireClaim("sub");
                    x.RequireClaim("internal");
                });
                x.AddPolicy(AuthorizationPolicies.ServerCommunicationExternal, x =>
                {
                    x.RequireAuthenticatedUser();
                    x.RequireAssertion(s => !s.User.HasClaim(z => z.Type.Equals("sub")));
                });
                x.AddPolicy(AuthorizationPolicies.ServerCommunicationInternal, x =>
                {
                    x.RequireAuthenticatedUser();
                    x.RequireAssertion(s => !s.User.HasClaim(z => z.Type.Equals("sub")));
                    x.RequireClaim("internal");
                });
            });

            services.AddScoped<RequestInformation>();
            services.AddHttpClient();
            services.AddHttpContextAccessor();

            services.AddSingleton<IUserIdProvider, SignalRUserIdProvider>();
            services.AddSignalR().AddStackExchangeRedis(redisConnectionString);
            services.AddWebSocket<TransactionalWebSocketHub>().AddAuthentication<WebSocketUserIdProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger(c =>
            {
                c.SerializeAsV2 = true;
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Version 1.0");
            });

            app.UseRequestLogging();

            app.UseWebSocketsWithPattern("/transactionalws");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<TransactionalHub>("/transactional");
            });

            // OnApplicationStarted event is published only after the application host has fully started, i.e.
            // after the services and request processing pipeline have been configured and the app has started serving web requests.
            // source: https://levelup.gitconnected.com/3-ways-to-run-code-once-at-application-startup-in-asp-net-core-bcf45a6b6605
            appLifetime.ApplicationStopping.Register(app.OnApplicationStopping);

            app.OnApplicationStarted();
        }
    }
}
