using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class Cliente
    {
        public static ML.Result GetById(ML.Cliente cliente)
        {
            ML.Result result = new ML.Result();
            try
            {
                using (SqlConnection context = new SqlConnection(DL.Conexion.GetConnectionString()))
                {
                    string Query = "ClienteGetById";
                    SqlCommand cmd = DL.Conexion.CreateComand(Query, context);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdCliente", cliente.IdCliente);
                    DataTable tableCliente = DL.Conexion.ExecuteComandSelect(cmd);
                    DataRow row = tableCliente.Rows[0];

                    
                    cliente.Rfc = row[1].ToString();
                    
                    cliente.NumeroEmpleado = int.Parse(row[2].ToString());
                    cliente.Nombre = row[3].ToString();
                    cliente.FechaDeControl = DateTime.Parse(row[4].ToString());
                    cliente.Salario = decimal.Parse(row[5].ToString());

                    result.Object = cliente;

                    if (tableCliente.Rows.Count > 0)
                    {
                        result.Correct = true;
                    }
                    else
                    {
                        result.Correct = false;
                        result.ErrorMessage = "No se encontro ningun registro";
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Correct = false;
                result.ErrorMessage = ex.Message;

            }
            return result;
        }
        public static ML.Result GetAll()
        {
            ML.Result result = new ML.Result();
            try
            {
                using (SqlConnection context = new SqlConnection(DL.Conexion.GetConnectionString()))
                {
                    string Query = "ClienteGetAll";
                    SqlCommand cmd = DL.Conexion.CreateComand(Query, context);

                    cmd.CommandType = CommandType.StoredProcedure;
                    DataTable table = DL.Conexion.ExecuteComandSelect(cmd);

                    if (table.Rows.Count > 0)
                    {
                        result.Objects = new List<object>();
                        foreach (DataRow row in table.Rows)
                        {
                            ML.Cliente cliente = new ML.Cliente();
                            cliente.IdCliente = int.Parse(row[0].ToString());
                            cliente.Rfc = row[1].ToString();
                            cliente.NumeroEmpleado = int.Parse(row[2].ToString());
                            cliente.Nombre = row[3].ToString();
                            
                            cliente.FechaDeControl = DateTime.Parse(row[4].ToString());
                            cliente.Salario = decimal.Parse(row[5].ToString());

                            result.Objects.Add(cliente);
                        }
                        result.Correct = true;
                    }
                    else
                    {
                        result.Correct = false;
                        result.ErrorMessage = "No se encontro ningun registro";
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {

                result.Correct = false;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }
        public static ML.Result Add(ML.Cliente cliente)
        {
            ML.Result result = new ML.Result();
            try
            {
                using (SqlConnection context = new SqlConnection(DL.Conexion.GetConnectionString()))
                {
                    string Query = "ClienteAdd";
                    SqlCommand cmd = DL.Conexion.CreateComand(Query, context);


                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Rfc", cliente.Rfc);
                    cmd.Parameters.AddWithValue("@Nombre", cliente.Nombre);
                    cmd.Parameters.AddWithValue("NumeroEmpleado", cliente.NumeroEmpleado);
                    cmd.Parameters.AddWithValue("@Salario", cliente.Salario);
                    cmd.Parameters.AddWithValue("@FechaControl", cliente.FechaDeControl);

                    int RowsAffected = DL.Conexion.ExecuteComand(cmd);

                    if (RowsAffected >= 1)
                    {
                        result.Correct = true;

                    }
                    else
                    {
                        result.Correct = false;
                        result.ErrorMessage = "No se pudo insertar el registro";

                    }
                    return result;

                }
            }
            catch (Exception ex)
            {

                result.Correct = false;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }
        public static ML.Result Update(ML.Cliente cliente)
        {
            ML.Result result = new ML.Result();
            try
            {
                using (SqlConnection context = new SqlConnection(DL.Conexion.GetConnectionString()))
                {
                    string Query = "ClienteUpdate";
                    SqlCommand cmd = DL.Conexion.CreateComand(Query, context);


                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdCliente", cliente.IdCliente);
                    cmd.Parameters.AddWithValue("@Rfc", cliente.Rfc);
                    cmd.Parameters.AddWithValue("@Nombre", cliente.Nombre);
                    cmd.Parameters.AddWithValue("NumeroEmpleado", cliente.NumeroEmpleado);
                    cmd.Parameters.AddWithValue("@FechaControl", cliente.FechaDeControl);
                    cmd.Parameters.AddWithValue("@Salario", cliente.Salario);

                    int RowsAffected = DL.Conexion.ExecuteComand(cmd);

                    if (RowsAffected > 0)
                    {
                        result.Correct = true;

                    }
                    else
                    {
                        result.Correct = false;
                        result.ErrorMessage = "No se pudo insertar el registro";

                    }
                    return result;
                }

            }
            catch (Exception ex)
            {

                result.Correct = false;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        public static ML.Result Delete(ML.Cliente cliente)
        {
            ML.Result result = new ML.Result();
            try
            {
                using (SqlConnection context = new SqlConnection(DL.Conexion.GetConnectionString()))
                {
                    string Query = "ClienteDelete";
                    SqlCommand cmd = DL.Conexion.CreateComand(Query, context);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdCliente", cliente.IdCliente);

                    int RowsAffected = DL.Conexion.ExecuteComand(cmd);
                    if (RowsAffected >= 0)
                    {
                        result.Correct = true;
                    }
                    else
                    {
                        result.Correct = false;
                        result.ErrorMessage = "No se pudo eliminar el registro";
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Correct = false;
                result.ErrorMessage = ex.Message;

            }
            return result;
        }
        public static ML.Result ValidarDatos(string[] columns, ML.Result result)
        {
            
            string Mensaje = " ";
            int errores=0;
            int ValidacionNumero;
            //Validacion de datos del numero de empleado
            bool success1 = Int32.TryParse(columns[1], out ValidacionNumero);
            if (success1==false)
            {
                result.ErrorMessage = "El dato del numero contiene valores invalidos";
                errores++;
            }
            //Validacion de datos del nombre
            int ValidacionNombre;
            bool success2 = Int32.TryParse(columns[2], out ValidacionNombre);
            if (success2)
            {
                result.ErrorMessage = "El dato del Nombre contiene valores invalidos";
                errores++;
            }
            //Validacion de datos de las horas
            DateTime ValidicacionFecha;
            bool Success3 = DateTime.TryParseExact(columns[3], "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out ValidicacionFecha);
            if (Success3==false)
            {
                result.ErrorMessage = "El dato de la fecha contiene valores invalidos";
                errores++;
            }
            decimal ValidacionSalario;
            bool success4 = Decimal.TryParse(columns[4], out ValidacionSalario);
            if (success4 == false)
            {
                result.ErrorMessage = "El dato del salario contiene valores invalidos";
                errores++;
            }
            if (errores >= 1)
            {
                result.Correct = false;
            }
            else
            {
                result.Correct = true;
            }
            return result;
        }
        
    }
}
