using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsolaODBCFox.dto;
using System.Configuration;
using Npgsql;
using System.Diagnostics;

namespace ConsolaODBCFox.ado
{
    public class PgGruposVencimiento
    {
        public static void GrabarVencimientos(List<FactVencimiento> vencimientos, GrupoVencimiento grupo, string USDTag)
        {
            List<FactVencimiento> listAllFactVencimientosInDB = new List<FactVencimiento>();
            
            string connectionString = ConfigurationManager.ConnectionStrings[Config.Common.JASPER].ConnectionString;

            using (NpgsqlConnection pgConnection = new NpgsqlConnection(connectionString))
            {
                pgConnection.Open();
                NpgsqlDataReader dr;
                NpgsqlCommand cmd;

                string sqlString = "SELECT id_cliente, id_grupo_vencimiento, saldo_vencido " +
                                    "FROM fact_vencido" + USDTag +";";

                cmd = new NpgsqlCommand(sqlString, pgConnection);
                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    FactVencimiento oFactVencimiento = new FactVencimiento();

                    DimCliente dCliente = new DimCliente();
                    dCliente.IdCliente = int.Parse(dr["id_cliente"].ToString());

                    oFactVencimiento.Cliente = dCliente;
                    oFactVencimiento.IdVencimiento = int.Parse(dr["id_grupo_vencimiento"].ToString());
                    oFactVencimiento.Importe = double.Parse(dr["saldo_vencido"].ToString());
                    listAllFactVencimientosInDB.Add(oFactVencimiento);
                }

                dr.Close();
                cmd.Dispose();
                pgConnection.Close();
            }

            lGrabarVencimientos(vencimientos, grupo, listAllFactVencimientosInDB, USDTag);
        }

        private static void lGrabarVencimientos(List<FactVencimiento> vencimientos, GrupoVencimiento grupo, List<FactVencimiento> listAllFactVencimientosInDB, string USDTag)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[Config.Common.JASPER].ConnectionString;
            // Para cada ID Doco en la BD de postgres
            using (NpgsqlConnection pgConnection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    pgConnection.Open();

                    using (NpgsqlTransaction transaction = pgConnection.BeginTransaction())
                    {
                        try
                        {
                            foreach (FactVencimiento vencimiento in vencimientos)
                            {
                                NpgsqlCommand theCommand;
                                FactVencimiento oFactVencimientToPersist = vencimiento;

                                // Check if record exists.
                                bool exists = false;
                                foreach (FactVencimiento record in listAllFactVencimientosInDB)
                                {
                                    if (record.Cliente.IdCliente == vencimiento.Cliente.IdCliente)
                                    {
                                        exists = true;
                                        break;
                                    }   
                                }

                                // if record exists, update the record.
                                string updateString = "";
                                if (exists)
                                {
                                    updateString = "UPDATE fact_vencido" + USDTag + " " +
                                        "SET saldo_vencido=@saldo_vencido " +
                                        "WHERE id_cliente=@id_cliente and id_grupo_vencimiento=@id_grupo_vencimiento;";
                                }
                                // if record does not exists, insert the record
                                else
                                {
                                    updateString = "INSERT INTO fact_vencido" + USDTag + "(" +
                                        "id_cliente, id_grupo_vencimiento, saldo_vencido) " +
                                        "VALUES (@id_cliente, @id_grupo_vencimiento, @saldo_vencido);";
                                }
                                
                                theCommand = new NpgsqlCommand(updateString, pgConnection);

                                theCommand.Parameters.Add("@saldo_vencido", NpgsqlTypes.NpgsqlDbType.Numeric);
                                theCommand.Parameters.Add("@id_cliente", NpgsqlTypes.NpgsqlDbType.Integer);
                                theCommand.Parameters.Add("@id_grupo_vencimiento", NpgsqlTypes.NpgsqlDbType.Integer);

                                theCommand.Parameters["@saldo_vencido"].Value = oFactVencimientToPersist.Importe;
                                theCommand.Parameters["@id_cliente"].Value = oFactVencimientToPersist.Cliente.IdCliente;
                                theCommand.Parameters["@id_grupo_vencimiento"].Value = grupo.Id;

                                theCommand.ExecuteNonQuery();
                            }
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            if (transaction != null) transaction.Rollback();
                            throw ex;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                pgConnection.Close();
            }
        }

        public static void GrabarPorVencer(List<FactVencimiento> porvencer, GrupoVencimiento grupo, string USDTag)
        {
            List<FactVencimiento> listAllFactPorVencerInDB = new List<FactVencimiento>();

            string connectionString = ConfigurationManager.ConnectionStrings[Config.Common.JASPER].ConnectionString;

            using (NpgsqlConnection pgConnection = new NpgsqlConnection(connectionString))
            {
                pgConnection.Open();
                NpgsqlDataReader dr;
                NpgsqlCommand cmd;

                string sqlString = "SELECT id_cliente, id_grupo_vencimiento, saldo_por_vencer " +
                                    "FROM fact_por_vencer" + USDTag + ";";

                cmd = new NpgsqlCommand(sqlString, pgConnection);
                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    FactVencimiento oFactPorVencer = new FactVencimiento();

                    DimCliente dCliente = new DimCliente();
                    dCliente.IdCliente = int.Parse(dr["id_cliente"].ToString());

                    oFactPorVencer.Cliente = dCliente;
                    oFactPorVencer.IdVencimiento = int.Parse(dr["id_grupo_vencimiento"].ToString());
                    oFactPorVencer.Importe = double.Parse(dr["saldo_por_vencer"].ToString());
                    listAllFactPorVencerInDB.Add(oFactPorVencer);
                }

                dr.Close();
                cmd.Dispose();
                pgConnection.Close();
            }


            // Para cada ID Doco en la BD de postgres
            using (NpgsqlConnection pgConnection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    pgConnection.Open();

                    using (NpgsqlTransaction transaction = pgConnection.BeginTransaction())
                    {

                        try
                        {
                            foreach (FactVencimiento currentPorVencer in porvencer)
                            {
                                NpgsqlCommand theCommand;
                                FactVencimiento oFactPorVencerToPersist = currentPorVencer;

                                // Check if record exists.
                                bool exists = false;
                                foreach (FactVencimiento record in listAllFactPorVencerInDB)
                                {
                                    if (record.Cliente.IdCliente == currentPorVencer.Cliente.IdCliente)
                                    {
                                        exists = true;
                                        break;
                                    }
                                }

                                //FactPorVencer oFactPorVencerExistente = listAllFactPorVencerInDB.First(record => record.IdCliente == currentPorVencer.IdCliente && record.IdEmpresa == currentPorVencer.IdEmpresa && record.IdVencimiento == currentPorVencer.IdVencimiento);

                                // if record exists, update the record.
                                if (exists)
                                {
                                    string sqlUpdateVencimiento = "UPDATE fact_por_vencer" + USDTag + " " +
                                                                    "SET saldo_por_vencer=@saldo_por_vencer " +
                                                                    "WHERE id_cliente=@id_cliente and id_grupo_vencimiento=@id_grupo_vencimiento;";
                                    theCommand = new NpgsqlCommand(sqlUpdateVencimiento, pgConnection);
                                }
                                // if record does not exists, insert the record
                                else
                                {
                                    string sqlUpdateVencimiento = "INSERT INTO fact_por_vencer" + USDTag + "(" +
                                                                    "id_cliente, id_grupo_vencimiento, saldo_por_vencer) " +
                                                                    "VALUES (@id_cliente, @id_grupo_vencimiento, @saldo_por_vencer);";
                                    theCommand = new NpgsqlCommand(sqlUpdateVencimiento, pgConnection);
                                }

                                theCommand.Parameters.Add("@saldo_por_vencer", NpgsqlTypes.NpgsqlDbType.Double);
                                theCommand.Parameters.Add("@id_cliente", NpgsqlTypes.NpgsqlDbType.Integer);
                                theCommand.Parameters.Add("@id_grupo_vencimiento", NpgsqlTypes.NpgsqlDbType.Integer);

                                theCommand.Parameters["@saldo_por_vencer"].Value = oFactPorVencerToPersist.Importe;
                                theCommand.Parameters["@id_cliente"].Value = oFactPorVencerToPersist.Cliente.IdCliente;
                                theCommand.Parameters["@id_grupo_vencimiento"].Value = grupo.Id;

                                theCommand.ExecuteNonQuery();

                            }
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            if (transaction != null) transaction.Rollback();
                            throw ex;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                pgConnection.Close();
            }
        }

        private static int IdCliente(NpgsqlConnection conn, string codigoCliente, int idEmpresa)
        {
            int result = 0;
            NpgsqlDataReader dr;
            NpgsqlCommand cmd;

            string sqlString = "SELECT id_cliente " +
                "FROM dim_clientes " +
                "WHERE codigo_cliente = @cdCliente AND id_empresa = @idEmpresa;";

            cmd = new NpgsqlCommand(sqlString, conn);
            cmd.Parameters.Add("@cdCliente", NpgsqlTypes.NpgsqlDbType.Varchar, 11);
            cmd.Parameters.Add("@idEmpresa", NpgsqlTypes.NpgsqlDbType.Integer);

            cmd.Parameters["@idCliente"].Value = codigoCliente;
            cmd.Parameters["@idEmpresa"].Value = idEmpresa;

            dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                // Cliente listo en base de datos; asignar identificador para relacion
                result = int.Parse(dr["id_cliente"].ToString());
            }

            dr.Close();
            cmd.Dispose();

            return result;
        }


        private static void ActualizarVencimiento(FactVencimiento vencimiento, GrupoVencimiento grupo, NpgsqlConnection conn)
        {
            string sqlString = "SELECT id_grupo_vencimiento, minimo_dias, maximo_dias " +
                    "FROM dim_grupo_vencimiento;";
            NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);
            cmd.ExecuteNonQuery();
            
        }

        public static List<GrupoVencimiento> GruposVencimiento(bool ParaVencidos)
        {
            List<GrupoVencimiento> result = new List<GrupoVencimiento>();
            string connectionString = ConfigurationManager.ConnectionStrings[Config.Common.JASPER].ConnectionString;
            // Para cada ID Doco en la BD de postgres
            using (NpgsqlConnection pgConnection = new NpgsqlConnection(connectionString))
            {
                pgConnection.Open();
                NpgsqlDataReader dr;
                NpgsqlCommand cmd;

                string sqlString = "";

                if (ParaVencidos)
                    sqlString = "SELECT id_grupo_vencimiento, minimo_dias, maximo_dias " +
                        "FROM dim_grupo_vencimiento " +
                        "WHERE a_vencidos=true";
                else
                    sqlString = "SELECT id_grupo_vencimiento, minimo_dias, maximo_dias " +
                        "FROM dim_grupo_vencimiento " +
                        "WHERE a_por_vencer=true";


                cmd = new NpgsqlCommand(sqlString, pgConnection);
                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    GrupoVencimiento grupo = new GrupoVencimiento();
                    grupo.Id = int.Parse(dr["id_grupo_vencimiento"].ToString());
                    grupo.Desde = int.Parse(dr["minimo_dias"].ToString());
                    grupo.Hasta = int.Parse(dr["maximo_dias"].ToString());
                    result.Add(grupo);
                }

                dr.Close();
                cmd.Dispose();
                pgConnection.Close();
            }
            return result;
        }
    }
}
