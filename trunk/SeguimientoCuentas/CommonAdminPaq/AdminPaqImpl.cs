using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonAdminPaq.dto;

namespace CommonAdminPaq
{
    public class AdminPaqImpl
    {
        private IList<Empresa> empresas = new List<Empresa>();
        private AdminPaqLib lib;

        public IList<Empresa> Empresas { get { return empresas; } set { empresas = value; } }
        
        public AdminPaqImpl() {
            lib = new AdminPaqLib();
            lib.SetDllFolder();
        }

        public void InitializeSDK() {
            int connEmpresas, dbResponse, fieldResponse;
            connEmpresas = AdminPaqLib.dbLogIn("", lib.DataDirectory);

            if (connEmpresas == 0)
            {
                ErrLogger.Log("No se pudo crear conexión a la tabla de Empresas de AdminPAQ.");
                return;
            }

            dbResponse = AdminPaqLib.dbGetTopNoLock(connEmpresas, TableNames.EMPRESAS, IndexNames.EMPRESAS_PK);
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
                dbResponse = AdminPaqLib.dbSkip(connEmpresas, TableNames.EMPRESAS, IndexNames.EMPRESAS_PK, 1);
            }

            AdminPaqLib.dbLogOut(connEmpresas);
        }

        public void RetrieveSales(Empresa empresa)
        {
            DateTime today = DateTime.Today;
            int weekStartDelta = 2 - (int)today.DayOfWeek;
            DateTime weekStart = today.AddDays(weekStartDelta);
            DateTime weeksAgo = weekStart.AddDays(-21);

            DateTime saleDate = weeksAgo;

            //KeyValuePair<string, long> connections = new KeyValuePair<string,long>();
            Dictionary<string, int> connections = new Dictionary<string, int>();
            connections.Add("documents", AdminPaqLib.dbLogIn("", empresa.Ruta));
            connections.Add("concepts", AdminPaqLib.dbLogIn("", empresa.Ruta));
            
            while (saleDate <= today) 
            {
                RetrieveSales(saleDate, empresa, connections);
                saleDate = saleDate.AddDays(1);
            }

            ReleaseConnections(connections);

        }

        private void ReleaseConnections(Dictionary<string, int> connections)
        {
            foreach (KeyValuePair<string, int> entry in connections)
            {
                AdminPaqLib.dbLogOut(entry.Value);
            }
        }

        private bool dateInCurrentWeek(DateTime date)
        {
            DateTime today = DateTime.Today;
            int weekStartDelta = 2 - (int)today.DayOfWeek;
            DateTime weekStart = today.AddDays(weekStartDelta);
            return date.CompareTo(weekStart) >= 0;
        }

        private long conceptCode(int conceptId, Dictionary<string, int> connections)
        {
            int connection, dbResponse, spaceCounter;
            bool connected = connections.TryGetValue("concepts", out connection);
            StringBuilder sbConceptCode = new StringBuilder(30);

            if (!connected || connection == 0)
            {
                ErrLogger.Log("Connection not allowed in adminpaq for concepts");
                return 0;
            }

            spaceCounter = 11 - conceptId.ToString().Length;
            string key = new string(' ', spaceCounter);
            key = key + conceptId.ToString();

            dbResponse = AdminPaqLib.dbGetNoLock(connection, "MGW10006", "PRIMARYKEY", key);
            if (dbResponse == 0)
            {
                int fqResult = AdminPaqLib.dbFieldChar(connection, "MGW10006", 2, sbConceptCode, 30);
                string sConcept = sbConceptCode.ToString().Substring(0, 29).Trim();
                long result = 0;
                bool casted = long.TryParse(sConcept, out result);
                if (!casted) return 0;

                return result;
            }

            return 0;
            
        }

        private bool agentInCompany(int agentId, Empresa empresa)
        {
            foreach (Agente agent in empresa.Agentes)
            {
                if (agent.Id == agentId)
                    return true;
            }
            return false;
        }

        private void addSale(int agentId, Empresa empresa, double sold, DateTime saleDate) 
        { 
            foreach(Agente agent in empresa.Agentes)
            {
                if (agent.Id != agentId) continue;

                agent.Phone.Vendido4S += sold;
                if (dateInCurrentWeek(saleDate))
                    agent.Phone.VendidoSemana += sold;
            }
        }

        private void RetrieveSales(DateTime date, Empresa empresa, Dictionary<string, int> connections) 
        {

            string filterDate;
            int cancelled, returned, conceptId, agentId, currencyId, connection, dbResponse;
            double changeValue, sold;
            StringBuilder docDate = new StringBuilder(9);
            long conceptCode;

            bool isSale, isReturn, validAgent, connected;

            connected = connections.TryGetValue("documents", out connection);
            if (!connected || connection == 0) 
            {
                ErrLogger.Log("Connection not allowed in adminpaq for company [" + empresa.Nombre + "] @ route [" + empresa.Ruta + "]");
                return;
            }

            filterDate = date.ToString("yyyyMMdd");
            dbResponse = AdminPaqLib.dbGetNoLock(connection, "MGW10008", "CFECHA", filterDate);
            while (dbResponse == 0)
            {
                int fqResult = 0;
                cancelled = 0;
                fqResult = AdminPaqLib.dbFieldLong(connection, "MGW10008", 26, ref cancelled);
                if (cancelled == 1) 
                {
                    dbResponse = AdminPaqLib.dbSkip(connection, "MGW10008", "CFECHA", 1);
                    continue;
                }

                returned = 0;
                fqResult = AdminPaqLib.dbFieldLong(connection, "MGW10008", 27, ref returned);
                if (returned == 1)
                {
                    dbResponse = AdminPaqLib.dbSkip(connection, "MGW10008", "CFECHA", 1);
                    continue;
                }

                fqResult = AdminPaqLib.dbFieldChar(connection, "MGW10008", 6, docDate, 9);
                string sDocDate = docDate.ToString().Substring(0, 8).Trim();
                if (!filterDate.Equals(sDocDate)) break;

                conceptId = 0;
                fqResult = AdminPaqLib.dbFieldLong(connection, "MGW10008", 3, ref conceptId);
                conceptCode = this.conceptCode(conceptId, connections);

                isReturn = false;
                isSale = empresa.CodigosVenta.Contains(conceptCode);
                if(!isSale) isReturn = empresa.CodigosDevolucion.Contains(conceptCode);

                if (!isSale && !isReturn)
                {
                    dbResponse = AdminPaqLib.dbSkip(connection, "MGW10008", "CFECHA", 1);
                    continue;
                }

                agentId = 0;
                fqResult = AdminPaqLib.dbFieldLong(connection, "MGW10008", 10, ref agentId);
                validAgent = agentInCompany(agentId, empresa);

                if (!validAgent)
                {
                    dbResponse = AdminPaqLib.dbSkip(connection, "MGW10008", "CFECHA", 1);
                    continue;
                }


                // Currency 1 = Pesos Mexicanos
                currencyId = 1;
                changeValue = 1;
                fqResult = AdminPaqLib.dbFieldLong(connection, "MGW10008", 15, ref currencyId);

                if (currencyId != 1) 
                {
                    fqResult = AdminPaqLib.dbFieldDouble(connection, "MGW10008", 16, ref changeValue);
                }

                sold = 0;
                fqResult = AdminPaqLib.dbFieldDouble(connection, "MGW10008", 31, ref sold);

                if (isReturn) sold *= -1;
                if (currencyId != 1) sold *= changeValue;

                addSale(agentId, empresa, sold, date);

                dbResponse = AdminPaqLib.dbSkip(connection, "MGW10008", "CFECHA", 1);
            }
        }

    }
}
