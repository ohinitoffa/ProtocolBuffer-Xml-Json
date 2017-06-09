using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using NJsonSchema;
namespace Google.Protobuf.Examples.AddressBook.Json
{
  public class GenerateJsonClass
  {
    public static void Main()
    {
      Task.Run(async () =>
      {
        var schema = await JsonSchema4.FromFileAsync(@"..\..\Json\addressbook.json");
        var generator = new NJsonSchema.CodeGeneration.CSharp.CSharpGenerator(schema);
        string file = generator.GenerateFile(@"Google.Protobuf.Examples.Json.AddressBook");
        using (StreamWriter outputFile = new StreamWriter(@"..\..\Json\Addressbook.cs"))
        {
            outputFile.Write(file);
        }
      }).GetAwaiter().GetResult();
    }
  }
}
