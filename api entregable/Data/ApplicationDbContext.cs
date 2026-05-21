using Microsoft.EntityFrameworkCore;
using PasilloVR_API.Models;

namespace PasilloVR_API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Jugador> Jugadores { get; set; }
        public DbSet<SesionJuego> SesionesJuego { get; set; }
        public DbSet<HistorialEvento> HistorialEventos { get; set; }
    }
}