using DandDAdventures.XAML.Listeners;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DandDAdventures.XAML.Controls
{
    /// <summary>
    /// Logique d'interaction pour PlaceTabItemCtrl.xaml
    /// </summary>
    public partial class PlaceTabItemCtrl : UserControl
    {
        private IActionListener m_addListener = null;
        private WindowData m_wd;
        public PlaceTabItemCtrl(WindowData wd)
        {
            m_wd = wd;
            InitializeComponent();
        }

        public void LoadContent(WindowData wd)
        {
            var places = wd.SQLDatabase.GetPlaces();
            foreach (var p in places)
                wd.PlaceDatas.PlaceList.Add(p);
        }

        public void SetAddListener(IActionListener add)
        {
            m_addListener = add;
        }

        private void AddClick(object sender, RoutedEventArgs e)
        {
            m_addListener?.OnFire();
        }

        private void ListViewItemMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            Place place = ((ListViewItem)sender).Content as Place;
            EditPlace ep = new EditPlace(place, m_wd);
            ep.ShowDialog();
            m_wd.PlaceDatas.PlaceSelected = null;
            m_wd.PlaceDatas.PlaceSelected = place;
        }
    }
}
