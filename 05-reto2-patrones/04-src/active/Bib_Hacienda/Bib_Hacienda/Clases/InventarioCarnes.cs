using Bib_Hacienda.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bib_Hacienda.Clases
{
    public class InventarioCarnes : IInventario<Carne>
    {
        private readonly List<Carne> carnes = new List<Carne>();

        public IReadOnlyList<Carne> Carnes => carnes;

        public void agregar(Carne producto)
        {
            try
            {
                if (producto == null)
                    throw new ArgumentNullException(nameof(producto));

                if (carnes.Any(c => c.Nombre.Equals(producto.Nombre, StringComparison.OrdinalIgnoreCase)))
                    throw new InvalidOperationException($"La carne '{producto.Nombre}' ya existe en el inventario.");

                carnes.Add(producto);
            }
            catch (Exception er)
            {
                throw new Exception("Error inesperado en el metodo agregar: " + er.Message, er);
            }
        }

        public bool contiene(Carne producto)
        {
            try
            {
                return producto != null && carnes.Any(c => c.Nombre.Equals(producto.Nombre, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception er)
            {
                throw new Exception("Error inesperado en el metodo contiene: " + er.Message, er);
            }
        }

        public Carne retirar(Carne producto)
        {
            try
            {
                if (producto == null)
                    throw new ArgumentNullException(nameof(producto));

                var existente = carnes.FirstOrDefault(c => c.Nombre.Equals(producto.Nombre, StringComparison.OrdinalIgnoreCase));
                if (existente == null)
                    throw new InvalidOperationException("La carne no se encuentra en el inventario.");

                carnes.Remove(existente);
                return existente;
            }
            catch (Exception er)
            {
                throw new Exception("Error inesperado en el metodo retirar: " + er.Message, er);
            }
        }
    }
}
