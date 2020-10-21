using System;
using System.Collections; 
using System.Text;

namespace ConexaoBD
{
    public class ConstrutorSql
    {

        private string tabela;
        ArrayList campos = new ArrayList();
        ArrayList condicoes = new ArrayList();
        ArrayList ordenacao = new ArrayList(); 


        public ConstrutorSql()
        { }

        public void AdicionarCampo(string campoNome)
        {
            campos.Add(campoNome);   
        }

        public void AdicionarCondicao(string condicao)
        {
            condicoes.Add(condicao);
        }

        public void AdicionarOrdenacao(string campo)
        {
            ordenacao.Add(campo);
        }

        public void AdicionarOrdenacao(string campo, bool ascedente)
        {
            ordenacao.Add(campo + (ascedente?string.Empty:" desc") );
        }

        public string Tabela
        {
            get{return tabela;}
            set { tabela = value; } 
        }

        public string ObterComando()
        {
            if (campos.Count == 0)
                throw new Exception("Campos não selecionados");
            if (this.tabela.Trim().Equals (string.Empty))
                throw new Exception("Tabela não selecionada");     

            StringBuilder s = new StringBuilder();
            s.Append("select ");
            foreach (string campo in campos)
                s.Append(campo + ",");
            s.Remove(s.Length - 1, 1); 

            s.Append(" from " + tabela + (condicoes.Count>0?" Where ":string.Empty ) );   
            foreach (string condicao in condicoes)
                s.Append(condicao + " and ");

            if (condicoes.Count >0)
                s.Remove(s.Length - 4, 4);

            if (ordenacao.Count > 0)
                s.Append(" order by ");

            foreach (string ordem in ordenacao )
                s.Append(ordem+ ",");

            if (ordenacao.Count > 0)
                s.Remove(s.Length - 1, 1);

            return  s.ToString(); 
        }
    }
}

