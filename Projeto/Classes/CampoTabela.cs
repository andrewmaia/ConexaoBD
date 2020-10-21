using System;

namespace ConexaoBD
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class CampoTabela : Attribute
    {
        private bool retorna;
        private bool identificador;

        public CampoTabela() { }

        public bool Retorna
        {
            get { return this.retorna; }
            set { this.retorna = value; }
        }

        public bool Identificador
        {
            get { return this.identificador; }
            set { this.identificador = value; }
        }
    }
}
