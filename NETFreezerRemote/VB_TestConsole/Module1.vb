
Imports System.Net
Imports FreezerRemote
Module Module1



    Sub Main()
        'initialize Single freezer status variable for user input And modbus tcp connected status
        Dim LineIn As String = ""
        Dim Connected As Boolean = False
        Dim thisRemoteFreezer As FreezerRemoteClass = New FreezerRemoteClass()
        Dim addrIn As IPAddress
        Dim outString As String = ""

        'loop forever
        While True
            Try
                'connect, If Not connected
                If Not thisRemoteFreezer.IsConnected Then

                    Console.WriteLine("Type an IP Address then Press Enter to Connect:")
                    ' read user input from console
                    LineIn = Console.ReadLine()

                    ' parse input / action on input
                    If Not (LineIn = "") Then

                        If (IPAddress.TryParse(LineIn, addrIn)) Then

                            thisRemoteFreezer.ipAddr = addrIn.ToString()

                        End If
                    End If


                    Console.WriteLine("Connecting..." + thisRemoteFreezer.ipAddr)
                    ' try to connect
                    thisRemoteFreezer.Connect()

                Else
                    'if connected, parse, query, report
                    'read user input from console
                    LineIn = Console.ReadLine()

                    If LineIn = "" Then
                        ' Do Nothing
                    ElseIf LineIn.ToLower = "exit" Then
                        Exit While ' Exit loop close program
                    ElseIf LineIn.ToLower = "start" Then
                        thisRemoteFreezer.StartFill() ' start fill using dll
                    ElseIf LineIn.ToLower = "stop" Then
                        thisRemoteFreezer.StopFill() ' stop fill using dll
                    ElseIf LineIn.ToLower = "reset" Then
                        thisRemoteFreezer.ResetAlarmsWarnings() ' reset alarms/warning using dll
                    ElseIf LineIn.ToLower = "demo" Then
                        ' check if simulation mode or demo mode enabled
                        If (thisRemoteFreezer.simModeBool() Or
                            thisRemoteFreezer.demoModeBool()) Then
                            thisRemoteFreezer.StopSimulationDemoMode() ' turn off simulation mode and demo mode
                        Else
                            thisRemoteFreezer.StartDemoMode() ' turn on simulation mode and demo mode
                        End If
                    ElseIf LineIn.ToLower = "ftp" Then
                        ' check if ftp enabled
                        If (thisRemoteFreezer.ftpRunningBool()) Then
                            thisRemoteFreezer.DisableFTP() ' disable ftp server
                        Else
                            thisRemoteFreezer.EnableFTP() ' enable ftp server
                        End If
                    ElseIf LineIn.ToLower.StartsWith("h") Then
                        Console.WriteLine("Start - to Start a Manual Fill/Cool cycle")
                        Console.WriteLine("Stop - to Stop a Manual Fill/Cool cycle")
                        Console.WriteLine("Reset - to Reset Alarms and Warnings")
                        Console.WriteLine("FTP - to Enable/Disable FTP Server")
                        Console.WriteLine("Demo - to Start/Stop Demo Mode")
                    End If
                    'query freezer status over Modbus tcp
                    thisRemoteFreezer.GetStatusVars()

                    'status update to console
                    'outString = thisRemoteFreezer.LastStatusString()
                    outString = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + ", "
                    outString += thisRemoteFreezer.freezerIDString() + ", "
                    outString += thisRemoteFreezer.alarmsWarningString() + ", "
                    outString += thisRemoteFreezer.valvesCycleString() + ", "
                    outString += thisRemoteFreezer.inchesLN2Float().ToString() + " (in), "
                    outString += thisRemoteFreezer.cmLN2Float().ToString() + " (cm), "
                    outString += thisRemoteFreezer.sampleTempFloat().ToString() + " (deg C), "
                    outString += thisRemoteFreezer.domeTempFloat().ToString() + " (deg C), "
                    outString += thisRemoteFreezer.ftpRunningString()

                End If
                If (thisRemoteFreezer.IsConnected And
                    Not Connected) Then
                    Console.WriteLine("Connected - Press Enter to Query Status, Type Help then Press Enter for More")
                ElseIf Not thisRemoteFreezer.IsConnected Then
                    Console.WriteLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + ", NOT Connected!")
                Else
                    Console.WriteLine(outString)
                End If


            Catch ex As Exception
                Console.WriteLine("Error")
            End Try

            'Capture Connected history, sleep to rest processor
            Connected = thisRemoteFreezer.IsConnected()
            System.Threading.Thread.Sleep(1000)

        End While

    End Sub

End Module
