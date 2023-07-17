/* Simple script that compares two folders, and delete files and folders in the second folder that exist in first folder
 * Usage:
 * syncfolder -toKeep "path_to_keep" -toDelete "path_to_keep" -run
 *
 * Parameters:
 * -toKeep : path to folder that will be used to compare
 * -toDelete: path to folder in which delete operation will be perform
 * -run: use to really perform the operation. if not provided, only simulation will be perform, no delete operations
 */

string folderToKeep = @"to_keep_folder";
string folderToDelete = @"to_delete_folder";
bool simulation = true;

if (args.Length > 0)
{
    for (int i = 0; i < args.Length; i++)
    {
        if (args[i] == "-toKeep" && i + 1 < args.Length)
        {
            folderToKeep = args[i + 1];
        }
        else if (args[i] == "-toDelete" && i + 1 < args.Length)
        {
            folderToDelete = args[i + 1];
        }
        else if (args[i] == "-run")
        {
            simulation = false;
        }
    }
}

static (string[] allfiles, List<string> duplicateFiles) GetDuplicateFiles(string folderToKeep, string folderToDelete )
{
    List<string> duplicateFiles = new List<string>();

    string[] allFiles = Directory.GetFiles(folderToDelete , "*", SearchOption.AllDirectories);
    foreach (string fileB in allFiles)
    {
        string fileA = fileB.Replace(folderToDelete , folderToKeep);

        if (File.Exists(fileA) && AreFilesEqual(fileA, fileB))
        {
            duplicateFiles.Add(fileB);
        }
    }

    return (allFiles, duplicateFiles);
}

static bool AreFilesEqual(string fileA, string fileB)
{
    var streamA = File.ReadAllBytes(fileA);
    var streamB = File.ReadAllBytes(fileB);
    {
        var hashBytesA = Blake3.Hasher.Hash(streamA);
        var hashBytesB = Blake3.Hasher.Hash(streamB);

        return hashBytesA == hashBytesB;
    }
}

static void DeleteEmptyFoldersAndSelf(string folderPath)
{
    string[] subdirectories = Directory.GetDirectories(folderPath);

    foreach (string subdirectory in subdirectories)
    {
        DeleteEmptyFoldersAndSelf(subdirectory);
    }

    if (IsDirectoryEmpty(folderPath))
    {
        Directory.Delete(folderPath);        
    }
}

static void DeleteEmptyFolders(string folderPath)
{
    string[] subdirectories = Directory.GetDirectories(folderPath);

    foreach (string subdirectory in subdirectories)
    {
        DeleteEmptyFoldersAndSelf(subdirectory);
    }
}

static bool IsDirectoryEmpty(string folderPath)
{
    string[] files = Directory.GetFiles(folderPath);
    string[] subdirectories = Directory.GetDirectories(folderPath);

    return (files.Length == 0 && subdirectories.Length == 0);
}


Console.WriteLine($"Sync process started");
if (simulation)
{
    Console.WriteLine($"Simulation mode, no actions will be perform");
}

Console.WriteLine($"  - Sync files");

var initialDirectoryCount = Directory.GetDirectories(folderToDelete, "*", SearchOption.AllDirectories).Count();
Console.WriteLine($"    · Total {initialDirectoryCount} folders");

var result = GetDuplicateFiles(folderToKeep, folderToDelete);
Console.WriteLine($"    · Total {result.allfiles.Count()} files");
Console.WriteLine($"    · Found {result.duplicateFiles.Count()} duplicates");
Console.WriteLine($"    · Remains {(result.allfiles.Count() - result.duplicateFiles.Count())}");


if (simulation == false)
{
    Console.WriteLine($"  - Deleting duplicate files");
    foreach (string file in result.duplicateFiles)
    {
        if (simulation == false) File.Delete(file);
    }
    Console.WriteLine($"  - Deleting empty folders");
    DeleteEmptyFolders(folderToDelete );
}
Console.ReadLine();