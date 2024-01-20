
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
                    ElseIf LineIn.ToLower.StartsWith("set") Then
                        Dim value As String
                        Dim fvalue As Single
                        If LineIn.ToLower.StartsWith("set ln2high ") Then
                            value = LineIn.ToLower.Replace("set ln2high ", "")
                            If Single.TryParse(value, fvalue) Then
                                thisRemoteFreezer.setLN2MaxSpt(fvalue)
                            End If
                        ElseIf LineIn.ToLower.StartsWith("set ln2stop ") Then
                            value = LineIn.ToLower.Replace("set ln2stop ", "")
                            If Single.TryParse(value, fvalue) Then
                                thisRemoteFreezer.setLN2HighSpt(fvalue)
                            End If
                        ElseIf LineIn.ToLower.StartsWith("set ln2start ") Then
                            value = LineIn.ToLower.Replace("set ln2start ", "")
                            If Single.TryParse(value, fvalue) Then
                                thisRemoteFreezer.setLN2LowSpt(fvalue)
                            End If
                        ElseIf LineIn.ToLower.StartsWith("set ln2low ") Then
                            value = LineIn.ToLower.Replace("set ln2low ", "")
                            If Single.TryParse(value, fvalue) Then
                                thisRemoteFreezer.setLN2MinSpt(fvalue)
                            End If
                        ElseIf LineIn.ToLower.StartsWith("set temphigh ") Then
                            value = LineIn.ToLower.Replace("set temphigh ", "")
                            If Single.TryParse(value, fvalue) Then
                                thisRemoteFreezer.setHighTempSpt((fvalue * 10.0F))
                            End If
                        ElseIf LineIn.ToLower.StartsWith("set templow ") Then
                            value = LineIn.ToLower.Replace("set templow ", "")
                            If Single.TryParse(value, fvalue) Then
                                thisRemoteFreezer.setLowTempSpt((fvalue * 10.0F))
                            End If
                        End If
                        Console.WriteLine("LN2 High Spt= " + thisRemoteFreezer.getLN2MaxSpt().ToString())
                        Console.WriteLine("LN2 Stop Spt= " + thisRemoteFreezer.getLN2HighSpt().ToString())
                        Console.WriteLine("LN2 Start Spt= " + thisRemoteFreezer.getLN2LowSpt().ToString())
                        Console.WriteLine("LN2 Low Spt= " + thisRemoteFreezer.getLN2MinSpt().ToString())
                        Console.WriteLine("Temp High Spt= " + (thisRemoteFreezer.getTempHighSpt() / 10.0F).ToString())
                        Console.WriteLine("Temp Low Spt= " + (thisRemoteFreezer.getTempLowSpt() / 10.0F).ToString())

                    ElseIf LineIn.ToLower.StartsWith("h") Then
                        Console.WriteLine("Start - to Start a Manual Fill/Cool cycle")
                        Console.WriteLine("Stop - to Stop a Manual Fill/Cool cycle")
                        Console.WriteLine("Reset - to Reset Alarms and Warnings")
                        Console.WriteLine("FTP - to Enable/Disable FTP Server")
                        Console.WriteLine("Demo - to Start/Stop Demo Mode")
                        Console.WriteLine("Set - to View Settings")
                        Console.WriteLine("Set LN2High value - to set LN2 Alarm Level (high)")
                        Console.WriteLine("Set LN2Stop value - to set LN2 Stop Level")
                        Console.WriteLine("Set LN2Start value - to set LN2 Start Level")
                        Console.WriteLine("Set LN2Low value - to set LN2 Alarm Level (low)")
                        Console.WriteLine("Set TempHigh value - to set TempA Alarm Level (high)")
                        Console.WriteLine("Set TempLow value - to set TempB Alarm Level (low)")
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
