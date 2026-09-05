using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bib_Hacienda.Clases
{
    public abstract class Producto
    {
        private string nombre;

        protected Producto(string nombre)
        {
            Nombre = nombre;
        }

        public string Nombre
        {
            get => nombre;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException(
                        "El nombre del producto no puede estar vacío.",
                        nameof(value));

                nombre = value;
            }
        }
    }
}
