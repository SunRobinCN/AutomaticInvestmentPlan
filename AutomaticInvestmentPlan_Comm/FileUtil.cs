using System;
using System.IO;

namespace AutomaticInvestmentPlan_Comm
{
    public static class FileUtil
    {
        private static readonly string Path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\AutomatciInvestmentPlan";
        private static readonly string FileName = "\\mark.data";

        public static void WriteSingalToFile()
        {
            if (Directory.Exists(Path) == false)
            {
                Directory.CreateDirectory(Path);
            }
            using (FileStream fs = new FileStream(Path + FileName, FileMode.OpenOrCreate))
            {
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(DateTime.Now.ToString("yyyy-MM-dd"));
                sw.Close();
            }
        }

        public static bool ReadSingalFromFile()
        {
            try
            {
                string result;
                using (StreamReader reader = new StreamReader(Path + FileName))
                {
                    result = reader.ReadToEnd();
                    reader.Close();
                }
                if (result.Contains(DateTime.Now.ToString("yyyy-MM-dd")))
                {
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                CombineLog.LogError("ReadSingalFromFile", e);
                return false;
            }
        }
    }
}