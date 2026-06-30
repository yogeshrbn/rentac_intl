using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileNames = Directory.GetFiles("hartron");
            foreach(var f in fileNames)
            {
                var newFileName = f.Insert(16, ".");
                newFileName = newFileName.Replace("jpeg", "jpg").Replace("png", "jpg");
                    var fInfo = new FileInfo(f);
                fInfo.CopyTo(newFileName);
            }
        }
    }
}
