using Bib_Hacienda.Clases;

namespace p_mvcHacienda.Servicios
{
    // Puerto de persistencia para los eventos clinicos de SC-3. Vive en el
    // proyecto web y no en Bib_Hacienda.Interfaces porque la libreria de dominio
    // no se modifica en este trabajo: el dominio expone HistoriaClinica y esta
    // capa decide como se guarda.
    public interface IPersistenciaEventosClinicos
    {
        string GuardarEventosClinicos(List<Potrero> potreros);

        void CargarEventosClinicos(List<Potrero> potreros);
    }
}
