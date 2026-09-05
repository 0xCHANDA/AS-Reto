using System;

namespace Bib_Hacienda.Clases
{
    public class EventoClinico
    {
        public EventoClinico(DateTime fecha, string concepto, string observacion)
        {
            if (string.IsNullOrWhiteSpace(concepto))
                throw new ArgumentException("El concepto del evento clinico es obligatorio.", nameof(concepto));

            if (string.IsNullOrWhiteSpace(observacion))
                throw new ArgumentException("La observacion del evento clinico es obligatoria.", nameof(observacion));

            Fecha = fecha;
            Concepto = concepto;
            Observacion = observacion;
        }

        public DateTime Fecha { get; }
        public string Concepto { get; }
        public string Observacion { get; }
    }
}
