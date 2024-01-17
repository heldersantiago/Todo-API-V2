using Microsoft.AspNetCore.Identity;

namespace TodoApi.Models;

public class User
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int Email { get; set; }
    public int Password { get; set; }
}