using Microsoft.OpenApi.Models;

namespace API.Supports.Constants
{
    public static class SwashbuckleConfig
    {
        public static readonly OpenApiSecurityScheme SecurityScheme = new()
        {
            
            Description = "Please insert JWT with Bearer into field",
            In = ParameterLocation.Header,
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey
        };

        public static readonly OpenApiSecurityRequirement SecurityRequirements = new()
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                },
                new string[] { }
            }
        };
    }
}
