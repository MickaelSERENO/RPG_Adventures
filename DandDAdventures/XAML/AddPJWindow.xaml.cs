using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DandDAdventures.XAML
{
    public class ClassRow
    {
        public String ClassName { get; set; }
        public int ClassLevel { get; set; }

        public ClassRow() { }
    }

    /// <summary>
    /// Logique d'interaction pour AddPJWindow.xaml
    /// </summary>
    public partial class AddPJWindow : Window
    {
        protected WindowData m_wd;
        protected TextBox    m_nameEntry;
        protected ComboBox   m_classCB;
        protected ComboBox   m_raceCB;
        protected TreeView   m_raceTV;
        protected TextBox    m_storyEntry;
        protected DataGrid   m_dataGrid;

        protected PJDataWindow m_pjWindowData;

        public AddPJWindow(WindowData wd)
        {
            m_pjWindowData   = new PJDataWindow(wd);
            this.DataContext = m_pjWindowData;

            InitializeComponent();

            m_wd = wd;

            //Get every Widgets
            m_nameEntry  = (TextBox)FindName("NameEntry");
            m_raceCB     = (ComboBox)FindName("RaceCB");
            m_raceTV     = (TreeView)FindName("RaceTV");
            m_storyEntry = (TextBox)FindName("StoryEntry");
            m_dataGrid   = (DataGrid)FindName("CharaDataGrid");

            var superRaceVal = wd?.SQLDatabase.GetSuperRaces();
            var raceVal      = wd?.SQLDatabase.GetRaces();

            //Fill the Items thanks to the Database
            foreach(var sr in superRaceVal)
                m_raceTV.Items.Add(new TreeViewItem() { Header = sr.Name });

            foreach(var r in raceVal)
                foreach(TreeViewItem s in m_raceTV.Items)
                    if(s.Header.ToString() == r.SuperName)
                        s.Items.Add(new TreeViewItem() { Header = r.Name });
        }

        public T FindParent<T>(DependencyObject obj) where T : DependencyObject
        {
            if (obj == null)
                return null;
            T parent = VisualTreeHelper.GetParent(obj) as T;
            if (parent != null)
                return parent;
            else
                return FindParent<T>(VisualTreeHelper.GetParent(obj));
        }

        private void AddPJClick(object sender, RoutedEventArgs e)
        {
            //TODO show where is a problem
            if (m_pjWindowData.ClassRows.Count == 0 || m_raceCB.Text == "" || m_nameEntry.Text == "")
                return;

            var classVal = m_wd?.SQLDatabase.GetClasses();
            Class[] classArray = classVal.ToArray();

            foreach (var c1 in m_pjWindowData.ClassRows)
            {
                bool add = true;
                foreach (var c2 in classArray)
                    if (c1.ClassName == c2.Name)
                    {
                        add = false;
                        break;
                    }
                if(add)
                    m_wd.SQLDatabase.AddClass(c1.ClassName);
            }

            //Add the Race if does not exist
            String[] raceSplitted = m_raceCB.Text.Split(new Char[] { '/' });
            if(raceSplitted.Length > 2 || m_raceCB.Text == "")
            {
                //TODO
            }

            bool newSuperRace = true;
            bool newRace      = true;
            foreach(TreeViewItem sr in m_raceTV.Items)
            {
                if (sr.Header.ToString() == raceSplitted[0])
                    newSuperRace = false;

                if(raceSplitted.Length > 1)
                foreach(TreeViewItem r in sr.Items)
                    if (r.Header.ToString() == raceSplitted[1])
                        newRace = false;
            }

            if (newSuperRace)
                m_wd.SQLDatabase.AddSuperRace(raceSplitted[0]);

            if (newRace && raceSplitted.Length > 1)
                m_wd.SQLDatabase.AddRace(raceSplitted[0], raceSplitted[1]);
   
            //Add the Character
            m_pjWindowData.AddPJ(new Character { Name = m_nameEntry.Text, Race = m_raceCB.Text.Split(new Char[] { '/' }).Last(), Story = StoryEntry.Text }, new PJ { Name = m_nameEntry.Text, Level = 0 });

            //Then Add the Character Classes
            m_pjWindowData.AddCharaRowsToDb(m_nameEntry.Text);

            Close();
        }

        private void RaceCBDropDownClosed(object sender, EventArgs e)
        {
            //At the drop down of the RaceCB, change the Text following what is selected in the TreeView
            var cb = (ComboBox)sender;
            if (m_raceTV.SelectedItem != null)
                cb.Text = ((TreeViewItem)(m_raceTV.SelectedItem)).Header.ToString();
            else
                cb.Text = "";
        }
                   
        private void LevelUpClick(object sender, RoutedEventArgs e)
        {
            ClassRow row = (ClassRow)m_dataGrid.SelectedItem;
            row.ClassLevel++;

            Button btn = (Button)sender;
            ((TextBox)(btn.FindName("LevelTxt"))).Text = row.ClassLevel.ToString();
        }

        private void LevelDownClick(object sender, RoutedEventArgs e)
        {
            ClassRow row = (ClassRow)m_dataGrid.SelectedItem;
            row.ClassLevel--;

            Button btn = (Button)sender;
            ((TextBox)(btn.FindName("LevelTxt"))).Text = row.ClassLevel.ToString();
        }

        private void LevelTxtTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox text = (TextBox)sender;
            if (!int.TryParse(text.Text, out int numValue))
                text.Text = numValue.ToString();
        }

        public PJDataWindow Datas { get => m_pjWindowData; }

        private void ClassNameLostFocus(object sender, RoutedEventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            m_pjWindowData.AddClassList(cb.Text);
        }
    }

    public class PJDataWindow : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected WindowData m_wd;
        protected List<ClassRow>  m_classRow;
        protected List<String>    m_classList;
        protected List<Character> m_characters;
        protected List<PJ>        m_pjs;
        protected bool            m_characterAdded;

        public PJDataWindow(WindowData wd)
        {
            m_wd = wd;
            m_characters = new List<Character>();
            m_pjs        = new List<PJ>();
            m_classRow   = new List<ClassRow>();
            m_classList  = new List<String>();

            //Fill the class List
            var classVal = wd?.SQLDatabase.GetClasses();
            foreach (var c in classVal)
                m_classList.Add(c.Name);

            OnPropertyChanged("DataContext.ClassRows");
        }

        public void AddPJ(Character chara, PJ pj)
        {
            m_wd.SQLDatabase?.AddPJ(chara, pj);
            m_wd.SQLDatabase.Commit();
            m_characters.Add(chara);
            m_pjs.Add(pj);
            m_characterAdded = true;
        }

        public void AddCharaRowsToDb(String charaName)
        {
            foreach(var d in m_classRow)
            {
                CharaClass cc = new CharaClass{ CharaName = charaName, ClassName = d.ClassName, Level = d.ClassLevel};
                m_wd?.SQLDatabase.AddCharaClass(cc);
            }
        }

        public void AddClassList(String s)
        {
            if (s == "")
                return;

            bool find = false;
            foreach (String cl in ClassList)
                if (cl == s)
                {
                    find = true;
                }
            if(!find)
            {
                ClassList.Add(s);
                OnPropertyChanged("ClassList");
            }

        }

        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public Boolean         CharacterAdded { get => m_characterAdded; }
        public List<Character> NewCharacters { get => m_characters; }
        public List<PJ>        NewPJs { get => m_pjs; }
        public List<ClassRow>  ClassRows
        {
            get
            {
                return m_classRow;
            }

            set
            {
                m_classRow = value;
                OnPropertyChanged("ClassRows");
            }
        }

        public List<String> ClassList { get => m_classList; set { m_classList = value; OnPropertyChanged("ClassList"); } }
    }
}
