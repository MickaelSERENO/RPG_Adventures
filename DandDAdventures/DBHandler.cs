using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SQLite;

namespace DandDAdventures
{
    public class DBHandler
    {
        static String DB_INIT = @"CREATE TABLE CHARACTER(
                                    NAME VARCHAR(28) PRIMARY KEY,
                                    RACE VARCHAR(28),
                                    CLASSE VARCHAR(28));";

        protected SQLiteConnection  m_dbConnection = null;
        protected SQLiteTransaction m_dbTrans      = null;
        protected SQLiteCommand     m_dbCommand    = null;
        protected bool              m_isInMemory;


        public DBHandler(String path)
        {
            m_dbConnection          = new SQLiteConnection(path);
            m_dbConnection.Open();
            m_dbTrans               = m_dbConnection.BeginTransaction();
            m_dbCommand             = new SQLiteCommand(m_dbConnection);
            m_dbCommand.Transaction = m_dbTrans;

            m_dbCommand.CommandText = DB_INIT;
            m_dbCommand.ExecuteNonQuery();

            if (path == "Data Source=:memory:")
                m_isInMemory = true;
        }

        ~DBHandler()
        {
        }

        public void commit()
        {
            m_dbTrans.Commit();
        }

        public void rollback()
        {
            m_dbTrans.Rollback();
        }

        public void backup(String path)
        {
            using (SQLiteConnection dest = new SQLiteConnection(String.Format("Data Source = {0}", path)))
            {
                dest.Open();
                m_dbTrans.Commit();
                m_dbConnection.BackupDatabase(dest, "main", "main", -1, null, -1);
            }
        }
    }
}