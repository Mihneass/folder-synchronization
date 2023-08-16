using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchronization
{
    public static class Helpers
    {
        public static List<string> GetFolderContents(string rootPath, string folderName)
        {
            string fullPath;
            List<string> folderContents = new List<string>();
            try
            {
                fullPath = Path.Combine(rootPath, folderName);

                folderContents.AddRange(Directory.GetFiles(fullPath).ToList());
                folderContents.AddRange(Directory.GetDirectories(fullPath).ToList());
                return folderContents;
            }
            catch(DirectoryNotFoundException)
            {
                Console.WriteLine("Exception: The folder could not be found at the following path: " + rootPath + @"\" +  folderName);
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

            if(recursive)
                foreach(var subDir in dirs)
                {
                    directoryMap.Add(subDir.FullName);
                    return MapDirectory(subDir.FullName, directoryMap);
                }
            return directoryMap;
        }

        public static List<string> GetNewlyAddedFiles(List<string> newFolder, List<string> oldFolder)
        {
            List<string> newlyAddedFiles = new List<string>();

            newlyAddedFiles = newFolder.Except(oldFolder).ToList();

            return newlyAddedFiles;
        }
        
        public static List<string> GetRecentlyDeletedFiles(List<string> newFolder, List<string> oldFolder)
        {
            List<string> recentlyDeletedFiles = new List<string>();

            recentlyDeletedFiles = oldFolder.Except(newFolder).ToList();

            return recentlyDeletedFiles;
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
                    string newDestinationDir = Path.Combine(Path.Combine(destinationPath, dir.Name), subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }
    }
}
