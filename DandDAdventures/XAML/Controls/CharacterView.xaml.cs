using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace DandDAdventures.XAML.Controls
{  
    /// <summary>
    /// GroupEvent class for characters. Represent an event applied to a set of characters
    /// </summary>
    public class CharacterGroupEvent
    {
        /// <summary>
        /// ID of the Event displayed (application ID)
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The GroupEventID (Database ID)
        /// </summary>
        public int    GroupEventID { get; set; }

        /// <summary>
        /// Description of the Event
        /// </summary>
        public String Description  { get; set; }

        /// <summary>
        /// The Characters of this event (String form)
        /// </summary>
        public String CharaList    { get; set; }
    }

    /// <summary>
    /// CharacterView.xaml Logique. Controls the Character view
    /// </summary>
    public partial class CharacterView : UserControl
    {
        /// <summary>
        /// The summary text field
        /// </summary>
        private TextBlock m_summaryVal;

        /// <summary>
        /// The Main application data
        /// </summary>
        private WindowData m_wd;

        /// <summary>
        /// Constructor. Initialize the CharacterView.
        /// </summary>
        /// <param name="wd">The application data</param>
        public CharacterView(WindowData wd)
        {
            m_wd = wd;
            InitializeComponent();

            m_summaryVal = (TextBlock)FindName("SummaryVal");
        }

        public void LoadContent(WindowData wd)
        {
        }

        /// <summary>
        /// Add a groupevent in the character view
        /// </summary>
        /// <param name="ge">The groupevent to add</param>
        public void AddGroupEvent(GroupEvent ge)
        {
            m_wd.CharacterDatas.AddGroupEvent(ge);
        }

        /// <summary>
        /// Set the current displayed groupEvent
        /// </summary>
        /// <param name="Name"></param>
        public void SetGroupEvent(String Name)
        {
            m_wd.CharacterDatas.EventList.Clear();
            var list = m_wd.SQLDatabase.GetGroupEvents(Name);

            foreach (var ge in list)
                m_wd.CharacterDatas.AddGroupEvent(ge);
        }

        /// <summary>
        /// Set the summary displayed. The String will be altered in order to create links to others pages
        /// </summary>
        /// <param name="s">The summary string</param>
        public void SetSummary(String s)
        {
            String[] listName = m_wd.SQLDatabase.GetListName();
            Utils.SetTextLink(m_summaryVal, s, listName, (Style)FindResource("LinkStyle"), this.LinkDown);
        }

        /// <summary>
        /// Set the displayed character icon
        /// </summary>
        /// <param name="icon">The icon to display</param>
        public void SetCharacterIcon(BitmapImage icon)
        {
            m_wd.CharacterDatas.CharacterIcon = icon;
        }

        /// <summary>
        /// Function called when a place or a character has been clicked on
        /// </summary>
        /// <param name="sender">The object calling this function</param>
        /// <param name="e"></param>
        private void LinkDown(object sender, MouseButtonEventArgs e)
        {
            Run r = sender as Run;
            m_wd.LinkName?.LinkToName(r.Text);
        }

        private void EventMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            CharacterGroupEvent ev = ((ListViewItem)sender).Content as CharacterGroupEvent;
            CharacterGroupEvent edEvent = new CharacterGroupEvent { ID = ev.ID, GroupEventID = ev.GroupEventID, Description = ev.Description.Clone() as String, CharaList = ev.CharaList.Clone() as String };

            EditDate ed = new EditDate(m_wd, edEvent);
            ed.ShowDialog();

            if (((EditDateDatas)ed.DataContext).Valid)
            {
                TextBlock tb = FindName("EventDesc") as TextBlock;

                tb.Inlines.Clear();

                ev.Description = edEvent.Description;

                m_wd.CharacterDatas.CurrentDate = null;
                m_wd.CharacterDatas.CurrentDate = ev;
            }
        }

        private void ListEventsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextBlock tb = FindName("EventDesc") as TextBlock;

            if (m_wd.CharacterDatas.CurrentDate != null)
            {
                String[] listName = m_wd.SQLDatabase.GetListName();

                Utils.SetTextLink(tb, m_wd.CharacterDatas.CurrentDate.Description, listName, (Style)FindResource("LinkStyle"), this.LinkDown);
            }
            else
                tb.Inlines.Clear();
        }
    }

    public class CharacterDataContext : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected ObservableCollection<CharacterGroupEvent> m_groupEvent;
        protected ObservableCollection<Character> m_listCharacters;
        protected Character m_characterSelected = null;
        protected CharacterGroupEvent m_currentDate = null;

        private WindowData  m_wd;
        private BitmapImage m_characterIcon = null;

        public CharacterDataContext(WindowData wd)
        {
            m_wd = wd;
            m_characterIcon  = StringToImageConverter.BitmapFromPath(System.AppDomain.CurrentDomain.BaseDirectory+"/Resources/DefaultCaracterIcon.png");
            m_groupEvent     = new ObservableCollection<CharacterGroupEvent>();
            m_listCharacters = new ObservableCollection<Character>();
        }

        public void AddGroupEvent(GroupEvent ge)
        {
            String charaList = String.Join(", ", m_wd.SQLDatabase.GetCharaNamesEvent(ge).ToArray());
            m_groupEvent.Add(new CharacterGroupEvent { ID = m_groupEvent.Count+1, Description = ge.Description, GroupEventID = ge.ID, CharaList = charaList });
        }

        public void Clean()
        {
            EventList.Clear();
            CharacterList.Clear();
            CharacterSelected = null;
        }

        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public ObservableCollection<CharacterGroupEvent> EventList  { get => m_groupEvent; set { m_groupEvent = value; } }
        public ObservableCollection<Character> CharacterList { get => m_listCharacters; set { m_listCharacters = value; } }
        public CharacterGroupEvent CurrentDate { get => m_currentDate; set { m_currentDate = value; OnPropertyChanged("CurrentDate"); } }

        public Character CharacterSelected
        {
            get
            {
                return m_characterSelected;
            }
            set
            {
                m_characterSelected = value;
                OnPropertyChanged("CharacterSelected");
            }
        }

        public BitmapImage CharacterIcon { get => m_characterIcon; set { m_characterIcon = value; OnPropertyChanged("CharacterIcon"); } }
    }
}