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
    /// Interaction Logic for TabItem.xaml
    /// </summary>
    public partial class PJTabItemCtrl : UserControl
    {
        /// <summary>
        /// The Application data
        /// </summary>
        private WindowData      m_wd;
        private IActionListener m_addListener  = null;
        private ICommand        m_createDate   = null;

        private ListView m_listView = null;

        public PJTabItemCtrl(WindowData wd) : base()
        {
            InitializeComponent();
            m_wd = wd;
            m_listView = (ListView)FindName("MainListView");
        }

        public void SetAddListener(IActionListener l)
        {
            m_addListener = l;
        }

        public void LoadContent(WindowData datas)
        {
            var charas = datas.SQLDatabase.GetCharacters();
            foreach (var c in charas)
                datas.PJDatas.CharacterList.Add(c);
        }

        private void AddClick(object sender, RoutedEventArgs e)
        {
            m_addListener?.OnFire();
        }

        private void MainListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView l = (ListView)sender;
            m_wd.SelectedTree?.OnSelectPJ(l.SelectedItems.Cast<Character>().ToArray());
        }

        //For right click avoidance
        private void OnListViewItemPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void MainListViewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                HitTestResult r = VisualTreeHelper.HitTest(this, e.GetPosition(this));
                if (r.VisualHit.GetType() != typeof(ListBoxItem))
                    m_listView.UnselectAll();
            }
        }

        private void ListViewItemMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            Character chara = ((ListViewItem)sender).Content as Character;
            EditPJ ep = new EditPJ(m_wd, chara);
            ep.ShowDialog();
            m_wd.PJDatas.CharacterSelected = null;
            m_wd.PJDatas.CharacterSelected = chara;
        }

        //Menu handlers
        public ICommand CreateDate
        {
            get
            {
                return m_createDate ?? (m_createDate = new RelayCommand(CanCreateDate, CreateDateCommand));
            }
        }

        public bool CanCreateDate(object sender)
        {
            return m_listView.SelectedItems.Count > 0;
        }

        public void CreateDateCommand(object param)
        {
            CreateDate cd = new CreateDate(m_wd, m_listView.SelectedItems.Cast<Character>().ToArray());
            cd.ShowDialog();

            if(cd.IsAdded)
                m_wd.SelectedTree.AddDate(cd, m_listView.SelectedItems.Cast<Character>().ToArray());
        }
    }
}
