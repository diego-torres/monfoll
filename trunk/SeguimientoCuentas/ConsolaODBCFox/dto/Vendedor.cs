using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsolaODBCFox.dto
{
    public class Vendedor
    {
        public int Id { get; set; }
        public int ApId { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Empresa { get; set; }
        public int EmpresaId { get; set; }
    }
}
