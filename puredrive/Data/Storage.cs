using Microsoft.AspNetCore.Components;
using puredrive.Models;

namespace puredrive.Data
{
    public static class Storage
    {
        public static int GroupID { get; set; }
        public static User User { get; set; } = new User();

        public static int FileID { get; set; }
        public static Models.File File { get; set; } = new Models.File(0);

        public static MarkupString Document { get; set; } = new MarkupString();
    }
}
