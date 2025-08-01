using Exchange_appl.Models;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Data;
using System.Windows;

namespace Exchange_appl
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            /* var optionsBuilder = new DbContextOptionsBuilder<ExchangeLiteContext>();
            optionsBuilder.UseSqlite("Data Source='C:\\Documents\\Labs\\код — копия\\Exchange_appl\\ExchangeLite.db'");

            using (var context = new ExchangeLiteContext(optionsBuilder.Options))
            {
                 context.Database.EnsureCreated();
            */
            base.OnStartup(e);

        }
    }

}
