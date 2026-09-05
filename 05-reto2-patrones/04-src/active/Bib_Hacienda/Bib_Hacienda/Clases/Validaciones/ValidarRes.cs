using Bib_Hacienda.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bib_Hacienda.Clases.Validaciones
{
    //Valida objetos de tipo Res
    public class ValidadorRes : IValidadorRes
    {
        public virtual bool ValidarRes(Res res)
        {
            if (res == null || string.IsNullOrWhiteSpace(res.Nombre) || res.Peso <= 0 || res.Edad <= 0)
            {
                return false;
            }
            return true;
        }
    }
}
