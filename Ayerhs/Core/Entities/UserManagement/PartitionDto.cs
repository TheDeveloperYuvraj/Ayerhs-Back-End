namespace Ayerhs.Core.Entities.UserManagement
{
    public class PartitionDto
    {
        public int Id { get; set; }
        public string PartitionId { get; set; }
        public string PartitionName { get; set; }
        public DateTime? PartitionCreatedOn { get; set; }
        public DateTime? PartitionUpdatedOn { get; set; }
    }
}
