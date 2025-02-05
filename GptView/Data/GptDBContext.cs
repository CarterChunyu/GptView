using GptView.Models;
using Microsoft.EntityFrameworkCore;

namespace GptView.Data
{
    public class GptDBContext : DbContext
    {
        public GptDBContext(DbContextOptions<GptDBContext> options) : base(options)
        {

        }
        
        public DbSet<User> Users { get; set; }
        public DbSet<Function> Functions { get; set; }
        public DbSet<AccessPermission> AccessPermissions { get; set; }
    }
}
