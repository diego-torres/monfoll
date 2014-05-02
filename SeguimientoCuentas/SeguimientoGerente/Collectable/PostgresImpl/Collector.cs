using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Npgsql;

namespace SeguimientoGerente.Collectable.PostgresImpl
{
    public class Collector : CommonBase
    {
        public void AddCollector(string name, bool isLocal)
        {
            string sqlString = "INSERT INTO CAT_COBRADOR(nombre_cobrador, es_local) " +
                "VALUES(@nombre_cobrador, @es_local)";

            connect();
            NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);
            cmd.Parameters.Add("@nombre_cobrador", NpgsqlTypes.NpgsqlDbType.Varchar, 150);
            cmd.Parameters.Add("@es_local", NpgsqlTypes.NpgsqlDbType.Boolean);

            cmd.Parameters["@nombre_cobrador"].Value = name;
            cmd.Parameters["@es_local"].Value = isLocal;

            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public void UpdateCollector(int idCollector, string name, bool isLocal)
        {
            string sqlString = "UPDATE CAT_COBRADOR " +
                "SET nombre_cobrador = @nombre_cobrador, " +
                "es_local = @es_local " +
                "WHERE id_cobrador = @id_cobrador";

            connect();
            NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);
            cmd.Parameters.Add("@nombre_cobrador", NpgsqlTypes.NpgsqlDbType.Varchar, 150);
            cmd.Parameters.Add("@es_local", NpgsqlTypes.NpgsqlDbType.Boolean);
            cmd.Parameters.Add("@id_cobrador", NpgsqlTypes.NpgsqlDbType.Integer);

            cmd.Parameters["@nombre_cobrador"].Value = name;
            cmd.Parameters["@es_local"].Value = isLocal;
            cmd.Parameters["@id_cobrador"].Value = idCollector;

            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public void RemoveCollector(int collectorId)
        {
            string sqlString = "DELETE FROM CAT_COBRADOR " +
                "WHERE id_cobrador = @id_cobrador";

            connect();
            NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);
            cmd.Parameters.Add("@id_cobrador", NpgsqlTypes.NpgsqlDbType.Integer);

            cmd.Parameters["@id_cobrador"].Value = collectorId;

            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public DataTable ReadAssignments(int collectorId)
        {
            DataSet ds = new DataSet();
            NpgsqlDataAdapter da;
            string sqlString = "SELECT ctrl_cuenta.id_doco, f_documento, f_vencimiento, f_cobro, ctrl_cuenta.id_cliente, cd_cliente, nombre_cliente, ruta, dia_pago, " +
                "serie_doco, folio_doco, tipo_documento, tipo_cobro, facturado, saldo, moneda, observaciones, CURRENT_DATE - f_vencimiento AS dias_vencido " +
                "FROM ctrl_cuenta INNER JOIN cat_cliente ON ctrl_cuenta.id_cliente = cat_cliente.id_cliente " +
                "INNER JOIN ctrl_asignacion ON ctrl_asignacion.id_doco = ctrl_cuenta.id_doco " +
                "WHERE ctrl_asignacion.id_cobrador = " + collectorId.ToString() + " " +
                "AND ctrl_cuenta.id_doco NOT IN(SELECT id_doco FROM ctrl_seguimiento WHERE id_movimiento IN(9,10));";

            if (conn == null || conn.State != ConnectionState.Open)
                connect();

            da = new NpgsqlDataAdapter(sqlString, conn);

            ds.Reset();
            da.Fill(ds);
            conn.Close();
            return ds.Tables[0];
        }

        public DataTable ReadCollectors()
        {
            DataSet ds = new DataSet();
            NpgsqlDataAdapter da;
            string sqlString = "SELECT ID_COBRADOR, NOMBRE_COBRADOR, CASE WHEN ES_LOCAL THEN 'Local' ELSE 'Foráneo' END AS area " +
                "FROM CAT_COBRADOR;";

            if (conn == null || conn.State != ConnectionState.Open)
                connect();

            da = new NpgsqlDataAdapter(sqlString, conn);

            ds.Reset();
            da.Fill(ds);
            conn.Close();
            return ds.Tables[0];
        }

        public void RemoveLogCobrador(int logId)
        {
            string sqlString = "DELETE FROM LOG_COBRADOR " +
                "WHERE id_log_cobrador = @id_nota;";

            connect();
            NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);
            cmd.Parameters.Add("@id_nota", NpgsqlTypes.NpgsqlDbType.Integer);

            cmd.Parameters["@id_nota"].Value = logId;

            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public void UpdateLogCobrador(int logId, string log)
        {
            string sqlString = "UPDATE LOG_COBRADOR " +
                "SET nota = @nota " +
                "WHERE id_log_cobrador = @id_nota";

            connect();
            NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);
            cmd.Parameters.Add("@nota", NpgsqlTypes.NpgsqlDbType.Varchar, 250);
            cmd.Parameters.Add("@id_nota", NpgsqlTypes.NpgsqlDbType.Integer);

            cmd.Parameters["@nota"].Value = log;
            cmd.Parameters["@id_nota"].Value = logId;

            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public void AddLogCobrador(int idCobrador, string log)
        {
            string sqlString = "INSERT INTO LOG_COBRADOR(id_cobrador, nota) " +
                "VALUES(@id_cobrador, @nota);";

            connect();
            NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);
            cmd.Parameters.Add("@id_cobrador", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters.Add("@nota", NpgsqlTypes.NpgsqlDbType.Varchar, 250);

            cmd.Parameters["@id_cobrador"].Value = idCobrador;
            cmd.Parameters["@nota"].Value = log;

            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public DataTable ReadLogCobrador(string idCobrador)
        {
            DataSet ds = new DataSet();
            NpgsqlDataAdapter da;
            string sqlString = "SELECT ID_LOG_COBRADOR, NOTA " +
                "FROM LOG_COBRADOR WHERE ID_COBRADOR = " + idCobrador + ";";

            if (conn == null || conn.State != ConnectionState.Open)
                connect();

            da = new NpgsqlDataAdapter(sqlString, conn);

            ds.Reset();
            da.Fill(ds);
            conn.Close();
            return ds.Tables[0];
        }
    }
}
