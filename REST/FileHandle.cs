using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace REST
{
    class FileHandle
    {
        public string Root { get; private set; }
        public FileHandle() 
        {
            Root = Path.Join(Directory.GetCurrentDirectory(), "ressources");
        }

        public string[] GetFiles(string path)
        {
            string[] files = Directory.GetFiles(path);

            Array.Sort(files, (string item1, string item2) =>
                 Convert.ToInt32(Path.GetFileNameWithoutExtension(item1)).
                 CompareTo(Convert.ToInt32(Path.GetFileNameWithoutExtension(item2)))
             );

            return files;
        }

        public int ComputeNewFileName(string path)
        {
            string[] files = Directory.GetFiles(path);
            int[] ids = files.Select(el => Convert.ToInt32(Path.GetFileNameWithoutExtension(el))).ToArray();

            return ids.Max() + 1;
        }

        public string ComputePath(string route)
        {
            string[] tokens = route.Split('/');
            string path = "";

            foreach (string token in tokens)
                path = Path.Join(path, token);
            

            return path;
        } 

        public string PathWithRoot(string path)
        {
            return Path.Join(Root, path);
        }
    }
}
