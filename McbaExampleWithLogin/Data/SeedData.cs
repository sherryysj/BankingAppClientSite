using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Mcba.Models;

namespace Mcba.Data
{
    public static class SeedData
    {
        public static McbaContext context;

        public static void Initialize(IServiceProvider serviceProvider)
        {
            context = serviceProvider.GetRequiredService<McbaContext>();

            // Look for customers.
            if (context.Customers.Any())
                return; // DB has already been seeded.

            context.Customers.AddRange(
                new Customer
                {
                    CustomerID = 2100,
                    Name = "Matthew Bolger",
                    Address = "123 Fake Street",
                    City = "Melbourne",
                    PostCode = "3000"
                },
                new Customer
                {
                    CustomerID = 2200,
                    Name = "Rodney Cocker",
                    Address = "456 Real Road",
                    City = "Melbourne",
                    PostCode = "3005"
                },
                new Customer
                {
                    CustomerID = 2300,
                    Name = "Shekhar Kalra"
                });

            context.Logins.AddRange(
                new Login
                {
                    LoginID = "12345678",
                    CustomerID = 2100,
                    PasswordHash = "YBNbEL4Lk8yMEWxiKkGBeoILHTU7WZ9n8jJSy8TNx0DAzNEFVsIVNRktiQV+I8d2"
                },
                new Login
                {
                    LoginID = "23456789",
                    CustomerID = 2200,
                    PasswordHash = "EehwB3qMkWImf/fQPlhcka6pBMZBLlPWyiDW6NLkAh4ZFu2KNDQKONxElNsg7V04"
                },
                new Login
                {
                    LoginID = "34567890",
                    CustomerID = 2300,
                    PasswordHash = "LuiVJWbY4A3y1SilhMU5P00K54cGEvClx5Y+xWHq7VpyIUe5fe7m+WeI0iwid7GE"
                });

            context.Accounts.AddRange(
                new Account
                {
                    AccountNumber = 4100,
                    AccountType = AccountType.Saving,
                    CustomerID = 2100,
                    Balance = 500,
                    Active = true
                },
                new Account
                {
                    AccountNumber = 4101,
                    AccountType = AccountType.Checking,
                    CustomerID = 2100,
                    Balance = 500,
                    Active = true
                },
                new Account
                {
                    AccountNumber = 4200,
                    AccountType = AccountType.Saving,
                    CustomerID = 2200,
                    Balance = 500.95m,
                    Active = true
                },
                new Account
                {
                    AccountNumber = 4300,
                    AccountType = AccountType.Checking,
                    CustomerID = 2300,
                    Balance = 1250.50m,
                    Active = true
                });

            const string initialDeposit = "Initial deposit";
            const string format = "dd/MM/yyyy hh:mm:ss tt";

            context.Transactions.AddRange(
                new Transaction
                {
                    TransactionType = TransactionType.Deposit,
                    AccountNumber = 4100,
                    Amount = 100,
                    Comment = initialDeposit,
                    ModifyDate = DateTime.ParseExact("08/06/2020 08:00:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = TransactionType.Deposit,
                    AccountNumber = 4100,
                    Amount = 100,
                    Comment = initialDeposit,
                    ModifyDate = DateTime.ParseExact("09/06/2020 09:00:00 AM", format, null)
                },
                new Transaction
                {
                    TransactionType = TransactionType.Deposit,
                    AccountNumber = 4100,
                    Amount = 100,
                    Comment = initialDeposit,
                    ModifyDate = DateTime.ParseExact("09/06/2020 01:00:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = TransactionType.Deposit,
                    AccountNumber = 4100,
                    Amount = 100,
                    Comment = initialDeposit,
                    ModifyDate = DateTime.ParseExact("09/06/2020 03:00:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = TransactionType.Deposit,
                    AccountNumber = 4100,
                    Amount = 100,
                    Comment = initialDeposit,
                    ModifyDate = DateTime.ParseExact("10/06/2020 11:00:00 AM", format, null)
                },
                new Transaction
                {
                    TransactionType = TransactionType.Deposit,
                    AccountNumber = 4101,
                    Amount = 500,
                    Comment = initialDeposit,
                    ModifyDate = DateTime.ParseExact("08/06/2020 08:30:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = TransactionType.Deposit,
                    AccountNumber = 4200,
                    Amount = 500,
                    Comment = initialDeposit,
                    ModifyDate = DateTime.ParseExact("08/06/2020 09:00:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = TransactionType.Deposit,
                    AccountNumber = 4200,
                    Amount = 0.95m,
                    Comment = initialDeposit,
                    ModifyDate = DateTime.ParseExact("08/06/2020 09:00:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = TransactionType.Deposit,
                    AccountNumber = 4300,
                    Amount = 1250.50m,
                    Comment = initialDeposit,
                    ModifyDate = DateTime.ParseExact("08/06/2020 10:00:00 PM", format, null)
                });

            context.Payees.AddRange(
                new Payee
                {
                    PayeeName = "RMIT.EDU",
                    Address = "124 La Trobe St",
                    City = "Melbourne",
                    State = "VIC",
                    PostCode = "3000",
                    Phone = "+61 488888888"
                },
                new Payee
                {
                    PayeeName = "Rent Payment System",
                    Address = "123 Bob St",
                    City = "Melbourne",
                    State = "VIC",
                    PostCode = "3100",
                    Phone = "+61 400000000"
                }
            );

            context.SaveChanges();
        }
    }
}
