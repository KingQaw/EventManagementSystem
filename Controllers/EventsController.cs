using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using EventManagementSystem.Data;
using EventManagementSystem.Models;

[Authorize]
public class EventsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public EventsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: 活动列表（公开）
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var events = await _context.Events
            .Include(e => e.Organizer)
            .OrderByDescending(e => e.StartTime)
            .ToListAsync();
        return View(events);
    }

    // GET: 活动详情（公开）
    [AllowAnonymous]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var @event = await _context.Events
            .Include(e => e.Organizer)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (@event == null)
        {
            return NotFound();
        }

        return View(@event);
    }

    // GET: 创建活动（仅限组织者）
    [Authorize(Roles = "Organizer")]
    public IActionResult Create()
    {
        return View();
    }

    // POST: 创建活动（仅限组织者）
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Organizer")]
    public async Task<IActionResult> Create([Bind("Title,Description,StartTime,EndTime,Location,MaxParticipants")] Event @event)
    {
        if (ModelState.IsValid)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            @event.OrganizerId = userId;

            _context.Add(@event);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MyEvents));
        }
        return View(@event);
    }

    // GET: 我发布的活动
    [Authorize(Roles = "Organizer")]
    public async Task<IActionResult> MyEvents()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var events = await _context.Events
            .Where(e => e.OrganizerId == userId)
            .OrderByDescending(e => e.StartTime)
            .ToListAsync();

        return View(events);
    }

    // GET: 活动报名情况
    [Authorize(Roles = "Organizer")]
    public async Task<IActionResult> Registrations(int id)
    {
        var @event = await _context.Events
            .Include(e => e.Registrations)
            .ThenInclude(r => r.Attendee)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (@event == null)
        {
            return NotFound();
        }

        // 检查当前用户是否是活动发布者
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (@event.OrganizerId != userId)
        {
            return Forbid();
        }

        return View(@event);
    }
}