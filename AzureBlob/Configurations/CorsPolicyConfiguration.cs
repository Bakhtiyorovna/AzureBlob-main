namespace AzureBlob1.Configurations;

public static class CorsPolicyConfiguration
{
   
        public static void ConfigureCORSPolicy(this WebApplicationBuilder builder)
        {
            builder.Services.AddCors(option =>
            {
                option.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });

                option.AddPolicy("OnlySite", builder =>
                {
                    builder.WithOrigins("http://localhost:5173")
                        .AllowAnyMethod().AllowAnyHeader();
                });
            });
        }
    
}
