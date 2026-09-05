using Bib_Hacienda.Clases;

namespace p_mvcHacienda.Servicios
{
    public class VentaService
    {
        private readonly Hacienda _hacienda;

        public VentaService(Hacienda hacienda)
        {
            _hacienda = hacienda;
        }

        public List<Venta> ObtenerTodasLasVentas()
        {
            return _hacienda.L_ventas.OrderByDescending(v => v.Fecha).ToList();
        }

        public List<Venta> ObtenerVentasPorPotrero(string potreroId)
        {
            return _hacienda.L_ventas
                .Where(v => v.Potrero != null && v.Potrero.Identificacion == potreroId)
                .OrderByDescending(v => v.Fecha)
                .ToList();
        }

        public List<Venta> ObtenerVentasPorFechas(DateTime fechaInicio, DateTime fechaFin)
        {
            return _hacienda.L_ventas
                .Where(v => v.Fecha >= fechaInicio && v.Fecha <= fechaFin)
                .OrderByDescending(v => v.Fecha)
                .ToList();
        }

        public Dictionary<string, object> ObtenerEstadisticas()
        {
            var ventas = _hacienda.L_ventas;

            return new Dictionary<string, object>
            {
                { "TotalVentas", ventas.Count },
                { "MontoTotal", ventas.Sum(v => v.Monto) },
                { "PromedioVenta", ventas.Any() ? ventas.Average(v => v.Monto) : 0 },
                { "VentasEsteMes", ventas.Count(v => v.Fecha.Month == DateTime.Now.Month && v.Fecha.Year == DateTime.Now.Year) },
                { "MontoEsteMes", ventas.Where(v => v.Fecha.Month == DateTime.Now.Month && v.Fecha.Year == DateTime.Now.Year).Sum(v => v.Monto) }
            };
        }
    }
}
