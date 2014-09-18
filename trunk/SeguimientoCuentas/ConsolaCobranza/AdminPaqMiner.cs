using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using ConsolaCobranza.Collectable;
using System.Configuration;
using System.Collections.Specialized;
using ConsolaCobranza.Config;
using CommonAdminPaq;
using ConsolaCobranza.Loader;
using ConsolaCobranza.Miner;
using Npgsql;
using ConsolaCobranza.Facts;

namespace ConsolaCobranza
{
    class AdminPaqMiner
    {
        public void UpdateAccounts(EventLog log)
        {
            var configuredClients = ConfigurationManager.AppSettings as NameValueCollection;
            if (configuredClients == null)
            {
                log.WriteEntry("Unable to load the configuration file.", EventLogEntryType.Warning, 13, 1);
                return;
            }
			
            if (configuredClients.Count == 0)
            {
                log.WriteEntry("No keys detected in configuration file.", EventLogEntryType.Warning, 14, 1);
                return;
            }

            foreach (var key in configuredClients.AllKeys)
            {
                string configuredClient = configuredClients.GetValues(key).FirstOrDefault();
                EnterpriseSection clientConfig = (EnterpriseSection)System.Configuration.ConfigurationManager.GetSection("Empresas/" + configuredClient);

                if (clientConfig == null)
                {
                    log.WriteEntry("Client configuration not found for Empresas/" + configuredClient + ".", EventLogEntryType.Warning, 15, 1);
                    continue;
                }

                Empresa empresa = PgDbCollector.GetCompanyByName(clientConfig.NombreEmpresa);
                log.WriteEntry("Client configuration found in database: " + clientConfig.NombreEmpresa + "; " + empresa.Ruta);

                if (empresa != null)
                {
                    log.WriteEntry("Payment codes found for " + clientConfig.NombreEmpresa + " as [" + clientConfig.CodigosPago + "]", EventLogEntryType.Information, 17, 1);
                    string[] abonos = clientConfig.CodigosPago.Split(',');


                    List<int> accounts = PgDbCollector.GetAccointIds(empresa.Id);
                    log.WriteEntry("Updating " + accounts.Count + " Accounts", EventLogEntryType.Information, 19, 1);

                    int connection = AdminPaqLib.dbLogIn("", empresa.Ruta);
                    if (connection == 0)
                    {
                        throw new Exception("No fue posible establecer la conexión con la base de datos de la empresa en la ruta: " + empresa.Ruta);
                    }

                    foreach (int accountId in accounts)
                    {
                        Account account = PgDbCollector.GetAccountById(accountId);
                        AdminPaqImpl.DownloadCollectable(account, abonos, connection);
                    }
                    accounts.Clear();
                    accounts = null;

                    AdminPaqLib.dbLogOut(connection);
                }
            }
        }

        public void DownloadTodayAccounts(EventLog log)
        {
            DownloadAccountsForDate(DateTime.Today, log);
        }

        public void DownloadAllAccounts(EventLog log)
        {

            var configuredClients = ConfigurationManager.AppSettings as NameValueCollection;
            if (configuredClients == null)
            {
                log.WriteEntry("Unable to load the configuration file.", EventLogEntryType.Warning, 13, 1);
                return;
            }

            if (configuredClients.Count == 0)
            {
                log.WriteEntry("No keys detected in configuration file.", EventLogEntryType.Warning, 14, 1);
                return;
            }

            foreach (var key in configuredClients.AllKeys)
            {
                string configuredClient = configuredClients.GetValues(key).FirstOrDefault();
                if("libDir".Equals(key.ToString())) continue;

                EnterpriseSection clientConfig = (EnterpriseSection)System.Configuration.ConfigurationManager.GetSection("Empresas/" + configuredClient);

                if (clientConfig == null)
                {
                    log.WriteEntry("Client configuration not found for Empresas/" + configuredClient + ".", EventLogEntryType.Warning, 15, 1);
                    continue;
                }

                Empresa empresa = PgDbCollector.GetCompanyByName(clientConfig.NombreEmpresa);
                log.WriteEntry("Client configuration found in database: " + clientConfig.NombreEmpresa + "; " + empresa.Ruta);

                if (empresa != null)
                {
                    log.WriteEntry("Invoice codes found for " + clientConfig.NombreEmpresa + " as [" + clientConfig.CodigosFactura + "]", EventLogEntryType.Information, 16, 1);
                    log.WriteEntry("Payment codes found for " + clientConfig.NombreEmpresa + " as [" + clientConfig.CodigosPago + "]", EventLogEntryType.Information, 17, 1);
                    string[] facturas = clientConfig.CodigosFactura.Split(',');
                    string[] abonos = clientConfig.CodigosPago.Split(',');

                    AdminPaqImpl.DownloadAllCollectables(facturas, abonos, empresa, log);
                }
            }
        }

        public void DownloadAccountsForDate(DateTime date, EventLog log)
        {

            var configuredClients = ConfigurationManager.AppSettings as NameValueCollection;
            if (configuredClients == null)
            {
                log.WriteEntry("Unable to load the configuration file.", EventLogEntryType.Warning, 13, 1);
                return;
            }

            if (configuredClients.Count == 0)
            {
                log.WriteEntry("No keys detected in configuration file.", EventLogEntryType.Warning, 14, 1);
                return;
            }

            foreach (var key in configuredClients.AllKeys)
            {
                string configuredClient = configuredClients.GetValues(key).FirstOrDefault();
                if ("libDir".Equals(key.ToString())) continue;

                EnterpriseSection clientConfig = (EnterpriseSection)System.Configuration.ConfigurationManager.GetSection("Empresas/" + configuredClient);

                if (clientConfig == null)
                {
                    log.WriteEntry("Client configuration not found for Empresas/" + configuredClient + ".", EventLogEntryType.Warning, 15, 1);
                    continue;
                }

                Empresa empresa = PgDbCollector.GetCompanyByName(clientConfig.NombreEmpresa);
                log.WriteEntry("Client configuration found in database: " + clientConfig.NombreEmpresa + "; " + empresa.Ruta);

                if (empresa != null)
                {
                    log.WriteEntry("Invoice codes found for " + clientConfig.NombreEmpresa + " as [" + clientConfig.CodigosFactura + "]", EventLogEntryType.Information, 16, 1);
                    log.WriteEntry("Payment codes found for " + clientConfig.NombreEmpresa + " as [" + clientConfig.CodigosPago + "]", EventLogEntryType.Information, 17, 1);
                    string[] facturas = clientConfig.CodigosFactura.Split(',');
                    string[] abonos = clientConfig.CodigosPago.Split(',');

                    AdminPaqImpl.DownloadCollectables(date, facturas, abonos, empresa, log);
                }
            }
        }

        public void MineMonitors(EventLog log)
        {

            var configuredClients = ConfigurationManager.AppSettings as NameValueCollection;
            if (configuredClients == null)
            {
                log.WriteEntry("Unable to load the configuration file.", EventLogEntryType.Warning, 13, 1);
                return;
            }

            if (configuredClients.Count == 0)
            {
                log.WriteEntry("No keys detected in configuration file.", EventLogEntryType.Warning, 14, 1);
                return;
            }

            //PgDbCollector.CleanFactDocument();

            foreach (var key in configuredClients.AllKeys)
            {
                string configuredClient = configuredClients.GetValues(key).FirstOrDefault();
                if ("libDir".Equals(key.ToString())) continue;

                EnterpriseSection clientConfig = (EnterpriseSection)System.Configuration.ConfigurationManager.GetSection("Empresas/" + configuredClient);

                if (clientConfig == null)
                {
                    log.WriteEntry("Client configuration not found for Empresas/" + configuredClient + ".", EventLogEntryType.Warning, 15, 1);
                    continue;
                }

                Empresa empresa = PgDbCollector.GetCompanyByName(clientConfig.NombreEmpresa);
                log.WriteEntry("Client configuration found in database: " + clientConfig.NombreEmpresa + "; " + empresa.Ruta);

                NpgsqlConnection conn = new NpgsqlConnection();
                string connectionString = ConfigurationManager.ConnectionStrings[Config.Common.JASPER].ConnectionString;
                conn = new NpgsqlConnection(connectionString);
                conn.Open();

                if (empresa != null)
                {
                    log.WriteEntry("Invoice codes found for " + clientConfig.NombreEmpresa + " as [" + clientConfig.CodigosFactura + "]", EventLogEntryType.Information, 16, 1);
                    log.WriteEntry("Payment codes found for " + clientConfig.NombreEmpresa + " as [" + clientConfig.CodigosPago + "]", EventLogEntryType.Information, 17, 1);
                    log.WriteEntry("Sale codes found for " + clientConfig.NombreEmpresa + " as [" + clientConfig.CodigosVenta + "]", EventLogEntryType.Information, 17, 1);
                    log.WriteEntry("Return codes found for " + clientConfig.NombreEmpresa + " as [" + clientConfig.CodigosDevolucion + "]", EventLogEntryType.Information, 17, 1);
                    string[] facturas = clientConfig.CodigosFactura.Split(',');
                    string[] abonos = clientConfig.CodigosPago.Split(',');
                    string[] venta = clientConfig.CodigosVenta.Split(',');
                    string[] devolucion = clientConfig.CodigosDevolucion.Split(',');
                    try 
                    {   
                        //AdminPaqImpl.DownloadMonitors(venta, devolucion, facturas, abonos, empresa, log);

                        log.WriteEntry("Downloading from AdminPaq: " + empresa.Ruta, EventLogEntryType.Information, 8, 2);
                        // DIM ETLs
                        List<CatCliente> clientes = CatCliente.GetClientes(empresa.Ruta);
                        log.WriteEntry(clientes.Count + " clientes found for " + empresa.Nombre + " in AdminPaq", EventLogEntryType.Information, 9, 2);
                        ETLClientes.Execute(empresa.Id, empresa.Nombre, clientes, conn);

                        List<CatSeller> sellers = CatSeller.GetSellers(empresa.Ruta);
                        log.WriteEntry(sellers.Count + " agents found for " + empresa.Nombre + " in AdminPaq", EventLogEntryType.Information, 10, 2);
                        ETLSellers.Execute(empresa.Id, empresa.Nombre, sellers, conn);

                        ETLMeses.Execute(conn);

                        // FACT Preparation
                        FactVencido vencido = new FactVencido();
                        vencido.Prepare(empresa.Id, empresa.Ruta, conn);
                        log.WriteEntry(string.Format("Prepared due documents for {0}", empresa.Nombre), EventLogEntryType.Information, 11, 2);

                        FactPorVencer porVencer = new FactPorVencer();
                        porVencer.Prepare(empresa.Id, empresa.Ruta, conn);
                        log.WriteEntry(string.Format("Prepared documents about to due for {0}", empresa.Nombre), EventLogEntryType.Information, 12, 2);

                        FactCobranza cobranza = new FactCobranza();
                        cobranza.Prepare(empresa.Id, empresa.Ruta, conn);
                        log.WriteEntry(string.Format("Prepared collection documents for {0}", empresa.Nombre), EventLogEntryType.Information, 13, 2);

                        FactSales factSale = new FactSales();
                        factSale.Prepare(empresa.Id, empresa.Ruta, conn);
                        log.WriteEntry(string.Format("Prepared sale documents for {0}", empresa.Nombre), EventLogEntryType.Information, 14, 2);

                        // FILL FACTS
                        DocsMiner dMiner = new DocsMiner();
                        dMiner.Vencidos = vencido.GruposVencimiento;
                        dMiner.PorVencer = porVencer.GruposVencimiento;
                        dMiner.Cobranza = cobranza.GruposCobranza;
                        dMiner.Ventas = factSale.GruposVenta;

                        log.WriteEntry(string.Format("Mining documents for {0} started", empresa.Nombre), EventLogEntryType.Information, 15, 2);

                        dMiner.Execute(empresa, facturas, abonos, venta, devolucion, log);
                        log.WriteEntry(string.Format("Mining documents for {0} completed", empresa.Nombre), EventLogEntryType.Information, 16, 2);

                        MainLoader loader = new MainLoader();
                        loader.Vencidos = dMiner.Vencidos;
                        loader.PorVencer = dMiner.PorVencer;
                        loader.Cobranza = dMiner.Cobranza;
                        loader.Ventas = dMiner.Ventas;

                        log.WriteEntry(string.Format("Loading documents for {0} started", empresa.Nombre), EventLogEntryType.Information, 17, 2);
                        loader.Load(empresa.Id, conn);
                        log.WriteEntry(string.Format("Loading documents for {0} completed", empresa.Nombre), EventLogEntryType.Information, 18, 2);
                        
                    }catch(Exception ex){
                        log.WriteEntry("Exception while mining monitors: " + ex.Message + " || " + ex.StackTrace, EventLogEntryType.Error, 19, 2);
                    }
                }
                conn.Close();
            }
        }


        private string GetCodigos(Empresa empresa, bool factura, EventLog log)
        {
            var configuredClients = ConfigurationManager.AppSettings as NameValueCollection;
            if (configuredClients == null)
            {
                log.WriteEntry("Unable to load the configuration file.", EventLogEntryType.Warning, 21, 1);
                return null;
            }

            if (configuredClients.Count == 0)
            {
                log.WriteEntry("No keys detected in configuration file.", EventLogEntryType.Warning, 22, 1);
                return null;
            }

            foreach (var key in configuredClients.AllKeys)
            {
                string configuredClient = configuredClients.GetValues(key).FirstOrDefault();
                EnterpriseSection clientConfig = (EnterpriseSection)System.Configuration.ConfigurationManager.GetSection("Empresas/" + configuredClient);

                if (clientConfig == null)
                {
                    log.WriteEntry("Client configuration not found for Empresas/" + configuredClient + ".", EventLogEntryType.Warning, 23, 1);
                    continue;
                }

                if (empresa.Nombre.Equals(clientConfig.NombreEmpresa))
                {
                    if (factura)
                    {
                        return clientConfig.CodigosFactura;
                    }
                    else
                    {
                        return clientConfig.CodigosPago;
                    }
                }
            }

            return null;
        }

    }
}
