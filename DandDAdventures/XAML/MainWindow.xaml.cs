using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using DandDAdventures.XAML.Controls;
using DandDAdventures.XAML.Listeners;
using Microsoft.Win32;
using System;
using System.IO;
using System.Data.SQLite;

namespace DandDAdventures.XAML
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, ICommitDatabase
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
        protected PJTabItemCtrl m_pjTabItem;
        protected PJTabItemCtrl m_pnjTabItem;
        protected PJTabItemCtrl m_placeTabItem;

        //The AddListeners
        protected AddPJListener    m_addPJListener;
        protected AddPNJListener   m_addPNJListener;
        protected AddPlaceListener m_addPlaceListener;

        public MainWindow()
        {
            InitializeComponent();
            m_windowData = new WindowData(null, this);
            this.DataContext = m_windowData;

            //Initialize the MainPanel
            m_mainPanel = (Grid)this.FindName("MainPanel");
            m_mainPanel.Visibility = Visibility.Hidden;

            //Get and initialize the values for the MainControl View
            m_pjView      = new PJView();
            m_placeView   = new PlaceView();
            m_mainControl = (ContentControl)this.FindName("MainControl");

            //Initialize the Listeners
            m_addPJListener    = new AddPJListener(m_windowData);
            m_addPNJListener   = new AddPNJListener(m_windowData);
            m_addPlaceListener = new AddPlaceListener(m_windowData);

            //Get and initialize the TabControl 
            m_pjTabControl    = (ContentControl)this.FindName("PJTabItem");
            m_pnjTabControl   = (ContentControl)this.FindName("PNJTabItem");
            m_placeTabControl = (ContentControl)this.FindName("PlaceTabItem");

            m_pjTabItem       = new PJTabItemCtrl();
            m_pnjTabItem      = new PJTabItemCtrl();
            m_placeTabItem    = new PJTabItemCtrl();

            m_pjTabControl.Content    = m_pjTabItem;
            m_pnjTabControl.Content   = m_pnjTabItem;
            m_placeTabControl.Content = m_placeTabItem;

            m_pjTabItem.SetAddListener(m_addPJListener);
            m_pnjTabItem.SetAddListener(m_addPNJListener);
            m_placeTabItem.SetAddListener(m_addPlaceListener);

            //Launch the Window
            SetToPJMainView();
        }

        //Set the Main View
        private void SetToPJMainView()
        {
            m_mainControl.Content = m_pjView;
        }

        private void SetToPlaceMainView()
        {
            m_mainControl.Content = m_placeView;
        }

        //Simple menu commands
        private void NewFile(object sender, RoutedEventArgs e)
        {
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
            OpenFileDialog openFile = new OpenFileDialog();
            if (openFile.ShowDialog() == true)
            {
                //Open and load the Database
                if (m_windowData.SQLDatabase == null)
                    m_windowData.SQLDatabase = new DBHandler(String.Format("DataSource = {0}", openFile.FileName.Replace('\\', '/')));
                else
                {
                    var sql = new SQLiteConnection(openFile.FileName);
                    sql.Open();
                    m_windowData.SQLDatabase.ChangeSQLiteConnection(sql);
                }

                //Change to the memory
                m_windowData.SQLDatabase.ChangeSQLiteConnection(m_windowData.SQLDatabase.Backup(":memory:"));
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

        private void SetPNJ(object sender, RoutedEventArgs e)
        {

        }

        private void SetPlace(object sender, RoutedEventArgs e)
        {

        }

        public void AddPJ(Character[] charas, PJ[] pjs)
        {
            TreeView tr = (TreeView)m_pjTabItem.FindName("MainTreeView");

            foreach (var c in charas)
                tr.Items.Add(c);
        }

        public void AddPlace()
        {
            throw new NotImplementedException();
        }

        public void AddPNJ()
        {
            throw new NotImplementedException();
        }
    }

    public class WindowData : INotifyPropertyChanged
    {
        public bool m_canSave = false;
        public String m_sqlPath = null;

        protected DBHandler m_dbHandler;
        protected ICommitDatabase m_commitDB;

        public event PropertyChangedEventHandler PropertyChanged;

        public WindowData(DBHandler sqlDatas, ICommitDatabase commitDB)
        {
            m_dbHandler = sqlDatas;
            m_commitDB = commitDB;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public DBHandler SQLDatabase { get => m_dbHandler; set => m_dbHandler = value; }
        public ICommitDatabase CommitDB { get => m_commitDB; }

        public bool CanSave
        {
            get { return m_canSave; }
            set
            {
                m_canSave = value;
                OnPropertyChanged("CanSave");
            }
        }

        public String SQLPath { get => m_sqlPath; set => m_sqlPath = value; }
    }
}
