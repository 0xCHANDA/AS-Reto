using System;
using Bib_Hacienda.Clases;

namespace Caracterizacion
{
    // AS-IS: 03-src/redisenado/HaciendaNEW. La venta usa vender_res y la
    // atenuacion es el enum anidado en Viva.
    internal class Program : IDiferencias
    {
        private static void Main()
        {
            Console.WriteLine("# Caracterizacion AS-IS (03-src/redisenado/HaciendaNEW)");
            Escenarios.Ejecutar(new Program());
        }

        public string CrearVacunaViva(Hacienda h, string nombre, string lote, DateTime venc, DateTime aplic)
            => h.crear_vacuna(nombre, lote, venc, aplic, Viva.enum_l_atenuaciones.Atenuacion10);

        public string CrearLoteVivo(Hacienda h, string nombre, string loteBase, DateTime venc, DateTime aplic, uint cantidad)
            => h.crear_vacuna(nombre, loteBase, venc, aplic, Viva.enum_l_atenuaciones.Atenuacion10, cantidad);

        public string VenderRes(Hacienda h, string potrero, string res, uint monto)
            => h.vender_res(potrero, res, monto);
    }
}
