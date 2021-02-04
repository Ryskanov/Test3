using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.View;
using Test.Model;
using System.Threading;

namespace Test.Presenters
{
    public class SitePresenter
    {
        ISite siteView;              //Форма
        SiteModel siteModel;         //Модель
        Thread checkThread = null;   //Поток
        bool stopThread = false;     //Флаг для остановки потока

        //Конструктор
        public SitePresenter(ISite view)
        {
            //Сохраним ссылку на форму
            siteView = view;
            //Создадим экземпляр модели
            siteModel = new SiteModel();
            //Соединимся с БД
            siteModel.ConnectionDB();
            //Прочитаем данные из БД
            siteModel.GetDate();
            //Настроим источник данных для DataGridView
            siteView.DataSource = siteModel.Sites;

            //Создаем новый поток
            Thread checkThread = new Thread(new ThreadStart(CheckSite));
            //Запускаем поток
            checkThread.Start(); 
        }

        //Добавить строку
        public void addRow()
        {
            siteModel.addRow();
        }

        //Удалить строку
        public void deleteRow(int id)
        {
            siteModel.deleteRow(id);
        }

        //Изменить строку
        public void UpdateRow(int id, string namesite, string url, int interval, string lasttime, int status)
        {
            siteModel.UpdateRow(id, namesite, url, interval, lasttime, status);
        }

        //Остановить поток
        public void StopCheck(bool val)
        {
            stopThread = val;
        }


        //Это выполняется в отдельном потоке
        public void CheckSite()
        {
            while(! stopThread)
            {
                Thread.Sleep(10000);
                siteModel.CheckSite();
            }
        }

    }
}
