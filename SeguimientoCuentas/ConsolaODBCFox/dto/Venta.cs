using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsolaODBCFox.dto
{
    public class Venta
    {
        public Vendedor Vendedor { get; set; }
        public double Mensual { get; set; }
        public double Semanal { get; set; }
        public double Diaria { get; set; }
    }

    public class FactVenta
    {
        public int IdVendedor { get; set; }
        public double Importe { get; set; }
        public int IdMoneda { get; set; }
        public double TipoCambio { get; set; }
        public DateTime Fecha { get; set; }
    }
}
