// using System;
// using System.IO;
//
// class Program
// {
//     static void Main()
//     {
//         var root = @"/Users/neetall/Code/WordExchange/APC/ProductionScheduling/ProductionScheduling";
//
//         var repoUrl = "https://raw.githubusercontent.com/Neetall/aps/refs/heads/main/";
//
//         foreach (var file in Directory.GetFiles(root, "*.cs", SearchOption.AllDirectories))
//         {
//             var relative = Path.GetRelativePath(root, file)
//                 .Replace("\\", "/");
//
//             if (relative.StartsWith("bin/") ||
//                 relative.StartsWith("obj/"))
//             {
//                 continue;
//             }
//
//             Console.WriteLine(repoUrl + "ProductionScheduling/" + relative);
//         }
//     }
// }


using System;
using System.IO;
using System.Text;

class Program
{
    static void Main()
    {
        var root =
            @"/Users/neetall/Code/WordExchange/APC/ProductionScheduling/ProductionScheduling";

        var output =
            @"/Users/neetall/Desktop/ProductionScheduling_Source.md";


        var sb = new StringBuilder();


        sb.AppendLine("# ProductionScheduling Source Code");
        sb.AppendLine();


        foreach (var file in Directory.EnumerateFiles(
                     root,
                     "*.cs",
                     SearchOption.AllDirectories))
        {
            var relative =
                Path.GetRelativePath(root, file)
                    .Replace("\\", "/");


            // 排除编译目录
            if (relative.StartsWith("bin/") ||
                relative.StartsWith("obj/"))
            {
                continue;
            }


            // 排除自动生成
            if (relative.EndsWith(".Designer.cs") ||
                relative.EndsWith(".g.cs") ||
                relative.EndsWith(".AssemblyInfo.cs"))
            {
                continue;
            }


            sb.AppendLine();
            sb.AppendLine("---");
            sb.AppendLine();


            sb.AppendLine($"# File: {relative}");
            sb.AppendLine();


            sb.AppendLine("```csharp");


            var code = File.ReadAllText(file);

            sb.AppendLine(code);


            sb.AppendLine("```");
            sb.AppendLine();
        }


        File.WriteAllText(
            output,
            sb.ToString(),
            Encoding.UTF8);


        Console.WriteLine(
            $"Generated: {output}");
    }
}