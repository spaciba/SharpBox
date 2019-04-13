using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Dir
{
    class Program
    {
        static void Main(string[] args)
        {

            bool isDir = false;
            String path = args[0];
            

            if ((File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory)
                isDir = true;

            if(isDir == true)
            {
                Console.WriteLine("Directory of " + path);
                Console.WriteLine("Last Write: " + (Directory.GetLastWriteTime(path)));
                Console.WriteLine("\n\n");

                string[] files = Directory.GetFileSystemEntries(path);
                foreach(string f in files)
                {
                    String output;

                    if ((File.GetAttributes(f) & FileAttributes.Directory) == FileAttributes.Directory)
                    {
                        output = String.Format("{0}\t{1}\t\t{2}", File.GetLastWriteTime(f), "<DIR>", f.Substring(f.LastIndexOf('\\') + 1));
                        Console.WriteLine(output);
                    }
                    else
                    {
                        FileInfo fin = new FileInfo(f);
                        output = String.Format("{0}\t\t{1,2} {2,5}", File.GetLastWriteTime(f), fin.Length, f.Substring(f.LastIndexOf('\\') + 1));
                        Console.WriteLine(output);
                    }
                }
       
            }
            else
            {
                FileInfo fin = new FileInfo(path);
                Console.WriteLine("Directory of " + path.Substring(0, path.LastIndexOf('\\')));
                Console.WriteLine("Last Write: " + (Directory.GetLastWriteTime(path.Substring(0, path.LastIndexOf('\\')))));
                Console.WriteLine("\n\n");


                String output = String.Format("{0}\t\t{1} {2}", File.GetLastWriteTime(path), fin.Length, path.Substring(path.LastIndexOf('\\') + 1));
                Console.WriteLine(output);
            }

        }
    }
}

