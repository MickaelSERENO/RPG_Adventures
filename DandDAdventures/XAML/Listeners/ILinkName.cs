using System;

namespace DandDAdventures.XAML.Listeners
{
    /// <summary>
    /// Interface permitting to set the View according to a name, for example a name the user has clicked on.
    /// This Listener was originally created for not passing MainWindow as parameter to other class
    /// </summary>
    public interface ILinkName
    {
        /// <summary>
        /// Link the View to a specific name (place or character name for example)
        /// </summary>
        /// <param name="s">The name</param>
        void LinkToName(String s);
    }
}
