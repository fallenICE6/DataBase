using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlTypes;
using System.Data;
using Devart.Data;
using Devart.Data.SQLite;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;

namespace sqlite1
{
    internal class sqliteclass
    {
        private string sLasterror = string.Empty;
        private string sErrors = string.Empty;
        SQLiteDataAdapter myDataAdapter;
        SQLiteCommandBuilder mycommandbuilder;
        public string sGetErrors()
        {
            return sErrors;
        }
        public string sGeLastError()
        {
            return sLasterror;
        }

        #region Execute SQLiteDataReader
        public byte[] rgbytedreaderExecute(string FileData, string sSql)
        {
            byte[] data = null;
            SQLiteDataReader dr = null;
            try
            {
                using (SQLiteConnection con = new SQLiteConnection())
                {
                    con.ConnectionString = @"Data Source=" + FileData;
                    con.Open();
                    using (SQLiteCommand sqlCommand = con.CreateCommand())
                    {
                        sqlCommand.CommandText = sSql;
                        dr = sqlCommand.ExecuteReader();
                    }
                    dr.Read();
                    data = GetBytes(dr);

                    con.Close();
                }
            }
            catch (Exception ex)
            {
                data = null;
            }
            return data;
        }
        static byte[] GetBytes(SQLiteDataReader reader)
        {
            //const int CHUNK_SIZE = 2 * 1024;
            byte[] bDate = new byte[1024];
            long lRead = 0;
            long lOffset = 0;
            using (MemoryStream memorystream = new MemoryStream())
            {
                while ((lRead = reader.GetBytes(0, lOffset, bDate, 0,
                bDate.Length)) > 0)
                {
                    byte[] bRead = new byte[lRead];
                    Buffer.BlockCopy(bDate, 0, bRead, 0, (int)lRead);
                    memorystream.Write(bRead, 0, bRead.Length); lOffset +=
                    lRead;
                }
                return memorystream.ToArray();
            }
        }
        #endregion
        #region Чтение 1 поля BLOB
        public byte[] rgbtReadBlob(string FileData, string sSql, string field)
        {
            byte[] buffer = null;
            DataTable datatable = new DataTable();
            using (SQLiteConnection con = new SQLiteConnection())
            {
                try
                {
                    con.ConnectionString = @"Data Source=" + FileData;
                    con.Open();
                    SQLiteDataAdapter myDataAdapter = new SQLiteDataAdapter(sSql, con);
                    SQLiteCommandBuilder commandbuilder = new
                    SQLiteCommandBuilder(myDataAdapter);
                    myDataAdapter.Fill(datatable);
                    DataRow[] datarows = datatable.Select();
                    if (datarows.Length > 0)
                    {
                        DataRow datarow = datarows[0];
                        //Получаем быйты фото из БД в массив
                        buffer = (byte[])datarow[field];
                    }
                }
                catch (Exception ex)
                {
                    buffer = null;
                    MessageBox.Show(ex.Message);
                }
            }
            return buffer;
        }
        #endregion
        #region ExecuteNonQuery
        public int iExecuteNonQueryBlob(string FileData, string sSql, string s0, Int32 number, string s1, byte[] data)
        {
            int n = 0;
            try
            {
                using (SQLiteConnection con = new SQLiteConnection())
                {
                    con.ConnectionString = @"Data Source=" + FileData;
                    con.Open();
                    using (SQLiteCommand sqlCommand = con.CreateCommand())
                    {
                        sqlCommand.CommandText = sSql;
                        sqlCommand.Parameters.Add("@" + s0, SQLiteType.Int32).Value = number;
                        sqlCommand.Parameters.Add("@" + s1, SQLiteType.Blob, data.Length).Value = data;
                        n = sqlCommand.ExecuteNonQuery();
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                n = 0;
                MessageBox.Show(ex.Message);
            }
            return n;
        }
        #endregion
        #region ExecuteScalar
        public object oExecuteScalar(string FileData, string sSql)
        {
            object obj = null;
            try
            {
                using (SQLiteConnection con = new SQLiteConnection())
                {
                    con.ConnectionString = @"Data Source=" + FileData;
                    con.Open();
                    using (SQLiteCommand sqlCommand = con.CreateCommand())
                    {
                        sqlCommand.CommandText = sSql;
                        obj = sqlCommand.ExecuteScalar();
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show(ex.Message);
            }
            return obj;
        }
        #endregion

        #region fReagSchema - Чтение схемы БД
        public DataTable dtReagSchema(string FileData, string sSql)
        {
            bool f = true;
            DataTable mydatatable = new DataTable();
            SQLiteConnection con = new SQLiteConnection();
            try
            {
                con.ConnectionString = @"Data Source=" + FileData;
                con.Open();
                myDataAdapter = new SQLiteDataAdapter(sSql, con);
                mycommandbuilder = new SQLiteCommandBuilder(myDataAdapter);
                myDataAdapter.FillSchema(mydatatable, SchemaType.Source);
            }
            catch (Exception ex)
            {
                mydatatable = null;
                MessageBox.Show(ex.Message);
            }
            if (myDataAdapter == null || mycommandbuilder == null)
            {
                mydatatable = null;
            }
            return mydatatable;
        }
        #endregion
        #region Изменение базы
        public int iWriteBlob(string FileData, DataTable mydatatable)
        {
            int n = 0;
            using (SQLiteConnection con = new SQLiteConnection())
            {
                try
                {
                    n = myDataAdapter.Update(mydatatable);
                }
                catch (Exception ex)
                {
                    n = 0;
                    MessageBox.Show(ex.Message);
                }
            }
            return n;
        }
        #endregion
        #region iExecuteNonQuery
        public int iExecuteNonQuery(string FileData, string sSql, int where)
        {
            int n = 0;
            try
            {
                using (SQLiteConnection con = new SQLiteConnection())
                {
                    if (where == 0)
                    {
                        SQLiteConnection.CreateFile(FileData);
                        con.ConnectionString = @"Data Source=" + FileData;
                    }
                    else
                    {
                        con.ConnectionString = @"Data Source=" + FileData;
                    }
                    con.Open();
                    using (SQLiteCommand sqlCommand = con.CreateCommand())
                    {
                        sqlCommand.CommandText = sSql;
                        n = sqlCommand.ExecuteNonQuery();
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                sLasterror = ex.Message;
                sErrors += ex.Message + " ";
                MessageBox.Show(ex.Message);
                File.AppendAllText("logerrors.txt", DateTime.Now.ToString() + " " +
                Path.GetFileName(FileData) + " " + sSql + " " + ex.Message + " " + Environment.NewLine,
                System.Text.Encoding.UTF8);
            }
            return n;
        }
        #endregion
        #region drExecute
        public DataRow[] drExecute(string FileData, string sSql)
        {
            DataRow[] datarows = null;
            SQLiteDataAdapter dataadapter = null;
            DataSet dataset = new DataSet();
            DataTable datatable = new DataTable();
            try
            {
                using (SQLiteConnection con = new SQLiteConnection())
                {
                    con.ConnectionString = @"Data Source=" + FileData;
                    con.Open();
                    using (SQLiteCommand sqlCommand = con.CreateCommand())
                    {
                        dataadapter = new SQLiteDataAdapter(sSql, con);
                        dataset.Reset();
                        dataadapter.Fill(dataset);
                        datatable = dataset.Tables[0];
                        datarows = datatable.Select();
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                sLasterror = ex.Message;
                sErrors += ex.Message + " ";
                MessageBox.Show(ex.Message);
                File.AppendAllText("logerrors.txt", DateTime.Now.ToString() + " " +
                Path.GetFileName(FileData) + " " + sSql + " " + ex.Message + " " + Environment.NewLine,
                System.Text.Encoding.UTF8);
            }

            return datarows;
        }
        #endregion

    }
}
