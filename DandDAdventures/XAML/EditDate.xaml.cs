using DandDAdventures.XAML.Controls;
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
    /// <summary>
    /// Logique d'interaction pour EditCharacter.xaml
    /// </summary>
    public partial class EditDate : Window
    {
        WindowData m_wd;
        public EditDate(WindowData wd, CharacterGroupEvent ev)
        {
            m_wd = wd;
            DataContext = new EditDateDatas(wd, ev);
            InitializeComponent();
        }

        private void OKClick(object sender, RoutedEventArgs e)
        {
            EditDateDatas datas = DataContext as EditDateDatas;
            datas.Valid = true;
            m_wd.SQLDatabase.SetDate(new GroupEvent{ID=datas.Event.GroupEventID, Description=datas.Event.Description});
            Close();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            EditDateDatas datas = DataContext as EditDateDatas;
            datas.Valid = false;
            Close();
        }
    }

    public class EditDateDatas : INotifyPropertyChanged
    {
        protected WindowData m_wd;
        protected CharacterGroupEvent m_event;

        public event PropertyChangedEventHandler PropertyChanged;

        public EditDateDatas(WindowData wd, CharacterGroupEvent ev)
        {
            m_event = ev;
            m_wd = wd;
        }

        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public Boolean Valid { get; set; }
        public CharacterGroupEvent Event{ get => m_event; set => m_event = value; }
    }
}