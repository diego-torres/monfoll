using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsolaODBCFox.dto
{
    public class FactVencimiento
    {
        public int IdVencimiento { get; set; }
        public DimCliente Cliente { get; set; }
        public string CodigoCliente { get; set; }
        public int IdEmpresa { get; set; }
        public double Importe { get; set; }
    }

    public class DimCliente
    {
        public int IdCliente { get; set; }
        public string CodigoCliente { get; set; }
        public string NombreCliente { get; set; }
        public bool EsLocal { get; set; }
        public string NombreEmpresa { get; set; }
        public int IdEmpresa { get; set; }
    }
}
