using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DandDAdventures.XAML.Controls
{  
    /// <summary>
    /// GroupEvent class for characters. Represent an event applied to a set of characters
    /// </summary>
    public class PJGroupEvent
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
    /// PJView.xaml Logique. Controls the Character view
    /// </summary>
    public partial class PJView : UserControl
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
        /// Constructor. Initialize the PJView.
        /// </summary>
        /// <param name="wd">The application data</param>
        public PJView(WindowData wd)
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
            m_wd.PJDatas.AddGroupEvent(ge);
        }

        public void SetGroupEvent(String Name)
        {
            m_wd.PJDatas.EventList.Clear();
            var list = m_wd.SQLDatabase.GetGroupEvents(Name);

            foreach (var ge in list)
                m_wd.PJDatas.AddGroupEvent(ge);
        }

        /// <summary>
        /// Set the summary displayed. The String will be altered in order to create links to others pages
        /// </summary>
        /// <param name="s">The summary string</param>
        public void SetSummary(String s)
        {
            String[] listName = m_wd.SQLDatabase.GetListName();
            Utils.SetTextLink(m_summaryVal, s, listName, (Style)FindResource("LinkStyle"), this.PlaceDown);
        }

        public void SetCharacterIcon(BitmapImage icon)
        {
            m_wd.PJDatas.CharacterIcon = icon;
        }

        private void PlaceDown(object sender, MouseButtonEventArgs e)
        {
            Run r = sender as Run;
            m_wd.LinkName?.LinkToName(r.Text);
        }

        private void EventMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            PJGroupEvent ev = ((ListViewItem)sender).Content as PJGroupEvent;
            PJGroupEvent edEvent = new PJGroupEvent { ID = ev.ID, GroupEventID = ev.GroupEventID, Description = ev.Description.Clone() as String, CharaList = ev.CharaList.Clone() as String };

            EditDate ed = new EditDate(m_wd, edEvent);
            ed.ShowDialog();

            if (((EditDateDatas)ed.DataContext).Valid)
            {
                TextBlock tb = FindName("EventDesc") as TextBlock;

                tb.Inlines.Clear();

                ev.Description = edEvent.Description;

                m_wd.PJDatas.CurrentDate = null;
                m_wd.PJDatas.CurrentDate = ev;
            }
        }

        private void ListEventsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextBlock tb = FindName("EventDesc") as TextBlock;

            if (m_wd.PJDatas.CurrentDate != null)
            {
                String[] listName = m_wd.SQLDatabase.GetListName();

                Utils.SetTextLink(tb, m_wd.PJDatas.CurrentDate.Description, listName, (Style)FindResource("LinkStyle"), this.PlaceDown);
            }
            else
                tb.Inlines.Clear();
        }
    }

    public class PJDataContext : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected ObservableCollection<PJGroupEvent> m_groupEvent;
        protected ObservableCollection<Character> m_listCharacters;
        protected Character m_characterSelected = null;
        protected PJGroupEvent m_currentDate = null;

        private WindowData  m_wd;
        private BitmapImage m_characterIcon = null;

        public PJDataContext(WindowData wd)
        {
            m_wd = wd;
            m_characterIcon  = StringToImageConverter.BitmapFromPath(System.AppDomain.CurrentDomain.BaseDirectory+"/Resources/DefaultCaracterIcon.png");
            m_groupEvent     = new ObservableCollection<PJGroupEvent>();
            m_listCharacters = new ObservableCollection<Character>();
        }

        public void AddGroupEvent(GroupEvent ge)
        {
            String charaList = String.Join(", ", m_wd.SQLDatabase.GetCharaNamesEvent(ge).ToArray());
            m_groupEvent.Add(new PJGroupEvent { ID = m_groupEvent.Count+1, Description = ge.Description, GroupEventID = ge.ID, CharaList = charaList });
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

        public ObservableCollection<PJGroupEvent> EventList  { get => m_groupEvent; set { m_groupEvent = value; } }
        public ObservableCollection<Character> CharacterList { get => m_listCharacters; set { m_listCharacters = value; } }
        public PJGroupEvent CurrentDate { get => m_currentDate; set { m_currentDate = value; OnPropertyChanged("CurrentDate"); } }

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