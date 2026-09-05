using Bib_Hacienda.Clases;
using Bib_Hacienda.Interfaces;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace p_mvcHacienda.Servicios
{
    public class UsuarioService
    {
        private static List<Usuario> _usuarios = new List<Usuario>();
        private readonly IPersistenciaUsuarios _persistenciaUsuarios;

        public UsuarioService(IPersistenciaUsuarios persistenciaUsuarios)
        {
            _persistenciaUsuarios = persistenciaUsuarios;
        }

        public void CargarUsuarios()
        {
            _usuarios = _persistenciaUsuarios.CargarUsuarios();
        }

        public string CrearUsuario(string nombre, string contrasena)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    throw new ArgumentException("El nombre del usuario no puede estar vacío");
                }

                if (string.IsNullOrWhiteSpace(contrasena))
                {
                    throw new ArgumentException("La contraseña no puede estar vacía");
                }

                if (_usuarios.Any(u => u.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException($"Ya existe un usuario con el nombre '{nombre}'");
                }

                var nuevoUsuario = new Usuario(nombre, contrasena);
                _usuarios.Add(nuevoUsuario);
                _persistenciaUsuarios.GuardarUsuarios(_usuarios);

                return $"Usuario '{nombre}' creado exitosamente";
            }
            catch (Exception ex)
            {
                return $"{ex.Message}";
            }
        }

        public bool AutenticarUsuario(string nombre, string contrasena)
        {
            return _usuarios.Any(u => u.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase) &&
            u.Contrasena == contrasena);
        }

        public List<Usuario> ObtenerTodosLosUsuarios()
        {
            return _usuarios.OrderBy(u => u.Nombre).ToList();
        }

        public Usuario? BuscarUsuario(string nombre)
        {
            return _usuarios.FirstOrDefault(u => u.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase));
        }

        public Dictionary<string, object> ObtenerEstadisticas()
        {
            return new Dictionary<string, object>
            {
                {"TotalUsuarios", _usuarios.Count}
            };
        }

        public async Task<(bool Success, IEnumerable<Claim>? Claims)> ValidateUserAsync(string username, string password)
        {
            var user = _usuarios.FirstOrDefault(u => u.Nombre.Equals(username, StringComparison.OrdinalIgnoreCase) && u.Contrasena == password);

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Nombre),
                };
                return (true, claims);
            }

            return (false, null);
        }
    }
}
