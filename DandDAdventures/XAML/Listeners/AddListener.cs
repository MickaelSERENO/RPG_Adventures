using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DandDAdventures.XAML.Listeners
{
    public class AddPJListener : IActionListener
    {
        protected WindowData m_wd;

        public AddPJListener(WindowData wd) : base()
        {
            m_wd = wd;
        }

        public void OnFire()
        {
            //Show the window
            var w = new AddPJWindow(m_wd);
            w.ShowDialog();

            //Commit the Dialog
            if(w.Datas.CharacterAdded)
                m_wd.CommitDB.AddPJ(w.Datas.NewCharacters.ToArray());
        }
    }

    public class AddPlaceListener : IActionListener
    {
        protected WindowData m_wd;

        public AddPlaceListener(WindowData wd) : base()
        {
            m_wd = wd;
        }

        public void OnFire()
        {
            //Show the window
            var w = new AddPlaceWindow(m_wd);
            w.ShowDialog();

            AddPlaceDatas datas = w.DataContext as AddPlaceDatas;
            //Commit the Dialog
            if (datas.PlaceAdded)
                m_wd.CommitDB.AddPlace(new Place{Name = datas.Name, Story = datas.PlaceStory});
        }
    }
}
