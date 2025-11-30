using HelloCSharp.Areas.UserManagement.Models;
using HelloCSharp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelloCSharp.Areas.UserManagement.Controllers;

[Area("UserManagement")]
public class AttributeController : Controller
{
    private readonly AppDbContext _context;

    // DIでAppDbContextを受け取る
    public AttributeController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /UserManagement/Attribute
    // SPA版の属性管理画面（TypeScriptでレンダリング）
    public IActionResult Index()
    {
        return View("IndexSpa");
    }

    // GET: /UserManagement/Attribute/Legacy
    // 従来のMVC版（比較用に残す）
    public async Task<IActionResult> Legacy()
    {
        var attributes = await _context.Attributes
            .OrderBy(a => a.DisplayOrder)
            .ToListAsync();
        return View("Index", attributes);
    }

    // GET: /UserManagement/Attribute/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: /UserManagement/Attribute/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AttributeDefinition attribute)
    {
        if (!ModelState.IsValid)
        {
            return View(attribute);
        }

        _context.Attributes.Add(attribute);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: /UserManagement/Attribute/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var attribute = await _context.Attributes.FindAsync(id);
        if (attribute == null) return NotFound();

        return View(attribute);
    }

    // POST: /UserManagement/Attribute/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AttributeDefinition attribute)
    {
        if (id != attribute.Id) return NotFound();

        if (!ModelState.IsValid)
        {
            return View(attribute);
        }

        try
        {
            _context.Update(attribute);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await AttributeExists(id)) return NotFound();
            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: /UserManagement/Attribute/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var attribute = await _context.Attributes
            .Include(a => a.UserAttributeValues)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (attribute == null) return NotFound();

        return View(attribute);
    }

    // POST: /UserManagement/Attribute/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var attribute = await _context.Attributes.FindAsync(id);
        if (attribute != null)
        {
            _context.Attributes.Remove(attribute);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task<bool> AttributeExists(int id)
    {
        return await _context.Attributes.AnyAsync(e => e.Id == id);
    }
}
