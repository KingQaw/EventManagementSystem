using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using EventManagementSystem.Models;
using EventManagementSystem.Data;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class RegistrationsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public RegistrationsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: 报名活动
    public async Task<IActionResult> Register(int eventId)
    {
        var @event = await _context.Events.FindAsync(eventId);
        if (@event == null)
        {
            return NotFound();
        }

        // 检查是否已经报名
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var existingRegistration = await _context.Registrations
            .FirstOrDefaultAsync(r => r.EventId == eventId && r.AttendeeId == userId);

        if (existingRegistration != null)
        {
            return RedirectToAction("AlreadyRegistered", new { eventId });
        }

        // 检查活动是否已满
        var registrationCount = await _context.Registrations
            .CountAsync(r => r.EventId == eventId);

        if (registrationCount >= @event.MaxParticipants)
        {
            return RedirectToAction("EventFull", new { eventId });
        }

        return View(@event);
    }

    // POST: 确认报名
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmRegister(int eventId)
    {
        var @event = await _context.Events.FindAsync(eventId);
        if (@event == null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // 再次检查是否已经报名
        var existingRegistration = await _context.Registrations
            .FirstOrDefaultAsync(r => r.EventId == eventId && r.AttendeeId == userId);

        if (existingRegistration != null)
        {
            return RedirectToAction("AlreadyRegistered", new { eventId });
        }

        // 再次检查活动是否已满
        var registrationCount = await _context.Registrations
            .CountAsync(r => r.EventId == eventId);

        if (registrationCount >= @event.MaxParticipants)
        {
            return RedirectToAction("EventFull", new { eventId });
        }

        // 创建报名记录
        var registration = new Registration
        {
            EventId = eventId,
            AttendeeId = userId,
            RegistrationDate = DateTime.UtcNow
        };

        _context.Registrations.Add(registration);
        await _context.SaveChangesAsync();

        return RedirectToAction("RegistrationSuccess", new { eventId });
    }

    // 报名成功页面
    public async Task<IActionResult> RegistrationSuccess(int eventId)
    {
        var @event = await _context.Events.FindAsync(eventId);
        if (@event == null)
        {
            return NotFound();
        }

        return View(@event);
    }

    // 已经报名页面
    public async Task<IActionResult> AlreadyRegistered(int eventId)
    {
        var @event = await _context.Events.FindAsync(eventId);
        if (@event == null)
        {
            return NotFound();
        }

        return View(@event);
    }

    // 活动已满页面
    public async Task<IActionResult> EventFull(int eventId)
    {
        var @event = await _context.Events.FindAsync(eventId);
        if (@event == null)
        {
            return NotFound();
        }

        return View(@event);
    }
}