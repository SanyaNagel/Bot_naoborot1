using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bot_naoborot
{
    class BollingerBands
    {
        // поля 
       // private double ML { get { return ML; } set { ML = value; } } // значение средней волны
       // private double TL { get { return TL; } set { TL = value; } } // значение верхней линии
       // private double BL { get { return BL; } set { BL = value; } } // значение нижней линии
        // методы вычисления
        // вычисление ml, параметры - список цен закрытия
        public static double MLCacl(List <double> CloseP)
        {
            return SMACalc(CloseP);  //вычислить значение BB
        }
        
        
        
        // вычисление tl, параметры - кол-во стандартных отклонений 
        public static double TLCalc(List<double> CloseP, int D)
        {
            return SMACalc(CloseP) + D * StdDevCalc(CloseP);
        }
        public static double BLCalc(List<double> CloseP, int D)
        {
            return SMACalc(CloseP) - D * StdDevCalc(CloseP);
      
        }
        
        
        
        // метод вычисления SMA при заданном списке цен закрытия
        public static double SMACalc(List <double> CloseP)
        {
            if (CloseP.Count() != 0)
                return CloseP.Sum() / CloseP.Count();  //вычислить значение SMA
            else
                return 0;
        }
        // метод вычисления StdDev стандартных отклонений при заданных CloseP
        public static double StdDevCalc(List<double> CloseP)
        {
            double SMA = SMACalc(CloseP); //сохранение sma-парметра
            double StdDev = 0;
            foreach (double close in CloseP) {
                StdDev += (close - SMA) * (close - SMA);
            }
            StdDev /= CloseP.Count();
            StdDev = Math.Sqrt(StdDev);
            return StdDev;
        }
    }
}
