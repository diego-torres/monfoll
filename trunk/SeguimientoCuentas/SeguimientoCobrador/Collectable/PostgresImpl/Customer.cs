using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Npgsql;
using SeguimientoCobrador.Properties;

namespace SeguimientoCobrador.Collectable.PostgresImpl
{
    public class Customer : CommonBase
    {
        public List<string> SeriesFromCustomer(string customerCode)
        {
            List<string> result = new List<string>();
            Settings set = Settings.Default;
            NpgsqlDataReader dr;
            NpgsqlCommand cmd;
            
            
            string sqlString = "SELECT DISTINCT serie_doco " +
                "FROM ctrl_cuenta " +
                "INNER JOIN cat_cliente ON ctrl_cuenta.id_cliente = cat_cliente.id_cliente " + 
                "WHERE cd_cliente=@codigo AND id_empresa=@empresa;";

            connect();
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

        public string CustomerNameByCode(string code)
        {
            Settings set = Settings.Default;
            NpgsqlDataReader dr;
            NpgsqlCommand cmd;
            string result = null;

            string sqlString = "SELECT nombre_cliente " +
                "FROM cat_cliente "+
                "WHERE cd_cliente=@codigo AND id_empresa=@empresa;";

            connect();
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

        public DataTable ReadCustomers()
        {
            DataSet ds = new DataSet();
            NpgsqlDataAdapter da;
            string sqlString = "SELECT ID_CLIENTE, CD_CLIENTE, NOMBRE_CLIENTE, RUTA, DIA_PAGO, " +
                "CASE WHEN cat_cliente.es_local THEN 'Local' ELSE 'Foráneo' END AS area " +
                "FROM CAT_CLIENTE;";

            if (conn == null || conn.State != ConnectionState.Open)
                connect();

            da = new NpgsqlDataAdapter(sqlString, conn);

            ds.Reset();
            da.Fill(ds);
            conn.Close();
            return ds.Tables[0];
        }

        public DataTable ReadNotes(int idCliente)
        {
            DataSet ds = new DataSet();
            NpgsqlDataAdapter da;
            string sqlString = "SELECT id_log_cliente, nota " +
                "FROM log_cliente " +
                "WHERE id_cliente = " + idCliente.ToString() + ";";

            if (conn == null || conn.State != ConnectionState.Open)
                connect();

            da = new NpgsqlDataAdapter(sqlString, conn);

            ds.Reset();
            da.Fill(ds);
            conn.Close();
            return ds.Tables[0];
        }

        public DataTable ReadAccounts(int customerId)
        {
            DataSet ds = new DataSet();
            NpgsqlDataAdapter da;
            string sqlString = "SELECT id_doco, ctrl_cuenta.ap_id, f_documento, f_vencimiento, f_cobro, ctrl_cuenta.id_cliente, cd_cliente, nombre_cliente, ruta, dia_pago, " +
                "serie_doco, folio_doco, tipo_documento, tipo_cobro, facturado, saldo, moneda, observaciones " +
                "FROM ctrl_cuenta INNER JOIN cat_cliente ON ctrl_cuenta.id_cliente = cat_cliente.id_cliente " +
                "WHERE ctrl_cuenta.id_cliente = " + customerId.ToString() + " " +
                "AND id_doco NOT IN(SELECT id_doco FROM ctrl_seguimiento WHERE id_movimiento IN(9,10));";

            if (conn == null || conn.State != ConnectionState.Open)
                connect();

            da = new NpgsqlDataAdapter(sqlString, conn);

            ds.Reset();
            da.Fill(ds);
            conn.Close();

            ds.Tables[0].Columns.Add("dias_vencido", typeof(int));
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                DateTime now = DateTime.Now;
                DateTime dueDate = DateTime.Parse(row["f_vencimiento"].ToString());
                TimeSpan elapsed = now.Subtract(dueDate);

                row["dias_vencido"] = int.Parse(elapsed.TotalDays.ToString("0"));
            }

            return ds.Tables[0];
        }

        public void AddNote(int customerId, string note)
        {
            string sqlString = "INSERT INTO log_cliente(id_cliente, nota) " +
                "VALUES(@id_cliente, @nota);";

            connect();
            NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);
            cmd.Parameters.Add("@id_cliente", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters.Add("@nota", NpgsqlTypes.NpgsqlDbType.Varchar, 250);

            cmd.Parameters["@id_cliente"].Value = customerId;
            cmd.Parameters["@nota"].Value = note;

            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public void UpdateNote(int logId, string log)
        {
            string sqlString = "UPDATE log_cliente " +
                "SET nota = @nota " +
                "WHERE id_log_cliente = @id_nota";

            connect();
            NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);
            cmd.Parameters.Add("@nota", NpgsqlTypes.NpgsqlDbType.Varchar, 250);
            cmd.Parameters.Add("@id_nota", NpgsqlTypes.NpgsqlDbType.Integer);

            cmd.Parameters["@nota"].Value = log;
            cmd.Parameters["@id_nota"].Value = logId;

            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public void RemoveNote(int logId)
        {
            string sqlString = "DELETE FROM log_cliente " +
                "WHERE id_log_cliente = @id_nota;";

            connect();
            NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);
            cmd.Parameters.Add("@id_nota", NpgsqlTypes.NpgsqlDbType.Integer);

            cmd.Parameters["@id_nota"].Value = logId;

            cmd.ExecuteNonQuery();
            conn.Close();
        }

    }
}
