namespace UnityInitializer.Core.Consts
{
    public static class CommandConstants
    {
        public static class Options
        {
            public static class Version
            {
                public const string SHORT_ALIAS = "-v";
                public const string LONG_ALIAS = "--version";
                public const string DESCRIPTION = "Unity version to initialize project with";
            }

            public static class List
            {
                public const string SHORT_ALIAS = "-l";
                public const string LONG_ALIAS = "--list";
                public const string DESCRIPTION = "List all available Unity versions";
            }
        }

        public static class Messages
        {
            public const string ROOT_COMMAND_DESCRIPTION = "Unity project initializer";
            public const string VERSION_REQUIRED_ERROR = "Error: Version parameter is required. Use -v to specify Unity version or -l to list available versions.";
            public const string AVAILABLE_VERSIONS_HEADER = "Available Unity versions:";
            public const string NO_VERSIONS_FOUND = "No Unity versions found in embedded resources.";
            public const string INITIALIZING_PROJECT = "Initializing Unity project version {0} in {1}...";
            public const string INITIALIZATION_FAILED = "Failed: {0}";
        }

        public static class SystemOptions
        {
            public const string VERSION_OPTION_NAME = "--version";
        }
    }
}