using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeguimientoSuper.Collectable
{
    public class TableNames
    {
        public const string EMPRESAS = "MGW00001";

        public const string AGENTES = "MGW10001";
        public const string CLIENTES_PROVEEDORES = "MGW10002";
        public const string CONCEPTOS_DOCUMENTOS = "MGW10006";
        public const string DOCUMENTOS = "MGW10008";
        public const string ABONOS_CARGOS = "MGW10009";
        public const string MONEDAS = "MGW10034";
    }
    public class IndexNames
    {
        public const string PRIMARY_KEY = "PRIMARYKEY";
        public const string DOCUMENTOS_ID_DOCUMENTO01 = "IDOCUMEN01";
        public const string ABONOS_DOCUMENTOS = "IDOCTOCA01";
        public const string DATE_FORMAT_PATTERN = "yyyyMMdd";
    }

    public class Concepto
    {
        public int Id { get; set; }
        public int APId { get; set; }
        public int IdEmpresa { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Razon { get; set; }
    }

    public class Empresa : IComparable<Empresa>
    {
        // FIELD INDEX IN TABLE
        public const int ID_EMPRESA = 1;
        public const int NOMBRE_EMPRESA = 2;
        public const int RUTA_EMPRESA = 3;

        // FIELD PROPERTIES FOR OBJECT
        private int id;
        private string nombre, ruta, alias, resultadoSemanal, tendencia;

        public int Id { get { return id; } set { id = value; } }
        public string Nombre { get { return nombre; } set { nombre = value; } }
        public string Ruta { get { return ruta; } set { ruta = value; } }

        public int CompareTo(Empresa obj)
        {
            if (this.id < obj.id)
                return 1;
            if (this.id > obj.id)
                return -1;
            else
                return 0;
        }
    }

    public class Account : IComparable<Account>
    {
        private int docId, folio;
        private DateTime docDate, collectDate, dueDate, lastDownload;
        private string serie, collectType, currency, note, docType;
        private double amount, balance;
        private Company company = new Company();
        private List<Payment> payments = new List<Payment>();

        public int DocId { get { return docId; } set { docId = value; } }
        public int Folio { get { return folio; } set { folio = value; } }
        public int ApId { get; set; }

        public DateTime DocDate { get { return docDate; } set { docDate = value; } }
        public DateTime CollectDate { get { return collectDate; } set { collectDate = value; } }
        public DateTime DueDate { get { return dueDate; } set { dueDate = value; } }
        public DateTime LastDownload { get { return lastDownload; } set { lastDownload = value; } }

        public string Serie { get { return serie; } set { serie = value; } }
        public string CollectType { get { return collectType; } set { collectType = value; } }
        public string Currency { get { return currency; } set { currency = value; } }
        public string Note { get { return note; } set { note = value; } }
        public string DocType { get { return docType; } set { docType = value; } }

        public double Amount { get { return amount; } set { amount = value; } }
        public double Balance { get { return balance; } set { balance = value; } }

        public Company Company { get { return company; } set { company = value; } }

        public List<Payment> Payments { get { return payments; } set { payments = value; } }

        public int CompareTo(Account obj)
        {
            if (this.docId < obj.DocId)
                return 1;
            if (this.docId > obj.DocId)
                return -1;
            else
                return 0;
        }
    }

    public class Payment : IComparable<Payment>
    {
        private int docId, folio, paymentId;
        private string paymentType, concept, account;
        private double amount;
        private DateTime depositDate;

        public int PaymentId { get { return paymentId; } set { paymentId = value; } }
        public int DocId { get { return docId; } set { docId = value; } }
        public int Folio { get { return folio; } set { folio = value; } }

        public string PaymentType { get { return paymentType; } set { paymentType = value; } }
        public string Concept { get { return concept; } set { concept = value; } }
        public string Account { get { return account; } set { account = value; } }

        public double Amount { get { return amount; } set { amount = value; } }

        public DateTime DepositDate { get { return depositDate; } set { depositDate = value; } }

        public int CompareTo(Payment obj)
        {
            if (this.docId < obj.DocId)
                return 1;
            if (this.docId > obj.DocId)
                return -1;
            else
                return 0;
        }

    }

    public class Company
    {
        private int companyId;
        private int apId;
        private int enterpriseId;
        private string code;
        private string name;
        private string paymentDay;
        private string agentCode;

        public int Id { get { return companyId; } set { companyId = value; } }
        public int ApId { get { return apId; } set { apId = value; } }
        public int EnterpriseId { get { return enterpriseId; } set { enterpriseId = value; } }
        public string EnterprisePath { get; set; }
        public string Code { get { return code; } set { code = value; } }
        public string Name { get { return name; } set { name = value; } }
        public string PaymentDay { get { return paymentDay; } set { paymentDay = value; } }
        public string AgentCode { get { return agentCode; } set { agentCode = value; } }
        public bool EsLocal { get; set; }
    }

    public class DocumentConcept
    {
        private int id;
        private string name, code;
        public int Id { get { return id; } set { id = value; } }
        public string Code { get { return code; } set { code = value; } }
        public string Name { get { return name; } set { name = value; } }
    }
}
