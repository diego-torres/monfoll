using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;
using ConsolaODBCFox.dto;
using System.Configuration;

namespace ConsolaODBCFox.ado
{
    public class PgClientes
    {
        public static void AgregarCliente(NpgsqlConnection conn, Cliente cliente)
        {
            string sqlString = "INSERT INTO cat_cliente (ap_id, id_empresa, cd_cliente, nombre_cliente, ruta, dia_pago, es_local) " +
                "VALUES(@id, @empresa, @codigo, @nombre_cliente,  @agente,  @dia_pago, @local)";

            NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters.Add("@empresa", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters.Add("@codigo", NpgsqlTypes.NpgsqlDbType.Varchar, 6);
            cmd.Parameters.Add("@nombre_cliente", NpgsqlTypes.NpgsqlDbType.Varchar, 150);
            cmd.Parameters.Add("@agente", NpgsqlTypes.NpgsqlDbType.Varchar, 20);
            cmd.Parameters.Add("@dia_pago", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            cmd.Parameters.Add("@local", NpgsqlTypes.NpgsqlDbType.Boolean);

            cmd.Parameters["@id"].Value = cliente.ApId;
            cmd.Parameters["@empresa"].Value = cliente.IdEmpresa;
            cmd.Parameters["@codigo"].Value = cliente.Codigo;
            cmd.Parameters["@nombre_cliente"].Value = cliente.RazonSocial;
            cmd.Parameters["@agente"].Value = cliente.Ruta;
            cmd.Parameters["@dia_pago"].Value = cliente.DiaPago;
            cmd.Parameters["@local"].Value = cliente.EsLocal;

            cmd.ExecuteNonQuery();
        }

        public static void ActualizarCliente(Cliente cliente)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[Config.Common.MONFOLL].ConnectionString;
            // Para cada ID Doco en la BD de postgres
            using (NpgsqlConnection pgConnection = new NpgsqlConnection(connectionString))
            {
                pgConnection.Open();
                NpgsqlCommand cmd;

                string sqlString = "UPDATE cat_cliente " +
                    "SET cd_cliente = @codigo, " +
                    "nombre_cliente = @nombre_cliente, " +
                    "ruta = @agente, " +
                    "dia_pago = @dia_pago, " +
                    "es_local = @local " +
                    "WHERE id_cliente = @id;";

                cmd = new NpgsqlCommand(sqlString, pgConnection);
                cmd.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer);
                cmd.Parameters.Add("@codigo", NpgsqlTypes.NpgsqlDbType.Varchar, 6);
                cmd.Parameters.Add("@nombre_cliente", NpgsqlTypes.NpgsqlDbType.Varchar, 150);
                cmd.Parameters.Add("@agente", NpgsqlTypes.NpgsqlDbType.Varchar, 20);
                cmd.Parameters.Add("@dia_pago", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
                cmd.Parameters.Add("@local", NpgsqlTypes.NpgsqlDbType.Boolean);

                cmd.Parameters["@id"].Value = cliente.Id;
                cmd.Parameters["@codigo"].Value = cliente.Codigo;
                cmd.Parameters["@nombre_cliente"].Value = cliente.RazonSocial;
                cmd.Parameters["@agente"].Value = cliente.Ruta;
                cmd.Parameters["@dia_pago"].Value = cliente.DiaPago;
                cmd.Parameters["@local"].Value = cliente.EsLocal;

                cmd.ExecuteNonQuery();

                cmd.Dispose();
                pgConnection.Close();
            }
        }

        public static int IdCliente(NpgsqlConnection conn, int apId, int idEmpresa)
        {
            int result = 0;
            NpgsqlDataReader dr;
            NpgsqlCommand cmd;

            string sqlString = "SELECT id_cliente " +
                "FROM cat_cliente " +
                "WHERE ap_id = @idCliente AND id_empresa = @idEmpresa;";

            cmd = new NpgsqlCommand(sqlString, conn);
            cmd.Parameters.Add("@idCliente", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters.Add("@idEmpresa", NpgsqlTypes.NpgsqlDbType.Integer);

            cmd.Parameters["@idCliente"].Value = apId;
            cmd.Parameters["@idEmpresa"].Value = idEmpresa;

            dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                // Cliente listo en base de datos; asignar identificador para relacion
                result = int.Parse(dr["id_cliente"].ToString());
            }

            dr.Close();
            cmd.Dispose();

            return result;
        }

        public static List<Cliente> Clientes(int idEmpresa)
        {
            List<Cliente> result = new List<Cliente>();
            string connectionString = ConfigurationManager.ConnectionStrings[Config.Common.MONFOLL].ConnectionString;
            // Para cada ID Doco en la BD de postgres
            using (NpgsqlConnection pgConnection = new NpgsqlConnection(connectionString))
            {
                pgConnection.Open();
                NpgsqlDataReader dr;
                NpgsqlCommand cmd;

                string sqlString = "SELECT id_cliente, ap_id  " +
                    "FROM cat_cliente " +
                    "WHERE id_empresa = @idEmpresa;";

                cmd = new NpgsqlCommand(sqlString, pgConnection);
                cmd.Parameters.Add("@idEmpresa", NpgsqlTypes.NpgsqlDbType.Integer);
                cmd.Parameters["@idEmpresa"].Value = idEmpresa;

                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Cliente cliente = new Cliente();
                    cliente.Id = int.Parse(dr["id_cliente"].ToString());
                    cliente.ApId = int.Parse(dr["ap_id"].ToString());
                    
                    result.Add(cliente);
                }

                dr.Close();
                cmd.Dispose();
                pgConnection.Close();
            }

            return result;
        }
    }
}
