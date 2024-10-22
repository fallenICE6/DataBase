using Devart.Data.SQLite;
using sqlite1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;

namespace lab2bd
{
    public partial class Form1 : Form
    {
        private sqliteclass mydb = null;
        private string sCurDir = string.Empty;
        private string sPath = string.Empty;
        private string sSql = string.Empty;
        private int viNum = 0;
        public byte[] data = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            sPath = Path.Combine(Application.StartupPath, "mybd.db");
            SQLiteConnection.CreateFile(sPath);
            Text = sPath;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            mydb = new sqliteclass();
            sSql = "create table myphoto (id INTEGER PRIMARY KEY NOT NULL,photos blob)";
            //Используем функцию класса, рассмотренную выше
            mydb.iExecuteNonQuery(sPath, sSql, 1);
            Text = "Таблица создана";
            mydb = null;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            mydb = new sqliteclass();
            sSql = "delete from myphoto";
            if (mydb.iExecuteNonQuery(sPath, sSql, 1) == 0)
            {
                Text = "Ошибка удаления записи!";
                mydb = null;
                return;
            }
            mydb = null;
            Text = "Записи удалены из БД!";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = @"C:\";
            openFileDialog1.Filter = "All Embroidery Files | *.bmp; *.gif; *.jpeg; *.jpg; " +
         "*.fif;*.fiff;*.png;*.wmf;*.emf" +
         "|Windows Bitmap (*.bmp)|*.bmp" +
         "|JPEG File Interchange Format (*.jpg)|*.jpg;*.jpeg" +
         "|Graphics Interchange Format (*.gif)|*.gif" +
         "|Portable Network Graphics (*.png)|*.png" +
         "|Tag Embroidery File Format (*.tif)|*.tif;*.tiff";
            //openFileDialog1.Filter += "|All Files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Image image = Image.FromFile(openFileDialog1.FileName);
                MemoryStream memoryStream = new MemoryStream();
                //Здесь можно выбрать формат хранения
                image.Save(memoryStream, ImageFormat.Jpeg);
                data = memoryStream.ToArray();
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox1.Image = image;
                
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (data == null)
            {
                Text = "Не выбрано фото для записи в БД";
                return;
            }
            int id = 0;
            mydb = new sqliteclass();
            sSql = "Select max(id) from myphoto";
            object obj = mydb.oExecuteScalar(sPath, sSql);
            if (!string.IsNullOrEmpty(obj.ToString()))
            {
                id = Convert.ToInt32(obj) + 1;
            }
            else
            {
                id = 1;
            }
            //SQL предложение для получения схемы таблицы
            sSql = "Select * from myphoto";
            //Чтение схемы
            DataTable dt = mydb.dtReagSchema(sPath, sSql);
            //Добавление строки с данными
            DataRow datarow = dt.NewRow();
            datarow["id"] = id;
            datarow["photos"] = data;
            dt.Rows.Add(datarow);
            //Сохранение изменения
            int n = mydb.iWriteBlob(sPath, dt);
            if (n == 0)
            {
                Text = "Фото в БД не записано!";
            }
            else
            {
                Text = "Фото записано в БД!";
            }
            mydb = null;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = null;
            mydb = new sqliteclass();
            sSql = "SELECT * FROM myphoto WHERE photos NOT NULL";
            data = mydb.rgbtReadBlob(sPath, sSql, "photos");
            if (data == null)
            {
                MessageBox.Show("Нет фото формата blob");
                return;
            }
            MemoryStream ms = new MemoryStream(data);
            System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.Image = image;
            Text = "Фото формата blob";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            mydb = new sqliteclass();
            sSql = "create table Employee (id INTEGER PRIMARY KEY NOT NULL, lastname varchar2(256), name varchar2(256), birhtday date, salary double(2), active boolean)";
            //Используем функцию класса, рассмотренную выше
            mydb.iExecuteNonQuery(sPath, sSql, 1);
            Text = "Таблица Employee создана";
            mydb = null;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            int id = 0;
            mydb = new sqliteclass();
            sSql = "Select max(id) from Employee";
            object obj = mydb.oExecuteScalar(sPath, sSql);
            if (!string.IsNullOrEmpty(obj.ToString()))
            {
                id = Convert.ToInt32(obj) + 1;
            }
            else
            {
                id = 1;
            }
            //SQL предложение для получения схемы таблицы
            sSql = "Select * from Employee";
            //Чтение схемы
            DataTable dt = mydb.dtReagSchema(sPath, sSql);
            DataRow datarow = dt.NewRow();
            datarow["id"] = id;
            datarow["lastname"] = textBox1.Text;
            datarow["name"] = textBox2.Text;
            datarow["birhtday"] = textBox4.Text;
            datarow["salary"] = textBox3.Text;
            if (textBox5.Text.Length > 0)
            {
                if (textBox5.Text.ToLower() == "да")
                {
                    datarow["active"] = true;
                }
                else
                {
                    datarow["active"] = false;
                }
            }
            dt.Rows.Add(datarow);
            //Сохранение изменения
            int n = mydb.iWriteBlob(sPath, dt);
            if (n == 0)
            {
                Text = "Строка в БД не записана";
            }
            else
            {
                Text = "Строка записана в БД";
            }
            mydb = null;

        }

        private void button8_Click(object sender, EventArgs e)
        {
            mydb = new sqliteclass();
            sSql = "select * from Employee";
            DataRow[] datarows = mydb.drExecute(sPath, sSql);
            if (datarows == null)
            {
                Text = "Ошибка чтения!";
                mydb = null;
                return;
            }
            Text = "";
            string str = "";
            foreach (DataRow dr in datarows)
            {
                if ((bool)dr["active"])
                {
                    str = " да ";
                }
                else
                {
                    str = " нет ";
                }
                Text += dr["id"].ToString().Trim() + " " +
               dr["lastname"].ToString().Trim()
                + " " + dr["name"].ToString().Trim()
                + " " + dr["salary"].ToString().Trim()
                + " " + dr["birhtday"].ToString().Trim() + str;
            }
            mydb = null;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            mydb = new sqliteclass();
            sSql = "delete from Employee";
            if (mydb.iExecuteNonQuery(sPath, sSql, 1) == 0)
            {
                Text = "Ошибка удаления записи!";
                mydb = null;
                return;
            }
            mydb = null;
            Text = "Записи удалены из БД!";
        }
    }
}
