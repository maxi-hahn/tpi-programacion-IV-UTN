namespace Presentation.Authorization
{
    public enum AuthorizationPolicy
    {
        SoloAdmin,
        SoloClient,
        SoloSysAdmin,
        AdminOSysAdmin
    }

    public static class Policies
    {
        public const string SoloAdmin = nameof(AuthorizationPolicy.SoloAdmin);
        public const string SoloClient = nameof(AuthorizationPolicy.SoloClient);
        public const string SoloSysAdmin = nameof(AuthorizationPolicy.SoloSysAdmin);
        public const string AdminOSysAdmin = nameof(AuthorizationPolicy.AdminOSysAdmin);
    }
}