using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;

namespace ConsolaCobranza.Facts
{
    interface Factoreable
    {
        void Prepare(int idEmpresa, string rutaEmpresa, NpgsqlConnection conn);
    }
}
