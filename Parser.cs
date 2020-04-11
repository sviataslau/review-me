using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewMe
{
    public class Parser
    {
        public string File { get; set; }

        public string GetContent() => string.IsNullOrEmpty(File) ? String.Empty : System.IO.File.ReadAllText(File);

        private static void WriteLog(string text)
        {
            string logPath = ConfigurationManager.AppSettings["LogPath"];
            if(!string.IsNullOrEmpty(logPath)) 
                System.IO.File.AppendAllText(logPath, text); // append only the last line
        }

        public void SaveContent(string content, Encoding encoding) //Refactored, made generic
        {
            WriteLog($"content saved at {DateTime.Now}");
            if(encoding.Equals(Encoding.UTF8) || encoding.Equals(Encoding.Unicode))
                System.IO.File.AppendAllText(File, content, encoding); // append only the last line instead of reading all lines
            else
                System.IO.File.AppendAllText(File, content); 
        }

        public DateTime GetCheckedTimestamp()
        {
            // Refactored this, since Index was not working properly
            int counter = 0; 
            string[] allLines = System.IO.File.ReadAllLines(File);
            var currentTimestamp = allLines
                    .Where(l => l.Contains("Checked at"))
                    .Select(l => new { Line = l, Index = counter++ }).OrderByDescending(l => l.Index)
                    .Select(l => l.Line).FirstOrDefault();
            return DateTime.Parse(currentTimestamp.Replace("Checked at ", string.Empty));
        }
    }

    public class Program
    {
        private static readonly Parser p = new Parser();

        public static void Main(params string[] args)
        {
            p.File = @"C:\file.txt";
            Console.WriteLine(p.GetContent());

            string[] files = {@"C:\file1.txt", @"C:\file2.txt", @"C:\file3.txt"};

            Parallel.ForEach(files, f =>
            {
                p.File = f;
                
                var encoding = Encoding.Default;
                using (var reader = new StreamReader(p.File, true))
                {
                    reader.ReadLine();
                    encoding = reader.CurrentEncoding;
                }

                p.SaveContent($"Checked at {DateTime.Now}", encoding);
            });

            p.File = @"C:\file4.txt";
            DateTime dt = p.GetCheckedTimestamp();
            Console.WriteLine("Last checked: " + dt);
        }
    }
}