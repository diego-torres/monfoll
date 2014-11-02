using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Npgsql;
using SeguimientoSuper.Properties;
using System.Globalization;

namespace SeguimientoSuper.Collectable.PostgresImpl
{
    public class Account : CommonBase
    {
        public DataTable Uncollectable()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(ConnString))
            {
                DataSet ds = new DataSet();
                NpgsqlDataAdapter da;
                string sqlString = "SELECT f_documento, f_vencimiento, 0 AS dias_vencido, f_cobro, " +
                    "cat_cliente.ruta, serie_doco, folio_doco, " +
                    "cd_cliente, nombre_cliente, " +
                    "tipo_cobro, facturado, saldo, moneda, observaciones, " +
                    "tipo_documento, dia_pago, " +
                    "ctrl_cuenta.lista_negra, ctrl_cuenta.id_doco, ctrl_cuenta.ap_id,  ctrl_cuenta.id_cliente, " +
                    "CASE WHEN cat_cliente.es_local THEN 'Local' ELSE 'Foráneo' END AS area, " +
                    "f_cobro_esperada, cat_empresa.ruta as ruta_e, cat_cliente.id_empresa " +
                    "FROM ctrl_cuenta " +
                    "INNER JOIN cat_cliente ON ctrl_cuenta.id_cliente = cat_cliente.id_cliente " +
                    "INNER JOIN cat_empresa ON cat_cliente.id_empresa = cat_empresa.id_empresa " +
                    "WHERE ctrl_cuenta.id_doco IN(SELECT id_doco FROM ctrl_seguimiento WHERE id_movimiento=7) " +
                    "AND ctrl_cuenta.enterprise_id = " + ConfiguredCompany() + " " +
                    "ORDER BY f_vencimiento desc, folio_doco;";

                conn.Open();

                da = new NpgsqlDataAdapter(sqlString, conn);

                ds.Reset();
                da.Fill(ds);
                conn.Close();

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    DateTime now = DateTime.Now;
                    DateTime dueDate = DateTime.Parse(row["f_vencimiento"].ToString());
                    TimeSpan elapsed = now.Subtract(dueDate);

                    row["dias_vencido"] = int.Parse(elapsed.TotalDays.ToString("0"));
                }

                return ds.Tables[0];
            }
        }

        public DataTable Escalated()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(ConnString))
            {
                DataSet ds = new DataSet();
                NpgsqlDataAdapter da;
                string sqlString = "SELECT f_documento, f_vencimiento, 0 AS dias_vencido, f_cobro, " +
                    "cat_cliente.ruta, serie_doco, folio_doco, " +
                    "cd_cliente, nombre_cliente, " +
                    "tipo_cobro, facturado, saldo, moneda, observaciones, " +
                    "tipo_documento, dia_pago, " +
                    "ctrl_cuenta.lista_negra, ctrl_cuenta.id_doco, ctrl_cuenta.ap_id,  ctrl_cuenta.id_cliente, " +
                    "CASE WHEN cat_cliente.es_local THEN 'Local' ELSE 'Foráneo' END AS area, " +
                    "f_cobro_esperada, cat_empresa.ruta as ruta_e, cat_cliente.id_empresa " +
                    "FROM ctrl_cuenta " +
                    "INNER JOIN cat_cliente ON ctrl_cuenta.id_cliente = cat_cliente.id_cliente " +
                    "INNER JOIN cat_empresa ON cat_cliente.id_empresa = cat_empresa.id_empresa " +
                    "WHERE ctrl_cuenta.id_doco IN(SELECT id_doco FROM ctrl_seguimiento WHERE id_movimiento=6) " +
                    "AND ctrl_cuenta.id_doco NOT IN (SELECT id_doco FROM ctrl_seguimiento WHERE id_movimiento=7) " +
                    "AND ctrl_cuenta.enterprise_id = " + ConfiguredCompany() + " " +
                    "ORDER BY f_vencimiento desc, folio_doco;";

                conn.Open();

                da = new NpgsqlDataAdapter(sqlString, conn);

                ds.Reset();
                da.Fill(ds);
                conn.Close();

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    DateTime now = DateTime.Now;
                    DateTime dueDate = DateTime.Parse(row["f_vencimiento"].ToString());
                    TimeSpan elapsed = now.Subtract(dueDate);

                    row["dias_vencido"] = int.Parse(elapsed.TotalDays.ToString("0"));
                }

                return ds.Tables[0];
            }

        }

        public DataTable Attended()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(ConnString))
            {
                DataSet ds = new DataSet();
                NpgsqlDataAdapter da;
                string sqlString = "SELECT f_documento, f_vencimiento, 0 AS dias_vencido, f_cobro, " +
                    "cat_cliente.ruta, serie_doco, folio_doco, " +
                    "cd_cliente, nombre_cliente, " +
                    "tipo_cobro, facturado, saldo, moneda, observaciones, " +
                    "tipo_documento, dia_pago, " +
                    "ctrl_cuenta.lista_negra, ctrl_cuenta.id_doco, ctrl_cuenta.ap_id,  ctrl_cuenta.id_cliente, " +
                    "CASE WHEN cat_cliente.es_local THEN 'Local' ELSE 'Foráneo' END AS area, " +
                    "f_cobro_esperada, cat_empresa.ruta as ruta_e, cat_cliente.id_empresa " +
                    "FROM ctrl_cuenta " +
                    "INNER JOIN cat_cliente ON ctrl_cuenta.id_cliente = cat_cliente.id_cliente " +
                    "INNER JOIN cat_empresa ON cat_cliente.id_empresa = cat_empresa.id_empresa " +
                    "WHERE ctrl_cuenta.id_doco IN(SELECT id_doco FROM ctrl_seguimiento WHERE id_movimiento=4) " +
                    "AND ctrl_cuenta.id_doco NOT IN (SELECT id_doco FROM ctrl_seguimiento WHERE id_movimiento IN(6,7)) " +
                    "AND ctrl_cuenta.enterprise_id = " + ConfiguredCompany() + " " +
                    "ORDER BY f_vencimiento desc, folio_doco;";

                conn.Open();

                da = new NpgsqlDataAdapter(sqlString, conn);

                ds.Reset();
                da.Fill(ds);
                conn.Close();

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    DateTime now = DateTime.Now;
                    DateTime dueDate = DateTime.Parse(row["f_vencimiento"].ToString());
                    TimeSpan elapsed = now.Subtract(dueDate);

                    row["dias_vencido"] = int.Parse(elapsed.TotalDays.ToString("0"));
                }

                return ds.Tables[0];
            }
        }

        public DataTable BlackListed()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(ConnString))
            {

                DataSet ds = new DataSet();
                NpgsqlDataAdapter da;
                string sqlString = "SELECT f_documento, f_vencimiento, 0 AS dias_vencido, f_cobro, " +
                    "cat_cliente.ruta, serie_doco, folio_doco, " +
                    "cd_cliente, nombre_cliente, " +
                    "tipo_cobro, facturado, saldo, moneda, observaciones, " +
                    "tipo_documento, dia_pago, " +
                    "ctrl_cuenta.lista_negra, ctrl_cuenta.id_doco, ctrl_cuenta.ap_id,  ctrl_cuenta.id_cliente, " +
                    "CASE WHEN cat_cliente.es_local THEN 'Local' ELSE 'Foráneo' END AS area, " +
                    "f_cobro_esperada, cat_empresa.ruta as ruta_e, cat_cliente.id_empresa " +
                    "FROM ctrl_cuenta " +
                    "INNER JOIN cat_cliente ON ctrl_cuenta.id_cliente = cat_cliente.id_cliente " +
                    "INNER JOIN cat_empresa ON cat_cliente.id_empresa = cat_empresa.id_empresa " +
                    "WHERE ctrl_cuenta.lista_negra = true AND ctrl_cuenta.id_doco NOT IN(SELECT id_doco FROM ctrl_seguimiento WHERE id_movimiento IN(4,6,7)) " +
                    "AND ctrl_cuenta.enterprise_id = " + ConfiguredCompany() + " " +
                    "ORDER BY f_vencimiento desc, folio_doco;";

                conn.Open();

                da = new NpgsqlDataAdapter(sqlString, conn);

                ds.Reset();
                da.Fill(ds);
                conn.Close();

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    DateTime now = DateTime.Now;
                    DateTime dueDate = DateTime.Parse(row["f_vencimiento"].ToString());
                    TimeSpan elapsed = now.Subtract(dueDate);

                    row["dias_vencido"] = int.Parse(elapsed.TotalDays.ToString("0"));
                }

                return ds.Tables[0];
            }
        }

        public DataTable MasterTable()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(ConnString))
            {
                DataSet ds = new DataSet();
                NpgsqlDataAdapter da;
                string sqlString = "SELECT f_documento, f_vencimiento, 0 AS dias_vencido, f_cobro, " +
                    "cat_cliente.ruta, serie_doco, folio_doco, " +
                    "cd_cliente, nombre_cliente, " +
                    "tipo_cobro, facturado, saldo, moneda, observaciones, " +
                    "tipo_documento, dia_pago, " +
                    "ctrl_cuenta.lista_negra, ctrl_cuenta.id_doco, ctrl_cuenta.ap_id,  ctrl_cuenta.id_cliente, " +
                    "CASE WHEN cat_cliente.es_local THEN 'Local' ELSE 'Foráneo' END AS area, " +
                    "f_cobro_esperada, cat_empresa.ruta as ruta_e, cat_cliente.id_empresa " +
                    "FROM ctrl_cuenta " +
                    "INNER JOIN cat_cliente ON ctrl_cuenta.id_cliente = cat_cliente.id_cliente " +
                    "INNER JOIN cat_empresa ON cat_cliente.id_empresa = cat_empresa.id_empresa " +
                    "WHERE ctrl_cuenta.lista_negra = false AND ctrl_cuenta.id_doco NOT IN(SELECT id_doco FROM ctrl_seguimiento WHERE id_movimiento IN(4,6,7)) " +
                    "AND ctrl_cuenta.enterprise_id = " + ConfiguredCompany() + " " +
                    "ORDER BY f_vencimiento desc, folio_doco;";

                conn.Open();

                da = new NpgsqlDataAdapter(sqlString, conn);

                ds.Reset();
                da.Fill(ds);
                conn.Close();

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    DateTime now = DateTime.Now;
                    DateTime dueDate = DateTime.Parse(row["f_vencimiento"].ToString());
                    TimeSpan elapsed = now.Subtract(dueDate);

                    row["dias_vencido"] = int.Parse(elapsed.TotalDays.ToString("0"));
                }

                return ds.Tables[0];
            }
        }

        public void SetObservations(int docId, string collectType, string observations)
        {

            using (NpgsqlConnection conn = new NpgsqlConnection(ConnString))
            {
                conn.Open();
                string sqlString = "UPDATE ctrl_cuenta " +
                "SET tipo_cobro = @collect_type, " +
                "observaciones = @observations " +
                "WHERE ID_DOCO = @id";

                NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);

                cmd.Parameters.Add("@collect_type", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
                cmd.Parameters.Add("@observations", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
                cmd.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer);

                cmd.Parameters["@collect_type"].Value = collectType;
                cmd.Parameters["@observations"].Value = observations;
                cmd.Parameters["@id"].Value = docId;

                cmd.ExecuteNonQuery();

                sqlString = "INSERT INTO ctrl_seguimiento(id_movimiento, id_doco, descripcion) " +
                    "VALUES(2, @doc, @detail);";

                cmd = new NpgsqlCommand(sqlString, conn);
                cmd.Parameters.Add("@doc", NpgsqlTypes.NpgsqlDbType.Integer);
                cmd.Parameters.Add("@detail", NpgsqlTypes.NpgsqlDbType.Varchar, 250);

                cmd.Parameters["@doc"].Value = docId;
                cmd.Parameters["@detail"].Value = string.Format("Observaciones Actualizadas {0}; Tipo de Cobro: {1}", observations, collectType);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    throw new Exception(UNEXISTING_DOCO_ERR);
                }

                conn.Close();
            }
        }

        public void SetCollectDate(int docId, DateTime collectDate)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(ConnString))
            {
                conn.Open();
                string sqlString = "UPDATE ctrl_cuenta " +
                    "SET F_COBRO = @f_cobro " +
                    "WHERE ID_DOCO = @id";

                NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);

                cmd.Parameters.Add("@f_cobro", NpgsqlTypes.NpgsqlDbType.Date);
                cmd.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer);

                cmd.Parameters["@f_cobro"].Value = collectDate;
                cmd.Parameters["@id"].Value = docId;

                //18991230
                if (collectDate.Equals(new DateTime(1899, 12, 30)))
                {
                    sqlString = "UPDATE ctrl_cuenta " +
                        "SET f_cobro = null " +
                        "WHERE ID_DOCO = @id";

                    cmd = new NpgsqlCommand(sqlString, conn);
                    cmd.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer);
                    cmd.Parameters["@id"].Value = docId;
                }


                cmd.ExecuteNonQuery();

                sqlString = "INSERT INTO ctrl_seguimiento(id_movimiento, id_doco, descripcion) " +
                    "VALUES(2, @doc, @detail);";

                cmd = new NpgsqlCommand(sqlString, conn);
                cmd.Parameters.Add("@doc", NpgsqlTypes.NpgsqlDbType.Integer);
                cmd.Parameters.Add("@detail", NpgsqlTypes.NpgsqlDbType.Varchar, 250);

                cmd.Parameters["@doc"].Value = docId;
                cmd.Parameters["@detail"].Value = string.Format("Fecha de cobro actualizada al {0}", collectDate.ToString("dd-MMM-yyyy"));

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    throw new Exception(UNEXISTING_DOCO_ERR);
                }

                conn.Close();
            }
        }

        public void Review(int docId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(ConnString))
            {
                conn.Open();
                string sqlString = "UPDATE ctrl_seguimiento SET id_movimiento = 13 " +
                    "WHERE id_doco = @docId AND id_movimiento = 4;";

                NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);

                cmd.Parameters.Add("@docId", NpgsqlTypes.NpgsqlDbType.Integer);
                cmd.Parameters["@docId"].Value = docId;

                cmd.ExecuteNonQuery();

                sqlString = "INSERT INTO ctrl_seguimiento(id_movimiento, id_doco, descripcion) " +
                    "VALUES(5, @documento, 'Cuenta revisada por supervisor.');";

                cmd = new NpgsqlCommand(sqlString, conn);

                cmd.Parameters.Add("@documento", NpgsqlTypes.NpgsqlDbType.Integer);
                cmd.Parameters["@documento"].Value = docId;

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    conn.Close();
                    throw new Exception(UNEXISTING_DOCO_ERR);
                }

                conn.Close();
            }
        }

        public void Especialize(int docId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(ConnString))
            {
                conn.Open();
                string sqlString = "UPDATE ctrl_cuenta SET lista_negra = true " +
                    "WHERE id_doco = @documento;";

                NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);

                cmd.Parameters.Add("@documento", NpgsqlTypes.NpgsqlDbType.Integer);
                cmd.Parameters["@documento"].Value = docId;

                cmd.ExecuteNonQuery();

                sqlString = "INSERT INTO ctrl_seguimiento(id_movimiento, id_doco, descripcion) " +
                    "VALUES(3, @documento, 'Cuenta marcada en la lista de cuentas especiales.');";
                cmd = new NpgsqlCommand(sqlString, conn);

                cmd.Parameters.Add("@documento", NpgsqlTypes.NpgsqlDbType.Integer);
                cmd.Parameters["@documento"].Value = docId;

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    conn.Close();
                    throw new Exception(UNEXISTING_DOCO_ERR);
                }

                conn.Close();
            }
        }

        public void UnEspecialize(int docId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(ConnString))
            {
                conn.Open();

                string sqlString = "UPDATE ctrl_cuenta SET lista_negra = false " +
                    "WHERE id_doco = @docId;";

                NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);

                cmd.Parameters.Add("@docId", NpgsqlTypes.NpgsqlDbType.Integer);
                cmd.Parameters["@docId"].Value = docId;

                cmd.ExecuteNonQuery();

                sqlString = "INSERT INTO ctrl_seguimiento(id_movimiento, id_doco, descripcion) " +
                    "VALUES(8, @documento, 'Cuenta recuperada de la lista de cuentas especiales.');";
                cmd = new NpgsqlCommand(sqlString, conn);

                cmd.Parameters.Add("@documento", NpgsqlTypes.NpgsqlDbType.Integer);
                cmd.Parameters["@documento"].Value = docId;

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    conn.Close();
                    throw new Exception(UNEXISTING_DOCO_ERR);
                }

                conn.Close();
            }
        }

        public void Escale(int docId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(ConnString))
            {
                conn.Open();

                string sqlString = "INSERT INTO ctrl_seguimiento(id_movimiento, id_doco, descripcion) " +
                    "VALUES(6, @documento, 'Cuenta escalada a gerencia.');";

                NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);

                cmd.Parameters.Add("@documento", NpgsqlTypes.NpgsqlDbType.Integer);
                cmd.Parameters["@documento"].Value = docId;

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    conn.Close();
                    throw new Exception(UNEXISTING_DOCO_ERR);
                }
                conn.Close();
            }
        }

        public void Unescale(int docId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(ConnString))
            {
                conn.Open();

                string sqlString = "DELETE FROM ctrl_seguimiento " +
                    "WHERE id_doco = @docId AND id_movimiento = 6;";

                NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);

                cmd.Parameters.Add("@docId", NpgsqlTypes.NpgsqlDbType.Integer);
                cmd.Parameters["@docId"].Value = docId;

                cmd.ExecuteNonQuery();

                sqlString = "INSERT INTO ctrl_seguimiento(id_movimiento, id_doco, descripcion) " +
                    "VALUES(8, @documento, 'Cuenta recuperada de la lista de escalación.');";
                cmd = new NpgsqlCommand(sqlString, conn);

                cmd.Parameters.Add("@documento", NpgsqlTypes.NpgsqlDbType.Integer);
                cmd.Parameters["@documento"].Value = docId;

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    conn.Close();
                    throw new Exception(UNEXISTING_DOCO_ERR);
                }

                conn.Close();
            }
        }

        public void Uncollectable(int docId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(ConnString))
            {
                conn.Open();

                string sqlString = "INSERT INTO ctrl_seguimiento(id_movimiento, id_doco, descripcion) " +
                        "VALUES(7, @documento, 'Documento marcado incobrable por gerencia.');";

                NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);

                cmd.Parameters.Add("@documento", NpgsqlTypes.NpgsqlDbType.Integer);
                cmd.Parameters["@documento"].Value = docId;

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    conn.Close();
                    throw new Exception(UNEXISTING_DOCO_ERR);
                }
                conn.Close();
            }
        }

        public void Collectable(int docId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(ConnString))
            {
                conn.Open();

                string sqlString = "DELETE FROM ctrl_seguimiento " +
                    "WHERE id_doco = @docId AND id_movimiento = 7;";

                NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);

                cmd.Parameters.Add("@docId", NpgsqlTypes.NpgsqlDbType.Integer);
                cmd.Parameters["@docId"].Value = docId;

                cmd.ExecuteNonQuery();

                sqlString = "INSERT INTO ctrl_seguimiento(id_movimiento, id_doco, descripcion) " +
                    "VALUES(8, @documento, 'Cuenta Recuperada de incobrables.');";
                cmd = new NpgsqlCommand(sqlString, conn);

                cmd.Parameters.Add("@documento", NpgsqlTypes.NpgsqlDbType.Integer);
                cmd.Parameters["@documento"].Value = docId;

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    conn.Close();
                    throw new Exception(UNEXISTING_DOCO_ERR);
                }

                conn.Close();
            }
        }

        public DataTable ReadSeries()
        {
            DataSet ds = new DataSet();
            NpgsqlDataAdapter da;
            string sqlString = "SELECT DISTINCT serie_doco " +
                "FROM ctrl_cuenta;";

            using (NpgsqlConnection conn = new NpgsqlConnection(ConnString))
            {
                conn.Open();

                da = new NpgsqlDataAdapter(sqlString, conn);

                ds.Reset();
                da.Fill(ds);
                conn.Close();

                return ds.Tables[0];
            }
        }

        public DataTable ReadFolios()
        {
            DataSet ds = new DataSet();
            NpgsqlDataAdapter da;
            string sqlString = "SELECT folio_doco " +
                "FROM ctrl_cuenta;";

            using (NpgsqlConnection conn = new NpgsqlConnection(ConnString))
            {
                conn.Open();

                da = new NpgsqlDataAdapter(sqlString, conn);

                ds.Reset();
                da.Fill(ds);
                conn.Close();

                return ds.Tables[0];
            }
        }

        public DataTable FollowUp(int docId)
        {
            DataSet ds = new DataSet();
            NpgsqlDataAdapter da;

            string sqlString = "SELECT id_seguimiento, ctrl_seguimiento.id_movimiento, cat_movimiento.descripcion AS movimiento, " +
                "system_based, ctrl_seguimiento.descripcion as seguimiento, ts_seguimiento " +
                "FROM ctrl_seguimiento INNER JOIN cat_movimiento ON ctrl_seguimiento.id_movimiento = cat_movimiento.id_movimiento " +
                "WHERE id_doco = " + docId.ToString() + ";";

            using (NpgsqlConnection conn = new NpgsqlConnection(ConnString))
            {
                conn.Open();

                da = new NpgsqlDataAdapter(sqlString, conn);

                ds.Reset();
                da.Fill(ds);
                conn.Close();

                if (ds.Tables.Count == 0) return null;

                return ds.Tables[0];
            }
        }

        public DataTable ReadPayments(int accountId)
        {
            DataSet ds = new DataSet();
            NpgsqlDataAdapter da;
            string sqlString = "SELECT ID_ABONO, TIPO_PAGO, IMPORTE_PAGO, FOLIO, CONCEPTO, FECHA_DEPOSITO, CUENTA " +
                "FROM CTRL_ABONO " +
                "WHERE ID_DOCO = " + accountId.ToString() + ";";

            using (NpgsqlConnection conn = new NpgsqlConnection(ConnString))
            {
                conn.Open();

                da = new NpgsqlDataAdapter(sqlString, conn);

                ds.Reset();
                da.Fill(ds);
                conn.Close();
                return ds.Tables[0];
            }
        }

        public void UpdateAccountById(Collectable.Account adminPaqAccount)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(ConnString))
            {
                conn.Open();

                string sqlString = "UPDATE ctrl_cuenta " +
                    "SET F_DOCUMENTO = @f_documento, " +
                    "F_VENCIMIENTO = @f_vencimiento, " +
                    "F_COBRO = @f_cobro, " +
                    "ID_CLIENTE = @id_cliente, " +
                    "SERIE_DOCO = @serie_doco, " +
                    "FOLIO_DOCO = @folio_doco, " +
                    "TIPO_DOCUMENTO = @tipo_documento, " +
                    "TIPO_COBRO = @tipo_cobro, " +
                    "FACTURADO = @facturado, " +
                    "SALDO = @saldo, " +
                    "MONEDA = @moneda, " +
                    "OBSERVACIONES = @observaciones, " +
                    "TS_DESCARGADO = CURRENT_TIMESTAMP " +
                    "WHERE ID_DOCO = @id";

                NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);

                cmd.Parameters.Add("@f_documento", NpgsqlTypes.NpgsqlDbType.Date);
                cmd.Parameters.Add("@f_vencimiento", NpgsqlTypes.NpgsqlDbType.Date);
                cmd.Parameters.Add("@f_cobro", NpgsqlTypes.NpgsqlDbType.Date);
                cmd.Parameters.Add("@id_cliente", NpgsqlTypes.NpgsqlDbType.Integer);
                cmd.Parameters.Add("@serie_doco", NpgsqlTypes.NpgsqlDbType.Varchar, 4);
                cmd.Parameters.Add("@folio_doco", NpgsqlTypes.NpgsqlDbType.Integer);
                cmd.Parameters.Add("@tipo_documento", NpgsqlTypes.NpgsqlDbType.Varchar, 150);
                cmd.Parameters.Add("@tipo_cobro", NpgsqlTypes.NpgsqlDbType.Varchar, 100);
                cmd.Parameters.Add("@facturado", NpgsqlTypes.NpgsqlDbType.Money);
                cmd.Parameters.Add("@saldo", NpgsqlTypes.NpgsqlDbType.Money);
                cmd.Parameters.Add("@moneda", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
                cmd.Parameters.Add("@observaciones", NpgsqlTypes.NpgsqlDbType.Varchar, 250);
                cmd.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer);

                cmd.Parameters["@f_documento"].Value = adminPaqAccount.DocDate;
                cmd.Parameters["@f_vencimiento"].Value = adminPaqAccount.DueDate;
                cmd.Parameters["@f_cobro"].Value = adminPaqAccount.CollectDate;
                cmd.Parameters["@id_cliente"].Value = adminPaqAccount.Company.Id;
                cmd.Parameters["@serie_doco"].Value = adminPaqAccount.Serie;
                cmd.Parameters["@folio_doco"].Value = adminPaqAccount.Folio;
                cmd.Parameters["@tipo_documento"].Value = adminPaqAccount.DocType;
                cmd.Parameters["@tipo_cobro"].Value = adminPaqAccount.CollectType;
                cmd.Parameters["@facturado"].Value = adminPaqAccount.Amount;
                cmd.Parameters["@saldo"].Value = adminPaqAccount.Balance;
                cmd.Parameters["@moneda"].Value = adminPaqAccount.Currency;
                cmd.Parameters["@observaciones"].Value = adminPaqAccount.Note;
                cmd.Parameters["@id"].Value = adminPaqAccount.DocId;

                cmd.ExecuteNonQuery();

                if (adminPaqAccount.CollectDate.Ticks == 0)
                {
                    sqlString = "UPDATE ctrl_cuenta SET f_cobro = null where id_doco=@id";
                    cmd = new NpgsqlCommand(sqlString, conn);

                    cmd.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer);
                    cmd.Parameters["@id"].Value = adminPaqAccount.DocId;

                    cmd.ExecuteNonQuery();
                }


                sqlString = "INSERT INTO ctrl_seguimiento(id_movimiento, id_doco, descripcion) " +
                    "VALUES(2, @documento, 'Cuenta actualizada en AdminPaq por el Supervisor. FC:" + adminPaqAccount.CollectDate.ToShortDateString() + ";O:" + adminPaqAccount.Note + ";TC:" + adminPaqAccount.CollectType + ";');";
                cmd = new NpgsqlCommand(sqlString, conn);

                cmd.Parameters.Add("@documento", NpgsqlTypes.NpgsqlDbType.Integer);
                cmd.Parameters["@documento"].Value = adminPaqAccount.DocId;

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    conn.Close();
                    throw new Exception(UNEXISTING_DOCO_ERR);
                }

                conn.Close();
            }
        }

        public void AddFollowUp(string followUpType, string descripcion, int docId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(ConnString))
            {
                conn.Open();

                string sqlString = "INSERT INTO ctrl_seguimiento (id_movimiento, id_doco, descripcion)" +
                    "VALUES( @id_movimiento, @id_doco, @descripcion);";

                NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);

                cmd.Parameters.Add("@id_movimiento", NpgsqlTypes.NpgsqlDbType.Integer);
                cmd.Parameters.Add("@id_doco", NpgsqlTypes.NpgsqlDbType.Integer);
                cmd.Parameters.Add("@descripcion", NpgsqlTypes.NpgsqlDbType.Varchar, 250);

                int id_movimiento = 8;
                switch (followUpType)
                {
                    case "Llamada":
                        id_movimiento = 9;
                        break;
                    case "Visita":
                        id_movimiento = 10;
                        break;
                    case "Email":
                        id_movimiento = 11;
                        break;
                    default:
                        id_movimiento = 12;
                        break;
                }

                cmd.Parameters["@id_movimiento"].Value = id_movimiento;
                cmd.Parameters["@id_doco"].Value = docId;
                cmd.Parameters["@descripcion"].Value = descripcion;

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    conn.Close();
                    throw new Exception(UNEXISTING_DOCO_ERR);
                }

                conn.Close();
            }
        }

        public void UpdateFollowUp(string followUpType, string descripcion, int followUpId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(ConnString))
            {
                conn.Open();

                string sqlString = "UPDATE ctrl_seguimiento " +
                    "SET id_movimiento = @id_movimiento, " +
                    "descripcion = @descripcion " +
                    "WHERE id_seguimiento = @followUpId;";

                NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);

                cmd.Parameters.Add("@id_movimiento", NpgsqlTypes.NpgsqlDbType.Integer);
                cmd.Parameters.Add("@descripcion", NpgsqlTypes.NpgsqlDbType.Varchar, 250);
                cmd.Parameters.Add("@followUpId", NpgsqlTypes.NpgsqlDbType.Integer);

                int id_movimiento = 8;
                switch (followUpType)
                {
                    case "Llamada":
                        id_movimiento = 9;
                        break;
                    case "Visita":
                        id_movimiento = 10;
                        break;
                    case "Email":
                        id_movimiento = 11;
                        break;
                    default:
                        id_movimiento = 12;
                        break;
                }

                cmd.Parameters["@id_movimiento"].Value = id_movimiento;
                cmd.Parameters["@descripcion"].Value = descripcion;
                cmd.Parameters["@followUpId"].Value = followUpId;

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    conn.Close();
                    throw new Exception(UNEXISTING_DOCO_ERR);
                }
                conn.Close();
            }
        }

        public void RemoveFollowUp(int followUpId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(ConnString))
            {
                conn.Open();

                string sqlString = "DELETE FROM ctrl_seguimiento " +
                    "WHERE id_seguimiento = @followUpId;";

                NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);

                cmd.Parameters.Add("@followUpId", NpgsqlTypes.NpgsqlDbType.Integer);
                cmd.Parameters["@followUpId"].Value = followUpId;

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    conn.Close();
                    throw new Exception(UNEXISTING_DOCO_ERR);
                }
                conn.Close();
            }
        }
    }
}
