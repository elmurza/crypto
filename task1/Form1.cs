﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;


namespace Elgamal
{
    public partial class Form1 : Form
    {
        int randP = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) //Шифрование
        {
            int p = 0, g = 0, x = 0;
            if (textBox3.Text == "")
            {
                Random random = new Random();
                for (int i = 0; i < 1000000; i++)
                {
                    randP = random.Next(10000, 100000);
                    if (GenerKey_1_MillerRabinTest(randP, 3) == true)
                    {
                        p = randP;
                        textBox3.Text = Convert.ToString(p);
                        break;
                    }

                }
            }
            else
            {
                p = Convert.ToInt32(textBox3.Text); // Простое число    p = 353;
            }

            if (textBox4.Text == "")
            {
                textBox4.Text = Convert.ToString(GetPRoot(randP));
                //MessageBox.Show(Convert.ToString(GetPRoot(randP)));
                g = Convert.ToInt32(textBox4.Text);
            }
            else
            {
                g = Convert.ToInt32(textBox4.Text); //    g = 50;
            }
            if (textBox5.Text == "")
            {
                Random random = new Random();
                for (int i = 0; i < 1000000; i++)
                {
                    randP = random.Next(10000, p-1);
                    if (NOD(randP, p) == 1)
                    {
                        x = randP;
                        break;
                    }
                }
                textBox5.Text = Convert.ToString(x);
            }
            else
            {
                x = Convert.ToInt32(textBox5.Text); //   x = 15;
            }
                       
            Encrypt(p,g,x);
        }

        private void button4_Click(object sender, EventArgs e) //Дешифрование
        {
            int p = 0, g = 0, x = 0;
            p = Convert.ToInt32(textBox3.Text);  // Простое число    p = 353;
            g = Convert.ToInt32(textBox4.Text);  // g = 50;
            x = Convert.ToInt32(textBox5.Text);  // x = 15;
            Decrypt(p, g, x);
        }

        public string Encrypt(int p, int g, int x) //Шифрование ЭльГамаля
        {
            int  k = 0;
            Random random = new Random();

            BigInteger y = 0, a = 0, p0 = 0, m1 = 0;
            BigInteger[] b = { };
            BigInteger[] r = { };

            string encData="";

            char[] array;

        //Шифрование
            if (IsPrime(p)) // Проверка на простоту
            {
                y = BigInteger.ModPow(g, x, p); // вычисляем y
                textBox7.Text = Convert.ToString(y);
                string data = textBox1.Text; // PlainText

                array = data.ToCharArray();

                k = 20;
                a = BigInteger.ModPow(g, k, p); // вычисляем a
                textBox6.Text = Convert.ToString(a);


                b = new BigInteger[array.Length];
                for (int i = 0; i < array.Length; i++)
                {

                    b[i] = BigInteger.Remainder(BigInteger.Multiply(BigInteger.Pow(y, k), array[i]), p);
                    encData = encData + b[i].ToString() + " ";
                }

                textBox2.Text=(encData);
            }
            else
            {
                MessageBox.Show("Введите простое число");
            }
            return "False";
        }

        public string Decrypt(int p, int g, int x) //Дешифрование ЭльГамаля
        {
            Random random = new Random();

            BigInteger y = 0, a = 0, p0 = 0, m1 = 0;
            // BigInteger[] b = { };
            BigInteger[] r = { };
            a = Convert.ToInt64(textBox6.Text);
            string decData = "";

            //дешифрование
            int j = 0;
            if (IsPrime(p)) // Проверка на простоту
            {
                string data = textBox1.Text; // PlainText
                string[] b = new string[10];
                b = data.Split(' ');

                r = new BigInteger[b.Length];
                
                for (int i = 0; i < b.Length-1; i++)
                {
                    p0 = BigInteger.Subtract(BigInteger.Subtract(p, new BigInteger(1)), x);
                    if (p0 < 0)
                    {
                        p0 = 0-p0;
                    }
                    m1 = BigInteger.ModPow(a, p0, p);
                    r[i] = BigInteger.Remainder(BigInteger.Multiply(m1, Convert.ToInt32(b[i])), p);

                    decData = decData + ((char)r[i]).ToString();
                }
                textBox2.Text= decData;
            }
            else
            {
                MessageBox.Show("Введите простое число");
            }
            return "False";
        }

        public static int NOD(int a, int b) //Взаимно простое 
        {
            if (a == b)
                return a;
            else
                if (a > b)
                return NOD(a - b, b);
            else
                return NOD(b - a, a);
        }

        static bool GenerKey_1_MillerRabinTest(int n, int k)
        {
            if (n <= 1)
                return false;
            if (n == 2)
                return true;
            if (n % 2 == 0)
                return false;
            int s = 0, d = n - 1;
            while (d % 2 == 0)
            {
                d /= 2;
                s++;
            }

            var rand = new Random();

            for (int i = 0; i < k; i++)
            {
                long a = rand.Next(2, n - 1);
                int x = (int)BigInteger.ModPow(a, d, n);
                if (x == 1 || x == n - 1)
                    continue;
                for (int j = 0; j < s - 1; j++)
                {
                    x = (x * x) % n;
                    if (x == 1)
                        return false;
                    if (x == n - 1)
                        break;
                }
                if (x != n - 1)
                    return false;
            }
            return true;

        } // Тест Миллера-Раббина Генерация ключей Этап №1

        public static bool IsPrime(int Number) //Взаимно простое 
        {

            if ((Number & 1) == 0)
            {
                if (Number == 2)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            for (int i = 3; (i * i) <= Number; i += 2)
            {
                if ((Number % i) == 0)
                {
                    return false;
                }
            }
            return Number != 1;
        }

        public BigInteger GetPRoot(BigInteger p) //первообразный корень
        {
            for (BigInteger i = (p/4); i < p; i++)
                if (IsPRoot(p, i))
                    return i;
            return 0;
        }

        public bool IsPRoot(BigInteger p, BigInteger a)
        {
            if (a == 0 || a == 1)
                return false;
            BigInteger last = 1;
            HashSet<BigInteger> set = new HashSet<BigInteger>();
            for (BigInteger i = 0; i < p - 1; i++)
            {
                last = (last * a) % p;
                if (set.Contains(last)) // Если повтор
                    return false;
                set.Add(last);
            }
            return true;
        }


    }
}
