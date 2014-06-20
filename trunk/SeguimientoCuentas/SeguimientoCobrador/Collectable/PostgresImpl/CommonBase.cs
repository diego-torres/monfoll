using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;
using SeguimientoCobrador.Properties;

namespace SeguimientoCobrador.Collectable.PostgresImpl
{
    public abstract class CommonBase
    {
        protected NpgsqlConnection conn;

        protected void connect()
        {
            Settings set = Settings.Default;

            string connString = String.Format("Server={0};Port={1};" +
                    "User Id={2};Password={3};Database={4};Timeout=5;",
                    set.server, set.port, set.user,
                    set.password, set.database);
            conn = new NpgsqlConnection(connString);
            conn.Open();
        }

    }
}
