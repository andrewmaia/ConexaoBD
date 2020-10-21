using System;
using System.Reflection;
using System.Data;
using System.Data.SqlClient;
using System.Collections;  

namespace ConexaoBD
{
    public abstract class Consulta
    {


        #region Selecionar

        public static ArrayList Selecionar(Type tipo,string stringConexao)
        {
            SqlParameter[] parm = null;
            return Selecionar(tipo, tipo.Name, parm, stringConexao);
        }

        public static ArrayList Selecionar(Type tipo, string nomeProcedure, string stringConexao)
        {
            SqlParameter[] parm = null;
            return Selecionar(tipo, nomeProcedure, parm, stringConexao);
        }

        public static ArrayList Selecionar(Type tipo, SqlParameter parm, string stringConexao)
        {
            SqlParameter[] parms = new SqlParameter[1] { parm };
            return Selecionar(tipo, tipo.Name, parms, stringConexao);

        }

        public static ArrayList Selecionar(Type tipo, string nomeProcedure, SqlParameter parm, string stringConexao)
        {
            SqlParameter[] parms = new SqlParameter[1] { parm };
            return Selecionar(tipo, nomeProcedure, parms, stringConexao);
        }

        public static ArrayList Selecionar(Type tipo, SqlParameter[] parm, string stringConexao)
        {
            return Selecionar(tipo, tipo.Name, parm,stringConexao );
        }

        public static ArrayList Selecionar(Type tipo, string nomeProcedure, SqlParameter[] parm, string stringConexao)
        {
            ArrayList objetos = new ArrayList();
            PropertyInfo[] propriedades = tipo.GetProperties();
            object obj;

            SqlDataReader dr = SqlHelper.ExecuteReader(stringConexao , CommandType.StoredProcedure, nomeProcedure, parm);

            try
            {
                while (dr.Read())
                {
                    obj = Activator.CreateInstance(tipo);
                    foreach (PropertyInfo propriedade in propriedades)
                    {

                        object[] Atributos = propriedade.GetCustomAttributes(true);
                        foreach (object atributo in Atributos)
                        {
                            if (atributo.GetType() == typeof(CampoTabela))
                            {
                                if (Convert.IsDBNull(dr[propriedade.Name]))
                                    propriedade.SetValue(obj, null, null);
                                else
                                    propriedade.SetValue(obj, Convert.ChangeType(dr[propriedade.Name].ToString(), System.Nullable.GetUnderlyingType(propriedade.PropertyType) == null ? propriedade.PropertyType : System.Nullable.GetUnderlyingType(propriedade.PropertyType)), null);
                            }
                        }

                    }

                    objetos.Add(obj);
                }
                return objetos;
            }
            finally
            {
                dr.Close();
            }
        }

        public static ArrayList Selecionar(Type tipo,  DataTable  dt)
        {
            ArrayList objetos = new ArrayList();
            PropertyInfo[] propriedades = tipo.GetProperties();
            object obj;

            foreach (DataRow r in dt.Rows)
            {
                obj = Activator.CreateInstance(tipo);
                foreach (PropertyInfo propriedade in propriedades)
                {
                    if (Convert.IsDBNull(r[propriedade.Name]))
                        propriedade.SetValue(obj, null, null);
                    else
                        propriedade.SetValue(obj, Convert.ChangeType(r[propriedade.Name].ToString(), System.Nullable.GetUnderlyingType(propriedade.PropertyType) == null ? propriedade.PropertyType : System.Nullable.GetUnderlyingType(propriedade.PropertyType)), null);
                }

                objetos.Add(obj);
            }
            return objetos;
        }

        #endregion 

        #region ObterDataTable

        public static DataTable ObterDataTable( string stringConexao, CommandType tipoComando, string textoComando)
        {
            SqlParameter[] parametros = null;
            return ObterDataTable( stringConexao, tipoComando, textoComando, parametros);
        }

        public static DataTable ObterDataTable(string stringConexao, CommandType tipoComando, string textoComando, SqlParameter parametro)
        {
            SqlParameter[] parametros = new SqlParameter[1] { parametro };
            return ObterDataTable( stringConexao, tipoComando, textoComando, parametros);
        }

        public static DataTable ObterDataTable(string stringConexao, CommandType tipoComando, string textoComando, params SqlParameter[] parametros)
        {
            SqlConnection conexao = new SqlConnection(stringConexao);
            return ObterDataTable( conexao, tipoComando, textoComando, parametros);
        }

        public static DataTable ObterDataTable( SqlConnection conexao, CommandType tipoComando, string textoComando)
        {
            SqlParameter[] parametros = null;
            return ObterDataTable( conexao, tipoComando, textoComando, parametros);
        }

        public static DataTable ObterDataTable( SqlConnection conexao, CommandType tipoComando, string textoComando, SqlParameter parametro)
        {
            SqlParameter[] parametros = new SqlParameter[1] { parametro };
            return  ObterDataTable( conexao, tipoComando, textoComando, parametros);
        }

        public static DataTable ObterDataTable( SqlConnection conexao, CommandType tipoComando, string textoComando, params SqlParameter[] parametros)
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand();
            SqlHelper.PrepareCommand(cmd, conexao, null, tipoComando, textoComando, parametros);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;
        }

        #endregion 

        #region ObterDataSet

        public static DataSet ObterDataSet( string stringConexao, CommandType tipoComando, string textoComando)
        {
            SqlParameter[] parametros = null;
            return ObterDataSet( stringConexao, tipoComando, textoComando, parametros);
        }

        public static DataSet ObterDataSet( string stringConexao, CommandType tipoComando, string textoComando, SqlParameter parametro)
        {
            SqlParameter[] parametros = new SqlParameter[1] { parametro };
            return ObterDataSet( stringConexao , tipoComando, textoComando, parametros);
        }

        public static DataSet ObterDataSet( string stringConexao, CommandType tipoComando, string textoComando, params SqlParameter[] parametros)
        {
            SqlConnection conexao = new SqlConnection(stringConexao);
            return ObterDataSet( conexao, tipoComando, textoComando, parametros);
        }

        public static DataSet ObterDataSet( SqlConnection conexao, CommandType tipoComando, string textoComando)
        {
            SqlParameter[] parametros = null;
            return ObterDataSet( conexao, tipoComando, textoComando, parametros);
        }

        public static DataSet ObterDataSet( SqlConnection conexao, CommandType tipoComando, string textoComando, SqlParameter parametro)
        {
            SqlParameter[] parametros = new SqlParameter[1] { parametro };
            return ObterDataSet( conexao, tipoComando, textoComando, parametros);
        }

        public static DataSet ObterDataSet( SqlConnection conexao, CommandType tipoComando, string textoComando, params SqlParameter[] parametros)
        {
            DataSet ds = new DataSet(); 
            SqlCommand cmd = new SqlCommand();
            SqlHelper.PrepareCommand(cmd, conexao, null, tipoComando, textoComando, parametros);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            return ds;
        }
        #endregion

        public static DateTime ObterDataServidor(string stringConexao)
        {
          return (DateTime) SqlHelper.ExecuteScalar(stringConexao, CommandType.Text, "Select getdate()", null);  
        
        }

    }
}
