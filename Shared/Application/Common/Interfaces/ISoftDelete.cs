namespace Shared.Application.Common.Interfaces
{
    public interface ISoftDelete
    {
        DateTime? DeletedOn { get; set; }
        Guid? DeletedBy { get; set; }
        string? DeletedReason { get; set; }
        bool IsDeleted { get; set; }
    }
}
