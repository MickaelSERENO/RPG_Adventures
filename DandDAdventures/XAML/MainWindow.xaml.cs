using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using DandDAdventures.XAML.Controls;
using DandDAdventures.XAML.Listeners;
using Microsoft.Win32;
using System;
using System.IO;
using System.Data.SQLite;
using System.Collections.Generic;
using System.Windows.Input;
using System.Linq;
using System.IO.Compression;
using DandDAdventures.Database;
using System.Windows.Media.Imaging;

namespace DandDAdventures.XAML
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, ICommitDatabase, ISelectedTree, ILinkName
    {
        /// <summary>
        /// The Window Data associated with this Window
        /// </summary>
        protected WindowData     m_windowData;

        /// <summary>
        /// The Main Panel (center part of the UI).
        /// </summary>
        protected Grid           m_mainPanel;

        /// <summary>
        /// The Content of the center part of the UI
        /// </summary>
        protected ContentControl m_mainControl;

        /// <summary>
        /// The Content of the Character Tab Control (right part of the UI)
        /// </summary>
        protected ContentControl m_CharacterTabControl;

        /// <summary>
        /// The Content of the PNJ Tab Control (
        /// </summary>
        protected ContentControl m_pnjTabControl;

        /// <summary>
        /// The Content of the Place Tab Control
        /// </summary>
        protected ContentControl m_placeTabControl;

        //The Main Views
        protected CharacterView         m_CharacterView;
        protected PlaceView      m_placeView;

        //The TabItem Views
        protected CharacterTabItemCtrl    m_CharacterTabItem;
        protected PlaceTabItemCtrl m_placeTabItem;

        //The AddListeners
        protected AddCharacterListener    m_addCharacterListener;
        protected AddPlaceListener m_addPlaceListener;

        /// <summary>
        /// Constructor. Initialize the Main Window
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            m_windowData = new WindowData(null, this, this, this);
            this.DataContext = m_windowData;

            //Initialize the MainPanel
            m_mainPanel = (Grid)this.FindName("MainPanel");
            m_mainPanel.Visibility = Visibility.Hidden;

            //Get and initialize the values for the MainControl View
            m_CharacterView      = new CharacterView(m_windowData);
            m_placeView   = new PlaceView(m_windowData);
            m_mainControl = (ContentControl)this.FindName("MainControl");

            //Initialize the Listeners
            m_addCharacterListener    = new AddCharacterListener(m_windowData);
            m_addPlaceListener = new AddPlaceListener(m_windowData);

            //Get and initialize the TabControl 
            m_CharacterTabControl    = (ContentControl)this.FindName("CharacterTabItem");
            m_pnjTabControl   = (ContentControl)this.FindName("PNJTabItem");
            m_placeTabControl = (ContentControl)this.FindName("PlaceTabItem");

            m_CharacterTabItem       = new CharacterTabItemCtrl(m_windowData);
            m_placeTabItem    = new PlaceTabItemCtrl(m_windowData);

            m_CharacterTabControl.Content    = m_CharacterTabItem;
            m_placeTabControl.Content = m_placeTabItem;

            m_CharacterTabItem.SetAddListener(m_addCharacterListener);
            m_placeTabItem.SetAddListener(m_addPlaceListener);

            //Launch the Window
            SetToCharacterMainView();
        }

        /// <summary>
        /// Function called by WPF when this Window is about to close.
        /// Delete the temporary directory
        /// </summary>
        protected override void OnClosing(CancelEventArgs e)
        {
            //Delete the temporary directory
            if(m_windowData.TempDir != null)
                Directory.Delete(m_windowData.TempDir, true);
            base.OnClosing(e);
        }

        /// <summary>
        /// Set the main view to the Character Tab View
        /// </summary>
        private void SetToCharacterMainView()
        {
            m_mainControl.Content = m_CharacterView;
            TabControl tabCtrl = FindName("TabCtrl") as TabControl;
            tabCtrl.SelectedIndex = ((TabItem)(m_CharacterTabControl.Parent)).TabIndex;
        }

        /// <summary>
        /// Set the main view to the Place Tab View
        /// </summary>
        private void SetToPlaceMainView()
        {
            m_mainControl.Content = m_placeView;
            TabControl tabCtrl = FindName("TabCtrl") as TabControl;
            tabCtrl.SelectedIndex = ((TabItem)(m_placeTabControl.Parent)).TabIndex;
        }

        /// <summary>
        /// Function called when the "New" Button is pressed. Initialize a new database
        /// </summary>
        /// <param name="sender">The object calling this function</param>
        /// <param name="e">The RoutedEventArgs associated</param>
        private void NewFile(object sender, RoutedEventArgs e)
        {
            m_windowData.Clean();

            if (m_windowData.SQLDatabase != null)
            {
                //TODO maybe something
            }

            m_windowData.SQLDatabase = new DBHandler("Data Source=:memory:");
            m_windowData.CanSave     = true;
            m_mainPanel.Visibility   = Visibility.Visible;
        }

        /// <summary>
        /// Function called by the Open Button. Open a file and load the application
        /// </summary>
        /// <param name="sender">The object calling this function.</param>
        /// <param name="e">The RoutedEventArgs associated</param>
        private void OpenFile(object sender, RoutedEventArgs e)
        {
            m_windowData.Clean();

            OpenFileDialog openFile = new OpenFileDialog();
            if (openFile.ShowDialog() == true)
            {
                //Unzip the database (.db) file
                String tempDir = $"c:/Temp/DandDAdventures/{DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds}/";
                String tempDB  = tempDir + "database.db";
                Directory.CreateDirectory(tempDir);
                using(FileStream fs = new FileStream(openFile.FileName, FileMode.Open))
                {
                    using(ZipArchive zip = new ZipArchive(fs))
                    {
                        ZipArchiveEntry dbEntry = zip.GetEntry("database.db");
                        using(Stream dbStream = dbEntry.Open())
                        {
                            using(FileStream fileStream = File.Create(tempDB))
                            {
                                dbStream.CopyTo(fileStream);
                            }
                        }
                    }
                }

                //Open and load the Database
                if(m_windowData.SQLDatabase == null)
                    m_windowData.SQLDatabase = new DBHandler(String.Format("DataSource = {0}", tempDB));
                else
                {
                    var sql = new SQLiteConnection(String.Format("DataSource = {0}", tempDB));
                    sql.Open();
                    m_windowData.SQLDatabase.ChangeSQLiteConnection(sql);
                }

                //Change to the memory
                m_windowData.SQLDatabase.ChangeSQLiteConnection(m_windowData.SQLDatabase.Backup(":memory:"));

                //Delete the temporary database
                File.Delete(tempDB);

                //Load the content in the FrontEnd
                m_CharacterTabItem.LoadContent(m_windowData);
                m_CharacterView.LoadContent(m_windowData);

                m_placeTabItem.LoadContent(m_windowData);
                m_placeView.LoadContent(m_windowData);

                m_windowData.SQLPath = openFile.FileName;
                m_windowData.CanSave = true;
                m_mainPanel.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Function called when the Save Button is pressed. Save the data into a ZIP file
        /// </summary>
        /// <param name="sender">The object calling this function</param>
        /// <param name="e">The RoutedEventArgs associated</param>
        private void SaveFile(object sender, RoutedEventArgs e)
        {
            if (m_windowData.SQLPath == null)
            {
                SaveFileDialog saveFile = new SaveFileDialog();
                if (saveFile.ShowDialog() == true)
                    m_windowData.SQLPath = saveFile.FileName; 
            }

            if(m_windowData.SQLPath != null)
            {
                m_windowData.SQLDatabase.Commit();
                m_windowData.SQLDatabase.Backup(m_windowData.TempDir + "database.db").Close();

                using(FileStream fs = new FileStream(m_windowData.SQLPath, FileMode.OpenOrCreate))
                {
                    //Modify the ZIP file
                    using(ZipArchive zip = new ZipArchive(fs, ZipArchiveMode.Update))
                    {
                        ////////////////////////////////
                        //Update the database file entry
                        ////////////////////////////////
                    
                        //Delete the entry if it exists
                        ZipArchiveEntry entry = zip.GetEntry("database.db");
                        if(entry != null)
                            entry.Delete();
                        entry = zip.CreateEntry("database.db");

                        //Set the entry data
                        using(Stream str = entry.Open())
                        {
                            FileStream dbFS = new FileStream(m_windowData.TempDir + "database.db", FileMode.Open);
                            dbFS.CopyTo(str);
                        }

                        //Update the new resources file entry
                        //Actually a new resources can be a modification of another resource (like changing a character icon)
                        foreach(DBResource r in m_windowData.NewResources.Values)
                        {
                            //Delete the entry if it exists
                            entry = zip.GetEntry(r.Key);
                            if(entry != null)
                                entry.Delete();
                            entry = zip.CreateEntry(r.Key);

                            //Set the entry data
                            using(Stream entryStr = entry.Open())
                            {
                                entryStr.Write(r.Data, 0, (int)r.Length);
                            }
                        }
                    }
                }
            }
        }

        private void SetCharacter(object sender, RoutedEventArgs e)
        {
            
        }

        private void SetPlace(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Add characters into the application memory
        /// </summary>
        /// <param name="charas"></param>
        public void AddCharacter(Character[] charas)
        {
            foreach(var chara in charas)
            {
                m_windowData.CharacterDatas.CharacterList.Add(chara);
            }
        }

        /// <summary>
        /// Add a place into the application memory
        /// </summary>
        /// <param name="p"The place name></param>
        public void AddPlace(Place p)
        {
            m_windowData.PlaceDatas.PlaceList.Add(p);
        }

        public BitmapImage GetBitmapImageFromKeyRes(String key)
        {
            DBResource res = m_windowData.GetResource(key);
            if (res != null)
            {
                BitmapImage img = new BitmapImage();
                img.BeginInit();
                img.StreamSource = new MemoryStream(res.Data);
                img.EndInit();
                return img;
            }
            return null;
        }
        
        public void OnSelectCharacter(Character[] chara)
        {
            if(chara.Length == 1)
            {
                m_windowData.CurrentCharacter = chara[0].Name;

                m_CharacterView.SetSummary(chara[0].Story);
                m_CharacterView.SetGroupEvent(chara[0].Name);

                if(chara[0].Icon != null)
                {
                    BitmapImage img = GetBitmapImageFromKeyRes(chara[0].Icon);
                    if (img != null) { m_CharacterView.SetCharacterIcon(img); }
                }
            }
        }

        public void OnSelectPlace(Place place)
        {
            if(place == null)
            {
                m_placeView.SetPlaceStory("");
                m_windowData.PlaceDatas.TreasureList.Clear();
            }

            else
            {
                //Set the Story
                m_placeView.SetPlaceStory(place.Story);

                //Fill the model about Treasures
                m_windowData.PlaceDatas.TreasureList.Clear();

                var treasures = m_windowData.SQLDatabase.GetTreasures(place.Name);
                int i = 0;

                foreach (var tr in treasures)
                {
                    TreasureItem trItem = new TreasureItem() { Treasure = tr, TreasureID = i };
                    var values = m_windowData.SQLDatabase.GetTreasureValues(tr.ID);
                    var owners = m_windowData.SQLDatabase.GetTreasureOwners(tr.ID);

                    foreach (var owner in owners)
                        trItem.TreasureOwner.Add(new StringWrapped { Value = owner });
                    foreach (var value in values)
                        trItem.TreasureValue.Add(value);
                    m_windowData.PlaceDatas.TreasureList.Add(trItem);
                    i++;
                }

                BitmapImage img = GetBitmapImageFromKeyRes(place.Icon);
                if (img != null) { m_windowData.PlaceDatas.PlaceIcon = img; } ;
            }
        }

        public void AddDate(CreateDate cd, Character[] chara)
        {
            GroupEvent ge = m_windowData.SQLDatabase.AddDate(cd.Description, chara);
            m_CharacterView.AddGroupEvent(ge);

        }

        private void CharacterTabItemSelected(object sender, RoutedEventArgs e)
        {
            SetToCharacterMainView();
        }

        private void PlaceTabItemSelected(object sender, RoutedEventArgs e)
        {
            SetToPlaceMainView();
        }

        public void LinkToName(String name)
        {
            String[] placeName = m_windowData.SQLDatabase.GetPlaceListName().ToArray();
            String[] charaName = m_windowData.SQLDatabase.GetCharaListName().ToArray();

            if (placeName.Contains(name))
            {
                foreach (Place p in m_windowData.PlaceDatas.PlaceList)
                {
                    if (p.Name == name)
                    {
                        m_windowData.PlaceDatas.PlaceSelected = p;
                        break;
                    }
                }
                SetToPlaceMainView();
            }

            else
            {
                foreach(Character c in m_windowData.CharacterDatas.CharacterList)
                {
                    if(c.Name == name)
                    {
                        m_windowData.CharacterDatas.CharacterSelected = c;
                        break;
                    }
                }
                SetToCharacterMainView();
            }
        }
    }

    /// <summary>
    /// The Application data shared by all the application components.
    /// It extends INotifyPropertyChanged for changing the view when the internal data state has changed
    /// </summary>
    public class WindowData : INotifyPropertyChanged
    {
        //////////////////////////
        ////PRIVATE ATTRIBUTES////
        //////////////////////////
#region
        /// <summary>
        /// The temporary directory in use
        /// </summary>
        protected String m_tempDir = null;

        /// <summary>
        /// Can we save the application ?
        /// </summary>
        protected bool   m_canSave   = false;

        /// <summary>
        /// The actual sql system path
        /// </summary>
        protected String m_sqlPath   = null;

        /// <summary>
        /// The name of the current Character selected
        /// </summary>
        protected String m_currentCharacter = "";

        /// <summary>
        /// The database handler
        /// </summary>
        protected DBHandler       m_dbHandler;

        /// <summary>
        /// The interface permitting to commit into the database. Point to MainWindow object
        /// </summary>
        protected ICommitDatabase m_commitDB;

        /// <summary>
        /// The interface permitting to link a name to another part of the UI (shortcuts). Point to MainWindow object
        /// For example, clicking on "Westcott Yulis" can point you to the character shit of Westcott Yulis
        /// </summary>
        protected ILinkName       m_linkName;

        /// <summary>
        /// Interface permitting to update the UI when selecting part of the UI (places, characters, dates, etc.)
        /// Points to MainWindow object
        /// </summary>
        protected ISelectedTree   m_selectedTree;

        /// <summary>
        /// The DataContext of the CharacterView.xaml
        /// </summary>
        protected CharacterDataContext    m_CharacterDatas;

        /// <summary>
        /// The DataContext of the PlaceView.xaml
        /// </summary>
        protected PlaceDataContext m_placeDatas;

        /// <summary>
        /// The new resources to add into the next data ZIP 
        /// Can also be used directly by the application
        /// </summary>
        protected Dictionary<String, DBResource> m_newResources;

        /// <summary>
        /// Dictionary containing already loaded resources
        /// </summary>
        protected Dictionary<String, DBResource> m_loadedResources;

#endregion

        /// <summary>
        /// Event attributes permiting to notify the view that the model has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        //////////////////////////
        /////PUBLIC FUNCTIONS/////
        //////////////////////////
#region
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sqlDatas">The database handler</param>
        /// <param name="commitDB">The interface permitting to modify the database</param>
        /// <param name="linkName">The interface permitting to add link into a text based on already known name (character names, place names, etc.)</param>
        /// <param name="selectedTree"></param>
        public WindowData(DBHandler sqlDatas, ICommitDatabase commitDB, ILinkName linkName, ISelectedTree selectedTree)
        {
            m_dbHandler       = sqlDatas;
            m_commitDB        = commitDB;
            m_linkName        = linkName;
            m_selectedTree    = selectedTree;
            m_newResources    = new Dictionary<String, DBResource>();
            m_loadedResources = new Dictionary<String, DBResource>();
            m_CharacterDatas         = new CharacterDataContext(this);
            m_placeDatas      = new PlaceDataContext(this);
        }

        /// <summary>
        /// Clear the application data. Used when "New" is pressed for example
        /// </summary>
        public void Clean()
        {
            CanSave = false;
            CurrentCharacter = null;
            m_sqlPath = null;

            m_CharacterDatas.Clean();
            m_placeDatas.Clean();
        }

        /// <summary>
        /// Add a new resource for save purpose
        /// </summary>
        /// <param name="input">The input data</param>
        /// <param name="length">The input length</param>
        /// <param name="key">The key associated with this resource</param>
        public void AddResource(byte[] input, long length, String key)
        {
            m_newResources.Add(key, new DBResource(input, length, key));
        }
        /// <summary>
        /// Add a new resource for save purpose
        /// </summary>
        /// <param name="s">The Stream data</param>
        /// <param name="key">The key associated with this resource</param>
        public void AddResource(Stream s, String key)
        {
            DBResource res = new DBResource(s, key);
            m_newResources.Add(key, res);
            m_loadedResources.Add(key, res);
        }

        /// <summary>
        /// Get a Resource base on the key of the resources (ZIP key). 
        /// If the resource is already loaded, we do not read again the ZIP file
        /// </summary>
        /// <param name="key">The key to look at</param>
        /// <returns>The DBResource corresponding to this key if found. Null otherwise</returns>
        public DBResource GetResource(String key)
        {
            if(m_loadedResources.ContainsKey(key))
                return m_loadedResources[key];

            using(FileStream fs = new FileStream(SQLPath, FileMode.Open))
            {
                //Modify the ZIP file
                using(ZipArchive zip = new ZipArchive(fs, ZipArchiveMode.Update))
                {
                    ZipArchiveEntry entry = zip.GetEntry(key);
                    if(entry == null)
                        return null;
                    return new DBResource(entry.Open(), key);
                }
            }

            return null;
        }

        /// <summary>
        /// Specify that a property has changed
        /// </summary>
        /// <param name="name">The name of the property</param>
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
#endregion

        ///////////////////////////
        /////PUBLIC PROPERTIES/////
        ///////////////////////////
#region
        /// <summary>
        /// Property of the Temporary directory
        /// </summary>
        public String          TempDir      { get => m_tempDir;   set => m_tempDir   = value; }

        /// <summary>
        /// The SQL Database in use
        /// </summary>
        public DBHandler       SQLDatabase  { get => m_dbHandler; set => m_dbHandler = value; }

        /// <summary>
        /// The interface permitting to commit into the database. Point to MainWindow object
        /// </summary>
        public ICommitDatabase CommitDB     { get => m_commitDB;     }

        /// <summary>
        /// The interface permitting to link a name to another part of the UI (shortcuts). Point to MainWindow object
        /// For example, clicking on "Westcott Yulis" can point you to the character shit of Westcott Yulis
        /// </summary>
        public ILinkName       LinkName     { get => m_linkName;     }

        /// <summary>
        /// Interface permitting to update the UI when selecting part of the UI (places, characters, dates, etc.)
        /// Points to MainWindow object
        /// </summary>
        public ISelectedTree   SelectedTree { get => m_selectedTree; }

        /// <summary>
        /// Has the internal state of the data changed ? 
        /// CanSave is here to tell that the application can save the data because the data that the application contains is different than at the beginning
        /// </summary>
        public bool CanSave
        {
            get { return m_canSave; }
            set
            {
                m_canSave = value;
                OnPropertyChanged("CanSave");
            }
        }

        /// <summary>
        /// What is the current player selected ?
        /// </summary>
        public String CurrentCharacter
        {
            get => m_currentCharacter;
            set
            {
                m_currentCharacter = value;
                OnPropertyChanged("CurrentCharacter");
            }
        }

        /// <summary>
        /// The players data context
        /// </summary>
        public CharacterDataContext    CharacterDatas    { get => m_CharacterDatas; }

        /// <summary>
        /// The place data context
        /// </summary>
        public PlaceDataContext PlaceDatas { get => m_placeDatas; }

        /// <summary>
        /// The actual path of the database (ZIP file)
        /// Before it was a SQL path, hence the name
        /// </summary>
        public String SQLPath { get => m_sqlPath; set => m_sqlPath = value; }

        /// <summary>
        /// The New Resources to add at the next save command
        /// </summary>
        public Dictionary<String, DBResource> NewResources { get => m_newResources; }

        /// <summary>
        /// The Loaded resources
        /// </summary>
        public Dictionary<String, DBResource> LoadedResources { get => m_loadedResources; }
#endregion
    }
}