using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace REST
{
    class FileHandle
    {
        public FileHandle() { }

        static public string[] GetFiles(string path)
        {
            string[] files = Directory.GetFiles(path);

            Array.Sort(files, (string item1, string item2) =>
                 Convert.ToInt32(Path.GetFileNameWithoutExtension(item1)).
                 CompareTo(Convert.ToInt32(Path.GetFileNameWithoutExtension(item2)))
             );

            return files;
        }

        static public int ComputeNewFileName(string path)
        {
            string[] files = Directory.GetFiles(path);
            int[] ids = files.Select(el => Convert.ToInt32(Path.GetFileNameWithoutExtension(el))).ToArray();

            return ids.Max() + 1;
        }

        static public string ComputePath(string path, string[] tokens)
        {
            foreach (string token in tokens)
            {
                path = Path.Join(path, token);
            }

            return path;
        }
    }
}
