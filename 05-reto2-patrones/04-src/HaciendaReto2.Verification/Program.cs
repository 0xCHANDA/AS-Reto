using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bib_Hacienda.Clases;
using Bib_Hacienda.Clases.Construccion;
using Bib_Hacienda.Clases.Creacion;
using Bib_Hacienda.Eventos;
using Bib_Hacienda.enums;
using Bib_Hacienda.Interfaces;
using Bib_Hacienda.Reglas;
using static Bib_Hacienda.Clases.Potrero;

namespace HaciendaReto2.Verification
{
    // Caracterizacion del dominio Hacienda antes de introducir los patrones
    // aprobados en A2 (Factory Method, Builder, Observer). Los valores esperados
    // se tomaron ejecutando el baseline estabilizado, no de la documentacion.
    internal class Program
    {
        private static int _fallos = 0;
        private static int _checks = 0;

        private static readonly DateTime Aplicacion = new DateTime(2026, 1, 10);
        private static readonly DateTime Vencimiento = new DateTime(2027, 1, 10);

        private static void Main(string[] args)
        {
            Console.WriteLine("=== HaciendaReto2.Verification ===");

            // --- Creacion de reses (P-01, Factory Method) ---
            CrearResPorCategoriaSegunEdad();
            FronterasDeEdadEntreCategorias();
            SubtipoRechazaEdadFueraDeSuRango();
            PotreroRechazaEdadQueNoCorresponde();
            PotreroRechazaResDuplicada();
            PotreroRechazaResCuandoEstaLleno();
            CatalogoResuelveCreadorPorEdadYPorCategoria();
            CatalogoAdmiteCategoriaNuevaSinTocarClientes();

            // --- Construccion de vacunas (P-04, Builder) ---
            CrearBacterianaIndividual();
            CrearVivaIndividual();
            CrearLoteBacteriano();
            CrearLoteVivo();
            LoteBacterianoConservaLiteralNombreSinInterpolar();
            LoteOmiteDuplicadosYCuentaSoloLasCreadas();
            LoteCompletamenteDuplicadoFalla();
            PeriodoBacterianoFueraDeRangoFalla();
            FechaDeVencimientoNoPosteriorFalla();
            LoteIndividualDuplicadoFalla();
            CantidadDeLoteFueraDeRangoFalla();
            DirectorAdmiteVarianteNuevaSinModificarse();

            // --- Eventos y suscripciones (P-03, Observer) ---
            AlimentarInformaDesnutricion();
            AlimentarInformaAptaParaVenta();
            AlimentarSinAvisoNoAgregaLineas();
            AlimentarRepetidoNoDuplicaMensajes();
            AplicarVacunaInformaEsquemaIncompleto();
            AplicarVacunaInformaEsquemaCompleto();

            SuscripcionNoCreceConLasLlamadas();
            CapturaConservaElOrdenDeEmision();
            CapturaAislaOperacionesEntreSi();

            // --- SC-3: historia clinica ---
            ResTieneHistoriaClinicaInicializada();
            HistoriaClinicaIniciaSinRegistros();
            VacunaAplicadaSeRegistraUnaVezEnHistoria();
            VacunaDuplicadaNoAlteraHistoria();
            VacunaVencidaNoAlteraHistoria();
            ResesTienenHistoriasIndependientes();
            FachadaLegacyDeVacunasComparteLaHistoria();
            HistoriaClinicaNoPuedeCompartirseEntreReses();

            Console.WriteLine($"Verificaciones ejecutadas: {_checks}");
            if (_fallos == 0)
            {
                Console.WriteLine("TODAS LAS VERIFICACIONES PASARON.");
                Environment.Exit(0);
            }
            else
            {
                Console.WriteLine($"FALLARON {_fallos} VERIFICACIONES.");
                Environment.Exit(1);
            }
        }

        // ------------------------------------------------------------------
        // Creacion de reses
        // ------------------------------------------------------------------

        private static void CrearResPorCategoriaSegunEdad()
        {
            var hacienda = new Hacienda();

            AssertEqual(
                "El potrero P-T se a añadido a la hacienda. ",
                hacienda.crear_potrero("P-T", l_tipos_potreros.ternero),
                "crear_potrero devuelve el mensaje de alta");

            hacienda.crear_potrero("P-C", l_tipos_potreros.cebon);
            hacienda.crear_potrero("P-N", l_tipos_potreros.novillo);

            AssertEqual(
                "La res Lucero ha sido añadida al potrero P-T con exito.",
                hacienda.anadir_res_potrero("P-T", "Lucero", 6, 160),
                "anadir_res_potrero devuelve el mensaje de alta");

            hacienda.anadir_res_potrero("P-C", "Manchas", 30, 300);
            hacienda.anadir_res_potrero("P-N", "Toro", 60, 500);

            Assert(hacienda.buscar_potrero("P-T").L_reses.Single() is Ternero,
                "una res de 6 meses se crea como Ternero");
            Assert(hacienda.buscar_potrero("P-C").L_reses.Single() is Cebon,
                "una res de 30 meses se crea como Cebon");
            Assert(hacienda.buscar_potrero("P-N").L_reses.Single() is Novillo,
                "una res de 60 meses se crea como Novillo");
        }

        // Las fronteras de edad son la regla que el Factory Method debe conservar
        // exactamente: 12 sigue siendo Ternero y 48 sigue siendo Cebon.
        private static void FronterasDeEdadEntreCategorias()
        {
            var hacienda = new Hacienda();
            hacienda.crear_potrero("F-T", l_tipos_potreros.ternero);
            hacienda.crear_potrero("F-C", l_tipos_potreros.cebon);
            hacienda.crear_potrero("F-N", l_tipos_potreros.novillo);

            hacienda.anadir_res_potrero("F-T", "Borde12", ReglaRes.edad_max_ternero, 200);
            hacienda.anadir_res_potrero("F-C", "Borde13", (ushort)(ReglaRes.edad_max_ternero + 1), 300);
            hacienda.anadir_res_potrero("F-C", "Borde48", ReglaRes.edad_max_cebon, 300);
            hacienda.anadir_res_potrero("F-N", "Borde49", (ushort)(ReglaRes.edad_max_cebon + 1), 500);

            Assert(BuscarRes(hacienda, "F-T", "Borde12") is Ternero,
                "edad 12 (edad_max_ternero) sigue siendo Ternero");
            Assert(BuscarRes(hacienda, "F-C", "Borde13") is Cebon,
                "edad 13 pasa a Cebon");
            Assert(BuscarRes(hacienda, "F-C", "Borde48") is Cebon,
                "edad 48 (edad_max_cebon) sigue siendo Cebon");
            Assert(BuscarRes(hacienda, "F-N", "Borde49") is Novillo,
                "edad 49 pasa a Novillo");
        }

        private static void SubtipoRechazaEdadFueraDeSuRango()
        {
            AssertThrows("El ternero excedió la edad maxima",
                () => new Ternero("FueraDeRango", 200, 13),
                "el constructor de Ternero valida su rango de edad");

            AssertThrows("El cebon excedió la edad maxima",
                () => new Cebon("FueraDeRango", 300, 12),
                "el constructor de Cebon valida su rango de edad");

            AssertThrows("El ternero excedió la edad maxima",
                () => new Novillo("FueraDeRango", 500, 48),
                "el constructor de Novillo valida su rango de edad (mensaje heredado del baseline)");
        }

        private static void PotreroRechazaEdadQueNoCorresponde()
        {
            var hacienda = new Hacienda();
            hacienda.crear_potrero("X-T", l_tipos_potreros.ternero);

            AssertThrows(
                "Error inesperado en el método anadir_res_potrero: La edad de la res no corresponde al tipo de potrero ternero.",
                () => hacienda.anadir_res_potrero("X-T", "Intrusa", 30, 300),
                "un potrero de terneros rechaza una res de 30 meses");
        }

        private static void PotreroRechazaResDuplicada()
        {
            var hacienda = new Hacienda();
            hacienda.crear_potrero("D-T", l_tipos_potreros.ternero);
            hacienda.anadir_res_potrero("D-T", "Repetida", 6, 160);

            AssertThrows(
                "Error inesperado en el método anadir_res_potrero: Ya existe una res con el nombre 'Repetida'.",
                () => hacienda.anadir_res_potrero("D-T", "Repetida", 6, 160),
                "el potrero rechaza una res con nombre repetido");
        }

        private static void PotreroRechazaResCuandoEstaLleno()
        {
            var hacienda = new Hacienda();
            hacienda.crear_potrero("L-T", l_tipos_potreros.ternero);

            for (int i = 1; i <= ReglaPotrero.max_reses_potrero; i++)
            {
                hacienda.anadir_res_potrero("L-T", $"Res{i:D3}", 6, 160);
            }

            AssertEqual(ReglaPotrero.max_reses_potrero,
                hacienda.buscar_potrero("L-T").L_reses.Count,
                "el potrero admite exactamente max_reses_potrero reses");

            AssertThrows(
                "Error inesperado en el método anadir_res_potrero: El potrero L-T está lleno.",
                () => hacienda.anadir_res_potrero("L-T", "Sobrante", 6, 160),
                "el potrero lleno rechaza la res siguiente");
        }

        private static void CatalogoResuelveCreadorPorEdadYPorCategoria()
        {
            var catalogo = CatalogoCreadoresRes.PorDefecto();

            AssertEqual("Ternero", catalogo.ParaEdad(ReglaRes.edad_max_ternero).Categoria,
                "el catalogo resuelve Ternero en su edad limite");
            AssertEqual("Cebon", catalogo.ParaEdad((ushort)(ReglaRes.edad_max_ternero + 1)).Categoria,
                "el catalogo resuelve Cebon apenas se pasa el limite de ternero");
            AssertEqual("Novillo", catalogo.ParaEdad((ushort)(ReglaRes.edad_max_cebon + 1)).Categoria,
                "el catalogo resuelve Novillo apenas se pasa el limite de cebon");

            Assert(catalogo.ParaCategoria("cebon") != null,
                "el catalogo resuelve la categoria persistida sin distinguir mayusculas");
            Assert(catalogo.ParaCategoria("Bufalo") == null,
                "una categoria no registrada no resuelve creador");

            Assert(catalogo.ParaEdad(6).Crear("Nueva", 160, 6) is Ternero,
                "el creador resuelto instancia el subtipo que declara");
        }

        // OCP: una categoria nueva entra registrando su creador. Ni Hacienda ni
        // la persistencia se modifican, y por eso este creador vive solo aqui.
        private static void CatalogoAdmiteCategoriaNuevaSinTocarClientes()
        {
            var catalogo = new CatalogoCreadoresRes(new ICreadorRes[] { new CreadorVerificador() });

            Res res = catalogo.ParaEdad(200).Crear("Experimental", 900, 200);

            Assert(res is ResVerificador,
                "un creador registrado despues cubre edades que ninguna categoria actual atiende");
            AssertEqual("ResVerificador", catalogo.ParaCategoria("ResVerificador").Categoria,
                "la categoria nueva se resuelve por su discriminador persistido");

            AssertThrows("Ninguna categoría de res cubre una edad de 5 meses.",
                () => catalogo.ParaEdad(5),
                "un catalogo sin creador aplicable lo dice explicitamente");
        }

        // ------------------------------------------------------------------
        // Construccion de vacunas
        // ------------------------------------------------------------------

        private static void CrearBacterianaIndividual()
        {
            var hacienda = new Hacienda();

            AssertEqual(
                "Vacuna bacteriana 'Aftosa' del lote 'LB-1' agregada al inventario con éxito. Período de aplicación: 3 semanas.",
                hacienda.crear_vacuna("Aftosa", "LB-1", Vencimiento, Aplicacion, 3u),
                "crear vacuna bacteriana individual");

            AssertEqual(1, hacienda.L_vacunas.Count, "la bacteriana queda en el inventario");
            Assert(hacienda.L_vacunas.Single() is Bacteriana, "el inventario guarda una Bacteriana");
        }

        private static void CrearVivaIndividual()
        {
            var hacienda = new Hacienda();

            AssertEqual(
                "Vacuna viva 'Brucelosis' del lote 'LV-1' agregada al inventario con éxito. Grado de atenuación: 20.",
                hacienda.crear_vacuna("Brucelosis", "LV-1", Vencimiento, Aplicacion, Atenuaciones.Atenuacion20),
                "crear vacuna viva individual");

            AssertEqual(1, hacienda.L_vacunas.Count, "la viva queda en el inventario");
            Assert(hacienda.L_vacunas.Single() is Viva, "el inventario guarda una Viva");
        }

        private static void CrearLoteBacteriano()
        {
            var hacienda = new Hacienda();

            AssertEqual(
                "Lote de vacunas bacterianas creado con éxito:\n" +
                "- Nombre: {nombre}\n" +
                "- Cantidad creada: 3 de 3\n" +
                "- Lotes: LB-3-001 a LB-3-003\n" +
                "- Período de aplicación: 2 semanas",
                hacienda.crear_vacuna("Aftosa", "LB-3", Vencimiento, Aplicacion, 2u, 3u),
                "crear lote bacteriano");

            AssertEqual(3, hacienda.L_vacunas.Count, "el lote bacteriano agrega 3 vacunas");
            AssertEqual("LB-3-001", hacienda.L_vacunas.First().Lote, "el lote se numera desde 001");
            AssertEqual("LB-3-003", hacienda.L_vacunas.Last().Lote, "el lote se numera hasta la cantidad pedida");
        }

        private static void CrearLoteVivo()
        {
            var hacienda = new Hacienda();

            AssertEqual(
                "Lote de vacunas vivas creado con éxito:\n" +
                "- Nombre: Brucelosis\n" +
                "- Cantidad creada: 2 de 2\n" +
                "- Lotes: LV-2-001 a LV-2-002\n" +
                "- Grado de atenuación: 30",
                hacienda.crear_vacuna("Brucelosis", "LV-2", Vencimiento, Aplicacion, Atenuaciones.Atenuacion30, 2u),
                "crear lote vivo");

            AssertEqual(2, hacienda.L_vacunas.Count, "el lote vivo agrega 2 vacunas");
        }

        // El lote bacteriano imprime el literal "{nombre}" porque a esa linea le
        // falta el prefijo de interpolacion. Es comportamiento observable actual
        // y ningun patron esta autorizado a corregirlo.
        private static void LoteBacterianoConservaLiteralNombreSinInterpolar()
        {
            var hacienda = new Hacienda();
            string salida = hacienda.crear_vacuna("Aftosa", "LIT-1", Vencimiento, Aplicacion, 2u, 1u);

            Assert(salida.Contains("- Nombre: {nombre}"),
                "el lote bacteriano conserva el literal '- Nombre: {nombre}'");
            Assert(!salida.Contains("- Nombre: Aftosa"),
                "el lote bacteriano no interpola el nombre real");
        }

        private static void LoteOmiteDuplicadosYCuentaSoloLasCreadas()
        {
            var hacienda = new Hacienda();
            hacienda.crear_vacuna("Aftosa", "MIX", Vencimiento, Aplicacion, 2u, 2u);

            AssertEqual(
                "Lote de vacunas bacterianas creado con éxito:\n" +
                "- Nombre: {nombre}\n" +
                "- Cantidad creada: 2 de 4\n" +
                "- Lotes: MIX-001 a MIX-002\n" +
                "- Período de aplicación: 2 semanas",
                hacienda.crear_vacuna("Aftosa", "MIX", Vencimiento, Aplicacion, 2u, 4u),
                "un lote parcialmente existente solo cuenta las vacunas nuevas");

            AssertEqual(4, hacienda.L_vacunas.Count, "el inventario acumula 2 + 2 vacunas");
        }

        private static void LoteCompletamenteDuplicadoFalla()
        {
            var hacienda = new Hacienda();
            hacienda.crear_vacuna("Aftosa", "DUP", Vencimiento, Aplicacion, 2u, 2u);

            AssertThrows(
                "Error inesperado en el método crear_vacuna (lote bacteriano): No se pudo crear ninguna vacuna. Todos los lotes ya existen en el inventario",
                () => hacienda.crear_vacuna("Aftosa", "DUP", Vencimiento, Aplicacion, 2u, 2u),
                "un lote totalmente duplicado falla");
        }

        private static void PeriodoBacterianoFueraDeRangoFalla()
        {
            var hacienda = new Hacienda();

            AssertThrows(
                "Error inesperado en el método crear_vacuna (bacteriana): el valor del periodo de aplicacion debe estar entre 2 y 4 semanas",
                () => hacienda.crear_vacuna("Aftosa", "PER-1", Vencimiento, Aplicacion, 1u),
                "un periodo bacteriano menor al minimo falla");

            AssertThrows(
                "Error inesperado en el método crear_vacuna (bacteriana): el valor del periodo de aplicacion debe estar entre 2 y 4 semanas",
                () => hacienda.crear_vacuna("Aftosa", "PER-2", Vencimiento, Aplicacion, 5u),
                "un periodo bacteriano mayor al maximo falla");
        }

        private static void FechaDeVencimientoNoPosteriorFalla()
        {
            var hacienda = new Hacienda();

            AssertThrows(
                "Error inesperado en el método crear_vacuna (bacteriana): La fecha de vencimiento debe ser posterior a la fecha de aplicación",
                () => hacienda.crear_vacuna("Aftosa", "FEC-1", Aplicacion, Aplicacion, 2u),
                "vencimiento igual a aplicacion falla en la bacteriana individual");

            AssertThrows(
                "Error inesperado en el método crear_vacuna (lote vivo): La fecha de vencimiento debe ser posterior a la fecha de aplicación",
                () => hacienda.crear_vacuna("Brucelosis", "FEC-2", Aplicacion, Vencimiento, Atenuaciones.Atenuacion10, 2u),
                "vencimiento anterior a aplicacion falla en el lote vivo");
        }

        private static void LoteIndividualDuplicadoFalla()
        {
            var hacienda = new Hacienda();
            hacienda.crear_vacuna("Aftosa", "UNI-1", Vencimiento, Aplicacion, 2u);

            AssertThrows(
                "Error inesperado en el método crear_vacuna (viva): Ya existe una vacuna con el lote 'UNI-1' en el inventario",
                () => hacienda.crear_vacuna("Brucelosis", "UNI-1", Vencimiento, Aplicacion, Atenuaciones.Atenuacion10),
                "el lote individual duplicado falla sin importar la variante");
        }

        private static void CantidadDeLoteFueraDeRangoFalla()
        {
            var hacienda = new Hacienda();

            AssertThrows(
                "Error inesperado en el método crear_vacuna (lote bacteriano): La cantidad debe ser mayor a 0 (Parameter 'cantidad')",
                () => hacienda.crear_vacuna("Aftosa", "CAN-1", Vencimiento, Aplicacion, 2u, 0u),
                "cantidad 0 falla");

            AssertThrows(
                "Error inesperado en el método crear_vacuna (lote bacteriano): No se pueden crear más de 100 vacunas en un solo lote (Parameter 'cantidad')",
                () => hacienda.crear_vacuna("Aftosa", "CAN-2", Vencimiento, Aplicacion, 2u, 101u),
                "cantidad mayor a 100 falla");
        }

        // OCP: una variante nueva de vacuna reutiliza la secuencia del director
        // aportando solo sus partes. FabricadorVacunas no se modifica, y por eso
        // este builder vive unicamente en el verifier.
        private static void DirectorAdmiteVarianteNuevaSinModificarse()
        {
            var inventario = new List<Vacuna>();
            var fabricador = new FabricadorVacunas(inventario);
            var builder = new BuilderVerificador();

            AssertEqual(
                "Vacuna verificadora 'Experimental' del lote 'VER-1' agregada al inventario con éxito. Dosis de prueba: 7.",
                fabricador.Crear(builder, "Experimental", "VER-1", Vencimiento, Aplicacion),
                "el director arma el mensaje individual de una variante que no conocia");

            AssertEqual(
                "Lote de vacunas verificadoras creado con éxito:\n" +
                "- Nombre: Experimental\n" +
                "- Cantidad creada: 2 de 2\n" +
                "- Lotes: VER-L-001 a VER-L-002\n" +
                "- Dosis de prueba: 7",
                fabricador.CrearLote(builder, "Experimental", "VER-L", Vencimiento, Aplicacion, 2u),
                "el director reutiliza el algoritmo de lote con la variante nueva");

            AssertEqual(3, inventario.Count, "la variante nueva llega al mismo inventario");
            Assert(inventario.All(v => v is VacunaVerificadora),
                "el director agrega exactamente el producto que devuelve el builder");

            AssertThrows(
                "Error inesperado en el método crear_vacuna (lote verificador): No se pudo crear ninguna vacuna. Todos los lotes ya existen en el inventario",
                () => fabricador.CrearLote(builder, "Experimental", "VER-L", Vencimiento, Aplicacion, 2u),
                "las etiquetas de error tambien salen de la variante");
        }

        // ------------------------------------------------------------------
        // Eventos
        // ------------------------------------------------------------------

        private static void AlimentarInformaDesnutricion()
        {
            var hacienda = HaciendaConRes("E-T", l_tipos_potreros.ternero, "Flaca", 6, 100);

            AssertEqual(
                "La res 'Flaca' ha sido alimentada, ahora pesa 120 kg.\n" +
                "[Evento] La res 'Flaca' tiene un peso 120, está en desnutrición.",
                hacienda.alimentar_res("E-T", "Flaca", 20),
                "alimentar una res bajo el peso minimo agrega el aviso de desnutricion");
        }

        private static void AlimentarInformaAptaParaVenta()
        {
            var hacienda = HaciendaConRes("E-T2", l_tipos_potreros.ternero, "Gorda", 6, 240);

            AssertEqual(
                "La res 'Gorda' ha sido alimentada, ahora pesa 260 kg.\n" +
                "[Evento] La res 'Gorda' tiene un peso 260, apta para venta.",
                hacienda.alimentar_res("E-T2", "Gorda", 20),
                "alimentar una res sobre el peso de venta agrega el aviso de venta");
        }

        private static void AlimentarSinAvisoNoAgregaLineas()
        {
            var hacienda = HaciendaConRes("E-T3", l_tipos_potreros.ternero, "Normal", 6, 180);

            AssertEqual(
                "La res 'Normal' ha sido alimentada, ahora pesa 200 kg.",
                hacienda.alimentar_res("E-T3", "Normal", 20),
                "sin aviso el mensaje queda en una sola linea");
        }

        // Gate de acumulacion observable: repetir la operacion no puede aumentar
        // el numero de lineas que devuelve.
        private static void AlimentarRepetidoNoDuplicaMensajes()
        {
            var hacienda = HaciendaConRes("E-R", l_tipos_potreros.ternero, "Repetidora", 6, 100);

            for (int i = 1; i <= 20; i++)
            {
                string salida = hacienda.alimentar_res("E-R", "Repetidora", 1);
                int lineas = salida.Split('\n').Length;

                if (lineas != 2)
                {
                    Fail($"la llamada {i} a alimentar_res devolvio {lineas} lineas en vez de 2");
                    return;
                }
            }

            _checks++;
            AssertEqual(
                "La res 'Repetidora' ha sido alimentada, ahora pesa 121 kg.\n" +
                "[Evento] La res 'Repetidora' tiene un peso 121, está en desnutrición.",
                hacienda.alimentar_res("E-R", "Repetidora", 1),
                "la llamada 21 devuelve exactamente el mismo formato que la primera");
        }

        private static void AplicarVacunaInformaEsquemaIncompleto()
        {
            var hacienda = HaciendaConRes("V-T", l_tipos_potreros.ternero, "Vacunada", 6, 200);
            hacienda.crear_vacuna("Aftosa", "AP-1", Vencimiento, Aplicacion, 2u);
            Vacuna vacuna = hacienda.L_vacunas.Single();

            AssertEqual(
                "Vacuna aplicada correctamente a la res Vacunada. " +
                "[Evento] La res 'Vacunada' aún no ha completado su esquema de vacunación. Bacterianas: 1, Vivas: 0",
                hacienda.aplicar_vacuna(vacuna, "Vacunada", "V-T"),
                "aplicar una vacuna informa el esquema incompleto");

            AssertEqual(0, hacienda.L_vacunas.Count, "la vacuna aplicada sale del inventario");
        }

        private static void AplicarVacunaInformaEsquemaCompleto()
        {
            var hacienda = HaciendaConRes("V-T2", l_tipos_potreros.ternero, "Completa", 6, 200);

            for (int i = 1; i <= ReglaVacuna.max_bac_ternero; i++)
            {
                hacienda.crear_vacuna($"Aftosa{i}", $"CB-{i}", Vencimiento, Aplicacion, 2u);
            }
            hacienda.crear_vacuna("Brucelosis", "CV-1", Vencimiento, Aplicacion, Atenuaciones.Atenuacion10);

            var pendientes = hacienda.L_vacunas.ToList();
            for (int i = 0; i < pendientes.Count - 1; i++)
            {
                hacienda.aplicar_vacuna(pendientes[i], "Completa", "V-T2");
            }

            AssertEqual(
                "Vacuna aplicada correctamente a la res Completa. " +
                "[Evento] La res 'Completa' ha completado su esquema de vacunación.",
                hacienda.aplicar_vacuna(pendientes[pendientes.Count - 1], "Completa", "V-T2"),
                "al cerrar el esquema el aviso cambia a completado");
        }

        // Gate de P-03: la suscripcion se establece una vez, en el constructor,
        // asi que repetir la operacion no deja handlers acumulados.
        private static void SuscripcionNoCreceConLasLlamadas()
        {
            var hacienda = HaciendaConRes("S-T", l_tipos_potreros.ternero, "Contada", 6, 100);

            for (int i = 0; i < 20; i++)
            {
                hacienda.alimentar_res("S-T", "Contada", 1);
            }

            AssertEqual(1, ContarSuscriptores(hacienda, "publisher_peso_min", "evt_peso_min"),
                "evt_peso_min conserva un solo handler tras 20 llamadas");
            AssertEqual(1, ContarSuscriptores(hacienda, "publisher_peso_ideal", "evt_peso_venta"),
                "evt_peso_venta conserva un solo handler tras 20 llamadas");

            var vacunada = HaciendaConRes("S-V", l_tipos_potreros.ternero, "Pinchada", 6, 200);
            for (int i = 1; i <= ReglaVacuna.max_bac_ternero; i++)
            {
                vacunada.crear_vacuna($"Aftosa{i}", $"SB-{i}", Vencimiento, Aplicacion, 2u);
            }
            foreach (var vacuna in vacunada.L_vacunas.ToList())
            {
                vacunada.aplicar_vacuna(vacuna, "Pinchada", "S-V");
            }

            AssertEqual(1, ContarSuscriptores(vacunada, "publisher_vacunacion_completa", "evt_vacunacion_completada"),
                "evt_vacunacion_completada conserva un solo handler tras varias aplicaciones");
        }

        // El orden de los avisos es observable: la captura los entrega en el
        // orden en que los publishers los emitieron, no en uno fijo.
        private static void CapturaConservaElOrdenDeEmision()
        {
            var recolector = new RecolectorMensajes();
            var potreroMitad = new PublisherPotreroMitad();
            var pesoMin = new PublisherPesoMin();

            potreroMitad.evt_potrero_mitad += recolector.Recibir;
            pesoMin.evt_peso_min += recolector.Recibir;

            var potrero = new Potrero("O-T", l_tipos_potreros.ternero);
            var flaca = new Ternero("Flaca", 100, 6);
            ushort mitad = (ushort)(ReglaPotrero.max_reses_potrero / 2);

            string avisoMitad = "[Evento] El potrero 'O-T' ha alcanzado la mitad de su capacidad máxima de reses.";
            string avisoPeso = "[Evento] La res 'Flaca' tiene un peso 100, está en desnutrición.";

            using (var captura = recolector.Capturar())
            {
                potreroMitad.Informar_Potrero_Mitad(mitad, potrero);
                pesoMin.Informar_Peso_Min(flaca);

                AssertEqual(avisoMitad + "\n" + avisoPeso, captura.Texto(),
                    "la captura entrega los dos avisos en el orden emitido");
            }

            using (var captura = recolector.Capturar())
            {
                pesoMin.Informar_Peso_Min(flaca);
                potreroMitad.Informar_Potrero_Mitad(mitad, potrero);

                AssertEqual(avisoPeso + "\n" + avisoMitad, captura.Texto(),
                    "invertir la emision invierte el orden entregado");
            }
        }

        private static void CapturaAislaOperacionesEntreSi()
        {
            var recolector = new RecolectorMensajes();
            var pesoMin = new PublisherPesoMin();
            pesoMin.evt_peso_min += recolector.Recibir;

            var flaca = new Ternero("Flaca", 100, 6);

            using (var primera = recolector.Capturar())
            {
                pesoMin.Informar_Peso_Min(flaca);
                AssertEqual(1, primera.Mensajes.Count, "la primera captura recoge su aviso");
            }

            // Emitido sin captura abierta: no debe quedar guardado en ningun lado.
            pesoMin.Informar_Peso_Min(flaca);

            using (var segunda = recolector.Capturar())
            {
                AssertEqual("", segunda.Texto(),
                    "una captura nueva no arrastra mensajes de operaciones anteriores");

                AssertThrows("Ya hay una captura de mensajes abierta.",
                    () => recolector.Capturar(),
                    "no se pueden abrir dos capturas a la vez");
            }
        }

        // ------------------------------------------------------------------
        // SC-3: historia clinica
        // ------------------------------------------------------------------

        private static void ResTieneHistoriaClinicaInicializada()
        {
            var res = new Ternero("Clinica", 200, 6);

            Assert(res.HistoriaClinica != null,
                "una res nueva siempre posee una historia clinica");
        }

        private static void HistoriaClinicaIniciaSinRegistros()
        {
            var historia = new HistoriaClinica();

            AssertEqual(0, historia.VacunasAplicadas.Count,
                "la historia clinica inicia sin vacunas");
            AssertEqual(0, historia.EventosClinicos.Count,
                "la historia clinica inicia sin eventos");
        }

        private static void VacunaAplicadaSeRegistraUnaVezEnHistoria()
        {
            var res = new Ternero("VacunacionValida", 200, 6);
            var vacuna = new Bacteriana("Aftosa", "HC-1", Vencimiento, Aplicacion, 2u);

            res.aplicar_vacuna(vacuna);

            AssertEqual(1, res.HistoriaClinica.VacunasAplicadas.Count,
                "una vacuna valida queda registrada una sola vez en la historia clinica");
            Assert(ReferenceEquals(vacuna, res.HistoriaClinica.VacunasAplicadas.Single()),
                "la historia clinica conserva la vacuna aplicada");
        }

        private static void VacunaDuplicadaNoAlteraHistoria()
        {
            var res = new Ternero("VacunacionDuplicada", 200, 6);
            var vacuna = new Bacteriana("Aftosa", "HC-2", Vencimiento, Aplicacion, 2u);
            res.aplicar_vacuna(vacuna);

            AssertThrows(
                "Error inesperado en el metodo aplicar_vacuna: La vacuna 'Aftosa' ya fue aplicada a la res 'VacunacionDuplicada'.",
                () => res.aplicar_vacuna(vacuna),
                "una vacuna duplicada conserva la regla existente");
            AssertEqual(1, res.HistoriaClinica.VacunasAplicadas.Count,
                "una vacuna duplicada no altera el historial");
        }

        private static void VacunaVencidaNoAlteraHistoria()
        {
            var res = new Ternero("VacunacionVencida", 200, 6);
            var vencida = new Bacteriana("Brucelosis", "HC-3", DateTime.Now.AddDays(-1), Aplicacion, 2u);

            AssertThrows(
                "Error inesperado en el metodo aplicar_vacuna: " +
                $"[Evento] La vacuna 'Brucelosis' del lote 'HC-3' está vencida desde {vencida.Fecha_vencimiento.ToShortDateString()}",
                () => res.aplicar_vacuna(vencida),
                "una vacuna vencida conserva la regla existente");
            AssertEqual(0, res.HistoriaClinica.VacunasAplicadas.Count,
                "una vacuna vencida no altera el historial");
        }

        private static void ResesTienenHistoriasIndependientes()
        {
            var primera = new Ternero("Primera", 200, 6);
            var segunda = new Ternero("Segunda", 200, 6);

            primera.HistoriaClinica.RegistrarEvento(
                new EventoClinico(Aplicacion, "Revision", "Sin hallazgos"));

            Assert(!ReferenceEquals(primera.HistoriaClinica, segunda.HistoriaClinica),
                "dos reses no comparten la misma historia clinica");
            AssertEqual(0, segunda.HistoriaClinica.EventosClinicos.Count,
                "los eventos de una res no aparecen en otra");
        }

        private static void FachadaLegacyDeVacunasComparteLaHistoria()
        {
            var res = new Ternero("Compatibilidad", 200, 6);

            Assert(ReferenceEquals(res.L_vacunas_aplicadas, res.HistoriaClinica.L_vacunas_aplicadas),
                "L_vacunas_aplicadas es la misma coleccion de la historia clinica");
        }

        private static void HistoriaClinicaNoPuedeCompartirseEntreReses()
        {
            var historia = new HistoriaClinica();
            var primera = new ResConHistoria("PrimeraConHistoria", 200, 6, historia);

            AssertThrows("La historia clinica ya pertenece a otra res.",
                () => new ResConHistoria("SegundaConHistoria", 200, 6, historia),
                "una historia clinica no puede asignarse a dos reses");
            Assert(ReferenceEquals(historia, primera.HistoriaClinica),
                "la res conserva su historia clinica propia");
        }

        internal static int ContarSuscriptores(object propietario, string campoPublisher, string campoEvento)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

            object publisher = propietario.GetType().GetField(campoPublisher, flags)?.GetValue(propietario);
            if (publisher == null)
            {
                return -1;
            }

            var delegado = publisher.GetType().GetField(campoEvento, flags)?.GetValue(publisher) as Delegate;
            return delegado == null ? 0 : delegado.GetInvocationList().Length;
        }

        // ------------------------------------------------------------------
        // Utilidades
        // ------------------------------------------------------------------

        private static Hacienda HaciendaConRes(string idPotrero, l_tipos_potreros tipo, string nombreRes, ushort edad, uint peso)
        {
            var hacienda = new Hacienda();
            hacienda.crear_potrero(idPotrero, tipo);
            hacienda.anadir_res_potrero(idPotrero, nombreRes, edad, peso);
            return hacienda;
        }

        private static Res BuscarRes(Hacienda hacienda, string idPotrero, string nombreRes)
        {
            return hacienda.buscar_potrero(idPotrero).buscar_res(nombreRes);
        }

        private static void Assert(bool condicion, string mensaje)
        {
            _checks++;
            if (!condicion)
            {
                Console.WriteLine($"[FALLA] {mensaje}");
                _fallos++;
            }
        }

        private static void AssertEqual<T>(T esperado, T obtenido, string mensaje)
        {
            _checks++;
            if (!EqualityComparer<T>.Default.Equals(esperado, obtenido))
            {
                Console.WriteLine($"[FALLA] {mensaje}");
                Console.WriteLine($"        esperado: {Mostrar(esperado)}");
                Console.WriteLine($"        obtenido: {Mostrar(obtenido)}");
                _fallos++;
            }
        }

        private static void AssertThrows(string mensajeEsperado, Action accion, string mensaje)
        {
            _checks++;
            try
            {
                accion();
                Console.WriteLine($"[FALLA] {mensaje}");
                Console.WriteLine($"        esperaba una excepcion: {mensajeEsperado}");
                _fallos++;
            }
            catch (Exception ex)
            {
                if (ex.Message != mensajeEsperado)
                {
                    Console.WriteLine($"[FALLA] {mensaje}");
                    Console.WriteLine($"        esperado: {Mostrar(mensajeEsperado)}");
                    Console.WriteLine($"        obtenido: {Mostrar(ex.Message)}");
                    _fallos++;
                }
            }
        }

        private static void Fail(string mensaje)
        {
            _checks++;
            Console.WriteLine($"[FALLA] {mensaje}");
            _fallos++;
        }

        private static string Mostrar(object valor)
        {
            if (valor == null)
            {
                return "<null>";
            }

            return valor.ToString().Replace("\n", "\\n");
        }
    }

    // Categoria de res definida solo en el verifier, para demostrar que agregar
    // una no obliga a modificar Hacienda ni PersistenciaService.
    internal class ResVerificador : Res
    {
        public ResVerificador(string nombre, uint peso, ushort edad) : base(nombre, peso, edad)
        {
        }

        public override byte MaxVacunasBacterianas => 1;

        public override byte MaxVacunasVivas => 1;
    }

    internal class ResConHistoria : Res
    {
        public ResConHistoria(string nombre, uint peso, ushort edad, HistoriaClinica historiaClinica)
            : base(nombre, peso, edad, historiaClinica)
        {
        }

        public override byte MaxVacunasBacterianas => 1;

        public override byte MaxVacunasVivas => 1;
    }

    // Variante de vacuna definida solo en el verifier. Tipo devuelve un valor
    // del enum existente porque TipoVacuna duplica la jerarquia: es P-05, punto
    // declarado y no intervenido en este reto.
    internal class VacunaVerificadora : Vacuna
    {
        public VacunaVerificadora(string nombre, string lote, DateTime fechaVencimiento, DateTime fechaAplicacion)
            : base(nombre, lote, fechaVencimiento, fechaAplicacion)
        {
        }

        public override TipoVacuna Tipo => TipoVacuna.Bacteriana;

        public override bool PuedeAplicarseA(Res res) => true;
    }

    internal class BuilderVerificador : IVacunaBuilder
    {
        public string Variante => "verificadora";

        public string VariantePlural => "verificadoras";

        public string VarianteLote => "verificador";

        public string DetalleEspecifico => "Dosis de prueba: 7";

        public string NombreEnResumenDeLote(string nombre) => nombre;

        public Vacuna Construir(string nombre, string lote, DateTime fechaVencimiento, DateTime fechaAplicacion)
        {
            return new VacunaVerificadora(nombre, lote, fechaVencimiento, fechaAplicacion);
        }
    }

    internal class CreadorVerificador : ICreadorRes
    {
        public string Categoria => nameof(ResVerificador);

        public bool AplicaA(ushort edad) => edad > 100;

        public Res Crear(string nombre, uint peso, ushort edad)
        {
            return new ResVerificador(nombre, peso, edad);
        }
    }
}
