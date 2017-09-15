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

namespace DandDAdventures.XAML
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, ICommitDatabase, ISelectedTree, ILinkName
    {
        protected WindowData     m_windowData;
        protected Grid           m_mainPanel;

        //ContentControls 
        protected ContentControl m_mainControl;
        protected ContentControl m_pjTabControl;
        protected ContentControl m_pnjTabControl;
        protected ContentControl m_placeTabControl;

        //The Main Views
        protected PJView         m_pjView;
        protected PlaceView      m_placeView;

        //The TabItem Views
        protected PJTabItemCtrl    m_pjTabItem;
        protected PlaceTabItemCtrl m_placeTabItem;

        //The AddListeners
        protected AddPJListener    m_addPJListener;
        protected AddPlaceListener m_addPlaceListener;

        public MainWindow()
        {
            InitializeComponent();
            m_windowData = new WindowData(null, this, this, this);
            this.DataContext = m_windowData;

            //Initialize the MainPanel
            m_mainPanel = (Grid)this.FindName("MainPanel");
            m_mainPanel.Visibility = Visibility.Hidden;

            //Get and initialize the values for the MainControl View
            m_pjView      = new PJView(m_windowData);
            m_placeView   = new PlaceView(m_windowData);
            m_mainControl = (ContentControl)this.FindName("MainControl");

            //Initialize the Listeners
            m_addPJListener    = new AddPJListener(m_windowData);
            m_addPlaceListener = new AddPlaceListener(m_windowData);

            //Get and initialize the TabControl 
            m_pjTabControl    = (ContentControl)this.FindName("PJTabItem");
            m_pnjTabControl   = (ContentControl)this.FindName("PNJTabItem");
            m_placeTabControl = (ContentControl)this.FindName("PlaceTabItem");

            m_pjTabItem       = new PJTabItemCtrl(m_windowData);
            m_placeTabItem    = new PlaceTabItemCtrl(m_windowData);

            m_pjTabControl.Content    = m_pjTabItem;
            m_placeTabControl.Content = m_placeTabItem;

            m_pjTabItem.SetAddListener(m_addPJListener);
            m_placeTabItem.SetAddListener(m_addPlaceListener);

            //Launch the Window
            SetToPJMainView();
        }

        //Set the Main View
        private void SetToPJMainView()
        {
            m_mainControl.Content = m_pjView;
            TabControl tabCtrl = FindName("TabCtrl") as TabControl;
            tabCtrl.SelectedIndex = ((TabItem)(m_pjTabControl.Parent)).TabIndex;
        }

        private void SetToPlaceMainView()
        {
            m_mainControl.Content = m_placeView;
            TabControl tabCtrl = FindName("TabCtrl") as TabControl;
            tabCtrl.SelectedIndex = ((TabItem)(m_placeTabControl.Parent)).TabIndex;
        }

        //Simple menu commands
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

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            m_windowData.Clean();

            OpenFileDialog openFile = new OpenFileDialog();
            if (openFile.ShowDialog() == true)
            {
                //Open and load the Database
                if (m_windowData.SQLDatabase == null)
                    m_windowData.SQLDatabase = new DBHandler(String.Format("DataSource = {0}", openFile.FileName.Replace('\\', '/')));
                else
                {
                    var sql = new SQLiteConnection(String.Format("DataSource = {0}", openFile.FileName.Replace('\\', '/')));
                    sql.Open();
                    m_windowData.SQLDatabase.ChangeSQLiteConnection(sql);
                }

                //Change to the memory
                m_windowData.SQLDatabase.ChangeSQLiteConnection(m_windowData.SQLDatabase.Backup(":memory:"));

                //Load the content in the FrontEnd
                m_pjTabItem.LoadContent(m_windowData);
                m_pjView.LoadContent(m_windowData);

                m_placeTabItem.LoadContent(m_windowData);
                m_placeView.LoadContent(m_windowData);

                m_windowData.SQLPath = openFile.FileName;
                m_windowData.CanSave = true;
                m_mainPanel.Visibility = Visibility.Visible;
            }
        }

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
                //Delete the file if needed
                if (File.Exists(m_windowData.SQLPath))
                    File.Delete(m_windowData.SQLPath);

                m_windowData.SQLDatabase.Commit();
                m_windowData.SQLDatabase.Backup(m_windowData.SQLPath).Close();
            }
        }

        private void SetPJ(object sender, RoutedEventArgs e)
        {
            
        }

        private void SetPlace(object sender, RoutedEventArgs e)
        {

        }

        //Interfaces implementations
        //Implementation of ICommitDatabase
        public void AddPJ(Character[] charas)
        {
            foreach(var chara in charas)
                m_windowData.PJDatas.CharacterList.Add(chara);
        }

        public void AddPlace(Place p)
        {
            m_windowData.PlaceDatas.PlaceList.Add(p);
        }
        
        public void OnSelectPJ(Character[] chara)
        {
            if(chara.Length == 1)
            {
                m_pjView.SetSummary(chara[0].Story);
                m_pjView.SetGroupEvent(chara[0].Name);

                m_windowData.CurrentPJ = chara[0].Name;
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
            }
        }

        public void AddDate(CreateDate cd, Character[] chara)
        {
            GroupEvent ge = m_windowData.SQLDatabase.AddDate(cd.Description, chara);
            m_pjView.AddGroupEvent(ge);

        }

        private void PJTabItemSelected(object sender, RoutedEventArgs e)
        {
            SetToPJMainView();
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
                foreach(Character c in m_windowData.PJDatas.CharacterList)
                {
                    if(c.Name == name)
                    {
                        m_windowData.PJDatas.CharacterSelected = c;
                        break;
                    }
                }
                SetToPJMainView();
            }
        }
    }

    public class WindowData : INotifyPropertyChanged
    {
        public bool   m_canSave   = false;
        public String m_sqlPath   = null;
        public String m_currentPJ = "";

        protected DBHandler       m_dbHandler;
        protected ICommitDatabase m_commitDB;
        protected ILinkName       m_linkName;
        protected ISelectedTree   m_selectedTree;

        //DataContexts
        protected PJDataContext    m_pjDatas;
        protected PlaceDataContext m_placeDatas;

        public event PropertyChangedEventHandler PropertyChanged;

        public WindowData(DBHandler sqlDatas, ICommitDatabase commitDB, ILinkName linkName, ISelectedTree selectedTree)
        {
            m_dbHandler    = sqlDatas;
            m_commitDB     = commitDB;
            m_linkName     = linkName;
            m_selectedTree = selectedTree;
            m_pjDatas    = new PJDataContext(this);
            m_placeDatas = new PlaceDataContext(this);
        }

        public void Clean()
        {
            CanSave = false;
            CurrentPJ = null;
            m_sqlPath = null;

            m_pjDatas.Clean();
            m_placeDatas.Clean();
        }

        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public DBHandler SQLDatabase { get => m_dbHandler; set => m_dbHandler = value; }
        public ICommitDatabase CommitDB { get => m_commitDB; }
        public ILinkName LinkName { get => m_linkName; }
        public ISelectedTree SelectedTree { get => m_selectedTree; }

        public bool CanSave
        {
            get { return m_canSave; }
            set
            {
                m_canSave = value;
                OnPropertyChanged("CanSave");
            }
        }

        public String CurrentPJ
        {
            get => m_currentPJ;
            set
            {
                m_currentPJ = value;
                OnPropertyChanged("CurrentPJ");
            }
        }

        public PJDataContext    PJDatas    { get => m_pjDatas; }
        public PlaceDataContext PlaceDatas { get => m_placeDatas; }

        public String SQLPath { get => m_sqlPath; set => m_sqlPath = value; }
    }
}
