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
        Console.WriteLine("Miner Auto-Restarter")

        If File.Exists(ConfigFile) = False Then
            Console.WriteLine("writing initial config to " & ConfigFile)
            INIReadWrite.WriteINI(ConfigFile, "Settings", "Minerdirectory", "c:\phoenixminer\")
            INIReadWrite.WriteINI(ConfigFile, "Settings", "MinerExecName", "phoenixminer")
            INIReadWrite.WriteINI(ConfigFile, "Settings", "RestartIntervalHours", "48")
            INIReadWrite.WriteINI(ConfigFile, "Settings", "CheckMinerCrashIntervalSec", "10")
            INIReadWrite.WriteINI(ConfigFile, "Settings", "CommandLine", "-epool etc-eu1.nanopool.org:19999 -ewal 0xe8ac5095915d9c6367daa3d0d2bc146f592fd935.Rig02 -pass x ")
        End If

        Console.WriteLine("Loading Config from " & ConfigFile)
        fpath = INIReadWrite.ReadINI(ConfigFile, "Settings", "Minerdirectory")
        pname = INIReadWrite.ReadINI(ConfigFile, "Settings", "MinerExecName")
        HoursTillRestart = 1000 * 60 * 60 * INIReadWrite.ReadINI(ConfigFile, "Settings", "RestartIntervalHours")
        CheckIfStillRunningInterval = 1000 * INIReadWrite.ReadINI(ConfigFile, "Settings", "CheckMinerCrashIntervalSec")
        cmdline = INIReadWrite.ReadINI(ConfigFile, "Settings", "CommandLine")

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
        Console.ReadKey()
        GoTo 001
        startTimer.Dispose()
        restartTimer.Dispose()

    End Sub

    Private Sub StopMiner()
        Dim processList() As Process
        processList = Process.GetProcessesByName(pname)
        For Each proc As Process In processList
            proc.Kill()
            startTimer.Start()
            Console.ForegroundColor = ConsoleColor.Red
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

            Console.WriteLine(ex.Message)
            Console.WriteLine("ERROR: " & fpath & pname & " not found!")
            Return
        End Try

        Console.ForegroundColor = ConsoleColor.Red
        Console.WriteLine(Now & ": Miner (Re)started")
        Console.ResetColor()

    End Sub
    Private Sub IsRunning()
        Dim processList() As Process
        processList = Process.GetProcessesByName(pname)
        If processList.Count = 0 Then
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine(Now & ": Detected that miner is not running! Restarting it.")
            Console.ResetColor()
            StartMiner()
        End If

    End Sub

End Module
