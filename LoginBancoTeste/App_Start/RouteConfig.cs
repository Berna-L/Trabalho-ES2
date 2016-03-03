using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace LoginBancoTeste
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);

            routes.MapRoute(
                "Opcoes",
                "Transacoes/Opcoes/{numero}",
                defaults: new { controller = "Transacoes", action = "Opcoes" }
            );

            routes.MapRoute(
                "Deposito",
                "Transacoes/Deposito/{numero}",
                defaults: new { controller = "Transacoes", action = "Deposito" }
            );

            routes.MapRoute(
                "Saldo",
                "Transacoes/Saldo/{numero}",
                defaults: new { controller = "Transacoes", action = "Saldo" }
            );

            routes.MapRoute(
                "Transferencia",
                "Transacoes/Transferencia/{numero}",
                defaults: new { controller = "Transacoes", action = "Transferencia" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Transacoes", action = "Index", id = UrlParameter.Optional }
            );

        }
    }
}
