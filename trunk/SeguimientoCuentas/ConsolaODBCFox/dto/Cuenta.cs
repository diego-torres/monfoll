using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Globalization;

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

        public DateTime FechaEsperadaCobro 
        { 
            get 
            {
                if (Cliente.DiaPago == null || FechaDoc == null || FechaVencimiento == null)
                    return new DateTime(1899, 12, 30);

                TimeSpan span = FechaVencimiento.Subtract(FechaDoc);
                int diasCredito = (int)span.TotalDays;

                string sConstantePrimerContacto = ConfigurationManager.AppSettings["primerContacto"].ToString();
                double dConstantePrimerContacto = double.Parse(sConstantePrimerContacto);

                int diasContacto = (int) Math.Round(diasCredito * dConstantePrimerContacto);

                DateTime result = FechaDoc.AddDays(diasContacto);

                while (result <= FechaVencimiento)
                { 
                    string diaPago = Cliente.DiaPago.ToLower();
                    diaPago = diaPago.Replace('á', 'a');
                    diaPago = diaPago.Replace('é', 'e');
                    string diaContacto = result.ToString("ddd", new CultureInfo("es-MX")).ToLower();
                    diaContacto = diaContacto.Replace('á', 'a');
                    diaContacto = diaContacto.Replace('é', 'e');

                    if(diaPago.Contains(diaContacto))
                        break;

                    result = result.AddDays(1);
                }

                return result; 
            } 
        }

    }
}
