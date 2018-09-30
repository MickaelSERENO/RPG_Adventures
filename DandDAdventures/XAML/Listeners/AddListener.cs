using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DandDAdventures.XAML.Listeners
{
    /// <summary>
    /// AddPJListener. This will create the AppPJWindow and modify the database when finishing (see OnFire)
    /// </summary>
    public class AddPJListener : IActionListener
    {
        /// <summary>
        /// The Window data (application data)
        /// </summary>
        protected WindowData m_wd;

        /// <summary>
        /// Constructor, initialize the object with the global data structure (application data)
        /// </summary>
        /// <param name="wd">The application data</param>
        public AddPJListener(WindowData wd) : base()
        {
            m_wd = wd;
        }

        /// <summary>
        /// This function create the AddPJWindow and modify the database if needed (adding a character)
        /// </summary>
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

    /// <summary>
    /// AddPlaceListener. Opens the AddPlace dialog for adding places and modify the database (see OnFire)
    /// </summary>
    public class AddPlaceListener : IActionListener
    {
        /// <summary>
        /// The Window data (application data)
        /// </summary>
        protected WindowData m_wd;

        /// <summary>
        /// Constructor, initialize the object with the global data structure (application data)
        /// </summary>
        /// <param name="wd">The application data</param>
        public AddPlaceListener(WindowData wd) : base()
        {
            m_wd = wd;
        }

        /// <summary>
        /// This function create the AddPJWindow and modify the database if needed (adding a character)
        /// </summary>
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
