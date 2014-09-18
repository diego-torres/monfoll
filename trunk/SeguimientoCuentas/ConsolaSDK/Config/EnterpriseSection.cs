using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace ConsolaSDK.Config
{
    /// <summary>
    /// Sección que define la configuración de una empresa y sus códigos de documentos
    /// </summary>
    public class EnterpriseSection : ConfigurationSection
    {
        [ConfigurationProperty("NombreEmpresa", DefaultValue = "Ramos Hermanos Internacional SPR de RL de CV")]
        public string NombreEmpresa
        {
            get { return (string)this["NombreEmpresa"]; }
            set { this["NombreEmpresa"] = value; }
        }

        [ConfigurationProperty("RutaEmpresa", DefaultValue = @"\\rh-srvadmin-01\Compacw\Empresas\RHI")]
        public string RutaEmpresa
        {
            get { return (string)this["RutaEmpresa"]; }
            set { this["RutaEmpresa"] = value; }
        }

        [ConfigurationProperty("CodigosFactura", DefaultValue = "2101,2103,2202,2302,2201,2301,2203,2303,2204,2304")]
        public string CodigosFactura
        {
            get { return (string)this["CodigosFactura"]; }
            set { this["CodigosFactura"] = value; }
        }

        [ConfigurationProperty("CodigosPago", DefaultValue = "10,1000,1001,1002,1003")]
        public string CodigosPago
        {
            get { return (string)this["CodigosPago"]; }
            set { this["CodigosPago"] = value; }
        }

        [ConfigurationProperty("CodigosVenta", DefaultValue = "2117,2118,2119,2120")]
        public string CodigosVenta
        {
            get { return (string)this["CodigosVenta"]; }
            set { this["CodigosVenta"] = value; }
        }

        [ConfigurationProperty("CodigosDevolucion", DefaultValue = "2109,2110")]
        public string CodigosDevolucion
        {
            get { return (string)this["CodigosDevolucion"]; }
            set { this["CodigosDevolucion"] = value; }
        }

    }
}
