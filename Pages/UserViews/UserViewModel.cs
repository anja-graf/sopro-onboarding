// DELETE THIS FILE IF NO USAGE

using Microsoft.AspNetCore.Mvc.RazorPages;
using Replay.Model;

namespace Replay.Pages.UserViews;

public class UserViewModel
{
    public string Name { get; set; }
    public bool IsBlocked { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public List<Role> Roles { get; set; }
}