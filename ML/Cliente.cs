using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML
{
    public class Cliente
    {
        public int IdCliente { get; set; }
        public string Rfc { get; set; }
        public int NumeroEmpleado { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaDeControl { get; set; }
        public decimal Salario { get; set; }
        public List<object> Clientes { get; set; }

        public int NumeroDeRegistro { get; set; }
        public string DetalleError { get; set; }
        public List<object> Errores { get; set; }


    }
}