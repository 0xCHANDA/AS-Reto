using Microsoft.AspNetCore.Mvc;
using p_mvcHacienda.Servicios;
using static Bib_Hacienda.Clases.Potrero;

namespace p_mvcHacienda.Controllers
{
    public class PotreroController : Controller
    {
        private readonly PotreroService _potreroService;

        public PotreroController(PotreroService potreroService)
        {
            _potreroService = potreroService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var potreros = _potreroService.ObtenerTodosLosPotreros();
            var estadisticas = _potreroService.ObtenerEstadisticas();

            ViewBag.Estadisticas = estadisticas;

            return View(potreros);
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Details(string id)
        {
            var potrero = _potreroService.ObtenerPotreroPorIdentificacion(id);

            if (potrero == null)
            {
                TempData["Mensaje"] = "Potrero no encontrado";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction(nameof(Index));
            }

            return View(potrero);
        }

        [HttpPost]
        public ActionResult Create(string identificacion, l_tipos_potreros tipo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(identificacion))
                {
                    ViewBag.Mensaje = "La identificación no puede estar vacía";
                    ViewBag.TipoMensaje = "danger";
                    return View();
                }

                string exitoso = _potreroService.CrearPotrero(identificacion, tipo);

                TempData["Mensaje"] = exitoso;
                TempData["TipoMensaje"] = "success";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = $"{ex.Message}";
                ViewBag.TipoMensaje = "danger";
            }

            return View();
        }
    }
}
