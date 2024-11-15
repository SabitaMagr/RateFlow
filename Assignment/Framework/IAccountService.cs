using Assignment.Models;

namespace Assignment.Framework
{
    public interface IAccountService
    {
        Task<bool> RegisterUserAsync(RegisterModel model);
        Task<LoginModel> AuthenticateUser(LoginCredModel model);
        Task<bool> transferMoney(MoneyTransferModel model);
        Task<IEnumerable<TransactionReportModel>> GetData(string fromDate, string toDate);
    }
}
