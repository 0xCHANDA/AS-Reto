using Bib_Hacienda.Interfaces;
using System;

namespace Bib_Hacienda.Clases.Construccion
{
    public class BuilderBacteriana : IVacunaBuilder
    {
        private readonly uint periodo_aplicacion;

        // El periodo no se valida aqui: lo valida Bacteriana al construirse, y
        // adelantar esa comprobacion cambiaria el orden de los errores.
        public BuilderBacteriana(uint periodo_aplicacion)
        {
            this.periodo_aplicacion = periodo_aplicacion;
        }

        public string Variante => "bacteriana";

        public string VariantePlural => "bacterianas";

        public string VarianteLote => "bacteriano";

        public string DetalleEspecifico => $"Período de aplicación: {periodo_aplicacion} semanas";

        // El resumen de lote bacteriano imprime el marcador literal en vez del
        // nombre. Es salida observable del sistema y se conserva sin cambios.
        public string NombreEnResumenDeLote(string nombre) => "{nombre}";

        public Vacuna Construir(string nombre, string lote, DateTime fecha_vencimiento, DateTime fecha_aplicacion)
        {
            return new Bacteriana(nombre, lote, fecha_vencimiento, fecha_aplicacion, periodo_aplicacion);
        }
    }
}
