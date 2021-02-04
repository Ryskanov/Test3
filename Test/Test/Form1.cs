 using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Test.View;
using Test.Presenters;

namespace Test
{
    public partial class Form1 : Form, View.ISite
    {
        SitePresenter presenter;
        public Form1()
        {
            InitializeComponent();
            //Создадим представление, он заполнит таблицу
            presenter = new SitePresenter(this);
            //Запретим изменять некоторые колонки
            dataGridView1.Columns["id"].ReadOnly = true;
            dataGridView1.Columns["lastTime"].ReadOnly = true;
            dataGridView1.Columns["status"].ReadOnly = true;

        }

        //Источник данных для таблицы
        public object DataSource 
        {
            get
            {
                return dataGridView1.DataSource;
            }
            set
            {
                dataGridView1.DataSource = value;
            }
        }


        //Добавить строку
        private void button1_Click(object sender, EventArgs e)
        {
            presenter.addRow();
        }

        //Удалить строку
        private void button2_Click(object sender, EventArgs e)
        {
            //Если количество строк больше нуля
            if (dataGridView1.Rows.Count > 0)
            {
                //индекс текущей строки
                int selRowNum = dataGridView1.SelectedCells[0].RowIndex;
                //id текущей строки
                int id = int.Parse(dataGridView1[0, selRowNum].Value.ToString());
                //Удаляем
                presenter.deleteRow(id);
            }
        }

        //Изменить строку
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //индекс текущей строки
            int selRowNum = dataGridView1.SelectedCells[0].RowIndex;
            //id текущей строки
            int id = int.Parse(dataGridView1[0, selRowNum].Value.ToString());
            //наименование сайта
            string namesite = dataGridView1[1, selRowNum].Value.ToString();
            //url
            string url = dataGridView1[2, selRowNum].Value.ToString();
            //интервал
            int interval = int.Parse(dataGridView1[3, selRowNum].Value.ToString());
            //время
            DateTime lasttimeDate = (DateTime)dataGridView1[4, selRowNum].Value;
            string lasttime = lasttimeDate.ToString("yyyy-MM-dd HH:mm:ss.fff");
            //статус
            int status = int.Parse(dataGridView1[5, selRowNum].Value.ToString());
            //Обновим запись
            presenter.UpdateRow(id, namesite, url, interval, lasttime, status);
        }

        //При закрытии формы закроем поток проверки сайтов
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            presenter.StopCheck(true);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            dataGridView1.Refresh();
        }
    }
}
