using Bib_Hacienda.Reglas;
using System;

namespace Bib_Hacienda.Clases
{
    public class Cebon : Res //Hereda de Res
    {
        //Constructor: valida el rango propio antes de delegar al constructor base.
        public Cebon(string nombre, uint peso, ushort edad) : base(nombre, peso, ValidarEdadInicial(edad))
        {
        }

        private static ushort ValidarEdadInicial(ushort edad)
        {
            if (edad <= ReglaRes.edad_max_ternero || edad > ReglaRes.edad_max_cebon)
                throw new Exception("El cebon excedió la edad maxima");

            return edad;
        }

        protected override void ValidarEdad(ushort edad)
        {
            if (edad <= ReglaRes.edad_max_ternero || edad > ReglaRes.edad_max_cebon)
                throw new Exception("El cebon excedió la edad maxima");
        }

        public override byte MaxVacunasBacterianas =>
            ReglaVacuna.max_bac_cebon;

        public override byte MaxVacunasVivas =>
            ReglaVacuna.max_viv_cebon;
    }
}
