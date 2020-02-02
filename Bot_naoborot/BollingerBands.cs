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
        public static double MLCacl(List<double> CloseP)
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
        public static double SMACalc(List<double> CloseP)
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

        // метод получения параметров поведения графика болинджера
        // входные параметры: 
        // 1) параметр плоской линии flat - если модуль коэффициента наклона полосы меньше flat, то считаем его = 0
        // 2) список предыдущих n цен закрытия
        // 3) текущая цена закрытия
        // 4) кол-во стандартных отклонений для линий болинджера
        // 5) время с момента закрытия последней свечи в секундах
        // результат: массив значений double: delta, верхнее направление, нижнее направление
        public static double [] GBAP(double flat, List<double> CloseP, double currentP, int D, int timeAfterClosing) // GBA = get bollinger analysis parameters
        {
            CloseP.Add(currentP); // добавить текущую цену в список цен
            double delta = TLCalc(CloseP, D) - BLCalc(CloseP, D); // получить разность верхней и нижней полосы
            double tk = currentP - CloseP[CloseP.Count - 1] / (Convert.ToDouble(timeAfterClosing + 1) / 60); // получить коэффициент наклона верхней прямой
            double bk = CloseP[CloseP.Count - 1] - currentP / (Convert.ToDouble(timeAfterClosing + 1) / 60); // получить коэффициент наклона нижней прямой
            // если незначительные отклонения, то принять их за 0
            if (Math.Abs(tk) < flat)
                tk = 0;
            if (Math.Abs(bk) < flat)
                bk = 0;
            return new double[] { delta, tk, bk }; // вернуть полученный значения в виде массива
        }

        // метод анализа поведения графика болинджера по параметрам боллинджера
        // входные параметры:
        // 1) массив значений double: delta, верхнее направление, нижнее направление
        // 2) значение максимально допустимого расширения коридора
        // результат: команда боту nothing <=> не ставить, all <=> можно ставить в любом направлении, up <=> ставить только вверх, down <=> ставить только вниз
        public static string DBA(double [] parameters, double prevDelta, double maxDelta) // do bollinger analyze
        {
            // проинциализзируем параметры явно
            double delta = parameters[0];
            double tk = parameters[1];
            double bk = parameters[2];

            // анализ поведения графика
            string result = "nothing";
            if (tk == 0 && bk == 0 && delta <= maxDelta) // если труба, то ставка в любую сторону
                result = "all";
            else if (tk > 0 && bk > 0 && delta <= maxDelta) // если коридор вверх, то ставки только на повышение
                result = "up";
            else if (tk < 0 && bk < 0 && delta <= maxDelta) // если коридор вниз, то ставки только на понижение
                result = "down";
            return result; //вернуть результат
        }
    }
}
