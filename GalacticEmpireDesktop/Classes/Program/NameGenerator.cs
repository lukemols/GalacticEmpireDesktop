using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace GalacticEmpire
{
    static class NameGenerator
    {
        static List<string> baseStrings;

        static Random rnd;

        static public string GetName(int num = 2)
        {
            if (baseStrings == null)
                throw new FileNotFoundException();
            if (num < 1)
                num = 1;
            else if (num > 5)
                num = 5;
            
            string s = "";
            for(int i = 0; i< num; i++)
            {
                s += baseStrings[rnd.Next(0, baseStrings.Count)];
            }

            return char.ToUpper(s[0]) + s.Substring(1);
        }

#if WINDOWS
        static public void LoadFile()
        {
            string inputPath = @"Files\Syllable.txt";
            baseStrings = new List<string>();
            rnd = new Random(DateTime.Now.Millisecond);

            StreamReader sr = new StreamReader(inputPath);
            string line;

            while ((line = sr.ReadLine()) != null)
            {
                if (!baseStrings.Contains(line))
                    baseStrings.Add(line);
            }
            sr.Close();
        }
#endif


#if WINDOWS_PHONE_APP
            static async public void LoadFile()
        {
            baseStrings = new List<string>();
            rnd = new Random(DateTime.Now.Millisecond);
            string inputPath = Windows.ApplicationModel.Package.Current.InstalledLocation.Path + @"\Files\Syllable.txt";
            Windows.Storage.StorageFile sFile = await Windows.Storage.StorageFile.GetFileFromPathAsync(inputPath);
            
            string text = await Windows.Storage.FileIO.ReadTextAsync(sFile);
            string[] lines = text.Split('\n');
            foreach (string s in lines)
                baseStrings.Add(s);
    }
#endif
    }
}
