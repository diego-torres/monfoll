using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Npgsql;
using SeguimientoGerente.Properties;
using System.Globalization;

namespace SeguimientoGerente.Collectable.PostgresImpl
{
    public class Account : CommonBase
    {
        public void SetCollectDate(AdminPaqImp api)
        {
            if (conn == null || conn.State != ConnectionState.Open)
                connect();

            string sqlString = "SELECT id_doco, f_documento, f_vencimiento, dia_pago, ctrl_cuenta.ap_id " + 
                "FROM ctrl_cuenta INNER JOIN cat_cliente ON ctrl_cuenta.id_cliente = cat_cliente.id_cliente " +
                "WHERE f_cobro = '0001-01-01'";

            NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();
            Dictionary<int, DateTime> sugeridasPorDocId = new Dictionary<int, DateTime>();
            //Dictionary<int, DateTime> fechasCobroByApId = new Dictionary<int, DateTime>();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    DateTime fDoco, fVto, fSugerida;
                    fDoco = reader.GetDateTime(1);
                    fVto = reader.GetDateTime(2);
                    int idDoco = int.Parse(reader.GetValue(0).ToString());
                    int apId = int.Parse(reader.GetValue(4).ToString());
                    string diaPago = reader.GetString(3);

                    TimeSpan span = fVto - fDoco;
                    int availableDays = (fVto - fDoco).Days;
                    int deltaCobro = (int)Math.Ceiling(availableDays * 0.4);

                    fSugerida = fDoco.AddDays(deltaCobro);
                    if (!string.Empty.Equals(diaPago.Trim()) && !"VENCIMIENTO".Equals(diaPago.ToUpper().Trim()))
                    {
                        int i = 0;
                        Boolean found = false;
                        for (i = 0; i <= 7; i++)
                        {
                            string dow = fSugerida.ToString("ddd", new CultureInfo("es-MX"));
                            dow = dow.Replace('á', 'a');
                            dow = dow.Replace('é', 'e');
                            if (diaPago.ToUpper().Contains(dow.ToUpper()))
                            {
                                found = true;
                                break;
                            }
                            fSugerida = fSugerida.AddDays(1);
                        }

                        if (!found)
                        {
                            i = i * -1;
                            fSugerida = fSugerida.AddDays(i);
                        }
                    }

                    string dayOfWeek = fSugerida.ToString("ddd", new CultureInfo("es-MX"));
                    if ("DOM".Equals(dayOfWeek.ToUpper()))
                        fSugerida = fSugerida.AddDays(1);

                    sugeridasPorDocId.Add(idDoco, fSugerida);
                }
            }
            reader.Close();

            foreach (int docId in sugeridasPorDocId.Keys)
            {
                DateTime fSugerida;
                bool gotValue = sugeridasPorDocId.TryGetValue(docId, out fSugerida);
                if (gotValue)
                {

                    string updateString = "UPDATE ctrl_cuenta SET f_cobro_esperada=@fecha WHERE id_doco = @id";
                    NpgsqlCommand updateCommand = new NpgsqlCommand(updateString, conn);

                    updateCommand.Parameters.Add("@fecha", NpgsqlTypes.NpgsqlDbType.Date);
                    updateCommand.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer);

                    updateCommand.Parameters["@fecha"].Value = fSugerida;
                    updateCommand.Parameters["@id"].Value = docId;

                    updateCommand.ExecuteNonQuery();

                    sqlString = "INSERT INTO ctrl_seguimiento(id_movimiento, id_doco, descripcion) " +
                    "VALUES(2, @documento, 'Fecha de cobro esperada calculada por el sistema.');";

                    updateCommand = new NpgsqlCommand(sqlString, conn);

                    updateCommand.Parameters.Add("@documento", NpgsqlTypes.NpgsqlDbType.Integer);
                    updateCommand.Parameters["@documento"].Value = docId;

                    updateCommand.ExecuteNonQuery();
                }
            }
            conn.Close();
        }

        public DataTable Uncollectable()
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
                "WHERE ctrl_cuenta.id_doco IN(SELECT id_doco FROM ctrl_seguimiento WHERE id_movimiento=7);";

            if (conn == null || conn.State != ConnectionState.Open)
                connect();

            da = new NpgsqlDataAdapter(sqlString, conn);

            ds.Reset();
            da.Fill(ds);
            conn.Close();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                DateTime now = DateTime.Now;
                DateTime dueDate = DateTime.Parse(row["f_vencimiento"].ToString());
                TimeSpan elapsed = now.Subtract(dueDate);

                row["dias_vencido"] = int.Parse(elapsed.TotalDays.ToString("0")) - 1;
            }


            return ds.Tables[0];
        }

        public DataTable Escalated()
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
                "AND ctrl_cuenta.id_doco NOT IN (SELECT id_doco FROM ctrl_seguimiento WHERE id_movimiento=7);";
             
            if (conn == null || conn.State != ConnectionState.Open)
                connect();

            da = new NpgsqlDataAdapter(sqlString, conn);

            ds.Reset();
            da.Fill(ds);
            conn.Close();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                DateTime now = DateTime.Now;
                DateTime dueDate = DateTime.Parse(row["f_vencimiento"].ToString());
                TimeSpan elapsed = now.Subtract(dueDate);

                row["dias_vencido"] = int.Parse(elapsed.TotalDays.ToString("0")) - 1;
            }

            return ds.Tables[0];
        }

        public DataTable Attended()
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
                "AND ctrl_cuenta.id_doco NOT IN (SELECT id_doco FROM ctrl_seguimiento WHERE id_movimiento IN(6,7));";

            if (conn == null || conn.State != ConnectionState.Open)
                connect();

            da = new NpgsqlDataAdapter(sqlString, conn);

            ds.Reset();
            da.Fill(ds);
            conn.Close();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                DateTime now = DateTime.Now;
                DateTime dueDate = DateTime.Parse(row["f_vencimiento"].ToString());
                TimeSpan elapsed = now.Subtract(dueDate);

                row["dias_vencido"] = int.Parse(elapsed.TotalDays.ToString("0")) - 1;
            }

            return ds.Tables[0];
        }

        public DataTable BlackListed()
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
                "WHERE ctrl_cuenta.lista_negra = true AND ctrl_cuenta.id_doco NOT IN(SELECT id_doco FROM ctrl_seguimiento WHERE id_movimiento IN(4,6,7));";

            if (conn == null || conn.State != ConnectionState.Open)
                connect();

            da = new NpgsqlDataAdapter(sqlString, conn);

            ds.Reset();
            da.Fill(ds);
            conn.Close();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                DateTime now = DateTime.Now;
                DateTime dueDate = DateTime.Parse(row["f_vencimiento"].ToString());
                TimeSpan elapsed = now.Subtract(dueDate);

                row["dias_vencido"] = int.Parse(elapsed.TotalDays.ToString("0")) - 1;
            }

            return ds.Tables[0];
        }

        public DataTable MasterTable()
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
                "WHERE ctrl_cuenta.lista_negra = false AND ctrl_cuenta.id_doco NOT IN(SELECT id_doco FROM ctrl_seguimiento WHERE id_movimiento IN(4,6,7))" +
                "ORDER BY f_documento, folio_doco;";

            if (conn == null || conn.State != ConnectionState.Open)
                connect();

            da = new NpgsqlDataAdapter(sqlString, conn);

            ds.Reset();
            da.Fill(ds);
            conn.Close();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                DateTime now = DateTime.Now;
                DateTime dueDate = DateTime.Parse(row["f_vencimiento"].ToString());
                TimeSpan elapsed = now.Subtract(dueDate);

                row["dias_vencido"] = int.Parse(elapsed.TotalDays.ToString("0")) - 1;
            }

            return ds.Tables[0];
        }

        public void SetObservations(int docId, string collectType, string observations)
        {
            if (conn == null || conn.State != ConnectionState.Open)
                connect();

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

            cmd.ExecuteNonQuery();

            conn.Close();
        }

        public void SetCollectDate(int docId, DateTime collectDate)
        {
            Boolean gotConnected = false;
            if (conn == null || conn.State != ConnectionState.Open)
            {
                connect();
                gotConnected = true;
            }   

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

            cmd.ExecuteNonQuery();

            if(gotConnected)
                conn.Close();
        }

        public void Review(int docId)
        {
            if (conn == null || conn.State != ConnectionState.Open)
                connect();

            string sqlString = "DELETE FROM ctrl_seguimiento " +
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

            cmd.ExecuteNonQuery();

            conn.Close();
        }

        public void Especialize(int docId)
        {
            if (conn == null || conn.State != ConnectionState.Open)
                connect();

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

            cmd.ExecuteNonQuery();

            conn.Close();
        }

        public void UnEspecialize(int docId)
        {
            if (conn == null || conn.State != ConnectionState.Open)
                connect();

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

            cmd.ExecuteNonQuery();

            conn.Close();
        }

        public void Escale(int docId)
        {
            if (conn == null || conn.State != ConnectionState.Open)
                connect();

            string sqlString = "INSERT INTO ctrl_seguimiento(id_movimiento, id_doco, descripcion) " +
                "VALUES(6, @documento, 'Cuenta escalada a gerencia.');";

            NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@documento", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters["@documento"].Value = docId;

            cmd.ExecuteNonQuery();
            conn.Close();
        }
        
        public void Unescale(int docId)
        {
            if (conn == null || conn.State != ConnectionState.Open)
                connect();

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

            cmd.ExecuteNonQuery();

            conn.Close();
        }

        public void Uncollectable(int docId)
        {
            if (conn == null || conn.State != ConnectionState.Open)
                connect();

            string sqlString = "INSERT INTO ctrl_seguimiento(id_movimiento, id_doco, descripcion) " +
                    "VALUES(7, @documento, 'Documento marcado incobrable por gerencia.');";

            NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@documento", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters["@documento"].Value = docId;

            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public void Collectable(int docId)
        {
            if (conn == null || conn.State != ConnectionState.Open)
                connect();

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

            cmd.ExecuteNonQuery();

            conn.Close();
        }

        public DataTable ReadSeries()
        {
            DataSet ds = new DataSet();
            NpgsqlDataAdapter da;
            string sqlString = "SELECT DISTINCT serie_doco " +
                "FROM ctrl_cuenta;";

            if (conn == null || conn.State != ConnectionState.Open)
                connect();

            da = new NpgsqlDataAdapter(sqlString, conn);

            ds.Reset();
            da.Fill(ds);
            conn.Close();

            return ds.Tables[0];
        }

        public List<int> AdminPaqIds(int enterprise)
        {
            Boolean gotConnection = false;
            if (conn == null || conn.State != ConnectionState.Open)
            {
                connect();
                gotConnection = true;
            }   

            DataSet ds = new DataSet();
            NpgsqlDataAdapter da;
            string sqlString = "SELECT DISTINCT ap_id " +
                "FROM ctrl_cuenta " + 
                "WHERE enterprise_id=" + enterprise.ToString() + ";";
           
            da = new NpgsqlDataAdapter(sqlString, conn);

            ds.Reset();
            da.Fill(ds);

            if(gotConnection)
                conn.Close();

            List<int> result = new List<int>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                result.Add(int.Parse(dr["ap_id"].ToString()));
            }

            return result;
        }

        public DataTable ReadFolios()
        {
            DataSet ds = new DataSet();
            NpgsqlDataAdapter da;
            string sqlString = "SELECT folio_doco " +
                "FROM ctrl_cuenta;";

            if (conn == null || conn.State != ConnectionState.Open)
                connect();

            da = new NpgsqlDataAdapter(sqlString, conn);

            ds.Reset();
            da.Fill(ds);
            conn.Close();

            return ds.Tables[0];
        }

        public DataTable FollowUp(int docId)
        {
            DataSet ds = new DataSet();
            NpgsqlDataAdapter da;

            string sqlString = "SELECT id_seguimiento, ctrl_seguimiento.id_movimiento, cat_movimiento.descripcion AS movimiento, " +
                "system_based, ctrl_seguimiento.descripcion as seguimiento, ts_seguimiento " +
                "FROM ctrl_seguimiento INNER JOIN cat_movimiento ON ctrl_seguimiento.id_movimiento = cat_movimiento.id_movimiento " +
                "WHERE id_doco = " + docId.ToString() + ";";

            if (conn == null || conn.State != ConnectionState.Open)
                connect();

            da = new NpgsqlDataAdapter(sqlString, conn);

            ds.Reset();
            da.Fill(ds);
            conn.Close();

            if (ds.Tables.Count == 0) return null;

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

        public void UploadAccounts(List<Collectable.Account> adminPaqAccounts, List<int> cancelled, List<int> saldados, Dictionary<int, Concepto> conceptos)
        {
            Boolean gotConnection = false;
            if (conn == null || conn.State != ConnectionState.Open)
            {
                connect();
                gotConnection = true;
            }
                

            Dictionary<int, Company> savedCompanies = new Dictionary<int, Company>();
            Dictionary<int, Concepto> savedConcepts = new Dictionary<int, Concepto>();
            foreach (Collectable.Account adminPaqAccount in adminPaqAccounts)
            {
                if (!savedCompanies.ContainsKey(adminPaqAccount.Company.ApId))
                {
                    SaveCompany(adminPaqAccount.Company);
                    savedCompanies.Add(adminPaqAccount.Company.ApId, adminPaqAccount.Company);
                }

                SaveAccount(adminPaqAccount);

                foreach (Collectable.Payment payment in adminPaqAccount.Payments)
                {
                    payment.DocId = GetDocIdFromAccount(adminPaqAccount);
                    SavePayment(payment);
                }

            }

            Settings set = Settings.Default;
            List<int> APIdsInDatabase = AdminPaqIds(set.empresa);
            foreach (int AdminPaqId in cancelled)
            {
                if(APIdsInDatabase.Contains(AdminPaqId))
                {
                    int pgId = GetDocIdFromAdminPaq(AdminPaqId);
                    if (pgId != -1) CancelAccount(pgId);
                }
            }

            
            foreach (int AdminPaqId in saldados)
            {
                if(APIdsInDatabase.Contains(AdminPaqId))
                {
                    int pgId = GetDocIdFromAdminPaq(AdminPaqId);
                    if (pgId != -1) CancelAccount(pgId);       
                }
            }

            foreach (Concepto concepto in conceptos.Values)
            {
                if (!savedConcepts.ContainsKey(concepto.APId))
                {
                    SaveConcepto(concepto);
                    savedConcepts.Add(concepto.APId, concepto);
                }
            }

            if(gotConnection)
                conn.Close();
        }

        public void UpdateAccountById(Collectable.Account adminPaqAccount)
        {
            if (conn == null || conn.State != ConnectionState.Open) connect();

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
                "VALUES(2, @documento, 'Cuenta actualizada en AdminPaq por el Gerente.');";
            cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@documento", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters["@documento"].Value = adminPaqAccount.DocId;

            cmd.ExecuteNonQuery();

            conn.Close();
        }

        public void CancelAccount(int docId)
        {
            bool connected = false;
            if (conn == null || conn.State != ConnectionState.Open)
            {
                connect();
                connected = true;
            }

            string sqlString = "DELETE FROM ctrl_cuenta " +
                "WHERE id_doco = @docId;";

            NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);
            cmd.Parameters.Add("@docId", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters["@docId"].Value = docId;

            cmd.ExecuteNonQuery();

            if (connected)
                conn.Close();
        }

        public void SavePayment(Payment payment)
        {
            bool connected = false;
            if (conn == null || conn.State != ConnectionState.Open)
            {
                connect();
                connected = true;
            }

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            NpgsqlDataAdapter da;
            string sqlString = "SELECT id_abono FROM ctrl_abono WHERE id_abono = " + payment.PaymentId.ToString() + ";";
            da = new NpgsqlDataAdapter(sqlString, conn);

            ds.Reset();
            da.Fill(ds);
            dt = ds.Tables[0];

            if (dt.Rows.Count >= 1)
                UpdatePayment(payment);
            else
                AddPayment(payment);

            if (connected) conn.Close();
        }

        public void SaveAccount(Collectable.Account adminPaqAccount)
        {
            bool connected = false;
            if (conn == null || conn.State != ConnectionState.Open)
            {
                connect();
                connected = true;
            }

            if (DocumentExists(adminPaqAccount.ApId, adminPaqAccount.Company.EnterpriseId))
                UpdateAccount(adminPaqAccount);
            else
                AddAccount(adminPaqAccount);

            if (connected)
                conn.Close();
        }

        public void AddFollowUp(string followUpType, string descripcion, int docId)
        {
            if (conn == null || conn.State != ConnectionState.Open)
                connect();

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

            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public void UpdateFollowUp(string followUpType, string descripcion, int followUpId)
        {
            if (conn == null || conn.State != ConnectionState.Open)
                connect();

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

            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public void RemoveFollowUp(int followUpId)
        {
            if (conn == null || conn.State != ConnectionState.Open)
                connect();

            string sqlString = "DELETE FROM ctrl_seguimiento " +
                "WHERE id_seguimiento = @followUpId;";

            NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@followUpId", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters["@followUpId"].Value = followUpId;

            cmd.ExecuteNonQuery();
            conn.Close();
        }

        private int GetDocIdFromAccount(Collectable.Account account)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            NpgsqlDataAdapter da;
            string sqlString = "SELECT id_doco " +
                "FROM ctrl_cuenta " +
                "WHERE ap_id = " + account.ApId.ToString() +
                " AND enterprise_id = " + account.Company.EnterpriseId.ToString() + ";";
            da = new NpgsqlDataAdapter(sqlString, conn);

            ds.Reset();
            da.Fill(ds);
            dt = ds.Tables[0];

            return int.Parse(dt.Rows[0]["id_doco"].ToString());
        }

        private int GetDocIdFromAdminPaq (int AdminPaqId)
        {
            Settings configusuario = Settings.Default;

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            NpgsqlDataAdapter da;
            string sqlString = "SELECT id_doco " +
                "FROM ctrl_cuenta " +
                "WHERE ap_id = " + AdminPaqId.ToString() +
                " AND enterprise_id = " + configusuario.empresa.ToString() + ";";
            da = new NpgsqlDataAdapter(sqlString, conn);

            ds.Reset();
            da.Fill(ds);
            dt = ds.Tables[0];
            if (dt.Rows.Count == 0)
            {
                return -1;
            }

            return int.Parse(dt.Rows[0]["id_doco"].ToString());
        }

        private void SaveConcepto(Concepto concepto)
        {
            if (!ConceptoExists(concepto))
            {
                AddConcepto(concepto);
            }
        }

        private void AddConcepto(Concepto concepto)
        {
            string sqlString = "INSERT INTO cat_concepto (ap_id, id_empresa, codigo_concepto, nombre_concepto, razon) " +
                "VALUES(@ap_id, @id_empresa, @codigo, @nombre, @razon);";

            NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@ap_id", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters.Add("@id_empresa", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters.Add("@codigo", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            cmd.Parameters.Add("@nombre", NpgsqlTypes.NpgsqlDbType.Varchar, 150);
            cmd.Parameters.Add("@razon", NpgsqlTypes.NpgsqlDbType.Varchar, 50);

            cmd.Parameters["@ap_id"].Value = concepto.APId;
            cmd.Parameters["@id_empresa"].Value = concepto.IdEmpresa;
            cmd.Parameters["@codigo"].Value = concepto.Codigo;
            cmd.Parameters["@nombre"].Value = concepto.Nombre;
            cmd.Parameters["@razon"].Value = concepto.Razon;

            cmd.ExecuteNonQuery();
        }

        private bool ConceptoExists(Concepto concepto)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            NpgsqlDataAdapter da;
            string sqlString = "SELECT id_concepto " +
                "FROM cat_concepto " +
                "WHERE ap_id = " + concepto.APId.ToString() + " " +
                "AND id_empresa = " + concepto.IdEmpresa.ToString() + ";";
            da = new NpgsqlDataAdapter(sqlString, conn);

            ds.Reset();
            da.Fill(ds);
            dt = ds.Tables[0];

            return dt.Rows.Count >= 1;
        }

        private bool DocumentExists(int AdminPaqId, int EnterpriseId)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            NpgsqlDataAdapter da;
            string sqlString = "SELECT id_doco FROM ctrl_cuenta " +
                "WHERE ap_id = " + AdminPaqId.ToString() +
                " AND enterprise_id = " + EnterpriseId.ToString () + ";";
            da = new NpgsqlDataAdapter(sqlString, conn);

            ds.Reset();
            da.Fill(ds);
            dt = ds.Tables[0];

            return dt.Rows.Count >= 1;
        }

        private void UpdatePayment(Payment payment)
        {
            string sqlString = "UPDATE ctrl_abono " +
                "SET ID_DOCO = @id_doc, " +
                "TIPO_PAGO = @tipo_pago, " +
                "IMPORTE_PAGO = @importe, " +
                "FOLIO = @folio, " +
                "concepto = @concepto, " +
                "fecha_deposito = @fecha_deposito, " +
                "cuenta = @cuenta " +
                "WHERE ID_ABONO = @id";

            NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@id_doc", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters.Add("@tipo_pago", NpgsqlTypes.NpgsqlDbType.Varchar, 150);
            cmd.Parameters.Add("@importe", NpgsqlTypes.NpgsqlDbType.Money);
            cmd.Parameters.Add("@folio", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters.Add("@concepto", NpgsqlTypes.NpgsqlDbType.Varchar, 150);
            cmd.Parameters.Add("@fecha_deposito", NpgsqlTypes.NpgsqlDbType.Date);
            cmd.Parameters.Add("@cuenta", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            cmd.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer);

            cmd.Parameters["@id_doc"].Value = payment.DocId;
            cmd.Parameters["@tipo_pago"].Value = payment.PaymentType;
            cmd.Parameters["@importe"].Value = payment.Amount;
            cmd.Parameters["@folio"].Value = payment.Folio;
            cmd.Parameters["@concepto"].Value = payment.Concept;
            cmd.Parameters["@fecha_deposito"].Value = payment.DepositDate;
            cmd.Parameters["@cuenta"].Value = payment.Account;
            cmd.Parameters["@id"].Value = payment.PaymentId;

            cmd.ExecuteNonQuery();
        }

        private void AddPayment(Payment payment)
        {
            string sqlString = "INSERT INTO ctrl_abono (id_abono, id_doco, tipo_pago, importe_pago, folio, concepto, fecha_deposito, cuenta) " +
                "VALUES(@id, @id_doc, @tipo_pago, @importe, @folio, @concepto, @fecha_deposito, @cuenta);";

            NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@id_doc", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters.Add("@tipo_pago", NpgsqlTypes.NpgsqlDbType.Varchar, 150);
            cmd.Parameters.Add("@importe", NpgsqlTypes.NpgsqlDbType.Money);
            cmd.Parameters.Add("@folio", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters.Add("@concepto", NpgsqlTypes.NpgsqlDbType.Varchar, 150);
            cmd.Parameters.Add("@fecha_deposito", NpgsqlTypes.NpgsqlDbType.Date);
            cmd.Parameters.Add("@cuenta", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            cmd.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer);

            cmd.Parameters["@id_doc"].Value = payment.DocId;
            cmd.Parameters["@tipo_pago"].Value = payment.PaymentType;
            cmd.Parameters["@importe"].Value = payment.Amount;
            cmd.Parameters["@folio"].Value = payment.Folio;
            cmd.Parameters["@concepto"].Value = payment.Concept;
            cmd.Parameters["@fecha_deposito"].Value = payment.DepositDate;
            cmd.Parameters["@cuenta"].Value = payment.Account;
            cmd.Parameters["@id"].Value = payment.PaymentId;

            cmd.ExecuteNonQuery();
        }

        private void SaveCompany(Company adminPaqCompany)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            NpgsqlDataAdapter da;
            string sqlString = "SELECT id_cliente " +
                "FROM cat_cliente " +
                "WHERE ap_id = " + adminPaqCompany.ApId.ToString() +
                " AND id_empresa = " + adminPaqCompany.EnterpriseId.ToString() + ";";
            da = new NpgsqlDataAdapter(sqlString, conn);

            ds.Reset();
            da.Fill(ds);
            dt = ds.Tables[0];

            if (dt.Rows.Count >= 1)
                UpdateCompany(adminPaqCompany);
            else
                AddCompany(adminPaqCompany);

        }

        private void UpdateCompany(Company adminPaqCompany)
        {
            if (conn == null || conn.State != ConnectionState.Open)
                connect();

            string sqlString = "UPDATE cat_cliente " +
                "SET cd_cliente = @codigo, " +
                "nombre_cliente = @nombre_cliente, " +
                "ruta = @agente, " +
                "dia_pago = @dia_pago, " +
                "es_local = @local " +
                "WHERE ap_id = @id " +
                "AND id_empresa = @empresa";

            NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@codigo", NpgsqlTypes.NpgsqlDbType.Varchar, 6);
            cmd.Parameters.Add("@nombre_cliente", NpgsqlTypes.NpgsqlDbType.Varchar, 150);
            cmd.Parameters.Add("@agente", NpgsqlTypes.NpgsqlDbType.Varchar, 20);
            cmd.Parameters.Add("@dia_pago", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            cmd.Parameters.Add("@local", NpgsqlTypes.NpgsqlDbType.Boolean);
            cmd.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters.Add("@empresa", NpgsqlTypes.NpgsqlDbType.Integer);

            cmd.Parameters["@codigo"].Value = adminPaqCompany.Code;
            cmd.Parameters["@nombre_cliente"].Value = adminPaqCompany.Name;
            cmd.Parameters["@agente"].Value = adminPaqCompany.AgentCode;
            cmd.Parameters["@dia_pago"].Value = adminPaqCompany.PaymentDay;
            cmd.Parameters["@local"].Value = adminPaqCompany.EsLocal;
            cmd.Parameters["@id"].Value = adminPaqCompany.ApId;
            cmd.Parameters["@empresa"].Value = adminPaqCompany.EnterpriseId;

            cmd.ExecuteNonQuery();
        }

        private void AddCompany(Company adminPaqCompany)
        {
            if (conn == null || conn.State != ConnectionState.Open)
                connect();

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

            cmd.Parameters["@id"].Value = adminPaqCompany.ApId;
            cmd.Parameters["@empresa"].Value = adminPaqCompany.EnterpriseId;
            cmd.Parameters["@codigo"].Value = adminPaqCompany.Code;
            cmd.Parameters["@nombre_cliente"].Value = adminPaqCompany.Name;
            cmd.Parameters["@agente"].Value = adminPaqCompany.AgentCode;
            cmd.Parameters["@dia_pago"].Value = adminPaqCompany.PaymentDay;
            cmd.Parameters["@local"].Value = adminPaqCompany.EsLocal;

            cmd.ExecuteNonQuery();
        }
        
        private void UpdateAccount(Collectable.Account adminPaqAccount)
        {
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
                "WHERE ap_id = @id AND enterprise_id = @enterprise";

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
            cmd.Parameters.Add("@enterprise", NpgsqlTypes.NpgsqlDbType.Integer);

            cmd.Parameters["@f_documento"].Value = adminPaqAccount.DocDate;
            cmd.Parameters["@f_vencimiento"].Value = adminPaqAccount.DueDate;
            cmd.Parameters["@f_cobro"].Value = adminPaqAccount.CollectDate;
            cmd.Parameters["@id_cliente"].Value = adminPaqAccount.Company.Id == 0 ? CompanyId(adminPaqAccount.Company.ApId, adminPaqAccount.Company.EnterpriseId) : adminPaqAccount.Company.Id;
            cmd.Parameters["@serie_doco"].Value = adminPaqAccount.Serie;
            cmd.Parameters["@folio_doco"].Value = adminPaqAccount.Folio;
            cmd.Parameters["@tipo_documento"].Value = adminPaqAccount.DocType;
            cmd.Parameters["@tipo_cobro"].Value = adminPaqAccount.CollectType;
            cmd.Parameters["@facturado"].Value = adminPaqAccount.Amount;
            cmd.Parameters["@saldo"].Value = adminPaqAccount.Balance;
            cmd.Parameters["@moneda"].Value = adminPaqAccount.Currency;
            cmd.Parameters["@observaciones"].Value = adminPaqAccount.Note;
            cmd.Parameters["@id"].Value = adminPaqAccount.ApId;
            cmd.Parameters["@enterprise"].Value = adminPaqAccount.Company.EnterpriseId;

            cmd.ExecuteNonQuery();
        }

        private void AddAccount(Collectable.Account adminPaqAccount)
        {
            if (conn == null || conn.State != ConnectionState.Open)
                connect();

            string sqlString = "INSERT INTO ctrl_cuenta (ap_id, enterprise_id, F_DOCUMENTO, F_VENCIMIENTO, F_COBRO, ID_CLIENTE, SERIE_DOCO, FOLIO_DOCO, TIPO_DOCUMENTO, TIPO_COBRO, FACTURADO, " +
                "SALDO, MONEDA, OBSERVACIONES)" +
                "VALUES( @id, @enterprise, @f_documento, @f_vencimiento, @f_cobro, @id_cliente, @serie_doco, @folio_doco, @tipo_documento, @tipo_cobro, @facturado, @saldo, " +
                "@moneda, @observaciones);";

            NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters.Add("@enterprise", NpgsqlTypes.NpgsqlDbType.Integer);
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

            cmd.Parameters["@id"].Value = adminPaqAccount.ApId;
            cmd.Parameters["@enterprise"].Value = adminPaqAccount.Company.EnterpriseId;
            cmd.Parameters["@f_documento"].Value = adminPaqAccount.DocDate;
            cmd.Parameters["@f_vencimiento"].Value = adminPaqAccount.DueDate;
            cmd.Parameters["@f_cobro"].Value = adminPaqAccount.CollectDate;
            cmd.Parameters["@id_cliente"].Value = CompanyId(adminPaqAccount.Company.ApId, adminPaqAccount.Company.EnterpriseId);
            cmd.Parameters["@serie_doco"].Value = adminPaqAccount.Serie;
            cmd.Parameters["@folio_doco"].Value = adminPaqAccount.Folio;
            cmd.Parameters["@tipo_documento"].Value = adminPaqAccount.DocType;
            cmd.Parameters["@tipo_cobro"].Value = adminPaqAccount.CollectType;
            cmd.Parameters["@facturado"].Value = adminPaqAccount.Amount;
            cmd.Parameters["@saldo"].Value = adminPaqAccount.Balance;
            cmd.Parameters["@moneda"].Value = adminPaqAccount.Currency;
            cmd.Parameters["@observaciones"].Value = adminPaqAccount.Note;

            cmd.ExecuteNonQuery();
        }

        private int CompanyId(int apId, int empresaId)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            NpgsqlDataAdapter da;
            string sqlString = "SELECT id_cliente " +
                "FROM cat_cliente " +
                "WHERE ap_id = " + apId.ToString() +
                " AND id_empresa = " + empresaId.ToString() + ";";
            da = new NpgsqlDataAdapter(sqlString, conn);

            ds.Reset();
            da.Fill(ds);
            dt = ds.Tables[0];

            return int.Parse(dt.Rows[0]["id_cliente"].ToString());
        }

    }
}
