using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Npgsql;
using SeguimientoGerente.Properties;

namespace SeguimientoGerente.Collectable.PostgresImpl
{
    public class Customer : CommonBase
    {

        public List<string> SeriesFromCustomer(string customerCode)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(ConnString))
            {
                conn.Open();
                List<string> result = new List<string>();
                Settings set = Settings.Default;
                NpgsqlDataReader dr;
                NpgsqlCommand cmd;


                string sqlString = "SELECT DISTINCT serie_doco " +
                    "FROM ctrl_cuenta " +
                    "INNER JOIN cat_cliente ON ctrl_cuenta.id_cliente = cat_cliente.id_cliente " +
                    "WHERE cd_cliente=@codigo AND id_empresa=@empresa;";

                cmd = new NpgsqlCommand(sqlString, conn);
                cmd.Parameters.Add("@codigo", NpgsqlTypes.NpgsqlDbType.Varchar, 6);
                cmd.Parameters.Add("@empresa", NpgsqlTypes.NpgsqlDbType.Integer);

                cmd.Parameters["@codigo"].Value = customerCode;
                cmd.Parameters["@empresa"].Value = set.empresa;

                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    result.Add(dr["serie_doco"].ToString());
                }

                conn.Close();

                return result;
            }
        }

        public string CustomerNameByCode(string code)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(ConnString))
            {
                conn.Open();
                Settings set = Settings.Default;
                NpgsqlDataReader dr;
                NpgsqlCommand cmd;
                string result = null;

                string sqlString = "SELECT nombre_cliente " +
                    "FROM cat_cliente " +
                    "WHERE cd_cliente=@codigo AND id_empresa=@empresa;";

                cmd = new NpgsqlCommand(sqlString, conn);
                cmd.Parameters.Add("@codigo", NpgsqlTypes.NpgsqlDbType.Varchar, 6);
                cmd.Parameters.Add("@empresa", NpgsqlTypes.NpgsqlDbType.Integer);

                cmd.Parameters["@codigo"].Value = code;
                cmd.Parameters["@empresa"].Value = set.empresa;

                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = dr["nombre_cliente"].ToString();
                }

                conn.Close();

                return result;
            }
        }

        public DataTable ReadCustomers()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(ConnString))
            {
                conn.Open();
                DataSet ds = new DataSet();
                NpgsqlDataAdapter da;
                string sqlString = "SELECT ID_CLIENTE, CD_CLIENTE, NOMBRE_CLIENTE, RUTA, DIA_PAGO, " +
                    "CASE WHEN cat_cliente.es_local THEN 'Local' ELSE 'Foráneo' END AS area " +
                    "FROM CAT_CLIENTE;";

                da = new NpgsqlDataAdapter(sqlString, conn);

                ds.Reset();
                da.Fill(ds);
                conn.Close();
                return ds.Tables[0];
            }
        }

    }
}
