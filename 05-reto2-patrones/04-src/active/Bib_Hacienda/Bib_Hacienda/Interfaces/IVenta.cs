using Bib_Hacienda.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bib_Hacienda.Interfaces
{
    public interface IVenta<T> where T : Producto 
    {
        //Metodo para vender res
        string vender(IInventario<T> inventario, T producto, uint monto);
    }
}
