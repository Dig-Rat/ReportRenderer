using System;
using System.Text.Json.Serialization;

namespace Shared
{
    public class Order
    {
        //public string CompanyId { get; set; }
        //public string OrderNumber { get; set; }
        //public int Units { get; set; }
        //public decimal Charges { get; set; }
        //public DateTime OrderDate { get; set; }
        private string _CompanyId = "";
        private string _OrderNumber = "";
        private int _Units = 0;
        private decimal _Charges = 0;
        private DateTime _OrderDate = DateTime.MinValue;

        
        public string CompanyId { get => _CompanyId; set => _CompanyId = value; }
        public string OrderNumber { get => _OrderNumber; set => _OrderNumber = value; }
        public int Units { get => _Units; set => _Units = value; }
        public decimal Charges { get => _Charges; set => _Charges = value; }
        public DateTime OrderDate { get => _OrderDate; set => _OrderDate = value; }
    }
}
