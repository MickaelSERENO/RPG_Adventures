using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace DandDAdventures
{
    class Utils
    {
        public static void SetTextLink(TextBlock txt, String s, String[] listName, Style linkStyle, MouseButtonEventHandler ev)
        {
            txt.Inlines.Clear();

            //Sort by length, because the longest name is allways the correct one
            Array.Sort(listName, ((a, b) => b.Length - a.Length));

            //Arrays containing the subString and if the str is a name or not
            List<String> subString = new List<String>();
            List<bool> indexFound = new List<bool>();
            subString.Add(s);
            indexFound.Add(false);

            foreach (String name in listName)
            {
                for (int i = 0; i < subString.Count; i++)
                {
                    if (indexFound[i])
                        continue;

                    String str = subString[i];
                    int index = str.IndexOf(name);

                    if (index != -1)
                    {
                        //Remove this String to remplace by 3 string : 0 -> index found; index found ->+name.Length; index found + name.Length -> end of str
                        subString.RemoveAt(i);
                        subString.Insert(i, str.Substring(0, index));
                        subString.Insert(i + 1, str.Substring(index, name.Length));
                        subString.Insert(i + 2, str.Substring(index + name.Length));

                        indexFound.RemoveAt(i);
                        indexFound.Insert(i, false);
                        indexFound.Insert(i, true);
                        indexFound.Insert(i, false);
                    }
                }
            }

            for (int i = 0; i < subString.Count; i++)
            {
                String str = subString[i];
                if (indexFound[i])
                {
                    Run r = new Run(str);
                    r.MouseDown += ev;
                    r.Style = linkStyle;

                    txt.Inlines.Add(r);
                }
                else
                    txt.Inlines.Add(new Run(str));
            }
        }
    }
}
