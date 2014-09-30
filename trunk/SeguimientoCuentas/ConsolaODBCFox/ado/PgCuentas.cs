using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsolaODBCFox.dto;
using System.Configuration;
using Npgsql;

namespace ConsolaODBCFox.ado
{
    public class PgCuentas
    {
        public static bool CuentaExiste(Cuenta cuenta, int empresaId)
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

            cmd.Parameters["@apId"].Value = cuenta.ApId;
            cmd.Parameters["@eId"].Value = empresaId;

            dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                records = int.Parse(dr[0].ToString());
            }

            dr.Close();
            conn.Close();

            return records > 0;
        }

        public static void AgregarCuenta(Cuenta cuenta, int idEmpresa)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[Config.Common.MONFOLL].ConnectionString;

            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {

                conn.Open();

                string sqlString = "INSERT INTO ctrl_cuenta (ap_id, enterprise_id, F_DOCUMENTO, F_VENCIMIENTO, F_COBRO, ID_CLIENTE, SERIE_DOCO, FOLIO_DOCO, TIPO_DOCUMENTO, TIPO_COBRO, FACTURADO, " +
                    "SALDO, MONEDA, OBSERVACIONES, f_cobro_esperada) " +
                    "VALUES( @id, @enterprise, @f_documento, @f_vencimiento, @f_cobro, @id_cliente, @serie_doco, @folio_doco, @tipo_documento, @tipo_cobro, @facturado, @saldo, " +
                    "@moneda, @observaciones, @esperada);";

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
                cmd.Parameters.Add("@esperada", NpgsqlTypes.NpgsqlDbType.Date);

                cmd.Parameters["@id"].Value = cuenta.ApId;
                cmd.Parameters["@enterprise"].Value = idEmpresa;
                cmd.Parameters["@f_documento"].Value = cuenta.FechaDoc;
                cmd.Parameters["@f_vencimiento"].Value = cuenta.FechaVencimiento;
                cmd.Parameters["@f_cobro"].Value = cuenta.FechaCobro;

                int coId = cuenta.Cliente.Id;
                if (coId == 0)
                {
                    PgClientes.AgregarCliente(conn, cuenta.Cliente);
                    coId = PgClientes.IdCliente(conn, cuenta.Cliente.ApId, cuenta.Cliente.IdEmpresa);
                    if (coId == 0)
                    {
                        conn.Close();
                        throw new Exception("Unable to save account, could not store the client information in postgres.");
                    }
                }

                cmd.Parameters["@id_cliente"].Value = coId;
                cmd.Parameters["@serie_doco"].Value = cuenta.Serie;
                cmd.Parameters["@folio_doco"].Value = cuenta.Folio;
                cmd.Parameters["@tipo_documento"].Value = cuenta.TipoDocumento;
                cmd.Parameters["@tipo_cobro"].Value = cuenta.TipoCobro;
                cmd.Parameters["@facturado"].Value = cuenta.Total;
                cmd.Parameters["@saldo"].Value = cuenta.Saldo;
                cmd.Parameters["@moneda"].Value = cuenta.Moneda;
                cmd.Parameters["@observaciones"].Value = cuenta.Observaciones;

                if(!cuenta.FechaEsperadaCobro.Equals(new DateTime(1899, 12, 30)))
                    cmd.Parameters["@esperada"].Value = cuenta.FechaEsperadaCobro;

                cmd.ExecuteNonQuery();

                GrabarAbonos(conn, cuenta);

                conn.Close();
            }
            
        }

        public static void BorrarCuenta(int accountId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[Config.Common.MONFOLL].ConnectionString;

            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {

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

        public static void ArreglarCuentas(List<Cuenta> cuentas)
        {

            string connectionString = ConfigurationManager.ConnectionStrings[Config.Common.MONFOLL].ConnectionString;

            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();


                foreach (Cuenta cuenta in cuentas)
                {
                    DateTime esperada = cuenta.FechaEsperadaCobro;
                    if (esperada.Equals(new DateTime(1899, 12, 30)))
                        continue;

                    string sqlString = "UPDATE ctrl_cuenta " +
                    "SET f_cobro_esperada = @fecha " +
                    "WHERE id_doco = @id";

                    NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);

                    cmd.Parameters.Add("@fecha", NpgsqlTypes.NpgsqlDbType.Date);
                    cmd.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer);

                    cmd.Parameters["@fecha"].Value = esperada;
                    cmd.Parameters["@id"].Value = cuenta.Id;

                    cmd.ExecuteNonQuery();
                }

                conn.Close();
            }
            
        }

        public static List<Cuenta> FixCuentas(int idEmpresa)
        {
            List<Cuenta> result = new List<Cuenta>();
            string connectionString = ConfigurationManager.ConnectionStrings[Config.Common.MONFOLL].ConnectionString;
            // Para cada ID Doco en la BD de postgres
            using (NpgsqlConnection pgConnection = new NpgsqlConnection(connectionString))
            {
                pgConnection.Open();
                NpgsqlDataReader dr;
                NpgsqlCommand cmd;

                string sqlString = "SELECT id_doco, f_documento, f_vencimiento, dia_pago  " +
                    "FROM ctrl_cuenta " +
                    "INNER JOIN cat_cliente ON cat_cliente.id_cliente = ctrl_cuenta.id_cliente " +
                    "WHERE enterprise_id = @idEmpresa;";

                cmd = new NpgsqlCommand(sqlString, pgConnection);
                cmd.Parameters.Add("@idEmpresa", NpgsqlTypes.NpgsqlDbType.Integer);
                cmd.Parameters["@idEmpresa"].Value = idEmpresa;

                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Cuenta cuenta = new Cuenta();
                    cuenta.Id = int.Parse(dr["id_doco"].ToString());
                    
                    bool parsed = false;
                    DateTime tempDate = DateTime.Today;
                    parsed = DateTime.TryParse(dr["f_documento"].ToString(), out tempDate);
                    if (parsed)
                        cuenta.FechaDoc = tempDate;

                    parsed = DateTime.TryParse(dr["f_vencimiento"].ToString(), out tempDate);
                    if (parsed)
                        cuenta.FechaVencimiento = tempDate;


                    Cliente cliente = new Cliente();
                    cliente.DiaPago = dr["dia_pago"].ToString();
                    cuenta.Cliente = cliente;

                    result.Add(cuenta);
                }

                dr.Close();
                cmd.Dispose();
                pgConnection.Close();
            }
            return result;
        }

        public static List<Cuenta> Cuentas(int idEmpresa)
        {
            List<Cuenta> result = new List<Cuenta>();
            string connectionString = ConfigurationManager.ConnectionStrings[Config.Common.MONFOLL].ConnectionString;
            // Para cada ID Doco en la BD de postgres
            using (NpgsqlConnection pgConnection = new NpgsqlConnection(connectionString))
            {
                pgConnection.Open();
                NpgsqlDataReader dr;
                NpgsqlCommand cmd;

                string sqlString = "SELECT id_doco, ap_id, saldo, f_cobro, observaciones, tipo_cobro " +
                    "FROM ctrl_cuenta " +
                    "WHERE enterprise_id = @idEmpresa;";

                cmd = new NpgsqlCommand(sqlString, pgConnection);
                cmd.Parameters.Add("@idEmpresa", NpgsqlTypes.NpgsqlDbType.Integer);
                cmd.Parameters["@idEmpresa"].Value = idEmpresa;

                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Cuenta cuenta = new Cuenta();
                    cuenta.Id = int.Parse(dr["id_doco"].ToString());
                    cuenta.ApId = int.Parse(dr["ap_id"].ToString());
                    cuenta.Saldo = double.Parse(dr["saldo"].ToString());

                    bool parsed = false;
                    DateTime fCobro = DateTime.Today;
                    parsed = DateTime.TryParse(dr["f_cobro"].ToString(), out fCobro);
                    if(parsed)
                        cuenta.FechaCobro = DateTime.Parse(dr["f_cobro"].ToString());
                        
                    
                    cuenta.Observaciones = dr["observaciones"].ToString();
                    cuenta.TipoCobro = dr["tipo_cobro"].ToString();
                    result.Add(cuenta);
                }

                dr.Close();
                cmd.Dispose();
                pgConnection.Close();
            }
            return result;
        }

        public static void ActualizarSimple(Cuenta cuenta)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[Config.Common.MONFOLL].ConnectionString;

            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                string sqlString = "UPDATE ctrl_cuenta " +
                    "SET SALDO = @saldo, " +
                    "TS_DESCARGADO = CURRENT_TIMESTAMP " +
                    "WHERE id_doco = @id";

                NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);

                cmd.Parameters.Add("@saldo", NpgsqlTypes.NpgsqlDbType.Money);
                cmd.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer);

                cmd.Parameters["@saldo"].Value = cuenta.Saldo;
                cmd.Parameters["@id"].Value = cuenta.Id;

                cmd.ExecuteNonQuery();

                GrabarAbonos(conn, cuenta);

                conn.Close();
            }
        }

        public static void ActualizarCompleto(Cuenta cuenta)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[Config.Common.MONFOLL].ConnectionString;

            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                string sqlString = "UPDATE ctrl_cuenta " +
                "SET SALDO = @saldo, " +
                "F_COBRO = @collect_date, " +
                "TIPO_COBRO = @collect_type, " +
                "OBSERVACIONES = @obs_note, " +
                "TS_DESCARGADO = CURRENT_TIMESTAMP " +
                "WHERE id_doco = @id";

                NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);

                cmd.Parameters.Add("@saldo", NpgsqlTypes.NpgsqlDbType.Money);
                cmd.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer);

                cmd.Parameters.Add("@collect_date", NpgsqlTypes.NpgsqlDbType.Date);
                cmd.Parameters.Add("@collect_type", NpgsqlTypes.NpgsqlDbType.Varchar, 100);
                cmd.Parameters.Add("@obs_note", NpgsqlTypes.NpgsqlDbType.Varchar, 250);

                cmd.Parameters["@saldo"].Value = cuenta.Saldo;
                cmd.Parameters["@id"].Value = cuenta.Id;

                cmd.Parameters["@collect_date"].Value = cuenta.FechaCobro;
                cmd.Parameters["@collect_type"].Value = cuenta.TipoCobro.Trim();
                cmd.Parameters["@obs_note"].Value = cuenta.Observaciones.Trim();

                cmd.ExecuteNonQuery();

                GrabarAbonos(conn, cuenta);

                conn.Close();
            }
        }

        private static void GrabarAbonos(NpgsqlConnection conn, Cuenta cuenta)
        {
            if (cuenta.Abonos == null) return;

            foreach(Abono abono in cuenta.Abonos)
            {
                GrabarAbono(conn, abono);
            }
        }

        private static void GrabarAbono(NpgsqlConnection conn, Abono abono)
        {
            string sqlString = "SELECT id_abono " +
                "FROM ctrl_abono " +
                "WHERE id_abono = @pId;";

            NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);
            cmd.Parameters.Add("@pId", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters["@pId"].Value = abono.Id;

            NpgsqlDataReader dr = cmd.ExecuteReader();
            bool exists = dr.Read();
            dr.Close();

            if (exists)
                ActualizarAbono(conn, abono);
            else
                AgregarAbono(conn, abono);
        }

        private static void ActualizarAbono(NpgsqlConnection conn, Abono abono)
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

            cmd.Parameters["@id_doc"].Value = abono.IdCuenta;
            cmd.Parameters["@tipo_pago"].Value = abono.TipoPago;
            cmd.Parameters["@importe"].Value = abono.Monto;
            cmd.Parameters["@folio"].Value = abono.Folio;
            cmd.Parameters["@concepto"].Value = abono.Concepto;
            cmd.Parameters["@fecha_deposito"].Value = abono.FechaDeposito;
            cmd.Parameters["@cuenta"].Value = abono.Cuenta;
            cmd.Parameters["@id"].Value = abono.Id;

            cmd.ExecuteNonQuery();
        }

        private static void AgregarAbono(NpgsqlConnection conn, Abono abono)
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

            cmd.Parameters["@id_doc"].Value = abono.IdCuenta;
            cmd.Parameters["@tipo_pago"].Value = abono.TipoPago;
            cmd.Parameters["@importe"].Value = abono.Monto;
            cmd.Parameters["@folio"].Value = abono.Folio;
            cmd.Parameters["@concepto"].Value = abono.Concepto;
            cmd.Parameters["@fecha_deposito"].Value = abono.FechaDeposito;
            cmd.Parameters["@cuenta"].Value = abono.Cuenta;
            cmd.Parameters["@id"].Value = abono.Id;

            cmd.ExecuteNonQuery();
        }


    }
}
