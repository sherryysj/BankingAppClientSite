using System;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mcba.Data;
using Mcba.Models;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace Mcba.McbaBackgroundService
{
    public class EmailBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<BillpayBackgroundService> _logger;
        private string mailContent;
        private decimal changeAmount;

        public EmailBackgroundService(IServiceProvider services, ILogger<BillpayBackgroundService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Background Service started");
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(180), cancellationToken);
                await SetEmail(cancellationToken);
            }
        }

        private async Task SetEmail(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Check customer update for sending report emails");

            using var scope = _services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<McbaContext>();

            // find customers having email address
            var customers = context.Customers.Where(p => p.Email != null);

            // for each customers found
            // check whether customer's accounts have any update after last email report sent
            // if has, generate email report Content and send
            foreach (Customer customer in customers)
            {
                string mailSubject = "MCBA - Customer Report for Customer " + customer.CustomerID;
                bool hasUpdate = GenerateMailContent(customer);
                if (hasUpdate) { 
                    if (SendEmail(customer.Email, mailSubject, mailContent))
                    {
                        customer.CheckDate = DateTime.UtcNow;
                    }
                }
            }

            await context.SaveChangesAsync();

        }

        // generate mailContent
        // for each account has updates, show its balance, transations after last email and balance updating amount
        public bool GenerateMailContent(Customer customer)
        {
            bool hasUpdate = false;
            mailContent = "";
            
            foreach (Account account in customer.Accounts)
            {
                // check whether account has updates or whether sent email report before
                if (customer.CheckDate == null || DateTime.Compare(account.ModifyDate, (DateTime)customer.CheckDate) > 0)
                {
                    hasUpdate = true;

                    mailContent += "<h3>Account Number: " + account.AccountNumber + "</h3>";
                    mailContent += "Current balance: $" + account.Balance + "<br>";
                    mailContent += "<h5><br>New Transactions: </h5>";
                    
                    changeAmount = 0;

                    // only find transactions after last report sent
                    if (customer.CheckDate != null)
                    {
                        var transations = account.Transactions.Where(
                            p => DateTime.Compare((DateTime)p.ModifyDate, (DateTime)customer.CheckDate) > 0);

                        foreach (Transaction transaction in transations)
                        {
                            AddTransactionContent(transaction);
                        }

                        // add the amount change if it's not first email
                        mailContent += "Total balance change since last report: $" + changeAmount + "<br>";

                    } else
                    {
                        var transations = account.Transactions;

                        foreach (Transaction transaction in transations)
                        {
                            AddTransactionContent(transaction);
                        }
                    }

                }

                mailContent += "<br><br>";
            }

            return hasUpdate;
        }

        // add transaction details to email report content
        public void AddTransactionContent(Transaction transaction)
        {
            string space = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
            mailContent += "<b>" + space + "Transaction ID: " + transaction.TransactionID + "</b>";
            mailContent += "<br>" + space + "Transaction Type: " + transaction.TransactionType;
            if (transaction.DestinationAccountNumber != null)
            {
                mailContent += "<br>" + space + "Destination Account Number: " + transaction.DestinationAccountNumber;
            }
            mailContent += "<br>" + space + "Amount: $" + transaction.Amount;
            mailContent += "<br>" + space + "Comment: " + transaction.Comment;
            mailContent += "<br>" + space + "Date: " + transaction.ModifyDate + "<br><br>";

            // count change amount
            var type = transaction.TransactionType.ToString("g");
            if (type == "Deposit")
            {
                changeAmount += (decimal)transaction.Amount;
            }
            else if (type == "Withdraw" || type == "ServiceCharge" ||
                type == "BillPay")
            {
                changeAmount -= (decimal)transaction.Amount;
            }
            else if (type == "Transfer")
            {
                // if there is no destination account number, then for transfer in, otherwise transfer out
                if (transaction.DestinationAccountNumber == null)
                {
                    changeAmount += (decimal)transaction.Amount;
                }
                else
                {
                    changeAmount -= (decimal)transaction.Amount;
                }
            }
        }

        // send email by smtp
        public static bool SendEmail(string mailTo, string mailSubject, string mailContent)
        {
            // set sender mail info
            string smtpServer = "smtp.gmail.com"; //SMTP server
            string mailFrom = "rmitwdta2@gmail.com"; 
            string userPassword = "s3704732";
 
            // set smtp server info
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            smtpClient.Host = smtpServer; 
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new System.Net.NetworkCredential(mailFrom, userPassword);
 
            // set mail message info
            MailMessage mailMessage = new MailMessage(mailFrom, mailTo); 
            mailMessage.Subject = mailSubject;
            mailMessage.Body = mailContent;
            mailMessage.BodyEncoding = Encoding.UTF8;
            mailMessage.IsBodyHtml = true;
            mailMessage.Priority = MailPriority.Low;

            // send email and return result
            try
            {
                smtpClient.Send(mailMessage);
                return true;
            }
            catch (SmtpException ex)
            {
                return false;
            }
        }
    }
}
