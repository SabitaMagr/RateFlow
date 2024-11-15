using Microsoft.EntityFrameworkCore;
using Assignment.Models;
using Assignment.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Assignment.Framework;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Assignment.Services
{
    public class AccountService : IAccountService
    {
         ApplicationDbContext _context;

        public AccountService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> RegisterUserAsync(RegisterModel model)
        {
            int id = await this.GetMaxId("dbo.SignUpTbl", "ID");

            var sql = "INSERT INTO dbo.SignUpTbl (Id,Username, Password,email,MobileNumber,Status) VALUES (@Id,@Username, @Password,@Email,@MobileNumber,'E')";
            var parameters = new[]
            {
                new SqlParameter("@Username", model.Username),
                new SqlParameter("@Id", id),
                new SqlParameter("@Password", model.Password),
                new SqlParameter("@Email", model.Email),
                new SqlParameter("@MobileNumber", model.MobileNumber)
            };

            var result = await _context.Database.ExecuteSqlRawAsync(sql, parameters);
            return result > 0; // returns true if at least one row was affected
        }
        public async Task<int> GetMaxId(string table, string column)
        {
            var query = $"SELECT ISNULL(MAX({column}), 0) + 1 AS {column} FROM {table}";
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                _context.Database.OpenConnection();
                var result = await command.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
        }
        public async Task<bool> transferMoney(MoneyTransferModel model)
        {
            int id = await this.GetMaxId("dbo.TransferMoney", "ID");

            var sql = @$"INSERT INTO dbo.TransferMoney (Id, SenderName, ReceiverName, FromCountry, ToCountry, MobileNo, Amount,Currency, Rate, DeletedFlag, Date) " +
                      $"VALUES ({id}, '{model.SenderName}', '{model.ReceiverName}', '{model.FromCountry}', '{model.ToCountry}', '{model.ReceivMobileNo}', {model.Amount},'{model.FromCurrency}', {model.Rate}, 'N', CAST(GETDATE() AS DATE))";

            var result = await _context.Database.ExecuteSqlRawAsync(sql);
            return result > 0; 
        }
        public async Task<LoginModel> AuthenticateUser(LoginCredModel model)
    {
        var user = new LoginModel();
        var sql = "SELECT * FROM dbo.SignUpTbl WHERE Username = @Username AND Password = @Password";
        var parameters = new[]
        {
                new SqlParameter("@Username", model.Username),
                new SqlParameter("@Password", model.Password)
            };
          user = await _context.Login.FromSqlRaw(sql, parameters).FirstOrDefaultAsync();
        return user;
    }
        public async Task<IEnumerable<TransactionReportModel>> GetData(string fromDate, string toDate)
        {
           var sql = @$"SELECT id,SenderName,ReceiverName,MobileNo,Amount,Currency,FromCountry,ToCountry,Rate,Date
                FROM dbo.TransferMoney  WHERE date BETWEEN '{fromDate}' AND '{toDate}' AND deletedflag = 'N' order by date desc";

            var data = await _context.MoneyTransfers.FromSqlRaw(sql).ToListAsync();
            return data;
        }

    }
}
