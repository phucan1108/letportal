namespace LetPortal.Core.Security
{
    public static class Roles
    {
        public const string SuperAdmin = "SuperAdmin";

        public const string Admin = "Admin";

        public const string Developer = "Developer";

        public const string User = "User";

        public const string BackEndRoles = SuperAdmin + "," + Admin + "," + Developer;

        public const string AdminRoles = SuperAdmin + "," + Admin;
    }
}
