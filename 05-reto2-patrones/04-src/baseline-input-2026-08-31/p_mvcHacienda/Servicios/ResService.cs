using Bib_Hacienda.Clases;
using Bib_Hacienda.Interfaces;

namespace p_mvcHacienda.Servicios
{
    public class ResService
    {
        private readonly Hacienda _hacienda;
        private readonly IPersistenciaReses _persistenciaReses;
        private readonly IPersistenciaVentas _persistenciaVentas;

        public ResService(Hacienda hacienda, IPersistenciaReses persistenciaReses, IPersistenciaVentas persistenciaVentas)
        {
            _hacienda = hacienda;
            _persistenciaReses = persistenciaReses;
            _persistenciaVentas = persistenciaVentas;
        }

        public List<(Potrero Potrero, Res Res)> ObtenerTodasLasReses()
        {
            var resesConPotrero = new List<(Potrero, Res)>();

            foreach (var potrero in _hacienda.L_potreros)
            {
                foreach (var res in potrero.L_reses)
                {
                    resesConPotrero.Add((potrero, res));
                }
            }

            return resesConPotrero;
        }

        public Res? BuscarRes(string potreroId, string nombreRes)
        {
            try
            {
                var potrero = _hacienda.buscar_potrero(potreroId);
                return potrero.buscar_res(nombreRes);
            }
            catch
            {
                return null;
            }
        }

        public List<Vacuna> ObtenerVacunasAplicadas(string potreroId, string nombreRes)
        {
            try
            {
                var potrero = _hacienda.buscar_potrero(potreroId);
                var res = potrero.buscar_res(nombreRes);
                return res?.L_vacunas_aplicadas ?? new List<Vacuna>();
            }
            catch
            {
                return new List<Vacuna>();
            }
        }

        public string Alimentar(string potreroId, string nombreRes, uint cantidadAlimento)
        {
            try
            {
                string mensaje = _hacienda.alimentar_res(potreroId, nombreRes, cantidadAlimento);
                _persistenciaReses.GuardarReses(_hacienda.L_potreros);
                return mensaje;
            }
            catch
            {
                throw;
            }
        }

        public string Vender(string potreroId, string nombreRes, uint monto)
        {
            try
            {
                string mensaje = _hacienda.vender_res(potreroId, nombreRes, monto);
                _persistenciaVentas.GuardarVentas(_hacienda.L_ventas);
                _persistenciaReses.GuardarReses(_hacienda.L_potreros);
                return mensaje;
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<string, object> ObtenerEstadisticas()
        {
            var todasLasReses = ObtenerTodasLasReses();

            return new Dictionary<string, object>
            {
                { "TotalReses", todasLasReses.Count },
                { "Terneros", todasLasReses.Count(r => r.Res is Ternero) },
                { "Cebones", todasLasReses.Count(r => r.Res is Cebon) },
                { "Novillos", todasLasReses.Count(r => r.Res is Novillo) },
                { "PesoPromedio", todasLasReses.Any() ? todasLasReses.Average(r => r.Res.Peso) : 0 }
            };
        }
    }
}
