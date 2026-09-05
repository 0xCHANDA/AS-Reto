using System;
using System.Collections.Generic;
using System.Linq;
using Bib_Hacienda.enums;

namespace Bib_Hacienda.Clases
{
    // Responsabilidad única: crear y añadir vacunas al inventario.
    // Movido desde Hacienda para que la fachada conserve solo coordinación.
    // Sigue siendo una clase concreta simple: no introduce factories,
    // registries ni reflexión. Permanece acoplada a las clases concretas
    // Bacteriana/Viva existentes (deuda consciente: solo hay dos tipos).
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
            try
            {
                ValidarDatosBasicos(nombre, lote, fecha_vencimiento, fecha_aplicacion);

                Bacteriana nueva_vacuna = new Bacteriana(nombre, lote, fecha_vencimiento, fecha_aplicacion, periodo_aplicacion);
                l_vacunas.Add(nueva_vacuna);

                return $"Vacuna bacteriana '{nombre}' del lote '{lote}' agregada al inventario con éxito. Período de aplicación: {periodo_aplicacion} semanas.";
            }
            catch (Exception er)
            {
                throw new Exception("Error inesperado en el método crear_vacuna (bacteriana): " + er.Message);
            }
        }

        // Vacuna viva individual
        public string Crear(string nombre, string lote, DateTime fecha_vencimiento, DateTime fecha_aplicacion, Atenuaciones grado_atenuacion)
        {
            try
            {
                ValidarDatosBasicos(nombre, lote, fecha_vencimiento, fecha_aplicacion);

                Viva nueva_vacuna = new Viva(nombre, lote, fecha_vencimiento, fecha_aplicacion, grado_atenuacion);
                l_vacunas.Add(nueva_vacuna);

                return $"Vacuna viva '{nombre}' del lote '{lote}' agregada al inventario con éxito. Grado de atenuación: {(int)grado_atenuacion}.";
            }
            catch (Exception er)
            {
                throw new Exception("Error inesperado en el método crear_vacuna (viva): " + er.Message);
            }
        }

        // Lote de vacunas bacterianas
        public string CrearLote(string nombre, string lote_base, DateTime fecha_vencimiento, DateTime fecha_aplicacion, uint periodo_aplicacion, uint cantidad)
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

                    Bacteriana nueva_vacuna = new Bacteriana(nombre, lote_numerado, fecha_vencimiento, fecha_aplicacion, periodo_aplicacion);
                    l_vacunas.Add(nueva_vacuna);
                    vacunas_creadas++;
                }

                if (vacunas_creadas == 0)
                    throw new Exception($"No se pudo crear ninguna vacuna. Todos los lotes ya existen en el inventario");

                return $"Lote de vacunas bacterianas creado con éxito:\n" +
                "- Nombre: {nombre}\n" +
                $"- Cantidad creada: {vacunas_creadas} de {cantidad}\n" +
                $"- Lotes: {lote_base}-001 a {lote_base}-{vacunas_creadas:D3}\n" +
                $"- Período de aplicación: {periodo_aplicacion} semanas";
            }
            catch (Exception er)
            {
                throw new Exception("Error inesperado en el método crear_vacuna (lote bacteriano): " + er.Message);
            }
        }

        // Lote de vacunas vivas
        public string CrearLote(string nombre, string lote_base, DateTime fecha_vencimiento, DateTime fecha_aplicacion, Atenuaciones grado_atenuacion, uint cantidad)
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

                    Viva nueva_vacuna = new Viva(nombre, lote_numerado, fecha_vencimiento, fecha_aplicacion, grado_atenuacion);
                    l_vacunas.Add(nueva_vacuna);
                    vacunas_creadas++;
                }

                if (vacunas_creadas == 0)
                    throw new Exception($"No se pudo crear ninguna vacuna. Todos los lotes ya existen en el inventario");

                return $"Lote de vacunas vivas creado con éxito:\n" +
                $"- Nombre: {nombre}\n" +
                $"- Cantidad creada: {vacunas_creadas} de {cantidad}\n" +
                $"- Lotes: {lote_base}-001 a {lote_base}-{vacunas_creadas:D3}\n" +
                $"- Grado de atenuación: {(int)grado_atenuacion}";
            }
            catch (Exception er)
            {
                throw new Exception("Error inesperado en el método crear_vacuna (lote vivo): " + er.Message);
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
