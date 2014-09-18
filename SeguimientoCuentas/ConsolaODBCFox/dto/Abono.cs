using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsolaODBCFox.dto
{
    public class Abono
    {
        public int Id { get; set; }
        public int IdCuenta { get; set; }
        public int Folio { get; set; }

        public string TipoPago { get; set; }
        public string Concepto { get; set; }
        public string Cuenta { get; set; }

        public double Monto { get; set; }

        public DateTime FechaDeposito { get; set; }
    }
}
