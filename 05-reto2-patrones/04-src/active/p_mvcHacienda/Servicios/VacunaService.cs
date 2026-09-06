using Bib_Hacienda.Clases;
using Bib_Hacienda.enums;
using Bib_Hacienda.Interfaces;

namespace p_mvcHacienda.Servicios
{
    public class VacunaService
    {
        private readonly Hacienda _hacienda;
        private readonly IPersistenciaVacunas _persistenciaVacunas;
        private readonly IPersistenciaPotreros _persistenciaPotreros;
        private readonly IPersistenciaReses _persistenciaReses;

        public VacunaService(
            Hacienda hacienda,
            IPersistenciaVacunas persistenciaVacunas,
            IPersistenciaPotreros persistenciaPotreros,
            IPersistenciaReses persistenciaReses)
        {
            _hacienda = hacienda;
            _persistenciaVacunas = persistenciaVacunas;
            _persistenciaPotreros = persistenciaPotreros;
            _persistenciaReses = persistenciaReses;
        }

        public string CrearVacuna(string nombre, string lote, DateTime fechaVencimiento, DateTime fechaAplicacion, uint? periodoAplicacion, Atenuaciones? atenuacion)
        {
            try
            {
                string resultadoDominio;

                if (periodoAplicacion.HasValue && !atenuacion.HasValue)
                {
                    resultadoDominio = _hacienda.crear_vacuna(nombre, lote, fechaVencimiento, fechaAplicacion, periodoAplicacion.Value);
                }
                else if (!periodoAplicacion.HasValue && atenuacion.HasValue)
                {
                    resultadoDominio = _hacienda.crear_vacuna(nombre, lote, fechaVencimiento, fechaAplicacion, atenuacion.Value);
                }
                else
                {
                    return "Error: parámetros inválidos para crear la vacuna (revise tipo, período o atenuación)";
                }

                string validado = _persistenciaVacunas.GuardarVacunas(_hacienda.L_vacunas);

                return $"{resultadoDominio}. {validado}";
            }
            catch (Exception ex)
            {
                return $"{ex.Message}";
            }
        }

        public string AplicarVacuna(string potreroId, string nombreRes, string loteVacuna)
        {
            try
            {
                if (_hacienda.L_vacunas.Count == 0)
                {
                    var cargadas = _persistenciaVacunas.CargarVacunas();
                    foreach (var v in cargadas) _hacienda.L_vacunas.Add(v);
                }

                var vacuna = _hacienda.L_vacunas.FirstOrDefault(v => v.Lote == loteVacuna);
                if (vacuna == null)
                {
                    throw new Exception($"No se encontró una vacuna con el lote '{loteVacuna}'");
                }

                string resultadoDominio = _hacienda.aplicar_vacuna(vacuna, nombreRes, potreroId);

                var existente = _hacienda.L_vacunas.FirstOrDefault(v => v.Lote == loteVacuna);
                if (existente != null)
                {
                    _hacienda.L_vacunas.Remove(existente);
                }

                var validadoAplicadas = _persistenciaVacunas.GuardarVacunasAplicadas(_hacienda.L_potreros);
                var validadoDisponibles = _persistenciaVacunas.GuardarVacunas(_hacienda.L_vacunas);
                _persistenciaPotreros.GuardarPotreros(_hacienda.L_potreros);
                _persistenciaReses.GuardarReses(_hacienda.L_potreros);

                var validado = ConsolidarValidaciones(validadoAplicadas, validadoDisponibles);

                return AsegurarPuntoFinal($"{resultadoDominio}. {validado}".Trim());
            }
            catch (Exception ex)
            {
                return $"{ex.Message}";
            }
        }

        public List<Vacuna> ObtenerVacunasDisponibles()
        {
            if (_hacienda.L_vacunas.Count == 0)
            {
                var cargadas = _persistenciaVacunas.CargarVacunas();
                foreach (var v in cargadas) _hacienda.L_vacunas.Add(v);
            }
            return _hacienda.L_vacunas.OrderBy(v => v.Nombre).ToList();
        }

        public List<Vacuna> ObtenerVacunasAplicadas(string potreroId, string nombreRes)
        {
            try
            {
                var potrero = _hacienda.buscar_potrero(potreroId);
                var res = potrero.buscar_res(nombreRes);
                return res.L_vacunas_aplicadas;
            }
            catch
            {
                return new List<Vacuna>();
            }
        }

        public Dictionary<string, object> ObtenerEstadisticas()
        {
            if (_hacienda.L_vacunas.Count == 0)
            {
                var cargadas = _persistenciaVacunas.CargarVacunas();
                foreach (var v in cargadas) _hacienda.L_vacunas.Add(v);
            }

            var vacunas = _hacienda.L_vacunas;
            return new Dictionary<string, object>
            {
                { "TotalVacunas", vacunas.Count },
                { "Bacterianas", vacunas.Count(v => v is Bacteriana) },
                { "Vivas", vacunas.Count(v => v is Viva) },
                { "Vencidas", vacunas.Count(v => v.Fecha_vencimiento < DateTime.Now) },
                { "Vigentes", vacunas.Count(v => v.Fecha_vencimiento >= DateTime.Now) }
            };
        }

        private string ConsolidarValidaciones(string a, string b)
        {
            a = (a ?? string.Empty).Trim();
            b = (b ?? string.Empty).Trim();
            if (string.Equals(a, b, StringComparison.OrdinalIgnoreCase)) return a;
            if (a.Contains(b, StringComparison.OrdinalIgnoreCase)) return a;
            if (b.Contains(a, StringComparison.OrdinalIgnoreCase)) return b;
            return a.Length > 0 ? a : b;
        }

        private string AsegurarPuntoFinal(string mensaje)
        {
            if (string.IsNullOrWhiteSpace(mensaje)) return mensaje;
            return mensaje.EndsWith(".") ? mensaje : mensaje + ".";
        }
    }
}
