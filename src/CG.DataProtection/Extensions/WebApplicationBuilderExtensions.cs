
using Microsoft.Extensions.Configuration;

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

    public static WebApplicationBuilder AddDataProtectionWithSharedKeys(
        this WebApplicationBuilder webApplicationBuilder,
        Action<DataProtectionOptions> optionsDelegate,
        ILogger? bootstrapLogger = null
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(webApplicationBuilder, nameof(webApplicationBuilder));

        // Tell the world what we are about to do.
        bootstrapLogger?.LogDebug(
            "Calling the options delegate"
            );

        // Give the caller a change to change the options.
        var dataProtectionOptions = new DataProtectionOptions();
        optionsDelegate?.Invoke(dataProtectionOptions);

        // Tell the world what we are about to do.
        bootstrapLogger?.LogDebug(
            "Validating the Data Protection options"
            );

        // Ensure the options are valid.
        Guard.Instance().ThrowIfInvalidObject(dataProtectionOptions, nameof(dataProtectionOptions));

        // Tell the world what we are about to do.
        bootstrapLogger?.LogDebug(
            "Wiring up the Data Protection options"
            );

        // Ensure the options are available via the DI container.
        webApplicationBuilder.Services.ConfigureOptions(
            dataProtectionOptions
            );

        // Tell the world what we are about to do.
        bootstrapLogger?.LogDebug(
            "Registering the ASP.NET data protection provider"
            );

        // Add the data protection provider.
        var builder = webApplicationBuilder.Services.AddDataProtection();

        // Should we add shared Azure keys?
        if (dataProtectionOptions.AzureKeyStorage is not null)
        {
            // Tell the world what we are about to do.
            bootstrapLogger?.LogDebug(
                "Wiring up the ASP.NET data protection provider"
                );

            // Wire up the Azure key storage.
            builder.PersistKeysToAzureBlobStorage(
                dataProtectionOptions.AzureKeyStorage.ConnectionString,
                dataProtectionOptions.AzureKeyStorage.ContainerName,
                dataProtectionOptions.AzureKeyStorage.BlobName
                );
        }

        // Should we disable automatic key generation?
        if (dataProtectionOptions.DisableAutomaticKeyGeneration)
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

    // *******************************************************************

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
            out var dataProtectionOptions
            );

        // Tell the world what we are about to do.
        bootstrapLogger?.LogDebug(
            "Registering the ASP.NET data protection provider"
            );

        // Add the data protection provider.
        var builder = webApplicationBuilder.Services.AddDataProtection();

        // Should we add shared Azure keys?
        if (dataProtectionOptions.AzureKeyStorage is not null)
        {
            // Tell the world what we are about to do.
            bootstrapLogger?.LogDebug(
                "Registering Azure storage for the ASP.NET data protection provider"
                );

            // Wire up the Azure key storage.
            builder.PersistKeysToAzureBlobStorage(
                dataProtectionOptions.AzureKeyStorage.ConnectionString,
                dataProtectionOptions.AzureKeyStorage.ContainerName,
                dataProtectionOptions.AzureKeyStorage.BlobName
                );
        }

        // Should we disable automatic key generation?
        if (dataProtectionOptions.DisableAutomaticKeyGeneration)
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
