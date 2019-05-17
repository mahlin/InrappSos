using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SFTPFileHandler;

namespace SFTPFileHandler
{
    class Program
    {
        static void Main(string[] args)
        {
            var sftpWatcher = new SFTPWatcher();

            //sftpWatcher.CheckFolders();
            TestC();

            //För test
            Console.ReadLine();

            //Run();
        }


        private static void TestA()
        {
            var fileName = "TestFil.txt";
            var local = Path.Combine(@"C:\TmpTestFiles", fileName);
            var remote = Path.Combine(@"\\DESKTOP-Q22BLMK\c$\temp\", fileName);

            WebClient request = new WebClient();
            request.Credentials = new NetworkCredential(@"marie", "0219");

            if (File.Exists(local))
            {
                File.Delete(local);

                File.Copy(remote, local, true);
            }
            else
            {
                File.Copy(remote, local, true);
            }

        }

        private static void TestB()
        {
            var networkPath = @"//DESKTOP-Q22BLMK/c";
            //var networkPath = @"//DESKTOP-Q22BLMK/c$";
            var credentials = new NetworkCredential("marie", "0219");
            string[] fileList;

            using (new NetworkConnection(networkPath, credentials))
            {
                fileList = Directory.GetFiles(networkPath);
            }

            foreach (var file in fileList)
            {
                Console.WriteLine("{0}", Path.GetFileName(file));
            }

        }

        private static byte[] TestC()
        {
            string networkPath = @"\\DESKTOP-Q22BLMK\c";
            //var networkPath = @"//DESKTOP-Q22BLMK/c$";
            NetworkCredential credentials = new NetworkCredential(@"marie", "0219");
            string myNetworkPath = string.Empty;
            byte[] fileBytes = null;
            string DownloadURL = Path.Combine(@"C:\TmpTestFiles");

            try
            {
                using (new NetworkConnection(networkPath, credentials))
                {
                    var fileList = Directory.GetDirectories(networkPath);
                    //fileList = Directory.GetFiles(networkPath);
                }
            }
            catch (Win32Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadLine();
            }

            return fileBytes;
        }

        private static void TestD()
        {
            string networkPath = @"\\DESKTOP-Q22BLMK\c";
            //var networkPath = @"//DESKTOP-Q22BLMK/c$";
            NetworkCredential credentials = new NetworkCredential(@"marie", "0219");

            try
            {
                using (new NetworkConnection(networkPath, credentials))
                {
                    var fileList = Directory.GetDirectories(networkPath);

                    //foreach (var item in fileList) { if (item.Contains("{ClientDocument}")) { myNetworkPath = item; } }

                    //myNetworkPath = myNetworkPath + UploadURL;

                    //using (FileStream fileStream = File.Create(UploadURL, file.Length))
                    //{
                    //    await fileStream.WriteAsync(file, 0, file.Length);
                    //    fileStream.Close();
                    //}
                }
            }
            catch (Exception ex)
            {

            }
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
