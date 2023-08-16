using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace FolderSynchronization
{
    public static class Helpers
    {
        public static List<string> GetFolderContents(string path)
        {
            List<string> folderContents = new List<string>();
            try
            {
                folderContents.AddRange(Directory.GetFiles(path).ToList());
                folderContents.AddRange(Directory.GetDirectories(path).ToList());
                return folderContents;
            }
            catch(DirectoryNotFoundException)
            {
                Console.WriteLine("Exception: The folder could not be found at the following path: " + path);
            }
            return null;
        }

        public static void CopyFolderContents(List<string> sourceFolder, string destinationPath)
        {
            EmptyFolder(destinationPath);
            foreach(var file in sourceFolder)
            {
                var fileInformation = new FileInfo(file);
                if (Directory.Exists(file))
                    CopyDirectory(file, destinationPath);
                else
                    File.Copy(file,Path.Combine(destinationPath, fileInformation.Name), true);
            }
        }

        public static List<string> MapDirectory(string path, List<string> directoryMap, bool recursive = false)
        {
            var dir = new DirectoryInfo(path);

            if (!dir.Exists)
                throw new Exception(@"Directory not found at the following address: " + dir.FullName);

            DirectoryInfo[] dirs = dir.GetDirectories();

            if (dirs.Length > 0) recursive = true;

            foreach(var f in dir.GetFiles())
            {
                directoryMap.Add(f.FullName);
            }

            foreach (var subDir in dir.GetDirectories())
            {
                directoryMap.Add(subDir.FullName);
            }

            if (recursive)
                foreach(var subDir in dirs)
                {
                   MapDirectory(subDir.FullName, directoryMap);
                }
            return directoryMap;
        }

        public static List<string> GetNewlyAddedFiles(List<string> firstFolder, List<string> secondFolder)
        {
            List<string> newlyAddedFiles = new List<string>();

            newlyAddedFiles = firstFolder.Except(secondFolder).ToList();

            return newlyAddedFiles;
        }
        
        public static List<string> GetRecentlyDeletedFiles(List<string> firstFolder, List<string> secondFolder)
        {
            List<string> recentlyDeletedFiles = new List<string>();

            recentlyDeletedFiles = secondFolder.Except(firstFolder).ToList();

            return recentlyDeletedFiles;
        }

        public static List<string> TruncatePaths(List<string> paths, string folderName)
        {
            List<string> truncatedPaths = new List<string>();
            foreach(var path in paths) 
            {
                var index = path.IndexOf(folderName) + folderName.Length;
                truncatedPaths.Add(path.Substring(path.LastIndexOf(folderName) + folderName.Length));
            }
            return truncatedPaths;
        }

        private static void EmptyFolder(string path)
        {
            DirectoryInfo folderInformation = new DirectoryInfo(path);
            foreach(var file in folderInformation.GetFiles())
            {
                file.Delete();
            }
            foreach(var folder in folderInformation.GetDirectories())
            {
                folder.Delete(true);
            }
        }

        private static void CopyDirectory(string path, string destinationPath, bool recursive = false)
        {
            var dir = new DirectoryInfo(path);

            if (!dir.Exists)
                throw new Exception(@"Directory not found at the following address: " + dir.FullName);

            DirectoryInfo[] dirs = dir.GetDirectories();

            if(dirs.Length > 0) recursive = true;

            Directory.CreateDirectory(Path.Combine(destinationPath, dir.Name));

            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(Path.Combine(destinationPath, dir.Name), file.Name);
                file.CopyTo(targetFilePath);
            }

            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationPath, dir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }
    }
}
