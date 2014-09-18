using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsolaCobranza.Loader;
using Npgsql;

namespace ConsolaCobranza.Facts
{
    public abstract class FactVencimiento : Factoreable
    {
        public DimClientes Cliente { get; set; }
        public DimGrupoVencimiento GrupoVencimiento { get; set; }
        public double Saldo { get; set; }

        public abstract void Prepare(int idEmpresa, string rutaEmpresa, NpgsqlConnection conn);
        public abstract void DeleteFacts(int idEmpresa, NpgsqlConnection conn);
    }
}
