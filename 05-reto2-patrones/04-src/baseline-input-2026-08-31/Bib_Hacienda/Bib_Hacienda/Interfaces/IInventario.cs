using Bib_Hacienda.Clases;

namespace Bib_Hacienda.Interfaces
{

    public interface IInventario<T> where T : Producto
    {
        void agregar(T producto);
        bool contiene(T producto);
        T retirar(T producto);
    }

}
