using Bib_Hacienda.Clases;
using Bib_Hacienda.Interfaces;
using Microsoft.AspNetCore.Hosting;
using static Bib_Hacienda.Clases.Potrero;
using static Bib_Hacienda.Clases.Viva;
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
        IPersistenciaUsuarios
    {
        private readonly string _directorioArchivos;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IValidadorPotrero _validadorPotrero;
        private readonly IValidadorRes _validadorRes;
        private readonly IValidadorVacuna _validadorVacuna;
        private readonly IValidadorVenta _validadorVenta;

        public PersistenciaService(
            IWebHostEnvironment env,
            IHttpContextAccessor httpContextAccessor,
            IValidadorPotrero validadorPotrero,
            IValidadorRes validadorRes,
            IValidadorVacuna validadorVacuna,
            IValidadorVenta validadorVenta)
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
                            potrero.anadir_res(nombreRes, edad, peso);
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
                            vacuna = new Viva(nombre, lote, fechaVenc, fechaAplic, enum_l_atenuaciones.Atenuacion10);
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
                                    vacuna = new Viva(nombreVacuna, lote, fechaVenc, fechaAplic, enum_l_atenuaciones.Atenuacion10);
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

                    // Formato legacy: venta de una Res dentro de un Potrero.
                    // Se conserva exactamente la forma de registro anterior para
                    // garantizar compatibilidad con archivos existentes.
                    if (venta.Potrero != null && venta.Res != null)
                    {
                        string tipoRes = venta.Res.GetType().Name;
                        lineas.Add($"{venta.Potrero.Identificacion}|{fecha}|{venta.Res.Nombre}|{venta.Res.Peso}|{venta.Res.Edad}|{tipoRes}|{venta.Monto}");
                        continue;
                    }

                    // Formato TO-BE (V2): snapshot genérico de cualquier Producto.
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

                        Producto producto;
                        if (tipo.Equals("Ternero", StringComparison.OrdinalIgnoreCase)
                            && partes.Length >= 7
                            && uint.TryParse(partes[5].Trim(), out uint pesoT) && pesoT > 0
                            && ushort.TryParse(partes[6].Trim(), out ushort edadT) && edadT > 0)
                        {
                            producto = new Ternero(nombre, pesoT, edadT);
                        }
                        else if (tipo.Equals("Novillo", StringComparison.OrdinalIgnoreCase)
                            && partes.Length >= 7
                            && uint.TryParse(partes[5].Trim(), out uint pesoN) && pesoN > 0
                            && ushort.TryParse(partes[6].Trim(), out ushort edadN) && edadN > 0)
                        {
                            producto = new Novillo(nombre, pesoN, edadN);
                        }
                        else if (tipo.Equals("Cebon", StringComparison.OrdinalIgnoreCase)
                            && partes.Length >= 7
                            && uint.TryParse(partes[5].Trim(), out uint pesoC) && pesoC > 0
                            && ushort.TryParse(partes[6].Trim(), out ushort edadC) && edadC > 0)
                        {
                            producto = new Cebon(nombre, pesoC, edadC);
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
                        string potreroId = partes[0].Trim();
                        if (!DateTime.TryParseExact(partes[1].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fecha))
                        {
                            continue;
                        }
                        string resNombre = partes[2];
                        uint resPeso = uint.Parse(partes[3].Trim());
                        ushort resEdad = ushort.Parse(partes[4].Trim());
                        string resTipo = partes[5];
                        uint monto = uint.Parse(partes[6].Trim());

                        var potrero = potreros.FirstOrDefault(p => string.Equals(p.Identificacion, potreroId, StringComparison.OrdinalIgnoreCase));
                        if (potrero == null)
                        {
                            potrero = new Potrero(potreroId, l_tipos_potreros.ternero);
                        }

                        Res res = resTipo switch
                        {
                            "Ternero" => new Ternero(resNombre, resPeso, resEdad),
                            "Novillo" => new Novillo(resNombre, resPeso, resEdad),
                            "Cebon" => new Cebon(resNombre, resPeso, resEdad),
                            _ => new Ternero(resNombre, resPeso, resEdad)
                        };

                        ventas.Add(new Venta(potrero, fecha, res, monto));
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
