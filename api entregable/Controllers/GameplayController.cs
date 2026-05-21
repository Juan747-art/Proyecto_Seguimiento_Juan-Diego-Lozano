using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasilloVR_API.Data;
using PasilloVR_API.Models;

namespace PasilloVR_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameplayController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GameplayController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. ENDPOINT: Registrar o Validar Estudiante
        [HttpPost("registrar-jugador")]
        public async Task<IActionResult> RegistrarJugador([FromBody] Jugador nuevoJugador)
        {
            var jugadorExistente = await _context.Jugadores
                .FirstOrDefaultAsync(j => j.Codigo == nuevoJugador.Codigo);

            if (jugadorExistente != null)
            {
                return Ok(jugadorExistente); // Retorna el estudiante si ya existe
            }

            _context.Jugadores.Add(nuevoJugador);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(RegistrarJugador), nuevoJugador);
        }

        // 2. ENDPOINT: Iniciar una Nueva Sesión (Al darle Play en Unity PC)
        [HttpPost("iniciar-sesion")]
        public async Task<IActionResult> IniciarSesion([FromBody] SesionJuego nuevaSesion)
        {
            _context.SesionesJuego.Add(nuevaSesion);
            await _context.SaveChangesAsync();
            return Ok(nuevaSesion);
        }

        // 3. ENDPOINT: Recibir Evento de Telemetría Dinámica (Event Sourcing)
        [HttpPost("guardar-evento")]
        public async Task<IActionResult> GuardarEvento([FromBody] HistorialEvento nuevoEvento)
        {
            if (nuevoEvento.FK_ID_Sesion <= 0)
            {
                return BadRequest("ID de sesión no válido.");
            }

            _context.HistorialEventos.Add(nuevoEvento);
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Evento dinámico registrado de forma forense.", id = nuevoEvento.ID_Evento });
        }
    }
}