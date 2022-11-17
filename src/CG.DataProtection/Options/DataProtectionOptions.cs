
namespace CG.DataProtection.Options;

/// <summary>
/// This class represents configuration settings for data protection startup.
/// </summary>
public class DataProtectionOptions
{
    // *******************************************************************
    // Properties.
    // *******************************************************************

    #region Properties

    /// <summary>
    /// This property indicates whether or not data protection should 
    /// generate new keys automatically.
    /// </summary>
    [Required]
    public bool DisableAutomaticKeyGeneration { get; set; }

    /// <summary>
    /// This property contains optional Azure specific settings for shared
    /// key storage.
    /// </summary>
    public AzureKeyStorageOptions? AzureKeyStorage {get;set;}

    #endregion
}
