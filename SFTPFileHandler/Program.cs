using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFTPFileHandler
{
    class Program
    {
        static void Main(string[] args)
        {
            var sftpWatcher = new SFTPWatcher();

            sftpWatcher.CheckFiles();

            //För test
            Console.ReadLine();

            //Run();
        }


        //private static void Run()
        //{
        //    string _fileareaPath = ConfigurationManager.AppSettings["FileAreaPath"];

        //    // Create a new FileSystemWatcher and set its properties.
        //    using (FileSystemWatcher watcher = new FileSystemWatcher())
        //    {
        //        watcher.Path = _fileareaPath;

        //        // Watch for changes in LastAccess and LastWrite times, and
        //        // the renaming of files or directories.
        //        watcher.NotifyFilter = NotifyFilters.LastAccess
        //                               | NotifyFilters.LastWrite
        //                               | NotifyFilters.FileName
        //                               | NotifyFilters.DirectoryName
        //                               | NotifyFilters.Attributes
        //                               | NotifyFilters.CreationTime;

        //        // Only watch text files.
        //        watcher.Filter = "*.txt";

        //        // Add event handlers.
        //        watcher.Created += OnChanged;

        //        // Begin watching.
        //        watcher.EnableRaisingEvents = true;

        //        // Wait for the user to quit the program.
        //        Console.WriteLine("Press 'q' to quit the sample.");
        //        while (Console.Read() != 'q') ;

        //    }
        //}

        // Define the event handlers.
        //private static void OnChanged(object source, FileSystemEventArgs e) =>
        //    // Specify what is done when a file is changed, created, or deleted.
        //    Console.WriteLine($"File: {e.FullPath} {e.ChangeType}");

    }
}
