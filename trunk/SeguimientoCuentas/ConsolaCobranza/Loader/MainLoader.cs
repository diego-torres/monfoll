using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsolaCobranza.Facts;
using Npgsql;

namespace ConsolaCobranza.Loader
{
    public class MainLoader
    {
        public List<FactVencido> Vencidos { get; set; }
        public List<FactPorVencer> PorVencer { get; set; }
        public List<FactCobranza> Cobranza { get; set; }
        public List<FactSales> Ventas { get; set; }

        public void Load(int idEmpresa, NpgsqlConnection conn)
        {
            LoadDues(conn);
            LoadAboutToDue(conn);
            LoadSales(conn);
            LoadCollection(conn);
        }

        private void LoadCollection(NpgsqlConnection conn)
        {
            foreach (FactCobranza collection in Cobranza)
            {
                if (CollectionExists(collection, conn))
                    UpdateCollection(collection, conn);
                else
                    AddCollection(collection, conn);
            }
        }

        private void LoadSales(NpgsqlConnection conn)
        {
            foreach (FactSales sale in Ventas)
            {
                if (SaleExists(sale, conn))
                    UpdateSale(sale, conn);
                else
                    AddSale(sale, conn);
            }
        }

        private void LoadDues(NpgsqlConnection conn)
        {
            foreach (FactVencido due in Vencidos)
            {
                if (VencidoExists(due, conn))
                    UpdateFactVencido(due, conn);
                else
                    AddFactVencido(due, conn);
            }
        }

        private void LoadAboutToDue(NpgsqlConnection conn)
        {
            foreach (FactPorVencer a2due in PorVencer)
            {
                if (PorVencerExists(a2due, conn))
                    UpdateFactPorVencer(a2due, conn);
                else
                    AddFactPorVencer(a2due, conn);
            }
        }

        private bool SaleExists(FactSales fact, NpgsqlConnection conn)
        {
            NpgsqlCommand cmd;

            string sqlString = "SELECT seller_id FROM fact_sales WHERE seller_id = @seller";

            cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@seller", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters["@seller"].Value = fact.Seller.IdSeller;

            NpgsqlDataReader dr = cmd.ExecuteReader();
            bool result = dr.Read();
            dr.Close();

            return result;
        }

        private void AddSale(FactSales fact, NpgsqlConnection conn)
        {
            NpgsqlCommand cmd;

            string sqlString = "INSERT INTO fact_sales(seller_id, sold_today, sold_week, sold_month) " +
                "VALUES(@seller, @hoy, @semana, @mes);";

            cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@seller", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters.Add("@hoy", NpgsqlTypes.NpgsqlDbType.Numeric);
            cmd.Parameters.Add("@semana", NpgsqlTypes.NpgsqlDbType.Numeric);
            cmd.Parameters.Add("@mes", NpgsqlTypes.NpgsqlDbType.Numeric);

            cmd.Parameters["@seller"].Value = fact.Seller.IdSeller;
            cmd.Parameters["@hoy"].Value = fact.SoldToday;
            cmd.Parameters["@semana"].Value = fact.SoldWeek;
            cmd.Parameters["@mes"].Value = fact.SoldMonth;

            cmd.ExecuteNonQuery();
        }

        private void UpdateSale(FactSales fact, NpgsqlConnection conn)
        {
            NpgsqlCommand cmd;

            string sqlString = "UPDATE fact_sales SET " + 
                "sold_today = @hoy, " +
                "sold_week = @semana, " +
                "sold_month = @mes " +
                "WHERE seller_id = @seller;";

            cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@seller", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters.Add("@hoy", NpgsqlTypes.NpgsqlDbType.Numeric);
            cmd.Parameters.Add("@semana", NpgsqlTypes.NpgsqlDbType.Numeric);
            cmd.Parameters.Add("@mes", NpgsqlTypes.NpgsqlDbType.Numeric);

            cmd.Parameters["@seller"].Value = fact.Seller.IdSeller;
            cmd.Parameters["@hoy"].Value = fact.SoldToday;
            cmd.Parameters["@semana"].Value = fact.SoldWeek;
            cmd.Parameters["@mes"].Value = fact.SoldMonth;

            cmd.ExecuteNonQuery();
        }

        private bool CollectionExists(FactCobranza fact, NpgsqlConnection conn)
        {
            NpgsqlCommand cmd;
            string sqlString = "SELECT id_mes FROM fact_collection WHERE id_mes = @mes";

            cmd = new NpgsqlCommand(sqlString, conn);
            cmd.Parameters.Add("@mes", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters["@mes"].Value = fact.Month.IdMes;

            NpgsqlDataReader dr = cmd.ExecuteReader();
            bool result = dr.Read();
            dr.Close();

            return result;
        }

        private void AddCollection(FactCobranza fact, NpgsqlConnection conn)
        {
            NpgsqlCommand cmd;

            string sqlString = "INSERT INTO fact_collection(id_mes, vendido, cobrado, incobrable)" +
                "VALUES(@mes, @vendido, @cobrado, @incobrable);";

            cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@mes", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters.Add("@vendido", NpgsqlTypes.NpgsqlDbType.Numeric);
            cmd.Parameters.Add("@cobrado", NpgsqlTypes.NpgsqlDbType.Numeric);
            cmd.Parameters.Add("@incobrable", NpgsqlTypes.NpgsqlDbType.Numeric);

            cmd.Parameters["@mes"].Value = fact.Month.IdMes;
            cmd.Parameters["@vendido"].Value = fact.Sold;
            cmd.Parameters["@cobrado"].Value = fact.Collected;
            cmd.Parameters["@incobrable"].Value = fact.Uncollectable;

            cmd.ExecuteNonQuery();
        }

        private void UpdateCollection(FactCobranza fact, NpgsqlConnection conn)
        {
            NpgsqlCommand cmd;

            string sqlString = "UPDATE fact_collection SET " +
                "vendido = @vendido, " +
                "cobrado = @cobrado, " +
                "incobrable = @incobrable " +
                "WHERE id_mes= @mes;";

            cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@mes", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters.Add("@vendido", NpgsqlTypes.NpgsqlDbType.Numeric);
            cmd.Parameters.Add("@cobrado", NpgsqlTypes.NpgsqlDbType.Numeric);
            cmd.Parameters.Add("@incobrable", NpgsqlTypes.NpgsqlDbType.Numeric);

            cmd.Parameters["@mes"].Value = fact.Month.IdMes;
            cmd.Parameters["@vendido"].Value = fact.Sold;
            cmd.Parameters["@cobrado"].Value = fact.Collected;
            cmd.Parameters["@incobrable"].Value = fact.Uncollectable;

            cmd.ExecuteNonQuery();
        }

        private bool PorVencerExists(FactVencimiento fact, NpgsqlConnection conn)
        {
            NpgsqlCommand cmd;

            string sqlString = "SELECT id_cliente " +
                "FROM fact_por_vencer " +
                "WHERE id_cliente = @cliente AND id_grupo_vencimiento = @grupo;";

            cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@cliente", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters.Add("@grupo", NpgsqlTypes.NpgsqlDbType.Integer);

            cmd.Parameters["@cliente"].Value = fact.Cliente.IdCliente;
            cmd.Parameters["@grupo"].Value = fact.GrupoVencimiento.IdGrupo;

            NpgsqlDataReader dr = cmd.ExecuteReader();
            bool result = dr.Read();
            dr.Close();

            return result;
        }

        private void AddFactPorVencer(FactVencimiento fact, NpgsqlConnection conn)
        {
            NpgsqlCommand cmd;

            string sqlString = "INSERT INTO fact_por_vencer(id_cliente, id_grupo_vencimiento, saldo_por_vencer)" +
                "VALUES(@cliente, @grupo, @saldo);";

            cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@cliente", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters.Add("@grupo", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters.Add("@saldo", NpgsqlTypes.NpgsqlDbType.Numeric);

            cmd.Parameters["@cliente"].Value = fact.Cliente.IdCliente;
            cmd.Parameters["@grupo"].Value = fact.GrupoVencimiento.IdGrupo;
            cmd.Parameters["@saldo"].Value = fact.Saldo;

            cmd.ExecuteNonQuery();
        }

        private void UpdateFactPorVencer(FactVencimiento fact, NpgsqlConnection conn)
        {
            NpgsqlCommand cmd;

            string sqlString = "UPDATE fact_por_vencer SET " +
                "saldo_por_vencer = @saldo " +
                "WHERE id_cliente = @cliente AND id_grupo_vencimiento = @grupo;";

            cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@cliente", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters.Add("@grupo", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters.Add("@saldo", NpgsqlTypes.NpgsqlDbType.Numeric);

            cmd.Parameters["@cliente"].Value = fact.Cliente.IdCliente;
            cmd.Parameters["@grupo"].Value = fact.GrupoVencimiento.IdGrupo;
            cmd.Parameters["@saldo"].Value = fact.Saldo;

            cmd.ExecuteNonQuery();
        }

        private bool VencidoExists(FactVencimiento fact, NpgsqlConnection conn)
        {
            NpgsqlCommand cmd;

            string sqlString = "SELECT id_cliente " +
                "FROM fact_vencido " +
                "WHERE id_cliente = @cliente AND id_grupo_vencimiento = @grupo;";

            cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@cliente", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters.Add("@grupo", NpgsqlTypes.NpgsqlDbType.Integer);

            cmd.Parameters["@cliente"].Value = fact.Cliente.IdCliente;
            cmd.Parameters["@grupo"].Value = fact.GrupoVencimiento.IdGrupo;

            NpgsqlDataReader dr = cmd.ExecuteReader();
            bool result = dr.Read();
            dr.Close();

            return result;
        }

        private void AddFactVencido(FactVencimiento fact, NpgsqlConnection conn)
        {
            NpgsqlCommand cmd;

            string sqlString = "INSERT INTO fact_vencido(id_cliente, id_grupo_vencimiento, saldo_vencido)" +
                "VALUES(@cliente, @grupo, @saldo);";

            cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@cliente", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters.Add("@grupo", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters.Add("@saldo", NpgsqlTypes.NpgsqlDbType.Numeric);

            cmd.Parameters["@cliente"].Value = fact.Cliente.IdCliente;
            cmd.Parameters["@grupo"].Value = fact.GrupoVencimiento.IdGrupo;
            cmd.Parameters["@saldo"].Value = fact.Saldo;

            cmd.ExecuteNonQuery();
        }

        private void UpdateFactVencido(FactVencimiento fact, NpgsqlConnection conn)
        {
            NpgsqlCommand cmd;

            string sqlString = "UPDATE fact_vencido SET " +
                "saldo_vencido = @saldo " +
                "WHERE id_cliente = @cliente AND id_grupo_vencimiento = @grupo;";

            cmd = new NpgsqlCommand(sqlString, conn);

            cmd.Parameters.Add("@cliente", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters.Add("@grupo", NpgsqlTypes.NpgsqlDbType.Integer);
            cmd.Parameters.Add("@saldo", NpgsqlTypes.NpgsqlDbType.Numeric);

            cmd.Parameters["@cliente"].Value = fact.Cliente.IdCliente;
            cmd.Parameters["@grupo"].Value = fact.GrupoVencimiento.IdGrupo;
            cmd.Parameters["@saldo"].Value = fact.Saldo;

            cmd.ExecuteNonQuery();
        }
    }
}
