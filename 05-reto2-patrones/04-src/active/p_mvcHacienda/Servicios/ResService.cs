using Bib_Hacienda.Clases;
using Bib_Hacienda.Interfaces;

namespace p_mvcHacienda.Servicios
{
    public class ResService
    {
        private readonly Hacienda _hacienda;
        private readonly IPersistenciaReses _persistenciaReses;
        private readonly IPersistenciaVentas _persistenciaVentas;
        private readonly IPersistenciaEventosClinicos _persistenciaEventosClinicos;

        public ResService(
            Hacienda hacienda,
            IPersistenciaReses persistenciaReses,
            IPersistenciaVentas persistenciaVentas,
            IPersistenciaEventosClinicos persistenciaEventosClinicos)
        {
            _hacienda = hacienda;
            _persistenciaReses = persistenciaReses;
            _persistenciaVentas = persistenciaVentas;
            _persistenciaEventosClinicos = persistenciaEventosClinicos;
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

        // SC-3: la historia clinica de una res. Devuelve el agregado del dominio
        // para que la vista muestre vacunas y eventos como una sola historia.
        public HistoriaClinica? ObtenerHistoriaClinica(string potreroId, string nombreRes)
        {
            try
            {
                var potrero = _hacienda.buscar_potrero(potreroId);
                var res = potrero.buscar_res(nombreRes);
                return res?.HistoriaClinica;
            }
            catch
            {
                return null;
            }
        }

        // SC-3: registra un evento clinico y lo persiste. La aplicacion de una
        // vacuna NO genera un evento: la vacuna ya se registra como vacuna en la
        // misma historia y duplicarla crearia dos representaciones del mismo hecho.
        public string RegistrarEventoClinico(string potreroId, string nombreRes, DateTime fecha, string concepto, string observacion)
        {
            var potrero = _hacienda.buscar_potrero(potreroId);
            var res = potrero.buscar_res(nombreRes);

            if (res == null)
            {
                throw new Exception($"No se encontro la res '{nombreRes}' en el potrero '{potreroId}'.");
            }

            res.HistoriaClinica.RegistrarEvento(new EventoClinico(fecha, concepto, observacion));
            _persistenciaEventosClinicos.GuardarEventosClinicos(_hacienda.L_potreros);

            return $"Evento clinico registrado en la historia de '{nombreRes}'.";
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
                // La firma generica exige que inventario y producto compartan el
                // mismo tipo concreto; un potrero se vende como inventario de Res.
                var potrero = _hacienda.buscar_potrero(potreroId);
                var res = potrero.buscar_res(nombreRes);

                if (res == null)
                {
                    throw new Exception($"No se encontro la res '{nombreRes}' en el potrero '{potreroId}'.");
                }

                string mensaje = _hacienda.vender(potrero, res, monto);
                _persistenciaVentas.GuardarVentas(_hacienda.L_ventas.ToList());
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
