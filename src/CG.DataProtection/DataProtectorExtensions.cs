using CG;
using CG.DataProtection;
using CG.DataProtection.Properties;
using CG.Validations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;

namespace Microsoft.AspNetCore.DataProtection
{
    /// <summary>
    /// This class contains extension methods related to the <see cref="IDataProtector"/>
    /// type.
    /// </summary>
    public static partial class DataProtectorExtensions
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method encrypts the value of any properties on the specified 
        /// <paramref name="options"/> object that are: (1) decorated with a 
        /// <see cref="ProtectedPropertyAttribute"/> attribute, (2) are of 
        /// type: string, and (3) have a value in them. 
        /// </summary>
        /// <param name="dataProtector">The data protector object to use for the 
        /// operation.</param>
        /// <param name="options">The options object to use for the operation.</param>
        /// <returns>The value of the <paramref name="dataProtector"/> parameter, for 
        /// chaining calls together.</returns>
        /// <exception cref="ArgumentException">This exception is thrown whenever
        /// one or more of the required parameters is missing or invalid.</exception>
        /// <exception cref="InvalidOperationException">This exception is thrown whenever
        /// the underlying cryptography operation fails, for any reason.</exception>
        public static IDataProtector ProtectProperties(
            this IDataProtector dataProtector,
            object options
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(dataProtector, nameof(dataProtector))
                .ThrowIfNull(options, nameof(options));

            // Get a list of all object type properties.
            var props = options.GetType().GetProperties()
                .Where(x => x.PropertyType.IsClass)
                .Where(x => x.PropertyType != typeof(string))
                .ToList();

            // Loop and protect each property, recursively.
            props.ForEach(prop =>
            {
                object obj = null;
                try
                {
                    // Get the object reference.
                    obj = prop.GetGetMethod().Invoke(
                        options,
                        Array.Empty<object>()
                        );

                    // Check for missing references first ...
                    if (null != obj)
                    {
                        // Protect any properties for the object.
                        dataProtector.ProtectProperties(
                            obj
                            );
                    }
                }
                catch (Exception ex)
                {
                    // Wrap the exception.
                    throw new InvalidOperationException(
                        message: string.Format(
                            Resources.ConfigurationExtensions_ProtectProperties,
                            prop.Name,
                            obj.GetType().Name
                            ),
                        innerException: ex
                        ).SetOriginator(nameof(DataProtectorExtensions))
                         .SetDateTime();
                }
            });

            // Get a list of all the read/write properties of type: string.
            props = options.GetType().GetProperties()
                .Where(
                    x => x.CanRead && x.CanWrite && x.PropertyType == typeof(string)
                    ).ToList();

            // Loop and check each writeable property of type: string.
            props.ForEach(prop =>
            {
                try
                {
                    // Look for a custom attribute on the property.
                    if (prop.GetCustomAttributes(true)
                        .FirstOrDefault(
                            x => x.GetType() == typeof(ProtectedPropertyAttribute)
                            ) is ProtectedPropertyAttribute attr)
                    {
                        // If we get here then we should try to protect the value
                        //   of the decorated property.
                        var unprotectedPropertyValue = prop.GetGetMethod().Invoke(
                            options,
                            Array.Empty<object>()
                            ) as string;

                        // Check for empty strings first ...
                        if (false == string.IsNullOrEmpty(unprotectedPropertyValue))
                        {
                            // Protect the value.
                            var protectedPropertyValue = dataProtector.Protect(
                                unprotectedPropertyValue
                                );

                            // Write the protected string to the original property.
                            prop.GetSetMethod().Invoke(
                                options,
                                new[] { protectedPropertyValue }
                                );
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Wrap the exception.
                    throw new InvalidOperationException(
                        message: string.Format(
                            Resources.ConfigurationExtensions_ProtectProperties,
                            prop.Name,
                            options.GetType().Name
                        ),
                        innerException: ex
                    ).SetOriginator(nameof(DataProtectorExtensions))
                     .SetDateTime();
                }
            });

            // Return the configuration.
            return dataProtector;
        }

        // *******************************************************************

        /// <summary>
        /// This method decrypts the value of any properties on the specified 
        /// <paramref name="options"/> object that: (1) are decorated with a 
        /// <see cref="ProtectedPropertyAttribute"/> attribute, (2) are of 
        /// type: string, and (3) have a value in them.
        /// </summary>
        /// <param name="dataProtector">The data protector object to use for 
        /// the operation.</param>
        /// <param name="options">The options object to use for the operation.</param>
        /// <returns>The value of the <paramref name="dataProtector"/> parameter,
        /// for chaining calls together.</returns>
        /// <exception cref="ArgumentException">This exception is thrown whenever
        /// one or more of the required parameters is missing or invalid.</exception>
        /// <exception cref="InvalidOperationException">This exception is thrown whenever
        /// the underlying cryptography operation fails, for any reason.</exception>
        public static IDataProtector UnprotectProperties(
            this IDataProtector dataProtector,
            object options
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(dataProtector, nameof(dataProtector))
                .ThrowIfNull(options, nameof(options));

            // Get the actual type of object.
            var optionType = options.GetType();

            // Get a list of all object type properties.
            var props = optionType.GetProperties()
                .Where(x => x.PropertyType.IsClass)
                .Where(x => x.PropertyType != typeof(string))
                .ToList();

            // Loop and unprotect each property, recursively.
            props.ForEach(prop =>
            {
                object obj = null;
                try
                {
                    // Get the object reference.
                    obj = prop.GetGetMethod().Invoke(
                        options,
                        Array.Empty<object>()
                        );

                    // Check for missing references first ...
                    if (null != obj)
                    {
                        // Unprotect any properties for the object.
                        dataProtector.UnprotectProperties(
                            obj
                            );
                    }
                }
                catch (Exception ex)
                {
                    // Wrap the exception.
                    throw new InvalidOperationException(
                        message: string.Format(
                            Resources.ConfigurationExtensions_UnprotectProperties,
                            prop.Name,
                            obj.GetType().Name
                            ),
                        innerException: ex
                        ).SetOriginator(nameof(DataProtectorExtensions))
                            .SetDateTime();
                }
            });

            // Get a list of all the read/write properties of type: string.
            props = optionType.GetProperties()
                .Where(x => x.CanRead && x.CanWrite && x.PropertyType == typeof(string))
                .ToList();

            // Loop and unprotect each property.
            props.ForEach(prop =>
            {
                try
                {
                    // Look for a custom attribute on the property.
                    var attr = prop.GetCustomAttributes(
                        true
                        ).OfType<ProtectedPropertyAttribute>()
                         .FirstOrDefault();

                    // Did we find one?
                    if (null != attr)
                    {
                        // If we get here then we should try to unprotect the value
                        //   of the decorated property.
                        var encryptedPropertyValue = prop.GetGetMethod().Invoke(
                            options,
                            Array.Empty<object>()
                            ) as string;

                        // Check for empty strings first ...
                        if (!string.IsNullOrEmpty(encryptedPropertyValue))
                        {
                            try
                            {
                                // Unprotect the value.
                                var unprotectedPropertyValue = dataProtector.Unprotect(
                                    encryptedPropertyValue
                                    );

                                // Write the unprotected string to the original property.
                                prop.GetSetMethod().Invoke(
                                    options,
                                    new[] { unprotectedPropertyValue }
                                    );
                            }
                            catch (CryptographicException ex)
                            {
                                // Is the encryption manditory?
                                if (false == attr.Optional)
                                {
                                    // Was it not a decryption failure?
                                    if ("An error occurred during a cryptographic operation." != ex.Message)
                                    {
                                        throw;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Wrap the exception.
                    throw new InvalidOperationException(
                        message: string.Format(
                            Resources.ConfigurationExtensions_UnprotectProperties,
                            prop.Name,
                            options.GetType().Name
                            ),
                        innerException: ex
                        ).SetOriginator(nameof(DataProtectorExtensions))
                         .SetDateTime();
                }
            });

            // Return the configuration.
            return dataProtector;
        }

        #endregion
    }
}
