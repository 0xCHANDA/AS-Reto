using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bib_Hacienda.Clases
{
    public class Venta
    {
        private DateTime fecha;
        private Producto producto;
        private uint monto;
        

       
        public Venta(
            DateTime fecha,
            Producto producto,
            uint monto)
        {
            this.Fecha = fecha;
            this.Producto = producto;
            this.Monto = monto;
        }

        

        public DateTime Fecha
        {
            get => fecha;
            set => fecha = value;
        }

        public Producto Producto
        {
            get => producto;
            set => producto = value;
        }

        public uint Monto
        {
            get => monto;
            set => monto = value;
        }

      
    }
}
