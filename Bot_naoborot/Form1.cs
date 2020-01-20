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
using System.Drawing;
using System.Media;

namespace Bot_naoborot
{
    public partial class Form1 : Form
    {
        Thread mythread = null;
        Thread soundT = null;
        Helper help;
        BollingerBot bb = null;
        public static IWebDriver Browser;

        public Form1()
        {
            InitializeComponent();
            
            //Открытия браузера и вход в профиль
            Authorization input = new Authorization();
            input.ShowDialog();

            //Создаём помошника для управления ботами
            help = new Helper(this, Browser);
        }
       
        /// Функция для вывода в консоль
        public void setConsoleText(String text)
        {
            console.Text = text;
        }
    
        private void settingsBot()
        {
            String numb = Convert.ToString(help.getPrice());
            int find = numb.IndexOf(",") + 1;
            double dnumb = Convert.ToInt32(departure.Text) * Math.Pow(10, -(numb.Substring(find, numb.Length - find).Length));


            //Сохраняем настройки бота
            bb.depart = dnumb;
            bb.limitTime = Convert.ToInt32(timeLimit.Text);
        }


        /// Запуск бота по болинжеру
        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.BackColor != Color.Green)
                button1.BackColor = Color.Green;
            else
                button1.BackColor = Color.WhiteSmoke;

            //Для повышения быстродействия заранее инициализируем указатели на элементы страницы
            try { help.update(); }catch(Exception ex) { setConsoleText("Браузер не был запущен"); }


            //Создаём бота по болинжеру
            if (bb == null)
            {
                bb = new BollingerBot(help);
                settingsBot();
                mythread = new Thread(bb.start);
                mythread.Start();
                timer1.Enabled = true;
            }
            else
            {
                if (mythread != null)
                    mythread.Abort();

                if (soundT != null)
                    soundT.Abort();

                timer1.Enabled = false;
            }

        }

        private void button2_Click(object sender, EventArgs e){}

        ///Событие по закрытию формы
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(mythread != null)
                mythread.Abort();
    
            if(soundT != null)
                soundT.Abort();
    
            Application.Exit();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            bb.tickTimerBB();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            help.call = checkBox1.Checked;
            if (checkBox1.Checked == false)
                setConsoleText("false");
            else
                setConsoleText("True");
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            help.put = checkBox2.Checked;
            if (checkBox2.Checked == false)
                setConsoleText("false");
            else
                setConsoleText("True");
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            help.put = checkBox3.Checked;
            help.call = checkBox3.Checked;
            checkBox1.Checked = checkBox3.Checked;
            checkBox2.Checked = checkBox3.Checked;

            if (checkBox3.Checked == false)
                setConsoleText("false");
            else
                setConsoleText("True");
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            settingsBot();
        }

    }
}
