using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Auth;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Data
{
    public class ApplicationDbContext:DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options)
            :base(options)
        {
            
        }

        public DbSet<LocalUser> LocalUsers { get; set; }
        public DbSet<Villa>Villas { get; set; }
        public DbSet<VillaNumber> VillaNumbers { get; set; }

        }
}
