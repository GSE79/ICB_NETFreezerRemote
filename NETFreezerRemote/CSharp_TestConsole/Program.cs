
// import namespaces
using FreezerRemote;
using System.Net;
using System.Security.Cryptography.X509Certificates;

// initialize single freezer remote instance (from dll)
FreezerRemoteClass thisRemoteFreezer = new FreezerRemoteClass();

// initialize single freezer status variable for user input and modbus tcp connected status
string LineIn;
bool Connected = false;
IPAddress addrIn;
string outString = "";

// loop forever
while (true)
{
    // connect, if not connected
    if (!thisRemoteFreezer.IsConnected())
    {
        Console.WriteLine("Type an IP Address then Press Enter to Connect:");
        // read user input from console
        LineIn = Console.ReadLine();        

        // parse input
        if (LineIn != null)
        {            
            if (IPAddress.TryParse(LineIn, out addrIn))
            {
                // set ip addr string if parse success
                thisRemoteFreezer.ipAddr = addrIn.ToString();
            }
        }
        Console.WriteLine("Connecting..."+thisRemoteFreezer.ipAddr);
        // try to connect
        thisRemoteFreezer.Connect();        
    }
    else// if connected,
    {
        // read user input from console
        LineIn = Console.ReadLine();

        // parse input / action on input
        if (LineIn == null)
            ;//do nothing
        else if (LineIn.ToLower() == "exit")
            break; // exit loop - close program
        else if (LineIn.ToLower() == "start")
            thisRemoteFreezer.StartFill(); // start fill using dll
        else if (LineIn.ToLower() == "stop")
            thisRemoteFreezer.StopFill(); // stop fill using dll
        else if (LineIn.ToLower() == "reset")
            thisRemoteFreezer.ResetAlarmsWarnings(); // reset alarms/warnings using dll
        else if (LineIn.ToLower() == "demo")
        {
            // check if in simulation mode OR demo mode
            if (thisRemoteFreezer.simModeBool() || thisRemoteFreezer.demoModeBool())
                thisRemoteFreezer.StopSimulationDemoMode(); // turn off demo and simulation modes
            else
                thisRemoteFreezer.StartDemoMode(); // turn on demo and simulation modes
        }
        else if (LineIn.ToLower() == "ftp")
        {
            // check if ftp enabled
            if (thisRemoteFreezer.ftpRunningBool())
                thisRemoteFreezer.DisableFTP(); // disable ftp server
            else
                thisRemoteFreezer.EnableFTP(); // enable ftp server
        }
        else if (LineIn.ToLower().StartsWith('h'))
            Console.WriteLine("Start - to Start a Manual Fill/Cool cycle\nStop - to Stop a Manual Fill/Cool cycle\nReset - to Reset Alarms and Warnings\nFTP - to Enable/Disable FTP Server\nDemo - to Start/Stop Demo Mode");

        // query freezer status over Modbus tcp
        thisRemoteFreezer.GetStatusVars();

        // status update to console
        //outString = thisRemoteFreezer.LastStatusString();
        outString = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + ", ";
        outString += thisRemoteFreezer.freezerIDString() + ", ";
        outString += thisRemoteFreezer.alarmsWarningString() + ", ";
        outString += thisRemoteFreezer.valvesCycleString() + ", ";
        outString += thisRemoteFreezer.inchesLN2Float().ToString() + " (in), ";
        outString += thisRemoteFreezer.cmLN2Float().ToString() + " (cm), ";
        outString += thisRemoteFreezer.sampleTempFloat().ToString() + " (deg C), ";
        outString += thisRemoteFreezer.domeTempFloat().ToString() + " (deg C), ";
        outString += thisRemoteFreezer.ftpRunningString();
    }
    if (thisRemoteFreezer.IsConnected() && !Connected)
        Console.WriteLine("Connected - Press Enter to Query Status, Type Help then Press Enter for More");
    else if (!thisRemoteFreezer.IsConnected())
        Console.WriteLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + ", NOT Connected!");
    else
        Console.WriteLine(outString);

    // Capture Connected history, sleep to rest processor
    Connected = thisRemoteFreezer.IsConnected();
    System.Threading.Thread.Sleep(1000);
    
}

