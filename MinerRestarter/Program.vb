Imports System.IO

Module Program
    Dim fpath As String = "C:\PhoenixMiner\"
    Dim pname As String = "phoenixminer.exe "
    Dim ConfigFile As String = CurDir() & "\ConfigMinerRestarter.ini"
    Dim HoursTillRestart As Integer = 1000 * 60 * 60 * 36
    Dim CheckIfStillRunningInterval As Integer = 1000 * 10
    Dim cmdline As String = ""

    Dim restartTimer As New Timers.Timer
    Dim startTimer As New Timers.Timer
    Dim startIfCrash As New Timers.Timer


    Sub Main(args As String())

        Dim DText As String
            DText = "Miner Auto Restarter!" & vbCrLf
            DText += "If you like this program or think it is usefull please feel free to donate" & vbCrLf
            DText += "ETC : 0xe8ac5095915d9c6367daa3d0d2bc146f592fd935" & vbCrLf
            DText += "SHIB: 0xC3cC2cC679492A78987B635eA6Ed574786Dd7B5b" & vbCrLf
            DText += "ETH : 0xB4636abA7e7B5Fbfc55836687DFa01c654953Ad7" & vbCrLf
            DText += "RVN : REgqA7YDskPaadAXcjpDj1cGkFo5HeQUmo" & vbCrLf
        DText += "Cheers DutchKingCobra" & vbCrLf

        Console.WriteLine(DText)
        If File.Exists("README-Miner Auto Restarter.txt") = False Then
            File.WriteAllText("README-Miner Auto Restarter.txt", DText)
        End If

        If File.Exists(ConfigFile) = False Then
            Console.WriteLine("writing initial config to " & ConfigFile)
            INIReadWrite.WriteINI(ConfigFile, "Settings", "Minerdirectory", "c:\phoenixminer\")
            INIReadWrite.WriteINI(ConfigFile, "Settings", "MinerExecName", "phoenixminer")
            INIReadWrite.WriteINI(ConfigFile, "Settings", "RestartIntervalHours", "48")
            INIReadWrite.WriteINI(ConfigFile, "Settings", "CheckMinerCrashIntervalSec", "10")
            INIReadWrite.WriteINI(ConfigFile, "Settings", "CommandLine", "-epool etc-eu1.nanopool.org:19999 -ewal 0xe8ac5095915d9c6367daa3d0d2bc146f592fd935.Rig02 -pass x ")
            Console.WriteLine("Config written! please edit " & ConfigFile & " to match your needs and restart.")
            Console.WriteLine("Press a key to close.")
            Console.ReadKey()
            End
        End If

        Console.WriteLine("Loading Config from " & ConfigFile)
        fpath = INIReadWrite.ReadINI(ConfigFile, "Settings", "Minerdirectory")
        pname = INIReadWrite.ReadINI(ConfigFile, "Settings", "MinerExecName")
        HoursTillRestart = 1000 * 60 * 60 * INIReadWrite.ReadINI(ConfigFile, "Settings", "RestartIntervalHours")
        CheckIfStillRunningInterval = 1000 * INIReadWrite.ReadINI(ConfigFile, "Settings", "CheckMinerCrashIntervalSec")
        cmdline = INIReadWrite.ReadINI(ConfigFile, "Settings", "CommandLine")

        If cmdline.Length = 0 Then
            Console.ForegroundColor = ConsoleColor.Yellow
            Console.WriteLine("INFO: No command line specified make sure the miner gets it from a file like ""epools.txt"" or add a command line in " & ConfigFile)
            Console.ResetColor()
        End If

        startTimer.Interval = 3000
        AddHandler startTimer.Elapsed, AddressOf StartMiner

        restartTimer.Interval = HoursTillRestart
        AddHandler restartTimer.Elapsed, AddressOf StopMiner

        startIfCrash.Interval = CheckIfStillRunningInterval
        AddHandler startIfCrash.Elapsed, AddressOf IsRunning

        Dim processList() As Process
        processList = Process.GetProcessesByName(pname)
        If processList.Count = 0 Then
            startTimer.Start()
        Else
            StopMiner()
        End If

        restartTimer.Start()
        startIfCrash.Start()
001:
        Console.WriteLine("q: to Close" & vbCrLf)
        Dim input As String = Console.ReadLine
        If input = "q" Then
            startTimer.Dispose()
            restartTimer.Dispose()
            startIfCrash.Dispose()
            Exit Sub
        End If
        Console.ReadKey()
        GoTo 001


    End Sub

    Private Sub StopMiner()
        Dim processList() As Process
        processList = Process.GetProcessesByName(pname)
        For Each proc As Process In processList
            proc.Kill()
            startTimer.Start()
            Console.ForegroundColor = ConsoleColor.Yellow
            Console.WriteLine(Now & ": Miner Stopped for scheduled restart")
            Console.ResetColor()
        Next
    End Sub
    Private Sub StartMiner()
        Try
            Dim p As Process = New Process()
            p.StartInfo.UseShellExecute = True
            p.StartInfo.FileName = fpath & pname
            p.StartInfo.Arguments = cmdline
            p.Start()
            startTimer.Stop()
        Catch ex As Exception
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine(ex.Message)
            Console.WriteLine("ERROR: " & fpath & pname & " not found!")
            Console.ResetColor()
            Return
        End Try

        Console.ForegroundColor = ConsoleColor.Green
        Console.WriteLine(Now & ": Miner (Re)started")
        Console.ResetColor()

    End Sub
    Private Sub IsRunning()
        Dim processList() As Process
        processList = Process.GetProcessesByName(pname)
        If processList.Count = 0 Then
            Console.ForegroundColor = ConsoleColor.Blue
            Console.WriteLine(Now & ": Detected that miner is not running! Restarting it.")
            Console.ResetColor()
            StartMiner()
        End If

    End Sub

End Module
