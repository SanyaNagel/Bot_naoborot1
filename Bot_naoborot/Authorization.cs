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
using System.Diagnostics;

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
            PathDownlods.Text = sr.ReadLine();
            sr.Close();
        }

        /// Сохранение логина и пароля в файл
        public void saveParamets()
        {
            StreamWriter sw = File.CreateText("user.dat");
            sw.WriteLine(email.Text);
            sw.WriteLine(Password.Text);
            sw.WriteLine(PathDownlods.Text);
            sw.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Сохраним введённые логин и пароль, чтобы они появились при следубщем запуске
            saveParamets();

            //Создаём BAT файл для удаления старых котировок
            StreamWriter sw = File.CreateText("delFile.bat");
            sw.WriteLine("DEL /F /S /Q /A \"" + PathDownlods.Text+ "\"" );
            sw.Close();

            //Удаление файла с котировками из мосбиржи с помощью созданного BAT файла
            System.Diagnostics.Process.Start("delFile.bat");

            //Открываем браузер 
            Form1.Browser = new OpenQA.Selenium.Chrome.ChromeDriver();
            
            //Скачиваем файл с котировками из мосбиржи
            Form1.Browser.Navigate().GoToUrl("http://export.finam.ru/POLY_170620_170623.txt?market=1&em=175924&code=POLY&apply=0&df=20&mf=5&yf=2017&from=20.06.2017&dt=23&mt=5&yt=2017&to=23.06.2017&p=8&f=POLY_170620_170623&e=.txt&cn=POLY&dtf=1&tmf=1&MSOR=1&mstime=on&mstimever=1&sep=1&sep2=1&datf=1&at=1");
            
            //Переходим на покет
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

        static bool fSettings = false;

        private void button1_Click(object sender, EventArgs e)
        {
            if (fSettings == false)
            {
                this.Height = 256;
                fSettings = true;
            }
            else
            {
                this.Height = 204;
                fSettings = false;
            }
        }
    }
}
