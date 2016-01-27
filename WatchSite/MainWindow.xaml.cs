using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace WatchSite
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
            if (Properties.Settings.Default.OsnMail != "")
            {
                textBox.Text = Properties.Settings.Default.UrlSite;
                osnMail.Text = Properties.Settings.Default.OsnMail;
                loginYa.Text = Properties.Settings.Default.LoginYa;
                passYa.Text = Properties.Settings.Default.PassYa;
            }
        }

        private string urlSite;
        private string login;
        private string pass;
        private string osn;
        private Task task;
        private Task task2;
        //private Thread thread2;
        //private Thread thread;
        private int countProverok;
        private List<Product> listProducts;
        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (textBox.Text == "")
            {
                MessageBox.Show("Введите url");

            }
            else
            {

                Properties.Settings.Default.PassYa = passYa.Text;
                Properties.Settings.Default.LoginYa = loginYa.Text;
                Properties.Settings.Default.OsnMail = osnMail.Text;
                Properties.Settings.Default.UrlSite = textBox.Text;
                Properties.Settings.Default.Save();

                urlSite = textBox.Text;
                login = loginYa.Text;
                pass = passYa.Text;
                osn = osnMail.Text;

                System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
                textBlock.Text = "Наблюдение началось";
                
                timer.Tick += timerTick;
                try
                {
                    timer.Interval = new TimeSpan(Int32.Parse(Chasy.Text), Int32.Parse(Minuty.Text), 0);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Период проверки неправильный");
                }
                task = new Task(MyAnalyze);
                task.Start();
                try
                {
                    task.Wait();
                }
                catch (AggregateException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                //thread = new Thread(() => MyAnalyze());
                //thread.IsBackground = true;
                //thread.Start();
                timer.Start();


                
            }
            

            //analyzeSite.GoToPage(textBox.Text, webBrowser);
            //webBrowser.LoadCompleted += WebBrowserOnLoadCompleted;
            


            

        }

        private void timerTick(object sender, EventArgs e)
        {
            if (task.Status != TaskStatus.Running && task2 == null)
            {
                task2 = new Task(MyAnalyze);
                task2.Start();
                try
                {
                    task2.Wait();
                }
                catch (AggregateException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                //thread2 = new Thread(() => MyAnalyze());
                //thread2.IsBackground = true;
                //thread2.Start();
            }
            else if (task2 != null && task2.Status != TaskStatus.Running)
            {
                task2 = new Task(MyAnalyze);
                task2.Start();

                try
                {
                    task2.Wait();
                }
                catch (AggregateException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                //thread2 = new Thread(() => MyAnalyze());
                //thread2.IsBackground = true;
                //thread2.Start();
            }
            

        }

        private void MyAnalyze()
        {
            List<Product> listProductsSend;
            if (countProverok > 0 && listProducts == null)
            {
                countProverok++;
                textBlock.Dispatcher.BeginInvoke(new Action(delegate ()
                {
                    textBlock.DataContext = null;
                    textBlock.Text = "Идет проверка №" + countProverok;
                }));
                List<Product> listProducts2;
                EasyAnalyze esAnalyze = new EasyAnalyze();
                listProducts = esAnalyze.GoToPage(urlSite);

                if (!File.Exists("Products.json"))
                {
                    //первый запуск
                    File.WriteAllText(@"Products.json", JsonConvert.SerializeObject(listProducts));

                }
                else
                {
                    listProducts2 = JsonConvert.DeserializeObject<List<Product>>(File.ReadAllText("Products.json"));
                    

                    var differences = listProducts2.Where(l2 => listProducts.Any(l1 => l1.Id == l2.Id && l1.Price != l2.Price)).ToList();

                    listProductsSend = listProducts2.Except(listProducts, new ProductComparer()).ToList();
                    listProductsSend.AddRange(differences);
                    listProductsSend.AddRange(listProducts.Except(listProducts2, new ProductComparer()).ToList());
                    if (listProductsSend.Count > 0)
                    {
                        string allprod = "";
                        foreach (var prod in listProductsSend)
                        {
                            prod.Url = Regex.Replace(prod.Url, @"vk.com/wkview\.php\?act=show&al=1&loc=market-([0-9]{1,22}?)&w=product-([0-9)]{1,33})",
                                "vk.com/market-$1?w=product-$2");
                            allprod = allprod + " | " + prod.Name + " | " + prod.Price + " | " + prod.Url + " ; \n\r ";
                        }
                        Email.SendMail(login, pass, osn, allprod);

                        File.WriteAllText(@"Products.json", JsonConvert.SerializeObject(listProducts));
                    }

                }
                

                listProducts = null;

                //Distinct(new ProductComparer()).ToList(); ;
            }
            else if (countProverok == 0 && listProducts == null)
            {
                textBlock.Dispatcher.BeginInvoke(new Action(delegate ()
                {
                    textBlock.DataContext = null;
                    textBlock.Text = "Первичный сбор информации";
                }));
                
                EasyAnalyze esAnalyze = new EasyAnalyze();
                listProducts = esAnalyze.GoToPage(urlSite);

                if (!File.Exists("Products.json"))
                {
                    File.WriteAllText(@"Products.json", JsonConvert.SerializeObject(listProducts));

                }

                countProverok++;
                listProducts = null;
            }
            
        }

        private void WebBrowserOnLoadCompleted(object sender, NavigationEventArgs navigationEventArgs)
        {
            EasyAnalyze esAnalyze = new EasyAnalyze();
            esAnalyze.GoToPage(textBox.Text);
        }

        //private void Window_Unloaded(object sender, RoutedEventArgs e)
        //{
        //    if (File.Exists("Products.json")) File.Delete("Products.json");
        //}

        //private void WinClose(object sender, CancelEventArgs e)
        //{
        //    //if (File.Exists("Products.json")) File.Delete("Products.json");
        //}
    }
}
