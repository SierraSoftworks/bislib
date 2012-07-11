using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace BISLib
{
    public class Game
    {

        #region Static Game Instances

        private static Game _arma2;
        public static Game ArmA2
        {
            get
            {
                if (_arma2 == null)
                {
                    _arma2 = new Game("ArmA2");
                    _arma2.InstallDirectory = launch =>
                        {
                            switch(launch)
                            {
                                case LaunchTypes.Steam:
                                case LaunchTypes.Beta:
                                case LaunchTypes.Release:
                                case LaunchTypes.Latest:
                                    return Game.ArmA2Directory;
                                default: return null;
                            }
                        };

                    _arma2.ExecutableFile = launch =>
                        {
                            switch(launch)
                            {
                                case LaunchTypes.Steam:
                                    return Path.Combine(SteamDirectory, "Steam.exe");
                                case LaunchTypes.Release:
                                    return Path.Combine(_arma2.InstallDirectory(launch), "arma2.exe");
                                case LaunchTypes.Beta:
                                    return Path.Combine(_arma2.InstallDirectory(launch), "beta", "arma2.exe");
                                default: return null;
                            }
                        };

                    _arma2.IsGamePresent = launch =>
                        {
                            string exePath = _arma2.ExecutableFile(launch);
                            if (exePath != null && File.Exists(exePath))
                                return true;
                            return false;
                        };

                    _arma2.EquivalentLaunch = launch =>
                        {
                            if (launch != LaunchTypes.Latest)
                                return launch;

                            Version releaseVersion = GetAssemblyVersion(_arma2.ExecutableFile(LaunchTypes.Release));
                            Version betaVersion = GetAssemblyVersion(_arma2.ExecutableFile(LaunchTypes.Beta));

                            if (releaseVersion < betaVersion)
                                return LaunchTypes.Beta;
                            return LaunchTypes.Release;
                        };

                    _arma2.BaseArguments = launch =>
                        {
                            if (launch == LaunchTypes.Steam)
                                return "-applaunch 33910 ";
                            return "";
                        };

                    _arma2.BaseMods = launch =>
                        {
                            if (launch == LaunchTypes.Beta)
                                return "beta";
                            return "";
                        };

                    _arma2.GameFolder = folder =>
                        {
                            switch(folder)
                            {
                                case "AddOns":
                                case "BattlEye":
                                case "BEsetup":
                                case "beta":
                                case "Campaigns":
                                case "DirectX":
                                case "Dta":
                                case "Keys":
                                case "Missions":
                                case "MPMissions":
                                case "userconfig":
                                    return true;
                                default: return false;
                            }
                        };
                }

                return _arma2;
            }
        }

        private static Game _operationArrowhead;
        public static Game OperationArrowhead
        {
            get
            {
                if (_operationArrowhead == null)
                {
                    _operationArrowhead = new Game("Operation Arrowhead");

                    _operationArrowhead.InstallDirectory = launch =>
                    {
                        switch (launch)
                        {
                            case LaunchTypes.Steam:
                            case LaunchTypes.Beta:
                            case LaunchTypes.Release:
                            case LaunchTypes.Latest:
                                return Game.OperationArrowheadDirectory;
                            default: return null;
                        }
                    };

                    _operationArrowhead.ExecutableFile = launch =>
                    {
                        switch (launch)
                        {
                            case LaunchTypes.Steam:
                                return Path.Combine(SteamDirectory, "Steam.exe");
                            case LaunchTypes.Release:
                                return Path.Combine(_operationArrowhead.InstallDirectory(launch), "arma2oa.exe");
                            case LaunchTypes.Beta:
                                return Path.Combine(_operationArrowhead.InstallDirectory(launch), "Expansion", "beta", "arma2oa.exe");
                            default: return null;
                        }
                    };

                    _operationArrowhead.IsGamePresent = launch =>
                    {
                        string exePath = _operationArrowhead.ExecutableFile(launch);
                        if (exePath != null && File.Exists(exePath))
                            return true;
                        return false;
                    };

                    _operationArrowhead.EquivalentLaunch = launch =>
                    {
                        if (launch != LaunchTypes.Latest)
                            return launch;

                        Version releaseVersion = GetAssemblyVersion(_operationArrowhead.ExecutableFile(LaunchTypes.Release));
                        Version betaVersion = GetAssemblyVersion(_operationArrowhead.ExecutableFile(LaunchTypes.Beta));

                        if (releaseVersion < betaVersion)
                            return LaunchTypes.Beta;
                        return LaunchTypes.Release;
                    };

                    _operationArrowhead.BaseArguments = launch =>
                    {
                        if (launch == LaunchTypes.Steam)
                            return "-applaunch 33930 ";
                        return "";
                    };

                    _operationArrowhead.BaseMods = launch =>
                    {
                        if (launch == LaunchTypes.Beta)
                            return "Expansion/beta";
                        return "";
                    };

                    _operationArrowhead.GameFolder = folder =>
                    {
                        switch (folder)
                        {
                            case "AddOns":
                            case "BattlEye":
                            case "BEsetup":
                            case "Common":
                            case "Campaigns":
                            case "DirectX":
                            case "Dta":
                            case "Keys":
                            case "Missions":
                            case "MPMissions":
                            case "userconfig":
                            case "BAF":
                            case "PMC":
                                return true;
                            default: return false;
                        }
                    };
                }

                return _operationArrowhead;
            }
        }

        private static Game _combinedOperations;
        public static Game CombinedOperations
        {
            get
            {
                if (_combinedOperations == null)
                {
                    _combinedOperations = new Game("Combined Operations");

                    _combinedOperations.InstallDirectory = launch =>
                    {
                        switch (launch)
                        {
                            case LaunchTypes.Steam:
                            case LaunchTypes.Beta:
                            case LaunchTypes.Release:
                            case LaunchTypes.Latest:
                                return Game.OperationArrowheadDirectory;
                            default: return null;
                        }
                    };

                    _combinedOperations.ExecutableFile = launch =>
                    {
                        switch (launch)
                        {
                            case LaunchTypes.Steam:
                                return Path.Combine(SteamDirectory, "Steam.exe");
                            case LaunchTypes.Release:
                                return Path.Combine(_combinedOperations.InstallDirectory(launch), "arma2oa.exe");
                            case LaunchTypes.Beta:
                                return Path.Combine(_combinedOperations.InstallDirectory(launch), "Expansion", "beta", "arma2oa.exe");
                            default: return null;
                        }
                    };

                    _combinedOperations.IsGamePresent = launch =>
                    {
                        string exePath = _combinedOperations.ExecutableFile(launch);
                        if (exePath != null && File.Exists(exePath))
                            return true;
                        return false;
                    };

                    _combinedOperations.EquivalentLaunch = launch =>
                    {
                        if (launch != LaunchTypes.Latest)
                            return launch;

                        Version releaseVersion = GetAssemblyVersion(_combinedOperations.ExecutableFile(LaunchTypes.Release));
                        Version betaVersion = GetAssemblyVersion(_combinedOperations.ExecutableFile(LaunchTypes.Beta));

                        if (releaseVersion < betaVersion)
                            return LaunchTypes.Beta;
                        return LaunchTypes.Release;
                    };

                    _combinedOperations.BaseArguments = launch =>
                    {
                        if (launch == LaunchTypes.Steam)
                            return "-applaunch 33930 ";
                        return "";
                    };

                    _combinedOperations.BaseMods = launch =>
                    {
                        string baseMods = "";
                        if (launch == LaunchTypes.Beta)
                            baseMods += "Expansion/beta;";

                        baseMods += "Expansion;ca";

                        return baseMods;
                    };

                    _combinedOperations.GameFolder = folder =>
                    {
                        switch (folder)
                        {
                            case "AddOns":
                            case "BattlEye":
                            case "BEsetup":
                            case "Common":
                            case "Campaigns":
                            case "DirectX":
                            case "Dta":
                            case "Keys":
                            case "Missions":
                            case "MPMissions":
                            case "userconfig":
                            case "BAF":
                            case "PMC":
                                return true;
                            default: return false;
                        }
                    };
                }

                return _combinedOperations;
            }
        }



        #endregion


        private Game(string name)
        {
            Name = name;
        }

        public Game(string name,
            Func<LaunchTypes, bool> isGamePresent,
            Func<LaunchTypes, string> installDirectory,
            Func<LaunchTypes, string> executableFile,
            Func<LaunchTypes, LaunchTypes> equivalentLaunch,
            Func<LaunchTypes, string> baseArguments,
            Func<LaunchTypes, string> baseMods,
            Func<string, bool> isGameFolder
            )
        {
            Name = name;
            IsGamePresent = isGamePresent;
            InstallDirectory = installDirectory;
            ExecutableFile = executableFile;
            EquivalentLaunch = equivalentLaunch;
            BaseArguments = baseArguments;
            BaseMods = baseMods;
            GameFolder = isGameFolder;
        }

        /// <summary>
        /// Human readable name for the game that can be used
        /// for displaying the game in a list
        /// </summary>
        public string Name
        { get; private set; }
        
        /// <summary>
        /// Determines whether or not all of the required functions are
        /// present in this game instance for the game to be launched.
        /// </summary>
        /// <remarks>
        /// In other words, determines whether or not this is a valid
        /// game definition or whether it has been poorly implemented
        /// as a custom game (Assuming all included definitions are solid)
        /// </remarks>
        public bool RequiredFunctionsPresent
        {
            get
            {
                return
                    IsGamePresent != null &&
                    InstallDirectory != null &&
                    ExecutableFile != null &&
                    EquivalentLaunch != null &&
                    BaseArguments != null &&
                    BaseMods != null &&
                    GameFolder != null;
            }
        }

        /// <summary>
        /// Checks whether the <see cref="Game"/> is present in its default
        /// directory for the given launch condition
        /// </summary>
        public Func<LaunchTypes, bool> IsGamePresent
        { get; set; }

        /// <summary>
        /// Gets the install directory for the current <see cref="Game"/>
        /// and should return <c>null</c> if no directory could be found
        /// </summary>
        public Func<LaunchTypes, string> InstallDirectory
        { get; set; }

        /// <summary>
        /// Gets the full path to the executable file used by
        /// the <see cref="Game"/> for the given <see cref="LaunchTypes"/>
        /// </summary>
        public Func<LaunchTypes, string> ExecutableFile
        { get; set; }

        /// <summary>
        /// Converts a specified launch type into one that can be launched
        /// by the game launcher.
        /// </summary>
        /// <remarks>
        /// Generally, this is used to change a <see cref="LaunchTypes.Latest"/>
        /// value into either a <see cref="LaunchTypes.Release"/> or
        /// <see cref="LaunchTypes.Beta"/> launch type which can then
        /// be processed by the launch library.
        /// </remarks>
        public Func<LaunchTypes, LaunchTypes> EquivalentLaunch
        { get; set; }

        /// <summary>
        /// Gets the base arguments to be passed to the <see cref="ExecutableFile"/>
        /// before any mods are added.
        /// </summary>
        public Func<LaunchTypes, string> BaseArguments
        { get; set; }

        /// <summary>
        /// Gets any base mods associated with this game which
        /// should be added before any selected mods
        /// </summary>
        public Func<LaunchTypes, string> BaseMods
        { get; set; }

        /// <summary>
        /// Determines whether or not a folder is a game core
        /// folder or not to prevent core folders from being
        /// selected as 'mods'.
        /// </summary>
        public Func<string, bool> GameFolder
        { get; set; }

        #region Utility Functions
        
        /// <summary>
        /// Gets the file version for a given assembly
        /// </summary>
        /// <param name="file">The file who's version should be determined</param>
        /// <returns></returns>
        private static Version GetAssemblyVersion(string file)
        {
            FileVersionInfo fi = FileVersionInfo.GetVersionInfo(file);

            return new Version(fi.ProductVersion);
        }


        #endregion

        #region Paths

        private static string __steamDirectory = null;
        private static string SteamDirectory
        {
            get
            {
                if (__steamDirectory == null)
                {
                    try
                    {
                        if (IntPtr.Size == 4 && RegistryManager.IsPresent("HKLM\\SOFTWARE\\Valve\\Steam"))
                        {
                            __steamDirectory = RegistryManager.GetValue("HKLM\\SOFTWARE\\Valve\\Steam\\InstallPath").ToString();
                        }
                        else if (IntPtr.Size == 8 && RegistryManager.IsPresent("HKLM\\SOFTWARE\\Wow6432Node\\Valve\\Steam"))
                        {
                            __steamDirectory = RegistryManager.GetValue("HKLM\\SOFTWARE\\Wow6432Node\\Valve\\Steam\\InstallPath").ToString();
                        }
                    }
                    catch
                    { }
                }

                return __steamDirectory;
            }
        }

        private static string __arma2Directory = null;
        private static string ArmA2Directory
        {
            get
            {
                if (__arma2Directory == null)
                {
                    if (File.Exists(Path.Combine(Environment.CurrentDirectory, "arma2.exe")))
                        __arma2Directory = Environment.CurrentDirectory;

                    if (IntPtr.Size == 4 && RegistryManager.IsPresent("HKLM\\SOFTWARE\\Bohemia Interactive Studio\\ArmA 2"))
                    {
                        __arma2Directory = RegistryManager.GetValue("HKLM\\SOFTWARE\\Bohemia Interactive Studio\\ArmA 2\\Main").ToString();
                    }
                    else if (IntPtr.Size == 8 && RegistryManager.IsPresent("HKLM\\SOFTWARE\\Wow6432Node\\Bohemia Interactive Studio\\ArmA 2"))
                    {
                        __arma2Directory = RegistryManager.GetValue("HKLM\\SOFTWARE\\Wow6432Node\\Bohemia Interactive Studio\\ArmA 2\\Main").ToString();
                    }
                }
                return __arma2Directory;
            }
        }

        private static string __oaDirectory = null;
        private static string OperationArrowheadDirectory
        {
            get
            {
                if (__oaDirectory == null)
                {
                    if (File.Exists(Path.Combine(Environment.CurrentDirectory, "arma2oa.exe")))
                        __oaDirectory = Environment.CurrentDirectory;

                    if (IntPtr.Size == 4 && RegistryManager.IsPresent("HKLM\\SOFTWARE\\Bohemia Interactive Studio\\ArmA 2 OA"))
                    {
                        __oaDirectory = RegistryManager.GetValue("HKLM\\SOFTWARE\\Bohemia Interactive Studio\\ArmA 2 OA\\Main").ToString();
                    }
                    else if (IntPtr.Size == 8 && RegistryManager.IsPresent("HKLM\\SOFTWARE\\Wow6432Node\\Bohemia Interactive Studio\\ArmA 2 OA"))
                    {
                        __oaDirectory = RegistryManager.GetValue("HKLM\\SOFTWARE\\Wow6432Node\\Bohemia Interactive Studio\\ArmA 2 OA\\Main").ToString();
                    }
                }
                return __oaDirectory;
            }
        }

        #endregion
    }
}
