using Bib_Hacienda.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bib_Hacienda.Interfaces
{
    public interface IVenta
    {
        // El inventario y el producto deben ser del mismo tipo concreto. Asi no
        // se promete que un potrero acepte cualquier Producto.
        string vender<T>(IInventario<T> inventario, T producto, uint monto)
            where T : Producto;
    }
}
