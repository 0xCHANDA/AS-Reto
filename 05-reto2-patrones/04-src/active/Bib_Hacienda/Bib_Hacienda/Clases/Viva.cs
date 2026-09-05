using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bib_Hacienda.enums;

namespace Bib_Hacienda.Clases
{
    public class Viva : Vacuna //Hereda de Vacuna
    {

        
        //Atributos
        private Atenuaciones periodo_atenuacion;

        //Constructor
        public Viva(string nombre, string lote, DateTime fecha_vencimiento, DateTime fecha_aplicacion, Atenuaciones periodo_atenuacion) : base(nombre, lote, fecha_vencimiento, fecha_aplicacion)
        {
            this.periodo_atenuacion = periodo_atenuacion;
        }

        public override bool PuedeAplicarseA(Res res)
        {
            int cantidad = res.L_vacunas_aplicadas
                .OfType<Viva>()
                .Count();

            return cantidad < res.MaxVacunasVivas;
        }

        public override TipoVacuna Tipo =>
          TipoVacuna.Viva;
    }
}
