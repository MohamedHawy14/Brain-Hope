
using BrainHope.DataAcess.Contexts;
using BrainHope.DataAcess.Models;
using BrainHope.DataAcess.Repositry.IRepository;
using BrainHope.DataAcess.Repositry;
using BrainHope.Services.DTO.Email;
using BrainHope.Services.InterFaces;
using BrainHope.Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using BrainHope.Services.Hubs;
using Microsoft.AspNetCore.HttpOverrides;
using Utilites;
using Microsoft.Extensions.Configuration;
using System;

namespace BrainHope_.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            //Services

            builder.Services.AddScoped<IPostService, PostService>();

            //For Entity FrameWork
            builder.Services.AddDbContext<BrainHopeDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("cs")),
            ServiceLifetime.Scoped);

            //For Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<BrainHopeDbContext>().AddDefaultTokenProviders();

            //Repository & UnitOfWork
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IChatRepository, ChatRepository>();
            builder.Services.AddScoped<IPostRepository, PostRepository>();

            //signalr
            builder.Services.AddSignalR();

            // Add in-memory distributed cache 
            builder.Services.AddDistributedMemoryCache();

            // Configure Session
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(5); 
                options.Cookie.HttpOnly = true; 
                options.Cookie.IsEssential = true; 
            });




            #region JWT
            //Add Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
 {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                   ValidateIssuerSigningKey = true,
                   ValidateLifetime = true,
                   ValidateIssuer = true,
                   ClockSkew = TimeSpan.Zero,
                   ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                   ValidateAudience = true,
                   ValidAudience = builder.Configuration["JWT:ValidAudience"],
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"])) // FIXED TYPO
                };
            });
            builder.Services.AddAuthorization();

            #endregion


            #region Email
            var Configure = builder.Configuration;
            var emailconfig = Configure.GetSection("EmailConfiguration").Get<EmailConfiguration>();
            builder.Services.AddSingleton(emailconfig);
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.Configure<IdentityOptions>(opts => opts.SignIn.RequireConfirmedEmail = true);
            #endregion

            builder.Services.Configure<DataProtectionTokenProviderOptions>(opts => opts.TokenLifespan = TimeSpan.FromMinutes(45));


            // Add services to the container.
            builder.Services.AddScoped<IAuthServices, AuthServices>();

            // Add Cors
            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                    policy =>
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader()
                              .WithExposedHeaders("Set-Cookie");
                    });
            });


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            #region Swagger
            builder.Services.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Brain Hope API",
                    Description = "Api"
                });

                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
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
                Array.Empty<string>()
            }
        });
            });
            #endregion
         
            var app = builder.Build();
            // Configure forwarded headers before anything else
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });

            // Enable serving static files from wwwroot
            app.UseStaticFiles();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors(MyAllowSpecificOrigins);
            app.UseAuthentication();
            app.UseSession();
            app.UseAuthorization();
            app.MapHub<ChatHub>("/chathub");
            app.MapHub<PostHub>("/postHub");
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Brain Hope API v1");
                options.RoutePrefix = string.Empty; 
            });

           

            app.MapControllers();

            app.Run();
        }
    }
}
