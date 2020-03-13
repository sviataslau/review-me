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

        public string GetContent() => System.IO.File.ReadAllText(File);

        private static void WriteLog(string text)
        {
            string logPath = ConfigurationManager.AppSettings["LogPath"];
            System.IO.File.WriteAllText(logPath, text);
        }

        public void SaveContent(string content)
        {
            WriteLog($"content saved at {DateTime.Now}");
            System.IO.File.WriteAllText(File, content);
        }

        public void SaveContentInUtf8(string content)
        {
            WriteLog($"content saved at {DateTime.Now}");
            System.IO.File.WriteAllText(File, content, Encoding.UTF8);
        }

        public void SaveContentInUnicode(string content)
        {
            WriteLog($"content saved at {DateTime.Now}");
            System.IO.File.WriteAllText(File, content, Encoding.Unicode);
        }

        public DateTime GetCheckedTimestamp()
        {
            string[] allLines = System.IO.File.ReadAllLines(File);
            string currentTimestamp = allLines
                .Select(l => new {Line = l, Index = l.IndexOf(l, StringComparison.Ordinal)})
                .Where(l => l.Line.Contains("Checked at")).OrderByDescending(l => l.Index).Select(l => l.Line)
                .FirstOrDefault();
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
                string currentContent = p.GetContent();

                var reader = new StreamReader(p.File, true);
                reader.ReadLine();
                var encoding = reader.CurrentEncoding;
                if (encoding == Encoding.UTF8)
                {
                    p.SaveContentInUtf8(currentContent + $"\r\nChecked at {DateTime.Now}");
                }
                else if (encoding == Encoding.Unicode)
                {
                    p.SaveContentInUnicode(currentContent + $"\r\nChecked at {DateTime.Now}");
                }
                else
                {
                    p.SaveContentInUnicode(currentContent + $"\r\nChecked at {DateTime.Now}");
                }
            });

            p.File = @"C:\file4.txt";
            DateTime dt = p.GetCheckedTimestamp();
            Console.WriteLine("Last checked: " + dt);
        }
    }
}