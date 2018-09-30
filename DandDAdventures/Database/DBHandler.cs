using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SQLite;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DandDAdventures.XAML;
using System.Collections.Generic;

namespace DandDAdventures
{

    /////////////////////////////////////////////////////
    //Database class, i.e sql class tables (Using Linq)//
    /////////////////////////////////////////////////////
    #region

    /// <summary>
    /// The Place SQL Table
    /// </summary>
    [Table("Place")]
    public class Place
    {
        /// <summary>
        /// The name of the place
        /// </summary>
        [Key]
        public String Name  { get; set; }

        /// <summary>
        /// A description
        /// </summary>
        public String Story { get; set; }
    }

    /// <summary>
    /// Treasure SQL Table
    /// </summary>
    [Table("Treasure")]
    public class Treasure
    {
        /// <summary>
        /// ID of the treasure. Generated automaticaly (do not set it when creating the object)
        /// </summary>
        [Key]
        public int    ID        { get; set; } = -1;

        /// <summary>
        /// The place associated with this treasure
        /// </summary>
        public String PlaceName { get; set; }

        /// <summary>
        /// Is the treasure opened ?
        /// </summary>
        public bool   Opened    { get; set; }
    }

    /// <summary>
    /// TreasureValue SQL Table representing an object appartening to a treasure (see Treasure)
    /// </summary>
    [Table("TreasureValue")]
    public class TreasureValue
    {
        /// <summary>
        /// ID of the object (treasure object). Generated automaticaly (do not set it when creating the object)
        /// </summary>
        [Key]
        public int    ID      { get; set; } = -1;

        /// <summary>
        /// ID of the treasure associated with this object.
        /// </summary>
        public int    IDTR    { get; set; }

        /// <summary>
        /// The object name
        /// </summary>
        public String ObjName { get; set; }

        /// <summary>
        /// The object value (in gold for example)
        /// </summary>
        public int    Value   { get; set; }
    }

    /// <summary>
    /// TreasureChara SQL Table representing the character which owns the treasure
    /// Multiple characters can own that treasure
    /// </summary>
    [Table("TreasureChara")]
    public class TreasureChara
    {
        /// <summary>
        /// ID of the treasure
        /// </summary>
        [Key, Column(Order=0)]
        public int IDTR { get; set; }

        /// <summary>
        /// ID of the character
        /// </summary>
        [Key, Column(Order=1)]
        public String CharaName { get; set; }
    }

    [Table("Class")]
    public class Class
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
        public String  Name  { get; set; }
        public String  Race  { get; set; }
        public String  Story { get; set; }
        public Int32   Type  { get; set; }
        public Int32   Str   { get; set; }
        public Int32   Con   { get; set; }
        public Int32   Dex   { get; set; }
        public Int32   Int   { get; set; }
        public Int32   Wis   { get; set; }
        public Int32   Cha   { get; set; }
    }

    [Table("CharaClass")]
    public class CharaClass
    {
        [Key, Column(Order=0)]
        public String  CharaName { get; set; }
        [Key, Column(Order=1)]
        public String  ClassName { get; set; }
        public decimal Level     { get; set; }
    }

    [Table("GroupEvent")]
    public class GroupEvent
    {
        [Key]
        public int    ID          { get; set; }
        public String Description { get; set; }
    }

    [Table("GroupBinding")]
    public class GroupBinding
    {
        [Key, Column(Order = 1)]
        public int GroupID { get; set; }
        [Key, Column(Order = 2)]
        public String Name { get; set; }
    }
    #endregion
    
    /// <summary>
    /// DBContext class (see linq). Contain object matching the content of the SQLite database (via DbSet)
    /// </summary>
    public class DandDContext : DbContext
    { 
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <param name="conn">The database connection</param>
        public DandDContext(DbConnection conn) : base(conn, true)
        { }

        /// <summary>
        /// List of places
        /// </summary>
        public DbSet<Place>         Place         { get; set; }

        /// <summary>
        /// List of treasures
        /// </summary>
        public DbSet<Treasure>      Treasures     { get; set; }
        public DbSet<TreasureChara> TreasureChara { get; set; }
        public DbSet<TreasureValue> TreasureValue { get; set; }
        public DbSet<SuperRace>     SuperRace     { get; set; }
        public DbSet<Race>          Race          { get; set; }
        public DbSet<Class>         Class         { get; set; }
        public DbSet<Character>     Character     { get; set; }
        public DbSet<CharaClass>    CharaClass    { get; set; }
        public DbSet<GroupEvent>    GroupEvent    { get; set; }
        public DbSet<GroupBinding>  GroupBinding  { get; set; }
    }

    public class DBHandler
    {
        public static readonly int TYPE_PJ = 0;

        static String DB_INIT = @"CREATE TABLE PLACE(
                                    NAME  VARCHAR(48) PRIMARY KEY,
                                    STORY TEXT);

                                  CREATE TABLE TREASURE(
                                    ID        INTEGER PRIMARY KEY AUTOINCREMENT,
                                    PLACENAME VARCHAR(48),
                                    OPENED    BOOLEAN,
                                    FOREIGN KEY(PLACENAME) REFERENCES PLACE(NAME));

                                  CREATE TABLE TREASURECHARA(
                                    IDTR      INTEGER,
                                    CHARANAME VARCHAR(48),
                                    PRIMARY KEY(IDTR, CHARANAME));

                                  CREATE TABLE TREASUREVALUE(
                                    ID      INTEGER PRIMARY KEY AUTOINCREMENT,
                                    IDTR    INTEGER,
                                    OBJNAME VARCHAR(48),
                                    VALUE   INTEGER,
                                    FOREIGN KEY(IDTR) REFERENCES TREASURE(ID));

                                  CREATE TABLE SUPERRACE(
                                    NAME VARCHAR(48) PRIMARY KEY);

                                  CREATE TABLE RACE(
                                    NAME VARCHAR(48) PRIMARY KEY,
                                    SUPERNAME VARCHAR(48),
                                    FOREIGN KEY(NAME) REFERENCES SUPERRACE(NAME));

                                  CREATE TABLE CLASS(
                                    NAME VARCHAR(48) PRIMARY KEY);

                                  CREATE TABLE CHARACTER(
                                    NAME VARCHAR(48) PRIMARY KEY,
                                    RACE VARCHAR(48),
                                    STORY TEXT,
                                    TYPE INTEGER,
                                    STR  INTEGER,
                                    CON  INTEGER,
                                    DEX  INTEGER,
                                    INT  INTEGER,
                                    WIS  INTEGER,
                                    CHA  INTEGER,
                                    FOREIGN KEY(RACE) REFERENCES RACE(NAME));

                                  CREATE TABLE CHARACLASS(
                                    CHARANAME VARCHAR(48),
                                    CLASSNAME VARCHAR(48),
                                    LEVEL     INTEGER,
                                    PRIMARY KEY(CHARANAME, CLASSNAME),
                                    FOREIGN KEY(CHARANAME) REFERENCES CHARACTER(NAME),
                                    FOREIGN KEY(CLASSNAME) REFERENCES CLASS(NAME));

                                  CREATE TABLE GROUPEVENT(
                                    ID          INTEGER PRIMARY KEY AUTOINCREMENT,
                                    DESCRIPTION TEXT);

                                  CREATE TABLE GROUPBINDING(
                                    GROUPID INTEGER,
                                    NAME    VARCHAR(48),
                                    PRIMARY KEY(GROUPID, NAME),
                                    FOREIGN KEY(GROUPID) REFERENCES GROUPEVENT(ID),
                                    FOREIGN KEY(NAME)  REFERENCES CHARACTER(NAME));";

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
            catch (Exception)
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
#region
        //Setters
        public bool AddClass(String c)
        {
            Class cc = new Class()
            {
                Name = c
            };

            m_dbContext.Class.Add(cc);
            Commit();
            return true;
        }

        public void SetChara(Character chara)
        {
            foreach (var c in m_dbContext.Character.Where(d => d.Name == chara.Name).ToList())
            {
                c.Story = chara.Story;
            }
        }

        public bool AddRace(String sr, String r)
        {
            Race race = new Race()
            {
                SuperName = sr,
                Name      = r
            };

            m_dbContext.Race.Add(race);
            Commit();

            return true;
        }

        public bool AddSuperRace(String name)
        {
            SuperRace superRace = new SuperRace()
            {
                Name = name
            };

            Race race = new Race()
            {
                SuperName = null,
                Name = name
            };

            m_dbContext.SuperRace.Add(superRace);
            m_dbContext.Race.Add(race);

            Commit();

            return true;
        }

        public bool AddPJ(Character c)
        {
            var d = from a in m_dbContext.Character
                    where a.Name == c.Name
                    select a;

            foreach (var a in d)
                return false;

            c.Type = DBHandler.TYPE_PJ;
            m_dbContext.Character.Add(c);

            Commit();

            return true;
        }

        public bool AddCharaClass(CharaClass cc)
        {
            m_dbContext.CharaClass.Add(cc);
            Commit();
            return true;
        }

        public GroupEvent AddDate(String desc, Character[] chara)
        {
            GroupEvent gp = new GroupEvent() { ID = 0, Description = desc };
            m_dbContext.GroupEvent.Add(gp);

            Commit();
            
            foreach (var c in chara)
                m_dbContext.GroupBinding.Add(new GroupBinding() { GroupID = gp.ID, Name = c.Name });

            Commit();
            return gp;
        }

        public void AddPlace(Place place)
        {
            m_dbContext.Place.Add(place);
            Commit();
        }

        public void SetPlace(Place place)
        {
            m_dbContext.Place.Where(d => d.Name == place.Name).ToList().ForEach(d=> d.Story = place.Story);
        }

        public void AddTreasure(IEnumerable<TreasureItem> collection)
        {
            foreach (var ti in collection)
                if(ti.Treasure.ID == -1)
                    m_dbContext.Treasures.Add(ti.Treasure);

            Commit();

            foreach (var ti in collection)
                foreach (var tv in ti.TreasureValue)
                {
                    if (tv.ID == -1)
                    {
                        tv.IDTR = ti.Treasure.ID;
                        m_dbContext.TreasureValue.Add(tv);
                    }
                    else
                    {
                        foreach(var c in m_dbContext.TreasureValue.Where(d => d.ID == tv.ID).ToList())
                        {
                            //do your stuff here
                            c.ObjName = tv.ObjName;
                            c.Value   = tv.Value;
                        }
                    }
                }

            Commit();
        }

        public void SetTreasure(Treasure tr)
        {
            m_dbContext.Treasures.Where(d => d.ID == tr.ID).First().Opened = tr.Opened;
            Commit();
        }

        public void SetDate(GroupEvent ev)
        {
            m_dbContext.GroupEvent.Where(d => d.ID == ev.ID).First().Description = ev.Description;
            Commit();
        }

        public void AddTreasureOwner(TreasureChara trch)
        {
            m_dbContext.TreasureChara.Add(trch);
            Commit();
        }

        public void RefreshTreasureOwner(TreasureItem ti)
        {
            //Delete every thing
            var deleteVal = from tr in m_dbContext.TreasureChara
                            where tr.IDTR == ti.Treasure.ID
                            select tr;

            m_dbContext.TreasureChara.RemoveRange(deleteVal);

            //Commit everything
            foreach (StringWrapped name in ti.TreasureOwner)
                if(name.Value != "")
                    AddTreasureOwner(new TreasureChara { CharaName = name.Value, IDTR = ti.Treasure.ID });

            Commit();
        }
        
        public void DeleteTreasureOwner(String charaName, int treasureID)
        {
            m_dbContext.TreasureChara.Remove(m_dbContext.TreasureChara.First(trch => trch.IDTR == treasureID && trch.CharaName == charaName));
            Commit();
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
        public IQueryable<Place> GetPlaces()
        {
            var d = from a in m_dbContext.Place
                    orderby a.Name
                    select a;

            return d;
        }

        public IQueryable<String> GetPlaceListName()
        {
            var d = from a in m_dbContext.Place
                    select a.Name;

            return d;
        }

        public IQueryable<Treasure> GetTreasures(String placeName)
        {
            var d = from a in m_dbContext.Treasures
                    where a.PlaceName == placeName
                    orderby a.ID
                    select a;

            return d;
        }

        public IQueryable<TreasureValue> GetTreasureValues(int trid)
        {
            var d = from a in m_dbContext.TreasureValue
                    where a.IDTR == trid
                    orderby a.ID
                    select a;

            return d;
        }

        public IQueryable<String> GetTreasureOwners(int trid)
        {
            var d = from a in m_dbContext.TreasureChara
                    where a.IDTR == trid
                    orderby a.CharaName
                    select a.CharaName;

            return d;
        }

        public IQueryable<Character> GetCharacters()
        {
            var d = from a in m_dbContext.Character
                    orderby a.Name
                    select a;

            return d;
        }

        public IQueryable<String> GetCharaListName()
        {
            var d = from a in m_dbContext.Character
                    select a.Name;

            return d;
        }

        public IQueryable<Class> GetClasses()
        {
            var d = from a in m_dbContext.Class
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

        public IQueryable<String> GetCharaSummary(String name)
        {
            var d = from a in m_dbContext.Character
                    where a.Name == name
                    select a.Story;

            return d;
        }

        public IQueryable<GroupEvent> GetGroupEvents(String name)
        {
            var d = from ge in m_dbContext.GroupEvent
                    join gb in m_dbContext.GroupBinding on ge.ID equals gb.GroupID
                    where gb.Name == name
                    select ge;

            return d;
        }

        public IQueryable<String> GetCharaNamesEvent(GroupEvent ge)
        {
            var d = from gb in m_dbContext.GroupBinding
                    where gb.GroupID == ge.ID
                    select gb.Name;

            return d;
        }

        //Simple Getters
        public String[] GetListName()
        {
            //Get a List of all Names
            String[] listPlaceName = GetPlaceListName().ToArray();
            String[] listCharaName = GetCharaListName().ToArray();
            String[] listName = new string[listCharaName.Length + listPlaceName.Length];

            Array.Copy(listPlaceName, listName, listPlaceName.Length);
            Array.Copy(listCharaName, 0, listName, listPlaceName.Length, listCharaName.Length);

            return listName;
        }
#endregion
    }
}