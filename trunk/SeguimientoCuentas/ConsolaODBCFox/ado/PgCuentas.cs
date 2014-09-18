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

                cmd.Parameters["@id"].Value = cuenta.ApId;
                cmd.Parameters["@enterprise"].Value = idEmpresa;
                cmd.Parameters["@f_documento"].Value = cuenta.FechaDoc;
                cmd.Parameters["@f_vencimiento"].Value = cuenta.FechaVencimiento;
                cmd.Parameters["@f_cobro"].Value = cuenta.FechaCobro;

                int coId = cuenta.Cliente.Id;
                if (coId == 0)
                {
                    AgregarCliente(conn, cuenta.Cliente);
                    coId = IdCliente(conn, cuenta.Cliente.ApId, cuenta.Cliente.IdEmpresa);
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

                string sqlString = "SELECT id_doco, ap_id, saldo " +
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
                cmd.Parameters["@collect_type"].Value = cuenta.TipoCobro;
                cmd.Parameters["@obs_note"].Value = cuenta.Observaciones;

                cmd.ExecuteNonQuery();

                GrabarAbonos(conn, cuenta);

                conn.Close();
            }
        }

        private static void GrabarAbonos(NpgsqlConnection conn, Cuenta cuenta)
        { 
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

        private static void AgregarCliente(NpgsqlConnection conn, Cliente cliente)
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

            cmd.Parameters["@id"].Value = cliente.ApId;
            cmd.Parameters["@empresa"].Value = cliente.IdEmpresa;
            cmd.Parameters["@codigo"].Value = cliente.Codigo;
            cmd.Parameters["@nombre_cliente"].Value = cliente.RazonSocial;
            cmd.Parameters["@agente"].Value = cliente.Ruta;
            cmd.Parameters["@dia_pago"].Value = cliente.DiaPago;
            cmd.Parameters["@local"].Value = cliente.EsLocal;

            cmd.ExecuteNonQuery();
        }

        private static int IdCliente(NpgsqlConnection conn, int apId, int idEmpresa)
        {
            int result = 0;
            NpgsqlDataReader dr;
            NpgsqlCommand cmd;

            string sqlString = "SELECT id_cliente " +
                "FROM cat_cliente " +
                "WHERE ap_id = @idCliente AND id_empresa = @idEmpresa;";

            cmd = new NpgsqlCommand(sqlString, conn);
            cmd.Parameters.Add("@idCliente", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters.Add("@idEmpresa", NpgsqlTypes.NpgsqlDbType.Integer);

            cmd.Parameters["@idCliente"].Value = apId;
            cmd.Parameters["@idEmpresa"].Value = idEmpresa;

            dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                // Cliente listo en base de datos; asignar identificador para relacion
                result = int.Parse(dr["id_cliente"].ToString());
            }
            
            dr.Close();
            cmd.Dispose();
            
            return result;
        }

    }
}
