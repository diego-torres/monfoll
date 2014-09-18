using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsolaCobranza.Facts;
using ConsolaCobranza.Collectable;
using Npgsql;
using System.Diagnostics;
using ConsolaCobranza.Loader;
using CommonAdminPaq;
using System.Globalization;

namespace ConsolaCobranza.Miner
{
    public class DocsMiner
    {
        public List<FactVencido> Vencidos { get; set; }
        public List<FactPorVencer> PorVencer { get; set; }
        public List<FactCobranza> Cobranza { get; set; }
        public List<FactSales> Ventas { get; set; }

        public const string TABLE_NAME = TableNames.DOCUMENTOS;
        public const string INDEX = IndexNames.PRIMARY_KEY;

        public const string CA_TABLE_NAME = "MGW10009";
        public const string CA_INDEX = "IDOCTOCA01";

        public void Execute(Empresa empresa, string[] conceptosFactura, string[] conceptosAbono, string[] codigosVenta,
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

            connDocos = AdminPaqLib.dbLogIn("", empresa.Ruta);
            if (connDocos == 0)
            {
                throw new Exception("Unable to work with route [" + empresa.Ruta + "]");
            }

            dbResponse = AdminPaqLib.dbGetTopNoLock(connDocos, TABLE_NAME, INDEX);
            log.WriteEntry("Downloading documents from AdminPaq");

            if (dbResponse != 0)
            {
                AdminPaqLib.dbLogOut(connDocos);
                log.WriteEntry("No data found in database, the following exception code was reported: "
                    + dbResponse.ToString() + " on path: " + empresa.Ruta, EventLogEntryType.Warning);
                throw new Exception("No data found in database, the following exception code was reported: "
                    + dbResponse.ToString());
            }
                

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

            log.WriteEntry("Data collection summary: cancelled=" + cancelados + ";returns=" + devueltos + ";other=" + otros + ";not printed=" + no_impresos + ";no_co=" + no_co + ";no_cliente=" + no_cliente + ";valid=" + valido);
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
                log.WriteEntry("seller not found for one of the records: " + document.SellerId, EventLogEntryType.Warning);
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
    }
}
