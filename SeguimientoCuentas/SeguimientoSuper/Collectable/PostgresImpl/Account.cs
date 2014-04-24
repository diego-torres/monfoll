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
        public void UploadAccounts(List<Collectable.Account> adminPaqAccounts)
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
            conn.Close();
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
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            NpgsqlDataAdapter da;
            string sqlString = "SELECT id_doco FROM ctrl_cuenta WHERE id_doco = " + adminPaqAccount.DocId.ToString() + ";";
            da = new NpgsqlDataAdapter(sqlString, conn);

            ds.Reset();
            da.Fill(ds);
            dt = ds.Tables[0];

            if (dt.Rows.Count >= 1)
                UpdateAccount(adminPaqAccount);
            else
                AddAccount(adminPaqAccount);
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
        }

        private void AddAccount(Collectable.Account adminPaqAccount)
        {
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

    }
}
