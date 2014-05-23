using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Npgsql;

namespace SeguimientoSuper.Collectable.PostgresImpl
{
    public class Enterprise : CommonBase
    {

        public string[] ConceptosPago(int enterpriseId)
        {
            List<string> conceptos = new List<string>();

            DataSet ds = new DataSet();
            NpgsqlDataAdapter da;
            string sqlString = "SELECT codigo_concepto " +
                "FROM cat_concepto " +
                "WHERE id_empresa = " + enterpriseId.ToString() + " AND razon='PAGO';";

            if (conn == null || conn.State != ConnectionState.Open)
                connect();

            da = new NpgsqlDataAdapter(sqlString, conn);

            ds.Reset();
            da.Fill(ds);
            conn.Close();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                conceptos.Add(row["codigo_concepto"].ToString());
            }

            return conceptos.ToArray();
        }

        public void SaveEnterprise(Empresa enterprise)
        {
            if (enterprise.Id == 0) return;

            if (EnterpriseExists(enterprise.Id))
                UpdateEnterprise(enterprise);
            else
                AddEnterprise(enterprise);
        }

        private bool EnterpriseExists(int enterpriseId)
        {
            DataSet ds = new DataSet();
            NpgsqlDataAdapter da;
            string sqlString = "SELECT id_empresa FROM cat_empresa WHERE id_empresa = " + enterpriseId.ToString() + ";";

            if (conn == null || conn.State != ConnectionState.Open)
                connect();

            da = new NpgsqlDataAdapter(sqlString, conn);

            ds.Reset();
            da.Fill(ds);
            conn.Close();
            return ds.Tables[0].Rows.Count > 0;
        }

        private void UpdateEnterprise(Empresa enterprise)
        {
            if (conn == null || conn.State != ConnectionState.Open)
                connect();

            string sqlString = "UPDATE cat_empresa " +
                "SET nombre_empresa = @nombre_empresa, " +
                "ruta = @ruta " +
                "WHERE id_empresa = @id";

            NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@nombre_empresa", NpgsqlTypes.NpgsqlDbType.Varchar, 150);
            cmd.Parameters.Add("@ruta", NpgsqlTypes.NpgsqlDbType.Varchar, 253);
            cmd.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer);

            cmd.Parameters["@nombre_empresa"].Value = enterprise.Nombre;
            cmd.Parameters["@ruta"].Value = enterprise.Ruta;
            cmd.Parameters["@id"].Value = enterprise.Id;

            cmd.ExecuteNonQuery();
        }

        private void AddEnterprise(Empresa enterprise)
        {
            if (conn == null || conn.State != ConnectionState.Open)
                connect();

            string sqlString = "INSERT INTO cat_empresa(id_empresa, nombre_empresa, ruta ) " +
                "VALUES(@id, @nombre_empresa, @ruta )";

            NpgsqlCommand cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@nombre_empresa", NpgsqlTypes.NpgsqlDbType.Varchar, 150);
            cmd.Parameters.Add("@ruta", NpgsqlTypes.NpgsqlDbType.Varchar, 253);
            cmd.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer);

            cmd.Parameters["@nombre_empresa"].Value = enterprise.Nombre;
            cmd.Parameters["@ruta"].Value = enterprise.Ruta;
            cmd.Parameters["@id"].Value = enterprise.Id;

            cmd.ExecuteNonQuery();
        }
    }
}
