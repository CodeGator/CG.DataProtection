using CG.Validations;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CG.DataProtection
{
    /// <summary>
    /// This class provides AES encryption / decryption support.
    /// </summary>
    public class AesProtector : SingletonBase<AesProtector>
    {
        // *******************************************************************
        // Constructors.
        // *******************************************************************

        #region Constructors

        /// <summary>
        /// This constructor creates a new instance of the <see cref="AesProtector"/>
        /// class.
        /// </summary>
        private AesProtector() { }

        #endregion

        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method returns the protected value of the <paramref name="plainText"/>
        /// parameter.
        /// </summary>
        /// <param name="password">The password to use for the operation.</param>
        /// <param name="salt">The SALT/IV value to use for the operation.</param>
        /// <param name="plainText">The plain text to use for the operation.</param>
        /// <returns>The protected value of the <paramref name="plainText"/> 
        /// parameter.</returns>
        public string Protect(
            string password,
            string salt,
            string plainText
            )
        {
            try
            {
                // Validate the parameters before attempting to use them.
                Guard.Instance().ThrowIfNullOrEmpty(password, nameof(password))
                    .ThrowIfNullOrEmpty(salt, nameof(salt))
                    .ThrowIfNullOrEmpty(plainText, nameof(plainText));

                var encryptedBytes = new byte[0];

                // Get the password bytes.
                var passwordBytes = Encoding.UTF8.GetBytes(
                    password
                    );

                // Get the salt bytes.
                var saltBytes = Encoding.UTF8.GetBytes(
                    salt
                    );

                // Create the algorithm
                using (var alg = Aes.Create())
                {
                    // Set the block and key sizes.
                    alg.KeySize = 256;
                    alg.BlockSize = 128;

                    // Derive the ACTUAL crypto key.
                    var key = new Rfc2898DeriveBytes(
                        passwordBytes,
                        saltBytes,
                        10000
                        );

                    // Generate the key and salt with proper lengths.
                    alg.Key = key.GetBytes(alg.KeySize / 8);
                    alg.IV = key.GetBytes(alg.BlockSize / 8);

                    // Create the encryptor.
                    using (var enc = alg.CreateEncryptor(
                        alg.Key,
                        alg.IV
                        ))
                    {
                        // Create a temporary stream.
                        using (var stream = new MemoryStream())
                        {
                            // Create a cryptographic stream.
                            using (var cryptoStream = new CryptoStream(
                                stream,
                                enc,
                                CryptoStreamMode.Write
                                ))
                            {
                                // Create a writer
                                using (var writer = new StreamWriter(
                                    cryptoStream
                                    ))
                                {
                                    // Write the bytes.
                                    writer.Write(
                                        plainText
                                        );
                                }

                                // Retrieve the bytes.
                                encryptedBytes = stream.ToArray();
                            }
                        }
                    }
                }

                // Convert the bytes back to an encoded string.
                var encryptedValue = Convert.ToBase64String(
                    encryptedBytes
                    );

                // Return the results.
                return encryptedValue;
            }
            catch (Exception ex)
            {
                // Provide better context for the error.
                throw new Exception(
                    message: $"Failed to protect the string!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(RijndaelProtector))
                     .SetDateTime();
            }
        }

        // *******************************************************************

        /// <summary>
        /// This method returns the unprotected value of the <paramref name="encryptedText"/>
        /// parameter.
        /// </summary>
        /// <param name="password">The password to use for the operation.</param>
        /// <param name="salt">The SALT/IV value to use for the operation.</param>
        /// <param name="encryptedText">The protected text to use for the operation.</param>
        /// <returns>The unprotected value of the <paramref name="encryptedText"/> 
        /// parameter.</returns>
        public string Unprotect(
            string password,
            string salt,
            string encryptedText
            )
        {
            try
            {
                // Validate the parameters before attempting to use them.
                Guard.Instance().ThrowIfNullOrEmpty(password, nameof(password))
                    .ThrowIfNullOrEmpty(salt, nameof(salt))
                    .ThrowIfNullOrEmpty(encryptedText, nameof(encryptedText));

                // Convert the encrypted value to bytes.
                var encryptedBytes = Convert.FromBase64String(
                    encryptedText
                    );

                // Get the password bytes.
                var passwordBytes = Encoding.UTF8.GetBytes(
                    password
                    );

                // Get the salt bytes.
                var saltBytes = Encoding.UTF8.GetBytes(
                    salt
                    );

                var plainValue = "";

                // Create the algorithm
                using (var alg = Aes.Create())
                {
                    // Set the block and key sizes.
                    alg.KeySize = 256;
                    alg.BlockSize = 128;

                    // Derive the ACTUAL crypto key.
                    var key = new Rfc2898DeriveBytes(
                        passwordBytes,
                        saltBytes,
                        10000
                        );

                    // Generate the key and salt with proper lengths.
                    alg.Key = key.GetBytes(alg.KeySize / 8);
                    alg.IV = key.GetBytes(alg.BlockSize / 8);

                    // Create the decryptor.
                    using (var dec = alg.CreateDecryptor(
                        alg.Key,
                        alg.IV
                        ))
                    {
                        // Create a temporary stream.
                        using (var stream = new MemoryStream(
                            encryptedBytes
                            ))
                        {
                            // Create a crypto stream.
                            using (var cryptoStream = new CryptoStream(
                                stream,
                                dec,
                                CryptoStreamMode.Read
                                ))
                            {
                                // Read the bytes.
                                using (var reader = new StreamReader(
                                    cryptoStream
                                    ))
                                {
                                    // Retrieve the plain text.
                                    plainValue = reader.ReadToEnd();
                                }
                            }
                        }
                    }
                }

                // Return the results.
                return plainValue;
            }
            catch (Exception ex)
            {
                // Provide better context for the error.
                throw new Exception(
                    message: $"Failed to unprotect the string!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(RijndaelProtector))
                     .SetDateTime();
            }
        }

        #endregion
    }
}
