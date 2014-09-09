using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using ConsolaCobranza.Collectable;
using System.Configuration;
using System.Collections.Specialized;
using ConsolaCobranza.Config;

namespace ConsolaCobranza
{
    class AdminPaqMiner
    {
        public void UpdateAccounts(EventLog log)
        {
            List<int> accounts = PgDbCollector.GetAccointIds();
            log.WriteEntry("Updating " + accounts.Count + " Accounts", EventLogEntryType.Information, 19, 1);

            foreach (int accountId in accounts)
            {
                Account account = PgDbCollector.GetAccountById(accountId);

                int eId = account.Company.EnterpriseId;
                Empresa empresa = PgDbCollector.GetAccountEnterprise(eId);
                string sa = GetCodigos(empresa, false, log);

                if (sa == null)
                    continue;

                string[] arrAbonos = sa.Split(',');
                AdminPaqImpl.DownloadCollectable(account, arrAbonos);
            }
            accounts.Clear();
            accounts = null;
        }

        public void DownloadTodayAccounts(EventLog log)
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
                    log.WriteEntry("Invoice codes found for " + clientConfig.NombreEmpresa + " as [" + clientConfig.CodigosFactura + "]", EventLogEntryType.Information, 16, 1);
                    log.WriteEntry("Paiment codes found for " + clientConfig.NombreEmpresa + " as [" + clientConfig.CodigosPago + "]", EventLogEntryType.Information, 17, 1);
                    string[] facturas = clientConfig.CodigosFactura.Split(',');
                    string[] abonos = clientConfig.CodigosPago.Split(',');

                    AdminPaqImpl.DownloadCollectables(DateTime.Today, facturas, abonos, empresa, log);
                }
            }
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
                    log.WriteEntry("Paiment codes found for " + clientConfig.NombreEmpresa + " as [" + clientConfig.CodigosPago + "]", EventLogEntryType.Information, 17, 1);
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
                    log.WriteEntry("Paiment codes found for " + clientConfig.NombreEmpresa + " as [" + clientConfig.CodigosPago + "]", EventLogEntryType.Information, 17, 1);
                    string[] facturas = clientConfig.CodigosFactura.Split(',');
                    string[] abonos = clientConfig.CodigosPago.Split(',');

                    AdminPaqImpl.DownloadCollectables(date, facturas, abonos, empresa, log);
                }
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
