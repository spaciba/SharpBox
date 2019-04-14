using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace sc
{
    class ServiceFunctions
    {
        //Service Function imports

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern Boolean QueryServiceConfig(IntPtr hService, IntPtr intPtrQueryConfig, UInt32 cbBufSize, out UInt32 pcbBytesNeeded);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
        public static extern Boolean QueryServiceConfig2(IntPtr hService, UInt32 dwInfoLevel, IntPtr buffer, UInt32 cbBufSize, out UInt32 pcbBytesNeeded);

        [DllImport("advapi32.dll",  CharSet = CharSet.Unicode, SetLastError = true)]
        static extern IntPtr OpenSCManager(string machineName, string databaseName, uint dwDesiredAccess);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr OpenService(IntPtr hSCManager, String lpServiceName, UInt32 dwDesiredAccess);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern Boolean QueryServiceStatusEx(IntPtr hService, UInt32 InfoLevel, IntPtr lpBuffer, UInt32 cbBufSize, out UInt32 pcbBytesNeeded);
        
        //Service structure definitions


        [StructLayout(LayoutKind.Sequential)]
        public class QUERY_SERVICE_CONFIG
        {
            UInt32 dwBytesNeeded;
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)]
            public UInt32 dwServiceType;
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)]
            public UInt32 dwStartType;
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)]
            public UInt32 dwErrorControl;
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
            public String lpBinaryPathName;
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
            public String lpLoadOrderGroup;
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)]
            public UInt32 dwTagID;
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
            public String lpDependencies;
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
            public String lpServiceStartName;
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
            public String lpDisplayName;
        };

        [StructLayout(LayoutKind.Sequential)]
        public class SERVICE_DESCRIPTION
        {
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)]
            public String lpDescription;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class QUERY_SERVICE_STATUS_PROCESS
        {
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)]
            public UInt32 dwServiceType;
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)]
            public UInt32 dwCurrentState;
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)]
            public UInt32 dwControlsAccepted;
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)]
            public UInt32 dwWin32ExitCode;
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)]
            public UInt32 dwServiceSpecificExitCode;
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)]
            public UInt32 dwCheckPoint;
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)]
            public UInt32 dwWaitHint;
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)]
            public UInt32 dwProcessId;
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)]
            public UInt32 dwServiceFlags;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public class SERVICE_FAILURE_ACTIONS
        {
            public int dwResetPeriod;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpRebootMsg;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpCommand;
            public int cActions;
            public IntPtr lpsaActions;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class SC_ACTION
        {
            public Int32 type;
            public UInt32 dwDelay;
        }


        //Service constant definitions

        private const int SC_MANAGER_ALL_ACCESS = 0x000F003F;
        private const int SERVICE_QUERY_CONFIG = 0x00000001;
        private const UInt32 SERVICE_CONFIG_DESCRIPTION = 0x01;
        private const UInt32 SERVICE_CONFIG_FAILURE_ACTIONS = 0x02;
        private const int SC_STATUS_PROCESS_INFO = 0;

        public static void ServiceQuery(string host, string svcname)
        {
            bool success;
            IntPtr dbHandle;
            IntPtr svcHandle;
            IntPtr ptr;
            UInt32 dwBytesNeeded;
            QUERY_SERVICE_CONFIG qconf;
            SERVICE_FAILURE_ACTIONS qfail;
            int failStructOffset;
            QUERY_SERVICE_STATUS_PROCESS qproc;
            

            if (host == "")
                dbHandle = OpenSCManager(null, null, SC_MANAGER_ALL_ACCESS);
            else
                dbHandle = OpenSCManager(host, null, SC_MANAGER_ALL_ACCESS);

            svcHandle = OpenService(dbHandle, svcname, SERVICE_QUERY_CONFIG);
            if(svcHandle == IntPtr.Zero)
            {
                throw new System.Runtime.InteropServices.ExternalException("Couldn't open the service");
            }

            ptr = Marshal.AllocHGlobal(4096);
            success = QueryServiceConfig(svcHandle, ptr, 4096, out dwBytesNeeded);

            if (success == false)
                throw new System.Runtime.InteropServices.ExternalException("Couldn't query the service");

            qconf = new QUERY_SERVICE_CONFIG(); // copy ptr to query_service_config struct
            Marshal.PtrToStructure(ptr, qconf);

            Marshal.FreeHGlobal(ptr); //reset states
            success = false;
            dwBytesNeeded = 0;

            success = QueryServiceConfig2(svcHandle, SERVICE_CONFIG_FAILURE_ACTIONS, IntPtr.Zero, 0, out dwBytesNeeded); //the first call of this function will get the number of bytes needed for the struct
            ptr = Marshal.AllocHGlobal((int)dwBytesNeeded);
            success = QueryServiceConfig2(svcHandle, SERVICE_CONFIG_FAILURE_ACTIONS, ptr, dwBytesNeeded, out dwBytesNeeded);

            qfail = new SERVICE_FAILURE_ACTIONS();
            Marshal.PtrToStructure(ptr, qfail);
            SC_ACTION[] actions = new SC_ACTION[qfail.cActions];
            failStructOffset = 0;

            Marshal.FreeHGlobal(ptr); //reset states
            success = false;
            dwBytesNeeded = 0;

            success = QueryServiceStatusEx(svcHandle, SC_STATUS_PROCESS_INFO, IntPtr.Zero, 0, out dwBytesNeeded);//again first call gets buffer size required for struct
            ptr = Marshal.AllocHGlobal((int)dwBytesNeeded);
            success = QueryServiceStatusEx(svcHandle, SC_STATUS_PROCESS_INFO, ptr, dwBytesNeeded, out dwBytesNeeded);

            qproc = new QUERY_SERVICE_STATUS_PROCESS();
            Marshal.PtrToStructure(ptr, qproc);

            Marshal.FreeHGlobal(ptr); //reset states
            success = false;
            dwBytesNeeded = 0;

            for (int i=0; i<qfail.cActions; i++)
            {
                SC_ACTION act = new SC_ACTION();
                act.type = Marshal.ReadInt32(qfail.lpsaActions, failStructOffset);
                failStructOffset += sizeof(Int32);
                act.dwDelay = (UInt32)Marshal.ReadInt32(qfail.lpsaActions, failStructOffset);
                failStructOffset += sizeof(Int32);
                actions[i] = act;
            }


            Console.WriteLine("Service Name:\t" + svcname);
            Console.WriteLine("Service Type:\t" + qproc.dwServiceType);
            Console.WriteLine("State:\t" + qproc.dwCurrentState);
            Console.WriteLine("PID:\t" + qproc.dwProcessId);
            Console.WriteLine();
            Console.WriteLine("Start Type:\t" + qconf.dwStartType);
            Console.WriteLine("Error Control:\t" + qconf.dwErrorControl);
            Console.WriteLine("BinPath:\t" + qconf.lpBinaryPathName);
            Console.WriteLine("Display Name:\t" + qconf.lpDisplayName);
            Console.WriteLine("Dependencies:\t" + qconf.lpDependencies);
            Console.WriteLine("Reset Period:\t" + qfail.dwResetPeriod);
            Console.WriteLine("Reboot Message:\t" + qfail.lpRebootMsg);
            Console.WriteLine("Failure Command Line:\t" + qfail.lpCommand);
            Console.WriteLine("Failure Actions:\t");
            for(int i =0; i<actions.Length; i++)
            {
                Console.Write("{0} -- Delay = {1}", actions[i].type, actions[i].dwDelay);
            }
        }
    }
}
