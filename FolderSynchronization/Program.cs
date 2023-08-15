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
        List<string> sourceFolderContents;
        while (true)
        {
            Thread.Sleep(new TimeSpan(0, 0, 5));
            Console.WriteLine("Starting synchronisation.");
            sourceFolderContents = Helpers.GetFolderContents(ROOT_PATH, SOURCE_FOLDER);
            string replicaFolderPath = ROOT_PATH + @"\" + REPLICA_FOLDER;
            Helpers.CopyFolderContents(sourceFolderContents, replicaFolderPath);
            Console.WriteLine("Synchronisation successful.");
        }
    }
}