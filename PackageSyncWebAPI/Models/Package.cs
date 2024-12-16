namespace PackageSyncWebAPI.Models
{
    /// <summary>
    /// Represents the Package entity.
    /// Each package has the following fields:
    /// <ul>
    ///     <li>Id - Guid</li>
    ///     <li>Name - string</li>
    ///     <li>Status - PackageStatus</li>
    ///     <li>DateOfCreation - DateTime</li>
    ///     <li>DateOfDelivery - DateTime?</li>
    /// </ul>
    /// </summary>
    public class Package
    {
        /// <summary>
        /// Represents a unique identifier for the package.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Represents the name of the package.
        /// The package name has to be between 3 and 255 characters.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Represents the current status of the package.
        /// </summary>
        public PackageStatus Status { get; set; }

        /// <summary>
        /// Represents the date on which the package was created.
        /// The value of this field is automatically generated when the package is created.
        /// </summary>
        public DateTime DateOfCreation { get; set; }

        /// <summary>
        /// Represents the date on which the package was delivered.
        /// The date of delivery has to be in the future.
        /// </summary>
        public DateTime? DateOfDelivery { get; set; }
    }
}
