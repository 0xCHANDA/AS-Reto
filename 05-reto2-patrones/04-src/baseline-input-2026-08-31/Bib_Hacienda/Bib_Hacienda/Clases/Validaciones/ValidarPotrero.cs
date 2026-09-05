using Bib_Hacienda.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bib_Hacienda.Clases.Validaciones
{
    //Valida objetos de tipo Potrero
    public class ValidadorPotrero : IValidadorPotrero
    {
        public virtual bool ValidarPotrero(Potrero potrero)
        {
            if (potrero == null || string.IsNullOrWhiteSpace(potrero.Identificacion))
            {
                return false;
            }
            return true;
        }
    }
}
