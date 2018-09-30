using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;

namespace DandDAdventures
{
    class Utils
    {
        /// <summary>
        /// Modify a TextBlock for adding Inlines permitting to link a part of the text to an object (caracter, place, etc.)
        /// </summary>
        /// <param name="txt">The TextBlock to modify</param>
        /// <param name="s">The string to look at</param>
        /// <param name="listName">the key name to look at (a.k.a the available link)</param>
        /// <param name="linkStyle">The style of the link text (blue text ?)</param>
        /// <param name="ev">The event handler to pass to the "link" texts</param>
        public static void SetTextLink(TextBlock txt, String s, String[] listName, Style linkStyle, MouseButtonEventHandler ev)
        {
            txt.Inlines.Clear();
            if (s == "")
                return;

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

            //Add the subtexts to the TextBlock
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

        /// <summary>
        /// Openes a FileDialog in order to get a file
        /// </summary>
        /// <param name="description">The Description to display</param>
        /// <param name="path">Variable where to store the Path. Must NOT BE NULL</param>
        /// <returns>true on Success (dialog closed with OK), false on failure.</returns>
        static public bool OpenFileDialog(String description, out String path)
        {
            path = null;

            using(FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                dlg.Description         = description;
                dlg.SelectedPath        = path;
                dlg.ShowNewFolderButton = true;
                DialogResult result     = dlg.ShowDialog();
                if(result == System.Windows.Forms.DialogResult.OK)
                {
                    path = dlg.SelectedPath;
                    return true;
                }
            }
            return false;
        }
    }
}
