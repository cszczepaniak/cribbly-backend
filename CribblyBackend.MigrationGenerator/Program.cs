using System;
using System.IO;
using System.Text.RegularExpressions;

namespace CribblyBackend.MigrationGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Must provide exactly one input argument");
                Environment.Exit(1);
            }
            var name = args[0];
            if (!Regex.IsMatch(name, @"[A-Za-z]+"))
            {
                Console.WriteLine("Name must contain only uppercase and lowercase letters");
                Environment.Exit(1);
            }
            var timestamp = DateTime.Now.ToString("yyyyMMddhhmmss");
            try
            {
                WriteMigrationFile(name, timestamp);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
        }

        static void WriteMigrationFile(string name, string timestamp)
        {
            string migrationBody = string.Format(template, timestamp, name);
            // Assume we're in the root of the project
            string startDir = Directory.GetCurrentDirectory();
            string outFile = Path.Combine(startDir, "src", "CribblyBackend.DataAccess", "Migrations", string.Format(@"{0}_{1}.cs", timestamp, name));
            File.WriteAllText(outFile, migrationBody);
        }

        const string template = @"using FluentMigrator;

namespace CribblyBackend.Migrations
{{
    [Migration({0})]
    public class {1} : Migration
    {{
        public override void Down()
        {{
        }}

        public override void Up()
        {{
        }}
    }}
}}
";
    }
}
