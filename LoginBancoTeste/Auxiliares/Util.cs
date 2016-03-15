using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoginBancoTeste.Auxiliares {
    public class Util {

        public static string ConversorReal(long valor) {
            string reais, centavos, negativo = "";
            if (valor < 0) {
                negativo = "-";
            }
            if (valor > 99) {
                reais = negativo + (valor / 100) + ",";
            } else {
                reais = negativo + "0,";
            }
            if (valor % 100 > 9) {
                centavos = (valor % 100).ToString();
            } else {
                centavos = "0" + (valor % 100);
            }
            return (reais + centavos);
        }

    }
}