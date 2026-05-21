using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PasilloVR_API.Models
{
    public class HistorialEvento
    {
        [Key]
        public int ID_Evento { get; set; }

        [ForeignKey("SesionJuego")]
        public int FK_ID_Sesion { get; set; }
        public double TimeStamp_Segundos { get; set; }
        public string TipoEvento { get; set; } = string.Empty;
        public string? NombreEstresor { get; set; }
        public double AnsiedadActual { get; set; }
        public double Pos_X { get; set; }
        public double Pos_Y { get; set; }
        public double Pos_Z { get; set; }

        public SesionJuego? SesionJuego { get; set; }
    }
}