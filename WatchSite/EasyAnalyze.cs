using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using HtmlAgilityPack;

namespace WatchSite
{
    class EasyAnalyze
    {
        private string _url;

        public List<Product> GoToPage(string url)
        {
            //http://vk.com/al_market.php?al=1&id=-77305585&load=1&offset=126&price_from=&price_to=&q=&sort=0
            //http://vk.com/wkview.php?act=show&al=1&loc=market-77305585&w=product-77305585_24022
            // /market-77305585?w=product-77305585_24041
            //http://vk.com/market-77305585

            
            string str1;
            try
            {
                return tryTakeProducts(url,500);
                
            }
            catch (Exception ex)
            {

                MessageBoxResult result = MessageBox.Show("Страница возвратила ошибку "+ ex.Message+" Продолжить?", "Ошибка", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    return tryTakeProducts(url, 1000);
                }
                return null;
            }
        }

        private List<Product> tryTakeProducts(string url, int Zaderzhka)
        {
            string page = "";

            using (System.Net.WebClient web = new System.Net.WebClient())
            {

                web.Encoding = UTF8Encoding.Default;
                string str1 = web.DownloadString(url);

                str1 = System.Web.HttpUtility.HtmlDecode(str1);
                var numArr = Regex.Matches(str1, @"itemsCount"":([0-9]{1,5}),")
                .Cast<Match>()
                .Select(m => m.Groups[1].Value)
                .Distinct()
                .ToArray();

                int num = Int32.Parse(numArr[0]);

                while (num >= 0)
                {
                    num = num - 10;

                    string zagotovka = "http://vk.com/al_market.php?al=1&id=-$2&load=1&offset=" + num +
                    "&price_from=&price_to=&q=&sort=0";
                    _url = Regex.Replace(url, @"(http://vk.com/market-)(.+)", zagotovka);

                    string str = web.DownloadString(_url);
                    str = System.Web.HttpUtility.HtmlDecode(str);

                    page = page + str; //doc.DocumentNode.InnerHtml
                    Thread.Sleep(Zaderzhka);
                }
                //<div id="market_item24046" class="market_row">
                //File.WriteAllText("C:\\Users\\Maxs\\Desktop\\www1.txt", page);
                //div[@class='market_row_inner_cont']
                List<Product> listProducts = new List<Product>();

                var tempUrlToItems = Regex.Matches(page, @"name""><a href=""(/market-[0-9]{2,20}\?w=product-[0-9_]{2,35})")
                .Cast<Match>()
                .Select(m => m.Groups[1].Value)
                .ToArray();

                var tempIdToItems = Regex.Matches(page, @"<div id=""market_item([0-9]{1,9})"" class=""market_row"">")
                .Cast<Match>()
                .Select(m => m.Groups[1].Value)
                .ToArray();

                var tempNameToItems = Regex.Matches(page, @"<div class=""market_row_name""><a.*?>(.*?)</a")
                .Cast<Match>()
                .Select(m => m.Groups[1].Value)
                .ToArray();

                var tempPriceToItems = Regex.Matches(page, @"<div class=""market_row_price"">(.*?)</div>")
                .Cast<Match>()
                .Select(m => m.Groups[1].Value)
                .ToArray();

                Product product;
                for (int i = 0; i < tempUrlToItems.Length; i++)
                {
                    product = new Product();
                    product.Url = Regex.Replace(tempUrlToItems[i], @"/(market-[0-9]{2,20})\?w=(product-[0-9_]{2,35})",
                        "http://vk.com/wkview.php?act=show&al=1&loc=$1&w=$2");
                    product.Name = tempNameToItems[i];
                    product.Id = tempIdToItems[i];
                    //<div class="market_row_price">2<span class="num_delim"> </span>230 руб.</div>
                    product.Price = Regex.Replace(tempPriceToItems[i], @"(<div class=""market_row_price"".*?)([7-9]{1,7} руб.)(</div>)",
                        "$2");

                    listProducts.Add(product);
                }

                listProducts = listProducts.Distinct(new ProductComparer()).ToList();


                str1 = System.Web.HttpUtility.HtmlDecode(str1);

                //3<span class="num_delim"> </span>990 руб.
                //</div> <div class="market_item_controls unshown">
                foreach (var prod in listProducts)
                {
                    prod.Price = Regex.Replace(prod.Price, @"<span class=""num_delim"">\s*?</span>",
                        " ");
                    string desc = web.DownloadString(prod.Url);
                    desc = System.Web.HttpUtility.HtmlDecode(desc);
                    prod.Desc = Regex.Replace(desc, @".*?<div class=""market_item_description"">[\n]*?\s+(.*?)</div>(.*?)<div class=""market_item_controls unshown"">.*",
                        "$1", RegexOptions.Singleline);
                    prod.Desc = Regex.Replace(prod.Desc, @"<.*?>", " ", RegexOptions.Singleline);
                    prod.Desc = Regex.Replace(prod.Desc, @"Показать полностью\.\.", "", RegexOptions.Singleline);
                    prod.Desc = Regex.Replace(prod.Desc, @"\s+", " ", RegexOptions.Singleline);
                    Thread.Sleep(Zaderzhka);
                }
                return listProducts;
            }
            
        }
    }
}
