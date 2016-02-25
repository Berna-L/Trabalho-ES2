using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoginBancoTeste.Models {
    public class TipoInvestimento {

        public int id { get; set; }

        public String nome { get; set; }

        public double jurosDia { get; set; }

    }



    public class TipoInvestimentoAux {
        
        public static double CalcularRendimento(Investimento invest, DateTime data_ini, DateTime data_final) {
            if (data_ini > data_final) {
                return -1;
            }
            TimeSpan periodo = data_final - data_ini;
            return invest.valor_ini * Math.Pow(invest.tipo_invest.jurosDia, Math.Floor(periodo.TotalDays));
        }

        public static double CalcularRendimento(double valor, TipoInvestimento tipo, DateTime data_ini, DateTime data_final) {
            if (data_ini > data_final) {
                return -1;
            }
            TimeSpan periodo = data_final - data_ini;
            return valor * Math.Pow(tipo.jurosDia, Math.Floor(periodo.TotalDays));
        }


    }
}