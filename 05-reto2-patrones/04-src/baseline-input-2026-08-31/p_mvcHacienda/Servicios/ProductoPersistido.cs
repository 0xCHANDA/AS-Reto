using Bib_Hacienda.Clases;

namespace p_mvcHacienda.Servicios
{
    // Snapshot genérico para productos desconocidos en tiempo de recarga.
    // Permite que PersistenciaService cumpla el contrato de persistencia sin
    // conocer cada subtipo de Producto de antemano (OCP).
    internal class ProductoPersistido : Producto
    {
        public ProductoPersistido(string tipoOriginal, string nombre) : base(nombre)
        {
            TipoOriginal = tipoOriginal;
        }

        public string TipoOriginal { get; }
    }
}
