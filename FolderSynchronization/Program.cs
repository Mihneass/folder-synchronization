using FolderSynchronization;

internal class Program
{
    private static void Main(string[] args)
    {
        if (Environment.CurrentDirectory == null)
            throw new Exception("CurrentDirectory could not be found");
        if (Directory.GetParent(Environment.CurrentDirectory).Parent == null || Directory.GetParent(Environment.CurrentDirectory).Parent.Parent == null)
            throw new Exception("Project directory could not be found");
        string ROOT_PATH = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
        const string SOURCE_FOLDER = "SourceFolder";
        const string REPLICA_FOLDER = "ReplicaFolder";
        const string LOG_FILE = "logs.txt";
        string LOG_FILE_PATH = Path.Combine(ROOT_PATH, LOG_FILE);
       
        RunSync(ROOT_PATH, SOURCE_FOLDER, REPLICA_FOLDER, LOG_FILE_PATH);
    }

    private static void RunSync(string ROOT_PATH, string SOURCE_FOLDER, string REPLICA_FOLDER, string LOG_FILE_PATH)
    {
        string sourcefolderPath = Path.Combine(ROOT_PATH, SOURCE_FOLDER);
        List<string> sourceFolderContentsOldMap = new List<string>();
        Helpers.MapDirectory(sourcefolderPath, sourceFolderContentsOldMap);
        List<string> sourceFolderContents = Helpers.GetFolderContents(ROOT_PATH, SOURCE_FOLDER);
        List<string> sourceFolderContentsMap = new List<string>();
        Helpers.MapDirectory(sourcefolderPath, sourceFolderContentsMap);
        MapFolderContents(sourceFolderContents, ROOT_PATH, SOURCE_FOLDER, REPLICA_FOLDER, LOG_FILE_PATH, sourceFolderContentsMap);
        Console.WriteLine("Startup synchronization complete.");
        bool changeHasTakenPlaceInSourceFolder = false;
        while (true)
        {
            Thread.Sleep(new TimeSpan(0, 0, 5));
            sourceFolderContentsMap = new List<string>();
            Helpers.MapDirectory(sourcefolderPath, sourceFolderContentsMap);

            List<string> newFilesAdded = Helpers.GetNewlyAddedFiles(sourceFolderContentsMap, sourceFolderContentsOldMap);
            if(newFilesAdded.Count > 0)
            {
                changeHasTakenPlaceInSourceFolder = true;
                Console.WriteLine("Newly added files have been logged.");
                using (StreamWriter outputFile = new StreamWriter(LOG_FILE_PATH, true))
                {
                    foreach (string file in newFilesAdded)
                    {
                        outputFile.WriteLine("New file added: " + file);
                    }
                }
            }

            List<string> recentlyDeletedFiles = Helpers.GetRecentlyDeletedFiles(sourceFolderContentsMap, sourceFolderContentsOldMap);
            if (recentlyDeletedFiles.Count > 0)
            {
                changeHasTakenPlaceInSourceFolder = true;
                Console.WriteLine("Deleted files have been logged.");
                using (StreamWriter outputFile = new StreamWriter(LOG_FILE_PATH, true))
                {
                    foreach (string file in recentlyDeletedFiles)
                    {
                        outputFile.WriteLine("New file deleted: " + file);
                    }
                }
            }

            if (changeHasTakenPlaceInSourceFolder)
            {
                MapFolderContents(sourceFolderContents, ROOT_PATH, SOURCE_FOLDER, REPLICA_FOLDER, LOG_FILE_PATH, sourceFolderContentsMap);
            }
            else
            {
                Console.WriteLine("No change detected in the Source folder");
            }
            sourceFolderContentsOldMap = new List<string>();
            Helpers.MapDirectory(Path.Combine(ROOT_PATH, SOURCE_FOLDER), sourceFolderContentsOldMap);
            changeHasTakenPlaceInSourceFolder = false;
        }
    }

    private static void MapFolderContents(List<string> sourceFolderContents, string ROOT_PATH, string SOURCE_FOLDER, string REPLICA_FOLDER, string LOG_FILE_PATH, List<string> sourceFolderContentsMap)
    {
        Console.WriteLine("Starting synchronization.");
        sourceFolderContents = Helpers.GetFolderContents(ROOT_PATH, SOURCE_FOLDER);
        string replicaFolderPath = Path.Combine(ROOT_PATH, REPLICA_FOLDER);
        Helpers.CopyFolderContents(sourceFolderContents, replicaFolderPath);
        using (StreamWriter outputFile = new StreamWriter(LOG_FILE_PATH, true))
        {
            foreach (string file in sourceFolderContentsMap)
            {
                outputFile.WriteLine("File copied: " + file);
            }
        }
        Console.WriteLine("Synchronization successful.");
    }
}