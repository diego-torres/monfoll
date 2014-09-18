using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Data.Odbc;
using ConsolaODBCFox.dto;
using System.Globalization;
using Npgsql;
using ConsolaODBCFox.ado;

namespace ConsolaODBCFox.FoxMiner
{
    public class Documentos
    {
        private const string DATE_FORMAT_PATTERN = "yyyyMMdd";
        public Empresa Empresa { get; set; }
        public EventLog Log { get; set; }
        public string MonfollConnectionString { get; set; }

        public void ExtraerCuentasPorCobrar()
        {
            foreach(string concepto in Empresa.ConceptosCredito)
            {
                try 
                {
                    ExtraerCuentasPorCobrar(IdConcepto(concepto));
                }catch(Exception ex)
                {
                    Log.WriteEntry("Error when trying to download accounts collectable: " + ex.Message + " || " + ex.StackTrace,
                        EventLogEntryType.Warning, 1, 3);
                }
            }
        }

        public void ActualizarCuentasPorCobrar()
        {
            try 
            {
                List<Cuenta> cuentas = PgCuentas.Cuentas(Empresa.Id);
                foreach (Cuenta cuenta in cuentas)
                {
                    Cuenta temp = CuentaFresca(cuenta.ApId);
                    if (temp == null)
                    {
                        PgCuentas.BorrarCuenta(cuenta.Id);
                    }
                    else
                    {
                        if ("".Equals(temp.Observaciones) && "".Equals(temp.TipoCobro))
                        {
                            // Actualizar Simple
                            if (cuenta.Saldo != temp.Saldo)
                                PgCuentas.ActualizarSimple(temp);
                        }
                        else
                        {
                            // Actualizar Full
                            PgCuentas.ActualizarCompleto(temp);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Log.WriteEntry("Error when trying to download accounts collectable: " + ex.Message + " || " + ex.StackTrace,
                        EventLogEntryType.Warning, 1, 3);
            }
            
        }

        public void CalcularVencidos()
        {
            throw new NotImplementedException("Not implemented yet");
        }

        public void CalcularPorVencer()
        {
            throw new NotImplementedException("Not implemented yet");
        }

        public void CalcularCobrado()
        {
            throw new NotImplementedException("Not implemented yet");
        }

        public void CalcularVentas()
        {
            try 
            {
                List<FactVenta> ventas = VentasMensuales(ConceptosVentaIds());
                List<FactVenta> devoluciones = VentasMensuales(ConceptosDevolucionIds());
                CalcularVentas(ventas, devoluciones);
                Log.WriteEntry("Sales calculation succesfully completed",
                        EventLogEntryType.Information, 1, 3);
            }
            catch (Exception ex) 
            {
                Log.WriteEntry("Error when trying to download sales information: " + ex.Message + " || " + ex.StackTrace,
                        EventLogEntryType.Warning, 1, 3);
            }
        }

        private void CalcularVentas(List<FactVenta> factVentas, List<FactVenta> factDevoluciones)
        {
            Dictionary<int, Venta> ventas = new Dictionary<int, Venta>();
            int weekStartDelta = 1 - (int)DateTime.Today.DayOfWeek;
            DateTime weekStart = DateTime.Today.AddDays(weekStartDelta);
            
            foreach (FactVenta factVenta in factVentas)
            {
                Venta venta = null;
                if (!ventas.ContainsKey(factVenta.IdVendedor))
                {
                    venta = new Venta();
                    venta.Vendedor = new Vendedor();
                    venta.Vendedor.ApId = factVenta.IdVendedor;
                    ventas.Add(factVenta.IdVendedor, venta);
                }

                venta = ventas[factVenta.IdVendedor];

                if (factVenta.IdMoneda != 1)
                {
                    factVenta.Importe *= factVenta.TipoCambio;
                }
                
                venta.Mensual += factVenta.Importe;

                if (factVenta.Fecha.CompareTo(weekStart) >= 0)
                    venta.Semanal += factVenta.Importe;

                if (factVenta.Fecha.CompareTo(DateTime.Today) == 0)
                    venta.Diaria += factVenta.Importe;
            }

            foreach (FactVenta factDevolucion in factDevoluciones)
            {
                Venta venta = null;
                if (!ventas.ContainsKey(factDevolucion.IdVendedor))
                {
                    venta = new Venta();
                    venta.Vendedor = new Vendedor();
                    venta.Vendedor.ApId = factDevolucion.IdVendedor;
                    ventas.Add(factDevolucion.IdVendedor, venta);
                }

                venta = ventas[factDevolucion.IdVendedor];

                if (factDevolucion.IdMoneda != 1)
                {
                    factDevolucion.Importe *= factDevolucion.TipoCambio;
                }

                venta.Mensual -= factDevolucion.Importe;

                if (factDevolucion.Fecha.CompareTo(weekStart) >= 0)
                    venta.Semanal -= factDevolucion.Importe;

                if (factDevolucion.Fecha.CompareTo(DateTime.Today) == 0)
                    venta.Diaria -= factDevolucion.Importe;
            }
            ResolverVendedor(ventas);
        }

        private void ResolverVendedor(Dictionary<int, Venta> ventas)
        { 
            // Resolver en postgres.
            foreach (int apId in ventas.Keys)
            {
                int pgId = PgFactVentas.IdVendedor(apId, Empresa.Id);
                if (pgId == 0)
                {
                    // Resolver de AdminPaq
                    Vendedor vendedor = VendedorAdminPaq(apId);
                    vendedor.Empresa = Empresa.Nombre;
                    vendedor.EmpresaId = Empresa.Id;
                    // Agregar a Postgres
                    PgFactVentas.AgregarVendedor(vendedor);
                    // Volver a consultar
                    pgId = PgFactVentas.IdVendedor(apId, Empresa.Id);
                    if (pgId == 0)
                    {
                        Log.WriteEntry("Unable to add seller to postgres database.", EventLogEntryType.Warning, 2, 3);
                        continue;
                    }

                    Venta current = ventas[apId];
                    current.Vendedor.Id = pgId;

                    PgFactVentas.AgregarVentas(current);

                }
                else
                {
                    Venta current = ventas[apId];
                    current.Vendedor.Id = pgId;

                    PgFactVentas.ActualizarVentas(current);
                }
            }
        }

        private Vendedor VendedorAdminPaq(int idVendedor)
        {
            string sqlString = "SELECT CCODIGOA01, CNOMBREA01 " +
                "FROM MGW10001 " +
                "WHERE CIDAGENTE = " + idVendedor;
            Vendedor result = null;
            try
            {
                string connString = "Driver={Microsoft Visual FoxPro Driver};SourceType=DBF;Exclusive=No;" +
                @"SourceDB=" + Empresa.Ruta + ";";
                using (OdbcConnection conn = new OdbcConnection(connString))
                {
                    conn.Open();

                    OdbcDataReader dr;
                    OdbcCommand cmd;

                    cmd = new OdbcCommand(sqlString, conn);
                    dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        result = new Vendedor();
                        result.ApId = idVendedor;
                        result.Codigo = dr["CCODIGOA01"].ToString();
                        result.Nombre = dr["CNOMBREA01"].ToString();
                    }

                    dr.Close();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Log.WriteEntry("EXCEPTION WHILE USING DATABASE: " + ex.Message + " || " + ex.StackTrace, EventLogEntryType.Warning, 2, 3);
            }

            return result;
        }

        private string ConceptosDevolucionIds()
        {
            string arrDevolucion = "-1,";
            foreach (string concepto in Empresa.ConceptosDevolucion)
            {
                arrDevolucion += IdConcepto(concepto).ToString() + ",";
            }
            arrDevolucion += "-2";
            return arrDevolucion;
        }

        private string ConceptosVentaIds()
        {
            string arrVenta = "-1,";
            foreach (string concepto in Empresa.ConceptosVenta)
            {
                arrVenta += IdConcepto(concepto).ToString() + ",";
            }
            arrVenta += "-2";
            return arrVenta;
        }

        private List<FactVenta> VentasMensuales(string arrVenta)
        {
            List<FactVenta> result = new List<FactVenta>();
            int monthDeltaDays = (DateTime.Today.Day - 1) * -1;
            DateTime BOM = DateTime.Today.AddDays(monthDeltaDays);

            string sqlString = "SELECT " +
                "SUM(CTOTAL) AS SUM_TOTAL, " +
                "CIDAGENTE, " +
                "CIDMONEDA, " +
                "CTIPOCAM01, " +
                "CFECHA " +
                "FROM MGW10008 " +
                "WHERE CCANCELADO = 0 " +
                "AND CIDCONCE01 IN (" + arrVenta + ") " +
                "AND CIMPRESO = 1 " +
                "AND CFECHA >= Date(" + BOM.Year + "," + BOM.Month + "," + BOM.Day + ") " +
                "GROUP BY CIDAGENTE, CIDMONEDA, CTIPOCAM01, CFECHA";

            string connString = "Driver={Microsoft Visual FoxPro Driver};SourceType=DBF;Exclusive=No;" +
                @"SourceDB=" + Empresa.Ruta + ";";
            using (OdbcConnection conn = new OdbcConnection(connString))
            {
                conn.Open();

                OdbcDataReader dr;
                OdbcCommand cmd;

                cmd = new OdbcCommand(sqlString, conn);
                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    FactVenta venta = new FactVenta();
                    venta.Importe = double.Parse(dr["SUM_TOTAL"].ToString());
                    venta.IdVendedor = int.Parse(dr["CIDAGENTE"].ToString());
                    venta.IdMoneda = int.Parse(dr["CIDMONEDA"].ToString());
                    venta.TipoCambio = double.Parse(dr["CTIPOCAM01"].ToString());
                    venta.Fecha = DateTime.Parse(dr["CFECHA"].ToString());

                    result.Add(venta);
                }
                dr.Close();
                conn.Close();
            }
            return result;
        }

        private Cuenta CuentaFresca(int apId)
        {
            Cuenta cuenta = null;

            string sqlString = "SELECT " +
                "CPENDIENTE, " +
                "CCANCELADO, " +
                "CFECHAEX01, " +
                "CTEXTOEX01, " +
                "CTEXTOEX02 " +
                "FROM MGW10008 " +
                "WHERE CIDDOCUM01 = " + apId;


            string connString = "Driver={Microsoft Visual FoxPro Driver};SourceType=DBF;Exclusive=No;" +
                @"SourceDB=" + Empresa.Ruta + ";";
            using (OdbcConnection conn = new OdbcConnection(connString))
            {
                conn.Open();

                OdbcDataReader dr;
                OdbcCommand cmd;

                cmd = new OdbcCommand(sqlString, conn);
                dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    int cancelado = int.Parse(dr["CCANCELADO"].ToString());

                    if (cancelado == 1)
                    {
                        dr.Close();
                        conn.Close();
                        return null;
                    }

                    double saldo = double.Parse(dr["CPENDIENTE"].ToString());

                    if (saldo <= 0)
                    {
                        dr.Close();
                        conn.Close();
                        return null;
                    }

                    cuenta = new Cuenta();
                    cuenta.ApId = apId;
                    cuenta.Saldo = saldo;

                    cuenta.TipoCobro = dr["CTEXTOEX01"].ToString();
                    cuenta.Observaciones = dr["CTEXTOEX02"].ToString();

                    string sFechaCobro = dr["CFECHAEX01"].ToString();
                    DateTime tempDate = DateTime.Today;
                    bool parsed = false;

                    parsed = DateTime.TryParse(sFechaCobro, out tempDate);
                    if (parsed)
                        cuenta.FechaCobro = tempDate;

                    // Retrieve payments
                    try
                    {
                        cuenta.Abonos = AbonosDocumento(cuenta.ApId, conn);
                    }
                    catch (Exception ex)
                    {
                        cuenta.Abonos = new List<Abono>();
                        Log.WriteEntry("Unavailable payments information for documentId: " +
                                cuenta.ApId + "; presented exception: "
                                + ex.Message + " || " + ex.StackTrace, EventLogEntryType.Warning, 6, 3);
                    }
                }
                dr.Close();
                conn.Close();
            }

            return cuenta;
        }

        private void ExtraerCuentasPorCobrar(int conceptoCredito)
        {
            if (conceptoCredito == 0)
            {
                throw new Exception("Credit concept not found in database.");
            }

            string sqlString = "SELECT " +
                "CIDDOCUM01, " +
                "CFOLIO, " +
                "CFECHA, " +
                "CIDCLIEN01, " +
                "CFECHAVE01, " +
                "CIDMONEDA, " +
                "CTIPOCAM01, " +
                "CTOTAL, " +
                "CPENDIENTE " +
                "FROM MGW10008 " +
                "WHERE CIDCONCE01 = " + conceptoCredito + " " +
                "AND CIDDOCUM02 = 4 " +
                "AND CCANCELADO = 0 " +
                "AND CPENDIENTE > 0";

            try
            {
                string connString = "Driver={Microsoft Visual FoxPro Driver};SourceType=DBF;Exclusive=No;" +
                @"SourceDB=" + Empresa.Ruta + ";";
                using (OdbcConnection conn = new OdbcConnection(connString))
                {
                    conn.Open();

                    OdbcDataReader dr;
                    OdbcCommand cmd;
                    Dictionary<int, Cliente> Clientes = new Dictionary<int, Cliente>();
                    Dictionary<int, string> Monedas = new Dictionary<int, string>();
                    int counter = 0;

                    cmd = new OdbcCommand(sqlString, conn);
                    dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        Cuenta cuenta = new Cuenta();

                        cuenta.ApId = int.Parse(dr["CIDDOCUM01"].ToString());
                        if (PgCuentas.CuentaExiste(cuenta, Empresa.Id)) continue;

                        cuenta.Folio = int.Parse(dr["CFOLIO"].ToString());
                        string sFechaDoc = dr["CFECHA"].ToString();
                        string sFechaVencimiento = dr["CFECHAVE01"].ToString();

                        DateTime tempDate = DateTime.Today;
                        bool parsed = false;

                        parsed = DateTime.TryParse(sFechaDoc, out tempDate);
                        if (parsed)
                            cuenta.FechaDoc = tempDate;

                        parsed = DateTime.TryParse(sFechaVencimiento, out tempDate);
                        if (parsed)
                            cuenta.FechaVencimiento = tempDate;


                        int idCliente = int.Parse(dr["CIDCLIEN01"].ToString());
                        if (!Clientes.ContainsKey(idCliente))
                        {
                            try
                            {
                                Cliente cliente = GetCliente(idCliente, conn);
                                if (cliente == null)
                                {
                                    Log.WriteEntry("Unavailable client information for clientId (Not found in AdminPaq): " +
                                    idCliente, EventLogEntryType.Warning);
                                    continue;
                                }

                                Clientes.Add(idCliente, cliente);
                            }
                            catch (Exception ex)
                            {
                                Log.WriteEntry("Unavailable client information for clientId: " + 
                                    idCliente + "; presented exception: " 
                                    + ex.Message + " || " + ex.StackTrace, EventLogEntryType.Warning, 3, 3);
                                continue;
                            }

                        }   

                        cuenta.Cliente = Clientes[idCliente];
                        int iMoneda = int.Parse(dr["CIDMONEDA"].ToString());
                        
                        double dTotal = double.Parse(dr["CTOTAL"].ToString());
                        double dSaldo = double.Parse(dr["CPENDIENTE"].ToString());

                        if (!Monedas.ContainsKey(iMoneda))
                        {
                            try
                            {
                                string sMoneda = NombreMoneda(iMoneda, conn);
                                if (sMoneda == null)
                                {
                                    Log.WriteEntry("Unavailable currency information for currencyId (Not found in AdminPaq): " +
                                    iMoneda, EventLogEntryType.Warning, 4, 3);
                                    continue;
                                }
                                Monedas.Add(iMoneda, sMoneda);
                            }
                            catch (Exception ex)
                            {
                                Log.WriteEntry("Unavailable currency information for currencyId: " +
                                    iMoneda + "; presented exception: "
                                    + ex.Message + " || " + ex.StackTrace, EventLogEntryType.Warning, 5, 3);
                                continue;
                            }
                        }

                        cuenta.Moneda = Monedas[iMoneda];
                        //cuenta.Total = double.Parse(dr["CTIPOCAM01"].ToString());
                        cuenta.Total = double.Parse(dr["CTOTAL"].ToString());
                        cuenta.Saldo = double.Parse(dr["CPENDIENTE"].ToString());

                        // Retrieve payments
                        try
                        {
                            cuenta.Abonos = AbonosDocumento(cuenta.ApId, conn);
                        }
                        catch (Exception ex)
                        {
                            cuenta.Abonos = new List<Abono>();
                            Log.WriteEntry("Unavailable payments information for documentId: " +
                                    cuenta.ApId + "; presented exception: "
                                    + ex.Message + " || " + ex.StackTrace, EventLogEntryType.Warning, 6, 3);
                        }
                        

                        // Save in postgres database

                        if (!PgCuentas.CuentaExiste(cuenta, Empresa.Id))
                        {
                            try 
                            {
                                PgCuentas.AgregarCuenta(cuenta, Empresa.Id);
                                counter++;
                            }
                            catch (Exception dbEx)
                            {
                                Log.WriteEntry("Unable to save account: " + dbEx.Message + " || " + dbEx.StackTrace, EventLogEntryType.Warning);
                            }
                        }
                    }
                    Log.WriteEntry("accounts found by fox library and added to postgres database: " + counter + " for concept " + conceptoCredito, EventLogEntryType.Information, 7, 3);
                    dr.Close();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Log.WriteEntry("EXCEPTION WHILE USING DATABASE: " + ex.Message + " || " + ex.StackTrace, EventLogEntryType.Warning, 8, 3);
            }
        }

        private string CodigoConcepto(int idConcepto, OdbcConnection conn)
        {
            string sqlString = "SELECT CCODIGOC01 " +
                "FROM MGW10006 " +
                "WHERE CIDCONCE01 = " + idConcepto;
            string result = "NOTFOUND";
            try
            {

                OdbcDataReader dr;
                OdbcCommand cmd;

                cmd = new OdbcCommand(sqlString, conn);
                dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    result = dr["CCODIGOC01"].ToString();
                }

                dr.Close();
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                Log.WriteEntry("EXCEPTION WHILE USING DATABASE: " + ex.Message + " || " + ex.StackTrace, EventLogEntryType.Warning, 2, 3);
            }

            return result;
        }

        private int IdConcepto(string codigoConcepto)
        {
            int result = 0;

            string sqlString = "SELECT CIDCONCE01 " +
                "FROM MGW10006 " +
                "WHERE CCODIGOC01 = '" + codigoConcepto + "'";

            try
            {
                string connString = "Driver={Microsoft Visual FoxPro Driver};SourceType=DBF;Exclusive=No;" +
                @"SourceDB=" + Empresa.Ruta + ";";
                using (OdbcConnection conn = new OdbcConnection(connString))
                {
                    conn.Open();

                    OdbcDataReader dr;
                    OdbcCommand cmd;

                    cmd = new OdbcCommand(sqlString, conn);
                    dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        result = int.Parse(dr["CIDCONCE01"].ToString());
                    }

                    dr.Close();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Log.WriteEntry("EXCEPTION WHILE USING DATABASE: " + ex.Message + " || " + ex.StackTrace, EventLogEntryType.Warning, 2, 3);
            }

            return result;
        }

        private List<Abono> AbonosDocumento(int idDocumento, OdbcConnection conn)
        {
            List<Abono> result = new List<Abono>();

            string sqlString = "SELECT " +
                "CIDDOCUM01, " +
                "CIMPORTE01 " +
                "FROM MGW10009 " +
                "WHERE CIDDOCUM02 = " + idDocumento;

            OdbcDataReader dr;
            OdbcCommand cmd = new OdbcCommand(sqlString, conn);
            dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                Abono abono = DocumentoAbono(conn, int.Parse(dr["CIDDOCUM01"].ToString()), double.Parse(dr["CIMPORTE01"].ToString()));
                if (abono != null)
                {
                    abono.IdCuenta = idDocumento;
                    result.Add(abono);
                }
                    
            }

            dr.Close();
            cmd.Dispose();

            return result;
        }

        private Abono DocumentoAbono(OdbcConnection conn, int id, double monto)
        {
            Abono result = new Abono();
            result.Id = id;
            result.Monto = monto;

            string sqlString = "SELECT " +
                "CIDCONCE01, " +
                "CREFEREN01, " +
                "CFOLIO, " +
                "CFECHA, " +
                "CTEXTOEX01 " +
                "FROM MGW10008 " +
                "WHERE CIDDOCUM01 = " + id;

            OdbcDataReader dr;
            OdbcCommand cmd = new OdbcCommand(sqlString, conn);
            dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                string codigoConcepto = CodigoConcepto(int.Parse(dr["CIDCONCE01"].ToString()), conn);
                if (!Empresa.ConceptosAbono.Contains(codigoConcepto)) 
                {
                    dr.Close();
                    cmd.Dispose();
                    return null;
                } 

                result.TipoPago = dr["CREFEREN01"].ToString();
                result.Folio = int.Parse(dr["CFOLIO"].ToString());

                DateTime fDeposito = DateTime.Today;
                bool parsed = false;

                parsed = DateTime.TryParse(dr["CFECHA"].ToString(), out fDeposito);
                if (parsed)
                    result.FechaDeposito = fDeposito;

                result.Cuenta = dr["CTEXTOEX01"].ToString();
            }
            else
            {   
                result = null;
            }

            dr.Close();
            cmd.Dispose();


            return result;
        }

        private Cliente GetCliente(int idCliente, OdbcConnection conn)
        {
            Cliente cliente = new Cliente();
            cliente.ApId = idCliente;
            cliente.IdEmpresa = Empresa.Id;
            cliente.RutaEmpresa = Empresa.Ruta;

            using (NpgsqlConnection pgConnection = new NpgsqlConnection(MonfollConnectionString))
            {
                pgConnection.Open();
                NpgsqlDataReader dr;
                NpgsqlCommand cmd;

                string sqlString = "SELECT id_cliente " +
                    "FROM cat_cliente " +
                    "WHERE ap_id = @idCliente AND id_empresa = @idEmpresa;";

                cmd = new NpgsqlCommand(sqlString, pgConnection);
                cmd.Parameters.Add("@idCliente", NpgsqlTypes.NpgsqlDbType.Integer);
                cmd.Parameters.Add("@idEmpresa", NpgsqlTypes.NpgsqlDbType.Integer);

                cmd.Parameters["@idCliente"].Value = idCliente;
                cmd.Parameters["@idEmpresa"].Value = Empresa.Id;

                dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    // Cliente listo en base de datos; asignar identificador para relacion
                    cliente.Id = int.Parse( dr["id_cliente"].ToString() );
                }
                else
                { 
                    // Cliente no existe en la base de datos, obtener datos de AdminPaq
                    // para grabar posteriormente en postgres.
                    cliente = ClienteFromAdminPaq(idCliente, conn);
                }

                dr.Close();
                cmd.Dispose();
                pgConnection.Close();
            }

            return cliente;
        }

        private Cliente ClienteFromAdminPaq(int idCliente, OdbcConnection conn)
        {
            Cliente cliente = null;

            string sqlString = "SELECT CCODIGOC01, CRAZONSO01, CIDAGENT02, CTEXTOEX05, CIDVALOR01 " +
                "FROM MGW10002 " +
                "WHERE CIDCLIEN01 = " + idCliente;

            OdbcDataReader dr;
            OdbcCommand cmd = new OdbcCommand(sqlString, conn);
            dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                cliente = new Cliente();
                cliente.Id = 0;
                cliente.ApId = idCliente;
                cliente.IdEmpresa = Empresa.Id;
                cliente.RutaEmpresa = Empresa.Ruta;
                cliente.Codigo = dr["CCODIGOC01"].ToString();
                cliente.RazonSocial = dr["CRAZONSO01"].ToString();
                cliente.Ruta = dr["CIDAGENT02"].ToString();
                cliente.DiaPago = dr["CTEXTOEX05"].ToString();
                int ubicacion = int.Parse(dr["CIDVALOR01"].ToString());
                cliente.EsLocal = ubicacion == 1;
            }

            dr.Close();
            cmd.Dispose();
            return cliente;
        }

        private string NombreMoneda(int idMoneda, OdbcConnection conn)
        {
            string result = null;

            string sqlString = "SELECT CNOMBREM01 " +
                "FROM MGW10034 " +
                "WHERE CIDMONEDA = " + idMoneda;

            OdbcDataReader dr;
            OdbcCommand cmd = new OdbcCommand(sqlString, conn);
            dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                result = dr["CNOMBREM01"].ToString();
            }

            dr.Close();
            cmd.Dispose();

            return result;
        }

    }
}
