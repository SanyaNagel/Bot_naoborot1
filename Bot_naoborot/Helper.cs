using System;
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
        public bool call = true;
        public bool put = true;
        public static Mutex mutexObj = new Mutex();
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
            if (call == false)  //Проверка на разрешимость ставить на повышение
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
            if (put == false)    //Проверка на разрешимость ставить на понижение
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
    }
}
