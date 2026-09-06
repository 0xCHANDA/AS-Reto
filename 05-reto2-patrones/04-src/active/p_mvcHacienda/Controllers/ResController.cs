using Bib_Hacienda.Clases;
using Microsoft.AspNetCore.Mvc;
using p_mvcHacienda.Servicios;

namespace p_mvcHacienda.Controllers
{
    public class ResController : Controller
    {
        private readonly ResService _resService;
        private readonly PotreroService _potreroService;

        public ResController(ResService resService, PotreroService potreroService)
        {
            _resService = resService;
            _potreroService = potreroService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var resesConPotrero = _resService.ObtenerTodasLasReses();
            var estadisticas = _resService.ObtenerEstadisticas();

            ViewBag.Estadisticas = estadisticas;

            return View(resesConPotrero);
        }

        [HttpGet]
        public ActionResult DetalleVacunas(string potreroId, string nombreRes)
        {
            try
            {
                var vacunas = _resService.ObtenerVacunasAplicadas(potreroId, nombreRes);

                ViewBag.PotreroId = potreroId;
                ViewBag.NombreRes = nombreRes;
                return View(vacunas);
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = ex.Message;
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction(nameof(Index));
            }
        }

        // SC-3: una sola pantalla con la historia clinica completa de la res
        // (vacunas aplicadas + eventos clinicos).
        [HttpGet]
        public ActionResult HistoriaClinica(string potreroId, string nombreRes)
        {
            try
            {
                var historia = _resService.ObtenerHistoriaClinica(potreroId, nombreRes);

                if (historia == null)
                {
                    TempData["Mensaje"] = $"No se encontro la res '{nombreRes}'.";
                    TempData["TipoMensaje"] = "danger";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.PotreroId = potreroId;
                ViewBag.NombreRes = nombreRes;
                return View(historia);
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = ex.Message;
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public ActionResult RegistrarEvento(string potreroId, string nombreRes, DateTime fecha, string concepto, string observacion)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(concepto) || string.IsNullOrWhiteSpace(observacion))
                {
                    TempData["Mensaje"] = "El concepto y la observacion son requeridos";
                    TempData["TipoMensaje"] = "danger";
                    return RedirectToAction(nameof(HistoriaClinica), new { potreroId, nombreRes });
                }

                string mensaje = _resService.RegistrarEventoClinico(potreroId, nombreRes, fecha, concepto, observacion);

                TempData["Mensaje"] = mensaje;
                TempData["TipoMensaje"] = "success";
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"{ex.Message}";
                TempData["TipoMensaje"] = "danger";
            }

            return RedirectToAction(nameof(HistoriaClinica), new { potreroId, nombreRes });
        }

        public ActionResult Create()
        {
            ViewBag.Potreros = _potreroService.ObtenerTodosLosPotreros();
            return View();
        }

        [HttpPost]
        public ActionResult Create(string potreroId, string nombre, ushort edad, uint peso)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(potreroId) || string.IsNullOrWhiteSpace(nombre))
                {
                    ViewBag.Mensaje = "Todos los campos son requeridos";
                    ViewBag.TipoMensaje = "danger";
                    ViewBag.Potreros = _potreroService.ObtenerTodosLosPotreros();
                    return View();
                }

                string mensaje = _potreroService.AgregarRes(potreroId, nombre, edad, peso);

                TempData["Mensaje"] = mensaje;
                TempData["TipoMensaje"] = "success";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = $"{ex.Message}";
                ViewBag.TipoMensaje = "danger";
            }

            ViewBag.Potreros = _potreroService.ObtenerTodosLosPotreros();
            return View();
        }

        public ActionResult Alimentar(string potreroId, string nombreRes, uint cantidadAlimento)
        {
            try
            {
                string mensaje = _resService.Alimentar(potreroId, nombreRes, cantidadAlimento);

                TempData["Mensaje"] = mensaje;
                TempData["TipoMensaje"] = "success";
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"{ex.Message}";
                TempData["TipoMensaje"] = "danger";
            }

            return RedirectToAction(nameof(Index));
        }

        public ActionResult Vender(string potreroId, string nombreRes, string monto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(monto))
                {
                    TempData["Mensaje"] = "El monto es requerido";
                    TempData["TipoMensaje"] = "danger";
                    return RedirectToAction(nameof(Index));
                }

                if (!decimal.TryParse(monto, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out var montoDec))
                {
                    TempData["Mensaje"] = "Monto inválido";
                    TempData["TipoMensaje"] = "danger";
                    return RedirectToAction(nameof(Index));
                }

                if (montoDec < 0 || montoDec > uint.MaxValue)
                {
                    TempData["Mensaje"] = $"El monto excede el máximo permitido ({uint.MaxValue})";
                    TempData["TipoMensaje"] = "danger";
                    return RedirectToAction(nameof(Index));
                }

                var montoUint = (uint)montoDec;

                string mensaje = _resService.Vender(potreroId, nombreRes, montoUint);

                TempData["Mensaje"] = mensaje;
                TempData["TipoMensaje"] = "success";
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"{ex.Message}";
                TempData["TipoMensaje"] = "danger";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
