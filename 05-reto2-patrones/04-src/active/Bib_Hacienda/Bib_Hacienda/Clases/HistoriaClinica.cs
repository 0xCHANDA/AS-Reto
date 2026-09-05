using System;
using System.Collections.Generic;
using System.Linq;

namespace Bib_Hacienda.Clases
{
    public class HistoriaClinica
    {
        private List<Vacuna> l_vacunas_aplicadas = new List<Vacuna>();
        private readonly List<EventoClinico> l_eventos_clinicos = new List<EventoClinico>();
        private Res resPropietaria;

        public IReadOnlyList<Vacuna> VacunasAplicadas => l_vacunas_aplicadas.AsReadOnly();
        public IReadOnlyList<EventoClinico> EventosClinicos => l_eventos_clinicos.AsReadOnly();

        // Fachada legacy: conserva la referencia asignada sin crear otra fuente de verdad.
        public List<Vacuna> L_vacunas_aplicadas
        {
            get => l_vacunas_aplicadas;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                if (value.Any(v => v == null) || TieneDuplicados(value))
                    throw new ArgumentException("La lista de vacunas contiene registros invalidos.", nameof(value));

                l_vacunas_aplicadas = value;
            }
        }

        internal void AsociarA(Res res)
        {
            if (res == null)
                throw new ArgumentNullException(nameof(res));

            if (resPropietaria != null && !ReferenceEquals(resPropietaria, res))
                throw new InvalidOperationException("La historia clinica ya pertenece a otra res.");

            resPropietaria = res;
        }

        public void RegistrarVacuna(Vacuna vacuna)
        {
            if (vacuna == null)
                throw new ArgumentNullException(nameof(vacuna));

            if (TieneDuplicados(l_vacunas_aplicadas.Append(vacuna)))
            {
                throw new InvalidOperationException($"La vacuna '{vacuna.Nombre}' ya fue aplicada.");
            }

            l_vacunas_aplicadas.Add(vacuna);
        }

        public void RegistrarEvento(EventoClinico evento)
        {
            if (evento == null)
                throw new ArgumentNullException(nameof(evento));

            l_eventos_clinicos.Add(evento);
        }

        private static bool TieneDuplicados(IEnumerable<Vacuna> vacunas)
        {
            return vacunas.GroupBy(v => v.Nombre).Any(g => g.Count() > 1) ||
                vacunas.GroupBy(v => v.Lote).Any(g => g.Count() > 1);
        }
    }
}
