using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bib_Hacienda.Eventos;
using Bib_Hacienda.Reglas;
using Bib_Hacienda.Interfaces;


namespace Bib_Hacienda.Clases
{
    public class Potrero: IInventario<Res>
    {

        //Atributos
        public enum l_tipos_potreros {ternero, novillo, cebon};
        private string identificacion;
        private List<Res> l_reses = new List<Res>();
        private l_tipos_potreros tipo_potrero;

        //Eventos
        private PublisherPotreroMitad publisher_potrero_mitad = new PublisherPotreroMitad();
        private PublisherPotreroLleno publisher_potrero_lleno = new PublisherPotreroLleno();
        private PublisherPesoVenta publisher_peso_venta = new PublisherPesoVenta();
        private PublisherPesoMin publisher_peso_min = new PublisherPesoMin();

        //EventHandler
        internal void EventHandler() { }

        //Constructor
        public Potrero(string identificacion, l_tipos_potreros tipo_potrero)
        {
            this.Identificacion = identificacion;
            this.tipo_potrero = tipo_potrero;
        }

        public void Suscribir(RecolectorMensajes recolector)
        {
            publisher_potrero_mitad.evt_potrero_mitad += recolector.Recibir;
            publisher_potrero_lleno.evt_potrero_lleno += recolector.Recibir;
            publisher_peso_min.evt_peso_min += recolector.Recibir;
            publisher_peso_venta.evt_peso_venta += recolector.Recibir;
        }

        public void agregar(Res producto){
    if (producto == null)
        throw new ArgumentNullException(nameof(producto));

    // Validar capacidad
    if (l_reses.Count >= ReglaPotrero.max_reses_potrero)
        throw new InvalidOperationException(
            $"El potrero {identificacion} está lleno.");

    // Validar que no exista
    if (l_reses.Any(r =>
        r.Nombre.Equals(producto.Nombre, StringComparison.OrdinalIgnoreCase)))
    {
        throw new InvalidOperationException(
            $"Ya existe una res con el nombre '{producto.Nombre}'.");
    }

    // Validar edad según el tipo de potrero
    bool edadValida = tipo_potrero switch
    {
        l_tipos_potreros.ternero =>
            producto.Edad <= ReglaRes.edad_max_ternero,

        l_tipos_potreros.cebon =>
            producto.Edad > ReglaRes.edad_max_ternero &&
            producto.Edad <= ReglaRes.edad_max_cebon,

        l_tipos_potreros.novillo =>
            producto.Edad > ReglaRes.edad_max_cebon,

        _ => false
    };

        if (!edadValida)
        {
            throw new Exception(
                $"Error inesperado en el metodo anadir_res: La res no puede ser añadida al potrero {identificacion} porque su edad no corresponde al tipo de potrero");
        }

    // Agregar
    l_reses.Add(producto);

    // Eventos
    publisher_potrero_mitad.Informar_Potrero_Mitad(
        (ushort)l_reses.Count, this);

    publisher_potrero_lleno.Informar_Potrero_Lleno(
        (ushort)l_reses.Count, this);

    publisher_peso_min.Informar_Peso_Min(producto);

    publisher_peso_venta.Informar_Peso_Venta(producto);
}

       //Metodo para buscar res por el nombre
        public Res buscar_res(string nombre)
        {
            try
            {
                // Validar nombre
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    throw new ArgumentException("El nombre de búsqueda no puede estar vacío.");
                }

                // Buscar la res que contengan el texto (ignorando mayúsculas/minúsculas)
                var res_encontrada = l_reses
                    .Where(p => p.Nombre.IndexOf(nombre, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();

                // Si no hay resultados
                if (res_encontrada.Count == 0)
                {
                    throw new Exception($"No se encontró ningúna vaca con el nombre o coincidencia '{nombre}'.");
                }

                // Si hay más de un resultado, mostrar opciones
                if (res_encontrada.Count > 1)
                {
                    throw new Exception($" se encontró mas de una res con el nombre o coincidencia '{nombre}'.");
                }

                //  devolver potrero
                return res_encontrada.First();
            }
            catch (Exception er)
            {
                throw new Exception("Error inesperado en el método buscar_potrero: " + er.Message);
            }
        }

        public Res retirar(Res res)
        {
            try
            {
                if (res == null)
                    throw new ArgumentNullException(nameof(res));

                var existente = l_reses
                    .FirstOrDefault(r => r.Nombre.Equals(res.Nombre, StringComparison.OrdinalIgnoreCase));

                if (existente == null)
                    throw new InvalidOperationException($"No se encontró ninguna res con el nombre '{res.Nombre}' para eliminar.");

                l_reses.Remove(existente);
                return existente;
            }
            catch (Exception er)
            {
                throw new Exception("Error inesperado en el método retirar: " + er.Message);
            }
        }

        public bool contiene(Res res)
        {
            try
            {
                if (res == null)
                    return false;

                return l_reses.Any(r => r.Nombre.Equals(res.Nombre, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception ex)
            {
                throw new Exception("Error inesperado en el metodo contiene: " + ex.Message);
            }
        }


        //Accesores
        public List<Res> L_reses { get => l_reses; set => l_reses = value; }
        public string Identificacion { get => identificacion; set => identificacion = value; }
        public l_tipos_potreros Tipo_potrero { get => tipo_potrero; set => tipo_potrero = value; }

    }
}
