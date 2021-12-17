using System;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mcba.Data;
using Mcba.Models;
using System.Linq;

//This service looks for available billpays and peocess them when there is enough balance in the account
namespace Mcba.McbaBackgroundService
{
    public class BillpayBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<BillpayBackgroundService> _logger;

        public BillpayBackgroundService(IServiceProvider services, ILogger<BillpayBackgroundService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Background Service started");
            while (!cancellationToken.IsCancellationRequested)
            {
                //Wait for 30s for the next billpay scan - for testing purposes
                await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
                await ScanBillpays(cancellationToken);
            }
        }

        private async Task ScanBillpays(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Looking for active billpays");

            using var scope = _services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<McbaContext>();

            // find billpays schedule date before or at today
            var billPays = context.BillPays.Where(p => (p.Active == true) && (DateTime.Compare(p.ScheduleDate, DateTime.Now) <= 0));

            //Wait for 30s then execute billpay - for testing purposes
            if (billPays.Count() > 0) { 
                _logger.LogInformation("Billpay found, execute after 15s");
                await Task.Delay(TimeSpan.FromSeconds(15), cancellationToken);
            }

            // execute all billpays found if the account has enough money
            foreach (BillPay billPay in billPays)
            {

                if (billPay.Amount < billPay.Account.Balance)
                {
                    billPay.Account.Balance -= billPay.Amount;
                    billPay.Account.ModifyDate = DateTime.UtcNow;
                    billPay.Account.Transactions.Add(
                        new Transaction
                        {
                            TransactionType = TransactionType.BillPay,
                            Amount = billPay.Amount,
                            ModifyDate = DateTime.UtcNow
                        });
                    if (billPay.Period == "O")
                    {
                        //revome one time bills
                        context.BillPays.Remove(billPay);
                        _logger.LogInformation("Onetime billpay deducted, billpay record removed"); 
                    }
                    else if (billPay.Period == "M")
                    {
                        //update schedule date for periodical bills
                        billPay.ScheduleDate = billPay.ScheduleDate.AddMonths(1);
                        _logger.LogInformation("Billpay deducted, next schedule date updated"); 
                    }
                    else
                    {
                        //update schedule date for periodical bills
                        billPay.ScheduleDate = billPay.ScheduleDate.AddYears(1);
                        _logger.LogInformation("Billpay deducted, next schedule date updated"); 
                    }
                }

            }

            await context.SaveChangesAsync();
        }
    }
}
