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

    public class DetalleVenta
    {
        public string Vendedor { get; set; }
        public int IdDoco { get; set; }
        public int Folio { get; set; }
        public string Serie { get; set; }
        public double Importe { get; set; }
        public string Moneda { get; set; }
        public int IdConcepto { get; set; }
        public double TipoCambio { get; set; }
        public DateTime Fecha { get; set; }
        public List<DetalleMovimiento> Movimientos { get; set; }
    }
    public class DetalleMovimiento
    {
        public int IdMov { get; set; }
        public string Producto { get; set; }
        public double Importe { get; set; }
    }
}
