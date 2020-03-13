namespace LetPortal.Portal.Constants
{
    public static class RolesConstants
    {
        public const string SUPER_ADMIN = "SuperAdmin";

        public const string ADMIN = "Admin";

        public const string DEVELOPER = "Developer";

        public const string USER = "User";

        public const string BACK_END_ROLES = SUPER_ADMIN + "," + ADMIN + "," + DEVELOPER;

        public const string ALL_ADMIN_ROLES = SUPER_ADMIN + "," + ADMIN;
    }
}
