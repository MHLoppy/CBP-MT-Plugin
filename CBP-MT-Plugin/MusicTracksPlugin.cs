/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.IO;
using System.Windows;
using CBPSDK;

namespace CBP_MT_Plugin
{
    public class MusicTracksPlugin : IPluginCBP
    {
        public string PluginTitle => "Music Track Selector";

        public string PluginVersion => "0.4.5";

        public string PluginAuthor => "MHLoppy";

        public bool CBPCompatible => true;

        public bool DefaultMultiplayerCompatible => true;

        public string PluginDescription => "Graphical interface (GUI) to customise music in RoN:EE - originally created to bugfix a music bug in the Rise of Babel taunt pack.\n\n"
            + "To reconfigure the tracks again, reload the plugin."
            + "\n\nSource code: https://github.com/MHLoppy/CBP-MT-Plugin";

        public bool IsSimpleMod => false;

        public string LoadResult { get; set; }

        private string soundOrig;
        private string tracksFolder;
        private string MTPFolder;
        private string loadedMTP;

        public void DoSomething(string workshopModsPath, string localModsPath)
        {
            soundOrig = Path.GetFullPath(Path.Combine(localModsPath, @"..\", "Data", "sound.xml"));
            tracksFolder = Path.GetFullPath(Path.Combine(localModsPath, @"..\", "sounds", "tracks"));
            MTPFolder = Path.GetFullPath(Path.Combine(localModsPath, @"..\", "CBP", "MTP"));
            loadedMTP = Path.Combine(MTPFolder, "musictracksplugin.txt");

            //if folder doesn't exist, make it
            if (!Directory.Exists(MTPFolder))
            {
                try
                {
                    Directory.CreateDirectory(MTPFolder);
                    LoadResult = (PluginTitle + " detected for first time. Doing first-time setup.");
                }
                catch (Exception ex)
                {
                    LoadResult = (PluginTitle + ": error writing first-time file:\n\n" + ex);
                }
            }
            else
            {
                LoadResult = (MTPFolder + " already exists; no action taken.");
            }

            //if file doesn't exist, make one
            if (!File.Exists(loadedMTP))
            {
                try
                {
                    File.WriteAllText(loadedMTP, "0");
                    LoadResult = (PluginTitle + " completed first time setup successfully. Created file:\n" + loadedMTP);
                    //MessageBox.Show(PluginTitle + ": Created file:\n" + loadedMTP);//removed to reduce number of popups for first-time CBP users
                }
                catch (Exception ex)
                {
                    LoadResult = (PluginTitle + ": error writing first-time file:\n\n" + ex);
                }
            }
            else
            {
                LoadResult = (loadedMTP + " already exists; no action taken.");
            }

            CheckIfLoaded();//this can be important to do here, otherwise the bool might be accessed without a value depending on how other stuff is set up
        }

        public bool CheckIfLoaded()
        {
            if (File.ReadAllText(loadedMTP) != "0")
            {
                if (!LoadResult.Contains("is loaded"))
                {
                    LoadResult += "\n\n" + PluginTitle + " is loaded.";
                }
                return true;
            }
            else
            {
                if (!LoadResult.Contains("is not loaded"))
                {
                    LoadResult += "\n\n" + PluginTitle + " is not loaded.";
                }
                return false;
            }
        }

        public void LoadPlugin(string workshopModsPath, string localModsPath)
        {
            try
            {
                BackupSoundXML();
                new MusicTracksSelectionWindow().Show();

                File.WriteAllText(loadedMTP, "1");
                CheckIfLoaded();
                LoadResult = (PluginTitle + " was loaded.");
            }
            catch (Exception ex)
            {
                LoadResult = (PluginTitle + " had an error while loading: " + ex);
                MessageBox.Show("Error while loading:\n\n" + ex);
            }
        }

        public void UnloadPlugin(string workshopModsPath, string localModsPath)
        {
            try
            {
                RestoreSoundXML();

                File.WriteAllText(loadedMTP, "0");
                CheckIfLoaded();
                LoadResult = (PluginTitle + ": Previous sound.xml file has been restored.");
                MessageBox.Show("Previous sound.xml file has been restored.");
            }
            catch (Exception ex)
            {
                LoadResult = (PluginTitle + " had an error while unloading: " + ex);
                MessageBox.Show("Error while unloading:\n\n" + ex);
            }
        }

        public void UpdatePlugin(string workshopModsPath, string localModsPath)
        {
            //not needed, so do nothing - plugin itself is kept updated by Steam
        }

        private void BackupSoundXML()
        {
            File.Copy(soundOrig, Path.Combine(MTPFolder, "sound.xml"), true);
        }

        private void RestoreSoundXML()
        {
            File.Copy(Path.Combine(MTPFolder, "sound.xml"), soundOrig, true);
        }
    }
}
