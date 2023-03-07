using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;
using System.Globalization;
using System.Windows;
using System.Diagnostics;

namespace Proyecto_Entrevistas.App_Code
{
    internal class Procedimientos_Sql
    {
        string sqlConexion = ConfigurationManager.ConnectionStrings["SQLWindowsAuthConnectionString"].ConnectionString;

        internal string ConsultarTabla(string nombreTabla)
        {
            string tablaJson = string.Empty;
            try
            {
                DataSet dataset = new DataSet();
                using (SqlConnection conexion = new SqlConnection(sqlConexion))
                {
                    SqlCommand comando = new SqlCommand("SP_CONSULTAR_TABLA", conexion);
                    comando.Parameters.AddWithValue("@nombre_tabla", nombreTabla);
                    comando.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = comando;

                    adapter.Fill(dataset);
                    if(dataset != null && dataset.Tables.Count>0)
                    {
                        tablaJson = JsonConvert.SerializeObject(dataset.Tables[0], Formatting.Indented);
                    }
                    adapter = null;
                    comando = null;
                }
            }
            catch(Exception x)
            {
                throw x;
            }
            return tablaJson;
        }

        internal bool AgregarRegistroSQL(object objetoClase)
        {
            bool operacionExitosa = false;
            try
            {
                if(!(ExisteRegistro(objetoClase)))
                {
                    string sqlQuery = "insert into " + objetoClase.GetType().Name + " values (";
                    string valores = string.Empty;
                    foreach (var propiedad in objetoClase.GetType().GetProperties())
                    {
                        if (valores.Length > 0) { valores += ","; }
                        if (propiedad.PropertyType.Name == "Int32" || propiedad.PropertyType.Name == "Double")
                        {

                            valores += Convert.ToString(propiedad.GetValue(objetoClase));
                        }
                        else
                        {
                            if (propiedad.PropertyType.Name == "Boolean")
                            {
                                valores += Convert.ToString(Convert.ToInt32(propiedad.GetValue(objetoClase)));
                            }
                            else
                            {
                                if (propiedad.PropertyType.Name == "DateTime")
                                {
                                    valores += "'" + Convert.ToString(Convert.ToDateTime(propiedad.GetValue(objetoClase)).Date.ToString("yyyy-MM-dd")) + "'";
                                }
                                else
                                {
                                    valores += "'" + Convert.ToString(propiedad.GetValue(objetoClase)) + "'";
                                }
                            }
                        }
                    }
                    sqlQuery += valores + ")";

                    using (SqlConnection conexion = new SqlConnection(sqlConexion))
                    {
                        using (SqlCommand comando = new SqlCommand(sqlQuery, conexion))
                        {
                            conexion.Open();
                            if (comando.ExecuteNonQuery() > 0) { operacionExitosa = true; }
                            conexion.Close();
                        }
                    }
                }
                else
                {
                    //preguntar si se desea actualizar
                    if((MessageBox.Show("Este registro ya esta almacenado en la base de datos, desea actualizarlo con los valores actuales?", "Actualizar", MessageBoxButton.YesNo, MessageBoxImage.Question)) == MessageBoxResult.Yes)
                    {
                        operacionExitosa = ActualizarRegistro(objetoClase);
                    }
                    else
                    {
                        throw new Exception("Error. Ya existe un registro previo para " + objetoClase.GetType().Name + ". Si desea volver a agregar este registro, por favor primero elimine el registro anterior.");
                    }                    
                }
            }
            catch(Exception x)
            {
                throw x;
            }
            return operacionExitosa;
        }

        internal bool ExisteRegistro(object objetoClase)
        {
            bool registroPrevio = false;
            try
            {
                DataSet dataset = new DataSet();
                using (SqlConnection conexion = new SqlConnection(sqlConexion))
                {
                    string nombreTabla = objetoClase.GetType().Name;
                    SqlCommand comando=null;

                    if(nombreTabla=="Entrevista")
                    {
                        if(ExisteRegistro(new Vacante() { Id= Convert.ToInt32(objetoClase.GetType().GetProperty("Vacante").GetValue(objetoClase)) }))
                        {
                            if (ExisteRegistro(new Prospecto() { Id = Convert.ToInt32(objetoClase.GetType().GetProperty("Prospecto").GetValue(objetoClase)) }))
                            {
                                comando = new SqlCommand("SP_EXISTE_ENTREVISTA", conexion);
                                comando.Parameters.AddWithValue("@vacante", objetoClase.GetType().GetProperty("Vacante").GetValue(objetoClase));
                                comando.Parameters.AddWithValue("@prospecto", objetoClase.GetType().GetProperty("Prospecto").GetValue(objetoClase));
                            }
                            else
                            {
                                throw new Exception("El prospecto que se ingreso, no existe en los registros. Por favor, agregue primero el registro.");
                            }
                        }
                        else
                        {
                            throw new Exception("La vacante que se ingreso, no existe en los registros. Por favor, agregue primero la vacante deseada.");
                        }
                    }
                    if (nombreTabla == "Prospecto")
                    {
                        comando = new SqlCommand("SP_EXISTE_PROSPECTO", conexion);
                        comando.Parameters.AddWithValue("@id", objetoClase.GetType().GetProperty("Id").GetValue(objetoClase));                        
                    }
                    if (nombreTabla == "Vacante")
                    {
                        comando = new SqlCommand("SP_EXISTE_VACANTE", conexion);
                        comando.Parameters.AddWithValue("@id", objetoClase.GetType().GetProperty("Id").GetValue(objetoClase));
                    }
                    comando.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = comando;
                    conexion.Open();
                    adapter.Fill(dataset);
                    if (dataset != null && dataset.Tables.Count > 0)
                    {
                        registroPrevio = Convert.ToBoolean(dataset.Tables[0].Rows[0][0]);
                    }
                    adapter = null;
                    comando = null;
                    conexion.Close();
                }
                dataset = null;
            }
            catch (Exception x)
            {
                throw x;
            }
            return registroPrevio;
        }

        internal bool ActualizarRegistro(object objetoClase)
        {
            bool operacionExitosa = false;
            try
            {                
                using (SqlConnection conexion = new SqlConnection(sqlConexion))
                {
                    string nombreTabla = objetoClase.GetType().Name;
                    SqlCommand comando = null;
                    if (nombreTabla == "Entrevista")
                    {
                        comando = new SqlCommand("SP_ACTUALIZAR_ENTREVISTA", conexion);
                    }
                    if (nombreTabla == "Prospecto")
                    {
                        comando = new SqlCommand("SP_ACTUALIZAR_PROSPECTO", conexion);
                    }
                    if (nombreTabla == "Vacante")
                    {
                        comando = new SqlCommand("SP_ACTUALIZAR_VACANTE", conexion);
                    }
                    foreach(var propiedad in objetoClase.GetType().GetProperties())
                    {
                        comando.Parameters.AddWithValue("@" + propiedad.Name, propiedad.GetValue(objetoClase));
                    }
                    comando.CommandType = CommandType.StoredProcedure;                                        
                    conexion.Open();
                    if (comando.ExecuteNonQuery() > 0) { operacionExitosa = true; }
                    comando = null;
                    conexion.Close();
                }                
            }
            catch (Exception x)
            {
                throw x;
            }
            return operacionExitosa;
        }

        internal bool EliminarRegistro(object objetoClase)
        {
            bool operacionExitosa = false;
            try
            {                
                using (SqlConnection conexion = new SqlConnection(sqlConexion))
                {
                    string nombreTabla = objetoClase.GetType().Name;
                    SqlCommand comando = null;

                    if (nombreTabla == "Entrevista")
                    {
                        comando = new SqlCommand("SP_ELIMINAR_ENTREVISTA", conexion);
                        comando.Parameters.AddWithValue("@vacante", objetoClase.GetType().GetProperty("Vacante").GetValue(objetoClase));
                        comando.Parameters.AddWithValue("@prospecto", objetoClase.GetType().GetProperty("Prospecto").GetValue(objetoClase));
                    }
                    if (nombreTabla == "Prospecto")
                    {
                        comando = new SqlCommand("SP_ELIMINAR_PROSPECTO", conexion);
                        comando.Parameters.AddWithValue("@id", objetoClase.GetType().GetProperty("Id").GetValue(objetoClase));
                    }
                    if (nombreTabla == "Vacante")
                    {
                        comando = new SqlCommand("SP_ELIMINAR_VACANTE", conexion);
                        comando.Parameters.AddWithValue("@id", objetoClase.GetType().GetProperty("Id").GetValue(objetoClase));
                    }
                    comando.CommandType = CommandType.StoredProcedure;                                        
                    conexion.Open();                    
                    if (comando.ExecuteNonQuery() > 0) { operacionExitosa = true; }                    
                    comando = null;
                    conexion.Close();
                }                
            }
            catch (Exception x)
            {
                throw x;
            }
            return operacionExitosa;
        }

        internal int ConsultarSiguienteID(string nombreTabla)
        {
            int nuevoId = 0;
            try
            {
                DataSet dataset = new DataSet();
                using (SqlConnection conexion = new SqlConnection(sqlConexion))
                {
                    SqlCommand comando = new SqlCommand("SP_CONSULTAR_ULTIMOID", conexion);
                    comando.Parameters.AddWithValue("@nombre_tabla", nombreTabla);
                    comando.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = comando;

                    adapter.Fill(dataset);
                    if (dataset != null && dataset.Tables.Count > 0)
                    {
                        nuevoId = Convert.ToInt32(dataset.Tables[0].Rows[0]["proximo_id"]);
                    }
                    adapter = null;
                    comando = null;
                }
            }
            catch (Exception x)
            {
                throw x;
            }
            return nuevoId;
        }
    }
}
