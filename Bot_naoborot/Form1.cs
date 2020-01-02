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
    public partial class Form1 : Form
    {
        Thread mythread = null;
        Thread soundT = null;
        Helper help;

        [DllImportAttribute("winmm.dll")]
        public static extern long PlaySound(String lpszName, long hModule, long dwFlags);
        public Form1()
        {
            InitializeComponent();
            readParametrs();        //Считываем логин и пароль пользователя
        }

        /// <summary>
        /// Считывание логина и пароля с файла
        /// </summary>
        public void readParametrs()
        {
            StreamReader sr = new StreamReader("user.dat");
            email.Text = sr.ReadLine();
            Password.Text = sr.ReadLine();
            sr.Close();
        }

        /// <summary>
        /// Сохранение логина и пароля в файл
        /// </summary>
        public void saveParamets()
        {
            StreamWriter sw = File.CreateText("user.dat");
            sw.WriteLine(email.Text);
            sw.WriteLine(Password.Text);
            sw.Close();
        }

        ~Form1()
        {

        }

        /// <summary>
        /// Функция для вывода в консоль
        /// </summary>
        /// <param name="text"></param>
        public void setConsoleText(String text)
        {
            console.Text = text;
        }

        /*
        void playSound()
        {
            PlaySound("prekol.wav", 1, 1);
        }


        void getData()
        {
            SpeechSynthesizer synth = new SpeechSynthesizer();
            synth.SetOutputToDefaultAudioDevice();
            var voices = synth.GetInstalledVoices(new CultureInfo("ru-RU"));
            synth.SelectVoice(voices[0].VoiceInfo.Name);

            String strInterest = "";    //Переменная для прошлого результата    
            String bufIntrest = "";
            while (true)
            {
                try
                {
                    IWebElement interest = Browser.FindElement(By.ClassName("deal-buttons__text"));
                    //IWebElement strike = Browser.FindElement(By.ClassName("strike-button-console"));

                    bufIntrest = interest.Text;
                }
                catch (Exception e)
                {
                    continue;
                }

                if ((strInterest == "80%" || strInterest == "82%") && bufIntrest != strInterest && (bufIntrest != "80%" && bufIntrest != "82%"))
                {
                    //soundT = new Thread(playSound);
                    //soundT.Start();

                    try
                    {
                        PlaySound("prekol.wav", 1, 1);
                    }
                    catch (Exception e) { continue; }
                    strInterest = bufIntrest;
                }
                else if (strInterest != bufIntrest)
                {
                    strInterest = bufIntrest;
                    synth.Speak(strInterest);
                }

                /*BeginInvoke(new MethodInvoker(delegate
                {
                    lableInterest.Text = strInterest; //Выводим проценты
                    lableStrike.Text = strike.Text;     //Выводим страйк цену
                }));

                // System.Threading.Thread.Sleep(100);   //Задержка что бы не подвисал основной поток
            }}
        */

                
    
/// Запуск бота по болинжеру
private void button1_Click(object sender, EventArgs e)
        {
            //Для повышения быстродействия заранее инициализируем указатели на элементы страницы
            try { help.update(); }catch(Exception ex) { setConsoleText("Браузер не был запущен"); }

            //Создаём бота по болинжеру
            BolingerBands bb = new BolingerBands(help);
            
            //Отображение настроек
            SettingsBolinger form2 = new SettingsBolinger(bb);
            form2.ShowDialog();

            //Запускаем в отдельном потоке работу бота
            mythread = new Thread(bb.start);
            mythread.Start();
        }

        ///Нажатие на кнопку "на повышение"
        

        ///Выключение звука
       // private void button2_Click(object sender, EventArgs e)
        //{
            //PlaySound(null, 0,0);
            //soundT.Abort();
        //}


        ///Открытие браузера и авторизация
        private void button3_Click(object sender, EventArgs e)
        {
            //Сохраним введённые логин и пароль, чтобы они появились при следубщем запуске
            saveParamets();

            //Перешли на сайт
            IWebDriver Browser = new OpenQA.Selenium.Chrome.ChromeDriver();
            Browser.Navigate().GoToUrl("https://pocketoption.com/ru/login");

            //Ввод мейла
            try
            {
                IJavaScriptExecutor jse = Browser as IJavaScriptExecutor;
                String scriptJS = "document.getElementsByName('email')[0].setAttribute('value','" + email.Text + "');";
                jse.ExecuteScript(scriptJS);
            }
            catch (Exception ee) { }

            //Ввод пароля
            try
            {
                IJavaScriptExecutor jse = Browser as IJavaScriptExecutor;
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

            //Создаём помошника для управления ботами
            help = new Helper(this, Browser);
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(mythread != null)
                mythread.Abort();
    
            if(soundT != null)
                soundT.Abort();
    
            Application.Exit();
        }
    }
}
