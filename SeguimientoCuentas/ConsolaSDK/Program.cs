using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Configuration;
using CommonAdminPaq;

namespace ConsolaSDK
{
    class Program
    {
        static void Main(string[] args)
        {
            EventLog eventLogService = new EventLog();

            if (!EventLog.SourceExists("ConsolaSDK"))
            {
                EventLog.CreateEventSource("ConsolaSDK", "SDKCmdLog");
            }

            eventLogService.Source = "ConsolaSDK";
            eventLogService.Log = "SDKCmdLog";

            eventLogService.WriteEntry("CONSOLE EXECUTION BEGIN.");

            if (args.Length == 0)
            {
                eventLogService.WriteEntry("Console usage: NO ARGUMENTS FOUND.", EventLogEntryType.Error, 1, 0);
                return;
            }
            
            string baseDir = ConfigurationManager.AppSettings["libDir"].ToString();
            eventLogService.WriteEntry("Setting baseDir as [" + baseDir + "]");

            AdminPaqSDK sdk = new AdminPaqSDK();
            sdk.SetDllFolder(baseDir);

            string rutaPrueba = ConfigurationManager.AppSettings["RutaPrueba"].ToString();
            int errCode = 0;
            string errDesc = "";
            eventLogService.WriteEntry("SDK Init.");
            errCode = AdminPaqSDK.fInicializaSDK();
            if (errCode != AdminPaqSDK.kSIN_ERRORES)
            {
                errDesc = AdminPaqSDK.ResuelveError(errCode);
                eventLogService.WriteEntry("Error al inicializar SDK: " + errDesc, EventLogEntryType.Error);
                AdminPaqSDK.fTerminaSDK();
                return;
            }

            errCode = AdminPaqSDK.fAbreEmpresa(rutaPrueba);
            if (errCode != AdminPaqSDK.kSIN_ERRORES)
            {
                errDesc = AdminPaqSDK.ResuelveError(errCode);
                eventLogService.WriteEntry("Error al abrir empresa: " + errDesc, EventLogEntryType.Error);
                AdminPaqSDK.fCierraEmpresa();
                AdminPaqSDK.fTerminaSDK();
                return;
            }

            errCode = AdminPaqSDK.fPosPrimerDocumento();
            if (errCode != AdminPaqSDK.kSIN_ERRORES)
            {
                errDesc = AdminPaqSDK.ResuelveError(errCode);
                eventLogService.WriteEntry("Error al posicionarse en primer documento: " + errDesc, EventLogEntryType.Error);
            }

            try 
            {
                //StringBuilder sbIdDoco = new StringBuilder(8);
                StringBuilder sbFecha = new StringBuilder(61);
                eventLogService.WriteEntry("Documento leido exitosamente, leyendo RAZON SOCIAL 61");

                errCode = AdminPaqSDK.fLeeDatoDocumento("CRAZONSO01", ref sbFecha, 61);
                eventLogService.WriteEntry("He intentado leer RAZON SOCIAL.");
                if (errCode != AdminPaqSDK.kSIN_ERRORES)
                {
                    eventLogService.WriteEntry("La lectura de LA RAZON SOCIAL presenta el error: " + errCode);
                    errDesc = AdminPaqSDK.ResuelveError(errCode);
                    eventLogService.WriteEntry("Error al leer campo CRAZONSO01: " + errDesc, EventLogEntryType.Error);
                }

                eventLogService.WriteEntry("RAZON SOCIAL: " + sbFecha.ToString());
            }
            catch (Exception ex) 
            {
                eventLogService.WriteEntry("Error al leer datos: " + ex.Message + " || " + ex.StackTrace, EventLogEntryType.Error);
            }

            AdminPaqSDK.fCierraEmpresa();
            AdminPaqSDK.fTerminaSDK();
            eventLogService.WriteEntry("CONSOLE EXECUTION END.");
        }
    }
}
