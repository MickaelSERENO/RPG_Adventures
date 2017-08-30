using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace DandDAdventures.XAML.Controls
{
    /// <summary>
    /// Logique d'interaction pour PlaceView.xaml
    /// </summary>
    public partial class PlaceView : UserControl
    {
        protected TextBlock m_summaryVal;
        protected WindowData m_wd;
        public PlaceView(WindowData wd)
        {
            InitializeComponent();
            m_wd = wd;
            m_summaryVal = FindName("PlaceStory") as TextBlock;
        }

        public void LoadContent(WindowData wd)
        {
        }

        public void SetPlaceStory(string s)
        {
            String[] listName = m_wd.SQLDatabase.GetListName();
            Utils.SetTextLink(m_summaryVal, s, listName, (Style)FindResource("LinkStyle"), this.PlaceDown);
        }

        private void PlaceDown(object sender, MouseButtonEventArgs e)
        {
            Run r = sender as Run;
            m_wd.LinkName?.LinkToName(r.Text);
        }
    }

    public class PlaceDataContext : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private      WindowData m_wd;
        protected    ObservableCollection<Place> m_placeList;
        protected    Place m_placeSelected = null;

        public PlaceDataContext(WindowData wd)
        {
            m_wd = wd;
            m_placeList = new ObservableCollection<Place>();
        }

        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public ObservableCollection<Place> PlaceList { get => m_placeList; set => m_placeList = value; }
        public Place                       PlaceSelected
        {
            get
            {
                return m_placeSelected;
            }
            set
            {
                m_placeSelected = value;
                OnPropertyChanged("PlaceSelected");
            }
        }
    }
}