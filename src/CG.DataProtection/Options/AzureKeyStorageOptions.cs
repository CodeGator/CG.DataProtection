
namespace CG.DataProtection.Options;

/// <summary>
/// This class represents configuration settings for Azure key storage.
/// </summary>
public class AzureKeyStorageOptions
{
    // *******************************************************************
    // Properties.
    // *******************************************************************

    #region Properties

    /// <summary>
    /// This property contains a connection string for azure BLOB storage.
    /// </summary>
    [Required]
    public string ConnectionString { get; set; } = null!;

    /// <summary>
    /// This property contains a container name for azure BLOB storage.
    /// </summary>
    [Required]
    public string ContainerName { get; set; } = null!;

    /// <summary>
    /// This property contains a BLOB name for key storage.
    /// </summary>
    [Required]
    public string BlobName { get; set; } = null!;

    #endregion
}
