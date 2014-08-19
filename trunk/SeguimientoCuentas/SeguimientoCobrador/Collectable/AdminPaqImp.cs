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
        public IList<Empresa> Empresas { get { return empresas; } set { empresas = value; } }
        
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
                throw new Exception("No se pudo establecer conexión con adminPaq en la siguiente ruta: " + rutaEmpresa);
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
                throw new Exception("No se pudo establecer conexión con adminPaq en la siguiente ruta: " + rutaEmpresa);
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
                throw new Exception("No se pudo establecer conexión con adminPaq en la siguiente ruta: " + account.Company.EnterprisePath);
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
