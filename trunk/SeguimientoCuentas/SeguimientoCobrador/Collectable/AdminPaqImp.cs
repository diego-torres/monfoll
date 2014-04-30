using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonAdminPaq;
using SeguimientoCobrador.Properties;
using System.Globalization;

namespace SeguimientoCobrador.Collectable
{
    public class AdminPaqImp
    {
        private AdminPaqLib lib;
        private IList<Empresa> empresas = new List<Empresa>();
        private List<int> cancelados = new List<int>();

        public IList<Empresa> Empresas { get { return empresas; } set { empresas = value; } }
        public List<int> Cancelados { get { return cancelados; } }

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

        public void SetCollectDate(int docId, DateTime collectDate)
        {
            int connection, dbResponse;
            string key, command;

            Empresa configuredCompany = ConfiguredCompany();

            if (configuredCompany == null)
            {
                ErrLogger.Log("Wrong Company configuration.");
                return;
            }

            connection = AdminPaqLib.dbLogIn("", configuredCompany.Ruta);
            if (connection == 0)
            {
                ErrLogger.Log("Unable to open connection to documents table for company [" + configuredCompany.Nombre + "]");
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
                throw new Exception("El registro del documento se encuentra bloqueado por otro usuario.");
            }

            AdminPaqLib.dbLogOut(connection);
        }

        public void UpdateCollectable(Account account)
        {
            int connection, dbResponse;
            string key, command;

            Empresa configuredCompany = ConfiguredCompany();

            if (configuredCompany == null)
            {
                ErrLogger.Log("Wrong Company configuration.");
                return;
            }

            connection = AdminPaqLib.dbLogIn("", configuredCompany.Ruta);
            if (connection == 0)
            {
                ErrLogger.Log("Unable to open connection to documents table for company [" + configuredCompany.Nombre + "]");
                return;
            }

            key = account.DocId.ToString().PadLeft(11);
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
                    command = string.Format("UPDATE {0}(CTEXTOEX01=\"{1}\",CTEXTOEX02=\"{2}\");",
                        TableNames.DOCUMENTOS, account.CollectType, account.Note);
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

            key = account.DocId.ToString().PadLeft(11);
            dbResponse = AdminPaqLib.dbGetNoLock(connPayments, TableNames.ABONOS_CARGOS, IndexNames.ABONOS_DOCUMENTOS, key);
            while (dbResponse == 0)
            {
                fqResponse = AdminPaqLib.dbFieldLong(connPayments, TableNames.ABONOS_CARGOS, 2, ref docId);
                if (docId != account.DocId) break;

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

                Payment payment = new Payment();
                payment.Concept = concept.Name;
                payment.DocId = account.DocId;
                payment.PaymentId = docId;
                payment.Amount = importe;

                fqResponse = AdminPaqLib.dbFieldChar(connPayment, TableNames.DOCUMENTOS, 17, sbTipoPago, 21);
                payment.PaymentType = sbTipoPago.ToString().Substring(0, 20).Trim();

                fqResponse = AdminPaqLib.dbFieldLong(connPayment, TableNames.DOCUMENTOS, 5, ref folio);
                payment.Folio = folio;

                fqResponse = AdminPaqLib.dbFieldChar(connPayment, TableNames.DOCUMENTOS, 56, sbDepositDate, 9);
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
            int connCompany, dbResponse, fqResponse, agentId=0;
            StringBuilder sbCompanyCode = new StringBuilder(31);
            StringBuilder sbPaymentDay = new StringBuilder(51);
            string key;

            connCompany = AdminPaqLib.dbLogIn("", filePath);
            if (connCompany == 0)
            {
                ErrLogger.Log("Unable to open connection for company");
                return;
            }

            key = company.Id.ToString().PadLeft(11);
            dbResponse = AdminPaqLib.dbGetNoLock(connCompany, TableNames.CLIENTES_PROVEEDORES, IndexNames.PRIMARY_KEY, key);

            if (dbResponse == 0)
            {
                fqResponse = AdminPaqLib.dbFieldChar(connCompany, TableNames.CLIENTES_PROVEEDORES, 2, sbCompanyCode, 31);
                company.Code = sbCompanyCode.ToString().Substring(0, 30).Trim();
                fqResponse = AdminPaqLib.dbFieldChar(connCompany, TableNames.CLIENTES_PROVEEDORES, 106, sbPaymentDay, 51);
                company.PaymentDay = sbPaymentDay.ToString().Substring(0, 50).Trim();
                fqResponse = AdminPaqLib.dbFieldLong(connCompany, TableNames.CLIENTES_PROVEEDORES, 37, ref agentId);
                company.AgentCode = AgentCode(agentId, filePath);
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
            string key, sCurrencyName=String.Empty;

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
