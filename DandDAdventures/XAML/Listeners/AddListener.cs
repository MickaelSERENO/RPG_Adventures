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
            if(w.CharacterAdded)
                m_wd.CommitDB.AddPJ(w.NewCharacters.ToArray(), w.NewPJs.ToArray());
        }
    }

    public class AddPNJListener : IActionListener
    {
        protected WindowData m_wd;

        public AddPNJListener(WindowData wd) : base()
        {
            m_wd = wd;
        }

        public void OnFire()
        {
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
        }
    }
}
