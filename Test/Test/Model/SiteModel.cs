using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using System.IO;

namespace Test.Model
{
    public class SiteModel
    {
        private SqlConnection sqlConnection = null;  //Соединение
        public DataTable Sites;                      //Таблица с сайтами

        //Подключиться к SQL
        public void ConnectionDB()
        {
            //Получим соединение
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TestDB"].ConnectionString);
            //Откроем соединение
            sqlConnection.Open();
        }

        //Получить данные из SQL DB и записать в таблицу
        public void GetDate()
        {
            //Создадим объект DataTable
            Sites = new System.Data.DataTable();
            //Создадим SQL запрос
            string query = "SELECT * FROM site";
            SqlCommand cmd = new SqlCommand(query, sqlConnection);
            //Заполним DataTable
            Sites.Load(cmd.ExecuteReader());
        }

        //Добавим новую записи
        public void addRow()
        {
            lock (Sites) // Заблокируем DataTable
            {
                int newid = 1;
                //Найдем максимальное значение id + 1
                string query = "SELECT MAX(id) as max FROM site";
                SqlCommand cmd = new SqlCommand(query, sqlConnection);
                object count = cmd.ExecuteScalar();
                if (count is int)
                {
                    newid = (int)count + 1;
                }
                //Добавим новую запись в базу данных
                query = "INSERT INTO site(id, namesite, url, interval, lasttime, status) " +
                        "VALUES(" + newid.ToString() + ", 'Name site', 'https://www.reg.ru', 10, '1900-01-01 00:00:00', 0)";
                cmd = new SqlCommand(query, sqlConnection);
                cmd.ExecuteNonQuery();
                //Добавим новую запись в DataGridView 
                Sites.Rows.Add(newid.ToString(), "Name site", "https://www.reg.ru", 10, new DateTime(), 0);
            }
        }

        //Удалить запись с номером id 
        public void deleteRow(int id)
        {
            lock (Sites) // Заблокируем DataTable
            {
                //Удалить запись из базы данных
                string query = "DELETE FROM site WHERE id = " + id.ToString();
                SqlCommand cmd = new SqlCommand(query, sqlConnection);
                cmd.ExecuteNonQuery();
                //Удалим запись из DataGridView 
                DataRow[] rows = Sites.Select("id = '" + id + "'");
                foreach (var row in rows)
                {
                    row.Delete();
                }
            }
        }

        //Записать запись с номером id 
        public void UpdateRow(int id, string namesite, string url, int interval, string lasttime, int status)
        {
            //Записать запись
            string query = " UPDATE site " +
                           " SET namesite='" + namesite.TrimEnd() + "'," +
                           "     url='" + url.TrimEnd() + "'," +
                           "     interval=" + interval.ToString() + "," +
                           "     lasttime='" + lasttime.ToString() + "'," +
                           "     status=" + status.ToString() +
                           " WHERE id=" + id.ToString();
            SqlCommand cmd = new SqlCommand(query, sqlConnection);
            cmd.ExecuteNonQuery();
        }

        //Пройдемся по сайтам и проверим их
        public void CheckSite()
        {
            lock (Sites) // Заблокируем DataTable
            {
                //Пройдемся по строкам DataTable
                for (int i = 0; i < Sites.Rows.Count; i++)
                {
                    //Расчитаем прошедшее время после последней проверки
                    System.TimeSpan diferent = DateTime.Now - (DateTime)Sites.Rows[i]["lasttime"];
                    if (diferent.Seconds > (int)Sites.Rows[i]["interval"])
                    {
                        int result = 0;
                        try
                        {
                            //Проверим сайт 
                            HttpWebRequest urlReq = (HttpWebRequest)WebRequest.Create(Sites.Rows[i]["url"].ToString().TrimEnd());
                            HttpWebResponse urlRes = (HttpWebResponse)urlReq.GetResponse();
                            Stream sStream = urlRes.GetResponseStream();
                            string read = new StreamReader(sStream).ReadToEnd();
                            //Все хорошо, сайт рабочий
                            result = 1;
                        }
                        catch (Exception ex)
                        {
                            //Ошибка сайт не работает
                        }
                        //Текущее время
                        DateTime curentTime = DateTime.Now;
                        //Запишем в DataTable результат и изменим время последней проверки
                        UpdateRow((int)Sites.Rows[i]["id"],                            //id  
                                  Sites.Rows[i]["namesite"].ToString().TrimEnd(),      //name
                                  Sites.Rows[i]["url"].ToString().TrimEnd(),           //url 
                                  (int)Sites.Rows[i]["interval"],                      //interval
                                  curentTime.ToString("yyyy-MM-dd HH:mm:ss.fff"),      //lasttime
                                  result);                                             //status
                        //Запишем время и статус DataTable для отображения в DataGridView
                        Sites.Rows[i]["lasttime"] = curentTime;
                        Sites.Rows[i]["status"] = result;
                    }
                }
            }
        }

    }
}
