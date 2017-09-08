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
            m_wd = wd;
            DataContext = m_wd;
            InitializeComponent();
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

        private void TreasureOwnerRowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if(e.EditAction == DataGridEditAction.Commit)
            {
                DataGrid dg = (DataGrid)sender;
                
                var newItem = e.Row.DataContext as StringWrapped;
                if(newItem.Value == null)
                {
                    return;
                }
                bool found = false;

                for(int i=0; i < m_wd.PlaceDatas.TreasureSelected.TreasureOwner.Count; i++)
                {
                    if (i == dg.SelectedIndex)
                        continue;
                    StringWrapped str = m_wd.PlaceDatas.TreasureSelected.TreasureOwner[i];
                    if (str.Value == newItem.Value)
                        found = true;
                }

                if(found)
                {
                    newItem.Value = "";
                    e.Cancel = true;
                }

                else
                {
                    m_wd.SQLDatabase.RefreshTreasureOwner(m_wd.PlaceDatas.TreasureSelected);
                    MessageBox.Show("Object not in collection");
                }
            }
        }

        private void OwnerPreviewDeleteCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == DataGrid.DeleteCommand)
            {
                DataGrid dg = sender as DataGrid;
                String charaName = ((StringWrapped)dg.SelectedItem).Value;
                e.Handled = false;
                m_wd.SQLDatabase.DeleteTreasureOwner(charaName, m_wd.PlaceDatas.TreasureSelected.Treasure.ID);
            }
        }

        private void TreasureCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            if(m_wd.PlaceDatas.TreasureSelected != null)
                m_wd.SQLDatabase.SetTreasure(m_wd.PlaceDatas.TreasureSelected.Treasure);
        }

        private void TreasureListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Actually nothing
        }
    }

    public class PlaceDataContext : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler        PropertyChanged;
        private      WindowData                         m_wd;
        protected    ObservableCollection<Place>        m_placeList;
        protected    ObservableCollection<TreasureItem> m_treasureList;
        protected    TreasureItem                       m_treasureSelected = null;
        protected    Place                              m_placeSelected    = null;

        public PlaceDataContext(WindowData wd)
        {
            m_wd = wd;
            m_placeList = new ObservableCollection<Place>();
            m_treasureList = new ObservableCollection<TreasureItem>();
        }

        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public ObservableCollection<Place>        PlaceList    { get => m_placeList; set => m_placeList = value; }
        public ObservableCollection<TreasureItem> TreasureList { get => m_treasureList; }
        public TreasureItem                       TreasureSelected
        {
            get
            {
                return m_treasureSelected;
            }
            set
            {
                m_treasureSelected = value;
                OnPropertyChanged("TreasureSelected");
            }
        }
        public Place                              PlaceSelected
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