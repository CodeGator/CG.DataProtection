using System;

namespace Microsoft.AspNetCore.DataProtection
{
    /// <summary>
    /// This class represents an encrypted option property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ProtectedPropertyAttribute : Attribute
    {

    }
}
