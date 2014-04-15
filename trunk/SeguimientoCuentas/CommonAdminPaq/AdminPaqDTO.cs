using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonAdminPaq.dto
{
    public class TableNames {
        public const string EMPRESAS = "MGW00001";
    }
    public class IndexNames {
        public const string EMPRESAS_PK = "PRIMARYKEY";
    }
    public class Empresa
    {
        // FIELD INDEX IN TABLE
        public const int ID_EMPRESA = 1;
        public const int NOMBRE_EMPRESA = 2;
        public const int RUTA_EMPRESA = 3;

        // FIELD PROPERTIES FOR OBJECT
        private long id;
        private string nombre, ruta, alias, resultadoSemanal, tendencia;
        private IList<Agente> agentes;
        private IList<long> cSale = new List<long>();
        private IList<long> cReturn = new List<long>();
        private IList<string> directores = new List<string>();

        public long Id { get { return id; } set { id = value; } }
        public string Nombre { get { return nombre; } set { nombre = value; } }
        public string Ruta { get { return ruta; } set { ruta = value; } }
        public string Alias { get { return alias; } set { alias = value; } }
        public IList<Agente> Agentes { get { return agentes; } set { agentes = value; } }
        public IList<long> CodigosVenta { get { return cSale; } set { cSale = value; } }
        public IList<long> CodigosDevolucion { get { return cReturn; } set { cReturn = value; } }
        public IList<string> TelefonosDirectores { get { return directores; } set { directores = value; } }
        public string ResultadoSemanal { get { return resultadoSemanal; } set { resultadoSemanal = value; } }
        public string Tendencia { get { return tendencia; } set { tendencia = value; } }
    }
    public class Agente
    {

        public const int ID_AGENTE = 1;
        public const int CODIGO_AGENTE = 2;
        public const int NOMBRE_AGENTE = 3;

        private long id;
        private string codigo;
        private string nombre;
        private CellPhone phone;

        public long Id { get { return id; } set { id = value; } }
        public string Codigo { get { return codigo; } set { codigo = value; } }
        public string Nombre { get { return nombre; } set { nombre = value; } }
        public CellPhone Phone { get { return phone; } set { phone = value; } }
    }
    public class CellPhone {
        private string phoneNumber;
        private double metaSemanal;
        private double vendido4S;
        private double vendidoSemana;
        
        public string PhoneNumber { get { return phoneNumber; } set { phoneNumber = value; } }
        public double MetaSemanal { get { return metaSemanal; } set { metaSemanal = value; } }
        public double Vendido4S { get { return vendido4S; } set { vendido4S = value; } }
        public double VendidoSemana { get { return vendidoSemana; } set { vendidoSemana = value; } }
        
        public double CumplimientoTendencia 
        {
            get 
            {
                if (metaSemanal == 0) return 0;
                return (vendido4S) / (metaSemanal * 4);
            }
        }

        public double CumplimientoSemana
        {
            get 
            {
                if (metaSemanal == 0) return 0;
                return vendidoSemana / metaSemanal;
            }
        }

        public double FaltanteTendencia
        {
            get
            {
                if (this.CumplimientoTendencia >= 1) return 0;
                return (metaSemanal * 4) - vendido4S;
            }
        }

        public double FaltanteSemana
        {
            get
            {
                if(this.CumplimientoSemana>=1) return 0;
                return metaSemanal - vendidoSemana;
            }
        }

    }
}
