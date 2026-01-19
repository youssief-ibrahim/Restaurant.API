
using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Restaurant.API.Middlewares.ExceptionMid;
using Restaurant.API.Middlewares.languageMid;
using Restaurant.Application.Common.Mapping;
using Restaurant.Application.Interfaces;
using Restaurant.Application.Services.CartSR;
using Restaurant.Application.Services.ChefSR;
using Restaurant.Application.Services.CustomerSR;
using Restaurant.Application.Services.DeliverySR;
using Restaurant.Application.Services.MealSR;
using Restaurant.Application.Services.OrderSR;
using Restaurant.Application.Services.PaymentSR;
using Restaurant.Application.Services.ReviewSR;
using Restaurant.Infrastructure.Data;
using Restaurant.Infrastructure.Identity;
using Restaurant.Infrastructure.Localization;
using Restaurant.Infrastructure.Repositories;
using Restaurant.Infrastructure.Seeds;
using Restaurant.Infrastructure.Services.Account;
using Restaurant.Infrastructure.Services.Auth.AuthService;
using Restaurant.Infrastructure.Services.Auth.Tokens;
using Restaurant.Infrastructure.Services.Authorize;
using Restaurant.Infrastructure.Services.Email;

namespace Restaurant.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            var connect = builder.Configuration.GetConnectionString("defultconnection");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connect));

            builder.Services.AddIdentity<AppilcationUser, ApplicationRole>(option =>
            {
                option.SignIn.RequireConfirmedEmail = true;
            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            builder.Services.AddLocalization();

            // Resolve 
            builder.Services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
            builder.Services.AddScoped(typeof(IGenericRebosatory<>), typeof(GenericRebosatory<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddAutoMapper(typeof(HelperMapper).Assembly);
            builder.Services.AddScoped<IEmailService, EmailSender>();
            builder.Services.AddScoped<ITokenReposatory, TokenReposatory>();

            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<ICustomerServices, CustomerServices>();
            builder.Services.AddScoped<IChefServices, ChefServices>();
            builder.Services.AddScoped<IMealServices, MealServices>();
            builder.Services.AddScoped<IOrderServices, OrderServices>();
            builder.Services.AddScoped<IReviewService, ReviewService>();
            builder.Services.AddScoped<ICartServices, CartServices>();
            builder.Services.AddScoped<IAuthorizeService, AuthorizeService>();
            builder.Services.AddScoped<IDeliveryServices, DeliveryServices>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();

            builder.Services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(option => // vervifed
            {
                option.RequireHttpsMetadata = true;
                option.SaveToken = true;  // still token is valied
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"])),
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JWT:ValidIssuerIP"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:ValidAudienceIP"],
                    ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services.AddSwaggerGen(swagger =>
            {
                //This is to generate the Default UI of Swagger Documentation
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Restaurant System API",
                    Description = "Api for Restaurant"
                });
                swagger.EnableAnnotations();
                // To Enable authorization using Swagger (JWT)
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token in the tex"
                });

                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                         new string[] {}
                     }
                });
            });

            builder.Services.AddMvc().AddDataAnnotationsLocalization(option =>
            option.DataAnnotationLocalizerProvider = (type, factory) =>
            {
                return factory.Create(typeof(JsonStringLocalizer));
            }
            );

            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var SupportedCulter = new CultureInfo[]
                {
                   new CultureInfo("en-US"),
                   new CultureInfo("fr-FR"),
                   new CultureInfo("ar-EG"),
                };
                //options.DefaultRequestCulture = new RequestCulture(culture: SupportedCulter[0]); // Optional: Set default culture
                options.SupportedCultures = SupportedCulter;
                //options.SupportedUICultures = SupportedCulter;

            });

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var userManager = services.GetRequiredService<UserManager<AppilcationUser>>();
                var rolemanger = services.GetRequiredService<RoleManager<ApplicationRole>>();

                await DefaultRoles.SeedAsync(rolemanger);
                await DefultUser.GenerateAdmin(userManager, rolemanger);

                //await DefultUser.SeedAdminAllPermissions(rolemanger);
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            var supportedCulter = new[] { "en-US", "fr-FR", "ar-EG" };
            var requestLocalizationOptions = new RequestLocalizationOptions()
                //.SetDefaultCulture(supportedCulter[0]) // Optional: Set default culture
                .AddSupportedCultures(supportedCulter);

            app.UseRequestLocalization(requestLocalizationOptions);

            app.UseRequestLanguesMidelware();

            app.UseHttpsRedirection();

            app.UseExceptionMiddleware();


            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
