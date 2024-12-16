namespace PackageSyncWebAPI.Models
{
    /// <summary>
    /// Represents the status of a package.
    /// A package can have one of the three following statuses:
    /// <ul>
    ///     <li>InWarehouse - The package was created and is waiting to be delivered to its destination.</li>
    ///     <li>InTransit - The package is being delivered to its destination.</li>
    ///     <li>Delivered - The package has been delivered to its destination.</li>
    /// </ul>
    /// </summary>
    public enum PackageStatus
    {
        InWarehouse, InTransit, Delivered
    }
}
