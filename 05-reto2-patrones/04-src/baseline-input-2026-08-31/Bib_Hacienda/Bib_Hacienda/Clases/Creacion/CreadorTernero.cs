using Bib_Hacienda.Interfaces;
using Bib_Hacienda.Reglas;

namespace Bib_Hacienda.Clases.Creacion
{
    public class CreadorTernero : ICreadorRes
    {
        public string Categoria => nameof(Ternero);

        public bool AplicaA(ushort edad) => edad <= ReglaRes.edad_max_ternero;

        public Res Crear(string nombre, uint peso, ushort edad)
        {
            return new Ternero(nombre, peso, edad);
        }
    }
}
