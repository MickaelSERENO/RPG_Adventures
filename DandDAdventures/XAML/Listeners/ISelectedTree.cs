using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DandDAdventures.XAML.Listeners
{
    public interface ISelectedTree
    {
        void OnSelectPJ(Character[] chara);
        void AddDate(CreateDate cd, Character[] characters);
        void OnSelectPlace(Place place);
    }
}
