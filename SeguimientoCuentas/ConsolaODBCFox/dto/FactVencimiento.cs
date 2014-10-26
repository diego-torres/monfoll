using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsolaODBCFox.dto
{
    public class FactVencimiento
    {
        public int IdVencimiento { get; set; }
        public int IdCliente { get; set; }
        public int IdEmpresa { get; set; }
        public double Importe { get; set; }
    }
}
