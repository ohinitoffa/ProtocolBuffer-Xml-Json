# ProtocolBuffer-Xml-Json
Performance and memory output comparison between marshalling tools: ProtocolBuffer vs Xml vs Json

This project implements the Addressbook example schema of ProtocolBuffer(https://github.com/google/protobuf/releases/download/v3.3.0/protobuf-csharp-3.3.0.tar.gz) in Json and Xml. For each one of the marsahlling tools, it compares the time it takes to parse the model from file or ouput it to file, the size of the ouput file, the times to generate a number of persons record and the time to traverse all the records of persons in the addressbook.
The code is implemented in C#. Xml C# class is generated with Xsd tool of microsoft (https://docs.microsoft.com/en-us/dotnet/framework/serialization/xml-schema-def-tool-gen) while Json C# class is generated using NJsonSchema(https://github.com/RSuter/NJsonSchema).

Results

Options:
  L: List contents
  A: Add new person
  G: Generate multiple persons
  Q: Quit  
  Action? G

Enter number of persons:

100000

*****Protocole Buffer*****

Time to generate 100000 records (ms): 353

Total records: 100000

Time to save(ms): 199

*****Xml*****

Time to generate 100000 records (ms): 280

Total records: 100000

Time to save(ms): 1082

*****Json*****

Time to generate 100000 records (ms): 507

Total records: 100000

Time to save(ms): 1345

Options:
  L: List contents
  A: Add new person
  G: Generate multiple persons
  Q: Quit
Action? L

*****Protocole Buffer*****

File size (bytes): 7327936

Time to read(ms): 186

Total records: 100000

Time to display(ms): 36369

*****Xml*****
File size (bytes): 45433534

Time to read(ms): 889

Total records: 100000

Time to display(ms): 36454

*****Json*****

File size (bytes): 17833352

Time to read(ms): 1676

Total records: 100000

Time to display(ms): 36299

It's obvious based on previous results that ProtocolBuffer offer better performance in read/write to file and has small file size. It's followed in ouput size by Json. Concerning time to read/write, Xml seems to be better than Json.
