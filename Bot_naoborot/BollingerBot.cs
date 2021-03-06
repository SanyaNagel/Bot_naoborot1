﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace Bot_naoborot
{
    public class BollingerBot
    {
        private Helper help;
        private List<double> closePrices;
        private int period = 9;
        private string currCurrency;    //Текущая валюта

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
            currCurrency = help.getCurrency();  //Получаем текущую валюту
            newCurrency();  //загружаем 9 закрытий свечей 
        }

        ///Новая валюта, загружаем в болинжер последние 9 свечей
        public void newCurrency()
        {
            //Создаём BAT файл для удаления старых котировок
            StreamWriter sw = File.CreateText("delFile.bat");
            string nameFile = help.getPathDownlods() + "\\" + help.getNameFile(help.getCurrency());

            sw.WriteLine("DEL /F /S /Q /A \"" + nameFile + "\"");
            sw.Close();

            //Удаление файла с котировками из мосбиржи с помощью созданного BAT файла
            System.Diagnostics.Process.Start("delFile.bat");

            //Скачиваем файл с котировками из мосбиржи
            string currency_ = help.getCurrency();
            help.getFileCotir(currency_);
            if (currency_ == "NZDCHF")
                return;

            System.Threading.Thread.Sleep(3000);

            //Извлекаем 9 последних закрытий из полученного файла
            //Получаем размер файла
            int sizeFile = getNumberString(nameFile);
            StreamReader sr = new StreamReader(nameFile);
            while(sizeFile-- > 10)
                sr.ReadLine();
            
            string line;
            while((line = sr.ReadLine()) != null)
            {
                string[] word = line.Split(' ');
                addClosePrice(Convert.ToDouble(word[4].Replace(".", ",")));
            }
            sr.Close();

        }
        
        ///Получение количества строк в файле (эффективный алгоритм)
        public int getNumberString(String nameFile)
        {
            var linesCount = 1;
            int nextLine = '\n';
            using (var streamReader = new StreamReader(
                new BufferedStream(
                    File.OpenRead(nameFile), 10 * 1024 * 1024))) // буфер в 10 мегабайт
            {
                while (!streamReader.EndOfStream)
                {
                    if (streamReader.Read() == nextLine) linesCount++;
                }
            }
            return linesCount;
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

        //Добавление текущей цены каждую секунду для плавющего боллинжера
        public void addCurPrice(double price)
        {
            closePrices.RemoveAt(8);
            closePrices.Add(price);
        }

        ///Каждую секунду вызывается метод болинжера
        public void tickTimerBB()
        {
            mutexObj1.WaitOne();
            if (++counter == 60)    //Каждую минуту
            {
                addClosePrice(help.getPrice()); //Добавляем в конец очереди 
                counter = 0;

            }
            else    //Если минута не прошла, то для плавающего болинжера меняем последнюю цену 
            {
                //mutexObj1.WaitOne();
                addCurPrice(help.getPrice());   //Добавление текущей цены для плавоющего болинжера
                //mutexObj1.ReleaseMutex();
            }
            mutexObj1.ReleaseMutex();
           
            mutexObj1.WaitOne();
            if (stopLimit > 0)  //Параметр определяющий интервал подряд идущих ставок
            {
                --stopLimit;
            }
            mutexObj1.ReleaseMutex();
        }

        ///Главная функция бота
        public void start()
        {

            while (true)
            {
                //Получаем новую текущую валюту
                string newCurCurrency = help.getCurrency();

                //Если мы не можем получить текущую валюту, значит мы ушли с графика
                if (newCurCurrency == null)
                    continue;   //Пропускаем работу бота

                //Если появилась новая валюта
                if (currCurrency != newCurCurrency)
                {
                    newCurrency();  //То обновляем 9 свечей на новые
                    currCurrency = newCurCurrency;  //И новая валюта становится текущей
                }

                if (currCurrency == "NZDCHF")
                    continue;


                logikClaw(BollingerBands.DBA(BollingerBands.GBAP(0.5, closePrices, 2, counter), 0.3));

                double curPrice = help.getPrice();  //Получаем текущую цену

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

        //Функция устанавливающая разрешения на ставки, в зависимсоти от клешень
        public void logikClaw(string resultGBAP)
        {
            //Ставить можно в обе стороны
            if (resultGBAP == "all")
            {
                help.setCallСlaw(true);
                help.setPutClaw(true);
            }

            //Ставить только вверх
            if(resultGBAP == "up")
            {
                help.setCallСlaw(true);
                help.setPutClaw(false);
            }

            if (resultGBAP == "down")
            {
                help.setCallСlaw(false);
                help.setPutClaw(true);
            }
        }
    }
}
