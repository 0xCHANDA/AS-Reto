using Bib_Hacienda.Clases;
using System;

namespace Bib_Hacienda.Interfaces
{
    // Builder de la jerarquia de Vacuna. Cada builder concreto se construye con
    // el dato propio de su variante y aporta las partes que distinguen a esa
    // variante; la secuencia comun (validar, numerar, registrar, resumir) queda
    // en el director y no se repite aqui.
    public interface IVacunaBuilder
    {
        // Nombre de la variante en singular, tal como aparece en los mensajes.
        string Variante { get; }

        // Nombre en plural, usado en el encabezado del resumen de lote.
        string VariantePlural { get; }

        // Forma masculina que acompaña a "lote" en los mensajes de error.
        string VarianteLote { get; }

        // Dato propio de la variante ya formateado para el mensaje de salida.
        string DetalleEspecifico { get; }

        // Nombre tal como debe aparecer en el resumen de lote. Existe porque las
        // dos variantes no lo imprimen igual y ese texto es observable.
        string NombreEnResumenDeLote(string nombre);

        Vacuna Construir(string nombre, string lote, DateTime fecha_vencimiento, DateTime fecha_aplicacion);
    }
}
