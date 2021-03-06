﻿using System;
using System.Windows.Forms;
using OpenQA.Selenium;
using System.Speech.Synthesis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Media;

namespace Bot_naoborot
{
    ///
    /// Класс позволяющий контролировать бота
    /// 
    
    public class Helper
    {
        IWebDriver Browser;
        IWebElement currentPrice = null;    //Указатель на текущую цену
        Form1 form;

        public SoundPlayer sp;
        public SoundPlayer egp;
        public static Mutex mutexObj = new Mutex();
        public SpeechSynthesizer synth;

        public bool call = true;        //Пользовательское разрешение ставок (На повышение)
        public bool put = true;         //Пользовательское разрешение ставок (На понижение)
        public bool callСlaw = true;    //Зависимость от логики клешень боллинжера (На повышение)
        public bool putСlaw = true;     //Зависимость от логики клешень боллинжера (На понижение)
        public Helper(Form1 f, IWebDriver brow)
        {
            form = f;
            Browser = brow;
            
            sp = new SoundPlayer();
            sp.SoundLocation = "alert.wav";
            sp.LoadAsync();

            egp = new SoundPlayer();
            egp.SoundLocation = "disconnect.wav";
            egp.LoadAsync();

            synth = new SpeechSynthesizer();
            synth.SetOutputToDefaultAudioDevice();
            var voices = synth.GetInstalledVoices(new CultureInfo("ru-RU"));
            synth.SelectVoice(voices[0].VoiceInfo.Name);
        }

        /// <summary>
        /// Повышение быстродействия
        /// Перед началом работы обновляем указатели на элементы страницы
        /// </summary>
        public void update()
        {
            mutexObj.WaitOne();
            //Указатель текущей цены
            try
            {
                List<IWebElement> listele = Browser.FindElements(By.ClassName("value__val")).ToList();
                currentPrice = listele.Last();
            }
            catch (Exception er) { }
            mutexObj.ReleaseMutex();
        }

        ///Получение текущей валюты
        public string getCurrency()
        {
            try
            {
                string cotir = Browser.FindElement(By.ClassName("current-symbol")).Text;
                if (cotir.Length > 7)   //Случай когда после цены ещё пишут ОТС
                    return cotir.Remove(3, 1).Remove(6);
                else
                    return cotir.Remove(3, 1);
            }
            catch (Exception ex)
            {
                form.BeginInvoke(new MethodInvoker(delegate
                {
                    form.setConsoleText("Текущая валюта не найдена");
                }));
                return null;
            }
        }

        public string get_EM_CurCotir(string curCotir)
        {
            StreamReader sr = new StreamReader("EmID.txt");
            string line;
            while((line = sr.ReadLine()) != null)
            {
                string[] word = line.Split(' ');
                if (word[0] == curCotir)
                {
                    sr.Close();

                    if (word[1] == "------") //Если это NZDCHF то мы её не можем получить из мосбиржи
                    {
                        synth.Speak("Ошибка, не могу получить валюту NZD CHF");
                        return null;
                    }
                    return word[1];
                }
            }
            sr.Close();
            return null;
        }

        //Получение имени файла который нужно будет скачать
        public string getNameFile(string currCotir)
        {
            DateTime curDate = DateTime.Now;                //Текущая дата
            DateTime prevDate = DateTime.Now.AddDays(-1);   //День назад

            return currCotir + "_" + prevDate.Year.ToString() + prevDate.Month.ToString() + prevDate.Day.ToString() + '_' + 
                curDate.Year.ToString() + curDate.Month.ToString() + curDate.Day.ToString() + ".txt";
        }

        //Функция скачивает файл последних катировок (на вход текущая валюта)
        public void getFileCotir(string currCotir)
        {
            DateTime curDate = DateTime.Now;                //Текущая дата
            DateTime prevDate = DateTime.Now.AddDays(-1);   //День назад
            string em = get_EM_CurCotir(currCotir);

            //В случае если попалась валюта NZDCHF которой нет на мосбирже
            if (em == null)
                return;

            Form1.Browser.Navigate().GoToUrl(
                "http://export.finam.ru/" + getNameFile(currCotir) +"?market=5" + "&em=" + em + "&code=" + currCotir + "&apply=0&df=" + prevDate.Day.ToString() +
                    "&mf=1&yf=" + curDate.Year.ToString() + "&from=" + prevDate.Day.ToString() + "." + prevDate.Month.ToString() + "." + prevDate.Year.ToString() + 
                    "&dt=" + curDate.Day.ToString() + "&mt=1&yt=" + curDate.Year.ToString() + "&to=" + curDate.Day.ToString() + "." + curDate.Month.ToString() + 
                    "." + curDate.Year.ToString() + "&p=2&f=" + currCotir + "_" + prevDate.Year.ToString() + prevDate.Month.ToString() + prevDate.Day.ToString() + 
                    "_" + curDate.Year.ToString() + curDate.Month.ToString() + curDate.Day.ToString() + "&e=.txt&cn=" + 
                    currCotir + "&dtf=1&tmf=1&MSOR=1&mstimever=0&sep=5&sep2=1&datf=4"
                );
        }

        //Получение пути до папки "загрузки"
        public string getPathDownlods()
        {
            StreamReader sr = new StreamReader("user.dat");
            sr.ReadLine();
            sr.ReadLine();
            string str = sr.ReadLine();
            sr.Close();
            return str;
        }

        ///Получение текущей цены
        public double getPrice()
        {
            try
            {
                if (currentPrice == null)
                {
                    List<IWebElement> listele = Browser.FindElements(By.ClassName("value__val")).ToList();
                    currentPrice = listele.Last();
                }
                return Convert.ToDouble(currentPrice.Text.Replace(".",","));
            }
            catch (Exception er) 
            {
                //Вторая попытка получить цену (для получения цены в первый раз)
                try
                {
                    List<IWebElement> listele = Browser.FindElements(By.ClassName("value__val")).ToList();
                    currentPrice = listele.Last();
                    return Convert.ToDouble(currentPrice.Text.Replace(".", ","));
                }
                catch (Exception ex)
                {
                    form.BeginInvoke(new MethodInvoker(delegate
                    { 
                        form.setConsoleText("Цена не найдена");             
                    }));
                    
                    egp.Play(); //Звук ошибки

                    currentPrice = null;
                    return -1;
                }
            }
        }

        ///Нажатие на кнопку повышения
        public void clickBattonCall()
        {
            mutexObj.WaitOne();
            if (call == false || callСlaw == false)  //Проверка на разрешимость ставить на повышение
                return;

            try
            {
                IJavaScriptExecutor jse = Browser as IJavaScriptExecutor;
                jse.ExecuteScript("document.querySelector('.btn.btn-call').click();");
            }
            catch (Exception ex) { }           
            
            sp.Play();  //Звук сделанной ставки
            
            mutexObj.ReleaseMutex();
        }

        ///Нажатие на кнопку понижения
        public void clickBattonPut()
        {
            mutexObj.WaitOne();
            if (put == false || putСlaw == false)    //Проверка на разрешимость ставить на понижение
                return;

            try
            {
                IJavaScriptExecutor jse = Browser as IJavaScriptExecutor;
                jse.ExecuteScript("document.querySelector('.btn.btn-put').click();");
            }
            catch (Exception ex) { }

            sp.Play();  //Звук сделанной ставки
            
            mutexObj.ReleaseMutex();
        }

        //Установка разрешимости ставок по боллинжеровским клешням
        public void setCallСlaw(bool value)
        {
            callСlaw = value;
        }
        public void setPutClaw(bool value)
        {
            putСlaw = value;
        }
    }
}
