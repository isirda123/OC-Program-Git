using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;

public class SteamLauncher : MonoBehaviour
{
    string[] games = { "pCars2", "NoLimits2", "prorace", "xplane" };

    void    runNoLimitsBat()
    {
        String myBatchFileName = Application.dataPath + "/../Game/Run NoLimits.bat";
        Process p = new Process();
        p.StartInfo.FileName = myBatchFileName;
        p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        p.Start();
    }

    public string id;
    public string ProcessName = "#UseLess";
    Process optitool_proc;
    Process game_proc;
    bool quit;
    // Use this for initialization
    void Awake()
    {
        var optitoolPath = string.Empty;
        var isSteamGame = false;
        var steamGameUrl = string.Empty;
        var gamePath = string.Empty;
        var arguments = string.Empty;

        var args = Environment.GetCommandLineArgs();

        int game = -1;
        StreamWriter SW = new StreamWriter(Application.dataPath + "/../Arguments");
        int i;
        foreach (string arg in args)
        {
            SW.WriteLine(arg);
            SW.Flush();
            i = 0;
            foreach (string str in games)
            {
                if (str == arg)
                    game = i;
                i++;
            }
        }
        SW.Close();

        switch (game)
        {
            case 0:
                ProcessName = "pCARS2AVX";
                optitoolPath = Application.dataPath + "/../Game/ConsoleApp1.exe";
                steamGameUrl = "steam://launch/378860/othervr";
                isSteamGame = true;
                break;
            case 1:
                runNoLimitsBat();
                ProcessName = "nolimits2app";
                //optitoolPath = Application.dataPath + "/../Game/Nolimits2telemetry/Nolimitstelemetry.exe";
                //gamePath = Application.dataPath + "/../Game/NoLimits 2/64bit/nolimits2app.exe";
                //arguments = "--telemetry";
                break;
            case 2:
                gamePath = Application.dataPath + "/../Game/DIVE v0.22/DIVE.exe";
                break;
            case 3:
                ProcessName = "X-Plane";
                gamePath = Application.dataPath + "/../X-Plane11/X-Plane.exe";
                optitoolPath = Application.dataPath + "/../Optitool/ConsoleApp1.exe";
                break;
            default:
                ProcessName = "pCARS2AVX";
                optitoolPath = Application.dataPath + "/../Game/ConsoleApp1.exe";
                steamGameUrl = "steam://launch/378860/othervr";
                isSteamGame = true;
                break;
        }
        UnityEngine.Debug.LogError(optitoolPath);
        UnityEngine.Debug.LogError(gamePath);
        // UnityEngine.Debug.LogError(optitool_proc.StartInfo.FileName);

        #region Lancement OptiTool
        if (!string.IsNullOrEmpty(optitoolPath))
        {
            optitool_proc = new Process();
            optitool_proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            optitool_proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(optitoolPath);
            optitool_proc.StartInfo.FileName = optitoolPath;
            optitool_proc.Start();
            Thread.Sleep(300);
        }
        #endregion



        if (game == 1)
        {
            Process[] ps = null;
            while (true)
            {
                ps = Process.GetProcessesByName(ProcessName);
                if (ps != null)
                    break;
                UnityEngine.Debug.LogError("L:121 - wait to find process name");
            }
            StartCoroutine(WaitForSteamProcess());
        }
        else
        {
            if (isSteamGame)
            {
                Application.OpenURL(steamGameUrl);
                StartCoroutine(WaitForSteamProcess());
            }
            else
            {
                UnityEngine.Debug.LogError(gamePath);
                game_proc = new Process();
                game_proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(gamePath);
                game_proc.StartInfo.FileName = gamePath;
                game_proc.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;

                //UnityEngine.Debug.LogError(game_proc.StartInfo.FileName);
                game_proc.Exited += P_Exited;
                game_proc.StartInfo.Arguments = arguments;
                game_proc.Start();
            }
        }
    }

    private void Update()
    {
        if (quit)
            Application.Quit();
    }


    /// <summary>
    /// When game is Steam Game
    /// </summary>
    /// <returns></returns>
    public IEnumerator WaitForSteamProcess()
    {
        yield return new WaitForSeconds(10);
        var ps = Process.GetProcessesByName(ProcessName);

        UnityEngine.Debug.LogError(ProcessName);
        game_proc = ps.FirstOrDefault();
        if (game_proc != null)
        {
            game_proc.EnableRaisingEvents = true;
            UnityEngine.Debug.LogError("process found");

            game_proc.Exited += P_Exited;

            Thread t = new Thread(new ThreadStart(exitThread));
            t.Start();

        }
        else
        {
            UnityEngine.Debug.LogError("process not found");
            Application.Quit();
        }
    }

    void exitThread()
    {
        UnityEngine.Debug.LogError("exe exited");
        game_proc.WaitForExit();
        P_Exited(null, null);
    }

    private void P_Exited(object sender, System.EventArgs e)
    {
        UnityEngine.Debug.LogError("game quit");
        game_proc = null;
        quit = true;
    }

    // Update is called once per frame
    void OnApplicationQuit()
    {
        UnityEngine.Debug.LogError("Application quit");
        if (game_proc != null)
        {
            try
            {
                UnityEngine.Debug.LogError("process inst found");
                game_proc.Kill();
                game_proc = null;
            }
            catch (System.Exception)
            {
                UnityEngine.Debug.LogError("process exception");
            }
        }
        if (optitool_proc != null)
        {
            try
            {
                UnityEngine.Debug.LogError("optitool found");
                optitool_proc.Kill();
                optitool_proc = null;
            }
            catch (System.Exception)
            {
                UnityEngine.Debug.LogError("optitool exception");
            }
        }
    }
}
