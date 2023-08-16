using FolderSynchronization;

internal class Program
{
    private static void Main(string[] args)
    {
        var sourceFolderPath = "";
        while (!Directory.Exists(sourceFolderPath))
        {
            Console.WriteLine("Please insert the path to the source folder: ");
            sourceFolderPath = Console.ReadLine();
            if (!Directory.Exists(sourceFolderPath)) Console.WriteLine("The provided path does not lead to an existing folder.");
        }
        var replicaFolderPath = "";
        while (!Directory.Exists(replicaFolderPath))
        {
            Console.WriteLine("Please insert the path to the replica folder: ");
            replicaFolderPath = Console.ReadLine();
            if (!Directory.Exists(replicaFolderPath)) Console.WriteLine("The provided path does not lead to an existing folder.");
        }
        var logFilePath = "";
        while (!File.Exists(logFilePath))
        {
            Console.WriteLine("Please insert the path to the log file: ");
            logFilePath = Console.ReadLine();
            if (!File.Exists(logFilePath)) Console.WriteLine("The provided path does not lead to an existing .txt file.");
        }
        int syncInterval = 0;
        while(syncInterval <= 0) 
        {
            Console.WriteLine("Please insert the desired interval for the file synchronization in seconds: ");
            var input = Console.ReadLine();
            if(!int.TryParse(input, out syncInterval))
            {
                syncInterval = 0;
                Console.WriteLine("Please input a valid value");
            }
            else if(syncInterval <= 0)
            {
                Console.WriteLine("Please input a positive number");
            }
        }
       
        RunSync(sourceFolderPath, replicaFolderPath, logFilePath, syncInterval);
    }

    private static void RunSync(string sourceFolderPath, string replicaFolderPath, string logFilePath, int syncInterval)
    {
        List<string> sourceFolderContentsOldMap = new List<string>();
        Helpers.MapDirectory(sourceFolderPath, sourceFolderContentsOldMap);
        List<string> sourceFolderContents = Helpers.GetFolderContents(sourceFolderPath);
        List<string> sourceFolderContentsMap = new List<string>();
        List<string> replicaFolderContentsMap = new List<string>();
        Helpers.MapDirectory(sourceFolderPath, sourceFolderContentsMap);
        MapFolderContents(sourceFolderContents, sourceFolderPath, replicaFolderPath, logFilePath, sourceFolderContentsMap);
        Console.WriteLine("Startup synchronization complete.");
        bool theFoldersAreDesynchronized = false;
        while (true)
        {
            Thread.Sleep(new TimeSpan(0, 0, syncInterval));
            sourceFolderContentsMap = new List<string>();
            Helpers.MapDirectory(sourceFolderPath, sourceFolderContentsMap);

            List<string> newFilesAdded = Helpers.GetNewlyAddedFiles(sourceFolderContentsMap, sourceFolderContentsOldMap);
            if(newFilesAdded.Count > 0)
            {
                theFoldersAreDesynchronized = true;
                Console.WriteLine("Newly added files have been logged.");
                using (StreamWriter outputFile = new StreamWriter(logFilePath, true))
                {
                    foreach (string file in newFilesAdded)
                    {
                        outputFile.WriteLine("New file added: " + file);
                        Console.WriteLine("New file added: " + file);
                    }
                }
            }

            List<string> recentlyDeletedFiles = Helpers.GetRecentlyDeletedFiles(sourceFolderContentsMap, sourceFolderContentsOldMap);
            if (recentlyDeletedFiles.Count > 0)
            {
                theFoldersAreDesynchronized = true;
                Console.WriteLine("Deleted files have been logged.");
                using (StreamWriter outputFile = new StreamWriter(logFilePath, true))
                {
                    foreach (string file in recentlyDeletedFiles)
                    {
                        outputFile.WriteLine("New file deleted: " + file);
                        Console.WriteLine("New file deleted: " + file);
                    }
                }
            }

            replicaFolderContentsMap = new List<string>();
            Helpers.MapDirectory(replicaFolderPath, replicaFolderContentsMap);
            var truncatedReplicaFolderContentsMap = Helpers.TruncatePaths(replicaFolderContentsMap, new DirectoryInfo(replicaFolderPath).Name);
            var truncatedSourceFolderContentsMap = Helpers.TruncatePaths(sourceFolderContentsMap, new DirectoryInfo(sourceFolderPath).Name);
            if(Helpers.GetRecentlyDeletedFiles(truncatedReplicaFolderContentsMap, truncatedSourceFolderContentsMap).Count > 0 
                || Helpers.GetNewlyAddedFiles(truncatedReplicaFolderContentsMap, truncatedSourceFolderContentsMap).Count > 0)
                theFoldersAreDesynchronized= true;

            if (theFoldersAreDesynchronized)
            {
                MapFolderContents(sourceFolderContents, sourceFolderPath, replicaFolderPath, logFilePath, sourceFolderContentsMap);
            }
            else
            {
                Console.WriteLine("No desynchronization has been detected");
            }
            sourceFolderContentsOldMap = new List<string>();
            Helpers.MapDirectory(sourceFolderPath, sourceFolderContentsOldMap);
            theFoldersAreDesynchronized = false;
        }
    }

    private static void MapFolderContents(List<string> sourceFolderContents, string sourceFolderPath, string replicaFolderPath, string logFilePath, List<string> sourceFolderContentsMap)
    {
        Console.WriteLine("Starting synchronization.");
        sourceFolderContents = Helpers.GetFolderContents(sourceFolderPath);
        Helpers.CopyFolderContents(sourceFolderContents, replicaFolderPath);
        using (StreamWriter outputFile = new StreamWriter(logFilePath, true))
        {
            foreach (string file in sourceFolderContentsMap)
            {
                outputFile.WriteLine("File copied: " + file);
               // Console.WriteLine("File copied: " + file);
            }
        }
        Console.WriteLine("Synchronization successful.");
    }
}