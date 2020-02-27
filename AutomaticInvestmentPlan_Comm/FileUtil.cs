using System;
using System.IO;

namespace AutomaticInvestmentPlan_Comm
{
    public static class FileUtil
    {
        public static void BackUpFile(string orignalPath, string newFilePath, string newName)
        {
            if (Directory.Exists(newFilePath) == false)
            {
                Directory.CreateDirectory(newFilePath);
            }
            FileInfo file = new FileInfo(orignalPath);
            FileInfo newFile = new FileInfo(newFilePath + "\\" + newName);
            if (newFile.Exists == false)
            {
                file.CopyTo(newFilePath + "\\" + newName);
            }
        }

    }
}