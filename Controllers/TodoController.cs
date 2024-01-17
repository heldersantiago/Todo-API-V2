using TodoApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;

namespace TodoApi.Controllers;

[Authorize]
[EnableCors("CorsPolicy")]
[ApiController]
[Route("api/[controller]")]
public class TodoController : ControllerBase
{
    private readonly ILogger<TodoController> _logger;

    private readonly TodoContext _TodoContext;

    public TodoController(ILogger<TodoController> logger, TodoContext todoContext)
    {
        _logger = logger;
        this._TodoContext = todoContext;
    }

    [HttpGet]
    public async Task<ActionResult<TodoItem>> GetAllAsync()
    {
        var items = await _TodoContext.TodoItems.ToListAsync();
        if (items == null) return BadRequest();

        var response = new ObjectResult(items)
        {
            StatusCode = 200,
            Value = new
            {
                Message = "lista das tarefas",
                Data = items
            },
        };
        return response;
    }

    [HttpPost]
    public async Task<TodoItem> CreateAsync([FromBody] TodoItem item)
    {
        _TodoContext.Add(item);
        await _TodoContext.SaveChangesAsync();
        return item;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TodoItem>> ShowAsync(int id)
    {
        if (id <= 0) return BadRequest($"argumento invalido: {id}");
        var item = await _TodoContext.FindAsync<TodoItem>(id);

        if (item == null) return NotFound($"tarefa com id: {id} nao encontrado");

        var response = new ObjectResult(item)
        {
            StatusCode = 200,
            Value = new
            {
                Message = "tarefa achado com sucesso",
                Data = item
            },
        };
        return response;
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TodoItem>> UpdateAsync(int id, [FromBody] TodoItem _item)
    {
        if (id <= 0 || _item == null)
        {
            return BadRequest($"argumento invalido: {id}");
        }

        var existingItem = await _TodoContext.FindAsync<TodoItem>(id);

        if (existingItem == null)
        {
            return NotFound();
        }

        if (id != existingItem.Id)
        {
            return BadRequest($"tarefa com id: {id} nao encontrado");
        }

        // Update properties of existingItem with values from _item
        existingItem.Name = _item.Name;

        try
        {
            _TodoContext.Update(existingItem);
            await _TodoContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            // Handle concurrency conflict
            return Conflict("Concurrency conflict");
        }

        var response = new ObjectResult(existingItem)
        {
            StatusCode = 200,
            Value = new
            {
                Message = "tarefa actualizado com sucesso",
                Data = existingItem
            },
        };
        return response;
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<TodoItem>> DeleteAsync(int id)
    {
        if (id <= 0) return BadRequest($"argumento invalido: {id}");
        var item = await _TodoContext.FindAsync<TodoItem>(id);
        if (item == null) return NotFound();

        _TodoContext.Remove(item);
        await _TodoContext.SaveChangesAsync();

        var response = new ObjectResult(item)
        {
            StatusCode = 200,
            Value = new
            {
                Message = "tarefa deletado com sucesso",
                Data = item
            },
        };
        return response;
    }

}
