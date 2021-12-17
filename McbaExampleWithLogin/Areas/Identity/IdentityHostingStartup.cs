using System;
using McbaExample.Areas.Identity.Data;
using McbaExample.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(McbaExample.Areas.Identity.IdentityHostingStartup))]
namespace McbaExample.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<McbaExampleContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("McbaExampleContextConnection")));

                services.AddDefaultIdentity<McbaExampleUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<McbaExampleContext>();
            });
        }
    }
}