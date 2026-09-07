using Bib_Hacienda.Clases.Creacion;
using Bib_Hacienda.enums;
using Bib_Hacienda.Eventos;
using Bib_Hacienda.Interfaces;
using Bib_Hacienda.Reglas;
using System;
using System.Collections.Generic;
using System.Linq;
using static Bib_Hacienda.Clases.Potrero;
 
namespace Bib_Hacienda.Clases
{
    // Fachada principal de coordinación. SRP: delega, no implementa.
    // - Crear potreros / buscar potreros / mover reses -> se hace via Potrero
    // - Vender -> se hace via RegistroVenta
    // - Crear vacunas -> se hace via FabricadorVacunas
    // - Aplicar vacunas -> sigue aquí (orquesta Potrero + Res + eventos)
    public class Hacienda : IVacunacion, IVenta, ICreacionVacuna
    {
        //Atributos
        private List<Potrero> l_potreros;

        private List<IInventario<Producto>> l_inventarios;
        private readonly RegistroVenta registroVentas;
        private List<Vacuna> l_vacunas;
        private readonly FabricadorVacunas fabricadorVacunas;
        private readonly CatalogoCreadoresRes catalogoCreadoresRes;

        //Accesores públicos para los servicios (get público, set privado)
        public List<Potrero> L_potreros
        {
            get => l_potreros;
            private set => l_potreros = value;
        }

        public IReadOnlyList<Venta> L_ventas => registroVentas.Ventas;

        public List<Vacuna> L_vacunas
        {
            get => l_vacunas;
            private set => l_vacunas = value;
        }

        //Eventos
        private readonly PublisherVacunacionCompletada publisher_vacunacion_completa = new PublisherVacunacionCompletada();
        private readonly PublisherVacunaVencida publisher_vacuna_vencida = new PublisherVacunaVencida();
        private readonly PublisherPesoMin publisher_peso_min = new PublisherPesoMin();
        private readonly PublisherPesoVenta publisher_peso_ideal = new PublisherPesoVenta();

        // Unico observador de los publishers de esta hacienda. Se suscribe en el
        // constructor y no se vuelve a tocar durante las operaciones.
        private readonly RecolectorMensajes recolectorMensajes = new RecolectorMensajes();


        //EventHandler
        internal void EventHandler() { }

        //Constructor por defecto (composition root sin DI externo):
        //crea las dependencias mínimas para conservar la API observable.
        public Hacienda()
            : this(registroVentas: null, fabricadorVacunas: null)
        {
        }

        // Constructor que permite proporcionar colaboradores desde la raíz de
        // composición; el constructor vacío se conserva por compatibilidad.
        public Hacienda(RegistroVenta registroVentas, FabricadorVacunas fabricadorVacunas)
            : this(registroVentas, fabricadorVacunas, catalogoCreadoresRes: null)
        {
        }

        // Sobrecarga que recibe además el catálogo de creadores de res; las dos
        // anteriores se conservan para los consumidores existentes.
        public Hacienda(RegistroVenta registroVentas, FabricadorVacunas fabricadorVacunas, CatalogoCreadoresRes catalogoCreadoresRes)
        {
            l_potreros = new List<Potrero>();
            this.registroVentas = registroVentas ?? new RegistroVenta();
            this.fabricadorVacunas = fabricadorVacunas ?? new FabricadorVacunas(new List<Vacuna>());
            this.catalogoCreadoresRes = catalogoCreadoresRes ?? CatalogoCreadoresRes.PorDefecto();
            l_vacunas = this.fabricadorVacunas.L_vacunas;

            // Observer: la suscripcion ocurre una sola vez, aqui. Repetir una
            // operacion ya no agrega handlers al publisher.
            publisher_peso_min.evt_peso_min += recolectorMensajes.Recibir;
            publisher_peso_ideal.evt_peso_venta += recolectorMensajes.Recibir;
            publisher_vacunacion_completa.evt_vacunacion_completada += recolectorMensajes.Recibir;
        }

        //Metodo para crear potreros
        public string crear_potrero(string indentificacion, l_tipos_potreros tipo_potrero)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(indentificacion))
                {
                    throw new ArgumentException("El nombre de la res no puede estar vacío", nameof(indentificacion));
                }
                if (l_potreros.Any(p => p.Identificacion.Equals(indentificacion, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException($"Ya existe un potrero con el nombre '{indentificacion}'.");
                }

                Potrero nuevo_potrero = new Potrero(indentificacion, tipo_potrero);
                nuevo_potrero.Suscribir(recolectorMensajes);
                l_potreros.Add(nuevo_potrero);

                return ($"El potrero {indentificacion} se a añadido a la hacienda. ");

            }
            catch (Exception er)
            {
                throw new Exception("Error inesperado en el metodo crear_potrero: " + er.Message);
            }
        }

        //Metodo para buscar potreros por el nombre
        public Potrero buscar_potrero(string nombre)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    throw new ArgumentException("El nombre de búsqueda no puede estar vacío.");
                }

                var potreros_encontrados = l_potreros
                    .Where(p => p.Identificacion.IndexOf(nombre, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();

                if (potreros_encontrados.Count == 0)
                {
                    throw new Exception($"No se encontró ningún potrero con el nombre o coincidencia '{nombre}'.");
                }

                if (potreros_encontrados.Count > 1)
                {
                    throw new Exception($" se encontró mas de un potrero con el nombre o coincidencia '{nombre}'.");
                }

                return potreros_encontrados.First();
            }
            catch (Exception er)
            {
                throw new Exception("Error inesperado en el método buscar_potrero: " + er.Message);
            }
        }

       public IInventario<Producto> buscar_inventario(string nombre)
        {
            try
            {
              if(nombre == null || string.IsNullOrWhiteSpace(nombre) )
                {
                    throw new ArgumentException("El nombre de búsqueda no puede estar vacío.");
                }

                IInventario<Producto> inventario = l_inventarios.Find(inv => inv.ToString().IndexOf(nombre, StringComparison.OrdinalIgnoreCase) >= 0);

                if (inventario == null)
                {
                    throw new Exception($"No se encontró ningún inventario con el nombre o coincidencia '{nombre}'.");
                }
                return inventario;
            }
            catch (System.Exception)
            {
                
                throw;
            }
        }  

        //Metodo para  anadir res a un potrero
        public string anadir_res_potrero (string id_potrero, string nombre, ushort edad, uint peso)
        {
            try
            {
                Potrero potrero = buscar_potrero(id_potrero);

                Res res = catalogoCreadoresRes.ParaEdad(edad).Crear(nombre, peso, edad);

                string mensajesEventos;
                using (CapturaMensajes captura = recolectorMensajes.Capturar())
                {
                    potrero.agregar(res);
                    mensajesEventos = captura.Texto();
                }

                string mensajeFinal = $"La res {nombre} ha sido añadida al potrero {potrero.Identificacion} con exito.";
                if (!string.IsNullOrEmpty(mensajesEventos))
                {
                    mensajeFinal += "\n" + mensajesEventos.TrimEnd();
                }

                return mensajeFinal;
            }
            catch (Exception er)
            {
                throw new Exception("Error inesperado en el método anadir_res_potrero: " + er.Message);
            }
        }

        //metodo para vender
        public string vender<T>(IInventario<T> inventario, T producto, uint monto)
            where T : Producto
        { 
            if (inventario == null)
                throw new ArgumentNullException(nameof(inventario));

            if (producto == null)
                throw new ArgumentNullException(nameof(producto));

            if (!inventario.contiene(producto))
                throw new InvalidOperationException(
                    $"El producto '{producto.Nombre}' no se encuentra en el inventario.");

            T productoRetirado = inventario.retirar(producto);

            Venta venta = new Venta(
                DateTime.Now,
                productoRetirado,
                monto
            );

            registroVentas.registrar(venta);

            return $"Venta de la res {productoRetirado.Nombre} realizada con exito";
        }
                  
        //Metodo para alimentar una res
        public string alimentar_res(string id_potrero, string nombre, uint cantidad)
        {
            try
            {
                Potrero potrero = buscar_potrero(id_potrero);
                Res res = potrero.buscar_res(nombre);

                if (potrero == null) throw new ArgumentNullException(nameof(potrero));
                if (res == null) throw new ArgumentNullException(nameof(res));

                res.Alimentar(cantidad);

                string mensaje_eventos;

                using (CapturaMensajes captura = recolectorMensajes.Capturar())
                {
                    publisher_peso_min.Informar_Peso_Min(res);
                    publisher_peso_ideal.Informar_Peso_Venta(res);

                    mensaje_eventos = captura.Texto();
                }

                string mensaje_final = $"La res '{res.Nombre}' ha sido alimentada, ahora pesa {res.Peso} kg.";
                if (!string.IsNullOrEmpty(mensaje_eventos))
                {
                    mensaje_final += "\n" + mensaje_eventos.TrimEnd();
                }

                return mensaje_final;
            }
            catch (Exception er)
            {
                throw new Exception("Error inesperado en el metodo alimentar_res: " + er.Message);
            }
        }

        // —— Creación de vacunas: delega a FabricadorVacunas (SRP). ——

        //Metodo para crear y añadir vacuna al inventario
        public string crear_vacuna(string nombre, string lote, DateTime fecha_vencimiento, DateTime fecha_aplicacion, uint periodo_aplicacion)
        {
            return fabricadorVacunas.Crear(nombre, lote, fecha_vencimiento, fecha_aplicacion, periodo_aplicacion);
        }

        //Metodo para crear vacuna viva individual
        public string crear_vacuna(string nombre, string lote, DateTime fecha_vencimiento, DateTime fecha_aplicacion, Atenuaciones grado_atenuacion)
        {
            return fabricadorVacunas.Crear(nombre, lote, fecha_vencimiento, fecha_aplicacion, grado_atenuacion);
        }

        //Metodo para crear lote de vacunas bacterianas
        public string crear_vacuna(string nombre, string lote_base, DateTime fecha_vencimiento, DateTime fecha_aplicacion, uint periodo_aplicacion, uint cantidad)
        {
            return fabricadorVacunas.CrearLote(nombre, lote_base, fecha_vencimiento, fecha_aplicacion, periodo_aplicacion, cantidad);
        }

        //Metodo para crear lote de vacunas vivas
        public string crear_vacuna(string nombre, string lote_base, DateTime fecha_vencimiento, DateTime fecha_aplicacion, Atenuaciones grado_atenuacion, uint cantidad)
        {
            return fabricadorVacunas.CrearLote(nombre, lote_base, fecha_vencimiento, fecha_aplicacion, grado_atenuacion, cantidad);
        }

        //Metodo nuevo para aplicar vacuna - hacienda
        public string aplicar_vacuna(Vacuna vacuna, string nombre, string idPotrero)
        {
            Potrero potrero = buscar_potrero(idPotrero);
            Res res = potrero.buscar_res(nombre);

            if (vacuna == null)
                throw new ArgumentNullException(nameof(vacuna));

            res.aplicar_vacuna(vacuna);

            L_vacunas.Remove(vacuna);

            string mensaje_vacunacion;

            using (CapturaMensajes captura = recolectorMensajes.Capturar())
            {
                publisher_vacunacion_completa.Informar_Vacunacion_Completada(
                    res,
                    res.CantidadVacunasBacterianas,
                    res.CantidadVacunasVivas);

                mensaje_vacunacion = captura.Texto();
            }

            return $"Vacuna aplicada correctamente a la res {res.Nombre}. {mensaje_vacunacion}";
        }
    }
}
