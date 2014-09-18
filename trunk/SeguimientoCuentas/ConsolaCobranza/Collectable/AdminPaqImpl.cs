using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using CommonAdminPaq;
using System.Globalization;
using ConsolaCobranza.Loader;
using ConsolaCobranza.Facts;

namespace ConsolaCobranza.Collectable
{
    public class AdminPaqImpl
    {

        public List<FactVencido> Vencidos { get; set; }
        public List<FactPorVencer> PorVencer { get; set; }
        public List<FactCobranza> Cobranza { get; set; }
        public List<FactSales> Ventas { get; set; }

        public static Company GetCompany(int companyId, string enterprisePath, int enterPriseId)
        {
            Company company = null;
            int connCompany, dbResponse, fqResponse, ubicacion = 0;
            int agentId = 0;
            StringBuilder sbCompanyCode = new StringBuilder(31);
            StringBuilder sbPaymentDay = new StringBuilder(51);
            string key;

            connCompany = AdminPaqLib.dbLogIn("", enterprisePath);
            if (connCompany == 0)
            {
                ErrLogger.Log("Unable to open connection for company");
                return null;
            }

            key = companyId.ToString().PadLeft(11);
            dbResponse = AdminPaqLib.dbGetNoLock(connCompany, TableNames.CLIENTES_PROVEEDORES, IndexNames.PRIMARY_KEY, key);

            if (dbResponse == 0)
            {
                company = new Company();
                fqResponse = AdminPaqLib.dbFieldChar(connCompany, TableNames.CLIENTES_PROVEEDORES, 2, sbCompanyCode, 31);
                company.Code = sbCompanyCode.ToString().Substring(0, 30).Trim();
                fqResponse = AdminPaqLib.dbFieldChar(connCompany, TableNames.CLIENTES_PROVEEDORES, 106, sbPaymentDay, 51);
                company.PaymentDay = sbPaymentDay.ToString().Substring(0, 50).Trim();
                fqResponse = AdminPaqLib.dbFieldLong(connCompany, TableNames.CLIENTES_PROVEEDORES, 37, ref agentId);
                company.AgentCode = AgentCode(agentId, enterprisePath);
                fqResponse = AdminPaqLib.dbFieldLong(connCompany, TableNames.CLIENTES_PROVEEDORES, 14, ref ubicacion);

                /*FORANEO=2/LOCAL=1*/
                company.EsLocal = ubicacion == 1;
            }

            AdminPaqLib.dbLogOut(connCompany);

            company.ApId = companyId;
            company.EnterpriseId = enterPriseId;

            return company;
        }

        private static string AgentCode(int agentId, string filePath)
        {
            int connAgent, dbResponse, fqResponse;
            StringBuilder sbAgentCode = new StringBuilder(31);
            string result = string.Empty, key;

            connAgent = AdminPaqLib.dbLogIn("", filePath);
            if (connAgent == 0)
            {
                ErrLogger.Log("Unable to open connection for agent codes");
                return result;
            }

            key = agentId.ToString().PadLeft(11);
            dbResponse = AdminPaqLib.dbGetNoLock(connAgent, TableNames.AGENTES, IndexNames.PRIMARY_KEY, key);

            if (dbResponse == 0)
            {
                fqResponse = AdminPaqLib.dbFieldChar(connAgent, TableNames.AGENTES, 2, sbAgentCode, 31);
                result = sbAgentCode.ToString().Substring(0, 30).Trim();
            }

            AdminPaqLib.dbLogOut(connAgent);
            return result;
        }

        public static void DownloadMonitors(string[] conceptosVenta, string[] conceptosDevolucion, string[] conceptosFactura, string[] conceptosAbono, Empresa configuredCompany, EventLog log)
        {
            int counter = 0;
            int connDocos, dbResponse, fieldResponse, collectableDocType = 4;

            int docType = collectableDocType, cancelled = 0, conceptId = 0, docId = 0, currencyId = 0, companyId = 0, impreso = 0, idAgente = 0;
            double saldo = 0, cambio = 0;
            StringBuilder sbFechaDoco = new StringBuilder(9);
            StringBuilder sbFechaVenc = new StringBuilder(9);
            StringBuilder sbCompanyName = new StringBuilder(61);
            StringBuilder sbSerieDoc = new StringBuilder(12);
            StringBuilder sbTipoCobro = new StringBuilder(51);
            StringBuilder sbObservations = new StringBuilder(51);
            string sFechaDoco, sFechaVenc, sCompanyName, currencyName;

            bool isCredit = false, isPayment = false, isSale = false, isReturn = false;

            Dictionary<int, string> currencies = new Dictionary<int, string>();
            Dictionary<int, Company> companies = new Dictionary<int, Company>();
            Company documentCo;

            if (configuredCompany == null)
            {
                log.WriteEntry("Wrong Company configuration.", EventLogEntryType.Error, 1, 2);
                throw new Exception("La empresa no ha sido correctamente configurada para realizar esta acción.");
            }

            connDocos = AdminPaqLib.dbLogIn("", configuredCompany.Ruta);
            if (connDocos == 0)
            {
                log.WriteEntry("Unable to open connection to documents table for company [" + configuredCompany.Nombre + "]", EventLogEntryType.Error, 2, 2);
                throw new Exception("No se pudo establecer conexión con adminPaq en la siguiente ruta: " + configuredCompany.Ruta);
            }

            dbResponse = AdminPaqLib.dbGetTopNoLock(connDocos, TableNames.DOCUMENTOS, IndexNames.PRIMARY_KEY);

            if (dbResponse != 0)
            {
                log.WriteEntry("No data found");
                AdminPaqLib.dbLogOut(connDocos);
            }


            while (dbResponse == 0)
            {
                fieldResponse = AdminPaqLib.dbFieldLong(connDocos, TableNames.DOCUMENTOS, 1, ref docId);
                fieldResponse = AdminPaqLib.dbFieldLong(connDocos, TableNames.DOCUMENTOS, 26, ref cancelled);
                if (cancelled != 0)
                {
                    dbResponse = AdminPaqLib.dbSkip(connDocos, TableNames.DOCUMENTOS, IndexNames.PRIMARY_KEY, 1);
                    continue;
                }

                fieldResponse = AdminPaqLib.dbFieldChar(connDocos, TableNames.DOCUMENTOS, 6, sbFechaDoco, 9);
                sFechaDoco = sbFechaDoco.ToString().Substring(0, 8);

                fieldResponse = AdminPaqLib.dbFieldLong(connDocos, TableNames.DOCUMENTOS, 3, ref conceptId);
                DocumentConcept concept = GetDocumentConcept(conceptId, configuredCompany.Ruta);
                if (concept == null)
                {
                    dbResponse = AdminPaqLib.dbSkip(connDocos, TableNames.DOCUMENTOS, IndexNames.PRIMARY_KEY, 1);
                    continue;
                }

                isCredit = conceptosFactura.Contains(concept.Code);
                isPayment = conceptosAbono.Contains(concept.Code);
                isSale = conceptosVenta.Contains(concept.Code);
                isReturn = conceptosDevolucion.Contains(concept.Code);

                if (!isCredit && !isPayment && !isSale && !isReturn)
                {
                    dbResponse = AdminPaqLib.dbSkip(connDocos, TableNames.DOCUMENTOS, IndexNames.PRIMARY_KEY, 1);
                    continue;
                }

                if (isSale)
                {
                    AdminPaqLib.dbFieldLong(connDocos, TableNames.DOCUMENTOS, 25, ref impreso);
                    if (impreso == 0)
                    {
                        dbResponse = AdminPaqLib.dbSkip(connDocos, TableNames.DOCUMENTOS, IndexNames.PRIMARY_KEY, 1);
                        continue;
                    }
                }

                if (isCredit)
                    AdminPaqLib.dbFieldDouble(connDocos, TableNames.DOCUMENTOS, 44, ref saldo);
                else
                    AdminPaqLib.dbFieldDouble(connDocos, TableNames.DOCUMENTOS, 31, ref saldo);

                AdminPaqLib.dbFieldLong(connDocos, TableNames.DOCUMENTOS, 10, ref idAgente);

                fieldResponse = AdminPaqLib.dbFieldLong(connDocos, TableNames.DOCUMENTOS, 7, ref companyId);

                FactDocument doco = new FactDocument();
                doco.IsCredit = isCredit;
                doco.IsPayment = isPayment;
                doco.IsSale = isSale;

                AdminPaqLib.dbFieldLong(connDocos, TableNames.DOCUMENTOS, 15, ref currencyId);
                if (!currencies.ContainsKey(currencyId))
                {
                    currencyName = CurrencyName(currencyId, configuredCompany.Ruta);
                    if (currencyName != null)
                        currencies.Add(currencyId, currencyName);
                }

                currencyName = currencies[currencyId];

                if (!currencyName.ToUpper().Contains("PESO"))
                {
                    AdminPaqLib.dbFieldDouble(connDocos, TableNames.DOCUMENTOS, 16, ref cambio);
                    saldo = saldo * cambio;
                }

                if(isReturn)
                    doco.Amount = saldo * -1;
                else
                    doco.Amount = saldo;


                doco.DocumentDate = DateTime.ParseExact(sFechaDoco, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);

                if (isCredit)
                {
                    if (!companies.ContainsKey(companyId))
                    {
                        fieldResponse = AdminPaqLib.dbFieldChar(connDocos, TableNames.DOCUMENTOS, 8, sbCompanyName, 61);
                        sCompanyName = sbCompanyName.ToString().Substring(0, 60).Trim();

                        documentCo = new Company();
                        documentCo.ApId = companyId;
                        documentCo.Name = sCompanyName;
                        documentCo.EnterpriseId = configuredCompany.Id;
                        documentCo.EnterprisePath = configuredCompany.Ruta;
                        FillCompany(documentCo, configuredCompany.Ruta);

                        companies.Add(companyId, documentCo);
                    }

                    doco.Company = companies[companyId];

                    AdminPaqLib.dbFieldChar(connDocos, TableNames.DOCUMENTOS, 11, sbFechaVenc, 9);
                    sFechaVenc = sbFechaVenc.ToString().Substring(0, 8).Trim();

                    doco.DueDate = DateTime.ParseExact(sFechaVenc, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);

                }

                if (isCredit || isReturn)
                {
                    doco.SellerId = idAgente;
                }
                try
                {
                    PgDbCollector.AddFactDocument(doco, log);
                    counter++;
                }
                catch (Exception dbEx)
                {
                    log.WriteEntry("Unable to save account: " + dbEx.Message + " || " + dbEx.StackTrace, EventLogEntryType.Warning);
                }

                dbResponse = AdminPaqLib.dbSkip(connDocos, TableNames.DOCUMENTOS, IndexNames.PRIMARY_KEY, 1);
            }

            currencies.Clear();
            currencies = null;

            companies.Clear();
            companies = null;

            AdminPaqLib.dbLogOut(connDocos);
            log.WriteEntry("Added " + counter.ToString() + " accounts to database", EventLogEntryType.Information, 10, 2);
        }

        public static void DownloadAllCollectables(string[] conceptosFactura, string[] conceptosAbono, Empresa configuredCompany, EventLog log)
        {
            int counter = 0;
            int connDocos, dbResponse, fieldResponse, collectableDocType = 4;

            int docType = collectableDocType, cancelled = 0, conceptId = 0, docId = 0, folioDoc = 0, currencyId = 0, companyId = 0;
            double saldoPendiente = 0, totalDoc = 0;
            StringBuilder sbFechaDoco = new StringBuilder(9);
            StringBuilder sbFechaVenc = new StringBuilder(9);
            StringBuilder sbCompanyName = new StringBuilder(61);
            StringBuilder sbSerieDoc = new StringBuilder(12);
            StringBuilder sbTipoCobro = new StringBuilder(51);
            StringBuilder sbObservations = new StringBuilder(51);
            string sFechaDoco, sFechaVenc, sCompanyName, sSerieDoc, currencyName;

            bool isFactura = false;

            Dictionary<int, string> currencies = new Dictionary<int, string>();
            Dictionary<int, Company> companies = new Dictionary<int, Company>();
            Company documentCo;

            if (configuredCompany == null)
            {
                log.WriteEntry("Wrong Company configuration.", EventLogEntryType.Error, 1, 2);
                throw new Exception("La empresa no ha sido correctamente configurada para realizar esta acción.");
            }

            connDocos = AdminPaqLib.dbLogIn("", configuredCompany.Ruta);
            if (connDocos == 0)
            {
                log.WriteEntry("Unable to open connection to documents table for company [" + configuredCompany.Nombre + "]", EventLogEntryType.Error, 2, 2);
                throw new Exception("No se pudo establecer conexión con adminPaq en la siguiente ruta: " + configuredCompany.Ruta);
            }

            dbResponse = AdminPaqLib.dbGetTopNoLock(connDocos, TableNames.DOCUMENTOS, IndexNames.PRIMARY_KEY);

            if (dbResponse != 0)
            {
                log.WriteEntry("No data found, AdminPaq responded as " + dbResponse, EventLogEntryType.Warning);
                AdminPaqLib.dbLogOut(connDocos);
            }


            while (dbResponse == 0)
            {
                fieldResponse = AdminPaqLib.dbFieldLong(connDocos, TableNames.DOCUMENTOS, 2, ref docType);
                if (docType != collectableDocType)
                {
                    dbResponse = AdminPaqLib.dbSkip(connDocos, TableNames.DOCUMENTOS, IndexNames.PRIMARY_KEY, 1);
                    continue;
                }

                fieldResponse = AdminPaqLib.dbFieldLong(connDocos, TableNames.DOCUMENTOS, 1, ref docId);
                fieldResponse = AdminPaqLib.dbFieldLong(connDocos, TableNames.DOCUMENTOS, 26, ref cancelled);
                if (cancelled != 0)
                {
                    dbResponse = AdminPaqLib.dbSkip(connDocos, TableNames.DOCUMENTOS, IndexNames.PRIMARY_KEY, 1);
                    continue;
                }

                fieldResponse = AdminPaqLib.dbFieldDouble(connDocos, TableNames.DOCUMENTOS, 44, ref saldoPendiente);

                if (saldoPendiente <= 0)
                {
                    dbResponse = AdminPaqLib.dbSkip(connDocos, TableNames.DOCUMENTOS, IndexNames.PRIMARY_KEY, 1);
                    continue;
                }

                fieldResponse = AdminPaqLib.dbFieldChar(connDocos, TableNames.DOCUMENTOS, 6, sbFechaDoco, 9);
                sFechaDoco = sbFechaDoco.ToString().Substring(0, 8);

                fieldResponse = AdminPaqLib.dbFieldLong(connDocos, TableNames.DOCUMENTOS, 3, ref conceptId);
                DocumentConcept concept = GetDocumentConcept(conceptId, configuredCompany.Ruta);
                if (concept == null)
                {
                    dbResponse = AdminPaqLib.dbSkip(connDocos, TableNames.DOCUMENTOS, IndexNames.PRIMARY_KEY, 1);
                    continue;
                }

                isFactura = conceptosFactura.Contains(concept.Code);

                if (!isFactura)
                {
                    dbResponse = AdminPaqLib.dbSkip(connDocos, TableNames.DOCUMENTOS, IndexNames.PRIMARY_KEY, 1);
                    continue;
                }

                Account account = new Account();
                account.DocDate = DateTime.ParseExact(sFechaDoco, IndexNames.DATE_FORMAT_PATTERN, CultureInfo.InvariantCulture);

                account.Balance = saldoPendiente;
                account.DocType = concept.Name;

                account.ApId = docId;

                fieldResponse = AdminPaqLib.dbFieldChar(connDocos, TableNames.DOCUMENTOS, 11, sbFechaVenc, 9);
                sFechaVenc = sbFechaVenc.ToString().Substring(0, 8).Trim();
                account.DueDate = DateTime.ParseExact(sFechaVenc, IndexNames.DATE_FORMAT_PATTERN, CultureInfo.InvariantCulture);

                fieldResponse = AdminPaqLib.dbFieldChar(connDocos, TableNames.DOCUMENTOS, 4, sbSerieDoc, 12);
                sSerieDoc = sbSerieDoc.ToString().Substring(0, 11).Trim();
                account.Serie = sSerieDoc;

                fieldResponse = AdminPaqLib.dbFieldLong(connDocos, TableNames.DOCUMENTOS, 5, ref folioDoc);
                account.Folio = folioDoc;

                fieldResponse = AdminPaqLib.dbFieldDouble(connDocos, TableNames.DOCUMENTOS, 43, ref totalDoc);
                account.Amount = totalDoc;

                fieldResponse = AdminPaqLib.dbFieldLong(connDocos, TableNames.DOCUMENTOS, 15, ref currencyId);
                if (!currencies.ContainsKey(currencyId))
                {
                    currencyName = CurrencyName(currencyId, configuredCompany.Ruta);
                    if (currencyName != null)
                        currencies.Add(currencyId, currencyName);
                }
                account.Currency = currencies[currencyId];

                fieldResponse = AdminPaqLib.dbFieldLong(connDocos, TableNames.DOCUMENTOS, 7, ref companyId);

                if (!companies.ContainsKey(companyId))
                {
                    fieldResponse = AdminPaqLib.dbFieldChar(connDocos, TableNames.DOCUMENTOS, 8, sbCompanyName, 61);
                    sCompanyName = sbCompanyName.ToString().Substring(0, 60).Trim();

                    documentCo = new Company();
                    documentCo.ApId = companyId;
                    documentCo.Name = sCompanyName;
                    documentCo.EnterpriseId = configuredCompany.Id;
                    documentCo.EnterprisePath = configuredCompany.Ruta;
                    FillCompany(documentCo, configuredCompany.Ruta);

                    companies.Add(companyId, documentCo);
                }

                account.Company = companies[companyId];
                FillPayments(account, configuredCompany.Ruta, conceptosAbono);

                if (!PgDbCollector.AccountExists(account))
                {
                    try
                    {
                        PgDbCollector.AddAccount(account, log);
                        counter++;
                    }
                    catch (Exception dbEx)
                    {
                        log.WriteEntry("Unable to save account: " + dbEx.Message + " || " + dbEx.StackTrace, EventLogEntryType.Warning);
                    }
                }

                dbResponse = AdminPaqLib.dbSkip(connDocos, TableNames.DOCUMENTOS, IndexNames.PRIMARY_KEY, 1);
            }

            currencies.Clear();
            currencies = null;

            companies.Clear();
            companies = null;

            AdminPaqLib.dbLogOut(connDocos);
            log.WriteEntry("Added " + counter.ToString() + " accounts to database", EventLogEntryType.Information, 10, 2);
        }

        public static void DownloadCollectables(DateTime date, string[] conceptosFactura, string[] conceptosAbono, Empresa configuredCompany, EventLog log)
        {
            int counter = 0;
            int connDocos, dbResponse, fieldResponse, collectableDocType = 4;
            string startDate, tipo_doc;

            int docType = collectableDocType, cancelled = 0, conceptId = 0, docId = 0, folioDoc = 0, currencyId = 0, companyId = 0;
            double saldoPendiente = 0, totalDoc = 0;
            StringBuilder sbFechaDoco = new StringBuilder(9);
            StringBuilder sbFechaVenc = new StringBuilder(9);
            StringBuilder sbCompanyName = new StringBuilder(61);
            StringBuilder sbSerieDoc = new StringBuilder(12);
            StringBuilder sbTipoCobro = new StringBuilder(51);
            StringBuilder sbObservations = new StringBuilder(51);
            string sFechaDoco, sFechaVenc, sCompanyName, sSerieDoc, currencyName;

            bool isFactura = false;

            Dictionary<int, string> currencies = new Dictionary<int, string>();
            Dictionary<int, Company> companies = new Dictionary<int, Company>();
            Company documentCo;

            if (configuredCompany == null)
            {
                log.WriteEntry("Wrong Company configuration.", EventLogEntryType.Error, 1, 2);
                throw new Exception("La empresa no ha sido correctamente configurada para realizar esta acción.");
            }

            connDocos = AdminPaqLib.dbLogIn("", configuredCompany.Ruta);
            if (connDocos == 0)
            {
                log.WriteEntry("Unable to open connection to documents table for company [" + configuredCompany.Nombre + "]", EventLogEntryType.Error, 2, 2);
                throw new Exception("No se pudo establecer conexión con adminPaq en la siguiente ruta: " + configuredCompany.Ruta);
            }

            startDate = date.ToString(IndexNames.DATE_FORMAT_PATTERN);

            tipo_doc = collectableDocType.ToString().PadLeft(11);

            string key = tipo_doc + startDate;
            //string key = startDate;
            dbResponse = AdminPaqLib.dbGetNoLock(connDocos, TableNames.DOCUMENTOS, IndexNames.DOCUMENTOS_ID_DOCUMENTO01, key);
            
            if (dbResponse != 0)
            {
                log.WriteEntry("No data found for date: " + startDate);
                AdminPaqLib.dbLogOut(connDocos);
            }


            while (dbResponse == 0)
            {
                fieldResponse = AdminPaqLib.dbFieldLong(connDocos, TableNames.DOCUMENTOS, 1, ref docId);
                fieldResponse = AdminPaqLib.dbFieldLong(connDocos, TableNames.DOCUMENTOS, 2, ref docType);
                if (docType != collectableDocType)
                {
                    break;
                }
                
                fieldResponse = AdminPaqLib.dbFieldLong(connDocos, TableNames.DOCUMENTOS, 26, ref cancelled);
                if (cancelled != 0)
                {
                    dbResponse = AdminPaqLib.dbSkip(connDocos, TableNames.DOCUMENTOS, IndexNames.DOCUMENTOS_ID_DOCUMENTO01, 1);
                    continue;
                }

                fieldResponse = AdminPaqLib.dbFieldDouble(connDocos, TableNames.DOCUMENTOS, 44, ref saldoPendiente);

                if (saldoPendiente <= 0)
                {
                    dbResponse = AdminPaqLib.dbSkip(connDocos, TableNames.DOCUMENTOS, IndexNames.DOCUMENTOS_ID_DOCUMENTO01, 1);
                    continue;
                }

                fieldResponse = AdminPaqLib.dbFieldChar(connDocos, TableNames.DOCUMENTOS, 6, sbFechaDoco, 9);
                sFechaDoco = sbFechaDoco.ToString().Substring(0, 8);

                if (!sFechaDoco.Equals(startDate))
                {
                    dbResponse = AdminPaqLib.dbSkip(connDocos, TableNames.DOCUMENTOS, IndexNames.DOCUMENTOS_ID_DOCUMENTO01, 1);
                    continue;
                }

                fieldResponse = AdminPaqLib.dbFieldLong(connDocos, TableNames.DOCUMENTOS, 3, ref conceptId);
                DocumentConcept concept = GetDocumentConcept(conceptId, configuredCompany.Ruta);
                if (concept == null)
                {
                    dbResponse = AdminPaqLib.dbSkip(connDocos, TableNames.DOCUMENTOS, IndexNames.DOCUMENTOS_ID_DOCUMENTO01, 1);
                    continue;
                }

                isFactura = conceptosFactura.Contains(concept.Code);

                if (!isFactura)
                {
                    dbResponse = AdminPaqLib.dbSkip(connDocos, TableNames.DOCUMENTOS, IndexNames.DOCUMENTOS_ID_DOCUMENTO01, 1);
                    continue;
                }

                Account account = new Account();
                account.DocDate = DateTime.ParseExact(sFechaDoco, IndexNames.DATE_FORMAT_PATTERN, CultureInfo.InvariantCulture);

                account.Balance = saldoPendiente;
                account.DocType = concept.Name;

                account.ApId = docId;

                fieldResponse = AdminPaqLib.dbFieldChar(connDocos, TableNames.DOCUMENTOS, 11, sbFechaVenc, 9);
                sFechaVenc = sbFechaVenc.ToString().Substring(0, 8).Trim();
                account.DueDate = DateTime.ParseExact(sFechaVenc, IndexNames.DATE_FORMAT_PATTERN, CultureInfo.InvariantCulture);

                fieldResponse = AdminPaqLib.dbFieldChar(connDocos, TableNames.DOCUMENTOS, 4, sbSerieDoc, 12);
                sSerieDoc = sbSerieDoc.ToString().Substring(0, 11).Trim();
                account.Serie = sSerieDoc;

                fieldResponse = AdminPaqLib.dbFieldLong(connDocos, TableNames.DOCUMENTOS, 5, ref folioDoc);
                account.Folio = folioDoc;

                fieldResponse = AdminPaqLib.dbFieldDouble(connDocos, TableNames.DOCUMENTOS, 43, ref totalDoc);
                account.Amount = totalDoc;

                fieldResponse = AdminPaqLib.dbFieldLong(connDocos, TableNames.DOCUMENTOS, 15, ref currencyId);
                if (!currencies.ContainsKey(currencyId))
                {
                    currencyName = CurrencyName(currencyId, configuredCompany.Ruta);
                    if (currencyName != null)
                        currencies.Add(currencyId, currencyName);
                }
                account.Currency = currencies[currencyId];

                fieldResponse = AdminPaqLib.dbFieldLong(connDocos, TableNames.DOCUMENTOS, 7, ref companyId);

                if (!companies.ContainsKey(companyId))
                {
                    fieldResponse = AdminPaqLib.dbFieldChar(connDocos, TableNames.DOCUMENTOS, 8, sbCompanyName, 61);
                    sCompanyName = sbCompanyName.ToString().Substring(0, 60).Trim();

                    documentCo = new Company();
                    documentCo.ApId = companyId;
                    documentCo.Name = sCompanyName;
                    documentCo.EnterpriseId = configuredCompany.Id;
                    documentCo.EnterprisePath = configuredCompany.Ruta;
                    FillCompany(documentCo, configuredCompany.Ruta);

                    companies.Add(companyId, documentCo);
                }

                account.Company = companies[companyId];
                FillPayments(account, configuredCompany.Ruta, conceptosAbono);

                if (!PgDbCollector.AccountExists(account))
                {
                    try 
                    {
                        PgDbCollector.AddAccount(account, log);
                        counter++;
                    }
                    catch (Exception dbEx)
                    {
                        log.WriteEntry("Unable to save account: " + dbEx.Message + " || " + dbEx.StackTrace, EventLogEntryType.Warning);
                    }
                }

                dbResponse = AdminPaqLib.dbSkip(connDocos, TableNames.DOCUMENTOS, "CFECHA", 1);
            }

            currencies.Clear();
            currencies = null;

            companies.Clear();
            companies = null;

            AdminPaqLib.dbLogOut(connDocos);
            log.WriteEntry("Added " + counter.ToString() + " accounts to database", EventLogEntryType.Information, 10, 2);
        }

        public static void DownloadCollectable(Account source, string[] conceptosAbono, int connection)
        {
            bool isCancelled = false;
            int dbResponse, fieldResponse;
            string key;

            int cancelled = 0, folioDoc = 0, currencyId = 0, companyId = 0;
            double saldoPendiente = 0, totalDoc = 0;
            StringBuilder sbFechaDoco = new StringBuilder(9);
            StringBuilder sbFechaCobro = new StringBuilder(9);
            StringBuilder sbFechaVenc = new StringBuilder(9);
            StringBuilder sbCompanyName = new StringBuilder(61);
            StringBuilder sbSerieDoc = new StringBuilder(12);
            StringBuilder sbTipoCobro = new StringBuilder(51);
            StringBuilder sbObservations = new StringBuilder(51);
            string sFechaDoco, sFechaCobro, sFechaVenc, sSerieDoc, sTipoCobro, sObservations, sCompanyName;

            Company sourceCompany = source.Company;
            if (sourceCompany == null)
            {
                throw new Exception("Error en la configuración de empresa.");
            }

            key = source.ApId.ToString().PadLeft(11);
            dbResponse = AdminPaqLib.dbGet(connection, TableNames.DOCUMENTOS, IndexNames.PRIMARY_KEY, key);

            if (dbResponse != 0)
            {
                PgDbCollector.UpdateAccountSimple(source, true);
                return;
            }

            if (dbResponse == 0)
            {
                // Obtener datos de la cuenta
                fieldResponse = AdminPaqLib.dbFieldLong(connection, TableNames.DOCUMENTOS, 26, ref cancelled);
                isCancelled = cancelled != 0;

                fieldResponse = AdminPaqLib.dbFieldDouble(connection, TableNames.DOCUMENTOS, 44, ref saldoPendiente);

                fieldResponse = AdminPaqLib.dbFieldChar(connection, TableNames.DOCUMENTOS, 6, sbFechaDoco, 9);
                fieldResponse = AdminPaqLib.dbFieldChar(connection, TableNames.DOCUMENTOS, 56, sbFechaCobro, 9);

                sFechaDoco = sbFechaDoco.ToString().Substring(0, 8);
                sFechaCobro = sbFechaCobro.ToString().Substring(0, 8);

                source.DocDate = DateTime.ParseExact(sFechaDoco, IndexNames.DATE_FORMAT_PATTERN, CultureInfo.InvariantCulture);

                if (!string.Empty.Equals(sFechaCobro.Trim()) && !"18991230".Equals(sFechaCobro.Trim()))
                    source.CollectDate = DateTime.ParseExact(sFechaCobro, IndexNames.DATE_FORMAT_PATTERN, CultureInfo.InvariantCulture);

                source.Balance = saldoPendiente;
                if (saldoPendiente == 0) isCancelled = true;

                fieldResponse = AdminPaqLib.dbFieldChar(connection, TableNames.DOCUMENTOS, 11, sbFechaVenc, 9);
                sFechaVenc = sbFechaVenc.ToString().Substring(0, 8).Trim();
                source.DueDate = DateTime.ParseExact(sFechaVenc, IndexNames.DATE_FORMAT_PATTERN, CultureInfo.InvariantCulture);

                fieldResponse = AdminPaqLib.dbFieldChar(connection, TableNames.DOCUMENTOS, 4, sbSerieDoc, 12);
                sSerieDoc = sbSerieDoc.ToString().Substring(0, 11).Trim();
                source.Serie = sSerieDoc;

                fieldResponse = AdminPaqLib.dbFieldLong(connection, TableNames.DOCUMENTOS, 5, ref folioDoc);
                source.Folio = folioDoc;

                fieldResponse = AdminPaqLib.dbFieldDouble(connection, TableNames.DOCUMENTOS, 43, ref totalDoc);
                source.Amount = totalDoc;

                fieldResponse = AdminPaqLib.dbFieldChar(connection, TableNames.DOCUMENTOS, 53, sbTipoCobro, 51);
                sTipoCobro = sbTipoCobro.ToString().Substring(0, 50).Trim();
                source.CollectType = sTipoCobro;

                fieldResponse = AdminPaqLib.dbFieldChar(connection, TableNames.DOCUMENTOS, 54, sbObservations, 51);
                sObservations = sbObservations.ToString().Substring(0, 50).Trim();
                source.Note = sObservations;

                fieldResponse = AdminPaqLib.dbFieldLong(connection, TableNames.DOCUMENTOS, 15, ref currencyId);
                source.Currency = CurrencyName(currencyId, sourceCompany.EnterprisePath);


                fieldResponse = AdminPaqLib.dbFieldLong(connection, TableNames.DOCUMENTOS, 7, ref companyId);
                fieldResponse = AdminPaqLib.dbFieldChar(connection, TableNames.DOCUMENTOS, 8, sbCompanyName, 61);
                sCompanyName = sbCompanyName.ToString().Substring(0, 60).Trim();

                source.Company.ApId = companyId;
                source.Company.Name = sCompanyName;
                source.Company.EnterpriseId = sourceCompany.EnterpriseId;
                source.Company.EnterprisePath = sourceCompany.EnterprisePath;

                FillCompany(source.Company, sourceCompany.EnterprisePath);

                if(!isCancelled)
                    FillPayments(source, sourceCompany.EnterprisePath, conceptosAbono);

                if ("".Equals(source.Note) && "".Equals(source.CollectType) && source.CollectDate == null)
                {
                    try
                    {
                        PgDbCollector.UpdateAccountSimple(source, isCancelled);
                    }
                    catch (Exception ex)
                    {
                        ErrLogger.Log(ex.StackTrace);
                    }
                }
                else
                {
                    try
                    {
                        PgDbCollector.UpdateAccount(source, isCancelled);
                    }
                    catch (Exception ex)
                    {
                        ErrLogger.Log(ex.StackTrace);
                    }
                }
                    

                if (!isCancelled)
                {
                    foreach (Payment pay in source.Payments)
                    {
                        pay.DocId = source.DocId;
                        try 
                        {
                            PgDbCollector.SavePayment(pay);
                        }
                        catch(Exception ex)
                        {
                            ErrLogger.Log(ex.StackTrace);
                        }
                        
                    }
                }

            }
        }

        public static void DownloadCollectables(string[] conceptosFactura, string[] conceptosAbono, Empresa configuredCompany, EventLog log)
        {
            int counter = 0;
            int connDocos, dbResponse, fieldResponse, collectableDocType = 4;

            int docType = collectableDocType, cancelled = 0, conceptId = 0, docId = 0, folioDoc = 0, currencyId = 0, companyId = 0;
            double saldoPendiente = 0, totalDoc = 0;
            StringBuilder sbFechaDoco = new StringBuilder(9);
            StringBuilder sbFechaVenc = new StringBuilder(9);
            StringBuilder sbCompanyName = new StringBuilder(61);
            StringBuilder sbSerieDoc = new StringBuilder(12);
            StringBuilder sbTipoCobro = new StringBuilder(51);
            StringBuilder sbObservations = new StringBuilder(51);
            string sFechaDoco, sFechaVenc, sCompanyName, sSerieDoc, currencyName;

            bool isFactura = false;

            Dictionary<int, string> currencies = new Dictionary<int, string>();
            Dictionary<int, Company> companies = new Dictionary<int, Company>();
            Company documentCo;

            if (configuredCompany == null)
            {
                log.WriteEntry("Wrong Company configuration.");
                throw new Exception("La empresa no ha sido correctamente configurada para realizar esta acción.");
            }

            connDocos = AdminPaqLib.dbLogIn("", configuredCompany.Ruta);
            if (connDocos == 0)
            {
                log.WriteEntry("Unable to open connection to documents table for company [" + configuredCompany.Nombre + "]");
                throw new Exception("No se pudo establecer conexión con adminPaq en la siguiente ruta: " + configuredCompany.Ruta);
            }

            dbResponse = AdminPaqLib.dbGetTopNoLock(connDocos, TableNames.DOCUMENTOS, IndexNames.PRIMARY_KEY);
            
            if (dbResponse != 0)
            {
                log.WriteEntry("No data found for company: " + configuredCompany.Nombre);
                AdminPaqLib.dbLogOut(connDocos);
            }


            while (dbResponse == 0)
            {
                fieldResponse = AdminPaqLib.dbFieldLong(connDocos, TableNames.DOCUMENTOS, 1, ref docId);
                fieldResponse = AdminPaqLib.dbFieldLong(connDocos, TableNames.DOCUMENTOS, 26, ref cancelled);
                if (cancelled != 0)
                {
                    dbResponse = AdminPaqLib.dbSkip(connDocos, TableNames.DOCUMENTOS, IndexNames.PRIMARY_KEY, 1);
                    continue;
                }

                fieldResponse = AdminPaqLib.dbFieldDouble(connDocos, TableNames.DOCUMENTOS, 44, ref saldoPendiente);

                if (saldoPendiente <= 0)
                {
                    dbResponse = AdminPaqLib.dbSkip(connDocos, TableNames.DOCUMENTOS, IndexNames.PRIMARY_KEY, 1);
                    continue;
                }

                fieldResponse = AdminPaqLib.dbFieldChar(connDocos, TableNames.DOCUMENTOS, 6, sbFechaDoco, 9);
                sFechaDoco = sbFechaDoco.ToString().Substring(0, 8);

                fieldResponse = AdminPaqLib.dbFieldLong(connDocos, TableNames.DOCUMENTOS, 3, ref conceptId);
                DocumentConcept concept = GetDocumentConcept(conceptId, configuredCompany.Ruta);
                if (concept == null)
                {
                    dbResponse = AdminPaqLib.dbSkip(connDocos, TableNames.DOCUMENTOS, IndexNames.PRIMARY_KEY, 1);
                    continue;
                }

                isFactura = conceptosFactura.Contains(concept.Code);

                if (!isFactura)
                {
                    dbResponse = AdminPaqLib.dbSkip(connDocos, TableNames.DOCUMENTOS, IndexNames.PRIMARY_KEY, 1);
                    continue;
                }

                Account account = new Account();
                account.DocDate = DateTime.ParseExact(sFechaDoco, IndexNames.DATE_FORMAT_PATTERN, CultureInfo.InvariantCulture);

                account.Balance = saldoPendiente;
                account.DocType = concept.Name;

                account.ApId = docId;

                fieldResponse = AdminPaqLib.dbFieldChar(connDocos, TableNames.DOCUMENTOS, 11, sbFechaVenc, 9);
                sFechaVenc = sbFechaVenc.ToString().Substring(0, 8).Trim();
                account.DueDate = DateTime.ParseExact(sFechaVenc, IndexNames.DATE_FORMAT_PATTERN, CultureInfo.InvariantCulture);

                fieldResponse = AdminPaqLib.dbFieldChar(connDocos, TableNames.DOCUMENTOS, 4, sbSerieDoc, 12);
                sSerieDoc = sbSerieDoc.ToString().Substring(0, 11).Trim();
                account.Serie = sSerieDoc;

                fieldResponse = AdminPaqLib.dbFieldLong(connDocos, TableNames.DOCUMENTOS, 5, ref folioDoc);
                account.Folio = folioDoc;

                fieldResponse = AdminPaqLib.dbFieldDouble(connDocos, TableNames.DOCUMENTOS, 43, ref totalDoc);
                account.Amount = totalDoc;

                fieldResponse = AdminPaqLib.dbFieldLong(connDocos, TableNames.DOCUMENTOS, 15, ref currencyId);
                if (!currencies.ContainsKey(currencyId))
                {
                    currencyName = CurrencyName(currencyId, configuredCompany.Ruta);
                    if (currencyName != null)
                        currencies.Add(currencyId, currencyName);
                }
                account.Currency = currencies[currencyId];

                fieldResponse = AdminPaqLib.dbFieldLong(connDocos, TableNames.DOCUMENTOS, 7, ref companyId);

                if (!companies.ContainsKey(companyId))
                {
                    fieldResponse = AdminPaqLib.dbFieldChar(connDocos, TableNames.DOCUMENTOS, 8, sbCompanyName, 61);
                    sCompanyName = sbCompanyName.ToString().Substring(0, 60).Trim();

                    documentCo = new Company();
                    documentCo.ApId = companyId;
                    documentCo.Name = sCompanyName;
                    documentCo.EnterpriseId = configuredCompany.Id;
                    documentCo.EnterprisePath = configuredCompany.Ruta;
                    FillCompany(documentCo, configuredCompany.Ruta);

                    companies.Add(companyId, documentCo);
                }

                account.Company = companies[companyId];
                FillPayments(account, configuredCompany.Ruta, conceptosAbono);

                if (!PgDbCollector.AccountExists(account))
                {
                    try
                    {
                        PgDbCollector.AddAccount(account, log);
                        counter++;
                    }
                    catch (Exception dbEx)
                    {
                        log.WriteEntry("Unable to save account: " + dbEx.Message + " || " + dbEx.StackTrace, EventLogEntryType.Warning);
                    }
                }

                dbResponse = AdminPaqLib.dbSkip(connDocos, TableNames.DOCUMENTOS, IndexNames.PRIMARY_KEY, 1);
            }
            currencies.Clear();
            currencies = null;

            companies.Clear();
            companies = null;

            AdminPaqLib.dbLogOut(connDocos);
            log.WriteEntry("Added " + counter.ToString() + " accounts", EventLogEntryType.Information, 11, 2);
        }


        public void FillMonitors(Empresa empresa, string[] conceptosFactura, string[] conceptosAbono, string[] codigosVenta, 
            string[] codigosDevolucion, EventLog log)
        {
            int connDocos, dbResponse, fieldResponse;
            DateTime today = DateTime.Today;
            DateTime fromDate = today.AddYears(-1);
            DateTime toDate = today;
            DateTime dueDate = today;
            string sFromDate = fromDate.ToString("yyyyMMdd");

            Dictionary<int, string> currencies = new Dictionary<int, string>();
            Dictionary<int, DimClientes> customers = new Dictionary<int, DimClientes>();

            int cancelado = 0, devuelto = 0, impreso = 0, conceptId = 0, companyId = 0, currencyId = 0, idAgente = 0;
            StringBuilder sbFechaDoc = new StringBuilder(9);
            StringBuilder sbFechaVto = new StringBuilder(9);
            string sFechaDoc, sFechaVto, companyCode, currencyName;
            double saldo = 0, cambio = 0;

            bool esVenta = false, esDevolucion = false, esCredito = false, esPago = false;

            string TABLE_NAME = TableNames.DOCUMENTOS, INDEX = IndexNames.PRIMARY_KEY;


            connDocos = AdminPaqLib.dbLogIn("", empresa.Ruta);
            if (connDocos == 0)
            {
                throw new Exception("Unable to work with route [" + empresa.Ruta + "]");
            }

            dbResponse = AdminPaqLib.dbGetTopNoLock(connDocos, TABLE_NAME, INDEX);


            log.WriteEntry("GENERANDO DATOS DE VENTAS DESDE ADMINPAQ");

            if (dbResponse != 0)
                log.WriteEntry("NO FUE POSIBLE ENCONTRAR NINGúN REGISTRO, LA BASE DE DATOS ANUNCIó EL ERROR: "
                    + dbResponse.ToString(), EventLogEntryType.Warning);

            int cancelados = 0, devueltos = 0, otros = 0, no_impresos = 0, no_co = 0, no_cliente = 0, valido = 0;
            while (dbResponse == 0)
            {
                esVenta = false;
                esDevolucion = false;
                esCredito = false;
                esPago = false;

                AdminPaqLib.dbFieldLong(connDocos, TABLE_NAME, 26, ref cancelado);
                if (cancelado != 0)
                {
                    dbResponse = AdminPaqLib.dbSkip(connDocos, TABLE_NAME, INDEX, 1);
                    cancelados++;
                    continue;
                }

                AdminPaqLib.dbFieldLong(connDocos, TABLE_NAME, 27, ref devuelto);
                if (devuelto != 0)
                {
                    dbResponse = AdminPaqLib.dbSkip(connDocos, TABLE_NAME, INDEX, 1);
                    devueltos++;
                    continue;
                }

                AdminPaqLib.dbFieldLong(connDocos, TABLE_NAME, 3, ref conceptId);

                DocumentConcept concept = GetDocumentConcept(conceptId, empresa.Ruta);
                if (concept == null)
                {
                    dbResponse = AdminPaqLib.dbSkip(connDocos, TABLE_NAME, INDEX, 1);
                    otros++;
                    continue;
                }

                esVenta = codigosVenta != null && codigosVenta.Contains(concept.Code);
                esDevolucion = codigosDevolucion != null && codigosDevolucion.Contains(concept.Code);
                esCredito = conceptosFactura.Contains(concept.Code);
                esPago = conceptosAbono.Contains(concept.Code);

                if (!esVenta && !esDevolucion && !esCredito && !esPago)
                {
                    dbResponse = AdminPaqLib.dbSkip(connDocos, TABLE_NAME, INDEX, 1);
                    otros++;
                    continue;
                }

                if (esVenta)
                {
                    AdminPaqLib.dbFieldLong(connDocos, TABLE_NAME, 25, ref impreso);
                    if (impreso == 0)
                    {
                        dbResponse = AdminPaqLib.dbSkip(connDocos, TABLE_NAME, INDEX, 1);
                        no_impresos++;
                        continue;
                    }
                }

                if (esCredito)
                    AdminPaqLib.dbFieldDouble(connDocos, TABLE_NAME, 44, ref saldo);
                else
                    AdminPaqLib.dbFieldDouble(connDocos, TABLE_NAME, 31, ref saldo);

                AdminPaqLib.dbFieldLong(connDocos, TABLE_NAME, 15, ref currencyId);
                if (!currencies.ContainsKey(currencyId))
                {
                    currencyName = CurrencyName(currencyId, empresa.Ruta);
                    if (currencyName != null)
                        currencies.Add(currencyId, currencyName);
                }

                currencyName = currencies[currencyId];

                if (!currencyName.ToUpper().Contains("PESO"))
                {
                    AdminPaqLib.dbFieldDouble(connDocos, TABLE_NAME, 16, ref cambio);
                    saldo = saldo * cambio;
                }

                if (esDevolucion)
                    saldo = saldo * -1;

                AdminPaqLib.dbFieldChar(connDocos, TABLE_NAME, 6, sbFechaDoc, 9);
                sFechaDoc = sbFechaDoc.ToString().Substring(0, 8).Trim();

                AdminPaqLib.dbFieldLong(connDocos, TABLE_NAME, 10, ref idAgente);

                AdminPaqDocument doco = new AdminPaqDocument();
                doco.IsCredit = esCredito;
                doco.IsSale = esVenta || esDevolucion;
                doco.IsPayment = esPago;
                doco.Amount = saldo;
                doco.SellerId = idAgente;
                doco.DocumentDate = DateTime.ParseExact(sFechaDoc, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);

                // SI ES CREDITO NECESITO LA FECHA DE VTO.
                if (esCredito)
                {
                    AdminPaqLib.dbFieldChar(connDocos, TABLE_NAME, 11, sbFechaVto, 9);
                    sFechaVto = sbFechaVto.ToString().Substring(0, 8).Trim();

                    doco.DueDate = DateTime.ParseExact(sFechaVto, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);

                    // detect client assignment
                    fieldResponse = AdminPaqLib.dbFieldLong(connDocos, TABLE_NAME, 7, ref companyId);

                    if (!customers.ContainsKey(companyId))
                    {
                        companyCode = GetCompanyCode(companyId, empresa.Ruta);

                        if (companyCode == null)
                        {
                            dbResponse = AdminPaqLib.dbSkip(connDocos, TABLE_NAME, INDEX, 1);
                            no_co++;
                            continue;
                        }
                        DimClientes cliente = DimClientes.GetCliente(empresa.Id, companyCode, null);
                        if (cliente == null)
                        {
                            dbResponse = AdminPaqLib.dbSkip(connDocos, TABLE_NAME, INDEX, 1);
                            no_cliente++;
                            continue;
                        }
                        customers.Add(companyId, cliente);
                    }

                    DimClientes customer = customers[companyId];
                    doco.Client = customer;
                }

                OrganizeDoco(doco, log);
                valido++;

                dbResponse = AdminPaqLib.dbSkip(connDocos, TABLE_NAME, INDEX, 1);
            }

            log.WriteEntry("Resumen de colección de datos: cancelados=" + cancelados + ";devueltos=" + devueltos + ";otros=" + otros + ";no_impresos=" + no_impresos + ";no_co=" + no_co + ";no_cliente=" + no_cliente + ";valido=" + valido);
            AdminPaqLib.dbLogOut(connDocos);
        }


        private void OrganizeDoco(AdminPaqDocument document, EventLog log)
        {
            if (document.IsCredit)
            {
                bool due = document.DueDate.CompareTo(DateTime.Today) < 0;
                if (due)
                    MergeDueCreditDocument(document);
                else
                    MergeNotDueCreditDocument(document);
            }

            if (document.IsPayment)
            {
                MergeCollected(document);
            }

            if (document.IsSale)
            {
                MergeSale(document, log);
            }

        }

        private void MergeSale(AdminPaqDocument document, EventLog log)
        {
            MergeAgentSale(document, log);
            MergeCollectedSale(document);
        }

        private void MergeAgentSale(AdminPaqDocument document, EventLog log)
        {
            bool found = false;
            int monthDeltaDays = (DateTime.Today.Day - 1) * -1;
            DateTime BOM = DateTime.Today.AddDays(monthDeltaDays);

            if (document.DocumentDate.CompareTo(BOM) < 0)
                return;

            int weekStartDelta = 1 - (int)DateTime.Today.DayOfWeek;
            DateTime weekStart = DateTime.Today.AddDays(weekStartDelta);

            foreach (FactSales sale in Ventas)
            {
                if (sale.Seller.ApId == document.SellerId)
                {
                    if (document.DocumentDate.Month == DateTime.Today.Month)
                        sale.SoldMonth += document.Amount;

                    if (document.DocumentDate.CompareTo(weekStart) >= 0)
                        sale.SoldWeek += document.Amount;

                    if (document.DocumentDate.CompareTo(DateTime.Today) == 0)
                        sale.SoldToday += document.Amount;

                    found = true;
                    break;
                }
            }
            if (!found)
                log.WriteEntry("No se encontró el agente de venta al cual pertenece uno de los registros: " + document.SellerId, EventLogEntryType.Warning);
        }

        public void MergeCollectedSale(AdminPaqDocument document)
        {
            foreach (FactCobranza fact in Cobranza)
            {
                if (document.DocumentDate.Month != (int)fact.Month.IndiceMes) continue;
                if (document.DocumentDate.Year != fact.Month.YYYY) continue;

                fact.Sold += document.Amount;
            }
        }

        private void MergeCollected(AdminPaqDocument document)
        {
            foreach (FactCobranza fact in Cobranza)
            {
                if (document.DocumentDate.Month != (int)fact.Month.IndiceMes) continue;
                if (document.DocumentDate.Year != fact.Month.YYYY) continue;

                fact.Collected += document.Amount;
                break;
            }
        }

        private void MergeDueCreditDocument(AdminPaqDocument document)
        {
            foreach (FactVencido fact in Vencidos)
            {
                if (fact.Cliente.CodigoCliente == document.Client.CodigoCliente && fact.Cliente.IdEmpresa == document.Client.IdEmpresa)
                {
                    int startFac = fact.GrupoVencimiento.Inicio == 0 ? 1 : fact.GrupoVencimiento.Inicio;
                    DateTime startFactDate = DateTime.Today.AddDays(-startFac);
                    DateTime endFactDate = DateTime.Today.AddDays(-fact.GrupoVencimiento.Fin);

                    bool InfiniteGroup = fact.GrupoVencimiento.Fin == 0;
                    bool BeforeGroupStartDate = document.DueDate.CompareTo(startFactDate) <= 0;
                    bool AfterGroupEndDate = document.DueDate.CompareTo(endFactDate) >= 0;

                    if ((InfiniteGroup && BeforeGroupStartDate)
                        || (AfterGroupEndDate && BeforeGroupStartDate))
                    {
                        fact.Saldo = fact.Saldo + document.Amount;
                    }
                }
            }
        }

        private void MergeNotDueCreditDocument(AdminPaqDocument document)
        {
            foreach (FactPorVencer fact in PorVencer)
            {
                if (fact.Cliente.CodigoCliente == document.Client.CodigoCliente && fact.Cliente.IdEmpresa == document.Client.IdEmpresa)
                {
                    DateTime startFactDate = DateTime.Today.AddDays(fact.GrupoVencimiento.Inicio);
                    DateTime endFactDate = DateTime.Today.AddDays(fact.GrupoVencimiento.Fin);

                    bool InfiniteGroup = fact.GrupoVencimiento.Fin == 0;
                    bool AfterGroupStartDate = document.DueDate.CompareTo(startFactDate) >= 0;
                    bool BeforeGroupEndDate = document.DueDate.CompareTo(endFactDate) <= 0;

                    if ((InfiniteGroup && AfterGroupStartDate)
                        || (BeforeGroupEndDate && AfterGroupStartDate))
                    {
                        fact.Saldo = fact.Saldo + document.Amount;
                    }
                }
            }
        }

        private static string CurrencyName(int currencyId, string filePath)
        {
            int connCurrency, dbResponse, fqResponse;
            StringBuilder sbCurrencyName = new StringBuilder(61);
            string key, sCurrencyName = String.Empty;

            connCurrency = AdminPaqLib.dbLogIn("", filePath);
            if (connCurrency == 0)
            {
                ErrLogger.Log("Unable to open connection for currency table from [" + filePath + "]");
                return null;
            }

            key = currencyId.ToString().PadLeft(11);
            dbResponse = AdminPaqLib.dbGetNoLock(connCurrency, TableNames.MONEDAS, IndexNames.PRIMARY_KEY, key);

            if (dbResponse == 0)
            {
                fqResponse = AdminPaqLib.dbFieldChar(connCurrency, TableNames.MONEDAS, 6, sbCurrencyName, 61);
                sCurrencyName = sbCurrencyName.ToString().Substring(0, 60).Trim();
            }
            AdminPaqLib.dbLogOut(connCurrency);
            return sCurrencyName;
        }

        private static void FillPayments(Account account, string filePath, string[] conceptosAbono)
        {
            int connPayments, dbResponse, fqResponse;
            string key;

            int docId = 0;
            double amount = 0;

            connPayments = AdminPaqLib.dbLogIn("", filePath);
            if (connPayments == 0)
            {
                ErrLogger.Log("Unable to open connection for Payments");
                return;
            }

            key = account.ApId.ToString().PadLeft(11);
            dbResponse = AdminPaqLib.dbGetNoLock(connPayments, TableNames.ABONOS_CARGOS, IndexNames.ABONOS_DOCUMENTOS, key);
            while (dbResponse == 0)
            {
                fqResponse = AdminPaqLib.dbFieldLong(connPayments, TableNames.ABONOS_CARGOS, 2, ref docId);
                if (docId != account.ApId) break;

                fqResponse = AdminPaqLib.dbFieldLong(connPayments, TableNames.ABONOS_CARGOS, 1, ref docId);
                fqResponse = AdminPaqLib.dbFieldDouble(connPayments, TableNames.ABONOS_CARGOS, 3, ref amount);

                AddPayment(account, filePath, docId, conceptosAbono, amount);

                dbResponse = AdminPaqLib.dbSkip(connPayments, TableNames.ABONOS_CARGOS, IndexNames.ABONOS_DOCUMENTOS, 1);
            }

            AdminPaqLib.dbLogOut(connPayments);
        }

        private static void AddPayment(Account account, string filePath, int docId, string[] conceptosAbono, double importe)
        {
            int connPayment, dbResponse, fqResponse;
            string key;

            StringBuilder sbTipoPago = new StringBuilder(21);
            StringBuilder sbDepositDate = new StringBuilder(9);
            StringBuilder sbDepositAccount = new StringBuilder(51);

            string sDepositDate;

            int folio = 0;

            bool isAbono = false;
            int idPaymentConcept = 0;

            connPayment = AdminPaqLib.dbLogIn("", filePath);
            if (connPayment == 0)
            {
                ErrLogger.Log("Unable to open connection for single payment");
                return;
            }

            key = docId.ToString().PadLeft(11);
            dbResponse = AdminPaqLib.dbGetNoLock(connPayment, TableNames.DOCUMENTOS, IndexNames.PRIMARY_KEY, key);

            if (dbResponse == 0)
            {
                fqResponse = AdminPaqLib.dbFieldLong(connPayment, TableNames.DOCUMENTOS, 3, ref idPaymentConcept);
                DocumentConcept concept = GetDocumentConcept(idPaymentConcept, filePath);

                if (concept == null) return;

                isAbono = conceptosAbono.Contains(concept.Code);
                if (!isAbono) return;

                Payment payment = new Payment();
                payment.Concept = concept.Name;
                payment.DocId = account.DocId;
                payment.PaymentId = docId;
                payment.Amount = importe;

                fqResponse = AdminPaqLib.dbFieldChar(connPayment, TableNames.DOCUMENTOS, 17, sbTipoPago, 21);
                payment.PaymentType = sbTipoPago.ToString().Substring(0, 20).Trim();

                fqResponse = AdminPaqLib.dbFieldLong(connPayment, TableNames.DOCUMENTOS, 5, ref folio);
                payment.Folio = folio;

                fqResponse = AdminPaqLib.dbFieldChar(connPayment, TableNames.DOCUMENTOS, 6, sbDepositDate, 9);
                sDepositDate = sbDepositDate.ToString().Substring(0, 8).Trim();
                if (!string.Empty.Equals(sDepositDate))
                    payment.DepositDate = DateTime.ParseExact(sDepositDate, IndexNames.DATE_FORMAT_PATTERN, CultureInfo.InvariantCulture);

                fqResponse = AdminPaqLib.dbFieldChar(connPayment, TableNames.DOCUMENTOS, 53, sbDepositAccount, 51);
                payment.Account = sbDepositAccount.ToString().Substring(0, 50).Trim();
                account.Payments.Add(payment);
            }

            AdminPaqLib.dbLogOut(connPayment);
        }

        private static DocumentConcept GetDocumentConcept(int docId, string filePath)
        {
            DocumentConcept response = null;
            int connDocos, dbResponse, fqResponse;
            StringBuilder sConceptCode = new StringBuilder(31);
            StringBuilder sConceptName = new StringBuilder(61);
            string key, conceptCode, conceptName;

            connDocos = AdminPaqLib.dbLogIn("", filePath);
            if (connDocos == 0)
            {
                ErrLogger.Log("Unable to open connection to concepts table from [" + filePath + "]");
                return response;
            }

            key = docId.ToString().PadLeft(11);
            dbResponse = AdminPaqLib.dbGetNoLock(connDocos, TableNames.CONCEPTOS_DOCUMENTOS, IndexNames.PRIMARY_KEY, key);

            if (dbResponse == 0)
            {
                fqResponse = AdminPaqLib.dbFieldChar(connDocos, TableNames.CONCEPTOS_DOCUMENTOS, 2, sConceptCode, 31);
                conceptCode = sConceptCode.ToString().Substring(0, 30).Trim();
                fqResponse = AdminPaqLib.dbFieldChar(connDocos, TableNames.CONCEPTOS_DOCUMENTOS, 3, sConceptName, 61);
                conceptName = sConceptName.ToString().Substring(0, 60).Trim();

                response = new DocumentConcept();
                response.Id = docId;
                response.Code = conceptCode;
                response.Name = conceptName;
            }

            AdminPaqLib.dbLogOut(connDocos);
            return response;
        }

        private static void FillCompany(Company company, string filePath)
        {
            int connCompany, dbResponse, fqResponse, ubicacion = 0;
            int agentId = 0;
            StringBuilder sbCompanyCode = new StringBuilder(31);
            StringBuilder sbPaymentDay = new StringBuilder(51);
            string key;

            connCompany = AdminPaqLib.dbLogIn("", filePath);
            if (connCompany == 0)
            {
                ErrLogger.Log("Unable to open connection for company");
                return;
            }

            key = company.ApId.ToString().PadLeft(11);
            dbResponse = AdminPaqLib.dbGetNoLock(connCompany, TableNames.CLIENTES_PROVEEDORES, IndexNames.PRIMARY_KEY, key);

            if (dbResponse == 0)
            {
                fqResponse = AdminPaqLib.dbFieldChar(connCompany, TableNames.CLIENTES_PROVEEDORES, 2, sbCompanyCode, 31);
                company.Code = sbCompanyCode.ToString().Substring(0, 30).Trim();
                fqResponse = AdminPaqLib.dbFieldChar(connCompany, TableNames.CLIENTES_PROVEEDORES, 106, sbPaymentDay, 51);
                company.PaymentDay = sbPaymentDay.ToString().Substring(0, 50).Trim();
                fqResponse = AdminPaqLib.dbFieldLong(connCompany, TableNames.CLIENTES_PROVEEDORES, 37, ref agentId);
                company.AgentCode = AgentCode(agentId, filePath);
                fqResponse = AdminPaqLib.dbFieldLong(connCompany, TableNames.CLIENTES_PROVEEDORES, 14, ref ubicacion);

                /*FORANEO=2/LOCAL=1*/
                company.EsLocal = ubicacion == 1;
            }

            AdminPaqLib.dbLogOut(connCompany);

        }

        private string GetCompanyCode(int companyId, string filePath)
        {
            int connCompany, dbResponse, fqResponse;
            StringBuilder sbCompanyCode = new StringBuilder(31);
            string key, result = null;

            connCompany = AdminPaqLib.dbLogIn("", filePath);

            if (connCompany == 0) return null;

            key = companyId.ToString().PadLeft(11);

            dbResponse = AdminPaqLib.dbGetNoLock(connCompany, "MGW10002", "PRIMARYKEY", key);

            if (dbResponse == 0)
            {
                fqResponse = AdminPaqLib.dbFieldChar(connCompany, "MGW10002", 2, sbCompanyCode, 31);
                result = sbCompanyCode.ToString().Substring(0, 30).Trim();
            }

            AdminPaqLib.dbLogOut(connCompany);
            return result;
        }

    }

    public class AdminPaqDocument
    {
        public Boolean IsSale { get; set; }
        public Boolean IsCredit { get; set; }
        public Boolean IsPayment { get; set; }
        public double Amount { get; set; }
        public int SellerId { get; set; }
        public DateTime DocumentDate { get; set; }
        public DateTime DueDate { get; set; }
        public DimClientes Client { get; set; }
    }
}
