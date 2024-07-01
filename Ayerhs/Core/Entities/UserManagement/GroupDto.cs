namespace Ayerhs.Core.Entities.UserManagement
{
    public class GroupDto
    {
        public int Id { get; set; }
        public string? GroupId { get; set; }
        public string? GroupName { get; set; }
        public int PartitionId { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? GroupCreatedOn { get; set; }
        public DateTime? GroupUpdatedOn { get; set; }
        public DateTime? GroupDeletedOn { get; set; }
        public PartitionDto Partition { get; set; }
    }
}
