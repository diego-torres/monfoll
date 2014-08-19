using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Npgsql;
using System.Diagnostics;

namespace Cobranza.Collectable
{
    public class PgDbCollector
    {

        public bool HasExecuted { get; set; }
        public bool PgHasData
        {
            get 
            {

                string connectionString = ConfigurationManager.ConnectionStrings[Config.Common.MONFOLL].ConnectionString;
                NpgsqlConnection conn;
                NpgsqlDataReader dr;
                NpgsqlCommand cmd;
                int records = 0;

                conn = new NpgsqlConnection(connectionString);
                conn.Open();

                string sqlString = "SELECT COUNT(id_doco) FROM ctrl_cuenta;";

                cmd = new NpgsqlCommand(sqlString, conn);

                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    records = int.Parse(dr[0].ToString());
                }

                dr.Close();
                conn.Close();

                return records>0;
            }
        }

        public Empresa GetCompanyByName(string companyName)
        {
            Empresa result = null;

            string connectionString = ConfigurationManager.ConnectionStrings[Config.Common.MONFOLL].ConnectionString;
            NpgsqlConnection conn;
            NpgsqlDataReader dr;
            NpgsqlCommand cmd;

            conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string sqlString = "SELECT id_empresa, ruta " +
                "FROM cat_empresa " +
                "WHERE nombre_empresa = @sEmpresa;";
            cmd = new NpgsqlCommand(sqlString, conn);
            cmd.Parameters.Add("@sEmpresa", NpgsqlTypes.NpgsqlDbType.Varchar, 150);
            cmd.Parameters["@sEmpresa"].Value = companyName;

            dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                result = new Empresa();
                result.Id = int.Parse( dr["id_empresa"].ToString() );
                result.Ruta = dr["ruta"].ToString();
                result.Nombre = companyName;
            }

            dr.Close();
            conn.Close();
            return result;
        }

        public List<Account> GetAccounts() 
        {
            List<Account> result = new List<Account>();

            string connectionString = ConfigurationManager.ConnectionStrings[Config.Common.MONFOLL].ConnectionString;
            NpgsqlConnection conn;
            NpgsqlDataReader dr;
            NpgsqlCommand cmd;

            conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string sqlString = "SELECT id_doco, ap_id, id_cliente, cat_empresa.id_empresa, cat_empresa.ruta " +
                "FROM ctrl_cuenta " +
                "INNER JOIN cat_empresa ON cat_empresa.id_empresa = ctrl_cuenta.enterprise_id;";
            cmd = new NpgsqlCommand(sqlString, conn);

            dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                Account act = new Account();
                act.DocId = int.Parse( dr["id_doco"].ToString() );
                act.ApId = int.Parse(dr["ap_id"].ToString());
                act.Company.Id = int.Parse(dr["id_cliente"].ToString());
                act.Company.EnterpriseId = int.Parse(dr["id_empresa"].ToString());
                act.Company.EnterprisePath = dr["ruta"].ToString();

                result.Add(act);
            }

            dr.Close();
            conn.Close();

            return result;
        }

        public Empresa GetAccountEnterprise(int enterpriseId)
        {
            Empresa result = null;

            string connectionString = ConfigurationManager.ConnectionStrings[Config.Common.MONFOLL].ConnectionString;
            NpgsqlConnection conn;
            NpgsqlDataReader dr;
            NpgsqlCommand cmd;

            conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string sqlString = "SELECT nombre_empresa, ruta " +
                "FROM cat_empresa " +
                "WHERE id_empresa = @id;";

            cmd = new NpgsqlCommand(sqlString, conn);
            cmd.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters["@id"].Value = enterpriseId;

            dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                result = new Empresa();
                result.Id = enterpriseId;
                result.Nombre = dr["nombre_empresa"].ToString();
                result.Ruta = dr["ruta"].ToString();
            }

            dr.Close();
            conn.Close();

            return result;
        }

        public bool AccountExists(Account account)
        {

            string connectionString = ConfigurationManager.ConnectionStrings[Config.Common.MONFOLL].ConnectionString;
            NpgsqlConnection conn;
            NpgsqlDataReader dr;
            NpgsqlCommand cmd;
            int records = 0;

            conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string sqlString = "SELECT COUNT(id_doco) " +
                "FROM ctrl_cuenta " +
                "WHERE ap_id = @apId AND enterprise_id = @eId;";

            cmd = new NpgsqlCommand(sqlString, conn);
            cmd.Parameters.Add("@apId", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters.Add("@eId", NpgsqlTypes.NpgsqlDbType.Integer);

            cmd.Parameters["@apId"].Value = account.ApId;
            cmd.Parameters["@eId"].Value = account.Company.EnterpriseId;

            dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                records = int.Parse(dr[0].ToString());
            }

            dr.Close();
            conn.Close();

            return records > 0;
        }

        public void AddAccount(Account account, EventLog log, AdminPaqImpl api)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[Config.Common.MONFOLL].ConnectionString;
            NpgsqlConnection conn;

            conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string sqlString = "INSERT INTO ctrl_cuenta (ap_id, enterprise_id, F_DOCUMENTO, F_VENCIMIENTO, F_COBRO, ID_CLIENTE, SERIE_DOCO, FOLIO_DOCO, TIPO_DOCUMENTO, TIPO_COBRO, FACTURADO, " +
                "SALDO, MONEDA, OBSERVACIONES) " +
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

            cmd.Parameters["@id"].Value = account.ApId;
            cmd.Parameters["@enterprise"].Value = account.Company.EnterpriseId;
            cmd.Parameters["@f_documento"].Value = account.DocDate;
            cmd.Parameters["@f_vencimiento"].Value = account.DueDate;
            cmd.Parameters["@f_cobro"].Value = account.CollectDate;

            int coId = CompanyId(conn, account.Company.ApId, account.Company.EnterpriseId);
            if (coId == 0)
            {
                log.WriteEntry("Unable to find client apId: " + account.Company.ApId + "; enterprise: " + account.Company.EnterpriseId, EventLogEntryType.Warning);
                Company company = api.GetCompany(account.Company.ApId, account.Company.EnterprisePath, account.Company.EnterpriseId);
                if (company != null)
                    AddCompany(conn, company, log);
                else
                {
                    log.WriteEntry("Unable to find client in adminPaq", EventLogEntryType.Warning);
                    conn.Close();
                    return;
                }

                coId = CompanyId(conn, account.Company.ApId, account.Company.EnterpriseId);
                if (coId == 0)
                {
                    conn.Close();
                    return;
                }
            }

            cmd.Parameters["@id_cliente"].Value = coId;
            cmd.Parameters["@serie_doco"].Value = account.Serie;
            cmd.Parameters["@folio_doco"].Value = account.Folio;
            cmd.Parameters["@tipo_documento"].Value = account.DocType;
            cmd.Parameters["@tipo_cobro"].Value = account.CollectType;
            cmd.Parameters["@facturado"].Value = account.Amount;
            cmd.Parameters["@saldo"].Value = account.Balance;
            cmd.Parameters["@moneda"].Value = account.Currency;
            cmd.Parameters["@observaciones"].Value = account.Note;

            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public void UpdateAccount(Account account, bool isCancelled, EventLog log, AdminPaqImpl api)
        {
            if (isCancelled)
            {
                CloseAccount(account.DocId);
                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings[Config.Common.MONFOLL].ConnectionString;
            NpgsqlConnection conn;

            conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string sqlString = "UPDATE ctrl_cuenta " +
                "SET F_DOCUMENTO = @f_documento, " +
                "F_VENCIMIENTO = @f_vencimiento, " +
                "ID_CLIENTE = @id_cliente, " +
                "SERIE_DOCO = @serie_doco, " +
                "FOLIO_DOCO = @folio_doco, " +
                "TIPO_DOCUMENTO = @tipo_documento, " +
                "FACTURADO = @facturado, " +
                "SALDO = @saldo, " +
                "MONEDA = @moneda, " +
                "TS_DESCARGADO = CURRENT_TIMESTAMP " +
                "WHERE ap_id = @id AND enterprise_id = @enterprise";

            NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@f_documento", NpgsqlTypes.NpgsqlDbType.Date);
            cmd.Parameters.Add("@f_vencimiento", NpgsqlTypes.NpgsqlDbType.Date);
            cmd.Parameters.Add("@id_cliente", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters.Add("@serie_doco", NpgsqlTypes.NpgsqlDbType.Varchar, 4);
            cmd.Parameters.Add("@folio_doco", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters.Add("@tipo_documento", NpgsqlTypes.NpgsqlDbType.Varchar, 150);
            cmd.Parameters.Add("@facturado", NpgsqlTypes.NpgsqlDbType.Money);
            cmd.Parameters.Add("@saldo", NpgsqlTypes.NpgsqlDbType.Money);
            cmd.Parameters.Add("@moneda", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            cmd.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters.Add("@enterprise", NpgsqlTypes.NpgsqlDbType.Integer);

            cmd.Parameters["@f_documento"].Value = account.DocDate;
            cmd.Parameters["@f_vencimiento"].Value = account.DueDate;

            int coId = CompanyId(conn, account.Company.ApId, account.Company.EnterpriseId);
            if (coId == 0)
            {
                log.WriteEntry("Unable to find client apId: " + account.Company.ApId + "; enterprise: " + account.Company.EnterpriseId, EventLogEntryType.Warning);
                Company company = api.GetCompany(account.Company.ApId, account.Company.EnterprisePath, account.Company.EnterpriseId);
                if (company != null)
                    AddCompany(conn, company, log);
                else
                {
                    log.WriteEntry("Unable to find client in adminPaq", EventLogEntryType.Warning);
                    conn.Close();
                    return;
                }

                coId = CompanyId(conn, account.Company.ApId, account.Company.EnterpriseId);
                if (coId == 0)
                {
                    conn.Close();
                    return;
                }
            }

            cmd.Parameters["@id_cliente"].Value = coId;
            cmd.Parameters["@serie_doco"].Value = account.Serie;
            cmd.Parameters["@folio_doco"].Value = account.Folio;
            cmd.Parameters["@tipo_documento"].Value = account.DocType;
            cmd.Parameters["@facturado"].Value = account.Amount;
            cmd.Parameters["@saldo"].Value = account.Balance;
            cmd.Parameters["@moneda"].Value = account.Currency;
            cmd.Parameters["@id"].Value = account.ApId;
            cmd.Parameters["@enterprise"].Value = account.Company.EnterpriseId;

            cmd.ExecuteNonQuery();
            conn.Close();
        }


        private void AddCompany(NpgsqlConnection conn, Company company, EventLog log)
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

            cmd.Parameters["@id"].Value = company.ApId;
            cmd.Parameters["@empresa"].Value = company.EnterpriseId;
            cmd.Parameters["@codigo"].Value = company.Code;
            cmd.Parameters["@nombre_cliente"].Value = company.Name;
            cmd.Parameters["@agente"].Value = company.AgentCode;
            cmd.Parameters["@dia_pago"].Value = company.PaymentDay;
            cmd.Parameters["@local"].Value = company.EsLocal;

            try {
                cmd.ExecuteNonQuery();
            }
            catch(Exception ex){
                log.WriteEntry("Unable to upload company: " + company.ApId + "; eid: " + company.EnterpriseId, EventLogEntryType.Warning);
                log.WriteEntry(ex.Message + " || " + ex.StackTrace, EventLogEntryType.Warning);
            }
            
        }

        public void SavePayment(Payment payment, EventLog log)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[Config.Common.MONFOLL].ConnectionString;
            NpgsqlConnection conn;

            conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string sqlString = "SELECT id_abono " +
                "FROM ctrl_abono " +
                "WHERE id_abono = @pId;";

            NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);
            cmd.Parameters.Add("@pId", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters["@pId"].Value = payment.PaymentId;
            
            NpgsqlDataReader dr = cmd.ExecuteReader();
            bool exists = dr.Read();
            dr.Close();

            if (exists)
                UpdatePayment(conn, payment, log);
            else
                AddPayment(conn, payment, log);

            conn.Close();
        }

        private void AddPayment(NpgsqlConnection conn, Payment payment, EventLog log)
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

            try {
                cmd.ExecuteNonQuery();
            } catch(Exception ex)
            {
                log.WriteEntry(ex.Message + " || " + ex.StackTrace, EventLogEntryType.Warning);
            }
            
        }

        private void UpdatePayment(NpgsqlConnection conn, Payment payment, EventLog log)
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

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                log.WriteEntry(ex.Message + " || " + ex.StackTrace, EventLogEntryType.Warning);
            }
        }

        private int CompanyId(NpgsqlConnection conn, int apId, int empresaId)
        {
            NpgsqlDataReader dr;
            NpgsqlCommand cmd;
            int result = 0;

            string sqlString = "SELECT id_cliente " +
                "FROM cat_cliente " +
                "WHERE ap_id = @apId AND id_empresa = @eId;";

            cmd = new NpgsqlCommand(sqlString, conn);
            cmd.Parameters.Add("@apId", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters["@apId"].Value = apId;
            cmd.Parameters.Add("@eId", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters["@eId"].Value = empresaId;

            dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                result = int.Parse(dr[0].ToString());
            }

            dr.Close();

            return result;
        }

        private void CloseAccount(int accountId)
        {
         
            string connectionString = ConfigurationManager.ConnectionStrings[Config.Common.MONFOLL].ConnectionString;
            NpgsqlConnection conn;

            conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string sqlString = "DELETE FROM ctrl_cuenta " +
                "WHERE id_doco = @docId;";

            NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);
            cmd.Parameters.Add("@docId", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters["@docId"].Value = accountId;

            cmd.ExecuteNonQuery();
            
            conn.Close();
        }

    }
}
