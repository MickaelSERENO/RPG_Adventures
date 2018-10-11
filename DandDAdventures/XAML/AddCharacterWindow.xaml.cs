using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DandDAdventures.XAML
{
    /// <summary>
    /// Represent a row of class in the Character Add Window (a.k.a class name and class level)
    /// </summary>
    public class ClassRow
    {
        /// <summary>
        /// The class name
        /// </summary>
        public String ClassName  { get; set; }

        /// <summary>
        /// The class level
        /// </summary>
        public int    ClassLevel { get; set; }

        public ClassRow() { }
    }

    /// <summary>
    /// The Playable Character caracteristics (Str, Con, etc.)
    /// </summary>
    public class CharacterCaracteristics
    {
        /// <summary>
        /// Strength
        /// </summary>
        public int Strength     { get; set; } = 10;

        /// <summary>
        /// Constitution
        /// </summary>
        public int Constitution { get; set; } = 10;

        /// <summary>
        /// Dexterity
        /// </summary>
        public int Dexterity    { get; set; } = 10;

        /// <summary>
        /// Intelligence
        /// </summary>
        public int Intelligence { get; set; } = 10;

        /// <summary>
        /// Wisdom
        /// </summary>
        public int Wisdom       { get; set; } = 10;

        /// <summary>
        /// Charisma
        /// </summary>
        public int Charisma     { get; set; } = 10;
    }

    /// <summary>
    /// Logique d'interaction pour AddCharacterWindow.xaml
    /// </summary>
    public partial class AddCharacterWindow : Window
    {
        ///////////////////////////////////
        ////////PROTECTED ATTRIBUTES///////
        ///////////////////////////////////
#region

        /// <summary>
        /// The application data
        /// </summary>
        protected WindowData m_wd;

        /// <summary>
        /// The character name field box
        /// </summary>
        protected TextBox    m_nameEntry;

        /// <summary>
        /// The player name field box
        /// </summary>
        protected TextBox    m_playerNameEntry;

        /// <summary>
        /// The character's gender
        /// </summary>
        protected TextBox    m_charaGender;

        /// <summary>
        /// The character's alignment
        /// </summary>
        protected TextBox    m_charaAlignment;

        /// <summary>
        /// The ComboxBox containing all the known class (Database)
        /// </summary>
        protected ComboBox   m_classCB;

        /// <summary>
        /// The ComboBox containing all the known race (Database)
        /// </summary>
        protected ComboBox   m_raceCB;

        /// <summary>
        /// The race tree view known in the database, a.k.a race hierarchy (super race)
        /// </summary>
        protected TreeView   m_raceTV;

        /// <summary>
        /// The story of the character entered
        /// </summary>
        protected TextBox    m_storyEntry;

        /// <summary>
        /// The Grid of the character class entered (multiple class available)
        /// </summary>
        protected DataGrid   m_classGrid;

        /// <summary>
        /// The data of this window
        /// </summary>
        protected CharacterDataWindow m_CharacterWindowData;

        /// <summary>
        /// The character image icon
        /// </summary>
        protected System.Windows.Controls.Image m_characterImage;
#endregion

        /// <summary>
        /// Constructor. Get and initialize all the Widget components and internal states
        /// </summary>
        /// <param name="wd">The application data</param>
        public AddCharacterWindow(WindowData wd)
        {
            m_CharacterWindowData   = new CharacterDataWindow(wd);
            this.DataContext = m_CharacterWindowData;

            InitializeComponent();

            m_wd = wd;

            //Get every Widgets
            m_nameEntry       = (TextBox)FindName("NameEntry");
            m_playerNameEntry = (TextBox)FindName("PlayerNameEntry");
            m_charaGender     = (TextBox)FindName("GenderEntry");
            m_charaAlignment  = (TextBox)FindName("AlignmentEntry");
            m_raceCB          = (ComboBox)FindName("RaceCB");
            m_raceTV          = (TreeView)FindName("RaceTV");
            m_storyEntry      = (TextBox)FindName("StoryEntry");
            m_classGrid       = (DataGrid)FindName("CharaDataGrid");
            m_characterImage  = (System.Windows.Controls.Image)FindName("CaraImageIcon");

            //Prompt the database
            var superRaceVal = wd?.SQLDatabase.GetSuperRaces();
            var raceVal      = wd?.SQLDatabase.GetRaces();

            //Fill the race thanks to the Database
            foreach(var sr in superRaceVal)
                m_raceTV.Items.Add(new TreeViewItem() { Header = sr.Name });

            foreach(var r in raceVal)
                foreach(TreeViewItem s in m_raceTV.Items)
                    if(s.Header.ToString() == r.SuperName)
                        s.Items.Add(new TreeViewItem() { Header = r.Name });
        }

        /// <summary>
        /// Find the first parent of the parameter matching the type T
        /// </summary>
        /// <typeparam name="T">The type of the nearest parent asked. Must inherite from DependencyObject (a.k.a Base WPF Class)</typeparam>
        /// <param name="obj">The object to look at</param>
        /// <returns>The parent of obj or null if not found</returns>
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

        ////////////////////////////////
        /////THE CALLBACK FUNCTIONS/////
        ////////////////////////////////
#region
        /// <summary>
        /// Function called when the Add Button is pressed. Add the Character into the database
        /// </summary>
        /// <param name="sender">The sender of the action (Button)</param>
        /// <param name="e">The RoutedEventArgs associated with this click</param>
        private void AddCharacterClick(object sender, RoutedEventArgs e)
        {
            //TODO show where is a problem
            if (m_CharacterWindowData.ClassRows.Count == 0 || m_raceCB.Text == "" || m_nameEntry.Text == "")
                return;

            var classVal = m_wd?.SQLDatabase.GetClasses();
            Class[] classArray = classVal.ToArray();

            //Look if the class entered has to be added into the database (not known class)
            foreach (var c1 in m_CharacterWindowData.ClassRows)
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
                //TODO multiple race hierarchy
            }

            //Look if we should add the super race and the race into the database
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

            String iconResKey = null;
            if(m_CharacterWindowData.IconDefined)
            {
                iconResKey = m_nameEntry.Text + "_Icon";
                m_wd.SQLDatabase.AddResource(iconResKey);
            }

            //Add the Character into the database
            m_CharacterWindowData.AddCharacter(new Character { Name = m_nameEntry.Text, PlayerName = m_playerNameEntry.Text, Sexe = m_charaGender.Text, 
                                                 Race = m_raceCB.Text.Split(new Char[] { '/' }).Last(), Alignment = m_charaAlignment.Text,
                                                 Story = StoryEntry.Text, Type = 0, Icon = iconResKey,
                                                 Str  = m_CharacterWindowData.Caracteristics.Strength,     Con  = m_CharacterWindowData.Caracteristics.Constitution, Dex  = m_CharacterWindowData.Caracteristics.Dexterity,
                                                 Int  = m_CharacterWindowData.Caracteristics.Intelligence, Wis  = m_CharacterWindowData.Caracteristics.Wisdom      , Cha  = m_CharacterWindowData.Caracteristics.Charisma
            });

            //Then Add the Character Classes (a.k.a association between a character and ALL its classes)
            m_CharacterWindowData.AddCharaRowsToDb(m_nameEntry.Text);
            m_characterImage.Source=null;

            //Close the window
            Close();
        }

        /// <summary>
        /// Function class when the Race ComboBox is closed. Changed the text displayed following what is selected
        /// </summary>
        /// <param name="sender">The sender of the action, a.k.a the ComboBox</param>
        /// <param name="e">The EventArgs associated with this action</param>
        private void RaceCBDropDownClosed(object sender, EventArgs e)
        {
            //At the drop down of the RaceCB, change the Text following what is selected in the TreeView
            var cb = (ComboBox)sender;
            if (m_raceTV.SelectedItem != null)
                cb.Text = ((TreeViewItem)(m_raceTV.SelectedItem)).Header.ToString();
            else
                cb.Text = "";
        }
                   
        /// <summary>
        /// Function called when the "+" Level button is pressed. Change the level of the associated class for this character
        /// </summary>
        /// <param name="sender">The sender of the action (Button +)</param>
        /// <param name="e">The RoutedEventArgs associated with this action</param>
        private void LevelUpClick(object sender, RoutedEventArgs e)
        {
            ClassRow row = (ClassRow)m_classGrid.SelectedItem;
            row.ClassLevel++;

            Button btn = (Button)sender;
            ((TextBox)(btn.FindName("LevelTxt"))).Text = row.ClassLevel.ToString();
        }

        /// <summary>
        /// Function called when the "-" Level button is pressed. Change the level of the associated class for this character
        /// </summary>
        /// <param name="sender">The sender of the action (Button -)</param>
        /// <param name="e">The RoutedEventArgs associated with this action</param>
        private void LevelDownClick(object sender, RoutedEventArgs e)
        {
            ClassRow row = (ClassRow)m_classGrid.SelectedItem;
            row.ClassLevel--;
            if(row.ClassLevel < 0)
                row.ClassLevel = 0;

            Button btn = (Button)sender;
            ((TextBox)(btn.FindName("LevelTxt"))).Text = row.ClassLevel.ToString();
        }

        /// <summary>
        /// Function called when the "+" button for a characteristics. The associated characteristics is defined by the Tag of the Button (sender).
        /// </summary>
        /// <param name="sender">The sender of the action (Button +)</param>
        /// <param name="e">The RoutedEventArgs associated with this action</param>
        private void CaraUpClick(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;

            switch(int.Parse((string)btn.Tag))
            {
                case 0:
                    m_CharacterWindowData.Caracteristics.Strength++;
                    break;
                case 1:
                    m_CharacterWindowData.Caracteristics.Constitution++;
                    break;
                case 2:
                    m_CharacterWindowData.Caracteristics.Dexterity++;
                    break;
                case 3:
                    m_CharacterWindowData.Caracteristics.Intelligence++;
                    break;
                case 4:
                    m_CharacterWindowData.Caracteristics.Wisdom++;
                    break;
                case 5:
                    m_CharacterWindowData.Caracteristics.Charisma++;
                    break;
            }
            
            m_CharacterWindowData.OnPropertyChanged("Caracteristics");
        }

        /// <summary>
        /// Function called when the "-" button for a characteristics. The associated characteristics is defined by the Tag of the Button (sender).
        /// </summary>
        /// <param name="sender">The sender of the action (Button -)</param>
        /// <param name="e">The RoutedEventArgs associated with this action</param>
        private void CaraDownClick(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;

            switch(int.Parse((string)btn.Tag))
            {
                case 0:
                    m_CharacterWindowData.Caracteristics.Strength     = (m_CharacterWindowData.Caracteristics.Strength < 1 ? 
                                                                  0 : m_CharacterWindowData.Caracteristics.Strength-1);
                    break;
                case 1:
                    m_CharacterWindowData.Caracteristics.Constitution = (m_CharacterWindowData.Caracteristics.Constitution < 1 ? 
                                                                  0 : m_CharacterWindowData.Caracteristics.Constitution-1);
                    break;
                case 2:
                    m_CharacterWindowData.Caracteristics.Dexterity    = (m_CharacterWindowData.Caracteristics.Dexterity < 1 ?
                                                                  0 : m_CharacterWindowData.Caracteristics.Dexterity-1);
                    break;
                case 3:
                    m_CharacterWindowData.Caracteristics.Intelligence = (m_CharacterWindowData.Caracteristics.Intelligence < 1 ? 
                                                                  0 : m_CharacterWindowData.Caracteristics.Intelligence-1);
                    break;
                case 4:
                    m_CharacterWindowData.Caracteristics.Wisdom       = (m_CharacterWindowData.Caracteristics.Wisdom < 1 ? 
                                                                  0 : m_CharacterWindowData.Caracteristics.Wisdom-1);
                    break;
                case 5:
                    m_CharacterWindowData.Caracteristics.Charisma     = (m_CharacterWindowData.Caracteristics.Charisma < 1 ? 
                                                                  0 : m_CharacterWindowData.Caracteristics.Charisma-1);
                    break;
            }

            m_CharacterWindowData.OnPropertyChanged("Caracteristics");
        }

        /// <summary>
        /// Function called when the text associated with the level is modified
        /// </summary>
        /// <param name="sender">The sender of the action (TextBox)</param>
        /// <param name="e">The TextChangedEventArgs associated with this action</param>
        private void LevelTxtTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox text = (TextBox)sender;
            if (!int.TryParse(text.Text, out int numValue))
                text.Text = numValue.ToString();
        }
        
        /// <summary>
        /// Function called when the class name has lost the focus. We add the text entered into the known class
        /// </summary>
        /// <param name="sender">The sender of the action (ComboBox)</param>
        /// <param name="e">The RoutedEventArgs associated with this action</param>
        private void ClassNameLostFocus(object sender, RoutedEventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            m_CharacterWindowData.AddClassList(cb.Text);
        }

        /// <summary>
        /// Function called when the Icon Image Button is clicked. Opens a Dialog to select a new icon
        /// </summary>
        /// <param name="sender">The sender of the action (Button)</param>
        /// <param name="e">The RoutedEventArgs associated with this action</param>
        private void ChangeCharacterIcon(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Title = "Sélectionner l'icon du personnage courant";
            if(openFile.ShowDialog() == true)
                Datas.IconPath = openFile.FileName;

            m_characterImage.Stretch = Stretch.Uniform;
        }
#endregion

        /// <summary>
        /// The Internal data associated with this Window accessible via the XAML
        /// </summary>
        public CharacterDataWindow Datas { get => m_CharacterWindowData; }
    }

    /// <summary>
    /// The internal data of the CharacterWindow
    /// </summary>
    public class CharacterDataWindow : INotifyPropertyChanged
    {
        ////////////////////////
        ///PRIVATE ATTRIBUTES///  
        ////////////////////////
#region
        /// <summary>
        /// Event object permitting to notify when a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The Application data structure
        /// </summary>
        protected WindowData       m_wd;

        /// <summary>
        /// List of class in creation (see ClassRow)
        /// </summary>
        protected List<ClassRow>   m_classRow;

        /// <summary>
        /// List of known classes
        /// </summary>
        protected List<String>     m_classList;

        /// <summary>
        /// List characters added. For now, this list is empty or contain only one entry
        /// </summary>
        protected List<Character>  m_characters;

        /// <summary>
        /// Caracteristics of the character (see CharacterCaracteristics)
        /// </summary>
        protected CharacterCaracteristics m_caracteristics;

        /// <summary>
        /// The path of the in-progress character's icon
        /// </summary>
        protected String           m_iconPath;

        /// <summary>
        /// Have we changed the icon ?
        /// </summary>
        protected bool             m_iconDefined;

        /// <summary>
        /// Is the character added ?
        /// </summary>
        protected bool             m_characterAdded;
#endregion

        /// <summary>
        /// The Constructor. Initialize the object based on the Application Data (WindowData)
        /// </summary>
        /// <param name="wd">The Application Data</param>
        public CharacterDataWindow(WindowData wd)
        {
            m_wd = wd;
            m_characters     = new List<Character>();
            m_classRow       = new List<ClassRow>();
            m_classList      = new List<String>();
            m_caracteristics = new CharacterCaracteristics();

            //Initialize the Icon values
            m_iconPath    = System.AppDomain.CurrentDomain.BaseDirectory+"/Resources/DefaultCaracterIcon.png";
            m_iconDefined = false;

            //Fill the list of class known
            var classVal = wd?.SQLDatabase.GetClasses();
            foreach (var c in classVal)
                m_classList.Add(c.Name);

            //Notify that we have filled the known classes
            OnPropertyChanged("DataContext.ClassRows");
        }

        //////////////////////////
        /////PUBLIC FUNCTIONS/////
        //////////////////////////
#region

        /// <summary>
        /// Get the Default Icon path used in the application
        /// </summary>
        /// <returns></returns>
        public String GetDefaultIconPath()
        {
            return System.AppDomain.CurrentDomain.BaseDirectory+"/Resources/DefaultCaracterIcon.png";
        }

        /// <summary>
        /// Add a Character into the Database.
        /// </summary>
        /// <param name="chara">The character to add</param>
        public void AddCharacter(Character chara)
        {
            m_wd.SQLDatabase?.AddCharacter(chara);
            m_characters.Add(chara);
            m_characterAdded = true;
        }

        /// <summary>
        /// Add the character class rows into the database. Using CharaClass SQL Table 
        /// </summary>
        /// <param name="charaName">The name of the character</param>
        public void AddCharaRowsToDb(String charaName)
        {
            foreach(var d in m_classRow)
            {
                CharaClass cc = new CharaClass{ CharaName = charaName, ClassName = d.ClassName, Level = d.ClassLevel};
                m_wd?.SQLDatabase.AddCharaClass(cc);
            }
        }

        /// <summary>
        /// Add the Class 's' into the SQL Database if it does not yet exist
        /// </summary>
        /// <param name="s">The class to add</param>
        public void AddClassList(String s)
        {
            if (s == "")
                return;

            //Look if we can find the class in the database. If found, exit
            foreach(String cl in ClassList)
                if (cl == s)
                {
                    return;
                }
            ClassList.Add(s);
            OnPropertyChanged("ClassList");
        }

        /// <summary>
        /// Function used to notify that a property (View property) has changed
        /// </summary>
        /// <param name="name">The property name</param>
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
#endregion

        ////////////////////////
        ////PUBLIC PROPERTIES///
        ////////////////////////
#region

        /// <summary>
        /// Is the character added into the Database ?
        /// </summary>
        public Boolean         CharacterAdded { get => m_characterAdded; }

        /// <summary>
        /// The list of the newly created character. For now, this list is empty or contain only one entry
        /// </summary>
        public List<Character> NewCharacters  { get => m_characters; }

        /// <summary>
        /// The class rows for the in construction character
        /// </summary>
        public List<ClassRow>  ClassRows      { get => m_classRow;       set { m_classRow = value; OnPropertyChanged("ClassRows"); } }

        /// <summary>
        /// The character's caracteristics
        /// </summary>
        public CharacterCaracteristics Caracteristics { get => m_caracteristics; set { m_caracteristics = value; OnPropertyChanged("Caracteristics"); } }

        /// <summary>
        /// The ClassList available
        /// </summary>
        public List<String>            ClassList      { get => m_classList;      set { m_classList      = value; OnPropertyChanged("ClassList"); } }

        /// <summary>
        /// Is the Icon defined ?
        /// </summary>
        public Boolean                 IconDefined    { get => m_iconDefined; }   

        /// <summary>
        /// The IconPath associated with the in construction character
        /// </summary>
        public String                  IconPath       { get => m_iconPath;       set { m_iconPath       = value; OnPropertyChanged("IconPath"); m_iconDefined = true; } }

        /// <summary>
        /// The Places known by the application. Quick access to another List in the application
        /// </summary>
        public ObservableCollection<Place> Places     { get => m_wd.PlaceDatas.PlaceList; }
#endregion
    }
}
