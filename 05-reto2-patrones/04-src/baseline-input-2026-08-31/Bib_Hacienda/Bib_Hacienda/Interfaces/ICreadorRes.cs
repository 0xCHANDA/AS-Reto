using Bib_Hacienda.Clases;

namespace Bib_Hacienda.Interfaces
{
    // Creator del Factory Method para la jerarquia de Res. Cada creador concreto
    // conoce una sola categoria: declara a que edades aplica, con que nombre se
    // persiste y como se instancia su subtipo.
    public interface ICreadorRes
    {
        // Discriminador con el que la categoria viaja a los archivos de datos.
        string Categoria { get; }

        // Rango de edad que cubre esta categoria. Permite resolver el creador
        // sin un condicional que crezca con cada categoria nueva.
        bool AplicaA(ushort edad);

        Res Crear(string nombre, uint peso, ushort edad);
    }
}
