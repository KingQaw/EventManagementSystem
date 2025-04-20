using System.ComponentModel.DataAnnotations;

namespace EventManagementSystem.Models;

public class Event
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    [Required]
    public string Location { get; set; }

    public int MaxParticipants { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // 外键关联到发布者
    public string OrganizerId { get; set; }
    public ApplicationUser Organizer { get; set; }

    public ICollection<Registration> Registrations { get; set; }
}