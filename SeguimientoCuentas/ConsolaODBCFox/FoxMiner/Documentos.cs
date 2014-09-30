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
using System.IO;

namespace ConsolaODBCFox.FoxMiner
{
    public class Documentos
    {
        public Empresa Empresa { get; set; }
        public EventLog Log { get; set; }
        public string MonfollConnectionString { get; set; }


        #region COBRANZA

        public void ExtraerCuentasPorCobrar()
        {
            foreach (string concepto in Empresa.ConceptosCredito)
            {
                try
                {
                    ExtraerCuentasPorCobrar(IdConcepto(concepto));
                }
                catch (Exception ex)
                {
                    Log.WriteEntry("Error when trying to download accounts collectable: " + ex.Message + " || " + ex.StackTrace,
                        EventLogEntryType.Warning, 1, 3);
                }
            }
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
                "CSERIEDO01, " +
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
                        cuenta.Serie = dr["CSERIEDO01"].ToString();
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

                string sqlString = "SELECT id_cliente, dia_pago " +
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
                    cliente.Id = int.Parse(dr["id_cliente"].ToString());
                    cliente.DiaPago = dr["dia_pago"].ToString();
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
        
        #endregion

        #region ACTUALIZAR

        public void ActualizarCuentasPorCobrar()
        {
            try
            {
                // Bring all accounts from postgres
                List<Cuenta> cuentas = PgCuentas.Cuentas(Empresa.Id);
                Log.WriteEntry("Updating " + cuentas.Count + " from AdminPaq.", EventLogEntryType.Information, 10,3);
                // Bring all accounts from AdminPaq
                List<Cuenta> apCuentas = ApCuentas();
                Log.WriteEntry("Found " + apCuentas.Count + " in AdminPaq.", EventLogEntryType.Information, 10, 3);

                foreach (Cuenta pgCuenta in cuentas)
                {
                    bool found = false;
                    int counter = 0;
                    foreach (Cuenta apCuenta in apCuentas)
                    {
                        counter++;
                        if (apCuenta.ApId == pgCuenta.ApId)
                        {
                            found = true;
                            if (apCuenta.Saldo != pgCuenta.Saldo
                                || !apCuenta.FechaCobro.Equals(pgCuenta.FechaCobro)
                                || !apCuenta.Observaciones.Equals(pgCuenta.Observaciones)
                                || !apCuenta.TipoCobro.Equals(pgCuenta.TipoCobro))
                            {
                                if (apCuenta.Saldo != pgCuenta.Saldo)
                                {
                                    pgCuenta.Abonos = AbonosDocumento(pgCuenta.ApId);
                                    pgCuenta.Saldo = apCuenta.Saldo;
                                }

                                if ("".Equals(apCuenta.Observaciones) && "".Equals(apCuenta.TipoCobro))
                                {
                                    PgCuentas.ActualizarSimple(pgCuenta);
                                }
                                else
                                {
                                    // Actualizar Full
                                    pgCuenta.Observaciones = apCuenta.Observaciones;
                                    pgCuenta.TipoCobro = apCuenta.TipoCobro;

                                    if(!apCuenta.FechaCobro.Equals(new DateTime(1899, 12, 30)))
                                        pgCuenta.FechaCobro = apCuenta.FechaCobro;

                                    PgCuentas.ActualizarCompleto(pgCuenta);
                                }

                            }
                            break;
                        }
                    }
                    if (!found)
                    {
                        Log.WriteEntry("Balanced account " + pgCuenta.ApId + "; not found after " + counter, EventLogEntryType.Information, 11, 3);
                        PgCuentas.BorrarCuenta(pgCuenta.Id);
                    }   
                }
            }
            catch (Exception ex)
            {
                Log.WriteEntry("Error when trying to download accounts collectable: " + ex.Message + " || " + ex.StackTrace,
                        EventLogEntryType.Warning, 1, 3);
            }

        }

        private List<Cuenta> ApCuentas()
        {
            List<Cuenta> cuentas = new List<Cuenta>();

            // Add IN.
            string sqlString = "SELECT " +
                "CIDDOCUM01, " +
                "CPENDIENTE, " +
                "CFECHAEX01, " +
                "CTEXTOEX01, " +
                "CTEXTOEX02 " +
                "FROM MGW10008 " +
                "WHERE CPENDIENTE > 0 " +
                "AND CCANCELADO = 0 " +
                "AND CIDCONCE01 IN (" + ConceptosCreditoIds() + ")";

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
                    Cuenta cuenta = new Cuenta();
                    cuenta.ApId = int.Parse(dr["CIDDOCUM01"].ToString());
                    cuenta.Saldo = double.Parse(dr["CPENDIENTE"].ToString());

                    cuenta.TipoCobro = dr["CTEXTOEX01"].ToString().Trim();
                    cuenta.Observaciones = dr["CTEXTOEX02"].ToString().Trim();

                    string sFechaCobro = dr["CFECHAEX01"].ToString();
                    DateTime tempDate = DateTime.Today;
                    bool parsed = false;

                    parsed = DateTime.TryParse(sFechaCobro, out tempDate);
                    if (parsed)
                        cuenta.FechaCobro = tempDate;

                    cuentas.Add(cuenta);
                }
                dr.Close();
                conn.Close();
            }

            return cuentas;
        }

        private String ConceptosCreditoIds()
        {
            string arrCredito = "-1,";
            foreach (string concepto in Empresa.ConceptosCredito)
            {
                arrCredito += IdConcepto(concepto).ToString() + ",";
            }
            arrCredito += "-2";
            return arrCredito;
        }


        #endregion

        #region VENTAS

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
                "AND CDEVUELTO = 0 " + 
                "AND CIDCONCE01 IN (" + arrVenta + ") " +
                "AND CAFECTADO = 1 " +
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

        private Vendedor VendedorAdminPaq(int idVendedor, OdbcConnection conn)
        {
            string sqlString = "SELECT CCODIGOA01, CNOMBREA01 " +
                "FROM MGW10001 " +
                "WHERE CIDAGENTE = " + idVendedor;
            Vendedor result = null;
            try
            {
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
            }
            catch (Exception ex)
            {
                Log.WriteEntry("EXCEPTION WHILE USING DATABASE: " + ex.Message + " || " + ex.StackTrace, EventLogEntryType.Warning, 2, 3);
            }

            return result;
        }

        private Vendedor VendedorAdminPaq(int idVendedor)
        {
            Vendedor result = null;
            try
            {
                string connString = "Driver={Microsoft Visual FoxPro Driver};SourceType=DBF;Exclusive=No;" +
                @"SourceDB=" + Empresa.Ruta + ";";
                using (OdbcConnection conn = new OdbcConnection(connString))
                {
                    conn.Open();

                    VendedorAdminPaq(idVendedor, conn);
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Log.WriteEntry("EXCEPTION WHILE USING DATABASE: " + ex.Message + " || " + ex.StackTrace, EventLogEntryType.Warning, 2, 3);
            }

            return result;
        }

        #endregion

        #region DETALLE

        public void CalcularDetalle()
        {
            try
            {
                Dictionary<int, string> dicVendedor = new Dictionary<int, string>();
                Dictionary<int, string> dicMoneda = new Dictionary<int, string>();
                Dictionary<int, string> dicProductos = new Dictionary<int, string>();

                List<DetalleVenta> ventas = Detalles(ConceptosVentaIds(), dicVendedor, dicMoneda, dicProductos);
                List<DetalleVenta> devoluciones = Detalles(ConceptosDevolucionIds(), dicVendedor, dicMoneda, dicProductos);
                Log.WriteEntry(ventas.Count + " ventas, " + devoluciones.Count + " devoluciones",
                        EventLogEntryType.Information, 2, 3);
                GrabarVentas(ventas, devoluciones);
                Log.WriteEntry("Sales detail calculation succesfully completed",
                        EventLogEntryType.Information, 3, 3);
            }
            catch (Exception ex)
            {
                Log.WriteEntry("Error when trying to download sales information: " + ex.Message + " || " + ex.StackTrace,
                        EventLogEntryType.Warning, 1, 3);
            }
        }

        private List<DetalleVenta> Detalles(string Conceptos, Dictionary<int, string> dicVendedor,
            Dictionary<int, string> dicMoneda, Dictionary<int, string> dicProductos)
        {
            List<DetalleVenta> result = new List<DetalleVenta>();
            DateTime BOM = DateTime.Today;

            string sqlString = "SELECT " +
                "CIDDOCUM01, " +
                "CSERIEDO01, " +
                "CIDCONCE01, " +
                "CFOLIO, " +
                "CTOTAL, " +
                "CIDAGENTE, " +
                "CIDMONEDA, " +
                "CTIPOCAM01, " +
                "CFECHA " +
                "FROM MGW10008 " +
                "WHERE CCANCELADO = 0 " +
                "AND CDEVUELTO = 0 " +
                "AND CIDCONCE01 IN (" + Conceptos + ") " +
                "AND CAFECTADO = 1 " +
                "AND CIMPRESO = 1 " +
                "AND CFECHA >= Date(" + BOM.Year + "," + BOM.Month + "," + BOM.Day + ")";

            string connString = "Driver={Microsoft Visual FoxPro Driver};SourceType=DBF;Exclusive=No;" +
                @"SourceDB=" + Empresa.Ruta + ";";
            using (OdbcConnection conn = new OdbcConnection(connString))
            {
                conn.Open();

                OdbcDataReader dr;
                OdbcCommand cmd;

                cmd = new OdbcCommand(sqlString, conn);
                dr = cmd.ExecuteReader();
                Log.WriteEntry("Executed reader for MGW1008",
                        EventLogEntryType.Information, 4, 3);

                while (dr.Read())
                {
                    DetalleVenta venta = new DetalleVenta();
                    venta.IdDoco = int.Parse(dr["CIDDOCUM01"].ToString());

                    int idVendedor = int.Parse(dr["CIDAGENTE"].ToString());
                    
                    if (!dicVendedor.ContainsKey(idVendedor))
                    {
                        Vendedor vendedor = VendedorAdminPaq(idVendedor, conn);
                        dicVendedor.Add(idVendedor, vendedor.Nombre.Trim());
                    }

                    venta.Vendedor = dicVendedor[idVendedor];
                    venta.Folio = int.Parse(dr["CFOLIO"].ToString());
                    venta.Serie = dr["CSERIEDO01"].ToString();
                    venta.Importe = double.Parse(dr["CTOTAL"].ToString());

                    int idMoneda = int.Parse(dr["CIDMONEDA"].ToString());
                    if (!dicMoneda.ContainsKey(idMoneda))
                    {
                        string moneda = NombreMoneda(idMoneda, conn);
                        dicMoneda.Add(idMoneda, moneda);
                    }

                    venta.Moneda = dicMoneda[idMoneda];
                    venta.IdConcepto = int.Parse(dr["CIDCONCE01"].ToString());
                    venta.TipoCambio = double.Parse(dr["CTIPOCAM01"].ToString());
                    venta.Fecha = DateTime.Parse(dr["CFECHA"].ToString());
                    venta.Movimientos = Movimientos(venta.IdDoco, conn, dicProductos);

                    result.Add(venta);
                }
                dr.Close();
                conn.Close();
            }
            return result;
        }

        private List<DetalleMovimiento> Movimientos(int idVenta, OdbcConnection conn, Dictionary<int, string> dicProductos)
        {
            List<DetalleMovimiento> result = new List<DetalleMovimiento>();

            string sqlString = "SELECT " +
                "CIDMOVIM01, " +
                "CIDPRODU01, " +
                "CTOTAL " +
                "FROM MGW10010 " +
                "WHERE CIDDOCUM01 = " + idVenta;

            OdbcDataReader dr;
            OdbcCommand cmd;

            cmd = new OdbcCommand(sqlString, conn);
            dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                DetalleMovimiento movimiento = new DetalleMovimiento();
                movimiento.IdMov = int.Parse(dr["CIDMOVIM01"].ToString());
                movimiento.Importe = double.Parse(dr["CTOTAL"].ToString());

                int idProducto = int.Parse(dr["CIDPRODU01"].ToString());
                if (!dicProductos.ContainsKey(idProducto))
                {
                    dicProductos.Add(idProducto, NombreProducto(idProducto, conn));
                }
                movimiento.Producto = dicProductos[idProducto];

                result.Add(movimiento);
            }
            dr.Close();

            return result;
        }

        private string NombreProducto(int IdProducto, OdbcConnection conn)
        {
            string sqlString = "SELECT CNOMBREP01 " +
                "FROM MGW10005 " +
                "WHERE CIDPRODU01 = " + IdProducto;
            string result = "NOTFOUND";
            try
            {

                OdbcDataReader dr;
                OdbcCommand cmd;

                cmd = new OdbcCommand(sqlString, conn);
                dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    result = dr["CNOMBREP01"].ToString();
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

        private void GrabarVentas(List<DetalleVenta> ventas, List<DetalleVenta> devoluciones)
        {
            // create a writer and open the file
            using(TextWriter tw = new StreamWriter(@"c:\temp\auditoria_" + Empresa.Id + "_" + DateTime.Now.ToString("MMddyy_HHmm") + ".txt"))
            {
                // write a line of text to the file
                tw.WriteLine("ID|VENDEDOR|FOLIO|SERIE|IMPORTE|MONEDA|CONCEPTO|TIPO CAMBIO|FECHA|MOV|IMPORTE MOV|PRODUCTO");
                foreach (DetalleVenta registro in devoluciones)
                {
                    registro.Importe *= -1;
                    if (registro.Movimientos.Count == 0)
                    {
                        tw.WriteLine(registro.IdDoco + "|" + registro.Vendedor + "|" + registro.Folio + "|" + registro.Serie +
                        "|" + registro.Importe + "|" + registro.Moneda + "|" + registro.IdConcepto + "|" + registro.TipoCambio +
                        "|" + registro.Fecha.ToShortDateString() + "|0|0|DEVOLUCION");
                    }
                    foreach (DetalleMovimiento movimiento in registro.Movimientos)
                    {
                        movimiento.Importe *= -1;
                        tw.WriteLine(registro.IdDoco + "|" + registro.Vendedor + "|" + registro.Folio + "|" + registro.Serie +
                        "|" + registro.Importe + "|" + registro.Moneda + "|" + registro.IdConcepto + "|" + registro.TipoCambio +
                        "|" + registro.Fecha.ToShortDateString() + "|" + movimiento.IdMov + "|" + movimiento.Importe + "|" + movimiento.Producto);
                    }
                }

                foreach (DetalleVenta registro in ventas)
                {
                    if (registro.Movimientos.Count == 0)
                    {
                        tw.WriteLine(registro.IdDoco + "|" + registro.Vendedor + "|" + registro.Folio + "|" + registro.Serie +
                        "|" + registro.Importe + "|" + registro.Moneda + "|" + registro.IdConcepto + "|" + registro.TipoCambio +
                        "|" + registro.Fecha.ToShortDateString() + "|0|0|DEVOLUCION");
                    }
                    foreach (DetalleMovimiento movimiento in registro.Movimientos)
                    {
                        tw.WriteLine(registro.IdDoco + "|" + registro.Vendedor + "|" + registro.Folio + "|" + registro.Serie +
                        "|" + registro.Importe + "|" + registro.Moneda + "|" + registro.IdConcepto + "|" + registro.TipoCambio +
                        "|" + registro.Fecha.ToShortDateString() + "|" + movimiento.IdMov + "|" + movimiento.Importe + "|" + movimiento.Producto);
                    }
                }

                // close the stream
                tw.Close();
            }
        }

        #endregion

        #region VENCIDOS

        public void CalcularVencidos()
        {
            List<GrupoVencimiento> grupos = PgGruposVencimiento.GruposVencimiento();
            foreach (GrupoVencimiento grupo in grupos)
            {

                throw new NotImplementedException("Not implemented yet");

            }

            
        }

        public List<FactVencimiento> VencimientosAdminPaq(GrupoVencimiento grupo, string arrCredito)
        {
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
                "AND CDEVUELTO = 0 " +
                "AND CIDCONCE01 IN (" + arrCredito + ") " +
                "AND CAFECTADO = 1 " +
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

                    //result.Add(venta);
                }
                dr.Close();
                conn.Close();
            }
            return null;
        }

        #endregion

        public void Fix()
        {
            List<Cuenta> cuentas = PgCuentas.FixCuentas(Empresa.Id);
            PgCuentas.ArreglarCuentas(cuentas);
        }

        public void Clientes()
        {
            List<Cliente> clientes = PgClientes.Clientes(Empresa.Id);

            try
            {
                string connString = "Driver={Microsoft Visual FoxPro Driver};SourceType=DBF;Exclusive=No;" +
                @"SourceDB=" + Empresa.Ruta + ";";
                using (OdbcConnection conn = new OdbcConnection(connString))
                {
                    conn.Open();
                    foreach (Cliente cliente in clientes)
                    {
                        Cliente apCliente = ClienteFromAdminPaq(cliente.ApId, conn);
                        apCliente.Id = cliente.Id;

                        PgClientes.ActualizarCliente(apCliente);
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Log.WriteEntry("EXCEPTION WHILE USING DATABASE: " + ex.Message + " || " + ex.StackTrace, EventLogEntryType.Warning, 2, 3);
            }
        }

        public void CalcularPorVencer()
        {
            throw new NotImplementedException("Not implemented yet");
        }

        public void CalcularCobrado()
        {
            throw new NotImplementedException("Not implemented yet");
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

        private List<Abono> AbonosDocumento(int idDocumento)
        {
            List<Abono> result = new List<Abono>();
            try
            {
                string connString = "Driver={Microsoft Visual FoxPro Driver};SourceType=DBF;Exclusive=No;" +
                @"SourceDB=" + Empresa.Ruta + ";";
                using (OdbcConnection conn = new OdbcConnection(connString))
                {
                    conn.Open();
                    result = AbonosDocumento(idDocumento, conn);
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
    }
}
