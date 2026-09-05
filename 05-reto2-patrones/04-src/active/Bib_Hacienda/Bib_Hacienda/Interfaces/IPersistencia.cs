using Bib_Hacienda.Clases;
using System.Collections.Generic;

namespace Bib_Hacienda.Interfaces
{
    public interface IPersistenciaPotreros
    {
        List<Potrero> CargarPotreros();
        string GuardarPotreros(List<Potrero> potreros);
    }

    public interface IPersistenciaReses
    {
        void CargarReses(List<Potrero> potreros);
        string GuardarReses(List<Potrero> potreros);
    }

    public interface IPersistenciaVacunas
    {
        List<Vacuna> CargarVacunas();
        string GuardarVacunas(List<Vacuna> vacunas);
        void CargarVacunasAplicadas(List<Potrero> potreros);
        string GuardarVacunasAplicadas(List<Potrero> potreros);
    }

    public interface IPersistenciaVentas
    {
        List<Venta> CargarVentas(List<Potrero> potreros);
        string GuardarVentas(List<Venta> ventas);
    }

    public interface IPersistenciaUsuarios
    {
        List<Usuario> CargarUsuarios();
        string GuardarUsuarios(List<Usuario> usuarios);
    }
}
