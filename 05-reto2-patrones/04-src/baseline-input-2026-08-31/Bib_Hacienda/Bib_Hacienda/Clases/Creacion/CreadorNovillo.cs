using Bib_Hacienda.Interfaces;
using Bib_Hacienda.Reglas;

namespace Bib_Hacienda.Clases.Creacion
{
    public class CreadorNovillo : ICreadorRes
    {
        public string Categoria => nameof(Novillo);

        public bool AplicaA(ushort edad) => edad > ReglaRes.edad_max_cebon;

        public Res Crear(string nombre, uint peso, ushort edad)
        {
            return new Novillo(nombre, peso, edad);
        }
    }
}
