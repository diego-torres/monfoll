using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsolaODBCFox.dto;
using System.Configuration;
using Npgsql;

namespace ConsolaODBCFox.ado
{
    public class PgGruposVencimiento
    {
        public static void GrabarVencimientos(List<FactVencimiento> vencimientos, GrupoVencimiento grupo)
        {
            throw new NotImplementedException("Not implemented yet.");
            string connectionString = ConfigurationManager.ConnectionStrings[Config.Common.JASPER].ConnectionString;
            // Para cada ID Doco en la BD de postgres
            using (NpgsqlConnection pgConnection = new NpgsqlConnection(connectionString))
            {
                pgConnection.Open();
                foreach (FactVencimiento vencimiento in vencimientos)
                {
                    // Check if record exists.
                    // if record exists, update the record.
                    // if record does not exists, insert the record
                }
                pgConnection.Close();
            }
        }

        private static int IdCliente(NpgsqlConnection conn, string codigoCliente, int idEmpresa)
        {
            int result = 0;
            NpgsqlDataReader dr;
            NpgsqlCommand cmd;

            string sqlString = "SELECT id_cliente " +
                "FROM dim_clientes " +
                "WHERE codigo_cliente = @cdCliente AND id_empresa = @idEmpresa;";

            cmd = new NpgsqlCommand(sqlString, conn);
            cmd.Parameters.Add("@cdCliente", NpgsqlTypes.NpgsqlDbType.Varchar, 11);
            cmd.Parameters.Add("@idEmpresa", NpgsqlTypes.NpgsqlDbType.Integer);

            cmd.Parameters["@idCliente"].Value = codigoCliente;
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


        private static void ActualizarVencimiento(FactVencimiento vencimiento, GrupoVencimiento grupo, NpgsqlConnection conn)
        {
            string sqlString = "SELECT id_grupo_vencimiento, minimo_dias, maximo_dias " +
                    "FROM dim_grupo_vencimiento;";
            NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);
            cmd.ExecuteNonQuery();
            
        }

        public static List<GrupoVencimiento> GruposVencimiento()
        {
            List<GrupoVencimiento> result = new List<GrupoVencimiento>();
            string connectionString = ConfigurationManager.ConnectionStrings[Config.Common.JASPER].ConnectionString;
            // Para cada ID Doco en la BD de postgres
            using (NpgsqlConnection pgConnection = new NpgsqlConnection(connectionString))
            {
                pgConnection.Open();
                NpgsqlDataReader dr;
                NpgsqlCommand cmd;

                string sqlString = "SELECT id_grupo_vencimiento, minimo_dias, maximo_dias " +
                    "FROM dim_grupo_vencimiento;";

                cmd = new NpgsqlCommand(sqlString, pgConnection);
                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    GrupoVencimiento grupo = new GrupoVencimiento();
                    grupo.Id = int.Parse(dr["id_grupo_vencimiento"].ToString());
                    grupo.Desde = int.Parse(dr["minimo_dias"].ToString());
                    grupo.Hasta = int.Parse(dr["maximo_dias"].ToString());
                    result.Add(grupo);
                }

                dr.Close();
                cmd.Dispose();
                pgConnection.Close();
            }
            return result;
        }
    }
}
