using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsolaODBCFox.dto
{
    public class Cuenta
    {
        public int Id { get; set; }
        public int Folio { get; set; }
        public int ApId { get; set; }

        public DateTime FechaDoc { get; set; }
        public DateTime FechaCobro { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public DateTime Descarga { get; set; }

        public string Serie { get; set; }
        public string TipoCobro { get; set; }
        public string Moneda { get; set; }
        public string Observaciones { get; set; }
        public string TipoDocumento { get; set; }

        public double Total { get; set; }
        public double Saldo { get; set; }

        public Cliente Cliente { get; set; }
        public List<Abono> Abonos { get; set; }
    }
}
