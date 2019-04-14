using System;
using System.Collections.Generic;
using System.Net;


namespace sc
{

    class Program
    {
        

        static void Main(string[] args)
        {
            Dictionary<string, string> commandargs = new Dictionary<string, string>();
            int argLength = args.Length;
            string host = "";
            string command = "";

            if (argLength > 1)
            {
                if ((args[0].StartsWith("\\\\")))
                {
                    host = args[0];
                }
                else
                {
                    command = args[0];

                    for(int i=1; i<args.Length-1; i++)
                    {
                        if (args[i].EndsWith("="))
                            commandargs.Add(args[i], args[i + 1]);
                    }
                }

                switch(command.ToLower())
                {
                    case "query":
                        ServiceFunctions.ServiceQuery(host, args[1]);
                        break;
                }

            }
            
        }
    }
}
