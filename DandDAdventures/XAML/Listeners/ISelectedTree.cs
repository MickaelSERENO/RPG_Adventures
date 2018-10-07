using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DandDAdventures.XAML.Listeners
{
    public interface ISelectedTree
    {
        /// <summary>
        /// Change the UI when characters are selected
        /// </summary>
        /// <param name="chara">List of selected characters</param>
        void OnSelectCharacter(Character[] chara);

        /// <summary>
        /// Add a date associated with multiple characters
        /// </summary>
        /// <param name="cd">The CreateData Window object</param>
        /// <param name="characters">The associated characters</param>
        void AddDate(CreateDate cd, Character[] characters);

        /// <summary>
        /// Change the UI when a place is selected
        /// </summary>
        /// <param name="place">The place selected</param>
        void OnSelectPlace(Place place);
    }
}
