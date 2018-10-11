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
    /// The resources used by this application
    /// </summary>
    [Table("Resource")]
    public class Resource
    {
        /// <summary>
        /// The Key of this resources (in the Zip file)
        /// </summary>
        [Key]
        public String Key { get; set; }
    }

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

        /// <summary>
        /// The Key of the Icon (Map)
        /// </summary>
        public String Icon { get; set; }

        /// <summary>
        /// The parent place
        /// </summary>
        public String ParentPlace { get; set; }
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

    /// <summary>
    /// Class SQL Table representing the created classes for the characters
    /// </summary>
    [Table("Class")]
    public class Class
    {
        /// <summary>
        /// Class name
        /// </summary>
        [Key]
        public String Name { get; set; }
    }

    /// <summary>
    /// Super race SQL Table representing the super races used by the characters
    /// </summary>
    [Table("SuperRace")]
    public class SuperRace
    {
        /// <summary>
        /// The SuperRace name
        /// </summary>
        [Key]
        public String Name { get; set; }
    }

    /// <summary>
    /// Race SQL Table representing the races used by the characters. A Race may have or be a SuperRace
    /// </summary>
    [Table("Race")]
    public class Race
    {
        /// <summary>
        /// The Race name
        /// </summary>
        [Key]
        public String Name      { get; set; }

        /// <summary>
        /// The SuperRace name which this Race inherits from. null if no SuperRace
        /// </summary>
        public String SuperName { get; set; }
    }

    /// <summary>
    /// The Character SQL Table representing the created characters
    /// </summary>
    [Table("Character")]
    public class Character
    {
        /// <summary>
        /// Name of the Character
        /// </summary>
        [Key]
        public String  Name  { get; set; }

        /// <summary>
        /// The name of the player playing this character
        /// </summary>
        public String PlayerName { get; set; }

        /// <summary>
        /// The Sexe of the character
        /// </summary>
        public String Sexe { get; set; }

        /// <summary>
        /// The alignment of the character
        /// </summary>
        public String Alignment { get; set; }

        /// <summary>
        /// Race of the Character (See Race SQL Table)
        /// </summary>
        public String  Race  { get; set; }

        /// <summary>
        /// The Story of this Character
        /// </summary>
        public String  Story { get; set; }

        /// <summary>
        /// The Key of the Icon resources of this Character
        /// </summary>
        public String  Icon  { get; set; }

        /// <summary>
        /// The Type of this character. 0 = PC, 1 = NPC
        /// </summary>
        public Int32   Type  { get; set; }

        /// <summary>
        /// The Strength of this character
        /// </summary>
        public Int32   Str   { get; set; }

        /// <summary>
        /// The Constitution of this character
        /// </summary>
        public Int32   Con   { get; set; }

        /// <summary>
        /// The Dexterity of this character
        /// </summary>
        public Int32   Dex   { get; set; }

        /// <summary>
        /// The Intelligence of this character
        /// </summary>
        public Int32   Int   { get; set; }

        /// <summary>
        /// The Wisdom of this character
        /// </summary>
        public Int32   Wis   { get; set; }

        /// <summary>
        /// The Charisma of this character
        /// </summary>
        public Int32   Cha   { get; set; }
    }

    /// <summary>
    /// SQL Table representing the associated class description (name + level) for a given character
    /// </summary>
    [Table("CharaClass")]
    public class CharaClass
    {
        /// <summary>
        /// The character name
        /// </summary>
        [Key, Column(Order=0)]
        public String  CharaName { get; set; }

        /// <summary>
        /// The Class name
        /// </summary>
        [Key, Column(Order=1)]
        public String  ClassName { get; set; }

        /// <summary>
        /// The level associated
        /// </summary>
        public int     Level     { get; set; }
    }

    /// <summary>
    /// GroupEvent SQL Table representing an event which has happened to a set of characters
    /// </summary>
    [Table("GroupEvent")]
    public class GroupEvent
    {
        /// <summary>
        /// The Event ID
        /// </summary>
        [Key]
        public int    ID          { get; set; }

        /// <summary>
        /// The Event Description
        /// </summary>
        public String Description { get; set; }
    }

    /// <summary>
    /// SQL Table representing a binding between characters and GroupEvent SQL Table.
    /// </summary>
    [Table("GroupBinding")]
    public class GroupBinding
    {
        /// <summary>
        /// The GroupEvent ID
        /// </summary>
        [Key, Column(Order = 1)]
        public int GroupID { get; set; }

        /// <summary>
        /// The Character Name
        /// </summary>
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
        /// The Resources known by the application. See Resource class
        /// </summary>
        public DbSet<Resource>      Resource      { get; set; }

        /// <summary>
        /// List of places. See "Place" class
        /// </summary> 
        public DbSet<Place>         Place         { get; set; }

        /// <summary>
        /// List of treasures. See Treasure class
        /// </summary>
        public DbSet<Treasure>      Treasures     { get; set; }

        /// <summary>
        /// Which character possesses a given treasure ? See TreasureChara class
        /// </summary>
        public DbSet<TreasureChara> TreasureChara { get; set; }

        /// <summary>
        /// What is the treasure value ? See TreasureValue class
        /// </summary>
        public DbSet<TreasureValue> TreasureValue { get; set; }

        /// <summary>
        /// The Super Races. See SuperRace class
        /// </summary>
        public DbSet<SuperRace>     SuperRace     { get; set; }

        /// <summary>
        /// The Races. See Race class
        /// </summary>
        public DbSet<Race>          Race          { get; set; }

        /// <summary>
        /// The Class available. See Class class
        /// </summary>
        public DbSet<Class>         Class         { get; set; }

        /// <summary>
        /// The characters available. see Character class
        /// </summary>
        public DbSet<Character>     Character     { get; set; }

        /// <summary>
        /// The chartacter class description. See CharaClass
        /// </summary>
        public DbSet<CharaClass>    CharaClass    { get; set; }

        /// <summary>
        /// The groupevent created. See GroupEvent class
        /// </summary>
        public DbSet<GroupEvent>    GroupEvent    { get; set; }

        /// <summary>
        /// The binding between characters and GroupEvent. See GroupBinding class
        /// </summary>
        public DbSet<GroupBinding>  GroupBinding  { get; set; }
    }

    /// <summary>
    /// DBHandler class. Interact with the SQLite Database to get, set and add values into it
    /// </summary>
    public class DBHandler
    {
        public static readonly int TYPE_Character = 0;

        /// <summary>
        /// The Database Initial string in order to create all the tables
        /// </summary>
        static String DB_INIT =
#region
            @"CREATE TABLE RESOURCE(
                KEY TEXT PRIMARY KEY);

              CREATE TABLE PLACE(
                NAME  VARCHAR(48) PRIMARY KEY,
                STORY TEXT,
                ICON TEXT,
                PARENTPLACE VARCHAR(48),
                FOREIGN KEY(PARENTPLACE) REFERENCES PLACE(NAME));

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
                FOREIGN KEY(SUPERNAME) REFERENCES SUPERRACE(NAME));

              CREATE TABLE CLASS(
                NAME VARCHAR(48) PRIMARY KEY);

              CREATE TABLE CHARACTER(
                NAME VARCHAR(48) PRIMARY KEY,
                PLAYERNAME VARCHAR(48),
                SEXE VARCHAR(48),
                ALIGNMENT VARCHAR(48),
                RACE VARCHAR(48),
                STORY TEXT,
                ICON  TEXT,
                TYPE INTEGER,
                STR  INTEGER,
                CON  INTEGER,
                DEX  INTEGER,
                INT  INTEGER,
                WIS  INTEGER,
                CHA  INTEGER,
                FOREIGN KEY(RACE) REFERENCES RACE(NAME),
                FOREIGN KEY(ICON) REFERENCES RESOURCES(KEY));

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
#endregion

        /// <summary>
        /// The SQLite connection
        /// </summary>
        protected SQLiteConnection  m_dbConnection = null;

        /// <summary>
        /// The Object permitting to use commands regarding the SQLite database
        /// </summary>
        protected SQLiteCommand     m_dbCommand    = null;

        /// <summary>
        /// The Database Context : matches between the Database and our application (through LINQ)
        /// </summary>
        protected DandDContext      m_dbContext;

        /// <summary>
        /// The Constructor. Initialize the SQLite connection determined by a path file
        /// </summary>
        /// <param name="path">The SQLite string initializer. Can be a path via 'Data Source=path'.</param>
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

        /// <summary>
        /// Destructor. Does nothing in its current state
        /// </summary>
        ~DBHandler()
        {
        }

        /// <summary>
        /// Commit the database
        /// </summary>
        public void Commit()
        {
            m_dbContext.SaveChanges();
        }

        /// <summary>
        /// Rollback to the last commit state
        /// </summary>
        public void Rollback()
        {
        }

        /// <summary>
        /// Backup this database to a file path
        /// </summary>
        /// <param name="path">The file path (see Data Source = in SQLite)</param>
        /// <returns></returns>
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
        //////////////////////
        ////////Setters///////
        //////////////////////

        /// <summary>
        /// Add a new Resources into the database. See Resource class
        /// </summary>
        /// <param name="key">The key of the resource</param>
        /// <returns>true on success, false on failure. TODO In its current state, the application returns only true</returns>
        public bool AddResource(String key)
        {
            m_dbContext.Resource.Add(new Resource { Key = key });
            return true;
        }

        /// <summary>
        /// Add a new class in the database
        /// </summary>
        /// <param name="c">The class name to add</param>
        /// <returns>True on success, false on failure. TODO: returns currently only true</returns>
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

        /// <summary>
        /// Set a character internal data
        /// The name cannot currently be changed
        /// </summary>
        /// <param name="chara">The character information to set in the database. 
        /// The Function look at chara.Name to determine which Character has to be changed in the database</param>
        public void SetChara(Character chara)
        {
            foreach (var c in m_dbContext.Character.Where(d => d.Name == chara.Name).ToList())
            {
                c.Icon  = chara.Icon;
                c.Str   = chara.Str;
                c.Con   = chara.Con;
                c.Dex   = chara.Dex;
                c.Int   = chara.Int;
                c.Wis   = chara.Wis;
                c.Cha   = chara.Cha;
                c.Story = chara.Story;
            }
        }

        /// <summary>
        /// Add a new Race into the database
        /// </summary>
        /// <param name="sr">The superrace name</param>
        /// <param name="r">The race name</param>
        /// <returns></returns>
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

        /// <summary>
        /// Add a SuperRace into the database
        /// </summary>
        /// <param name="name">The SuperRace name</param>
        /// <returns>true on success, false in failure. TODO returns only true currently</returns>
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

        /// <summary>
        /// Add a character into the database
        /// </summary>
        /// <param name="c">The character to Add</param>
        /// <returns>true on success, false in failure. TODO returns only true currently</returns>
        public bool AddCharacter(Character c)
        {
            var d = from a in m_dbContext.Character
                    where a.Name == c.Name
                    select a;

            foreach (var a in d)
                return false;

            c.Type = DBHandler.TYPE_Character;
            m_dbContext.Character.Add(c);

            Commit();

            return true;
        }

        /// <summary>
        /// Add a Character Class description (see CharaClass) into the database
        /// </summary>
        /// <param name="cc">The Character Class description</param>
        /// <returns></returns>
        public bool AddCharaClass(CharaClass cc)
        {
            m_dbContext.CharaClass.Add(cc);
            Commit();
            return true;
        }

        /// <summary>
        /// Add a new Data (Even) into the database
        /// </summary>
        /// <param name="desc">The description of the data</param>
        /// <param name="chara">The List of characters concerned by this date</param>
        /// <returns></returns>
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

        /// <summary>
        /// Add a place into the database
        /// </summary>
        /// <param name="place">The place to add</param>
        public void AddPlace(Place place)
        {
            m_dbContext.Place.Add(place);
            Commit();
        }

        /// <summary>
        /// Set a Place data into the database. This function looks into place.Name to determine which place he has to modified. He modifies the others fields
        /// </summary>
        /// <param name="place">The place containing the information to modify, except the name (the name is a key)</param>
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

        /// <summary>
        /// Get the GroupEvents associated to a character
        /// </summary>
        /// <param name="name">The character name</param>
        /// <returns>A Queryable object containing GroupEvents bind to the Character "name"</returns>
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