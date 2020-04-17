using System;
using System.Text;
using Broadcast.Core.Infrastructure.Security;
using Broadcast.Services.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Broadcast.Infrastructure.Auth
{
    public static class AuthenticationExtensions
    {
        private const string Issuer = "issuer";
        private const string Audience = "audience";

        private static SymmetricSecurityKey SigningKey => new SymmetricSecurityKey(Encoding.ASCII.GetBytes("somethinglongerforthisalgorithm"));
        private static SigningCredentials SigningCredentials => new SigningCredentials(SigningKey, SecurityAlgorithms.HmacSha256);

        public static void AddAuthenticationPipeline(this IServiceCollection services)
        {
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = Issuer;
                options.Audience = Audience;
                options.SigningCredentials = SigningCredentials;
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddCookieHandler()
                .AddJwtBearerHandler();
        }

        public static AuthenticationBuilder AddCookieHandler(this AuthenticationBuilder builder)
        {
            return builder.AddCookie(AuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.Name = "brdcst.auth";
                options.Cookie.HttpOnly = true;
                options.LoginPath = AuthenticationDefaults.LoginPath;
                options.AccessDeniedPath = AuthenticationDefaults.AccessDeniedPath;
            });
        }

        public static AuthenticationBuilder AddJwtBearerHandler(this AuthenticationBuilder builder)
        {
            return builder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = SigningCredentials.Key,
                    ValidateIssuer = true,
                    ValidIssuer = Issuer,
                    ValidateAudience = true,
                    ValidAudience = Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });
        }
    }
}