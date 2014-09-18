using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Odbc;
using System.Diagnostics;
using ConsolaODBCFox.dto;

namespace ConsolaODBCFox.FoxMiner
{
    public class Empresas
    {
        public static Empresa GetEmpresa(string nombreEmpresa, string rutaDatos, EventLog log)
        {
            Empresa result = null;

            // COMMAND PARAMETERS ARE NOT WELL HANDLED BY DRIVER, USING HARD CODED CONCATENATION.
            // Received error: EXCEPTION WHILE USING DATABASE: ERROR [S1000] [Microsoft][ODBC Visual FoxPro Driver]Missing operand. 
            string sqlString = "SELECT CIDEMPRESA " +
                "FROM MGW00001 " +
                "WHERE CNOMBREE01 = '" + nombreEmpresa + "'";

            try
            {
                string connString = "Driver={Microsoft Visual FoxPro Driver};SourceType=DBF;Exclusive=No;" +
                @"SourceDB=" + rutaDatos + ";";
                using (OdbcConnection conn = new OdbcConnection(connString))
                {
                    conn.Open();

                    OdbcDataReader dr;
                    OdbcCommand cmd;

                    cmd = new OdbcCommand(sqlString, conn);
                    dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        result = new Empresa();
                        result.Id = int.Parse(dr["CIDEMPRESA"].ToString());
                        result.Nombre = nombreEmpresa;
                    }

                    dr.Close();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                log.WriteEntry("EXCEPTION WHILE USING DATABASE: " + ex.Message + " || " + ex.StackTrace, EventLogEntryType.Warning, 1, 2);
            }

            return result;
        }
    }
}
