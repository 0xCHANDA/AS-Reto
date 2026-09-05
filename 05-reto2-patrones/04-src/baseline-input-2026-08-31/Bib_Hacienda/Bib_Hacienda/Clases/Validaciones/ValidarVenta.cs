using Bib_Hacienda.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bib_Hacienda.Clases.Validaciones
{
    // Valida objetos de tipo Venta
    public class ValidadorVenta : IValidadorVenta
    {
        public virtual bool ValidarVenta(Venta venta)
        {
            if (venta == null || venta.Monto <= 0)
            {
                return false;
            }

            // Venta de cualquier Producto: unico formato del modelo actual.
            if (venta.Producto != null)
            {
                return true;
            }

            return false;
        }
    }
}
