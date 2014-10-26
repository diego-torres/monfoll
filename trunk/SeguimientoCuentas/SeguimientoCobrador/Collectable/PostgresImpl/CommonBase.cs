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
        protected const string UNEXISTING_DOCO_ERR = "El documento ya no existe, pues ha sido saldado.";

        protected string ConnString
        {
            get
            {
                Settings set = Settings.Default;

                return String.Format("Server={0};Port={1};" +
                        "User Id={2};Password={3};Database={4};Timeout=5;",
                        set.server, set.port, set.user,
                        set.password, set.database);
            }
        }

        protected int ConfiguredCompany()
        {
            Settings set = Settings.Default;
            return set.empresa;
        }

    }
}
