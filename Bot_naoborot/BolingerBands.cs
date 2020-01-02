using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot_naoborot
{
    public class BolingerBands
    {
        private Helper help;

        public double bottom = 1.7000;
        public double top = 1.7100;
        public double depart = 0;  //Коэфициент вылета за полосу болинжера
        public BolingerBands(Helper hp)
        {
            help = hp;
        }

        public void start()
        {
            while (true)
            {
                double curPrice = help.getPrice();
                if (curPrice > top)
                    help.clickBattonPut();
                if (curPrice < bottom)
                    help.clickBattonCall();
            }
        }
    }
}
