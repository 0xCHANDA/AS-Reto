namespace Bib_Hacienda.Interfaces
{
    // Observer de los publishers de aviso. Todos ellos emiten un unico string,
    // asi que un solo rol de observador cubre a los seis.
    public interface IObservadorMensaje
    {
        void Recibir(string mensaje);
    }
}
