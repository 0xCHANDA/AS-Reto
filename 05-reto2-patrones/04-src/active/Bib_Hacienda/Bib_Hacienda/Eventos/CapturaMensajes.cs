using System;
using System.Collections.Generic;

namespace Bib_Hacienda.Eventos
{
    // Ventana de recoleccion de una sola operacion. Al cerrarse deja de recibir
    // avisos, pero conserva los suyos para que quien la abrio los lea.
    public sealed class CapturaMensajes : IDisposable
    {
        private readonly RecolectorMensajes recolector;
        private readonly List<string> mensajes;
        private bool cerrada;

        internal CapturaMensajes(RecolectorMensajes recolector, List<string> mensajes)
        {
            this.recolector = recolector;
            this.mensajes = mensajes;
        }

        public IReadOnlyList<string> Mensajes => mensajes;

        // Avisos en el orden en que los emitieron los publishers.
        public string Texto()
        {
            return string.Join("\n", mensajes);
        }

        public void Dispose()
        {
            if (cerrada)
                return;

            cerrada = true;
            recolector.Cerrar();
        }
    }
}
