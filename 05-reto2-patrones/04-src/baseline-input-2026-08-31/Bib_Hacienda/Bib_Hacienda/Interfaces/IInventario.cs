using Bib_Hacienda.Clases;

namespace Bib_Hacienda.Interfaces
{
    // Contrato completo de inventario: hereda el contrato de venta y añade
    // la capacidad de agregar productos. Los inventarios concretos implementan
    // esta interfaz; los clientes de venta dependen solo de IInventarioVendible<T>.
    public interface IInventario<T> : IInventarioVendible<T> where T : Producto
    {
        void agregar(T producto);
    }
}
