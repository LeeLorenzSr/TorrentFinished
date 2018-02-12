using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



//You can use the following parameters:

//%F - Name of downloaded file(for single file torrents)
//%D - Directory where files are saved
//%N - Title of torrent
//%P - Previous state of torrent
//%L - Label
//%T - Tracker
//%M - Status message string (same as status column)
//%I - hex encoded info-hash
//%S - State of torrent
//%K - kind of torrent(single|multi)

//Where State is one of:

//Error - 1
//Checked - 2
//Paused - 3
//Super seeding - 4
//Seeding - 5
//Downloading - 6
//Super seed[F] - 7
//Seeding[F] - 8
//Downloading[F] - 9
//Queued seed - 10
//Finished - 11
//Queued - 12
//Stopped - 13


//4:06:28 PM Tuesday, March 29, 2016 : ---------------------------
//4:06:29 PM Tuesday, March 29, 2016 : Program initialized - 5
//4:06:29 PM Tuesday, March 29, 2016 : Arg: Some Dir
//4:06:29 PM Tuesday, March 29, 2016 : Arg: 5
//4:06:29 PM Tuesday, March 29, 2016 : Arg: Unity
//4:06:29 PM Tuesday, March 29, 2016 : Arg: AU FPS KIT v1.5.1
//4:06:29 PM Tuesday, March 29, 2016 : Arg: multi
//4:06:29 PM Tuesday, March 29, 2016 : Arg: E:\TorrentsCompleted\AU FPS KIT v1.5.1
//4:06:29 PM Tuesday, March 29, 2016 : Arg: AU FPS KIT v1.5\Assets\_Resources\Meshes\Weapons\ARMS\HeroHands.FBX
//4:06:29 PM Tuesday, March 29, 2016 : ---------------------------

namespace TorrentFinished
{
    class Program
    {
        static void MoveFilesBetweenFolders(string srcFolder, string destFolder)
        {
            if ( System.IO.Directory.Exists(srcFolder) && !System.IO.Directory.Exists(destFolder))
            {
                System.IO.Directory.Move(srcFolder, destFolder);
            }
        }

        static string CleanFileName(string fileName)
        {
            char[] invalidPathChars = System.IO.Path.GetInvalidFileNameChars();
            string sReturn = fileName;
            foreach (char c in invalidPathChars)
            {
                sReturn = sReturn.Replace(c.ToString(), "_");
            }
            return sReturn;
        }

        static string CleanPath(string path)
        {
            char[] invalidPathChars = System.IO.Path.GetInvalidPathChars();
            string sReturn = path;
            foreach (char c in invalidPathChars)
            {
                sReturn = sReturn.Replace(c.ToString(), "");
            }
            return sReturn;
        }

        static void MoveTorrent(string finalPath, string label, string title, string multiOrSingle, string multiFolder, string singleFileName)
        {
            Logger.Write("Path: " + finalPath);
            Logger.Write("Label: " + label);
            Logger.Write("Title: " + title);
            Logger.Write("MultiOrSingle: " + multiOrSingle);
            Logger.Write("MultiFolder: " + multiFolder);
            Logger.Write("SinglePath: " + singleFileName);

            multiFolder = multiFolder.Trim();
            singleFileName = singleFileName.Trim();

            string newTitle = title;

            if (multiOrSingle != "multi" && title.Contains("."))
            {
                newTitle = System.IO.Path.GetFileNameWithoutExtension(title);
            }

            if (string.IsNullOrWhiteSpace(label))
            {
                label = "Unknown";
            }

            string labelDir = System.IO.Path.Combine(finalPath, label);

//            Logger.Write("Torrent labelled path: " + labelDir);

            if ( !System.IO.Directory.Exists(labelDir) )
            {
                Logger.Write(" Creating LabelPath: " + labelDir);
                System.IO.Directory.CreateDirectory(labelDir);
            }
            if (System.IO.Directory.Exists(labelDir))
            {
                Logger.Write(" LabelPath: " + labelDir);
                char[] invalidPathChars = System.IO.Path.GetInvalidFileNameChars();
                string titleProper = newTitle;
                foreach (char c in invalidPathChars)
                {
                    titleProper = titleProper.Replace(c.ToString(), "");
                }
                string torrentDir = System.IO.Path.Combine(labelDir, titleProper);
                Logger.Write(" TorrentDir: " + torrentDir);

                if (!System.IO.Directory.Exists(torrentDir))
                {
                    // Move files
                    Logger.Write(" Moving files");
                    if (multiOrSingle.Equals("multi"))
                    {
                        try
                        {
                            MoveFilesBetweenFolders(multiFolder, torrentDir);
                            Logger.Write("Moving files from " + multiFolder + " - to - " + torrentDir);
                        }
                        catch
                        {

                        }
                    }
                    else
                    {
                        try
                        {
                            System.IO.Directory.CreateDirectory(torrentDir);
                        }
                        catch { }

                        if (System.IO.Directory.Exists(torrentDir))
                        {
                            String sourceFile = "";
                            String destFile = "";
                            bool bDoMove = true;
                            // Single file to move'
                            try
                            {
                                try
                                {
                                    sourceFile = System.IO.Path.Combine(multiFolder, singleFileName);
                                }
                                catch (Exception e)
                                {
                                    bDoMove = false;
                                    Logger.Write(e, "Problem creating path from ["+CleanPath(multiFolder)+"] and [" + CleanFileName(singleFileName) +"]");

                                }
                                try
                                {
                                    destFile = System.IO.Path.Combine(torrentDir, singleFileName);
                                }
                                catch (Exception e)
                                {
                                    bDoMove = false;
                                    Logger.Write(e, "Problem creating path from [" + torrentDir + "] and [" + singleFileName + "]");
                                }

                                if (bDoMove)
                                {
                                    Logger.Write("Moving [" + sourceFile + "] - to - [" + destFile + "]");
                                    System.IO.File.Move(sourceFile, destFile);
                                }
                            }
                            catch(Exception e)
                            {
                                Logger.Write(e, "Problem moving file "+singleFileName);
                            }
                        }
                        else
                        {
                            Logger.Write("*******************Destination folder could not be created*******************");
                        }
                    }
                }
                else
                {
                    Logger.Write(" **********Destination folder already exists, not moving***************");
                }

            }
            else
            {
                Logger.Write("Unable to find or create destination folder: " + labelDir);
            }
            Logger.Write("--------------------------------------");

        }

        static void Main(string[] args)
        {
            string logPath = System.IO.Path.Combine(args[0], "finalizer.log");
            Logger.Init(logPath);
            Logger.Write("Program initialized - "+args[1]);

            try
            {
                if (args[1].Equals("11"))
                {
                    MoveTorrent(args[0], args[2], args[3], args[4], args[5], args[6]);
                }
                else
                {
                    Logger.Write("Not moving file(s)");
                }
            }
            catch (Exception e)
            {
                Logger.Write("Exception ("+e.GetType().Name+") encountered: " + e.Message);
                Logger.Write(e.ToString());
            }


            // Console.ReadKey();

            //using (StreamWriter w = File.AppendText("log.txt"))
            //{
            //    Log("Test1", w);
            //    Log("Test2", w);
            //}


        }

        public static void Log(string logMessage, TextWriter w)
        {
            w.Write("\r\nLog Entry : ");
            w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                DateTime.Now.ToLongDateString());
            w.WriteLine("  :");
            w.WriteLine("  :{0}", logMessage);
            w.WriteLine("-------------------------------");
        }
    }
}
