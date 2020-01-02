using System;
using System.Windows.Forms;
using OpenQA.Selenium;
using System.Speech.Synthesis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
namespace Bot_naoborot
{
    ///
    /// Класс позволяющий контролировать бота
    /// 
    
    public class Helper
    {
        IWebDriver Browser;
        IWebElement currentPrice = null;    //Указатель на текущую цену
        IWebElement btnCall = null;         //Указатель на кнопку повышение
        IWebElement btnPut = null;          //Указатель на кнопку понижения
        Form1 form;

        public Helper(Form1 f, IWebDriver brow)
        {
            form = f;
            Browser = brow;
        }

        /// <summary>
        /// Повышение быстродействия
        /// Перед началом работы обновляем указатели на элементы страницы
        /// </summary>
        public void update()
        {
            //Указатель текущей цены
            try
            {
                List<IWebElement> listele = Browser.FindElements(By.ClassName("value__val")).ToList();
                currentPrice = listele.Last();
            }
            catch (Exception er) { }

            //Указатель на кнопку понижения
            try { btnPut = Browser.FindElement(By.CssSelector(".btn.btn-put")); } catch (Exception ex2) { }

            //Указатель на кнопку понижения 
            try { btnCall = Browser.FindElement(By.CssSelector(".btn.btn-call")); } catch (Exception ex2) { }
        
        }

        ///Получение текущей цены
        public double getPrice()
        {///
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
                    form.setConsoleText("Цена не найдена");
                    currentPrice = null;
                    return -1;
                }
            }
        }

        ///Нажатие на кнопку повышения
        public void clickBattonCall()
        {
            try
            {
                btnCall.Click();
            }
            catch (Exception ex) 
            {
                //Вторая попытка нажатия клавиши (для первого нажатия на кнопку)
                try
                {
                    btnCall = Browser.FindElement(By.CssSelector(".btn.btn-call"));
                    btnCall.Click();
                }
                catch(Exception ex2)
                {
                    form.setConsoleText("Ошибка нажатия кнопки на повышение");
                    btnCall = null;
                }           
            }
        }

        ///Нажатие на кнопку понижения
        public void clickBattonPut()
        {
            try
            {
                btnPut.Click();
            }
            catch (Exception ex) 
            {
                //Вторая попытка нажатия клавиши (для первого нажатия на кнопку)
                try
                {
                    btnPut = Browser.FindElement(By.CssSelector(".btn.btn-put"));
                    btnPut.Click();
                }
                catch (Exception ex2)
                {
                    form.setConsoleText("Ошибка нажатия кнопки на понижение");
                    btnCall = null;
                }
            }
        }


    }
}
