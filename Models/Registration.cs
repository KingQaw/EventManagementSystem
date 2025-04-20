namespace EventManagementSystem.Models;

public class Registration
{
    public int Id { get; set; }

    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

    // 外键关联到活动
    public int EventId { get; set; }
    public Event Event { get; set; }

    // 外键关联到报名用户
    public string AttendeeId { get; set; }
    public ApplicationUser Attendee { get; set; }
}