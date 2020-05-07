namespace Broadcast.Core.Domain.Users
{
    public class UserDefaults
    {
        #region Default role

        public static string AdminRole => "Admin";

        public static string TesterRole => "Tester";

        #endregion

        #region Cookie

        public static string Prefix => "brdcst";

        public static string GuidCookie => ".xguid";

        #endregion
    }
}