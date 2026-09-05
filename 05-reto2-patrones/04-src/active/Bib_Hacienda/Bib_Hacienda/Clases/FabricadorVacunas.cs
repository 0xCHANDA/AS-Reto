using System;
using System.Collections.Generic;
using System.Linq;
using Bib_Hacienda.Clases.Construccion;
using Bib_Hacienda.enums;
using Bib_Hacienda.Interfaces;

namespace Bib_Hacienda.Clases
{
    // Director del Builder de vacunas: conoce la secuencia comun de creacion
    // (validar, construir, registrar, resumir) y la de lote (numerar, omitir
    // duplicados, contar), pero no que variante se esta creando. Cada variante
    // vive en su IVacunaBuilder.
    public class FabricadorVacunas
    {
        private readonly List<Vacuna> l_vacunas;

        public FabricadorVacunas(List<Vacuna> l_vacunas)
        {
            this.l_vacunas = l_vacunas ?? throw new ArgumentNullException(nameof(l_vacunas));
        }

        public List<Vacuna> L_vacunas => l_vacunas;

        // Vacuna bacteriana individual
        public string Crear(string nombre, string lote, DateTime fecha_vencimiento, DateTime fecha_aplicacion, uint periodo_aplicacion)
        {
            return Crear(new BuilderBacteriana(periodo_aplicacion), nombre, lote, fecha_vencimiento, fecha_aplicacion);
        }

        // Vacuna viva individual
        public string Crear(string nombre, string lote, DateTime fecha_vencimiento, DateTime fecha_aplicacion, Atenuaciones grado_atenuacion)
        {
            return Crear(new BuilderViva(grado_atenuacion), nombre, lote, fecha_vencimiento, fecha_aplicacion);
        }

        // Lote de vacunas bacterianas
        public string CrearLote(string nombre, string lote_base, DateTime fecha_vencimiento, DateTime fecha_aplicacion, uint periodo_aplicacion, uint cantidad)
        {
            return CrearLote(new BuilderBacteriana(periodo_aplicacion), nombre, lote_base, fecha_vencimiento, fecha_aplicacion, cantidad);
        }

        // Lote de vacunas vivas
        public string CrearLote(string nombre, string lote_base, DateTime fecha_vencimiento, DateTime fecha_aplicacion, Atenuaciones grado_atenuacion, uint cantidad)
        {
            return CrearLote(new BuilderViva(grado_atenuacion), nombre, lote_base, fecha_vencimiento, fecha_aplicacion, cantidad);
        }

        // Secuencia comun de creacion individual. Publica para que una variante
        // nueva reutilice al director sin modificarlo.
        public string Crear(IVacunaBuilder builder, string nombre, string lote, DateTime fecha_vencimiento, DateTime fecha_aplicacion)
        {
            try
            {
                ValidarDatosBasicos(nombre, lote, fecha_vencimiento, fecha_aplicacion);

                Vacuna nueva_vacuna = builder.Construir(nombre, lote, fecha_vencimiento, fecha_aplicacion);
                l_vacunas.Add(nueva_vacuna);

                return $"Vacuna {builder.Variante} '{nombre}' del lote '{lote}' agregada al inventario con éxito. {builder.DetalleEspecifico}.";
            }
            catch (Exception er)
            {
                throw new Exception($"Error inesperado en el método crear_vacuna ({builder.Variante}): " + er.Message);
            }
        }

        // Secuencia comun de creacion por lote: numera, omite los lotes que ya
        // existen, cuenta las creadas y resume. La variante solo aporta el
        // producto y las partes propias del mensaje.
        public string CrearLote(IVacunaBuilder builder, string nombre, string lote_base, DateTime fecha_vencimiento, DateTime fecha_aplicacion, uint cantidad)
        {
            try
            {
                ValidarCantidadYLoteBase(nombre, lote_base, fecha_vencimiento, fecha_aplicacion, cantidad);

                int vacunas_creadas = 0;

                for (int i = 1; i <= cantidad; i++)
                {
                    string lote_numerado = $"{lote_base}-{i:D3}";

                    if (l_vacunas.Any(v => v.Lote.Equals(lote_numerado, StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }

                    l_vacunas.Add(builder.Construir(nombre, lote_numerado, fecha_vencimiento, fecha_aplicacion));
                    vacunas_creadas++;
                }

                if (vacunas_creadas == 0)
                    throw new Exception($"No se pudo crear ninguna vacuna. Todos los lotes ya existen en el inventario");

                return $"Lote de vacunas {builder.VariantePlural} creado con éxito:\n" +
                $"- Nombre: {builder.NombreEnResumenDeLote(nombre)}\n" +
                $"- Cantidad creada: {vacunas_creadas} de {cantidad}\n" +
                $"- Lotes: {lote_base}-001 a {lote_base}-{vacunas_creadas:D3}\n" +
                $"- {builder.DetalleEspecifico}";
            }
            catch (Exception er)
            {
                throw new Exception($"Error inesperado en el método crear_vacuna (lote {builder.VarianteLote}): " + er.Message);
            }
        }

        // Validaciones comunes extraídas para eliminar duplicación entre las
        // cuatro sobrecargas y centralizar las reglas de dominio.
        private void ValidarDatosBasicos(string nombre, string lote, DateTime fecha_vencimiento, DateTime fecha_aplicacion)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre de la vacuna no puede estar vacío", nameof(nombre));

            if (string.IsNullOrWhiteSpace(lote))
                throw new ArgumentException("El lote de la vacuna no puede estar vacío", nameof(lote));

            if (fecha_vencimiento <= fecha_aplicacion)
                throw new Exception("La fecha de vencimiento debe ser posterior a la fecha de aplicación");

            if (l_vacunas.Any(v => v.Lote.Equals(lote, StringComparison.OrdinalIgnoreCase)))
                throw new Exception($"Ya existe una vacuna con el lote '{lote}' en el inventario");
        }

        private void ValidarCantidadYLoteBase(string nombre, string lote_base, DateTime fecha_vencimiento, DateTime fecha_aplicacion, uint cantidad)
        {
            if (cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a 0", nameof(cantidad));

            if (cantidad > 100)
                throw new ArgumentException("No se pueden crear más de 100 vacunas en un solo lote", nameof(cantidad));

            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre de la vacuna no puede estar vacío", nameof(nombre));

            if (string.IsNullOrWhiteSpace(lote_base))
                throw new ArgumentException("El lote base no puede estar vacío", nameof(lote_base));

            if (fecha_vencimiento <= fecha_aplicacion)
                throw new Exception("La fecha de vencimiento debe ser posterior a la fecha de aplicación");
        }
    }
}
