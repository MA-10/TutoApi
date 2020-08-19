namespace Test
{
    public class ApiRoutes
    {
        public const string Root = "api";

        public const string Version = "";

        public const string Base = Root + "/" + Version;
        public static class Cutomers
        {
            public const string GetAll = Base + "/Cutomers";

            public const string Update = Base + "/Cutomers/{Id}";

            public const string Delete = Base + "/Cutomers/{Id}";

            public const string Get = Base + "/Cutomers/{Id}";

            public const string Create = Base + "/Cutomers";
        }
        public static class Identity
        {
            public const string Login = Base + "/identity/login";

            public const string Register = Base + "/identity/register";

            public const string Refresh = Base + "/identity/refresh";
        }


    }
}