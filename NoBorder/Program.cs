using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Linq;

static class Program
{
    [DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)]
    private static extern IntPtr SetWindowLong32(HandleRef hwnd, WindowLongs nIndex, IntPtr dwNewLong);
    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Auto)]
    private static extern IntPtr SetWindowLongPtr64(HandleRef hWnd, WindowLongs nIndex, IntPtr dwNewLong);

    public static IntPtr SetWindowLong(HandleRef hWnd, WindowLongs nIndex, IntPtr dwNewLong)
    {
        if (IntPtr.Size == 4)
        {
            return SetWindowLong32(hWnd, nIndex, dwNewLong);
        }
        return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
    }

    [DllImport("user32.dll", EntryPoint = "GetWindowLong", CharSet = CharSet.Auto)]
    private static extern IntPtr GetWindowLong32(HandleRef hWnd, WindowLongs nIndex);
    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", CharSet = CharSet.Auto)]
    private static extern IntPtr GetWindowLongPtr64(HandleRef hWnd, WindowLongs nIndex);

    public static IntPtr GetWindowLong(HandleRef hWnd, WindowLongs nIndex)
    {
        if (IntPtr.Size == 4)
        {
            return GetWindowLong32(hWnd, nIndex);
        }
        return GetWindowLongPtr64(hWnd, nIndex);
    }

    [DllImport("user32.dll", EntryPoint = "SetWindowPos", CharSet = CharSet.Auto)]
    private static extern bool SetWindowPos(HandleRef hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);
    [DllImport("user32.dll", EntryPoint = "MoveWindow", CharSet = CharSet.Auto)]
    private static extern bool MoveWindow(HandleRef hwnd, int x, int y, int nWidth, int nHeight, bool bRepaint);
    [DllImport("user32.dll", EntryPoint = "SetForegroundWindow", CharSet = CharSet.Auto)]
    private static extern bool SetForegroundWindow(HandleRef hwnd);
    [DllImport("user32.dll", EntryPoint = "ShowWindow", CharSet = CharSet.Auto)]
    private static extern bool ShowWindow(HandleRef hwnd, int nCmdShow);

    [DllImport("user32.dll", EntryPoint = "GetClientRect", CharSet = CharSet.Auto)]
    private static extern bool GetClientRect(HandleRef hwnd, ref rect rect);
    [DllImport("user32.dll", EntryPoint = "GetWindowRect", CharSet = CharSet.Auto)]
    private static extern bool GetWindowRect(HandleRef hwnd, ref rect rect);
    [StructLayout(LayoutKind.Sequential)]
    public struct rect
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
        public override string ToString()
        {
            return string.Format("RECT: Left: {0}, Top: {1}, Right: {2}, Bottom {3}", left, top, right, bottom);
        }
    }
    
    [Flags()]
    internal enum WindowLongs : int
    {
        GWL_EXSTYLE = -20,
        GWL_STYLE = -16,
        GWL_WNDPROC = -4,
        GWL_HINSTANCE = -6,
        GWL_HWNDPARENT = -8,
        GWL_ID = -12,
        GWL_USERDATA = -21,
        DWL_DLGPROC = 4,
        DWL_MSGRESULT = 0,
        DWL_USER = 8
    }

    [Flags()]
    public enum SetWindowPosFlags : uint
    {
        SWP_ASYNCWINDOWPOS = 0x4000,
        SWP_DEFERERASE = 0x2000,
        SWP_DRAWFRAME = SWP_FRAMECHANGED,
        SWP_FRAMECHANGED = 0x20,
        SWP_HIDEWINDOW = 0x80,
        SWP_NOACTIVATE = 0x10,
        SWP_NOCOPYBITS = 0x100,
        SWP_NOMOVE = 0x2,
        SWP_NOOWNERZORDER = 0x200,
        SWP_NOREDRAW = 0x8,
        SWP_NOREPOSITION = SWP_NOOWNERZORDER,
        SWP_NOSENDCHANGING = 0x400,
        SWP_NOSIZE = 0x1,
        SWP_NOZORDER = 0x4,
        SWP_SHOWWINDOW = 0x40
    }

   [Flags()]
   public enum WindowStyles : uint
    {
      WS_BORDER = 0x00800000,
      WS_CAPTION = 0x00C00000,
      WS_CHILD = 0x40000000,
      WS_CHILDWINDOW = 0x40000000,
      WS_CLIPCHILDREN = 0x02000000,
      WS_CLIPSIBLINGS = 0x04000000,
      WS_DISABLED = 0x08000000,
      WS_DLGFRAME = 0x00400000,
      WS_GROUP = 0x00020000,
      WS_HSCROLL = 0x00100000,
      WS_ICONIC = 0x20000000,
      WS_MAXIMIZE = 0x01000000,
      WS_MAXIMIZEBOX = 0x00010000,
      WS_MINIMIZE = 0x20000000,
      WS_MINIMIZEBOX = 0x00020000,
      WS_OVERLAPPED = 0x00000000,
      WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
      WS_POPUP = 0x80000000,
      WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,
      WS_SIZEBOX = 0x00040000,
      WS_SYSMENU = 0x00080000,
      WS_TABSTOP = 0x00010000,
      WS_THICKFRAME = 0x00040000,
      WS_TILED = 0x00000000,
      WS_TILEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
      WS_VISIBLE = 0x10000000,
      WS_VSCROLL = 0x00200000
    }

    const int mandatoryArgs = 3;
    const char start = '/';
    const char delimiter = ':';
    static string startID;
    static string processName;
    static int width;
    static int height;
    static HandleRef ownhWnd;
    static int delay;
    static bool toFront;
    static string startArguments;
    static int PID;
    private static System.Timers.Timer timer = new System.Timers.Timer();
    static int repeat;
    static int counter;
    static string windowTitle;
    static bool equalsSize;
    static int x;
    static int y;
    public static void Main(string[] args)
    {
        timer.Elapsed += Execute;
        HideMe();
#if DEBUG
        //brink 1920 1200 /b /f:60 /e:"E:\Games\Steam\steam.exe" /ea:"-applaunch 22350"
        //E:\Games\NoBorder.exe E:\Games\Steam\steamapps\common\skyrim\skse_loader.exe 2560 1440 /F:60 /P:TESV
        args = new string[10];
        args[0] = @"E:\Games\Steam\steamapps\common\skyrim\SkyrimLauncher.exe";
        //args[0] = @"C:\Windows\system32\calc.exe";
        args[1] = "2560";
        args[2] = "1440";                                            
        args[3] = "/F:60";
        //args[4] = "/p:calc";
        args[4] = "/P:TESV";
        //args(5) = "/p:E:\Games\Microsoft Games\Rise Of Legends\legends.exe"
        //args(6) = "/sa:-applaunch 22350"
        //args[7] = "/s";
        //args[8] = "/x:0";
        //args[9] = "/y:0";
#endif
        try
        {
            SetOption(args.Where(arg => !string.IsNullOrWhiteSpace(arg) && arg.ToLower() == start + "help" || arg == start + "?").Count() >= 1 ? "?" : "", "");
            ParseArguments(args);
            ParseOptions(args);

            if (IsValidPath(startID))
            {
                startID = System.IO.Path.GetFullPath(startID);
                using (System.Diagnostics.Process proc = new System.Diagnostics.Process { StartInfo = new ProcessStartInfo { Arguments = startArguments, FileName = startID, WorkingDirectory = System.IO.Path.GetDirectoryName(startID) } })
                {
                    proc.Start();
                    if (string.IsNullOrEmpty(processName))
                        processName = proc.ProcessName;
                }
            }
            else if (!IsPath(startID))
            {
                processName = startID;
            }
            else if (IsPath(startID) && !System.IO.File.Exists(startID))
            {
                WriteError("File not found, check the supplied startID, processnames don't require an extension like \".exe\".");
            }

            System.Threading.Thread.Sleep(delay);
            timer.Interval = 1000;
            timer.Start();
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            WriteError("Microsoft, " + ex.Message);
        }
    }

    public static bool IsValidPath(string path)
    {
        return IsPath(path) && System.IO.File.Exists(path);
    }

    public static bool IsPath(string path)
    {
        return new System.Text.RegularExpressions.Regex(@"(([A-Z]:\\)?[^/:\*\?<>\|]+\.\w{2,6})|(\\{2}[^/:\*\?<>\|]+\.\w{2,6})").IsMatch(path);
    }

    public static bool SetOption(string key, string value)
    {
        try
        {
            if (string.IsNullOrEmpty(value))
            {
                switch (key)
                {
                    case "?":
                        WriteHelp();
                        return true;
                    case "b":
                        toFront = true;
                        return true;
                    case "s":
                        equalsSize = true;
                        return true;
                    default:
                        return false;
                }
            }
            else
            {
                switch (key)
                {
                    case "d":
                        delay = int.Parse(value) * 1000;
                        break;
                    case "sa":
                        startArguments = value;
                        break;
                    case "f":
                        repeat = int.Parse(value);
                        break;
                    case "t":
                        windowTitle = value;
                        break;
                    case "x":
                        x = int.Parse(value);
                        break;
                    case "y":
                        y = int.Parse(value);
                        break;
                    case "p":
                        processName = value;
                        break;
                    case "pid":
                        PID = int.Parse(value);
                        break;
                }
            }
        }
        catch (InvalidCastException ex)
        {
            WriteError(string.Format("in option \"{0}\"", ex.Message));
        }
        return false;
    }

    public static Dictionary<string, string> CreateOptions()
    {
        Dictionary<string, string> dict = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        dict.Add("help", "?");
        dict.Add("delay", "d");
        dict.Add("bringtofront", "b");
        dict.Add("startarguments", "sa");
        dict.Add("autofindwindow", "f");
        dict.Add("gamewindowtitle", "t");
        dict.Add("equalssize", "s");
        dict.Add("x", "x");
        dict.Add("y", "y");
        dict.Add("processname", "p");
        dict.Add("processid", "pid");
        return dict;
    }

    public static void ParseArguments(string[] arguments)
    {
        if (arguments.Length < mandatoryArgs)
            WriteError("Not all arguments are specified");

        arguments = arguments.Take(mandatoryArgs).Where(arg => !string.IsNullOrWhiteSpace(arg)).ToArray();
        if (arguments.Where(arg => arg.StartsWith(start.ToString())).Count() > 0)
            WriteError(string.Format("First {0} arguments cannot be options, see usage", mandatoryArgs));

        try
        {
            for (int i = 0; i <= mandatoryArgs; i++)
            {
                switch (i)
                {
                    case 0:
                        startID = arguments[i];
                        break;
                    case 1:
                        width = int.Parse(arguments[i]);
                        break;
                    case 2:
                        height = int.Parse(arguments[i]);
                        break;
                    default:
                        break;
                }
            }
        }
        catch (InvalidCastException ex)
        {
            WriteError(string.Format("in argument \"{0}\"", ex.Message));
        }

    }

    public static void ParseOptions(string[] arguments)
    {
        Dictionary<string, string> options = CreateOptions();
        arguments = arguments.Skip(mandatoryArgs).Where(arg => !string.IsNullOrWhiteSpace(arg)).ToArray();

        foreach (string opt in arguments)
        {
            string[] split = opt.Split(new char[] { delimiter }, 2);

            string key = split[0].Trim().ToLower();
            if (key.StartsWith(start.ToString())) key = key.Remove(0, 1); else WriteError(string.Format("option \"{0}\" is unknown", key));
            
            if (options.ContainsKey(key)) key = options[key];
            if (!options.ContainsValue(key)) WriteError(string.Format("option \"{0}{1}\" is unknown", start, key));

            string value = split.Length == 2 ? split[1].Trim() : null;

            if (!SetOption(key, value))
            {
                if (value == null)
                {
                    WriteError(string.Format("option \"{0}{1}\" needs a value but has none.", start, key));
                }
                else if (value.Length == 0)
                {
                    WriteError(string.Format("option \"{0}{1}\" has a delimiter but no value.", start, key));
                }
            }
            else if (split.Length == 2)
            {
                WriteError(string.Format("option \"{0}{1}\" has a delimiter but no need for a value.", start, key));
            }
        }
    }

    public static void Execute(object source, System.Timers.ElapsedEventArgs e)
    {
        if (repeat >= counter)
        {
            if (Process.GetProcessesByName(processName).Length > 0 && !(Process.GetProcessesByName(processName)[0].MainWindowHandle == IntPtr.Zero))
            {
                rect rectangle = default(rect);
                HandleRef hwnd = new HandleRef(Process.GetProcessesByName(processName)[0], Process.GetProcessesByName(processName)[0].MainWindowHandle);
                GetClientRect(hwnd, ref rectangle);
                if ((!equalsSize || rectangle.right / 100 * 102 >= width && rectangle.bottom / 100 * 105 >= height) && (string.IsNullOrEmpty(windowTitle) || Process.GetProcessesByName(processName)[0].MainWindowTitle.Contains(windowTitle)))
                {
                    timer.Stop();
                    MoveWindow(hwnd, x, y, width, height, true);
                    SetWindowLong(hwnd, WindowLongs.GWL_STYLE, new IntPtr(GetWindowLong(hwnd, WindowLongs.GWL_STYLE).ToInt32() & (uint)(~WindowStyles.WS_CAPTION & ~WindowStyles.WS_THICKFRAME)));
                    SetWindowPos(hwnd, -2, x, y, width, height, SetWindowPosFlags.SWP_NOACTIVATE | SetWindowPosFlags.SWP_ASYNCWINDOWPOS);
                    MoveWindow(hwnd, x, y, width, height, true);
                    if (toFront)
                        SetForegroundWindow(hwnd);
                    System.Environment.Exit(0);
                }
            }
        }
        else
        {
            timer.Stop();
            WriteError("Process name not found.");
        }
        counter++;
    }

    public static void ShowMe()
    {
        ShowWindow(ownhWnd, 9);
    }

    public static void HideMe()
    {
        ownhWnd = new HandleRef(Process.GetCurrentProcess(), Process.GetCurrentProcess().MainWindowHandle);
        ShowWindow(ownhWnd, 0);
    }

    public static void WriteError(string err)
    {
        ShowMe();
        Console.WriteLine("");
        Console.WriteLine("Error: " + err);
        Console.WriteLine("");
        BasicHelp();
        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
        System.Environment.Exit(0);
    }

    public static void BasicHelp()
    {
        Console.WriteLine(" -----------------------------------------------------------------------------");
        Console.WriteLine("usage: NOBORDER startID width heigth [options]");
        Console.WriteLine("");
        Console.WriteLine(" startID: This can be a (relative)path or a processname (see the taskmanager)");
        Console.WriteLine(" width: Your horizontal resolution e.g. 1920 or 1024.");
        Console.WriteLine(" height: Your vertical resolution e.g. 1200 or 768.");
        Console.WriteLine("");
        Console.WriteLine(" -----------------------------------------------------------------------------");
        Console.WriteLine("     options:");
        Console.WriteLine("");
        Console.WriteLine("         {/DELAY|/D}:seconds");
        Console.WriteLine("         {/BRINGTOFRONT|/B}");
        Console.WriteLine("         {/AUTOFINDWINDOW|/F}:seconds");
        Console.WriteLine("         {/WINDOWTITLE|/T}:\"title\"");
        Console.WriteLine("         {/PROCESSNAME|/P}:\"processname\"");
        Console.WriteLine("         {/PROCESSID|/PID}:processid");
        Console.WriteLine("         {/STARTARGUMENTS|/SA}:\"arguments\"");
        Console.WriteLine("         {/EQUALSSIZE|/S}");
        Console.WriteLine("         {/X}:x coördinate");
        Console.WriteLine("         {/Y}:y coördinate");
        Console.WriteLine("         {/HELP|/?}");
        Console.WriteLine("");
        Console.WriteLine(" -----------------------------------------------------------------------------");
    }

    public static void WriteHelp()
    {
        ShowMe();
        Console.WriteLine("");
        Console.WriteLine("Create a shortcut to NoBorder.exe, go to the properties of the shortcut, write   the necesarry arguments and your preferred options  after the NoBorder.exe path contained in the \"Target:\" box. See usage for more info.");
        Console.WriteLine("");
        BasicHelp();
        Console.WriteLine("                                 -EXAMPLES-");
        Console.WriteLine("");
        Console.WriteLine(" \"C:\\Folder\\NoBorder.exe\" Fallout3 1920 1200 /DELAY:5");
        Console.WriteLine(" \"C:\\Folder\\NoBorder.exe\" Fallout3 1920 1200 /D:5 /BRINGTOFRONT");
        Console.WriteLine("");
        Console.WriteLine(" \"C:\\Folder\\NoBorder.exe\" \"Bethesda Softworks\\Fallout 3\\fallout3.exe\" 1920 1200   /D:5 /B");
        Console.WriteLine("");
        Console.WriteLine(" \"C:\\Folder\\NoBorder.exe\" \"C:\\Program Files\\Steam\\steam.exe\" 1920 1080 /D:5 /B /P:  \"iw4mp\" /SA:\" -applaunch 10190\"");
        Console.WriteLine("");
        Console.WriteLine(" -----------------------------------------------------------------------------");
        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
        System.Environment.Exit(0);
    }




}
