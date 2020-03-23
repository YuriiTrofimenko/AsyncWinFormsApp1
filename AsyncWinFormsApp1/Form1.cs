using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AsyncWinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void Button1_Click(object sender, EventArgs e)
        {
            // BeginGetData("1.txt");
            // await GetDataAsync("1.txt");
            string resultString = await GetDataAsyncRun("2.txt");
            // Console.WriteLine(resultString);
            textBox1.Text = resultString.Count().ToString();
        }

        // Метод асинхронного чтения текста из файла,
        // успешно выводит текст в текстбокс,
        // но перед этим занимает главный поток выполнения
        async private Task GetDataAsync(string filename)
        {
            //готовим массив для приема прочитанных данных
            byte[] data = null;
            //создаем поток для чтения
            using (FileStream fs = File.Open(filename, FileMode.Open))
            {
                //создаем массив для чтения
                data = new byte[fs.Length];
                //вызываем встроенный в класс FileStream async
                //метод ReadAsync по имени этого метода ясно,
                //что он асинхронный, значит, вызывать его
                //надо со спецификатором await
                await fs.ReadAsync(data, 0, (int)fs.Length);
            }
            //преобразовываем прочитанные данные из байтового
            //массива в строку и отображаем в текстовом поле Result
            // fib(40);
            Console.WriteLine(System.Text.Encoding.UTF8.GetString(data));
            textBox1.Text = System.Text.Encoding.UTF8.GetString(data);
        }

        // Обычный блокирующий метод, который можно вызывать в главном
        // или дополнительном потоке выполнения
        private static string GetData(string filename)
        {
            //готовим массив для приема прочитанных данных
            byte[] data = null;
            //создаем поток для чтения
            using (FileStream fs = File.Open(filename, FileMode.Open))
            {
                //создаем массив для чтения
                data = new byte[fs.Length];
                //вызываем встроенный в класс FileStream async
                //метод ReadAsync по имени этого метода ясно,
                //что он асинхронный, значит, вызывать его
                //надо со спецификатором await
                fs.Read(data, 0, (int)fs.Length);
            }
            return System.Text.Encoding.UTF8.GetString(data);
        }

        // Асинхронный метод, внутри которого вызов блокирующего метода
        // помещается в параллельный поток выполнения,
        // и ожидается результат
        private static Task<string> GetDataAsyncRun(string file)
        {
            return Task.Run<string>(() =>
            {
                return GetData(file);
            });
        }

        // Метод чтения текста из файла параллельным потоком выполнения -
        // вызывает ошибку при попытке вывода текста в текстбокс,
        // созданный в основном потоке выполнения
        private void BeginGetData(string filename)
        {
            //готовим массив для приема прочитанных данных
            byte[] data = null;
            //создаем поток для чтения
            FileStream fs = File.Open(filename, FileMode.Open);
            //создаем массив для чтения
            data = new byte[fs.Length];
            //вызываем встроенный в класс FileStream async
            //метод ReadAsync по имени этого метода ясно,
            //что он асинхронный, значит, вызывать его
            //надо со спецификатором await
            fs.BeginRead(data, 0, (int)fs.Length, (result) => {
                FileStream fs1 = (FileStream)((IAsyncResult)result).AsyncState;
                try
                {
                    Console.WriteLine(fs1.CanRead);
                    fs1.EndRead(result);
                    //преобразовываем прочитанные данные из байтового
                    //массива в строку и отображаем в текстовом поле Result
                    Console.WriteLine(System.Text.Encoding.UTF8.GetString(data));
                    textBox1.Text = System.Text.Encoding.UTF8.GetString(data);
                }
                catch (Exception)
                {

                    throw;
                }
                finally
                {
                    fs1.Dispose();
                }
            }, fs);
        }

        private static long fib(long n) {
            if (n == 0 || n == 1)
            {
                return n;
            }
            return fib(n - 1) + fib(n - 2);
        }
    }
}
