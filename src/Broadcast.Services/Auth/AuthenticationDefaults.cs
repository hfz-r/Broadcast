using Microsoft.AspNetCore.Http;

namespace Broadcast.Services.Auth
{
    public static class AuthenticationDefaults
    {
        public static string AuthenticationScheme => "Authentication";

        public static string ClaimsIssuer => "brdcst";

        public static PathString LoginPath => new PathString("/login");

        public static PathString LogoutPath => new PathString("/logout");

        public static PathString AccessDeniedPath => new PathString("/page-not-found");
    }
}