using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SQLite;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DandDAdventures
{
    [Table("Class")]
    public class CharaClass
    {
        [Key]
        public String Name { get; set; }
    }

    [Table("SuperRace")]
    public class SuperRace
    {
        [Key]
        public String Name { get; set; }
    }

    [Table("Race")]
    public class Race
    {
        [Key]
        public String Name      { get; set; }
        public String SuperName { get; set; }
    }

    [Table("Character")]
    public class Character
    {
        [Key]
        public String Name  { get; set; }
        public String Race  { get; set; }
        public String Class { get; set; }
        public String Story { get; set; }
        public Decimal Type { get; set; }
    }

    [Table("PJ")]
    public class PJ
    {
        [Key]
        public String Name  { get; set; }
        public Decimal Level { get; set; }
    }

    public class DandDContext : DbContext
    { 
        public DandDContext(DbConnection conn) : base(conn, true)
        { }

        public DbSet<SuperRace>  SuperRace  { get; set; }
        public DbSet<Race>       Race       { get; set; }
        public DbSet<CharaClass> CharaClass { get; set; }
        public DbSet<Character>  Character  { get; set; }
        public DbSet<PJ>         PJ         { get; set; }
    }

    public class DBHandler
    {
        public static readonly int TYPE_PJ = 0;

        static String DB_INIT = @"CREATE TABLE SUPERRACE(
                                    NAME VARCHAR(28) PRIMARY KEY);

                                  CREATE TABLE RACE(
                                    NAME VARCHAR(28) PRIMARY KEY,
                                    SUPERNAME VARCHAR(28),
                                    FOREIGN KEY(NAME) REFERENCES SUPERRACE(NAME));


                                  CREATE TABLE CLASS(
                                    NAME VARCHAR(28) PRIMARY KEY);

                                  CREATE TABLE CHARACTER(
                                    NAME VARCHAR(28) PRIMARY KEY,
                                    RACE VARCHAR(28),
                                    CLASS VARCHAR(28),
                                    STORY TEXT,
                                    TYPE INTEGER,
                                    FOREIGN KEY(CLASS) REFERENCES CLASS(NAME)
                                    FOREIGN KEY(RACE) REFERENCES RACE(NAME));

                                  CREATE TABLE PJS(
                                    NAME VARCHAR(28),
                                    LEVEL INTEGER,
                                    FOREIGN KEY(NAME) REFERENCES CHARACTERS(NAME));";

        protected SQLiteConnection  m_dbConnection = null;
        protected SQLiteCommand     m_dbCommand    = null;
        protected DandDContext      m_dbContext;

        public DBHandler(String path)
        {
            m_dbConnection          = new SQLiteConnection(path);
            m_dbConnection.Open();

            m_dbCommand             = new SQLiteCommand(m_dbConnection)
            { 
                CommandText = DB_INIT
            };

            try
            {
                m_dbCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            { }
            
            m_dbContext = new DandDContext(m_dbConnection);
        }

        ~DBHandler()
        {
        }

        public void Commit()
        {
            m_dbContext.SaveChanges();
        }

        public void Rollback()
        {
        }

        public SQLiteConnection Backup(String path)
        {
            SQLiteConnection dest = new SQLiteConnection(String.Format("Data Source = {0}", path));
            dest.Open();
            m_dbContext.SaveChanges();
            m_dbConnection.BackupDatabase(dest, "main", "main", -1, null, -1);

            return dest;
        }

        //////////////////////////////////////
        ////////////////Queries///////////////
        //////////////////////////////////////

        //Setters
        public bool AddClass(String c)
        {
            CharaClass cc = new CharaClass()
            {
                Name = c
            };

            m_dbContext.CharaClass.Add(cc);
            return true;
        }

        public bool AddRace(String r)
        {
            String[] splitted = r.Split(new Char[] { '/' });
            if (splitted.Length > 2 || r == "")
                return false;

            else if(splitted.Length == 2)
            {
                SuperRace superRace = new SuperRace()
                {
                    Name = splitted[0]
                };

                Race race = new Race()
                {
                    SuperName = splitted[0],
                    Name      = splitted[1]
                };

                m_dbContext.SuperRace.Add(superRace);
                m_dbContext.Race.Add(race);
            }

            else
            {
                Race race = new Race()
                {
                    SuperName = null,
                    Name = splitted[0]
                };

                m_dbContext.Race.Add(race);
            }

            return true;
        }

        public bool AddPJ(Character c, PJ pj)
        {
            c.Type = DBHandler.TYPE_PJ;
            m_dbContext.Character.Add(c);
            m_dbContext.PJ.Add(pj);
            return true;
        }

        public void ChangeSQLiteConnection(SQLiteConnection c, bool saveBefore = false)
        {
            if (saveBefore)
                Commit();

            m_dbConnection?.Close();
            m_dbConnection = c;

            m_dbCommand = new SQLiteCommand(c);
            m_dbContext = new DandDContext(c);
        }

        //Getters
        public IQueryable<Character> GetCharacters()
        {
            var d = from a in m_dbContext.Character
                    orderby a.Name
                    select a;

            return d;
        }

        public IQueryable<CharaClass> GetClasses()
        {
            var d = from a in m_dbContext.CharaClass
                    orderby a.Name
                    select a;

            return d;
        }

        public IQueryable<SuperRace> GetSuperRaces()
        {
            var d = from a in m_dbContext.SuperRace
                    orderby a.Name
                    select a;

            return d;
        }

        public IQueryable<Race> GetRaces()
        {
            var d = from a in m_dbContext.Race
                    orderby a.Name
                    select a;

            return d;
        }
    }
}