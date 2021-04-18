using System;

namespace CG.DataProtection
{
    /// <summary>
    /// This class represents an encrypted option property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ProtectedPropertyAttribute : Attribute
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property indicates the encryption on the property is optional
        /// and the value of the property may also be stored in clear text.
        /// </summary>
        public bool Optional { get; set; }

        #endregion
    }
}
