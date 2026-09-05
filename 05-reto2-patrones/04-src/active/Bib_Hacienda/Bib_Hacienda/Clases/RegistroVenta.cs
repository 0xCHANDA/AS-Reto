using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bib_Hacienda.Clases
{
    public class RegistroVenta
    {
        private readonly List<Venta> ventas = new List<Venta>();

        public IReadOnlyList<Venta> Ventas => ventas;
                
        public void registrar(Venta venta)
        {
            if (venta == null)
                throw new ArgumentNullException(nameof(venta));

            ventas.Add(venta);
        }
    }
}
