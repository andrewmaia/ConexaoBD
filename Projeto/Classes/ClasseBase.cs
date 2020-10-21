using System;
using System.Collections.Generic;
using System.Reflection;
using System.Data.SqlClient;
using System.Data;

namespace ConexaoBD
{
    public class ClasseBase
    {
        private Type tipo;
        private SqlParameter[] parametros;
        private int quantidadePropriedades;
        private PropertyInfo[] propriedades;
        private PropertyInfo identificador;

        public ClasseBase(int id, string stringConexao)
        {
            Iniciar(); 
            CarregarObjeto(id, stringConexao);
        }

        public ClasseBase()
        {
            Iniciar();
        }

        protected void Iniciar()
        {
            tipo = this.GetType();
            propriedades = tipo.GetProperties();
            quantidadePropriedades = QuantidadePropriedades();
            parametros = new SqlParameter[quantidadePropriedades];
            parametros = ObterParametros();
            identificador = ObterIdentificador();
        }

        private int QuantidadePropriedades()
        {
            int q = 0;
            foreach (PropertyInfo propriedade in propriedades)
            {
                object[] Atributos = propriedade.GetCustomAttributes(true);
                foreach (object atributo in Atributos)
                {
                    if (atributo.GetType() == typeof(CampoTabela))
                        q++;
                }
            }
            return q;
        }

        public void Salvar(string stringConexao)
        {
            string nomeProcedure;
            
            SqlCommand comando = new SqlCommand();
            nomeProcedure = tipo.Name + "Salvar";
            int i = 0;

            foreach (PropertyInfo propriedade in propriedades)
            {
                object[] Atributos = propriedade.GetCustomAttributes(true);
                foreach (object atributo in Atributos)
                {
                    if (atributo.GetType() == typeof(CampoTabela))
                    {
                        parametros[i++].SqlValue = propriedade.GetValue(this, null);
                    }
                }
            }
            using (SqlConnection conexao = new SqlConnection(stringConexao))
            {
                conexao.Open();
                SqlHelper.ExecuteNonQuery(conexao, CommandType.StoredProcedure, nomeProcedure, parametros);
                conexao.Close();
            }
            i = 0;
            foreach (PropertyInfo propriedade in propriedades)
            {
                object[] Atributos = propriedade.GetCustomAttributes(true);
                foreach (object atributo in Atributos)
                {
                    
                    if (atributo.GetType() == typeof(CampoTabela))
                    {
                        if (((CampoTabela)atributo).Retorna)
                            propriedade.SetValue(this, Convert.ChangeType(parametros[i++].SqlValue.ToString(),     (Type.GetTypeCode(propriedade.PropertyType))), null);
                    }
                }
            }
        }

        public void Excluir(string stringConexao)
        {
            SqlParameter [] p = new SqlParameter [1];
            p[0]=new SqlParameter ("@" + identificador.Name , TipoSql(identificador ));
            p[0].Direction = ParameterDirection.Input;
            p[0].SqlValue = identificador.GetValue(this, null);
            string nomeProcedure = tipo.Name + "Excluir";
            SqlHelper.ExecuteNonQuery(stringConexao, CommandType.StoredProcedure, nomeProcedure,p);
            identificador.SetValue(this, 0, null);
        }

        private PropertyInfo ObterIdentificador()
        {


            foreach (PropertyInfo propriedade in propriedades)
            {
                object[] Atributos = propriedade.GetCustomAttributes(true);
                foreach (object atributo in Atributos)
                {
                    if (atributo.GetType() == typeof(CampoTabela))
                    {
                        if (((CampoTabela)atributo).Identificador)
                            return propriedade; 
                    }
                }
            }
            return null;
        }

        public void CarregarObjeto(int id,string stringConexao)
        {
            string nomeProcedure = tipo.Name  + "Selecionar";
            SqlParameter[] parms = new SqlParameter[1] { new SqlParameter("@" +identificador.Name, id) };
            SqlDataReader dr = SqlHelper.ExecuteReader(stringConexao, CommandType.StoredProcedure, nomeProcedure, parms);
            try
            {
                dr.Read();
                if (!dr.HasRows)
                    throw new Exception("SqlDataReader vazio!");
                else
                {
                    foreach (PropertyInfo propriedade in propriedades)
                    {
                        if (Convert.IsDBNull(dr[propriedade.Name]))
                            propriedade.SetValue(this, null, null);
                        else
                            propriedade.SetValue(this, Convert.ChangeType(dr[propriedade.Name].ToString(), System.Nullable.GetUnderlyingType(propriedade.PropertyType) == null ? propriedade.PropertyType : System.Nullable.GetUnderlyingType(propriedade.PropertyType)), null);
                    }
                }
            }
            finally
            {
                dr.Close();
            }
        }

        private SqlParameter[] ObterParametros()
        {
            SqlParameter[] pParametros = SqlHelper.GetCachedParameters(tipo.Name);

            if (pParametros == null)
            {
                pParametros = new SqlParameter[quantidadePropriedades];
                int i = 0;
                foreach (PropertyInfo propriedade in propriedades)
                {
                    object[] Atributos = propriedade.GetCustomAttributes(true);
                    foreach (object atributo in Atributos)
                    {
                        if (atributo.GetType() == typeof(CampoTabela))
                        {
                            pParametros[i] = new SqlParameter();
                            pParametros[i].ParameterName = "@" + propriedade.Name;
                            if (((CampoTabela)atributo).Retorna)
                                pParametros[i].Direction = ParameterDirection.InputOutput;
                            pParametros[i++].SqlDbType = TipoSql(propriedade);
                        }
                    }
                }
                SqlHelper.CacheParameters(tipo.Name, pParametros);
            }
            return pParametros;
        }

        private SqlDbType TipoSql(PropertyInfo propriedade)
        {
            switch (Type.GetTypeCode(System.Nullable.GetUnderlyingType(propriedade.PropertyType) == null ? propriedade.PropertyType : System.Nullable.GetUnderlyingType(propriedade.PropertyType)))
            {
                case TypeCode.Int32:
                    return SqlDbType.Int;
                case TypeCode.Int16:
                    return SqlDbType.Int;
                case TypeCode.Int64:
                    return SqlDbType.Int;
                case TypeCode.DateTime:
                    return SqlDbType.DateTime;
                case TypeCode.Boolean:
                    return SqlDbType.Bit;
                case TypeCode.Double :
                    return SqlDbType.Float;
                case TypeCode.Single :
                    return SqlDbType.Decimal ;
                case TypeCode.Decimal :
                    return SqlDbType.Money;
                default:
                    return SqlDbType.VarChar;
            }
        }

    }

}
