using Microsoft.AspNetCore.Mvc.Rendering;

namespace Assignment.Models
{
    public class ExchangeRateModel
    {
        public string Currency { get; set; }
        public decimal Buy { get; set; }
        public decimal Sell { get; set; }
    }

    public class ExchangeRateApiResponse
    {
        public List<ExchangeRateModel> Rates { get; set; }
    }

    public class MoneyTransferModel
    {
        public int id { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public string ReceivMobileNo { get; set; }
        public decimal Amount { get; set; }
        public string FromCurrency { get; set; }
        public string FromCountry { get; set; }
        public string ToCountry { get; set; }
        public decimal Rate { get; set; }
    }
    public class TransactionReportModel
    {
        public int id { get; set; }
        public string? SenderName { get; set; }
        public string? ReceiverName { get; set; }
        public string? MobileNo { get; set; }
        public decimal? Amount { get; set; }
        public string? Currency { get; set; }
        public string? FromCountry { get; set; }
        public string? ToCountry { get; set; }
        public decimal? Rate { get; set; }
        public DateTime Date { get; set; }
    }
}
