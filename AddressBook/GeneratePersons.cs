using System;
using System.IO;
using System.Xml.Serialization;
using System.Diagnostics;
using Newtonsoft.Json;

namespace Google.Protobuf.Examples.AddressBook
{
  internal class GeneratePersons
  {
    private static int LastProtoId = 0;
    private static uint LastXmlId = 0;
    private static int LastJsonId = 0;
    static Stopwatch watch = new Stopwatch();

    /// <summary>
    /// Builds a new person
    /// </summary>
    private static Person GeneratePerson()
    {
      Person person = new Person();
      person.Id = LastProtoId;

      person.Name = $"Name{LastProtoId}";
      person.Email = $"Email{LastProtoId}";
      Person.Types.PhoneNumber phoneNumber = new Person.Types.PhoneNumber { Number = $"mobile{LastProtoId}", Type = Person.Types.PhoneType.Mobile };
      person.Phones.Add(phoneNumber);
      phoneNumber = new Person.Types.PhoneNumber { Number = $"home{LastProtoId}", Type = Person.Types.PhoneType.Home };
      person.Phones.Add(phoneNumber);
      phoneNumber = new Person.Types.PhoneNumber { Number = $"work{LastProtoId}", Type = Person.Types.PhoneType.Work };
      person.Phones.Add(phoneNumber);
      LastProtoId++;
      return person;
    }

    /// <summary>
    /// Builds a new person
    /// </summary>
    private static Xml.Person GenerateXmlPerson()
    {
      Xml.Person person = new Xml.Person();
      person.Id = LastXmlId;

      person.Name = $"Name{LastXmlId}";
      person.Email = $"Email{LastXmlId}";
      person.Phones = new Xml.PersonPhoneNumber[3];
      Xml.PersonPhoneNumber xmlPhoneNumber = new Xml.PersonPhoneNumber { Number = $"mobile{LastXmlId}", Type = Xml.PersonPhoneNumberType.Mobile };
      person.Phones[0] = xmlPhoneNumber;
      xmlPhoneNumber = new Xml.PersonPhoneNumber { Number = $"home{LastXmlId}", Type = Xml.PersonPhoneNumberType.Home };
      person.Phones[1] = xmlPhoneNumber;
      xmlPhoneNumber = new Xml.PersonPhoneNumber { Number = $"work{LastXmlId}", Type = Xml.PersonPhoneNumberType.Work };
      person.Phones[2] = xmlPhoneNumber;
      LastXmlId++;
      return person;
    }

    /// <summary>
    /// Builds a new person
    /// </summary>
    private static Json.Person GenerateJsonPerson()
    {
      Json.Person person = new Json.Person();
      person.Id = LastJsonId;
      person.Name = $"Name{LastJsonId}";
      person.Email = $"Email{LastJsonId}";
      person.Phones = new System.Collections.ObjectModel.ObservableCollection<Json.PhoneNumber>();
      person.Phones.Add(new Json.PhoneNumber { Number = $"mobile{LastJsonId}", Type = Json.PhoneType.Mobile });
      person.Phones.Add(new Json.PhoneNumber { Number = $"home{LastJsonId}", Type = Json.PhoneType.Home });
      person.Phones.Add(new Json.PhoneNumber { Number = $"work{LastJsonId}", Type = Json.PhoneType.Work });
      LastJsonId++;
      return person;
    }

    /// <summary>
    /// Entry point - loads an existing addressbook or creates a new one,
    /// then writes it back to the file.
    /// </summary>
    public static int Main(string[] args)
    {
      if (args.Length != 3)
      {
        Console.Error.WriteLine("Usage:  GeneratePersons ADDRESS_BOOK_FILE");
        return -1;
      }

      AddressBook addressBook;
      Xml.AddressBook xmlAddressBook;
      Json.AddressBook jsonAddressBook;
      long protoSave = 0, protoGen = 0, protoCount = 0,
        xmlSave = 0, xmlGen = 0, xmlCount = 0,
      jsonSave = 0, jsonGen = 0, jsonCount = 0;

      if (File.Exists(args[0]))
      {
        using (Stream file = File.OpenRead(args[0]))
        {
          addressBook = AddressBook.Parser.ParseFrom(file);
        }
      }
      else
      {
        Console.WriteLine("{0}: File not found. Creating a new file.", args[0]);
        addressBook = new AddressBook();
      }

      if (File.Exists(args[1]))
      {
        using (Stream file = File.OpenRead(args[1]))
        {
          XmlSerializer xmlSerializer = new XmlSerializer(typeof(Xml.AddressBook));
          xmlAddressBook = xmlSerializer.Deserialize(file) as Xml.AddressBook;
        }
      }
      else
      {
        Console.WriteLine("{0}: File not found. Creating a new file.", args[1]);
        xmlAddressBook = new Xml.AddressBook();
      }

      if (File.Exists(args[2]))
      {
        using (StreamReader stream = new StreamReader(args[2]))
        {
          JsonSerializer serializer = new JsonSerializer();
          JsonTextReader reader = new JsonTextReader(stream);
          jsonAddressBook = serializer.Deserialize(reader, typeof(Json.AddressBook)) as Json.AddressBook;
        }
      }
      else
      {
        Console.WriteLine("{0}: File not found. Creating a new file.", args[2]);
        jsonAddressBook = new Json.AddressBook();
      }

      Console.WriteLine("Enter number of persons: ");
      string persons = Console.ReadLine();
      int nPersons = 0;
      while(!Int32.TryParse(persons, out nPersons))
      {
        Console.WriteLine("Enter Numeric Value");
        persons = Console.ReadLine();
      }

      // Add addresses proto.
      watch.Restart();
      for(int i = 0; i < nPersons; ++i)
      {
        addressBook.People.Add(GeneratePerson());
      }
      watch.Stop();
      protoGen = watch.ElapsedMilliseconds;
      protoCount = addressBook.People.Count;

      // Add addresses Xml.
      watch.Restart();
      Xml.Person[] newValue;
      int startIndex;
      if (xmlAddressBook.people != null)
      {
        newValue = new Xml.Person[xmlAddressBook.people.Length + nPersons];
        Array.Copy(xmlAddressBook.people, newValue, xmlAddressBook.people.Length);
        startIndex = xmlAddressBook.people.Length;
      }
      else
      {
        newValue = new Xml.Person[nPersons];
        startIndex = 0;
      }

      for (int i = 0; i < nPersons; ++i)
      {
        newValue[i + startIndex] = GenerateXmlPerson();
      }

      xmlAddressBook.people = newValue;
      watch.Stop();
      xmlGen = watch.ElapsedMilliseconds;
      xmlCount = xmlAddressBook.people.LongLength;

      // Add addresses json.
      watch.Restart();
      if(jsonAddressBook.People == null)
      {
        jsonAddressBook.People = new System.Collections.ObjectModel.ObservableCollection<Json.Person>();
      }

      for (int i = 0; i < nPersons; ++i)
      {
        jsonAddressBook.People.Add(GenerateJsonPerson());
      }
      watch.Stop();
      jsonGen = watch.ElapsedMilliseconds;
      jsonCount = addressBook.People.Count;

      // Write the new proto address book back to disk.
      watch.Restart();
      using (Stream output = File.OpenWrite(args[0]))
      {
        addressBook.WriteTo(output);
      }
      watch.Stop();
      protoSave = watch.ElapsedMilliseconds;

      // Write the new xml address book back to disk.
      watch.Restart();
      using (Stream output = File.OpenWrite(args[1]))
      {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(Xml.AddressBook));
        xmlSerializer.Serialize(output, xmlAddressBook);
      }
      watch.Stop();
      xmlSave = watch.ElapsedMilliseconds;

      // Write the new json address book back to disk.
      watch.Restart();
      using (StreamWriter stream = new StreamWriter(args[2]))
      {
        JsonSerializer serializer = new JsonSerializer();
        JsonTextWriter writer = new JsonTextWriter(stream);
        serializer.Serialize(writer, jsonAddressBook, typeof(Json.AddressBook));
      }
      watch.Stop();
      jsonSave = watch.ElapsedMilliseconds;

      Console.WriteLine("*****Protocole Buffer*****");
      Console.WriteLine($"Time to generate {nPersons} records (ms): {protoGen}");
      Console.WriteLine($"Total records: {protoCount}");
      Console.WriteLine($"Time to save(ms): {protoSave}");

      Console.WriteLine("*****Xml*****");
      Console.WriteLine($"Time to generate {nPersons} records (ms): {xmlGen}");
      Console.WriteLine($"Total records: {xmlCount}");
      Console.WriteLine($"Time to save(ms): {xmlSave}");

      Console.WriteLine("*****Json*****");
      Console.WriteLine($"Time to generate {nPersons} records (ms): {jsonGen}");
      Console.WriteLine($"Total records: {jsonCount}");
      Console.WriteLine($"Time to save(ms): {jsonSave}");

      return 0;
    }
  }
}
