using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PasilloVR_API.Models
{
    public class SesionJuego
    {
        [Key]
        public int ID_Sesion { get; set; }

        [ForeignKey("Jugador")]
        public int FK_ID_Jug { get; set; }
        public DateTime FechaHora { get; set; } = DateTime.Now;
        public string EntornoEjecucion { get; set; } = "Unity Editor (PC Staging)";

        public Jugador? Jugador { get; set; }
    }
}