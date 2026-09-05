using Bib_Hacienda.Interfaces;
using System;
using System.Collections.Generic;

namespace Bib_Hacienda.Eventos
{
    // ConcreteObserver. Se suscribe una sola vez a los publishers y solo guarda
    // los avisos mientras una operacion tiene una captura abierta, de modo que
    // ninguna operacion puede leer los mensajes de otra.
    public class RecolectorMensajes : IObservadorMensaje
    {
        private List<string> mensajes;

        public void Recibir(string mensaje)
        {
            // Fuera de una captura el aviso se descarta, igual que cuando el
            // publisher no tenia suscriptores.
            if (mensajes == null)
                return;

            if (string.IsNullOrEmpty(mensaje))
                return;

            mensajes.Add(mensaje);
        }

        public CapturaMensajes Capturar()
        {
            if (mensajes != null)
                throw new InvalidOperationException("Ya hay una captura de mensajes abierta.");

            mensajes = new List<string>();
            return new CapturaMensajes(this, mensajes);
        }

        internal void Cerrar()
        {
            mensajes = null;
        }
    }
}
