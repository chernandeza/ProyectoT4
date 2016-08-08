using System;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;

namespace LibreriaT4
{
    public class ConectorBD
    {
        #region Class Attributes
        private SqlConnectionStringBuilder ConnectionStr;
        private SqlConnection DBConnection; //Conexion a BD
        private SqlCommand SQLcmd; //Comando SQL a ejecutar en BD
        private EvtLogWriter elw;
        #endregion

        /// <summary>
        /// Base class constructor. Initializes the connection to the DB with the specific parameters.            
        /// </summary>
        public ConectorBD()
        {
            try
            {
                ConnectionStr = new SqlConnectionStringBuilder();
                ConnectionStr.DataSource = ".\\SQLEXPRESS";
                ConnectionStr.InitialCatalog = "T3SJTest1";
                ConnectionStr.IntegratedSecurity = true;
                elw = new EvtLogWriter("InfoPersonas", "Application");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void GuardarListaenBD(ListaPersonas lp)
        {
            DBConnection = new SqlConnection(ConnectionStr.ConnectionString);
            DBConnection.Open();
            try
            {   
                foreach (Persona item in lp.ListaP.Values)
                {
                    String commando = "INSERT INTO Persona (cedula, nombre, apellido1, apellido2, provincia, genero, autorizada) ";
                    commando += "VALUES ('";
                    commando += item.Cedula;
                    commando += "', '";
                    commando += item.Nombre;
                    commando += "', '";
                    commando += item.Apellido1;
                    commando += "', '";
                    commando += item.Apellido2;
                    commando += "', ";
                    commando += (int)item.Provincia1;
                    commando += ", '";
                    commando += item.Genero.ToString();
                    commando += "', 0";
                    commando += ");";
                    SQLcmd = new SqlCommand(commando, DBConnection);
                    elw.writeWarning(commando);
                    SQLcmd.ExecuteNonQuery();
                }
            }
            catch (Exception E)
            {
                elw.writeError("Error en Guardar Lista en BD " + E.Message);
                throw;
            }
            finally
            {
                DBConnection.Close();
            }
        }

        public Persona CargarInformacionPersona(String cedula)
        {
            //DBConnection = new SqlConnection(ConnectionStr.ConnectionString);
            //DBConnection.Open();

            try
            {
                Persona P = new Persona();
                String comando = "SELECT nombre, apellido1, apellido2, provincia, genero";
                comando += " FROM persona WHERE cedula='";
                comando += cedula;
                comando += "';";
                //elw.writeWarning(comando);
                SqlDataReader lectorBD;
                using (DBConnection = new SqlConnection(ConnectionStr.ConnectionString))
                {
                    using (SQLcmd = new SqlCommand(comando, DBConnection))
                    {
                        DBConnection.Open();
                        lectorBD = SQLcmd.ExecuteReader();
                        if (lectorBD.HasRows)
                        {
                            lectorBD.Read();
                            P.Cedula = cedula;
                            P.Nombre = lectorBD.GetString(0);
                            P.Apellido1 = lectorBD.GetString(1);
                            P.Apellido2 = lectorBD.GetString(2);
                            //elw.writeWarning(lectorBD.GetInt16(3).ToString());
                            P.Provincia1 = (Provincia)Enum.ToObject(typeof(Provincia), lectorBD.GetInt16(3));
                            P.Genero = (Genero)Enum.Parse(typeof(Genero), lectorBD.GetString(4));
                        }
                        else
                        {
                            return new Persona();
                        }
                        return P;
                    }
                }
                /*El lectorBD lee fila por fila. Para cada fila trae los datos indicados en el SELECT.
                  El select de este metodo retorna:
                    - String nombre,
                    - String apellido1, 
                    - String apellido2,
                    - int provincia
                    - String Genero
                    Entonces para el data reader esto viene en un arreglo asi:
                    |--------|-----------|-----------|-----------|--------|
                    | nombre | apellido1 | apellido2 | provincia | genero | 
                    |--------|-----------|-----------|-----------|--------|
                        0           1           2           3          4
                */
            }
            catch (Exception E)
            {
                elw.writeError("Error en Cargar Informacion Persona " + E.Message);
                throw;
            }
            finally
            {
                DBConnection.Close();
            }
        }

        public List<Comentario> CargarComentariosPersona(String cedP)
        {
            DBConnection = new SqlConnection(ConnectionStr.ConnectionString);
            DBConnection.Open();

            try
            {
                SqlDataReader lectorBD;
                SqlCommand SQLcmdSP;

                SQLcmdSP = new SqlCommand();
                SQLcmdSP.CommandText = "sp_retornarComentariosPersona";
                SQLcmdSP.CommandType = CommandType.StoredProcedure;
                SQLcmdSP.Parameters.Add(new SqlParameter("@cedPersona", SqlDbType.NVarChar, 50));
                SQLcmdSP.Parameters[0].Value = cedP;
                SQLcmdSP.Connection = DBConnection;
                lectorBD = SQLcmdSP.ExecuteReader();
                List<Comentario> listaCom = new List<Comentario>();
                //elw.writeWarning("Cedula = " + cedP);
                if (lectorBD.HasRows)
                {
                    //elw.writeWarning("LectorBD Has Rows");
                    while (lectorBD.Read())
                    {
                        //elw.writeWarning("LectorBD Read");
                        Comentario C = new Comentario();
                        C.ID = lectorBD.GetInt32(0);
                        C.Texto = lectorBD.GetString(1);
                        listaCom.Add(C);
                    }
                    return listaCom;
                }
                else
                {
                    listaCom = null;
                    return listaCom;
                }
            }
            catch (Exception E)
            {
                elw.writeError("Error en Cargar Comentarios Persona " + E.Message);
                throw;
            }
            finally
            {
                DBConnection.Close();
            }        
        }

        public List<String> CargarCedulasPersonas()
        {
            DBConnection = new SqlConnection(ConnectionStr.ConnectionString);
            DBConnection.Open();

            try
            {
                SqlDataReader lectorBD;
                SqlCommand SQLcmdSP;

                SQLcmdSP = new SqlCommand();
                SQLcmdSP.CommandText = "sp_ObtenerCedulas";
                SQLcmdSP.CommandType = CommandType.StoredProcedure;
                SQLcmdSP.Connection = DBConnection;
                lectorBD = SQLcmdSP.ExecuteReader();
                List<String> listaCed = new List<String>();

                if (lectorBD.HasRows)
                {
                    while (lectorBD.Read())
                    {
                        String Ced = lectorBD.GetString(0);
                        listaCed.Add(Ced);
                    }
                    return listaCed;
                }
                else
                {
                    listaCed = null;
                    return listaCed;
                }
            }
            catch (Exception E)
            {
                elw.writeError("Error cargando cedulas: " + E.Message);
                throw;
            }
            finally
            {
                DBConnection.Close();
            }
        }

        public bool GuardarComentario(Comentario Com)
        {
            DBConnection = new SqlConnection(ConnectionStr.ConnectionString);
            DBConnection.Open();
            try
            {
                SqlCommand SQLcmdSP;
                SQLcmdSP = new SqlCommand();
                SQLcmdSP.CommandText = "sp_GuardarComentario";
                SQLcmdSP.CommandType = CommandType.StoredProcedure;
                SQLcmdSP.Parameters.Add(new SqlParameter("@cedula", SqlDbType.NVarChar, 50));
                SQLcmdSP.Parameters.Add(new SqlParameter("@texto", SqlDbType.NVarChar, -1));
                SQLcmdSP.Parameters[0].Value = Com.Cedula;
                SQLcmdSP.Parameters[1].Value = Com.Texto;
                SQLcmdSP.Connection = DBConnection;
                SQLcmdSP.ExecuteNonQuery();
                return true;
            }
            catch (Exception E)
            {
                elw.writeError("Error almacenando comentario: " + E.Message);
                return false;
            }
            finally
            {
                DBConnection.Close();
            }
        }
    }
}
