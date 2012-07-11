using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;

namespace BISLib
{
    public class BISLauncher
    {
        /// <summary>
        /// Triggered after a call to <see cref="StartGameAsync"/> completes
        /// </summary>
        public event EventHandler<LaunchEventArgs> StartGameCompleted = null;

        /// <summary>
        /// Starts the game specified in the <paramref name="parameters"/> 
        /// and waits for it to exit before returning
        /// </summary>
        /// <param name="parameters">
        /// The options to use for launching the game
        /// </param>
        /// <returns>
        /// The result of the call to this method
        /// </returns>
        public static LaunchEventArgs StartGame(LaunchParameters parameters)
        {
            return StartGameInternal(parameters);
        }

        /// <summary>
        /// Starts the game specified in the <paramref name="parameters"/>
        /// asynchronously, returning immediately.
        /// </summary>
        /// <param name="parameters">
        /// The options to use for launching the game
        /// </param>
        public void StartGameAsync(LaunchParameters parameters)
        {
            ThreadPool.QueueUserWorkItem(AsyncLaunchThread, parameters);
        }

        #region Asynchronous Launches

        private void AsyncLaunchThread(object context)
        {
            LaunchParameters parameters = context as LaunchParameters;

            var result = StartGameInternal(parameters);

            if (StartGameCompleted != null)
                StartGameCompleted(this, result);
        }

        #endregion


        #region Synchronous Launches

        private static LaunchEventArgs StartGameInternal(LaunchParameters parameters)
        {
            if (!parameters.Game.RequiredFunctionsPresent)
                return LaunchEventArgs.ForFailure(parameters, "Selected game has not been implemented properly. Please contact the developer of this application");

            if (!parameters.Game.IsGamePresent(parameters.LaunchType))
                return LaunchEventArgs.ForFailure(parameters, "Game executable could not be located, please ensure that the game is installed");

            LaunchTypes equivalentLaunch = parameters.Game.EquivalentLaunch(parameters.LaunchType);

            ProcessStartInfo startInfo = GetBaseStartInfo(parameters.Game, equivalentLaunch);

            string modArguments = parameters.Game.BaseMods(equivalentLaunch) ?? "";
            
            if (parameters.PreModFiltering != null)
                modArguments = parameters.PreModFiltering(modArguments) ?? "";

            List<string> searchDirectories = new List<string>();
            searchDirectories.Add(parameters.Game.InstallDirectory(parameters.LaunchType));
            searchDirectories.AddRange(parameters.AdditionalModDirectories);


            List<string> selectedMods = new List<string>();
            List<string> excludedMods = new List<string>();
            List<ModSelector> failedSelections = new List<ModSelector>();
            failedSelections.AddRange(parameters.ModSelectionFilters);

            foreach (string searchPath in searchDirectories)
            {
                IEnumerable<string> childFolders = new DirectoryInfo(searchPath).GetDirectories().Select(_ => _.Name);

                foreach (ModSelector selector in parameters.ModSelectionFilters)
                {
                    IEnumerable<string> result = selector.Match(childFolders);

                    if (parameters.SelectorOverride != null)
                        result = parameters.SelectorOverride(selector, result);

                    if (result != null && result.Count() > 0)
                        failedSelections.Remove(selector);

                    foreach (string selectedMod in result)
                        if (parameters.Game.GameFolder(selectedMod))
                            continue;
                        else if (!selector.ExclusionFilter && !selectedMods.Contains(selectedMod))
                            selectedMods.Add(selectedMod);
                        else if (selector.ExclusionFilter && !excludedMods.Contains(selectedMod))
                            excludedMods.Add(selectedMod);
                }
            }

            foreach (string exclude in excludedMods)
                if (selectedMods.Contains(exclude))
                    selectedMods.Remove(exclude);

            if (failedSelections.Count > 0)            
                if((parameters.MissingMods != null && !parameters.MissingMods(failedSelections)) || parameters.MissingMods == null)
                    return LaunchEventArgs.ForMissingMods(parameters, "Could not find some of the mods that were specified", selectedMods, excludedMods, failedSelections);


            if (parameters.ModFilter != null)
                selectedMods = new List<string>(parameters.ModFilter(selectedMods));

            if (modArguments.Length > 0 && modArguments.Last() != ';')
                modArguments += ";";

            foreach (string mod in selectedMods)
                modArguments += mod + ";";

            if (modArguments.Length > 0)
            {
                modArguments = modArguments.Remove(modArguments.Length - 1);

                //Now append the -mod to the beginning and enclose in quotation marks if necessary
                modArguments = "-mod=" + modArguments;

                if (equivalentLaunch == LaunchTypes.Steam || modArguments.Contains(' '))
                    modArguments = "\"" + modArguments + "\"";

                if (parameters.PostModFiltering != null)
                    modArguments = parameters.PostModFiltering(modArguments);

                startInfo.Arguments += modArguments;
            }

            DateTime startTime = DateTime.Now;
            Process gameInstance = Process.Start(startInfo);
            gameInstance.WaitForExit();
            DateTime closeTime = DateTime.Now;

            return LaunchEventArgs.ForSuccess(parameters, startTime, closeTime, selectedMods, excludedMods);
        }

        #endregion
        
        #region Executables
        
        private static ProcessStartInfo GetBaseStartInfo(Game game, LaunchTypes launch)
        {
            Contract.Requires<ArgumentException>(launch != LaunchTypes.Latest, "Function requires equivalent launch type to be computed first, cannot recieve 'Latest' as an option");

            ProcessStartInfo startInfo = null;

            startInfo = new ProcessStartInfo(game.ExecutableFile(launch), game.BaseArguments(launch));
            startInfo.UseShellExecute = false;
            startInfo.WorkingDirectory = game.InstallDirectory(launch);


            return startInfo;
        }

        #endregion

        #region Generators

        private static string GetAdditionalArguments(List<string> additionalArgs)
        {
            Contract.Requires<ArgumentNullException>(additionalArgs != null);

            string values = "";
            foreach (string arg in additionalArgs)
                values += arg + " ";
            return values;
        }

        #endregion
    }
}
