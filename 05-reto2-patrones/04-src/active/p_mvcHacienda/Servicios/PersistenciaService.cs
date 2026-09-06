using Bib_Hacienda.Clases;
using Bib_Hacienda.Clases.Creacion;
using Bib_Hacienda.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Bib_Hacienda.enums;
using static Bib_Hacienda.Clases.Potrero;
using System.Globalization;

namespace p_mvcHacienda.Servicios
{
    // Servicio de persistencia con responsabilidad única: leer/escribir archivos.
    // Implementa los puertos de persistencia definidos en Bib_Hacienda.Interfaces
    // y recibe los validadores ya decorados desde la raíz de composición.
    public class PersistenciaService :
        IPersistenciaPotreros,
        IPersistenciaReses,
        IPersistenciaVacunas,
        IPersistenciaVentas,
        IPersistenciaUsuarios,
        IPersistenciaEventosClinicos
    {
        private readonly string _directorioArchivos;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IValidadorPotrero _validadorPotrero;
        private readonly IValidadorRes _validadorRes;
        private readonly IValidadorVacuna _validadorVacuna;
        private readonly IValidadorVenta _validadorVenta;

        private readonly CatalogoCreadoresRes _catalogoCreadoresRes;

        public PersistenciaService(
            IWebHostEnvironment env,
            IHttpContextAccessor httpContextAccessor,
            IValidadorPotrero validadorPotrero,
            IValidadorRes validadorRes,
            IValidadorVacuna validadorVacuna,
            IValidadorVenta validadorVenta,
            CatalogoCreadoresRes catalogoCreadoresRes)
        {
            _directorioArchivos = Path.Combine(env.ContentRootPath, "Datos");

            if (!Directory.Exists(_directorioArchivos))
            {
                Directory.CreateDirectory(_directorioArchivos);
            }

            _httpContextAccessor = httpContextAccessor;

            // Los validadores ya están compuestos con el interceptor en Program.cs;
            // este servicio no debe construir proxies ni interceptores.
            _validadorPotrero = validadorPotrero;
            _validadorRes = validadorRes;
            _validadorVacuna = validadorVacuna;
            _validadorVenta = validadorVenta;

            // Reconstruir reses es crear reses: se reutilizan los creadores en
            // vez de conocer aqui las clases concretas.
            _catalogoCreadoresRes = catalogoCreadoresRes;
        }

        #region IPersistenciaPotreros

        public string GuardarPotreros(List<Potrero> potreros)
        {
            try
            {
                foreach (var potrero in potreros)
                {
                    if (!_validadorPotrero.ValidarPotrero(potrero))
                    {
                        var mensaje = _httpContextAccessor.HttpContext?.Items["ResultadoValidacion"]?.ToString();
                        return mensaje ?? "Error de validación en potrero";
                    }
                }

                var lineas = potreros.Select(p => $"{p.Identificacion}|{p.Tipo_potrero}");
                File.WriteAllLines(Path.Combine(_directorioArchivos, "Potreros.txt"), lineas);

                return _httpContextAccessor.HttpContext?.Items["ResultadoValidacion"]?.ToString()
                    ?? "Guardado exitosamente";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al guardar potreros: {ex.Message}", ex);
            }
        }

        public List<Potrero> CargarPotreros()
        {
            try
            {
                string rutaArchivo = Path.Combine(_directorioArchivos, "Potreros.txt");

                if (!File.Exists(rutaArchivo))
                {
                    return new List<Potrero>();
                }

                var potreros = new List<Potrero>();
                var lineas = File.ReadAllLines(rutaArchivo);

                foreach (var linea in lineas)
                {
                    if (string.IsNullOrWhiteSpace(linea)) continue;

                    var partes = linea.Split('|');
                    if (partes.Length >= 2)
                    {
                        string identificacion = partes[0].Trim();
                        l_tipos_potreros tipo = Enum.Parse<l_tipos_potreros>(partes[1]);

                        if (!potreros.Any(p => string.Equals(p.Identificacion, identificacion, StringComparison.OrdinalIgnoreCase)))
                        {
                            potreros.Add(new Potrero(identificacion, tipo));
                        }
                    }
                }

                return potreros;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cargar potreros: {ex.Message}");
            }
        }

        #endregion

        #region IPersistenciaReses

        public string GuardarReses(List<Potrero> potreros)
        {
            try
            {
                var lineas = new List<string>();

                foreach (var potrero in potreros)
                {
                    foreach (var res in potrero.L_reses)
                    {
                        if (!_validadorRes.ValidarRes(res))
                        {
                            var mensaje = _httpContextAccessor.HttpContext?.Items["ResultadoValidacion"]?.ToString();
                            return mensaje ?? "Error de validación en res";
                        }

                        string tipoRes = res.GetType().Name;
                        lineas.Add($"{potrero.Identificacion}|{res.Nombre}|{res.Peso}|{res.Edad}|{tipoRes}");
                    }
                }

                File.WriteAllLines(Path.Combine(_directorioArchivos, "Reses.txt"), lineas);

                return _httpContextAccessor.HttpContext?.Items["ResultadoValidacion"]?.ToString()
                    ?? "Guardado exitosamente";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al guardar reses: {ex.Message}", ex);
            }
        }

        public void CargarReses(List<Potrero> potreros)
        {
            try
            {
                string rutaArchivo = Path.Combine(_directorioArchivos, "Reses.txt");

                if (!File.Exists(rutaArchivo))
                {
                    return;
                }

                var lineas = File.ReadAllLines(rutaArchivo);

                foreach (var linea in lineas)
                {
                    if (string.IsNullOrWhiteSpace(linea)) continue;

                    var partes = linea.Split('|');
                    if (partes.Length >= 5)
                    {
                        string nombrePotrero = partes[0].Trim();
                        string nombreRes = partes[1];
                        uint peso = uint.Parse(partes[2]);
                        ushort edad = ushort.Parse(partes[3]);

                        var potrero = potreros.FirstOrDefault(p => string.Equals(p.Identificacion, nombrePotrero, StringComparison.OrdinalIgnoreCase));
                        if (potrero != null)
                        {
                            Res res = _catalogoCreadoresRes.ParaEdad(edad).Crear(nombreRes, peso, edad);
                            potrero.agregar(res);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cargar reses: {ex.Message}");
            }
        }

        #endregion

        #region IPersistenciaVacunas

        public string GuardarVacunas(List<Vacuna> vacunas)
        {
            try
            {
                foreach (var vacuna in vacunas)
                {
                    if (!_validadorVacuna.ValidarVacuna(vacuna))
                    {
                        var mensaje = _httpContextAccessor.HttpContext?.Items["ResultadoValidacion"]?.ToString();
                        return mensaje ?? "Error de validación en vacuna";
                    }
                }

                var lineas = new List<string>();
                foreach (var vacuna in vacunas)
                {
                    string fechaVenc = vacuna.Fecha_vencimiento.ToString("yyyy-MM-dd");
                    string fechaAplic = vacuna.Fecha_aplicacion.ToString("yyyy-MM-dd");
                    string tipo = vacuna.GetType().Name;
                    uint periodo = vacuna is Bacteriana bacteriana ? bacteriana.Periodo_aplicacion : 0;

                    lineas.Add($"{vacuna.Nombre}|{vacuna.Lote}|{fechaVenc}|{fechaAplic}|{tipo}|{periodo}");
                }

                File.WriteAllLines(Path.Combine(_directorioArchivos, "Vacunas.txt"), lineas);

                return _httpContextAccessor.HttpContext?.Items["ResultadoValidacion"]?.ToString()
                    ?? "Guardado exitosamente";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al guardar vacunas: {ex.Message}", ex);
            }
        }

        public List<Vacuna> CargarVacunas()
        {
            try
            {
                string rutaArchivo = Path.Combine(_directorioArchivos, "Vacunas.txt");

                if (!File.Exists(rutaArchivo))
                {
                    return new List<Vacuna>();
                }

                var vacunas = new List<Vacuna>();
                var lineas = File.ReadAllLines(rutaArchivo);

                foreach (var linea in lineas)
                {
                    if (string.IsNullOrWhiteSpace(linea)) continue;

                    var partes = linea.Split('|');
                    if (partes.Length >= 6)
                    {
                        string nombre = partes[0];
                        string lote = partes[1];
                        if (!DateTime.TryParseExact(partes[2].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaVenc))
                        {
                            continue;
                        }
                        if (!DateTime.TryParseExact(partes[3].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaAplic))
                        {
                            continue;
                        }
                        string tipo = partes[4].Trim();
                        uint periodo = uint.TryParse(partes[5].Trim(), out uint per) ? per : 0u;

                        Vacuna vacuna;
                        if (tipo.Equals("Bacteriana", StringComparison.OrdinalIgnoreCase))
                        {
                            if (!uint.TryParse(partes[5].Trim(), out periodo) || periodo < 2 || periodo > 4)
                            {
                                continue;
                            }
                            try
                            {
                                vacuna = new Bacteriana(nombre, lote, fechaVenc, fechaAplic, periodo);
                            }
                            catch
                            {
                                continue;
                            }
                        }
                        else
                        {
                            vacuna = new Viva(nombre, lote, fechaVenc, fechaAplic, Atenuaciones.Atenuacion10);
                        }

                        vacunas.Add(vacuna);
                    }
                }

                return vacunas;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cargar vacunas: {ex.Message}");
            }
        }

        public string GuardarVacunasAplicadas(List<Potrero> potreros)
        {
            try
            {
                var lineas = new List<string>();

                foreach (var potrero in potreros)
                {
                    foreach (var res in potrero.L_reses)
                    {
                        if (!_validadorRes.ValidarRes(res))
                        {
                            var mensaje = _httpContextAccessor.HttpContext?.Items["ResultadoValidacion"]?.ToString();
                            return mensaje ?? "Error de validación en res";
                        }

                        foreach (var vacuna in res.L_vacunas_aplicadas)
                        {
                            if (!_validadorVacuna.ValidarVacuna(vacuna))
                            {
                                var mensaje = _httpContextAccessor.HttpContext?.Items["ResultadoValidacion"]?.ToString();
                                return mensaje ?? "Error de validación en vacuna aplicada";
                            }

                            string fechaVenc = vacuna.Fecha_vencimiento.ToString("yyyy-MM-dd");
                            string fechaAplic = vacuna.Fecha_aplicacion.ToString("yyyy-MM-dd");
                            string tipo = vacuna.GetType().Name;
                            uint periodo = vacuna is Bacteriana bacteriana ? bacteriana.Periodo_aplicacion : 0;

                            lineas.Add($"{potrero.Identificacion}|{res.Nombre}|{vacuna.Nombre}|{vacuna.Lote}|{fechaVenc}|{fechaAplic}|{tipo}|{periodo}");
                        }
                    }
                }

                File.WriteAllLines(Path.Combine(_directorioArchivos, "VacunasAplicadas.txt"), lineas);

                return _httpContextAccessor.HttpContext?.Items["ResultadoValidacion"]?.ToString()
                    ?? "Guardado exitosamente";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al guardar vacunas aplicadas: {ex.Message}", ex);
            }
        }

        public void CargarVacunasAplicadas(List<Potrero> potreros)
        {
            try
            {
                string rutaArchivo = Path.Combine(_directorioArchivos, "VacunasAplicadas.txt");

                if (!File.Exists(rutaArchivo))
                {
                    return;
                }

                var lineas = File.ReadAllLines(rutaArchivo);

                foreach (var linea in lineas)
                {
                    if (string.IsNullOrWhiteSpace(linea)) continue;

                    var partes = linea.Split('|');
                    if (partes.Length >= 8)
                    {
                        string nombrePotrero = partes[0].Trim();
                        string nombreRes = partes[1];
                        string nombreVacuna = partes[2];
                        string lote = partes[3];
                        if (!DateTime.TryParseExact(partes[4].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaVenc))
                        {
                            continue;
                        }
                        if (!DateTime.TryParseExact(partes[5].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaAplic))
                        {
                            continue;
                        }
                        string tipo = partes[6];
                        uint periodo = uint.TryParse(partes[7].Trim(), out uint per) ? per : 0u;

                        var potrero = potreros.FirstOrDefault(p => string.Equals(p.Identificacion, nombrePotrero, StringComparison.OrdinalIgnoreCase));
                        if (potrero != null)
                        {
                            var res = potrero.buscar_res(nombreRes);
                            if (res != null)
                            {
                                Vacuna vacuna;
                                if (tipo == "Bacteriana")
                                {
                                    vacuna = new Bacteriana(nombreVacuna, lote, fechaVenc, fechaAplic, periodo);
                                }
                                else
                                {
                                    vacuna = new Viva(nombreVacuna, lote, fechaVenc, fechaAplic, Atenuaciones.Atenuacion10);
                                }

                                res.L_vacunas_aplicadas.Add(vacuna);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cargar vacunas aplicadas: {ex.Message}");
            }
        }

        #endregion

        #region IPersistenciaEventosClinicos

        // SC-3: los eventos clinicos se guardan en su propio archivo, con el
        // mismo formato separado por '|' que el resto de la persistencia. Las
        // vacunas siguen viviendo en VacunasAplicadas.txt: la historia clinica
        // conserva las dos colecciones sin duplicar el mismo hecho.
        public string GuardarEventosClinicos(List<Potrero> potreros)
        {
            try
            {
                var lineas = new List<string>();

                foreach (var potrero in potreros)
                {
                    foreach (var res in potrero.L_reses)
                    {
                        foreach (var evento in res.HistoriaClinica.EventosClinicos)
                        {
                            string fecha = evento.Fecha.ToString("yyyy-MM-dd");

                            lineas.Add($"{potrero.Identificacion}|{res.Nombre}|{fecha}|{Sanitizar(evento.Concepto)}|{Sanitizar(evento.Observacion)}");
                        }
                    }
                }

                File.WriteAllLines(Path.Combine(_directorioArchivos, "EventosClinicos.txt"), lineas);

                return "Guardado exitosamente";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al guardar eventos clinicos: {ex.Message}", ex);
            }
        }

        public void CargarEventosClinicos(List<Potrero> potreros)
        {
            try
            {
                string rutaArchivo = Path.Combine(_directorioArchivos, "EventosClinicos.txt");

                if (!File.Exists(rutaArchivo))
                {
                    return;
                }

                var lineas = File.ReadAllLines(rutaArchivo);

                foreach (var linea in lineas)
                {
                    if (string.IsNullOrWhiteSpace(linea)) continue;

                    var partes = linea.Split('|');
                    if (partes.Length >= 5)
                    {
                        string nombrePotrero = partes[0].Trim();
                        string nombreRes = partes[1];

                        if (!DateTime.TryParseExact(partes[2].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fecha))
                        {
                            continue;
                        }

                        string concepto = partes[3];
                        string observacion = partes[4];

                        var potrero = potreros.FirstOrDefault(p => string.Equals(p.Identificacion, nombrePotrero, StringComparison.OrdinalIgnoreCase));
                        if (potrero != null)
                        {
                            var res = potrero.buscar_res(nombreRes);
                            if (res != null)
                            {
                                res.HistoriaClinica.RegistrarEvento(
                                    new EventoClinico(fecha, concepto, observacion));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cargar eventos clinicos: {ex.Message}");
            }
        }

        // El separador del formato de archivo no puede aparecer dentro de un campo.
        private static string Sanitizar(string texto)
        {
            return texto.Replace('|', '/').Replace('\n', ' ').Replace('\r', ' ').Trim();
        }

        #endregion

        #region IPersistenciaVentas

        public string GuardarVentas(List<Venta> ventas)
        {
            try
            {
                foreach (var venta in ventas)
                {
                    if (!_validadorVenta.ValidarVenta(venta))
                    {
                        var mensaje = _httpContextAccessor.HttpContext?.Items["ResultadoValidacion"]?.ToString();
                        return mensaje ?? "Error de validación en venta";
                    }
                }

                var lineas = new List<string>();
                foreach (var venta in ventas)
                {
                    string fecha = venta.Fecha.ToString("yyyy-MM-dd");

                    // Formato V2: snapshot genérico de cualquier Producto. Es el
                    // unico formato de escritura: Venta ya no conoce potrero ni res,
                    // solo el Producto vendido. La lectura sigue aceptando el
                    // formato legacy de 7 campos.
                    // No requiere modificar este servicio cuando aparezcan nuevos
                    // subtipos de Producto (OCP).
                    if (venta.Producto != null)
                    {
                        var producto = venta.Producto;
                        string tipo = producto.GetType().Name;
                        if (producto is Res res)
                        {
                            lineas.Add($"V2|{fecha}|{venta.Monto}|{tipo}|{res.Nombre}|{res.Peso}|{res.Edad}");
                        }
                        else
                        {
                            lineas.Add($"V2|{fecha}|{venta.Monto}|{tipo}|{producto.Nombre}");
                        }
                    }
                }

                File.WriteAllLines(Path.Combine(_directorioArchivos, "Ventas.txt"), lineas);

                return _httpContextAccessor.HttpContext?.Items["ResultadoValidacion"]?.ToString()
                    ?? "Guardado exitosamente";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al guardar ventas: {ex.Message}", ex);
            }
        }

        public List<Venta> CargarVentas(List<Potrero> potreros)
        {
            try
            {
                string rutaArchivo = Path.Combine(_directorioArchivos, "Ventas.txt");

                if (!File.Exists(rutaArchivo))
                {
                    return new List<Venta>();
                }

                var ventas = new List<Venta>();
                var lineas = File.ReadAllLines(rutaArchivo);

                foreach (var linea in lineas)
                {
                    if (string.IsNullOrWhiteSpace(linea)) continue;

                    var partes = linea.Split('|');
                    if (partes.Length == 0) continue;

                    // Formato V2: snapshot genérico de cualquier Producto.
                    if (partes[0].Equals("V2", StringComparison.OrdinalIgnoreCase) && partes.Length >= 5)
                    {
                        if (!DateTime.TryParseExact(partes[1].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fecha))
                        {
                            continue;
                        }

                        if (!uint.TryParse(partes[2].Trim(), out uint monto) || monto == 0)
                        {
                            continue;
                        }

                        string tipo = partes[3].Trim();
                        string nombre = partes[4];

                        ICreadorRes creadorRes = _catalogoCreadoresRes.ParaCategoria(tipo);

                        Producto producto;
                        if (creadorRes != null
                            && partes.Length >= 7
                            && uint.TryParse(partes[5].Trim(), out uint pesoRes) && pesoRes > 0
                            && ushort.TryParse(partes[6].Trim(), out ushort edadRes) && edadRes > 0)
                        {
                            producto = creadorRes.Crear(nombre, pesoRes, edadRes);
                        }
                        else if (tipo.Equals("Lacteo", StringComparison.OrdinalIgnoreCase))
                        {
                            producto = new Lacteo(nombre);
                        }
                        else if (tipo.Equals("Piel", StringComparison.OrdinalIgnoreCase))
                        {
                            producto = new Piel(nombre);
                        }
                        else
                        {
                            // Subtipos desconocidos se recargan como snapshot estable;
                            // no es necesario modificar este servicio para cada nuevo tipo.
                            producto = new ProductoPersistido(tipo, nombre);
                        }

                        ventas.Add(new Venta(fecha, producto, monto));
                        continue;
                    }

                    // Formato legacy: 7 campos, la primera posición es el potrero.
                    // Se conserva la semántica original de Parse: número malformado
                    // aborta la carga, no se omite silenciosamente.
                    if (partes.Length >= 7)
                    {
                        if (!DateTime.TryParseExact(partes[1].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fecha))
                        {
                            continue;
                        }
                        string resNombre = partes[2];
                        uint resPeso = uint.Parse(partes[3].Trim());
                        ushort resEdad = ushort.Parse(partes[4].Trim());
                        string resTipo = partes[5];
                        uint monto = uint.Parse(partes[6].Trim());

                        // Respaldo legacy conservado: un tipo desconocido se recarga
                        // como Ternero, igual que antes del catalogo.
                        ICreadorRes creadorLegacy = _catalogoCreadoresRes.ParaCategoria(resTipo)
                            ?? _catalogoCreadoresRes.ParaCategoria(nameof(Ternero));

                        Res res = creadorLegacy.Crear(resNombre, resPeso, resEdad);

                        // Venta ya no guarda el potrero de origen: el modelo actual
                        // registra el Producto vendido. El campo de potrero de la
                        // linea legacy (partes[0]) se ignora.
                        ventas.Add(new Venta(fecha, res, monto));
                    }
                }

                return ventas;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cargar ventas: {ex.Message}");
            }
        }

        #endregion

        #region IPersistenciaUsuarios

        public string GuardarUsuarios(List<Usuario> usuarios)
        {
            try
            {
                foreach (var usuario in usuarios)
                {
                    if (string.IsNullOrWhiteSpace(usuario.Nombre) || string.IsNullOrWhiteSpace(usuario.Contrasena))
                    {
                        return "Error: Usuario debe tener nombre y contraseña";
                    }
                }

                var lineas = usuarios.Select(u => $"{u.Nombre}|{u.Contrasena}");
                File.WriteAllLines(Path.Combine(_directorioArchivos, "Usuarios.txt"), lineas);

                return "Guardado exitosamente";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al guardar usuarios: {ex.Message}", ex);
            }
        }

        public List<Usuario> CargarUsuarios()
        {
            try
            {
                string rutaArchivo = Path.Combine(_directorioArchivos, "Usuarios.txt");

                if (!File.Exists(rutaArchivo))
                {
                    return new List<Usuario>();
                }

                var usuarios = new List<Usuario>();
                var lineas = File.ReadAllLines(rutaArchivo);

                foreach (var linea in lineas)
                {
                    if (string.IsNullOrWhiteSpace(linea)) continue;

                    var partes = linea.Split('|');
                    if (partes.Length >= 2)
                    {
                        string nombre = partes[0];
                        string contrasena = partes[1];
                        usuarios.Add(new Usuario(nombre, contrasena));
                    }
                }

                return usuarios;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar usuarios: {ex.Message}");
                return new List<Usuario>();
            }
        }

        #endregion
    }
}
