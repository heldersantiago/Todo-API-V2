using Microsoft.AspNetCore.Identity;

namespace TodoApi.Models;

public class TodoItem
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public bool Complete { get; set; }
    public required string OwnerId { get; set; }
}