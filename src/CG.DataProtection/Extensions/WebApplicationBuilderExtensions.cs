
namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// This class contains extension methods related to the <see cref="WebApplicationBuilder"/>
/// type.
/// </summary>
public static partial class WebApplicationBuilderExtensions
{
    // *******************************************************************
    // Public methods.
    // *******************************************************************

    #region Public methods

    /// <summary>
    /// This method adds the services required for ASP.NET data protection 
    /// using shared keys from an Azure BLOB container that is configured
    /// using the given configuration section. 
    /// </summary>
    /// <param name="webApplicationBuilder">The web application builder
    /// to use for the operation.</param>
    /// <param name="configurationPath">The configuration path to use for
    /// the operation.</param>
    /// <param name="bootstrapLogger">An optional bootstrap logger to use
    /// for the operation.</param>
    /// <returns>The value of the <paramref name="webApplicationBuilder"/>
    /// parameter, for chaining calls together, Fluent style.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more arguments are missing, or invalid.</exception>
    public static WebApplicationBuilder AddDataProtectionWithSharedKeys(
        this WebApplicationBuilder webApplicationBuilder,
        string configurationPath = "DataProtection" ,
        ILogger? bootstrapLogger = null
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(webApplicationBuilder, nameof(webApplicationBuilder));

        // Tell the world what we are about to do.
        bootstrapLogger?.LogInformation(
            "Configuring ASP.NET data protection startup options"
            );

        // Configure the data protection options.
        webApplicationBuilder.Services.ConfigureOptions<DataProtectionOptions>(
            webApplicationBuilder.Configuration.GetSection(configurationPath),
            out var options
            );

        // Tell the world what we are about to do.
        bootstrapLogger?.LogInformation(
            "Registering the ASP.NET data protection provider"
            );

        // Add the data protection provider.
        var builder = webApplicationBuilder.Services.AddDataProtection();

        // Should we add shared Azure keys?
        if (options.AzureKeyStorage is not null)
        {
            // Tell the world what we are about to do.
            bootstrapLogger?.LogInformation(
                "Registering Azure storage for the ASP.NET data protection provider"
                );

            // Wire up the Azure key storage.
            builder.PersistKeysToAzureBlobStorage(
                options.AzureKeyStorage.ConnectionString,
                options.AzureKeyStorage.ContainerName,
                options.AzureKeyStorage.BlobName
                );
        }

        // Should we disable automatic key generation?
        if (options.DisableAutomaticKeyGeneration)
        {
            // Tell the world what we are about to do.
            bootstrapLogger?.LogWarning(
                "Disabling automatic key generation for the ASP.NET data protection provider"
                );

            // Disable automatic key generation.
            builder.DisableAutomaticKeyGeneration();
        }
        
        // Return the application builder.
        return webApplicationBuilder;
    }

    #endregion
}
