using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonAdminPaq;
using SeguimientoSuper.Properties;
using System.Globalization;

namespace SeguimientoSuper.Collectable
{
    public class AdminPaqImp
    {
        private AdminPaqLib lib;
        private IList<Empresa> empresas = new List<Empresa>();
        private List<int> cancelados = new List<int>();
        private List<int> saldados = new List<int>();
        private Dictionary<int, Concepto> conceptos = new Dictionary<int, Concepto>();

        public IList<Empresa> Empresas { get { return empresas; } set { empresas = value; } }
        public List<int> Cancelados { get { return cancelados; } }
        public List<int> Saldados { get { return saldados; } }
        public Dictionary<int, Concepto> Conceptos { get { return conceptos; } }

        public AdminPaqImp()
        {
            lib = new AdminPaqLib();
            lib.SetDllFolder();

            empresas = new List<Empresa>();
            Empresa defEmpresa = new Empresa();

            defEmpresa.Id = 0;
            defEmpresa.Nombre = "Seleccione Empresa";

            empresas.Add(defEmpresa);

            InitializeSDK();
        }

        public void SetObservations(int docId, string collectType, string observations, string rutaEmpresa)
        {
            int connection, dbResponse;
            string key, command;
            
            connection = AdminPaqLib.dbLogIn("", rutaEmpresa);
            if (connection == 0)
            {
                ErrLogger.Log("Unable to open connection to documents table for company [" + rutaEmpresa + "]");
                return;
            }

            key = docId.ToString().PadLeft(11);
            dbResponse = AdminPaqLib.dbGet(connection, TableNames.DOCUMENTOS, IndexNames.PRIMARY_KEY, key);

            if (dbResponse == 0)
            {
                command = string.Format("UPDATE {0}(CTEXTOEX01=\"{1}\",CTEXTOEX02=\"{2}\");",
                        TableNames.DOCUMENTOS, collectType, observations);


                dbResponse = AdminPaqLib.dbCmdExec(connection, command);
                if (dbResponse != 0)
                {
                    dbResponse = AdminPaqLib.dbCmdExec(connection, "ROLLBACK;");
                    AdminPaqLib.dbLogOut(connection);
                    throw new Exception("No se pudo actualizar el registro");
                }
                else
                {
                    dbResponse = AdminPaqLib.dbCmdExec(connection, "COMMIT;");
                    if (dbResponse != 0)
                    {
                        dbResponse = AdminPaqLib.dbCmdExec(connection, "ROLLBACK;");
                        AdminPaqLib.dbLogOut(connection);
                        throw new Exception("No se pudo confirmar la actualización del registro.");
                    }
                }
            }
            else
            {
                AdminPaqLib.dbLogOut(connection);
                throw new Exception("El registro del documento se encuentra bloqueado por otro usuario. \n" +
                    "folio: " + docId + "\n" +
                    "ruta empresa: " + rutaEmpresa);
            }

            AdminPaqLib.dbLogOut(connection);
        }

        public void SetCollectDate(int docId, DateTime collectDate, string rutaEmpresa)
        {
            int connection, dbResponse;
            string key, command;

            connection = AdminPaqLib.dbLogIn("", rutaEmpresa);
            if (connection == 0)
            {
                ErrLogger.Log("Unable to open connection to documents table for company [" + rutaEmpresa + "]");
                return;
            }

            key = docId.ToString().PadLeft(11);
            dbResponse = AdminPaqLib.dbGet(connection, TableNames.DOCUMENTOS, IndexNames.PRIMARY_KEY, key);

            if (dbResponse == 0)
            {
                string sCollectDate = collectDate.ToString("yyyyMMdd");
                command = string.Format("UPDATE {0}(CFECHAEX01=\"{1}\");",
                    TableNames.DOCUMENTOS, sCollectDate);

                dbResponse = AdminPaqLib.dbCmdExec(connection, command);
                if (dbResponse != 0)
                {
                    dbResponse = AdminPaqLib.dbCmdExec(connection, "ROLLBACK;");
                    AdminPaqLib.dbLogOut(connection);
                    throw new Exception("No se pudo actualizar el registro");
                }
                else
                {
                    dbResponse = AdminPaqLib.dbCmdExec(connection, "COMMIT;");
                    if (dbResponse != 0)
                    {
                        dbResponse = AdminPaqLib.dbCmdExec(connection, "ROLLBACK;");
                        AdminPaqLib.dbLogOut(connection);
                        throw new Exception("No se pudo confirmar la actualización del registro.");
                    }
                }
            }
            else
            {
                AdminPaqLib.dbLogOut(connection);
                throw new Exception("El registro del documento se encuentra bloqueado por otro usuario. \n" +
                    "folio: " + docId + "\n" +
                    "ruta empresa: " + rutaEmpresa);
            }

            AdminPaqLib.dbLogOut(connection);
        }

        public void UpdateCollectable(Account account)
        {
            int connection, dbResponse;
            string key, command;

            connection = AdminPaqLib.dbLogIn("", account.Company.EnterprisePath);
            if (connection == 0)
            {
                ErrLogger.Log("Unable to open connection to documents table for company [" + account.Company.EnterprisePath + "]");
                return;
            }

            key = account.ApId.ToString().PadLeft(11);
            dbResponse = AdminPaqLib.dbGet(connection, TableNames.DOCUMENTOS, IndexNames.PRIMARY_KEY, key);

            if (dbResponse == 0)
            {
                if (account.CollectDate.Ticks > 0)
                {
                    string sCollectDate = account.CollectDate.ToString("yyyyMMdd");
                    command = string.Format("UPDATE {0}(CFECHAEX01=\"{1}\",CTEXTOEX01=\"{2}\",CTEXTOEX02=\"{3}\");",
                        TableNames.DOCUMENTOS, sCollectDate, account.CollectType, account.Note);
                }
                else
                {
                    command = string.Format("UPDATE {0}(CFECHAEX01=\"{1}\",CTEXTOEX01=\"{2}\",CTEXTOEX02=\"{3}\");",
                        TableNames.DOCUMENTOS, "18991230", account.CollectType, account.Note);
                }



                dbResponse = AdminPaqLib.dbCmdExec(connection, command);
                if (dbResponse != 0)
                {
                    dbResponse = AdminPaqLib.dbCmdExec(connection, "ROLLBACK;");
                    AdminPaqLib.dbLogOut(connection);
                    throw new Exception("No se pudo actualizar el registro");
                }
                else
                {
                    dbResponse = AdminPaqLib.dbCmdExec(connection, "COMMIT;");
                    if (dbResponse != 0)
                    {
                        dbResponse = AdminPaqLib.dbCmdExec(connection, "ROLLBACK;");
                        AdminPaqLib.dbLogOut(connection);
                        throw new Exception("No se pudo confirmar la actualización del registro.");
                    }
                }
            }
            else
            {
                AdminPaqLib.dbLogOut(connection);
                throw new Exception("El registro del documento se encuentra bloqueado por otro usuario. \n" +
                    "folio: " + account.ApId.ToString() + "\n" +
                    "ruta empresa: " + account.Company.EnterprisePath);
            }

            AdminPaqLib.dbLogOut(connection);

        }

        public List<Account> DownloadCollectables()
        {   
            Settings set = Settings.Default;
            return DownloadCollectables(set.fecha_inicio, set.fecha_fin);
        }

        public List<Account> DownloadCollectables(DateTime fromDate, DateTime toDate)
        {
            List<Account> result = new List<Account>();
            cancelados.Clear();
            conceptos.Clear();

            Settings set = Settings.Default;
            int connDocos, dbResponse, fieldResponse, collectableDocType = 4;
            string startDate, endDate, tipo_doc;

            int docType = collectableDocType, cancelled = 0, conceptId = 0, docId = 0, folioDoc = 0, currencyId = 0, companyId = 0;
            double saldoPendiente = 0, totalDoc = 0;
            StringBuilder sbFechaDoco = new StringBuilder(9);
            StringBuilder sbFechaCobro = new StringBuilder(9);
            StringBuilder sbFechaVenc = new StringBuilder(9);
            StringBuilder sbCompanyName = new StringBuilder(61);
            StringBuilder sbSerieDoc = new StringBuilder(12);
            StringBuilder sbTipoCobro = new StringBuilder(51);
            StringBuilder sbObservations = new StringBuilder(51);
            string sFechaDoco, sFechaCobro, sFechaVenc, sCompanyName, sSerieDoc, sTipoCobro, sObservations, currencyName;

            string[] conceptosFactura = set.facturas.Split(',');
            string[] conceptosAbono = set.abonos.Split(',');
            bool isFactura = false;

            Dictionary<int, string> currencies = new Dictionary<int, string>();
            Dictionary<int, Company> companies = new Dictionary<int, Company>();
            Company documentCo;
            Empresa configuredCompany = ConfiguredCompany();

            if (configuredCompany == null)
            {
                ErrLogger.Log("Wrong Company configuration.");
                return result;
            }

            connDocos = AdminPaqLib.dbLogIn("", configuredCompany.Ruta);
            if (connDocos == 0)
            {
                ErrLogger.Log("Unable to open connection to documents table for company [" + configuredCompany.Nombre + "]");
                return result;
            }

            startDate = fromDate.ToString(IndexNames.DATE_FORMAT_PATTERN);
            endDate = toDate.ToString(IndexNames.DATE_FORMAT_PATTERN);

            tipo_doc = collectableDocType.ToString().PadLeft(11);
            dbResponse = AdvanceConnectionIndex(tipo_doc, startDate, set.fecha_doco, connDocos);

            while (dbResponse == 0)
            {
                fieldResponse = AdminPaqLib.dbFieldLong(connDocos, TableNames.DOCUMENTOS, 2, ref docType);
                if (docType != collectableDocType) break;

                fieldResponse = AdminPaqLib.dbFieldLong(connDocos, TableNames.DOCUMENTOS, 1, ref docId);
                fieldResponse = AdminPaqLib.dbFieldLong(connDocos, TableNames.DOCUMENTOS, 26, ref cancelled);
                if (cancelled != 0)
                {
                    cancelados.Add(docId);
                    dbResponse = AdminPaqLib.dbSkip(connDocos, TableNames.DOCUMENTOS, IndexNames.DOCUMENTOS_ID_DOCUMENTO01, 1);
                    continue;
                }

                fieldResponse = AdminPaqLib.dbFieldDouble(connDocos, TableNames.DOCUMENTOS, 44, ref saldoPendiente);

                if (saldoPendiente <= 0 && set.con_saldo)
                {
                    saldados.Add(docId);
                    dbResponse = AdminPaqLib.dbSkip(connDocos, TableNames.DOCUMENTOS, IndexNames.DOCUMENTOS_ID_DOCUMENTO01, 1);
                    continue;
                }

                fieldResponse = AdminPaqLib.dbFieldChar(connDocos, TableNames.DOCUMENTOS, 6, sbFechaDoco, 9);
                fieldResponse = AdminPaqLib.dbFieldChar(connDocos, TableNames.DOCUMENTOS, 56, sbFechaCobro, 9);

                sFechaDoco = sbFechaDoco.ToString().Substring(0, 8);
                sFechaCobro = sbFechaCobro.ToString().Substring(0, 8);

                if (set.fecha_doco)
                {
                    if (!(sFechaDoco.CompareTo(startDate) >= 0 && sFechaDoco.CompareTo(endDate) <= 0))
                    {
                        dbResponse = AdminPaqLib.dbSkip(connDocos, TableNames.DOCUMENTOS, IndexNames.DOCUMENTOS_ID_DOCUMENTO01, 1);
                        continue;
                    }
                }
                else
                {
                    if (!(sFechaCobro.CompareTo(startDate) >= 0 && sFechaCobro.CompareTo(endDate) <= 0))
                    {
                        dbResponse = AdminPaqLib.dbSkip(connDocos, TableNames.DOCUMENTOS, IndexNames.DOCUMENTOS_ID_DOCUMENTO01, 1);
                        continue;
                    }
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

                if (!conceptos.ContainsKey(conceptId))
                {
                    Concepto concepto = new Concepto();
                    concepto.APId = conceptId;
                    concepto.Codigo = concept.Code;
                    concepto.Nombre = concept.Name;
                    concepto.IdEmpresa = configuredCompany.Id;
                    concepto.Razon = "COBRO";

                    conceptos.Add(conceptId, concepto);
                }

                Account account = new Account();
                account.DocDate = DateTime.ParseExact(sFechaDoco, IndexNames.DATE_FORMAT_PATTERN, CultureInfo.InvariantCulture);

                if (!string.Empty.Equals(sFechaCobro.Trim()) && !"18991230".Equals(sFechaCobro.Trim()))
                    account.CollectDate = DateTime.ParseExact(sFechaCobro, IndexNames.DATE_FORMAT_PATTERN, CultureInfo.InvariantCulture);

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

                fieldResponse = AdminPaqLib.dbFieldChar(connDocos, TableNames.DOCUMENTOS, 53, sbTipoCobro, 51);
                sTipoCobro = sbTipoCobro.ToString().Substring(0, 50).Trim();
                account.CollectType = sTipoCobro;

                fieldResponse = AdminPaqLib.dbFieldChar(connDocos, TableNames.DOCUMENTOS, 54, sbObservations, 51);
                sObservations = sbObservations.ToString().Substring(0, 50).Trim();
                account.Note = sObservations;

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
                result.Add(account);

                dbResponse = AdminPaqLib.dbSkip(connDocos, TableNames.DOCUMENTOS, IndexNames.DOCUMENTOS_ID_DOCUMENTO01, 1);
            }
            AdminPaqLib.dbLogOut(connDocos);
            return result;
        }

        public void DownloadCollectable(ref Account source, string[] conceptosAbono, out bool isCancelled)
        {
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

            Empresa configuredCompany = ConfiguredCompany();

            Dictionary<int, Company> companies = new Dictionary<int, Company>();

            if (configuredCompany == null)
            {
                ErrLogger.Log("Wrong Company configuration.");
                throw new Exception("Error en la configuración de empresa.");
            }

            connection = AdminPaqLib.dbLogIn("", configuredCompany.Ruta);
            if (connection == 0)
            {
                ErrLogger.Log("Unable to open connection to documents table for company [" + configuredCompany.Nombre + "]");
                throw new Exception("No fue posible establecer la conexión con la base de datos de la empresa.");
            }

            key = source.ApId.ToString().PadLeft(11);
            dbResponse = AdminPaqLib.dbGet(connection, TableNames.DOCUMENTOS, IndexNames.PRIMARY_KEY, key);

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
                source.Currency = CurrencyName(currencyId, configuredCompany.Ruta);


                fieldResponse = AdminPaqLib.dbFieldLong(connection, TableNames.DOCUMENTOS, 7, ref companyId);
                fieldResponse = AdminPaqLib.dbFieldChar(connection, TableNames.DOCUMENTOS, 8, sbCompanyName, 61);
                sCompanyName = sbCompanyName.ToString().Substring(0,60).Trim();

                source.Company = new Company();
                source.Company.ApId = companyId;
                source.Company.Name = sCompanyName;
                source.Company.EnterpriseId = configuredCompany.Id;
                source.Company.EnterprisePath = configuredCompany.Ruta;
                
                FillCompany(source.Company, configuredCompany.Ruta);
                FillPayments(source, configuredCompany.Ruta, conceptosAbono);
            }
            else
            {
                AdminPaqLib.dbLogOut(connection);
                throw new Exception("El registro del documento se encuentra bloqueado por otro usuario.");
            }

            AdminPaqLib.dbLogOut(connection);
        }

        private void FillPayments(Account account, string filePath, string[] conceptosAbono)
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

        private void AddPayment(Account account, string filePath, int docId, string[] conceptosAbono, double importe)
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

                if (!conceptos.ContainsKey(idPaymentConcept))
                {
                    Concepto concepto = new Concepto();
                    concepto.APId = idPaymentConcept;
                    concepto.Codigo = concept.Code;
                    concepto.Nombre = concept.Name;
                    concepto.IdEmpresa = ConfiguredCompany().Id;
                    concepto.Razon = "PAGO";

                    conceptos.Add(idPaymentConcept, concepto);
                }

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

        private void FillCompany(Company company, string filePath)
        {
            int connCompany, dbResponse, fqResponse, agentId = 0, ubicacion = 0;
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

        private string AgentCode(int agentId, string filePath)
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

        private string CurrencyName(int currencyId, string filePath)
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

        private DocumentConcept GetDocumentConcept(int docId, string filePath)
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

        private int AdvanceConnectionIndex(string tipoDoc, string startDate, bool useDocDate, int connection)
        {
            int dbResponse;
            string key;
            key = tipoDoc;

            if (useDocDate)
            {
                key = tipoDoc + startDate;
                dbResponse = AdminPaqLib.dbGetNoLock(connection, TableNames.DOCUMENTOS, IndexNames.DOCUMENTOS_ID_DOCUMENTO01, key);
                if (dbResponse != 0)
                {
                    key = tipoDoc + startDate.Substring(0, 6);
                    dbResponse = AdminPaqLib.dbGetNoLock(connection, TableNames.DOCUMENTOS, IndexNames.DOCUMENTOS_ID_DOCUMENTO01, key);

                    if (dbResponse != 0)
                    {
                        key = tipoDoc + startDate.Substring(0, 4);
                        dbResponse = AdminPaqLib.dbGetNoLock(connection, TableNames.DOCUMENTOS, IndexNames.DOCUMENTOS_ID_DOCUMENTO01, key);

                        if (dbResponse != 0)
                        {
                            key = tipoDoc;
                            dbResponse = AdminPaqLib.dbGetNoLock(connection, TableNames.DOCUMENTOS, IndexNames.DOCUMENTOS_ID_DOCUMENTO01, key);
                        }
                    }
                }
            }
            else dbResponse = AdminPaqLib.dbGetNoLock(connection, TableNames.DOCUMENTOS, IndexNames.DOCUMENTOS_ID_DOCUMENTO01, key);

            return dbResponse;
        }

        private Empresa ConfiguredCompany()
        {
            Settings set = Settings.Default;
            int companyId = set.empresa;

            Empresa selectedCompany = empresas.ToList<Empresa>().Find(x => x.Id == companyId);
            return selectedCompany;
        }

        private void InitializeSDK()
        {
            int connEmpresas, dbResponse, fieldResponse;
            connEmpresas = AdminPaqLib.dbLogIn("", lib.DataDirectory);

            if (connEmpresas == 0)
            {
                ErrLogger.Log("No se pudo crear conexión a la tabla de Empresas de AdminPAQ.");
                return;
            }

            dbResponse = AdminPaqLib.dbGetTopNoLock(connEmpresas, TableNames.EMPRESAS, IndexNames.PRIMARY_KEY);
            while (dbResponse == 0)
            {
                Empresa empresa = new Empresa();

                int idEmpresa = 0;
                fieldResponse = AdminPaqLib.dbFieldLong(connEmpresas, TableNames.EMPRESAS, Empresa.ID_EMPRESA, ref idEmpresa);
                empresa.Id = idEmpresa;

                StringBuilder nombreEmpresa = new StringBuilder(151);
                fieldResponse = AdminPaqLib.dbFieldChar(connEmpresas, TableNames.EMPRESAS, Empresa.NOMBRE_EMPRESA, nombreEmpresa, 151);
                string sNombreEmpresa = nombreEmpresa.ToString(0, 150).Trim();
                empresa.Nombre = sNombreEmpresa;

                StringBuilder rutaEmpresa = new StringBuilder(254);
                fieldResponse = AdminPaqLib.dbFieldChar(connEmpresas, TableNames.EMPRESAS, Empresa.RUTA_EMPRESA, rutaEmpresa, 254);
                string sRutaEmpresa = rutaEmpresa.ToString(0, 253).Trim();
                empresa.Ruta = sRutaEmpresa;

                empresas.Add(empresa);
                dbResponse = AdminPaqLib.dbSkip(connEmpresas, TableNames.EMPRESAS, IndexNames.PRIMARY_KEY, 1);
            }

            AdminPaqLib.dbLogOut(connEmpresas);
        }
    }
}
