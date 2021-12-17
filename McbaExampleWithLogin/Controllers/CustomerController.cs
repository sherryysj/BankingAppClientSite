using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mcba.Data;
using Mcba.Models;
using Mcba.Utilities;
using McbaWithLogin.Filters;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using X.PagedList;

namespace Mcba.Controllers
{
    [AuthorizeCustomer]
    public class CustomerController : Controller
    {
        private readonly McbaContext _context;

        // ReSharper disable once PossibleInvalidOperationException
        private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

        public CustomerController(McbaContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var customer = await _context.Customers.Include(x => x.Accounts).
                FirstOrDefaultAsync(x => x.CustomerID == CustomerID);

            return View(customer);
        }

        // View page jump controls
        public async Task<IActionResult> Deposit(int id) => View(await _context.Accounts.FindAsync(id));
        public async Task<IActionResult> Withdraw(int id) => View(await _context.Accounts.FindAsync(id));
        public async Task<IActionResult> Transfer(int id) => View(await _context.Accounts.FindAsync(id));
        public async Task<IActionResult> BillPayState(int id) => View(await _context.Accounts.FindAsync(id));
        public async Task<IActionResult> PayBill() => View(await _context.Customers.FindAsync(CustomerID));

        public async Task<IActionResult> Statement(int id, int? page) {
            var account = await _context.Accounts.FindAsync(id);
            var pageNumber = page ?? 1;
            ViewBag.AccountForStatement = account;

            const int pageSize = 4;
            var pagedList = await _context.Transactions.Where(x => x.AccountNumber == account.AccountNumber).
               ToPagedListAsync(pageNumber, pageSize);
            return View(pagedList);
        }     
        
        public async Task<IActionResult> BillPayModify(int id) {
            var billPay = await _context.BillPays.FindAsync(id);
            ViewBag.Amount = billPay.Amount;
            return View(billPay);     
        }

        // deposit and update database
        [HttpPost]
        public async Task<IActionResult> Deposit(int id, decimal amount)
        {
            var account = await _context.Accounts.FindAsync(id);

            // amount input validation
            if (amount <= 0)
                ModelState.AddModelError(nameof(amount), "Amount must be positive.");
            if(amount.HasMoreThanTwoDecimalPlaces())
                ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");
            
            if(!ModelState.IsValid)
            {
                ViewBag.Amount = amount;
                return View(account);
            }

            // update model data according deposit
            account.Balance += amount;
            account.ModifyDate = DateTime.UtcNow;
            account.Transactions.Add(
                new Transaction
                {
                    TransactionType = TransactionType.Deposit,
                    Amount = amount,
                    ModifyDate = DateTime.UtcNow
                });

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // withdraw and update database
        [HttpPost]
        public async Task<IActionResult> Withdraw(int id, decimal amount)
        {
            var account = await _context.Accounts.FindAsync(id);

            // amount input validation, check whether account have enough money
            if (amount <= 0)
                ModelState.AddModelError(nameof(amount), "Amount must be positive.");
            if (amount.HasMoreThanTwoDecimalPlaces())
                ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");
            if (amount > account.Balance)
                ModelState.AddModelError(nameof(amount), "There is no enough money in this account.");
            
            if (!ModelState.IsValid)
            {
                ViewBag.Amount = amount;
                return View(account);
            }

            // update model data according deposit
            account.Balance -= amount;
            account.ModifyDate = DateTime.UtcNow;
            account.Transactions.Add(
                new Transaction
                {
                    TransactionType = TransactionType.Withdraw,
                    Amount = amount,
                    ModifyDate = DateTime.UtcNow
                });

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // transfer and update database
        [HttpPost]
        public async Task<IActionResult> Transfer(int id, decimal amount, int destinationAccountNumber, string comment)
        {
            // check whether destination account is the same as the account user want to transfer money from
            if (destinationAccountNumber == id)
                ModelState.AddModelError(nameof(destinationAccountNumber), "You cannot transfer money between the same account.");

            // check whether destination account is existing
            var destinationAccount = await _context.Accounts.FindAsync(destinationAccountNumber);
            if (destinationAccount == null)
                ModelState.AddModelError(nameof(destinationAccountNumber), "The destination account is not exist, please enter a right account number.");

            // check whether comment is too long
            if (comment.Length > 255)
            {
                ModelState.AddModelError(nameof(comment), "The comment is too long, please keep it in 255 letters.");
            }

            var account = await _context.Accounts.FindAsync(id);

            // amount input validation, check whether out account have enough money
            if (amount <= 0)
                ModelState.AddModelError(nameof(amount), "Amount must be positive.");
            if (amount.HasMoreThanTwoDecimalPlaces())
                ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");
            if (amount > account.Balance)
                ModelState.AddModelError(nameof(amount), "There is no enough money in this account.");
            
            if (!ModelState.IsValid)
            {
                ViewBag.Amount = amount;
                return View(account);
            }

            // update model data according transafer, add transaction to both transfer out and transfer in account
            account.Balance -= amount;
            destinationAccount.Balance += amount;
            account.ModifyDate = DateTime.UtcNow;
            destinationAccount.ModifyDate = DateTime.UtcNow;
            account.Transactions.Add(
                new Transaction
                {
                    TransactionType = TransactionType.Transfer,
                    Amount = amount,
                    DestinationAccountNumber = destinationAccountNumber,
                    ModifyDate = DateTime.UtcNow,
                    Comment = comment
                });
            destinationAccount.Transactions.Add(
                new Transaction
                {
                    TransactionType = TransactionType.Transfer,
                    Amount = amount,
                    ModifyDate = DateTime.UtcNow,
                    Comment = comment
                });

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
            
        }

        // paybill and update database
        [HttpPost]
        public async Task<IActionResult> PayBill(int accountNumber, int payeeID, decimal amount, DateTime scheduleDate, string period)
        {

            // check whether payee is existing
            var payee = await _context.Payees.FindAsync(payeeID);
            if (payee == null)
                ModelState.AddModelError(nameof(payeeID), "The payee is not exist, please enter a right payee ID.");

            // check whether shedule date is early than current date
            int dateCompare = DateTime.Compare(scheduleDate, DateTime.Today);
            if (dateCompare < 0)
            {
                ModelState.AddModelError(nameof(scheduleDate), "Please schedule a date not earlier than today");
            }

            var account = await _context.Accounts.FindAsync(accountNumber);

            // amount input validation
            if (amount <= 0)
                ModelState.AddModelError(nameof(amount), "Amount must be positive.");
            if (amount.HasMoreThanTwoDecimalPlaces())
                ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");
            
            if (!ModelState.IsValid)
            {
                ViewBag.Amount = amount;
                var customer = await _context.Customers.Include(x => x.Accounts).
                    FirstOrDefaultAsync(x => x.CustomerID == CustomerID);
                return View(customer);
            }

            // update model data according billpay
            account.BillPays.Add(
                    new BillPay
                    {
                        PayeeID = payeeID,
                        Amount = amount,
                        ScheduleDate = scheduleDate,
                        ModifyDate = DateTime.UtcNow,
                        Period = period,
                        Active = true
                    });

            await _context.SaveChangesAsync();

            // jump to billpay statement page after submit billpay
            return RedirectToAction(nameof(BillPayState), new { id = account.AccountNumber });

        }

        // billpay delete and update database
        public async Task<IActionResult> BillPayDelete(string idString, string accountNumberString)
        {
            int id = Int32.Parse(idString);
            int accountNumber = Int32.Parse(accountNumberString);

            try {
                var billPay = await _context.BillPays.FindAsync(id);

                _context.BillPays.Remove(billPay);

                await _context.SaveChangesAsync();
            } catch
            {
                // refresh billpay statement page if delete billpay fail
                return RedirectToAction(nameof(BillPayState), new { id = accountNumber });
            }

            // refresh billpay statement page after delete billpay
            return RedirectToAction(nameof(BillPayState), new { id = accountNumber });
        }

        // billpay modify and update database
        [HttpPost]
        public async Task<IActionResult> BillPayModify(int billPayID, decimal amount, DateTime scheduleDate, string period)
        {
            
            var billPay = await _context.BillPays.FindAsync(billPayID);

            // date validation, forbid user choose a date before today
            int dateCompare = DateTime.Compare(scheduleDate, DateTime.Today);
            if (dateCompare < 0)
            {
                ModelState.AddModelError(nameof(scheduleDate), "Please schedule a date not earlier than today");
            }
            
            var account = await _context.Accounts.FindAsync(billPay.AccountNumber);

            // amount input validation
            if (amount <= 0)
                ModelState.AddModelError(nameof(amount), "Amount must be positive.");
            if (amount.HasMoreThanTwoDecimalPlaces())
                ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");
            if (!ModelState.IsValid)
            {
                ViewBag.Amount = amount;
                return View(billPay);
            }

            // update model data according billpay
            billPay.ScheduleDate = scheduleDate;
            billPay.ModifyDate = DateTime.UtcNow;
            billPay.Amount = amount;
            billPay.Period = period;

            await _context.SaveChangesAsync();

            // jump to billpay statement page after modify a billpay
            return RedirectToAction(nameof(BillPayState), new { id = billPay.AccountNumber });
        }

    }
}
