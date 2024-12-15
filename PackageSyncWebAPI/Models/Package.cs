namespace PackageSyncWebAPI.Models
{
    public class Package
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public PackageStatus Status { get; set; }

        public DateTime DateOfCreation { get; set; }

        public DateTime? DateOfDelivery { get; set; }
    }
}
