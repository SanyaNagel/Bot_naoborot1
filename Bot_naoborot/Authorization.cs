using System;
using System.Windows.Forms;
using OpenQA.Selenium;
using System.Threading;
using System.Speech.Synthesis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Bot_naoborot
{
    public partial class Authorization : Form
    {
        public Authorization()
        {
            InitializeComponent();
            readParametrs();        //Считываем логин и пароль пользователя
        }

        /// Считывание логина и пароля с файла
        public void readParametrs()
        {
            StreamReader sr = new StreamReader("user.dat");
            email.Text = sr.ReadLine();
            Password.Text = sr.ReadLine();
            sr.Close();
        }

        /// Сохранение логина и пароля в файл
        public void saveParamets()
        {
            StreamWriter sw = File.CreateText("user.dat");
            sw.WriteLine(email.Text);
            sw.WriteLine(Password.Text);
            sw.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Сохраним введённые логин и пароль, чтобы они появились при следубщем запуске
            saveParamets();

            //Перешли на сайт
            Form1.Browser = new OpenQA.Selenium.Chrome.ChromeDriver();
            Form1.Browser.Navigate().GoToUrl("https://pocketoption.com/ru/login");

            //Ввод мейла
            try
            {
                IJavaScriptExecutor jse = Form1.Browser as IJavaScriptExecutor;
                String scriptJS = "document.getElementsByName('email')[0].setAttribute('value','" + email.Text + "');";
                jse.ExecuteScript(scriptJS);
            }
            catch (Exception ee) { }

            //Ввод пароля
            try
            {
                IJavaScriptExecutor jse = Form1.Browser as IJavaScriptExecutor;
                String scriptJS = "document.getElementsByName('password')[0].setAttribute('value','" + Password.Text + "');";
                jse.ExecuteScript(scriptJS);
            }
            catch (Exception ee) { }

            //Проверка на бота
            SendKeys.SendWait("{TAB}");
            SendKeys.SendWait("{TAB}");
            SendKeys.SendWait("{TAB}");
            SendKeys.SendWait("{TAB}");

            System.Threading.Thread.Sleep(500);    //Задержка чтобы появилась капча 

            //SendKeys.Send("{ENTER}");

            /*System.Threading.Thread.Sleep(1000);    //Задержка чтобы появилась капча 
            
            //Нажимаем войти
            try
            {   //В случае если не будет появлятся капча
                IWebElement reg = Browser.FindElement(By.ClassName("btn-block"));
                reg.Click();
            }catch(Exception eee) { }*/
            this.Close();
        }
    }
}
