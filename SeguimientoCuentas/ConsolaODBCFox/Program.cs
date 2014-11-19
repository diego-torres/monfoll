using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Odbc;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Configuration;
using ConsolaODBCFox.Config;
using ConsolaODBCFox.FoxMiner;
using ConsolaODBCFox.dto;

namespace ConsolaODBCFox
{
    class Program
    {
        static void Main(string[] args)
        {
            EventLog eventLogService = new EventLog();

            if (!EventLog.SourceExists("ConsolaDBF"))
            {
                EventLog.CreateEventSource("ConsolaDBF", "DBFCmdLog");
            }

            eventLogService.Source = "ConsolaDBF";
            eventLogService.Log = "DBFCmdLog";


            if (args.Length == 0)
            {
                eventLogService.WriteEntry("Console usage: NO ARGUMENTS FOUND.", EventLogEntryType.Error, 1, 1);
                return;
            }

            eventLogService.WriteEntry("CONSOLE EXECUTION BEGIN FOR [" + args[0] + "].");

            // Read configuration file
            string rutaDatos = ConfigurationManager.AppSettings["rutaDatos"].ToString();
            var configuredClients = ConfigurationManager.AppSettings as NameValueCollection;
            if (configuredClients == null)
            {
                eventLogService.WriteEntry("Unable to load the configuration file.", EventLogEntryType.Warning, 2, 1);
                return;
            }

            if (configuredClients.Count == 0)
            {
                eventLogService.WriteEntry("No keys detected in configuration file.", EventLogEntryType.Warning, 3, 1);
                return;
            }

            foreach (var key in configuredClients.AllKeys)
            {
                string configuredClient = configuredClients.GetValues(key).FirstOrDefault();

                if ("rutaDatos".Equals(key.ToString())) continue;
                if ("primerContacto".Equals(key.ToString())) continue;
                EnterpriseSection clientConfig = (EnterpriseSection)System.Configuration.ConfigurationManager.GetSection("Empresas/" + configuredClient);

                if (clientConfig == null)
                {
                    eventLogService.WriteEntry("Company configuration not found for Empresas/" + configuredClient + ".", EventLogEntryType.Warning, 4, 1);
                    continue;
                }

                Empresa empresa = Empresas.GetEmpresa(clientConfig.NombreEmpresa, rutaDatos, eventLogService);
                empresa.Ruta = @clientConfig.RutaEmpresa;
                if (empresa == null)
                {
                    eventLogService.WriteEntry("Configuration not found in AdminPaq for configured company: " +
                        clientConfig.NombreEmpresa, EventLogEntryType.Warning, 5, 1);
                    continue;
                }
                else
                {
                    eventLogService.WriteEntry("Company configuration found in database: " + clientConfig.NombreEmpresa + "; " + @empresa.Ruta, EventLogEntryType.Information, 6,1);

                    string[] ConceptosAbono = clientConfig.CodigosPago.Split(',');
                    string[] ConceptosCredito = clientConfig.CodigosFactura.Split(',');
                    string[] ConceptosDevolucion = clientConfig.CodigosDevolucion.Split(',');
                    string[] ConceptosVenta = clientConfig.CodigosVenta.Split(',');
                    eventLogService.WriteEntry("Payment codes found for " + clientConfig.NombreEmpresa + " as [" + clientConfig.CodigosPago + "]", EventLogEntryType.Information, 7, 1);
                    eventLogService.WriteEntry("Credit codes found for " + clientConfig.NombreEmpresa + " as [" + clientConfig.CodigosFactura + "]", EventLogEntryType.Information, 8, 1);
                    eventLogService.WriteEntry("Sale codes found for " + clientConfig.NombreEmpresa + " as [" + clientConfig.CodigosVenta + "]", EventLogEntryType.Information, 9, 1);
                    eventLogService.WriteEntry("Return codes found for " + clientConfig.NombreEmpresa + " as [" + clientConfig.CodigosDevolucion + "]", EventLogEntryType.Information, 10, 1);

                    // Send configuration details as input to dataMiner.
                    empresa.ConceptosAbono = ConceptosAbono;
                    empresa.ConceptosCredito = ConceptosCredito;
                    empresa.ConceptosDevolucion = ConceptosDevolucion;
                    empresa.ConceptosVenta = ConceptosVenta;

                    Documentos docosAPI = new Documentos();
                    docosAPI.Empresa = empresa;
                    docosAPI.Log = eventLogService;
                    docosAPI.MonfollConnectionString = ConfigurationManager.ConnectionStrings[Config.Common.MONFOLL].ConnectionString;

                    // Execute dataMiner operation based on parameter
                    switch (args[0])
                    { 
                        case "COBRANZA":
                            docosAPI.ExtraerCuentasPorCobrar();
                            break;
                        case "ACTUALIZAR":
                            docosAPI.ActualizarCuentasPorCobrar();
                            break;
                        case "VENCIDOS":
                            docosAPI.CalcularVencidos();
                            break;
                        case "PORVENCER":
                            docosAPI.CalcularPorVencer();
                            break;
                        /*case "COBRADO":
                            docosAPI.CalcularCobrado();
                            break;*/
                        case "VENTAS":
                            docosAPI.CalcularVentas();
                            break;
                        /*case "DETALLE":
                            docosAPI.CalcularDetalle();
                            break;
                        case "FIX":
                            docosAPI.Fix();
                            break;*/
                        case "CLIENTES":
                            docosAPI.Clientes();
                            break;
                        default:
                            eventLogService.WriteEntry("Console usage: argument not allowed: " + args[0] + ".", EventLogEntryType.Error, 11, 1);
                            break;
                    }
                }
            }

            eventLogService.WriteEntry("CONSOLE EXECUTION END FOR " + args[0]);
        }
    }
}
