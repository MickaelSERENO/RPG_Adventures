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
using System.Windows.Shapes;

namespace DandDAdventures.XAML
{
    /// <summary>
    /// Logique d'interaction pour EditPlace.xaml
    /// </summary>
    public partial class EditPlace : Window
    {
        private WindowData m_wd;

        public EditPlace(Place place, WindowData wd)
        {
            m_wd        = wd;
            DataContext = new EditPlaceDatas(place, wd);
            InitializeComponent();
        }

        private void OKBtnClick(object sender, RoutedEventArgs e)
        {
            EditPlaceDatas datas = DataContext as EditPlaceDatas;
            m_wd.SQLDatabase.AddTreasure(datas.TreasureList);
            datas.Place.Story = ((TextBox)FindName("StoryText")).Text;
            m_wd.SQLDatabase.SetPlace(datas.Place);
            Close();
        }

        private void CancelBtnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AddTreasureBtnClick(object sender, RoutedEventArgs e)
        {
            EditPlaceDatas datas = DataContext as EditPlaceDatas;
            TreasureItem ti      = new TreasureItem { Treasure = new Treasure { PlaceName = datas.Place.Name }, TreasureID=datas.TreasureList.Count};
            datas.TreasureList.Add(ti);
        }

        private void TreasureListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Nothing
        }
    }

    public class StringWrapped
    {
        public String Value { get; set; }
    }

    public class TreasureItem
    {
        public Treasure Treasure   { get; set; }
        public int      TreasureID { get; set; }
        public ObservableCollection<TreasureValue> TreasureValue { get; set; } 
        public ObservableCollection<StringWrapped> TreasureOwner { get; set; }

        public TreasureItem()
        {
            TreasureValue = new ObservableCollection<TreasureValue>();
            TreasureOwner = new ObservableCollection<StringWrapped>();
        }
    }

    public class EditPlaceDatas : INotifyPropertyChanged
    {
        protected WindowData m_wd;
        protected Place m_place;
        protected ObservableCollection<TreasureItem> m_treasureList;
        public event PropertyChangedEventHandler PropertyChanged;

        public EditPlaceDatas(Place place, WindowData wd)
        {
            m_place = place;
            m_wd    = wd;

            m_treasureList  = new ObservableCollection<TreasureItem>();

            //Fill the collections
            int i = 0;
            var treasures = wd.SQLDatabase.GetTreasures(place.Name);
            foreach (var t in treasures)
            {
                TreasureItem ti = new TreasureItem { TreasureID = i, Treasure = t };
                var objects = wd.SQLDatabase.GetTreasureValues(t.ID);

                foreach (var tv in objects)
                    ti.TreasureValue.Add(tv);

                m_treasureList.Add(ti);
                i++;
            }
        }

        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public Place Place                                      { get => m_place; set => m_place = value; }
        public ObservableCollection<TreasureItem> TreasureList  { get => m_treasureList; }
    }
}
