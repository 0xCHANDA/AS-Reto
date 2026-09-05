using Bib_Hacienda.Interfaces;
using Bib_Hacienda.Reglas;

namespace Bib_Hacienda.Clases.Creacion
{
    public class CreadorCebon : ICreadorRes
    {
        public string Categoria => nameof(Cebon);

        public bool AplicaA(ushort edad) =>
            edad > ReglaRes.edad_max_ternero && edad <= ReglaRes.edad_max_cebon;

        public Res Crear(string nombre, uint peso, ushort edad)
        {
            return new Cebon(nombre, peso, edad);
        }
    }
}
