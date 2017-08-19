using System;
using System.Collections.Generic;
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
        protected List<Character> m_characters;
        protected List<PJ>   m_pjs;
        protected bool       m_characterAdded;

        public AddPJWindow(WindowData wd)
        {
            InitializeComponent();
            m_wd = wd;
            m_characters = new List<Character>();
            m_pjs        = new List<PJ>();

            //Get every Widgets
            m_nameEntry = (TextBox)FindName("NameEntry");
            m_classCB   = (ComboBox)FindName("ClassCB");
            m_raceCB    = (ComboBox)FindName("RaceCB");
            m_raceTV    = (TreeView)FindName("RaceTV");
            m_storyEntry = (TextBox)FindName("StoryEntry");

            var superRaceVal = wd?.SQLDatabase.GetSuperRaces();
            var raceVal      = wd?.SQLDatabase.GetRaces();
            var classVal     = wd?.SQLDatabase.GetClasses();

            //Fill the Items thanks to the Database
            foreach(var sr in superRaceVal)
                m_raceTV.Items.Add(new TreeViewItem() { Header = sr.Name });

            foreach(var r in raceVal)
                foreach(TreeViewItem s in m_raceTV.Items)
                    if(s.Header.ToString() == r.SuperName)
                        s.Items.Add(new TreeViewItem() { Header = r.Name });

            foreach (var c in classVal)
                m_classCB.Items.Add(c.Name);
        }

        private void AddPJClick(object sender, RoutedEventArgs e)
        {
            //TODO show where is a problem
            if (m_classCB.Text == "" || m_raceCB.Text == "" || m_nameEntry.Text == "")
                return;

            //Add the Class
            bool newClass = true;
            foreach (String c in m_classCB.Items)
                if (c == m_classCB.Text)
                    newClass = false;

            if (newClass)
                m_wd.SQLDatabase.AddClass(m_classCB.Text);

            //Add the Race if does not exist
            bool newRace = true;
            foreach(TreeViewItem sr in m_raceTV.Items)
            {
                if (sr.Header.ToString() != m_raceCB.Text)
                    newRace = false;
                foreach(TreeViewItem r in sr.Items)
                    if (r.Header.ToString() != m_raceCB.Text)
                        newRace = false;
            }

            if(newRace)
                m_wd.SQLDatabase.AddRace(m_raceCB.Text);

            //Add the Character
            m_characters.Add(new Character { Name = m_nameEntry.Text, Class = m_classCB.Text, Race = m_raceCB.Text.Split(new Char[] { '/' }).Last(), Story = StoryEntry.Text });
            m_pjs.Add(new PJ { Name = m_nameEntry.Text, Level = 0 });
            m_wd.SQLDatabase.AddPJ(m_characters.Last(), m_pjs.Last());

            m_characterAdded = true;

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

        public Boolean CharacterAdded { get => m_characterAdded; }
        public List<Character> NewCharacters { get => m_characters; }
        public List<PJ> NewPJs { get => m_pjs; }
    }
}
