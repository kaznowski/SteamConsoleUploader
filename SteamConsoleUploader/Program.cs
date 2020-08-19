using System;
using System.Diagnostics;
using System.IO;

namespace SteamConsoleUploader
{
    class Program
    {
        private const string SteamCmdArgFormat = "+login {0} {1} +run_app_build_http \"{2}\" +quit";

        static void Main(string[] args)
        {
            try
            {
                var config = SteamConfig.FromFilePath(args[0]);
                var steamBuildScript = Path.GetFullPath(config.BuildScript);
                var sdkPath = config.SDKPath;
                var username = config.Login;
                var password = config.Password;

                if (!Directory.Exists(sdkPath))
                {
                    throw new Exception($"SteamSDK {sdkPath} not found");
                }

                var steamCmdPath = Path.Combine(sdkPath, "tools/ContentBuilder/builder/steamcmd.exe");
                if (!File.Exists(steamCmdPath))
                {
                    throw new Exception($"SteamCMD {steamCmdPath} not found");
                }

                CopyContentToRelativeBuildScript(config);

                var uploaderProcess = new Process();
                uploaderProcess.StartInfo.FileName = steamCmdPath;
                uploaderProcess.StartInfo.WorkingDirectory = 
                    Path.GetDirectoryName(Path.GetDirectoryName(steamCmdPath));
                uploaderProcess.StartInfo.Arguments =
                    string.Format(SteamCmdArgFormat, username, password, steamBuildScript);


                uploaderProcess.StartInfo.UseShellExecute = false;
                uploaderProcess.StartInfo.RedirectStandardOutput = true;
                uploaderProcess.OutputDataReceived += (sender, msg) =>
                {
                    if (msg != null)
                        Console.WriteLine(msg.Data);
                };

                uploaderProcess.Start();
                Console.WriteLine("Executing SteamCMD \"{0}\"...", steamCmdPath);
                
                uploaderProcess.BeginOutputReadLine();
                uploaderProcess.WaitForExit();
                uploaderProcess.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private static void CopyContentToRelativeBuildScript(SteamConfig config)
        {
            if (!Directory.Exists(config.BuildPath))
            {
                throw new Exception($"Target path {config.BuildPath} does not exist. Did you forget to build?");
            }

            var contentFolderPath = Path.GetFullPath(config.BuildScript + config.BuildContentRelativeToBuildScript);

            if (Directory.Exists(contentFolderPath))
                Directory.Delete(contentFolderPath, true);

            CopyDirectory(config.BuildPath, contentFolderPath);
            Console.WriteLine($"Build files copied from {config.BuildPath} to {contentFolderPath}.");
        }

        public static void CopyDirectory(string fromDirectory, string toDirectory)
        {
            Directory.CreateDirectory(toDirectory);

            fromDirectory = Path.GetFullPath(fromDirectory);
            string[] files = Directory.GetFiles(fromDirectory, "*.*", SearchOption.AllDirectories);
            string[] directories = Directory.GetDirectories(fromDirectory, "*.*", SearchOption.AllDirectories);

            foreach (string directory in directories)
            {
                string directoryPath = Path.GetFullPath(directory);
                string newDirectoryPath = directoryPath.Replace(fromDirectory, toDirectory);

                Directory.CreateDirectory(newDirectoryPath);
            }

            foreach (string file in files)
            {
                string filePath = Path.GetFullPath(file);
                string newFilePath = filePath.Replace(fromDirectory, toDirectory);

                File.Copy(filePath, newFilePath);
            }
        }
    }
}