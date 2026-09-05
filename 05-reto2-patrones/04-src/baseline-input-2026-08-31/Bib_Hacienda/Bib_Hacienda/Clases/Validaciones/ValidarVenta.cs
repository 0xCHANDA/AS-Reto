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

            // Formato legacy: venta de una Res en un Potrero
            if (venta.Potrero != null && venta.Res != null)
            {
                return true;
            }

            // Formato TO-BE: venta de cualquier Producto
            if (venta.Producto != null)
            {
                return true;
            }

            return false;
        }
    }
}
