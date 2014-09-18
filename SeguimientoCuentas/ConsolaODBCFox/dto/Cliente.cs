using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsolaODBCFox.dto
{
    public class Cliente
    {
        public int Id { get; set; }
        public int ApId { get; set; }
        public int IdEmpresa { get; set; }
        public string RutaEmpresa { get; set; }
        public string Codigo { get; set; }
        public string RazonSocial { get; set; }
        public string DiaPago { get; set; }
        public string Ruta { get; set; }
        public bool EsLocal { get; set; }
    }
}
