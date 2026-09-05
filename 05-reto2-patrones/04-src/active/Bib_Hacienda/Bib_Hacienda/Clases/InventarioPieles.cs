using Bib_Hacienda.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bib_Hacienda.Clases
{
    public class InventarioPieles : IInventario<Piel>
    {
        private readonly List<Piel> pieles = new List<Piel>();

        public IReadOnlyList<Piel> Pieles => pieles;

        public void agregar(Piel producto)
        {
            try
            {
                if (producto == null)
                    throw new ArgumentNullException(nameof(producto));

                if (pieles.Any(p => p.Nombre.Equals(producto.Nombre, StringComparison.OrdinalIgnoreCase)))
                    throw new InvalidOperationException($"La piel '{producto.Nombre}' ya existe en el inventario.");

                pieles.Add(producto);
            }
            catch (Exception er)
            {
                throw new Exception("Error inesperado en el metodo agregar : " + er.Message, er);
            }
        }

        public bool contiene(Piel producto)
        {
            try
            {
                return producto != null && pieles.Any(p => p.Nombre.Equals(producto.Nombre, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception er)
            {
                throw new Exception("Error inesperado en el metodo contiene: " + er.Message, er);
            }
        }

        public Piel retirar(Piel producto)
        {
            try
            {
                if (producto == null)
                    throw new ArgumentNullException(nameof(producto));

                var existente = pieles.FirstOrDefault(p => p.Nombre.Equals(producto.Nombre, StringComparison.OrdinalIgnoreCase));
                if (existente == null)
                    throw new InvalidOperationException("La piel no se encuentra en el inventario.");

                pieles.Remove(existente);
                return existente;
            }
            catch (Exception er)
            {
                throw new Exception("Error inesperado en el metodo retirar: " + er.Message, er);
            }
        }
    }
}
