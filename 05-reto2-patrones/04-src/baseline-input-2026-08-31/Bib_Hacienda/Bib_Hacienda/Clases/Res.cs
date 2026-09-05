using Bib_Hacienda.Clases;
using Bib_Hacienda.enums;
using Bib_Hacienda.Eventos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bib_Hacienda.Clases
{
    public abstract class Res:Producto
    {

        //Atributos
        private uint peso;
        private ushort edad;
        private List<Vacuna> l_vacunas_aplicadas;

        internal void EventHandler() { }

        //Constructor
        public Res(string nombre, uint peso, ushort edad) : base(nombre)
        {
            this.Peso = peso;
            this.edad = edad;
            this.l_vacunas_aplicadas = new List<Vacuna>();
        }

        // Accesores legacy conservados. El contrato de Res establece que la edad
        // debe pertenecer al rango de la categoría concreta; cada subtipo aplica
        // esa misma regla tanto en construcción como en cambios posteriores.
        public virtual ushort Edad
        {
            get => edad;
            set
            {
                ValidarEdad(value);
                edad = value;
            }
        }

        public List<Vacuna> L_vacunas_aplicadas
        {
            get => l_vacunas_aplicadas;
            set => l_vacunas_aplicadas = value;
        }
        public uint Peso { get => peso; set => peso = value; }

        //metodo desde Res
        public void aplicar_vacuna(Vacuna vacuna)
        {
            try
            {
                {
                    if (vacuna == null)
                        throw new ArgumentNullException(nameof(vacuna));

                    if (l_vacunas_aplicadas.Any(v =>
                        v.Nombre.Equals(vacuna.Nombre) ||
                        v.Lote.Equals(vacuna.Lote)))
                    {
                        throw new InvalidOperationException(
                            $"La vacuna '{vacuna.Nombre}' ya fue aplicada a la res '{Nombre}'.");
                    }

                    if (!vacuna.PuedeAplicarseA(this))
                    {
                        string tipoPlural;
                        byte max;
                        switch (vacuna.Tipo)
                        {
                            case TipoVacuna.Bacteriana:
                                tipoPlural = "bacterianas";
                                max = MaxVacunasBacterianas;
                                break;
                            case TipoVacuna.Viva:
                                tipoPlural = "vivas";
                                max = MaxVacunasVivas;
                                break;
                            default:
                                throw new InvalidOperationException(
                                    $"La res '{Nombre}' no puede recibir más vacunas de este tipo.");
                        }

                        throw new InvalidOperationException(
                            $"No se puede aplicar más vacunas {tipoPlural} a la res '{Nombre}'. Ya tiene las {max} permitidas.");
                    }

                    if (vacuna.EstaVencida())
                        throw new InvalidOperationException(
                            $"[Evento] La vacuna '{vacuna.Nombre}' del lote '{vacuna.Lote}' está vencida desde {vacuna.Fecha_vencimiento.ToShortDateString()}");


                    l_vacunas_aplicadas.Add(vacuna);
                }
            }
            catch (Exception err)
            {
                throw new Exception("Error inesperado en el metodo aplicar_vacuna: " + err.Message);
            }
        }

        public abstract byte MaxVacunasBacterianas { get; }
        public abstract byte MaxVacunasVivas { get; }

        protected virtual void ValidarEdad(ushort edad){   
        }

        public ushort CantidadVacunasBacterianas
        {
            get
            {
                return (ushort)L_vacunas_aplicadas
                    .Count(v => v.Tipo == TipoVacuna.Bacteriana);
            }
        }

        public ushort CantidadVacunasVivas
        {
            get
            {
                return (ushort)L_vacunas_aplicadas
                    .Count(v => v.Tipo == TipoVacuna.Viva);
            }
        }

        public void Alimentar(uint cantidad)
        {
            try {
                Peso += cantidad;
            }
            catch (Exception er)
            {
                throw new Exception("Error inesperado en el metodo alimentar: " + er.Message);
            }
           
        }
    }
}



