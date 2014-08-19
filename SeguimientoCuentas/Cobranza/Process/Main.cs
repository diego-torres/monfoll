using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cobranza.Collectable;
using System.Diagnostics;
using System.Configuration;
using System.Collections.Specialized;
using Cobranza.Config;

namespace Cobranza.Process
{
    public class Main
    {
        private PgDbCollector db = new PgDbCollector();
        private AdminPaqImpl api = new AdminPaqImpl();

        public void Execute(EventLog log)
        {
            if (!db.HasExecuted)
            {
                if (!db.PgHasData)
                {
                    // Retrieve from first date
                    db.HasExecuted = true;
                }
                else
                { 
                    // Update and retrieve date
                    UpdateAccounts(log);
                    DownloadTodayAccounts(log);
                    db.HasExecuted = true;
                }
            }
            else 
            { 
                // Update and retrieve date
                UpdateAccounts(log);
                DownloadTodayAccounts(log);
                db.HasExecuted = true;
            }
        }

        private void DownloadTodayAccounts(EventLog log)
        { 

            var configuredClients = ConfigurationManager.AppSettings as NameValueCollection;
            if (configuredClients == null)
            {
                log.WriteEntry("Unable to load the configuration file.", EventLogEntryType.Warning, 19, 3);
                return;
            }

            if (configuredClients.Count == 0)
            {
                log.WriteEntry("No keys detected in configuration file.", EventLogEntryType.Warning, 20, 3);
                return;
            }

            foreach (var key in configuredClients.AllKeys)
            {
                string configuredClient = configuredClients.GetValues(key).FirstOrDefault();
                EnterpriseSection clientConfig = (EnterpriseSection)System.Configuration.ConfigurationManager.GetSection("Empresas/" + configuredClient);

                if (clientConfig == null)
                {
                    log.WriteEntry("Client configuration not found for Empresas/" + configuredClient + ".", EventLogEntryType.Warning, 21, 3);
                    continue;
                }

                Empresa empresa = db.GetCompanyByName(clientConfig.NombreEmpresa);

                if(empresa!=null)
                {
                    log.WriteEntry("Invoice codes found for " + clientConfig.NombreEmpresa + " as [" + clientConfig.CodigosFactura + "]", EventLogEntryType.Information, 22, 3);
                    string[] facturas = clientConfig.CodigosFactura.Split(',');
                    string[] abonos = clientConfig.CodigosPago.Split(',');

                    List<Account> todayAccounts = api.DownloadCollectables(facturas, abonos, empresa);
                    foreach (Account account in todayAccounts)
                    {
                        if (!db.AccountExists(account))
                            db.AddAccount(account, log, api);
                    }

                }
            }

            
        }

        private void UpdateAccounts(EventLog log)
        {
            List<Account> accounts = db.GetAccounts();
            Dictionary<int, string> facturas = new Dictionary<int, string>();
            Dictionary<int, string> abonos = new Dictionary<int, string>();

            foreach (Account account in accounts)
            {
                int eId = account.Company.EnterpriseId;
                if (!facturas.ContainsKey(account.Company.EnterpriseId))
                {   
                    Empresa empresa = db.GetAccountEnterprise(eId);

                    if (empresa == null) continue;
                    string sFacturas = GetCodigos(empresa, true, log);
                    string sAbonos = GetCodigos(empresa, false, log);

                    facturas.Add(eId, sFacturas);
                    abonos.Add(eId, sAbonos);
                }

                string sf, sa;
                bool got = false;

                got = facturas.TryGetValue(eId, out sf);
                if (!got) continue;

                got = abonos.TryGetValue(eId, out sa);
                if (!got) continue;

                string[] arrFacturas = sf.Split(',');
                string[] arrAbonos = sa.Split(',');

                bool isCancelled = false;
                Account source = account;
                api.DownloadCollectable(ref source, arrAbonos, log, out isCancelled);

                db.UpdateAccount(source, isCancelled, log, api);

                foreach (Payment pay in source.Payments)
                {
                    pay.DocId = account.DocId;
                    db.SavePayment(pay, log);
                }

            }
        }

        private string GetCodigos(Empresa empresa, bool factura, EventLog log)
        {
            var configuredClients = ConfigurationManager.AppSettings as NameValueCollection;
            if (configuredClients == null)
            {
                log.WriteEntry("Unable to load the configuration file.", EventLogEntryType.Warning, 19, 3);
                return null;
            }

            if (configuredClients.Count == 0)
            {
                log.WriteEntry("No keys detected in configuration file.", EventLogEntryType.Warning, 20, 3);
                return null;
            }

            foreach (var key in configuredClients.AllKeys)
            {
                string configuredClient = configuredClients.GetValues(key).FirstOrDefault();
                EnterpriseSection clientConfig = (EnterpriseSection)System.Configuration.ConfigurationManager.GetSection("Empresas/" + configuredClient);

                if (clientConfig == null)
                {
                    log.WriteEntry("Client configuration not found for Empresas/" + configuredClient + ".", EventLogEntryType.Warning, 21, 3);
                    continue;
                }

                if (empresa.Nombre.Equals(clientConfig.NombreEmpresa))
                {
                    log.WriteEntry("Invoice codes found for " + clientConfig.NombreEmpresa + " as [" + clientConfig.CodigosFactura + "]", EventLogEntryType.Information, 22, 3);
                    if(factura)
                        return clientConfig.CodigosFactura;
                    else
                        return clientConfig.CodigosPago;
                        //return clientConfig.CodigosPago.Split(',');
                }
            }

            return null;
        }

    }
}
