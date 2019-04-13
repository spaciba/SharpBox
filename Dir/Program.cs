using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Dir
{
    class Program
    {
        static void printDir(string path)
        {
            Console.WriteLine("Directory of " + path);
            Console.WriteLine("Last Write: " + (Directory.GetLastWriteTime(path)));
            Console.WriteLine("\n\n");

            string[] files = Directory.GetFileSystemEntries(path);
            foreach (string f in files)
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

        static void printFile(string path)
        {
            FileInfo fin = new FileInfo(path);
            
            String output = String.Format("{0}\t\t{1} {2}", File.GetLastWriteTime(path), fin.Length, path.Substring(path.LastIndexOf('\\') + 1));
            Console.WriteLine(output);
        }

        static string printFileString(string path) //for when you want to manipulate the data returned
        {
            FileInfo fin = new FileInfo(path);

            String output = String.Format("{0}\t\t{1} {2}", File.GetLastWriteTime(path), fin.Length, path.Substring(path.LastIndexOf('\\') + 1));
            return output;
        }

        static void unorderedRecursiveDir(string path)
        {
            foreach( string dir in Directory.GetDirectories(path))
            {
                Console.WriteLine(dir);
                try
                {
                    foreach (string file in Directory.GetFiles(dir))
                    {
                        string sfile = printFileString(file);
                        Console.WriteLine("\t" + sfile);
                    }

                    foreach (string subdir in Directory.GetDirectories(dir))
                    {
                        Console.WriteLine("\n\t");
                        Console.WriteLine(subdir);
                        
                        unorderedRecursiveDir(subdir);
                    }
                }
                catch(System.UnauthorizedAccessException e)
                {
                    Console.WriteLine("Access to the path '" + dir + "' is denied");
                }
                Console.WriteLine("\n\n");
            }
        }

        static void Main(string[] args)
        {

            int numArgs = args.Length;
            String path = "";
            bool isDir = false;
            bool orderDate = false;
            bool recur = false;

            switch (numArgs)
            {
                case 0:
                    Console.WriteLine("Not enough arguments");
                    break;
                case 1:
                    path = args[0];
                    break;
                case 2:
                    if (args[0] == "/od")
                        orderDate = true;
                    if (args[0] == "/s")
                        recur = true;
                    path = args[1];
                    break;
                case 3:
                    if ((args[0] == "/od") || args[1] == "/od")
                        orderDate = true;
                    if ((args[0] == "/s") || args[1] == "/s")
                        recur = true;
                    path = args[2];
                    break;
                default:
                    Console.WriteLine("Something went wrong");
                    break;
            }




            if ((File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory)
                isDir = true;

            if ((isDir == true) && (recur == false) && (orderDate == false))
            {
                printDir(path);
            }
            else if((isDir == false) && (recur == false) && (orderDate == false))
            {
                Console.WriteLine("Directory of " + path.Substring(0, path.LastIndexOf('\\')));
                Console.WriteLine("Last Write: " + (Directory.GetLastWriteTime(path.Substring(0, path.LastIndexOf('\\')))));
                Console.WriteLine("\n\n");

                printFile(path);
            }

            if((recur == true) && (orderDate == false))
            {
                unorderedRecursiveDir(path);
            }

        }
    }
}

