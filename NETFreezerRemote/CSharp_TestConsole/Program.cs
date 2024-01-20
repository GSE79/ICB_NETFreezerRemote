
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
        else if (LineIn.ToLower().StartsWith("set")) // "set" - view settings 
        {
            string value;
            float fvalue;
            if (LineIn.ToLower().StartsWith("set ln2high ")) // "set" - ln2 high alarm level 
            {
                value = LineIn.ToLower().Replace("set ln2high ", "");
                if (float.TryParse(value, out fvalue)) 
                {
                    thisRemoteFreezer.setLN2MaxSpt(fvalue);
                }
            }
            else if (LineIn.ToLower().StartsWith("set ln2stop ")) // "set" - ln2 stop level 
            {
                value = LineIn.ToLower().Replace("set ln2stop ", "");
                if (float.TryParse(value, out fvalue))
                {
                    thisRemoteFreezer.setLN2HighSpt(fvalue);
                }
            }
            else if (LineIn.ToLower().StartsWith("set ln2start ")) // "set" - ln2 start level 
            {
                value = LineIn.ToLower().Replace("set ln2start ", "");
                if (float.TryParse(value, out fvalue))
                {
                    thisRemoteFreezer.setLN2LowSpt(fvalue);
                }
            }
            else if (LineIn.ToLower().StartsWith("set ln2low ")) // "set" - ln2 low alarm level 
            {
                value = LineIn.ToLower().Replace("set ln2low ", "");
                if (float.TryParse(value, out fvalue))
                {
                    thisRemoteFreezer.setLN2MinSpt(fvalue);
                }
            }
            else if (LineIn.ToLower().StartsWith("set temphigh ")) // "set" - tempA high alarm level 
            {
                value = LineIn.ToLower().Replace("set temphigh ", "");
                if (float.TryParse(value, out fvalue))
                {
                    thisRemoteFreezer.setHighTempSpt((Int16)(fvalue*10.0f));
                }
            }
            else if (LineIn.ToLower().StartsWith("set templow ")) // "set" - tempA low alarm level 
            {
                value = LineIn.ToLower().Replace("set templow ", "");
                if (float.TryParse(value, out fvalue))
                {
                    thisRemoteFreezer.setLowTempSpt((Int16)(fvalue * 10.0f));
                }
            }

            Console.WriteLine("LN2 High Spt= " + thisRemoteFreezer.getLN2MaxSpt().ToString());
            Console.WriteLine("LN2 Stop Spt= " + thisRemoteFreezer.getLN2HighSpt().ToString());
            Console.WriteLine("LN2 Start Spt= " + thisRemoteFreezer.getLN2LowSpt().ToString());
            Console.WriteLine("LN2 Low Spt= " + thisRemoteFreezer.getLN2MinSpt().ToString());
            Console.WriteLine("Temp High Spt= " + (thisRemoteFreezer.getTempHighSpt()/10.0f).ToString());
            Console.WriteLine("Temp Low Spt= " + (thisRemoteFreezer.getTempLowSpt()/10.0f).ToString());

        }
        else if (LineIn.ToLower().StartsWith('h'))
        {
            Console.WriteLine("Start - to Start a Manual Fill/Cool cycle");
            Console.WriteLine("Stop - to Stop a Manual Fill/Cool cycle");
            Console.WriteLine("Reset - to Reset Alarms and Warnings");
            Console.WriteLine("FTP - to Enable/Disable FTP Server");
            Console.WriteLine("Demo - to Start/Stop Demo Mode");
            Console.WriteLine("Set - to View Settings");
            Console.WriteLine("Set LN2High value - to set LN2 Alarm Level (high)");
            Console.WriteLine("Set LN2Stop value - to set LN2 Stop Level");
            Console.WriteLine("Set LN2Start value - to set LN2 Start Level");
            Console.WriteLine("Set LN2Low value - to set LN2 Alarm Level (low)");
            Console.WriteLine("Set TempHigh value - to set TempA Alarm Level (high)");
            Console.WriteLine("Set TempLow value - to set TempB Alarm Level (low)");
        }
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

