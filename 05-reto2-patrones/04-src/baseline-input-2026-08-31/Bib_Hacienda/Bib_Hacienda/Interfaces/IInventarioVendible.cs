using Bib_Hacienda.Clases;

namespace Bib_Hacienda.Interfaces
{
    // Contrato estrecho para inventarios que pueden vender productos.
    // Separa las operaciones de consulta/retiro de la operación de agregado,
    // evitando que clientes de venta dependan de un contrato más ancho.
    public interface IInventarioVendible<T> where T : Producto
    {
        bool contiene(T producto);
        T retirar(T producto);
    }
}
