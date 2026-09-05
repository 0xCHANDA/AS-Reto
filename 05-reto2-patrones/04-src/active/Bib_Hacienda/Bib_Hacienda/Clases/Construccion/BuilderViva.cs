using Bib_Hacienda.enums;
using Bib_Hacienda.Interfaces;
using System;

namespace Bib_Hacienda.Clases.Construccion
{
    public class BuilderViva : IVacunaBuilder
    {
        private readonly Atenuaciones grado_atenuacion;

        public BuilderViva(Atenuaciones grado_atenuacion)
        {
            this.grado_atenuacion = grado_atenuacion;
        }

        public string Variante => "viva";

        public string VariantePlural => "vivas";

        public string VarianteLote => "vivo";

        public string DetalleEspecifico => $"Grado de atenuación: {(int)grado_atenuacion}";

        public string NombreEnResumenDeLote(string nombre) => nombre;

        public Vacuna Construir(string nombre, string lote, DateTime fecha_vencimiento, DateTime fecha_aplicacion)
        {
            return new Viva(nombre, lote, fecha_vencimiento, fecha_aplicacion, grado_atenuacion);
        }
    }
}
