using Bib_Hacienda.Clases;
using Bib_Hacienda.Interfaces;
using static Bib_Hacienda.Clases.Potrero;

namespace p_mvcHacienda.Servicios
{
    public class PotreroService
    {
        private readonly Hacienda _hacienda;
        private readonly IPersistenciaPotreros _persistenciaPotreros;
        private readonly IPersistenciaReses _persistenciaReses;

        public PotreroService(Hacienda hacienda, IPersistenciaPotreros persistenciaPotreros, IPersistenciaReses persistenciaReses)
        {
            _hacienda = hacienda;
            _persistenciaPotreros = persistenciaPotreros;
            _persistenciaReses = persistenciaReses;
        }

        public string CrearPotrero(string identificacion, l_tipos_potreros tipo)
        {
            try
            {
                if (_hacienda.L_potreros.Any(p => p.Identificacion == identificacion))
                {
                    throw new InvalidOperationException($"Ya existe un potrero con la identificación '{identificacion}'");
                }

                string resultado = _hacienda.crear_potrero(identificacion, tipo);
                string validado = _persistenciaPotreros.GuardarPotreros(_hacienda.L_potreros);

                return $"{resultado}. {validado}";
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException("Validación fallida: El potrero no cumple los requisitos");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al crear el potrero: {ex.Message}");
            }
        }

        public List<Potrero> ObtenerTodosLosPotreros()
        {
            return _hacienda.L_potreros.OrderBy(p => p.Identificacion).ToList();
        }

        public Potrero? ObtenerPotreroPorIdentificacion(string identificacion)
        {
            try
            {
                return _hacienda.buscar_potrero(identificacion);
            }
            catch
            {
                return null;
            }
        }

        public string AgregarRes(string potreroId, string nombreRes, ushort edad, uint peso)
        {
            try
            {
                var potrero = _hacienda.buscar_potrero(potreroId);
                if (potrero == null)
                {
                    throw new InvalidOperationException($"No se encontró el potrero '{potreroId}'");
                }

                if (potrero.L_reses.Any(r => r.Nombre == nombreRes))
                {
                    throw new InvalidOperationException($"Ya existe una res con el nombre '{nombreRes}' en el potrero '{potreroId}'");
                }

                string resultado = _hacienda.anadir_res_potrero(potreroId, nombreRes, edad, peso);
                string validado = _persistenciaReses.GuardarReses(_hacienda.L_potreros);

                return $"{resultado}. {validado}";
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException("Validación fallida: La res no cumple los requisitos");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al agregar la res: {ex.Message}");
            }
        }

        public Dictionary<string, object> ObtenerEstadisticas()
        {
            var potreros = _hacienda.L_potreros;

            return new Dictionary<string, object>
            {
                { "TotalPotreros", potreros.Count },
                { "TotalReses", potreros.Sum(p => p.L_reses.Count) },
                { "PotrerosVacios", potreros.Count(p => p.L_reses.Count == 0) },
                { "PotrerosConReses", potreros.Count(p => p.L_reses.Count > 0) }
            };
        }
    }
}
