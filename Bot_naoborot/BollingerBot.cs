using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Bot_naoborot
{
    public class BollingerBot
    {
        private Helper help;
        private List<double> closePrices;
        private int period = 9;

        //Настройки бота
        public double depart = 0;  //Коэфициент вылета за полосу болинжера
        private int stopLimit = 0;
        private int counter = 59;
        public int limitTime;
        public static Mutex mutexObj1 = new Mutex();

        public BollingerBot(Helper hp)
        {
            closePrices = new List<double>();
            help = hp;
        }

        /// Добавление цены закрытия в список
        public void addClosePrice(double price)
        {
            if (closePrices.Count() < period)
            {
                closePrices.Add(price);
            }
            else
            {
                closePrices.RemoveAt(0);
                closePrices.Add(price);
            }
        }

        public void tickTimerBB()
        {
            if (++counter == 60)
            {
                mutexObj1.WaitOne();
                addClosePrice(help.getPrice());
                mutexObj1.ReleaseMutex();
                counter = 0;

            }

            mutexObj1.WaitOne();
            if (stopLimit > 0)
            {
                --stopLimit;
            }
            mutexObj1.ReleaseMutex();
        }

        public void start()
        {
            while (true)
            {
                if (closePrices.Count() < 1)
                    continue;

                double curPrice = help.getPrice();

                if (curPrice > BollingerBands.TLCalc(closePrices, 2) + depart)
                {
                    if (stopLimit == 0)
                    {
                        help.clickBattonPut();
                        stopLimit = limitTime;
                    }
                }

                if (curPrice < BollingerBands.BLCalc(closePrices, 2) - depart)
                {
                    if (stopLimit == 0)
                    {
                        help.clickBattonCall();
                        stopLimit = limitTime;
                    }
                }
                
            }
        }


    }
}
