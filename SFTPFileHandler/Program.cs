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
            Console.WriteLine("******************  SFTPFileHandler startar ***************");
            var sftpWatcher = new SFTPWatcher();

            sftpWatcher.CheckFolders();

            //TODO - För koppling mot filarean i KT
            //TestB();

            Console.WriteLine("********************************************************");
            Console.WriteLine("******************  SFTPFileHandler klar ***************");
            //För test
            //Console.ReadLine();
        }
        
        //private static void TestB()
        //{
        //    var networkPath = @"\\INRAPPSFTP01V\d$\root";
        //    var credentials = new NetworkCredential(@"inrappsftpadm-svc", "LoveDataFiles1!");
        //    string[] fileList;

        //    try
        //    {
        //        using (new NetworkConnection(networkPath, credentials))
        //        {
        //            var dirList = Directory.GetDirectories(networkPath);
        //            //fileList = Directory.GetFiles(networkPath);

        //            foreach (var dir in dirList)
        //            {
        //                var lastSlashPos = dir.LastIndexOf("\\");
        //                var folderName = dir.Substring(lastSlashPos + 1);
        //                Console.WriteLine(folderName);
        //            }
        //        }
        //    }

        //    catch (Win32Exception e)
        //    {
        //        Console.WriteLine(e.Message);
        //        Console.ReadLine();
        //    }
        //    catch (Exception ex)
        //    {
        //        string Message = ex.Message.ToString();
        //        Console.WriteLine(Message);
        //        Console.ReadLine();
        //    }
        //}
    }
}
