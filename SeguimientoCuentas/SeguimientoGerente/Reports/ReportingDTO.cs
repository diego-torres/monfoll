using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeguimientoGerente.Reports
{
    public class Account : Collectable.Account
    {
        public string CompanyCode { get { return this.Company.Code; } set { this.Company.Code = value; } }
        public string Name { get { return this.Company.Name; } set { this.Company.Name = value; } }
        public string PaymentDay { get { return this.Company.PaymentDay; } set { this.Company.PaymentDay = value; } }
        public string AgentCode { get { return this.Company.AgentCode; } set { this.Company.AgentCode = value; } }
    }
}
