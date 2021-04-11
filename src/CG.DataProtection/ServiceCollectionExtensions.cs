using CG.Validations;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CG.DataProtection
{
    /// <summary>
    /// This class contains extension methods related to the <see cref="IServiceCollection"/>
    /// type.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method attempts to configure the specified <typeparamref name="TOptions"/>
        /// object as a singleton service. 
        /// </summary>
        /// <typeparam name="TOptions">The type of associated options.</typeparam>
        /// <param name="serviceCollection">The service collection to use for the 
        /// operation.</param>
        /// <param name="dataProtector">The data protector to use for the operation.</param>
        /// <param name="configuration">The configuration to use for the operation.</param>
        /// <returns>True if the options were configured; false otherwise.</returns>
        /// <exception cref="ArgumentException">This exception is thrown whenever
        /// one or more of the required parameters is missing or invalid.</exception>
        /// <remarks>
        /// In addition to binding the propeties of the <typeparamref name="TOptions"/>
        /// instance to the configuration, and validating the results, this method
        /// also unprotects any decorated properties on the <typeparamref name="TOptions"/>
        /// object.
        /// </remarks>
        public static bool TryConfigureOptions<TOptions>(
            this IServiceCollection serviceCollection,
            IDataProtector dataProtector,
            IConfiguration configuration
            ) where TOptions : class, new()
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(serviceCollection, nameof(serviceCollection))
                .ThrowIfNull(dataProtector, nameof(dataProtector))
                .ThrowIfNull(configuration, nameof(configuration));

            // Call the non-data protection overload.
            var result = serviceCollection.TryConfigureOptions<TOptions>(
                configuration,
                out var options
                );

            // Did we succeed?
            if (true == result)
            {
                // Unprotect any decorated properties.
                dataProtector.UnprotectProperties(options);
            }

            // Return the results.
            return result;
        }

        // *******************************************************************

        /// <summary>
        /// This method attempts to configure the specified <typeparamref name="TOptions"/>
        /// object as a singleton service. 
        /// </summary>
        /// <typeparam name="TOptions">The type of associated options.</typeparam>
        /// <param name="serviceCollection">The service collection to use for the 
        /// operation.</param>
        /// <param name="dataProtector">The data protector to use for the operation.</param>
        /// <param name="configuration">The configuration to use for the operation.</param>
        /// <param name="options">The options that were created by the operation.</param>
        /// <returns>True if the options were configured; false otherwise.</returns>
        /// <exception cref="ArgumentException">This exception is thrown whenever
        /// one or more of the required parameters is missing or invalid.</exception>
        /// <remarks>
        /// In addition to binding the propeties of the <typeparamref name="TOptions"/>
        /// instance to the configuration, and validating the results, this method
        /// also unprotects any decorated properties on the <typeparamref name="TOptions"/>
        /// object.
        /// </remarks>
        public static bool TryConfigureOptions<TOptions>(
            this IServiceCollection serviceCollection,
            IDataProtector dataProtector,
            IConfiguration configuration,
            out TOptions options
            ) where TOptions : class, new()
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(serviceCollection, nameof(serviceCollection))
                .ThrowIfNull(dataProtector, nameof(dataProtector))
                .ThrowIfNull(configuration, nameof(configuration));

            // Call the non-data protection overload.
            var result = serviceCollection.TryConfigureOptions<TOptions>(
                configuration,
                out options
                );

            // Did we succeed?
            if (true == result)
            {
                // Unprotect any decorated properties.
                dataProtector.UnprotectProperties(options);
            }

            // Return the results.
            return result;
        }

        #endregion
    }
}
