using System;
using Bib_Hacienda.Clases;
using Bib_Hacienda.enums;
using p_mvcHacienda.Servicios;

namespace Caracterizacion
{
    // TO-BE: 05-reto2-patrones/04-src/active. La venta usa vender<T> y la
    // atenuacion es el enum propio Atenuaciones.
    internal class Program : IDiferencias
    {
        private static void Main()
        {
            Console.WriteLine("# Caracterizacion TO-BE (05-reto2-patrones/04-src/active)");
            Escenarios.Ejecutar(new Program());
        }

        public string CrearVacunaViva(Hacienda h, string nombre, string lote, DateTime venc, DateTime aplic)
            => h.crear_vacuna(nombre, lote, venc, aplic, Atenuaciones.Atenuacion10);

        public string CrearLoteVivo(Hacienda h, string nombre, string loteBase, DateTime venc, DateTime aplic, uint cantidad)
            => h.crear_vacuna(nombre, loteBase, venc, aplic, Atenuaciones.Atenuacion10, cantidad);

        public string VenderRes(Hacienda h, string potrero, string res, uint monto)
        {
            var potreroObj = h.buscar_potrero(potrero);
            var resObj = potreroObj.buscar_res(res);
            return h.vender(potreroObj, resObj, monto);
        }
    }
}
