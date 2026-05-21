using System;
using System.ComponentModel.DataAnnotations;

namespace PasilloVR_API.Models
{
    public class Jugador
    {
        [Key]
        public int ID_Jug { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}