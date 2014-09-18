using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsolaODBCFox.dto
{
    public class Empresa
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Ruta { get; set; }
        public string[] ConceptosAbono { get; set; }
        public string[] ConceptosCredito { get; set; }
        public string[] ConceptosDevolucion { get; set; }
        public string[] ConceptosVenta { get; set; }
    }
}
