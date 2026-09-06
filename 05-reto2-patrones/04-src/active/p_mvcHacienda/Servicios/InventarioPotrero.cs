using Bib_Hacienda.Clases;
using Bib_Hacienda.Interfaces;

namespace p_mvcHacienda.Servicios
{
    // Adapter. Potrero implementa IInventario<Res>, pero Hacienda.vender exige
    // IInventario<Producto>. Como IInventario<T> es invariante, un
    // IInventario<Res> no puede pasar por IInventario<Producto>. Este adaptador
    // cierra esa brecha desde la capa web, sin modificar Bib_Hacienda.
    public class InventarioPotrero : IInventario<Producto>
    {
        private readonly Potrero _potrero;

        public InventarioPotrero(Potrero potrero)
        {
            _potrero = potrero ?? throw new ArgumentNullException(nameof(potrero));
        }

        public void agregar(Producto producto)
        {
            _potrero.agregar(ComoRes(producto));
        }

        public bool contiene(Producto producto)
        {
            return producto is Res res && _potrero.contiene(res);
        }

        public Producto retirar(Producto producto)
        {
            return _potrero.retirar(ComoRes(producto));
        }

        private static Res ComoRes(Producto producto)
        {
            if (producto is Res res)
            {
                return res;
            }

            throw new InvalidOperationException(
                $"Un potrero solo admite reses; '{producto?.Nombre}' no lo es.");
        }
    }
}
