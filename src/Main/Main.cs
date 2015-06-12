using System;
using System.IO;

namespace USC.GISResearchLab.Common.UnZipFile
{
  class MainClass
  {
    public static void Main(string[] args)
    {
      // Perform simple parameter checking.
      if (args.Length < 1)
      {
        Console.WriteLine("Usage UnzipFile NameOfFile");
        return;
      }

      if (!File.Exists(args[0]))
      {
        Console.WriteLine("Cannot find file '{0}'", args[0]);
        return;
      }
      try
      {
        UnZip a = new UnZip(args[0]);
        a.OutputDirectory = @"C:\Documents and Settings\kshahabi\Desktop";
        if (a.NeedPassword())
        {
          Console.Write("Password: ");
          a.ZipPassword = Console.ReadLine();
        }
        a.DoUnZip();
      }
      catch (Exception e)
      {
        Console.WriteLine(e.ToString());
      }
      Console.Write("Done.");
      Console.ReadLine();
    }
  }
}