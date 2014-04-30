using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Npgsql;

namespace SeguimientoSuper.Collectable.PostgresImpl
{
    public class Account : CommonBase
    {

        public void SetCollectDate(int docId, DateTime collectDate)
        {
            if (conn == null || conn.State != ConnectionState.Open)
                connect();

            string sqlString = "UPDATE ctrl_cuenta " +
                "SET F_COBRO = @f_cobro " +
                "WHERE ID_DOCO = @id";

            NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@f_cobro", NpgsqlTypes.NpgsqlDbType.Date);
            cmd.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer);

            cmd.Parameters["@f_cobro"].Value = collectDate;
            cmd.Parameters["@id"].Value = docId;

            cmd.ExecuteNonQuery();

            conn.Close();
        }

        public void ReOpen(int docId)
        {
            if (conn == null || conn.State != ConnectionState.Open)
                connect();

            string sqlString = "DELETE FROM ctrl_seguimiento " +
                "WHERE id_doco = @docId AND id_movimiento = 9;";

            NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@docId", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters["@docId"].Value = docId;

            cmd.ExecuteNonQuery();

            sqlString = "INSERT INTO ctrl_seguimiento(id_movimiento, id_doco, descripcion) " +
                "VALUES(11, @documento, 'Cuenta Abierta.');";
            cmd = new NpgsqlCommand(sqlString, conn);

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
                "WHERE id_doco = @docId AND id_movimiento = 4;";

            NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@docId", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters["@docId"].Value = docId;

            cmd.ExecuteNonQuery();

            sqlString = "INSERT INTO ctrl_seguimiento(id_movimiento, id_doco, descripcion) " +
                "VALUES(13, @documento, 'Cuenta desescalada.');";
            cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@documento", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters["@documento"].Value = docId;

            cmd.ExecuteNonQuery();

            conn.Close();
        }

        public void Unassign(int docId)
        {
            if (conn == null || conn.State != ConnectionState.Open)
                connect();

            string sqlString = "DELETE FROM ctrl_asignacion " +
                "WHERE id_doco = @docId;";

            NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@docId", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters["@docId"].Value = docId;

            cmd.ExecuteNonQuery();

            sqlString = "INSERT INTO ctrl_seguimiento(id_movimiento, id_doco, descripcion) " +
                "VALUES(12, @documento, 'Cuenta deasignada.');";
            cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@documento", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters["@documento"].Value = docId;

            cmd.ExecuteNonQuery();

            conn.Close();
        }

        public DataTable Cancelled()
        {
            DataSet ds = new DataSet();
            NpgsqlDataAdapter da;
            string sqlString = "SELECT ctrl_cuenta.id_doco, f_documento, f_vencimiento, f_cobro, ctrl_cuenta.id_cliente, cd_cliente, nombre_cliente, ruta, dia_pago, " +
                "serie_doco, folio_doco, tipo_documento, tipo_cobro, facturado, saldo, moneda, observaciones, CURRENT_DATE - f_vencimiento AS dias_vencido " +
                "FROM ctrl_cuenta INNER JOIN cat_cliente ON ctrl_cuenta.id_cliente = cat_cliente.id_cliente " +
                "WHERE ctrl_cuenta.id_doco IN(SELECT id_doco FROM ctrl_seguimiento WHERE id_movimiento=10);";

            if (conn == null || conn.State != ConnectionState.Open)
                connect();

            da = new NpgsqlDataAdapter(sqlString, conn);

            ds.Reset();
            da.Fill(ds);
            conn.Close();
            return ds.Tables[0];
        }

        public DataTable Closed()
        {
            DataSet ds = new DataSet();
            NpgsqlDataAdapter da;
            string sqlString = "SELECT ctrl_cuenta.id_doco, f_documento, f_vencimiento, f_cobro, ctrl_cuenta.id_cliente, cd_cliente, nombre_cliente, ruta, dia_pago, " +
                "serie_doco, folio_doco, tipo_documento, tipo_cobro, facturado, saldo, moneda, observaciones, CURRENT_DATE - f_vencimiento AS dias_vencido " +
                "FROM ctrl_cuenta INNER JOIN cat_cliente ON ctrl_cuenta.id_cliente = cat_cliente.id_cliente " +
                "WHERE ctrl_cuenta.id_doco IN(SELECT id_doco FROM ctrl_seguimiento WHERE id_movimiento=9);";

            if (conn == null || conn.State != ConnectionState.Open)
                connect();

            da = new NpgsqlDataAdapter(sqlString, conn);

            ds.Reset();
            da.Fill(ds);
            conn.Close();
            return ds.Tables[0];
        }

        public DataTable Escalated()
        {
            DataSet ds = new DataSet();
            NpgsqlDataAdapter da;
            string sqlString = "SELECT ctrl_cuenta.id_doco, f_documento, f_vencimiento, f_cobro, ctrl_cuenta.id_cliente, cd_cliente, nombre_cliente, ruta, dia_pago, " +
                "serie_doco, folio_doco, tipo_documento, tipo_cobro, facturado, saldo, moneda, observaciones, CURRENT_DATE - f_vencimiento AS dias_vencido " +
                "FROM ctrl_cuenta INNER JOIN cat_cliente ON ctrl_cuenta.id_cliente = cat_cliente.id_cliente " +
                "WHERE ctrl_cuenta.id_doco IN(SELECT id_doco FROM ctrl_seguimiento WHERE id_movimiento=4);";

            if (conn == null || conn.State != ConnectionState.Open)
                connect();

            da = new NpgsqlDataAdapter(sqlString, conn);

            ds.Reset();
            da.Fill(ds);
            conn.Close();
            return ds.Tables[0];
        }

        public DataTable Assigned()
        {
            DataSet ds = new DataSet();
            NpgsqlDataAdapter da;
            string sqlString = "SELECT ctrl_cuenta.id_doco, f_documento, f_vencimiento, f_cobro, ctrl_cuenta.id_cliente, cd_cliente, nombre_cliente, ruta, dia_pago, " +
                "serie_doco, folio_doco, tipo_documento, tipo_cobro, facturado, saldo, moneda, observaciones, CURRENT_DATE - f_vencimiento AS dias_vencido, " +
                "nombre_cobrador " +
                "FROM ctrl_cuenta INNER JOIN cat_cliente ON ctrl_cuenta.id_cliente = cat_cliente.id_cliente " +
                "INNER JOIN ctrl_asignacion ON ctrl_cuenta.id_doco = ctrl_asignacion.id_doco " +
                "INNER JOIN cat_cobrador ON ctrl_asignacion.id_cobrador = cat_cobrador.id_cobrador " +
                "WHERE ctrl_cuenta.id_doco NOT IN(SELECT id_doco FROM ctrl_seguimiento WHERE id_movimiento IN(4,9,10));";

            if (conn == null || conn.State != ConnectionState.Open)
                connect();

            da = new NpgsqlDataAdapter(sqlString, conn);

            ds.Reset();
            da.Fill(ds);
            conn.Close();
            return ds.Tables[0];
        }

        public DataTable UnAssigned()
        {
            DataSet ds = new DataSet();
            NpgsqlDataAdapter da;
            string sqlString = "SELECT ctrl_cuenta.id_doco, f_documento, f_vencimiento, f_cobro, ctrl_cuenta.id_cliente, cd_cliente, nombre_cliente, ruta, dia_pago, " +
                "serie_doco, folio_doco, tipo_documento, tipo_cobro, facturado, saldo, moneda, observaciones, CURRENT_DATE - f_vencimiento AS dias_vencido " +
                "FROM ctrl_cuenta INNER JOIN cat_cliente ON ctrl_cuenta.id_cliente = cat_cliente.id_cliente " +
                "WHERE ctrl_cuenta.id_doco NOT IN(SELECT id_doco FROM ctrl_asignacion) " +
                "AND ctrl_cuenta.id_doco NOT IN(SELECT id_doco FROM ctrl_seguimiento WHERE id_movimiento IN(4,9,10));";

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
                "system_based, ctrl_seguimiento.descripcion as seguimiento, ts_seguimiento AT TIME ZONE '00' AS ts_seguimiento " +
                "FROM ctrl_seguimiento INNER JOIN cat_movimiento ON ctrl_seguimiento.id_movimiento = cat_movimiento.id_movimiento " +
                "WHERE id_doco = " + docId.ToString() + ";";

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

        public void UploadAccounts(List<Collectable.Account> adminPaqAccounts, List<int> cancelled)
        {
            if (conn == null || conn.State != ConnectionState.Open)
                connect();

            Dictionary<int, Company> savedCompanies = new Dictionary<int, Company>();
            foreach (Collectable.Account adminPaqAccount in adminPaqAccounts)
            {
                if (!savedCompanies.ContainsKey(adminPaqAccount.Company.Id))
                {
                    SaveCompany(adminPaqAccount.Company);
                    savedCompanies.Add(adminPaqAccount.Company.Id, adminPaqAccount.Company);
                }

                SaveAccount(adminPaqAccount);

                foreach (Collectable.Payment payment in adminPaqAccount.Payments)
                {
                    SavePayment(payment);
                }

            }

            foreach (int docId in cancelled)
            {
                CancelAccount(docId);
            }

            conn.Close();
        }

        public void CloseAccount(int docId)
        {
            if (conn == null || conn.State != ConnectionState.Open)
                connect();

            string sqlString = "INSERT INTO ctrl_seguimiento(id_movimiento, id_doco, descripcion) " +
                    "VALUES(9, @documento, 'Documento cerrado por supervisor');";

            NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@documento", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters["@documento"].Value = docId;

            cmd.ExecuteNonQuery();
            conn.Close();
        }

        private void CancelAccount(int docId)
        {
            if (DocumentExists(docId))
            {
                if (IsCancelled(docId)) return;

                if (conn == null || conn.State != ConnectionState.Open)
                    connect();

                string sqlString = "INSERT INTO ctrl_seguimiento(id_movimiento, id_doco, descripcion) " +
                    "VALUES(10, @docId, 'Documento cancelado en AdminPaq');";

                NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);
                cmd.Parameters.Add("@docId", NpgsqlTypes.NpgsqlDbType.Integer);
                cmd.Parameters["@docId"].Value = docId;
                
                cmd.ExecuteNonQuery();
            }
        }

        private bool IsCancelled(int docId)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            NpgsqlDataAdapter da;
            string sqlString = "SELECT id_doco FROM ctrl_seguimiento " +
                "WHERE id_doco = " + docId.ToString() + " " +
                "AND id_movimiento = 10;";
            da = new NpgsqlDataAdapter(sqlString, conn);

            ds.Reset();
            da.Fill(ds);
            dt = ds.Tables[0];

            return dt.Rows.Count >= 1;
        }

        private bool DocumentExists(int docId)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            NpgsqlDataAdapter da;
            string sqlString = "SELECT id_doco FROM ctrl_cuenta WHERE id_doco = " + docId.ToString() + ";";
            da = new NpgsqlDataAdapter(sqlString, conn);

            ds.Reset();
            da.Fill(ds);
            dt = ds.Tables[0];

            return dt.Rows.Count >= 1;
        }

        private void SavePayment(Payment payment)
        {
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
        }

        private void UpdatePayment(Payment payment)
        {
            if (conn == null || conn.State != ConnectionState.Open)
                connect();

            string sqlString = "UPDATE ctrl_abono " +
                "SET ID_DOCO = @id_doc, " +
                "TIPO_PAGO = @tipo_pago, " +
                "IMPORTE_PAGO = @importe, " +
                "FOLIO = @folio, " +
                "concepto = @concepto, " +
                "fecha_deposito = @fecha_deposito, " +
                "cuenta = @cuenta "+
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
            string sqlString = "SELECT id_cliente FROM cat_cliente WHERE id_cliente = " + adminPaqCompany.Id.ToString() + ";";
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
                "dia_pago = @dia_pago " +
                "WHERE id_cliente = @id";

            NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@codigo", NpgsqlTypes.NpgsqlDbType.Varchar, 6);
            cmd.Parameters.Add("@nombre_cliente", NpgsqlTypes.NpgsqlDbType.Varchar, 150);
            cmd.Parameters.Add("@agente", NpgsqlTypes.NpgsqlDbType.Varchar, 20);
            cmd.Parameters.Add("@dia_pago", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
            cmd.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer);

            cmd.Parameters["@codigo"].Value = adminPaqCompany.Code;
            cmd.Parameters["@nombre_cliente"].Value = adminPaqCompany.Name;
            cmd.Parameters["@agente"].Value = adminPaqCompany.AgentCode;
            cmd.Parameters["@dia_pago"].Value = adminPaqCompany.PaymentDay;
            cmd.Parameters["@id"].Value = adminPaqCompany.Id;

            cmd.ExecuteNonQuery();
        }

        private void AddCompany(Company adminPaqCompany)
        {
            if (conn == null || conn.State != ConnectionState.Open)
                connect();

            string sqlString = "INSERT INTO cat_cliente (id_cliente, cd_cliente, nombre_cliente, ruta, dia_pago) " +
                "VALUES(@id, @codigo, @nombre_cliente,  @agente,  @dia_pago)";

            NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters.Add("@codigo", NpgsqlTypes.NpgsqlDbType.Varchar, 6);
            cmd.Parameters.Add("@nombre_cliente", NpgsqlTypes.NpgsqlDbType.Varchar, 150);
            cmd.Parameters.Add("@agente", NpgsqlTypes.NpgsqlDbType.Varchar, 20);
            cmd.Parameters.Add("@dia_pago", NpgsqlTypes.NpgsqlDbType.Varchar, 50);

            cmd.Parameters["@id"].Value = adminPaqCompany.Id;
            cmd.Parameters["@codigo"].Value = adminPaqCompany.Code;
            cmd.Parameters["@nombre_cliente"].Value = adminPaqCompany.Name;
            cmd.Parameters["@agente"].Value = adminPaqCompany.AgentCode;
            cmd.Parameters["@dia_pago"].Value = adminPaqCompany.PaymentDay;

            cmd.ExecuteNonQuery();
        }

        private void SaveAccount(Collectable.Account adminPaqAccount)
        {
            if (DocumentExists(adminPaqAccount.DocId))
                UpdateAccount(adminPaqAccount);
            else
                AddAccount(adminPaqAccount);
        }

        public void UpdateAccount(Collectable.Account adminPaqAccount)
        {
            if (conn == null || conn.State != ConnectionState.Open)
                connect();

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

            conn.Close();
        }

        private void AddAccount(Collectable.Account adminPaqAccount)
        {
            if (conn == null || conn.State != ConnectionState.Open)
                connect();

            string sqlString = "INSERT INTO ctrl_cuenta (ID_DOCO, F_DOCUMENTO, F_VENCIMIENTO, F_COBRO, ID_CLIENTE, SERIE_DOCO, FOLIO_DOCO, TIPO_DOCUMENTO, TIPO_COBRO, FACTURADO, " +
                "SALDO, MONEDA, OBSERVACIONES)" +
                "VALUES( @id, @f_documento, @f_vencimiento, @f_cobro, @id_cliente, @serie_doco, @folio_doco, @tipo_documento, @tipo_cobro, @facturado, @saldo, " +
                "@moneda, @observaciones);";

            NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer);
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

            cmd.Parameters["@id"].Value = adminPaqAccount.DocId;
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

            cmd.ExecuteNonQuery();
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
                    id_movimiento = 5;
                    break;
                case "Visita":
                    id_movimiento = 6;
                    break;
                case "Email":
                    id_movimiento = 7;
                    break;
                case "Cerrado":
                    id_movimiento = 9;
                    break;
                default:
                    id_movimiento = 8;
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
                    id_movimiento = 5;
                    break;
                case "Visita":
                    id_movimiento = 6;
                    break;
                case "Email":
                    id_movimiento = 7;
                    break;
                case "Cerrado":
                    id_movimiento = 9;
                    break;
                default:
                    id_movimiento = 8;
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

        public void Assign(int docId, int collectorId)
        {
            if (conn == null || conn.State != ConnectionState.Open)
                connect();

            string sqlString = "INSERT INTO ctrl_asignacion(id_cobrador, id_doco) "+
                "VALUES(@cobrador, @documento);";

            NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@cobrador", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters.Add("@documento", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters["@cobrador"].Value = collectorId;
            cmd.Parameters["@documento"].Value = docId;

            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public void Escale(int docId)
        {
            if (conn == null || conn.State != ConnectionState.Open)
                connect();

            string sqlString = "INSERT INTO ctrl_seguimiento(id_movimiento, id_doco, descripcion) " +
                "VALUES(4, @documento, 'Cuenta escalada a gerencia.');";

            NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@documento", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters["@documento"].Value = docId;

            cmd.ExecuteNonQuery();
            conn.Close();
        }

    }
}
