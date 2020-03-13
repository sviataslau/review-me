using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ReviewMe
{
    public class Parser
    {
        public string File { get; set; }

        public string GetContent() => System.IO.File.ReadAllText(File);

        public void SaveContent(string content) => System.IO.File.WriteAllText(File, content);

        public void SaveContentInUtf8(string content) => System.IO.File.WriteAllText(File, content, Encoding.UTF8);

        public void SaveContentInUnicode(string content) =>
            System.IO.File.WriteAllText(File, content, Encoding.Unicode);
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
        }
    }
}