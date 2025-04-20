using Microsoft.AspNetCore.Identity;

namespace EventManagementSystem.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; }

    // 发布的活动
    public ICollection<Event> OrganizedEvents { get; set; }

    // 报名的活动
    public ICollection<Registration> AttendedEvents { get; set; }
}