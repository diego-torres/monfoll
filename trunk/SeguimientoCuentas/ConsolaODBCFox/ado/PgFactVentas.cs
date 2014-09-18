using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Npgsql;
using ConsolaODBCFox.dto;

namespace ConsolaODBCFox.ado
{
    public class PgFactVentas
    {
        public static int IdVendedor(int apId, int empresaId)
        {
            int id = 0;
            string connectionString = ConfigurationManager.ConnectionStrings[Config.Common.JASPER].ConnectionString;
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                NpgsqlDataReader dr;
                NpgsqlCommand cmd;
                conn.Open();

                string sqlString = "SELECT seller_id " +
                    "FROM dim_sellers " +
                    "WHERE ap_id = @apId AND id_empresa = @eId;";

                cmd = new NpgsqlCommand(sqlString, conn);
                cmd.Parameters.Add("@apId", NpgsqlTypes.NpgsqlDbType.Integer);
                cmd.Parameters.Add("@eId", NpgsqlTypes.NpgsqlDbType.Integer);

                cmd.Parameters["@apId"].Value = apId;
                cmd.Parameters["@eId"].Value = empresaId;

                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    id = int.Parse(dr[0].ToString());
                }

                dr.Close();
                conn.Close();
            }
            return id;
        }

        public static void AgregarVendedor(Vendedor vendedor)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[Config.Common.JASPER].ConnectionString;
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                NpgsqlCommand cmd;
                conn.Open();

                string sqlString = "INSERT INTO dim_sellers(ap_id, agent_code, agent_name, empresa, id_empresa, is_local) " +
                    "VALUES( @apId, @codigo, @nombre, @empresa, @idEmpresa, False );";

                cmd = new NpgsqlCommand(sqlString, conn);
                cmd.Parameters.Add("@apId", NpgsqlTypes.NpgsqlDbType.Integer);
                cmd.Parameters.Add("@codigo", NpgsqlTypes.NpgsqlDbType.Varchar, 10);
                cmd.Parameters.Add("@nombre", NpgsqlTypes.NpgsqlDbType.Varchar, 150);
                cmd.Parameters.Add("@empresa", NpgsqlTypes.NpgsqlDbType.Varchar, 150);
                cmd.Parameters.Add("@idEmpresa", NpgsqlTypes.NpgsqlDbType.Integer);

                cmd.Parameters["@apId"].Value = vendedor.ApId;
                cmd.Parameters["@codigo"].Value = vendedor.Codigo;
                cmd.Parameters["@nombre"].Value = vendedor.Nombre;
                cmd.Parameters["@empresa"].Value = vendedor.Empresa;
                cmd.Parameters["@idEmpresa"].Value = vendedor.EmpresaId;

                cmd.ExecuteNonQuery();

                conn.Close();
            }
        }

        public static void ActualizarVentas(Venta venta)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[Config.Common.JASPER].ConnectionString;
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                NpgsqlCommand cmd;
                conn.Open();

                string sqlString = "UPDATE fact_sales " +
                    "SET sold_today = @hoy, " +
                    "sold_week = @semana, " +
                    "sold_month = @mes " +
                    "WHERE seller_id = @vendedor";

                cmd = new NpgsqlCommand(sqlString, conn);
                cmd.Parameters.Add("@hoy", NpgsqlTypes.NpgsqlDbType.Numeric);
                cmd.Parameters.Add("@semana", NpgsqlTypes.NpgsqlDbType.Numeric);
                cmd.Parameters.Add("@mes", NpgsqlTypes.NpgsqlDbType.Numeric);
                cmd.Parameters.Add("@vendedor", NpgsqlTypes.NpgsqlDbType.Integer);

                cmd.Parameters["@hoy"].Value = venta.Diaria;
                cmd.Parameters["@semana"].Value = venta.Semanal;
                cmd.Parameters["@mes"].Value = venta.Mensual;
                cmd.Parameters["@vendedor"].Value = venta.Vendedor.Id;

                cmd.ExecuteNonQuery();

                conn.Close();
            }
        }

        public static void AgregarVentas(Venta venta)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[Config.Common.JASPER].ConnectionString;
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                NpgsqlCommand cmd;
                conn.Open();

                string sqlString = "INSERT INTO fact_sales(seller_id, sold_today, sold_week, sold_month) " +
                    "VALUES( @vendedor, @hoy, @semana, @mes );";

                cmd = new NpgsqlCommand(sqlString, conn);
                cmd.Parameters.Add("@hoy", NpgsqlTypes.NpgsqlDbType.Numeric);
                cmd.Parameters.Add("@semana", NpgsqlTypes.NpgsqlDbType.Numeric);
                cmd.Parameters.Add("@mes", NpgsqlTypes.NpgsqlDbType.Numeric);
                cmd.Parameters.Add("@vendedor", NpgsqlTypes.NpgsqlDbType.Integer);

                cmd.Parameters["@hoy"].Value = venta.Diaria;
                cmd.Parameters["@semana"].Value = venta.Semanal;
                cmd.Parameters["@mes"].Value = venta.Mensual;
                cmd.Parameters["@vendedor"].Value = venta.Vendedor.Id;

                cmd.ExecuteNonQuery();

                conn.Close();
            }
        }

    }
}
