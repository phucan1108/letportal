using System.Collections.Generic;

namespace LetPortal.Core.Configurations
{
    public class CorsPortalOptions
    {
        public List<string> AllowedHosts { get; set; }

        public List<string> AllowedHeaders { get; set; }

        public List<string> AllowedMethods { get; set; }

        public List<string> ExposedHeaders { get; set; }

        public bool AllowAnyHost { get; set; } = true;

        public bool AllowAnyHeader { get; set; } = true;

        public bool AllowAnyMethod { get; set; } = true;

        public bool AllowCredentials { get; set; } = false;

        public bool AllowAny { get; set; } = true; // Turn all into AllowAny

    }
}
