using Bib_Hacienda.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bib_Hacienda.Clases.Creacion
{
    // Registro de creadores de res. Resolver por capacidad declarada, y no por
    // un condicional sobre el tipo, es lo que evita que una categoria nueva
    // obligue a modificar a los clientes: basta registrar su creador.
    public class CatalogoCreadoresRes
    {
        private readonly List<ICreadorRes> creadores;

        public CatalogoCreadoresRes(IEnumerable<ICreadorRes> creadores)
        {
            if (creadores == null)
                throw new ArgumentNullException(nameof(creadores));

            this.creadores = creadores.ToList();

            if (this.creadores.Count == 0)
                throw new ArgumentException("El catalogo necesita al menos un creador de res.", nameof(creadores));
        }

        // Composicion por defecto para los consumidores que no pasan por la raiz
        // de composicion de la aplicacion web.
        public static CatalogoCreadoresRes PorDefecto()
        {
            return new CatalogoCreadoresRes(new ICreadorRes[]
            {
                new CreadorTernero(),
                new CreadorCebon(),
                new CreadorNovillo()
            });
        }

        public ICreadorRes ParaEdad(ushort edad)
        {
            ICreadorRes creador = creadores.FirstOrDefault(c => c.AplicaA(edad));

            if (creador == null)
                throw new InvalidOperationException($"Ninguna categoría de res cubre una edad de {edad} meses.");

            return creador;
        }

        // Devuelve null cuando la categoria no esta registrada: la politica de
        // respaldo pertenece a quien lee el dato, no al catalogo.
        public ICreadorRes ParaCategoria(string categoria)
        {
            if (string.IsNullOrWhiteSpace(categoria))
                return null;

            return creadores.FirstOrDefault(c =>
                c.Categoria.Equals(categoria.Trim(), StringComparison.OrdinalIgnoreCase));
        }
    }
}
