using CG.Validations;
using Microsoft.AspNetCore.DataProtection;
using System;
using System.Diagnostics;
using System.IO;

namespace CG.DataProtection
{
    /// <summary>
    /// This class is a singleton implementation of the <see cref="IDataProtector"/>
    /// interface.
    /// </summary>
    /// <remarks>
    /// Use this class in non-DI environments, and/or, for local data protection 
    /// scenarios.  
    /// </remarks>
    public class DataProtector : SingletonBase<DataProtector>, IDataProtector
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains a reference to a local data protection provider.
        /// </summary>
        protected IDataProtectionProvider Provider { get; }

        /// <summary>
        /// This property contains a reference to a local data protector.
        /// </summary>
        protected IDataProtector Protector { get; }

        #endregion

        // *******************************************************************
        // Constructors.
        // *******************************************************************

        #region Constructors

        /// <summary>
        /// This constructor creates a new instance of the <see cref="DataProtector"/>
        /// class.
        /// </summary>
        [DebuggerStepThrough]
        private DataProtector() 
        {
            // Get the calling application's friendly name.
            var friendlyName = AppDomain.CurrentDomain.FriendlyNameEx(
                true
                );

            // Get the path to the localappdata folder.
            var localAppData = Environment.GetEnvironmentVariable(
                "LOCALAPPDATA"
                );

            // Create the complete path to any local keys.
            var destFolder = Path.Combine(
                localAppData,
                friendlyName
                );

            // Create a local provider instance.
            Provider = DataProtectionProvider.Create(
                new DirectoryInfo(destFolder)
                );

            // Create a local protector instance.
            Protector = Provider.CreateProtector(
                nameof(DataProtector)
                );
        }

        #endregion

        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method cryptographically protects a piece of plaintext data.
        /// </summary>
        /// <param name="plaintext">The plaintext string to protect.</param>
        /// <returns>The protected form of the plaintext data.</returns>
        [DebuggerStepThrough]
        public byte[] Protect(byte[] plaintext)
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(plaintext, nameof(plaintext));

            // Defer to the local protector.
            return Protector.Protect(plaintext);
        }

        // *******************************************************************

        /// <summary>
        /// This method cryptographically unprotects a piece of protected data.
        /// </summary>
        /// <param name="protectedData">The protected data to unprotect.</param>
        /// <returns>The plaintext form of the protected data.</returns>
        [DebuggerStepThrough]
        public byte[] Unprotect(byte[] protectedData)
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(protectedData, nameof(protectedData));

            // Defer to the local protector.
            return Protector.Unprotect(protectedData);
        }

        // *******************************************************************

        /// <summary>
        /// This method creates an <see cref="IDataProtector"/> for a given 
        /// purpose.
        /// </summary>
        /// <param name="purpose">The purpose to be assigned to the newly-created 
        /// <see cref="IDataProtector"/> object.</param>
        /// <returns>An <see cref="IDataProtector"/> tied to the provided purpose.</returns>
        [DebuggerStepThrough]
        public IDataProtector CreateProtector(string purpose)
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNullOrEmpty(purpose, nameof(purpose));

            // Defer to the local provider.
            return Provider.CreateProtector(purpose);
        }

        #endregion
    }
}
