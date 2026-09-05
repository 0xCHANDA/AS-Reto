using Bib_Hacienda.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bib_Hacienda.Clases
{
    public class InventarioLacteos : IInventario<Lacteo>
    {
        private readonly List<Lacteo> lacteos = new List<Lacteo>();

        public IReadOnlyList<Lacteo> Lacteos => lacteos;

        public void agregar(Lacteo producto)
        {
            try
            {
                if (producto == null)
                    throw new ArgumentNullException(nameof(producto));

                if (lacteos.Any(l => l.Nombre.Equals(producto.Nombre, StringComparison.OrdinalIgnoreCase)))
                    throw new InvalidOperationException($"El lácteo '{producto.Nombre}' ya existe en el inventario.");

                lacteos.Add(producto);
            }
            catch (Exception er)
            {
                throw new Exception("Error inesperado en el metodo agregar: " + er.Message, er);
            }
        }

        public bool contiene(Lacteo lacteo)
        {
            try
            {
                return lacteo != null && lacteos.Any(l => l.Nombre.Equals(lacteo.Nombre, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception er)
            {
                throw new Exception("Error inesperado en el metodo contiene: " + er.Message, er);
            }
        }

        public Lacteo retirar(Lacteo lacteo)
        {
            try
            {
                if (lacteo == null)
                    throw new ArgumentNullException(nameof(lacteo));

                var existente = lacteos.FirstOrDefault(l => l.Nombre.Equals(lacteo.Nombre, StringComparison.OrdinalIgnoreCase));
                if (existente == null)
                    throw new InvalidOperationException("El lácteo no se encuentra en el inventario.");

                lacteos.Remove(existente);
                return existente;
            }
            catch (Exception er)
            {
                throw new Exception("Error inesperado en el metodo retirar: " + er.Message, er);
            }
        }
    }
}
