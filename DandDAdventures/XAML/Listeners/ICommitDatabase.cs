using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DandDAdventures.XAML.Listeners
{
    public interface ICommitDatabase
    {
        void AddCharacter(Character[] charas);
        void AddPlace(Place p);
    }
}
