using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using CommonAdminPaq;
using System.Globalization;

namespace ConsolaCobranza.Collectable
{
    public class AdminPaqImpl
    {

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
                log.WriteEntry("No data found");
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

        public static void DownloadCollectable(Account source, string[] conceptosAbono)
        {
            bool isCancelled = false;
            int connection, dbResponse, fieldResponse;
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

            connection = AdminPaqLib.dbLogIn("", sourceCompany.EnterprisePath);
            if (connection == 0)
            {
                throw new Exception("No fue posible establecer la conexión con la base de datos de la empresa en la ruta: " + sourceCompany.EnterprisePath);
            }

            key = source.ApId.ToString().PadLeft(11);
            dbResponse = AdminPaqLib.dbGet(connection, TableNames.DOCUMENTOS, IndexNames.PRIMARY_KEY, key);

            if (dbResponse != 0)
            {
                AdminPaqLib.dbLogOut(connection);
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

            AdminPaqLib.dbLogOut(connection);
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
    }
}
