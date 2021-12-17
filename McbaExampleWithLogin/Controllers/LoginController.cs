using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mcba.Data;
using Mcba.Models;
using SimpleHashing;

namespace Mcba.Controllers
{
    [Route("/Mcba/SecureLogin")]
    public class LoginController : Controller
    {
        private readonly McbaContext _context;

        public LoginController(McbaContext context) => _context = context;

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string loginID, string password)
        {
            var login = await _context.Logins.FindAsync(loginID);
            var customer = await _context.Customers.FindAsync(login.CustomerID);
            //Check if there is a locked account, if so login will fail
            var inactiveAccountFound = false;
            foreach (var account in customer.Accounts) {
                if (!account.Active)
                    inactiveAccountFound = true;
            }

            if(login == null || !PBKDF2.Verify(login.PasswordHash, password) || inactiveAccountFound)
            { 
                ModelState.AddModelError("LoginFailed", "Login failed, please try again.");
                return View(new Login { LoginID = loginID });
            }

            // Login customer.
            HttpContext.Session.SetInt32(nameof(Customer.CustomerID), login.CustomerID);
            HttpContext.Session.SetString(nameof(Customer.Name), login.Customer.Name);

            return RedirectToAction("Index", "Customer");
        }

        [Route("LogoutNow")]
        public IActionResult Logout()
        {
            // Logout customer.
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }
    }
}
