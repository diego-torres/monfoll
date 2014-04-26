using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Npgsql;

namespace SeguimientoSuper.Collectable.PostgresImpl
{
    public class Customer : CommonBase
    {
        public DataTable ReadCustomers()
        {
            DataSet ds = new DataSet();
            NpgsqlDataAdapter da;
            string sqlString = "SELECT ID_CLIENTE, CD_CLIENTE, NOMBRE_CLIENTE, RUTA, DIA_PAGO " +
                "FROM CAT_CLIENTE;";

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
            string sqlString = "SELECT id_doco, f_documento, f_vencimiento, f_cobro, ctrl_cuenta.id_cliente, cd_cliente, nombre_cliente, ruta, dia_pago, " +
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
            return ds.Tables[0];
        }

        public DataTable ReadPayments(int accountId)
        {
            DataSet ds = new DataSet();
            NpgsqlDataAdapter da;
            string sqlString = "SELECT ID_ABONO, TIPO_PAGO, IMPORTE_PAGO, FOLIO, CONCEPTO, FECHA_DEPOSITO, CUENTA " +
                "FROM CTRL_ABONO " +
                "WHERE ID_DOCO = " + accountId.ToString() + ";";

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
