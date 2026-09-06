using System;
using Bib_Hacienda.Clases;
using static Bib_Hacienda.Clases.Potrero;

namespace Caracterizacion
{
    // Las tres operaciones cuya firma difiere entre el AS-IS y el TO-BE.
    // Todo lo demas se invoca igual en los dos arboles.
    public interface IDiferencias
    {
        string CrearVacunaViva(Hacienda h, string nombre, string lote, DateTime venc, DateTime aplic);
        string CrearLoteVivo(Hacienda h, string nombre, string loteBase, DateTime venc, DateTime aplic, uint cantidad);
        string VenderRes(Hacienda h, string potrero, string res, uint monto);
    }

    // Recorre los mismos escenarios en los dos arboles y emite una linea por caso
    // con el formato  Cnn|ESTADO|salida|estado_final.
    public static class Escenarios
    {
        static readonly DateTime Aplic = new DateTime(2026, 1, 10);
        static readonly DateTime Venc  = new DateTime(2027, 1, 10);
        static readonly DateTime Vencida = new DateTime(2020, 1, 1);

        public static void Ejecutar(IDiferencias d)
        {
            Caso("C01", () => { var h = new Hacienda();
                return (h.crear_potrero("P1", l_tipos_potreros.ternero), $"potreros={h.L_potreros.Count}"); });

            Caso("C02", () => { var h = new Hacienda(); h.crear_potrero("P1", l_tipos_potreros.ternero);
                return (h.crear_potrero("p1", l_tipos_potreros.ternero), $"potreros={h.L_potreros.Count}"); });

            Caso("C03", () => { var h = Con("P1", l_tipos_potreros.ternero);
                var m = h.anadir_res_potrero("P1", "Lola", 8, 100);
                return (m, $"reses={h.buscar_potrero("P1").L_reses.Count};tipo={h.buscar_potrero("P1").L_reses[0].GetType().Name}"); });

            Caso("C04", () => { var h = Con("P1", l_tipos_potreros.ternero);
                var m = h.anadir_res_potrero("P1", "Vieja", 60, 500);
                return (m, $"reses={h.buscar_potrero("P1").L_reses.Count}"); });

            Caso("C05", () => { var h = Con("PC", l_tipos_potreros.cebon);
                var m = h.anadir_res_potrero("PC", "Manchas", 30, 300);
                return (m, $"tipo={h.buscar_potrero("PC").L_reses[0].GetType().Name}"); });

            Caso("C06", () => { var h = Con("PN", l_tipos_potreros.novillo);
                var m = h.anadir_res_potrero("PN", "Toro", 60, 500);
                return (m, $"tipo={h.buscar_potrero("PN").L_reses[0].GetType().Name}"); });

            Caso("C07", () => { var h = ConRes("P1", l_tipos_potreros.ternero, "Lola", 8, 100);
                var m = h.alimentar_res("P1", "Lola", 1);
                return (m, $"peso={h.buscar_potrero("P1").buscar_res("Lola").Peso}"); });

            Caso("C08", () => { var h = new Hacienda();
                return (h.crear_vacuna("Bovina", "L1", Venc, Aplic, 4u), $"vacunas={h.L_vacunas.Count}"); });

            Caso("C09", () => { var h = new Hacienda(); h.crear_vacuna("Bovina", "L1", Venc, Aplic, 4u);
                return (h.crear_vacuna("Otra", "l1", Venc, Aplic, 4u), $"vacunas={h.L_vacunas.Count}"); });

            Caso("C10", () => { var h = new Hacienda();
                return (d.CrearVacunaViva(h, "Viral", "V1", Venc, Aplic), $"vacunas={h.L_vacunas.Count}"); });

            Caso("C11", () => { var h = new Hacienda();
                return (h.crear_vacuna("Lote", "LB", Venc, Aplic, 4u, 3u), $"vacunas={h.L_vacunas.Count}"); });

            Caso("C12", () => { var h = new Hacienda();
                return (d.CrearLoteVivo(h, "LoteV", "LV", Venc, Aplic, 2u), $"vacunas={h.L_vacunas.Count}"); });

            Caso("C13", () => { var h = new Hacienda();
                return (h.crear_vacuna("Periodo", "LP", Venc, Aplic, 9u), $"vacunas={h.L_vacunas.Count}"); });

            Caso("C14", () => { var h = ConRes("P1", l_tipos_potreros.ternero, "Lola", 8, 100);
                h.crear_vacuna("Bovina", "L1", Venc, Aplic, 4u);
                var m = h.aplicar_vacuna(h.L_vacunas[0], "Lola", "P1");
                return (m, $"inventario={h.L_vacunas.Count};aplicadas={h.buscar_potrero("P1").buscar_res("Lola").L_vacunas_aplicadas.Count}"); });

            Caso("C15", () => { var h = ConRes("P1", l_tipos_potreros.ternero, "Lola", 8, 100);
                h.crear_vacuna("Vencida", "LX", Vencida, Aplic, 4u);
                var m = h.aplicar_vacuna(h.L_vacunas[0], "Lola", "P1");
                return (m, $"aplicadas={h.buscar_potrero("P1").buscar_res("Lola").L_vacunas_aplicadas.Count}"); });

            Caso("C16", () => { var h = ConRes("P1", l_tipos_potreros.ternero, "Lola", 8, 100);
                h.crear_vacuna("Bovina", "L1", Venc, Aplic, 4u);
                h.aplicar_vacuna(h.L_vacunas[0], "Lola", "P1");
                h.crear_vacuna("Bovina", "L2", Venc, Aplic, 4u);
                var m = h.aplicar_vacuna(h.L_vacunas[0], "Lola", "P1");
                return (m, $"aplicadas={h.buscar_potrero("P1").buscar_res("Lola").L_vacunas_aplicadas.Count}"); });

            Caso("C17", () => { var h = ConRes("P1", l_tipos_potreros.ternero, "Lola", 8, 100);
                for (int i = 1; i <= 4; i++) {
                    h.crear_vacuna("Bac" + i, "LB" + i, Venc, Aplic, 4u);
                    try { h.aplicar_vacuna(h.L_vacunas[0], "Lola", "P1"); } catch (Exception e) { return (e.Message, $"aplicadas={h.buscar_potrero("P1").buscar_res("Lola").L_vacunas_aplicadas.Count}"); }
                }
                return ("sin excepcion", "-"); });

            Caso("C18", () => { var h = ConRes("P1", l_tipos_potreros.ternero, "Lola", 8, 100);
                var m = d.VenderRes(h, "P1", "Lola", 1200u);
                return (m, $"reses={h.buscar_potrero("P1").L_reses.Count};ventas={h.L_ventas.Count}"); });

            Caso("C19", () => { var h = ConRes("P1", l_tipos_potreros.ternero, "Gorda", 8, 400);
                var m = h.alimentar_res("P1", "Gorda", 10);
                return (m, $"peso={h.buscar_potrero("P1").buscar_res("Gorda").Peso}"); });

            Caso("C20", () => { var h = ConRes("P1", l_tipos_potreros.ternero, "Lola", 8, 100);
                var a = h.alimentar_res("P1", "Lola", 1);
                var b = h.alimentar_res("P1", "Lola", 1);
                return (b, $"iguales={(a.Split('\n').Length == b.Split('\n').Length)}"); });
        }

        static Hacienda Con(string id, l_tipos_potreros t)
        { var h = new Hacienda(); h.crear_potrero(id, t); return h; }

        static Hacienda ConRes(string id, l_tipos_potreros t, string res, ushort edad, uint peso)
        { var h = Con(id, t); h.anadir_res_potrero(id, res, edad, peso); return h; }

        static void Caso(string id, Func<(string, string)> f)
        {
            try { var (salida, estado) = f();
                Console.WriteLine($"{id}|OK|{Plano(salida)}|{estado}"); }
            catch (Exception e) {
                Console.WriteLine($"{id}|EXCEPTION|{e.GetType().Name}:{Plano(e.Message)}|-"); }
        }

        static string Plano(string s) => (s ?? "").Replace("\r", "").Replace("\n", "\\n").Trim();
    }
}
