using GestaoDeCantina.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace GestaoDeCantina.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<Prato> Pratos => Set<Prato>();
        public DbSet<Senha> Senhas => Set<Senha>();
    }
}
