using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace _74varyag
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private class Vacancy
        {
            public string dolz,org,ob,treb,usl,minZp,maxZp,url;
            public Vacancy(string nDolz,string nOrg,string nOb,string nTreb,string nUsl, string nminZp,string nmaxZp,string nurl)
            {
                dolz = nDolz;
                org = nOrg;
                ob = nOb;
                treb = nTreb;
                usl = nUsl;
                minZp = nminZp;
                maxZp = nmaxZp;
                url=nurl;
            }
        }

        private class Resume
        {
            public string url, spec, wplace, sumexp, realexp,abils,name;
            public int zp;
            public Resume(string nurl,string nspec,string nwplace,string nzp,string nsumexp,string nrealexp, string nabils,string nname)
            {
                url = nurl;
                spec = nspec;
                wplace = nwplace;
                //Проверка "договорная"
                int n;
                if (int.TryParse(nzp, out n))
                {
                    zp = n;
                }
                //
                sumexp = nsumexp;
                realexp = nrealexp;
                abils = nabils;
                name = nname;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            int pgCnt = 0;
            int.TryParse(this.textBox1.Text, out pgCnt);
            List<Vacancy> vacList = new List<Vacancy>();
            for (int i = 0; i < pgCnt; i++)
            {
                //Vacancy
                var vacUrl = @"http://chelyabinsk.zarplata.ru/vacancy?state%5B%5D=1&state%5B%5D=4&average_salary=1&city_id=1124&entity=2&geo_id%5B%5D=1124&categories_facets=1&limit=50&offset=" + 50*i;
                var constRef = @"http://chelyabinsk.zarplata.ru/";
                HtmlWeb hWeb = new HtmlWeb();
                HtmlAgilityPack.HtmlDocument hDoc = hWeb.Load(vacUrl);

                string ClassToGet = "ra-elements-list-hidden";
                foreach (HtmlNode node in hDoc.DocumentNode.SelectNodes("//ul[@class='" + ClassToGet + "']"))
                {
                    foreach (HtmlNode chNode in node.ChildNodes)
                    {
                        if (chNode.Name == "li")
                        {
                            var inTxt = chNode.InnerText;
                            //Разбор строки
                            var tmpStr = inTxt.Substring(1, inTxt.Length - 2);
                            int stPos = chNode.InnerHtml.IndexOf("<a href=");
                            int endPos = chNode.InnerHtml.IndexOf("\">\n");

                            var url = constRef + chNode.InnerHtml.Substring(stPos + 10, endPos - stPos - 10); 

                            //Должность
                            tmpStr = inTxt.Substring(1, inTxt.Length - 2);
                            var ptrn = "\n        \n";
                            var st = inTxt.IndexOf(ptrn) + ptrn.Length;
                            var ed = tmpStr.IndexOf(ptrn);
                            var Dolzh = inTxt.Substring(st, ed - st + 1).Trim();


                            //Организация
                            tmpStr = tmpStr.Substring(tmpStr.IndexOf(ptrn) + ptrn.Length).TrimStart(); //сдвиг
                            var Org = tmpStr.Substring(0, tmpStr.IndexOf("\n")); //почистить от / и прочего

                            //Обязанности
                            var posOb = tmpStr.IndexOf("Обязанности:") + 12;
                            //Требования
                            var posTreb = tmpStr.IndexOf("Требования:") + 11;
                            //Условия
                            var posUsl = tmpStr.IndexOf("Условия:") + 8;
                            //Зарплата
                            var posZP = tmpStr.IndexOf("Зарплата:") + 9;
                            if (posTreb < posOb) { posTreb = tmpStr.IndexOf("Требования") + 10; }

                            var Ob = "";
                            var Treb = "";
                            var Usl = "";
                            if (posOb != 11 && posTreb > 10 && posTreb>posOb) //Нормальный тип объявления
                            {

                                Ob = tmpStr.Substring(posOb, posTreb - 11 - posOb);
                                if (posUsl != 7)
                                {  //то есть найдено
                                    Treb = tmpStr.Substring(posTreb, posUsl - 8 - posTreb);
                                    Usl = tmpStr.Substring(posUsl, posZP - 9 - posUsl);
                                }
                                else //если не найдено
                                {
                                    Treb = tmpStr.Substring(posTreb, posZP - 8 - posTreb);
                                }
                            }
                            else //нестандартное объявление
                            {
                                var nPos = tmpStr.IndexOf("\n") + 2;
                                Treb = tmpStr.Substring(nPos, posZP - 9 - nPos);
                                nPos++;
                            }
                            var ZP = tmpStr.Substring(posZP).Trim();
                            ZP = ZP.Substring(0, ZP.Length);
                            string minZP="0",maxZP = "";
                            int posMin = ZP.IndexOf("от ") + 3;
                            int posMax = ZP.IndexOf("до ");
                            if (posMin> 2)
                            {
                                if (posMax < 0) //если зарплата от %s руб.
                                {
                                    minZP = ZP.Substring(posMin, ZP.Length-8);
                                }
                                else  //зарплата от %s до %s руб.
                                {
                                    minZP = ZP.Substring(posMin, posMax - posMin);
                                    maxZP = ZP.Substring(posMax + 3, ZP.Length - 8 - posMax);
                                }
                            }
                            else
                            {
                                if (posMax >= 0) //зарплата до %s руб.
                                {
                                    maxZP = ZP.Substring(posMax + 3, ZP.Length - posMax - 1);
                                }
                                else
                                {
                                    maxZP = minZP = ""; //договорная
                                }
                            }

                            //чистка строк
                           minZP= minZP.Replace(" ", "");
                           maxZP = maxZP.Replace(" ", "");

                            //Добавить вакансию в список
                            Vacancy newVac = new Vacancy(Dolzh.Trim(), Org.Trim(), Ob.Trim(), Treb.Trim(), Usl.Trim(), minZP,maxZP,url);
                            vacList.Add(newVac);
                        }
                    }
                }
            }

            //Загрузить вакансии в БД
            System.Data.SqlClient.SqlConnection sqlConnection1 =  new System.Data.SqlClient.SqlConnection("Data Source=ERU-ПК\\TESTSRV;Initial Catalog=var74;Persist Security Info=True;User ID=user_var;Password=pass123"); //в реальной ситуации не хранить пароль тут
            System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            sqlConnection1.Open();

            foreach (Vacancy vac in vacList)
            {
                var hash = (vac.dolz + vac.org + vac.minZp+vac.maxZp).GetHashCode();
            
                //Проверка есть ли в базе такая вакансия
                System.Data.SqlClient.SqlCommand cmdCheck = new System.Data.SqlClient.SqlCommand();
                cmdCheck.CommandType = System.Data.CommandType.Text;
                cmdCheck.CommandText = "SELECT Count(*) FROM vacs WHERE Hash1 like '%" + hash + "'";
                int count =0;
                cmdCheck.Connection = sqlConnection1;
                count = (int)cmdCheck.ExecuteScalar();
                //Если нет то добавим
                if (count == 0)
                {
                    int intMin, intMax;
                    int.TryParse(vac.minZp, out intMin);
                    int.TryParse(vac.maxZp, out intMax);
                    if (intMax > 0)
                    {
                        cmd.CommandText = String.Format("INSERT vacs (Dolzh,Org,Ob,Treb,Usl,minZP,maxZP, Hash1,Url) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')", vac.dolz, vac.org, vac.ob, vac.treb, vac.usl, intMin, intMax, hash,vac.url);
                    }
                    else 
                    {
                        if (intMin > 0)
                        {
                            cmd.CommandText = String.Format("INSERT vacs (Dolzh,Org,Ob,Treb,Usl,minZP, Hash1,Url) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')", vac.dolz, vac.org, vac.ob, vac.treb, vac.usl, intMin, hash, vac.url);
                        }
                        else
                        {
                            cmd.CommandText = String.Format("INSERT vacs (Dolzh,Org,Ob,Treb,Usl, Hash1,Url) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", vac.dolz, vac.org, vac.ob, vac.treb, vac.usl, hash, vac.url);
                        }
                    }
                    cmd.Connection = sqlConnection1;
                    cmd.ExecuteNonQuery();
                }
            }
            sqlConnection1.Close();
            MessageBox.Show("Вакансии загружены в БД!");
            //Загрузка новых вакансий завершена
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FormVacs frmv = new FormVacs();
            frmv.Show();
        }

        
        
        //Загрузка динамического содержания, традиционным путем не грузится.
        private  HtmlAgilityPack.HtmlDocument LoadHtmlWithBrowser(String url)
        {
            WebBrowser webBrowser1 = new WebBrowser();
            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.Navigate(url);
            
            while (webBrowser1.DocumentText == "")
            {
                Application.DoEvents();
               // webBrowser1.Refresh();
            }

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            var documentAsIHtmlDocument3 = (mshtml.IHTMLDocument3)webBrowser1.Document.DomDocument;
            if (documentAsIHtmlDocument3.documentElement != null)
            {
                StringReader sr = new StringReader(documentAsIHtmlDocument3.documentElement.outerHTML);
                doc.Load(sr);
                webBrowser1.Dispose();
            }
            else
            {
                doc.LoadHtml(webBrowser1.DocumentText);
            }
            webBrowser1.Dispose();
            return doc;
        }
        
        private void button3_Click(object sender, EventArgs e)
        {
            int pgCnt = 0;
            int.TryParse(this.textBox2.Text, out pgCnt);
            //List resume
            List<Resume> resList = new List<Resume>();
            int rLoaded = 0;
            int rToLoad = 25 * (pgCnt);
            for (int i = 0; i < pgCnt; i++)
            {
                //resume
                var resUrl = @"http://chelyabinsk.zarplata.ru/resume?state%5B%5D=1&visibility_type%5B%5D=1&average_salary=1&city_id=1124&currency_id=299&entity=1&geo_id%5B%5D=1124&is_new_only=0&search_type=fullThrottle&limit=25&categories_facets=1&offset=" + 25 * i;
                var constRef = @"http://chelyabinsk.zarplata.ru/";
                HtmlWeb hWeb = new HtmlWeb();
                HtmlAgilityPack.HtmlDocument hDoc = hWeb.Load(resUrl);

                string ClassToGet = "ra-elements-list-hidden";
                foreach (HtmlNode node in hDoc.DocumentNode.SelectNodes("//ul[@class='" + ClassToGet + "']"))
                {
                    foreach (HtmlNode chNode in node.ChildNodes)
                    {
                        if (chNode.Name == "li")
                        {
                            var inTxt = chNode.InnerText;
                            //Разбор строки
                            var ch1 = chNode.ChildNodes[1].OuterHtml.ToString();
                            int stPos = ch1.IndexOf("<a href=");
                            int endPos = ch1.IndexOf("\">\n");
                            
                            var url = constRef + ch1.Substring(stPos + 10, endPos - stPos - 10);
                            //специальность
                            stPos = ch1.IndexOf("<h3>");
                            endPos = ch1.IndexOf("</h3>");
                            
                            string Spec = ch1.Substring(stPos + 4, endPos - stPos-4);
                            
                            //Место работы (Город)
                            ch1 = chNode.ChildNodes[5].InnerText.ToString();
                            string Workplace = ch1.Substring(14, ch1.Length - 14);
                            
                            //Зарплата
                            ch1 = chNode.ChildNodes[7].InnerText.ToString();
                            string Zp = ch1.Substring(10, ch1.Length - 10);
                            if (Zp.IndexOf("руб.") > 0) { Zp = Zp.Substring(0, Zp.Length - 5); Zp = Zp.Replace(" ", ""); }
                            
                            //Получение со страницы вакансии данных 
                            //СТАНДАРТНЫЙ БРАУЗЕР НЕ ПОДХОДИТ ДЛЯ ДАННЫХ ВЕБ СТРАНИЦ, ДЛЯ КОРРЕКТНОЙ ЗАГРУЗКИ ДАННЫХ НИЖЕ ПОТРЕБУЕТСЯ СТОРОННИЙ БРАУЗЕР
                            HtmlAgilityPack.HtmlDocument hDoc2 = hWeb.Load(resUrl);
                            hDoc2 = LoadHtmlWithBrowser(url);
                            
                            //Чтение опыта работы
                            ClassToGet = "ra-resume__time-work";
                            HtmlNode newNode = hDoc2.DocumentNode.SelectSingleNode("//span[@class='" + ClassToGet + "']");
                            string summExp = "";
                            if (newNode!=null){summExp = newNode.InnerText.ToString().Trim();}
                            ClassToGet = "ra-resume__block-experience-length";
                            newNode = hDoc2.DocumentNode.SelectSingleNode("//div[@class='" + ClassToGet + "']");
                            string realExp="";
                            if(newNode!=null){realExp = newNode.ChildNodes[1].InnerText.ToString().Trim();}
                           
                            //Навыки
                            ClassToGet = "ra-resume__description";
                            newNode = hDoc2.DocumentNode.SelectSingleNode("//div[@class='" + ClassToGet + "']");
                            string Abilities = "";
                            if (newNode != null) {Abilities= newNode.InnerHtml.ToString().Trim(); }
                            
                            //ФИО
                            ClassToGet = "ra-resume__main-header";
                            newNode = hDoc2.DocumentNode.SelectSingleNode("//div[@class='" + ClassToGet + "']");

                            string Name = "";
                            if (newNode != null) { Name = newNode.InnerHtml.ToString().Trim(); }
                            
                                                      
                            //Добавить резюме в список
                            Resume res = new Resume(url,Spec,Workplace,Zp,summExp,realExp,Abilities,Name);
                            resList.Add(res);
                            //
                            rLoaded++;
                            label3.Text = "Загружено: " + rLoaded + " из " + rToLoad;
                        }
                    }
                }
            }
            //Загрузить резюме в БД
            System.Data.SqlClient.SqlConnection sqlConnection1 = new System.Data.SqlClient.SqlConnection("Data Source=ERU-ПК\\TESTSRV;Initial Catalog=var74;Persist Security Info=True;User ID=user_var;Password=pass123"); //в реальной ситуации не хранить пароль тут
            System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            sqlConnection1.Open();

            foreach (Resume res in resList)
            {
                var hash = (res.url+res.spec+res.zp).GetHashCode();

                //Проверка есть ли в базе такая вакансия
                System.Data.SqlClient.SqlCommand cmdCheck = new System.Data.SqlClient.SqlCommand();
                cmdCheck.CommandType = System.Data.CommandType.Text;
                cmdCheck.CommandText = "SELECT Count(*) FROM resume WHERE Hash1 like '%" + hash + "'";
                int count = 0;
                cmdCheck.Connection = sqlConnection1;
                count = (int)cmdCheck.ExecuteScalar();
                //Если нет то добавим
                if (count == 0)
                {
                    cmd.CommandText = String.Format("INSERT resume (Spec,Place, ZP, Url, sumExp, realExp, abils,name, Hash1) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')",res.spec,res.wplace,res.zp,res.url,res.sumexp,res.realexp,res.abils,res.name, hash);
                    cmd.Connection = sqlConnection1;
                    cmd.ExecuteNonQuery();
                }
            }
            sqlConnection1.Close();
            //Загрузка новых резюме завершена
            MessageBox.Show("Резюме загружены в БД!");
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FormResume frmrs = new FormResume();
            frmrs.Show();
        }
    }
}
