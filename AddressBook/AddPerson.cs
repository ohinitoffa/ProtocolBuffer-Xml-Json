#region Copyright notice and license
// Protocol Buffers - Google's data interchange format
// Copyright 2008 Google Inc.  All rights reserved.
// https://developers.google.com/protocol-buffers/
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are
// met:
//
//     * Redistributions of source code must retain the above copyright
// notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above
// copyright notice, this list of conditions and the following disclaimer
// in the documentation and/or other materials provided with the
// distribution.
//     * Neither the name of Google Inc. nor the names of its
// contributors may be used to endorse or promote products derived from
// this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System;
using System.IO;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Google.Protobuf.Examples.AddressBook
{
  internal class AddPerson
  {
    /// <summary>
    /// Builds a person based on user input
    /// </summary>
    private static object[] PromptForAddress(TextReader input, TextWriter output)
    {
      Person person = new Person();
      Xml.Person xmlPerson = new Xml.Person();
      Json.Person jsonPerson = new Json.Person();

      output.Write("Enter person ID: ");
      int id = int.Parse(input.ReadLine());
      person.Id = id;
      xmlPerson.Id = (uint)id;
      jsonPerson.Id = id;

      output.Write("Enter name: ");
      var name = input.ReadLine();
      person.Name = name;
      xmlPerson.Name = name;
      jsonPerson.Name = name;

      output.Write("Enter email address (blank for none): ");
      string email = input.ReadLine();
      if (email.Length > 0)
      {
        person.Email = email;
        xmlPerson.Email = email;
        jsonPerson.Email = email;
      }

      while (true)
      {
        output.Write("Enter a phone number (or leave blank to finish): ");
        string number = input.ReadLine();
        if (number.Length == 0)
        {
          break;
        }

        Person.Types.PhoneNumber phoneNumber = new Person.Types.PhoneNumber { Number = number };
        Xml.PersonPhoneNumber xmlPhoneNumber = new Xml.PersonPhoneNumber { Number = number };
        Json.PhoneNumber jsonPhoneNumber = new Json.PhoneNumber { Number = number };

        output.Write("Is this a mobile, home, or work phone? ");
        String type = input.ReadLine();
        switch (type)
        {
          case "mobile":
            phoneNumber.Type = Person.Types.PhoneType.Mobile;
            xmlPhoneNumber.Type = Xml.PersonPhoneNumberType.Mobile;
            jsonPhoneNumber.Type = Json.PhoneType.Mobile;
            break;
          case "home":
            phoneNumber.Type = Person.Types.PhoneType.Home;
            xmlPhoneNumber.Type = Xml.PersonPhoneNumberType.Home;
            jsonPhoneNumber.Type = Json.PhoneType.Home;
            break;
          case "work":
            phoneNumber.Type = Person.Types.PhoneType.Work;
            xmlPhoneNumber.Type = Xml.PersonPhoneNumberType.Work;
            jsonPhoneNumber.Type = Json.PhoneType.Work;
            break;
          default:
            output.Write("Unknown phone type. Using default.");
            break;
        }

        person.Phones.Add(phoneNumber);

        Xml.PersonPhoneNumber[] newValues;

        if (xmlPerson.Phones != null)
        {
          newValues = new Xml.PersonPhoneNumber[xmlPerson.Phones.Length + 1];
          Array.Copy(xmlPerson.Phones, newValues, xmlPerson.Phones.Length);
          newValues[xmlPerson.Phones.Length] = xmlPhoneNumber;
        }
        else
        {
          newValues = new Xml.PersonPhoneNumber[1];
          newValues[0] = xmlPhoneNumber;
        }

        xmlPerson.Phones = newValues;

        if(jsonPerson.Phones == null)
        {
          jsonPerson.Phones = new System.Collections.ObjectModel.ObservableCollection<Json.PhoneNumber>();
        }

        jsonPerson.Phones.Add(jsonPhoneNumber);
      }

      return new object[] { person, xmlPerson, jsonPerson };
    }

    /// <summary>
    /// Entry point - loads an existing addressbook or creates a new one,
    /// then writes it back to the file.
    /// </summary>
    public static int Main(string[] args)
    {
      if (args.Length != 3)
      {
        Console.Error.WriteLine("Usage:  AddPerson ADDRESS_BOOK_FILE");
        return -1;
      }

      AddressBook addressBook;
      Xml.AddressBook xmlAddressBook;
      Json.AddressBook jsonAddressBook;

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

      // Add an address.
      var persons = PromptForAddress(Console.In, Console.Out);
      addressBook.People.Add(persons[0] as Person);

      Xml.Person[] newValue;
      if (xmlAddressBook.people != null)
      {
        newValue = new Xml.Person[xmlAddressBook.people.Length + 1];
        Array.Copy(xmlAddressBook.people, newValue, xmlAddressBook.people.Length);
        newValue[xmlAddressBook.people.Length] = persons[1] as Xml.Person;
      }
      else
      {
        newValue = new Xml.Person[1];
        newValue[0] = persons[1] as Xml.Person;
      }

      xmlAddressBook.people = newValue;

      if(jsonAddressBook.People == null)
      {
        jsonAddressBook.People = new System.Collections.ObjectModel.ObservableCollection<Json.Person>();
      }

      jsonAddressBook.People.Add(persons[2] as Json.Person);

      // Write the new proto address book back to disk.
      using (Stream output = File.OpenWrite(args[0]))
      {
        addressBook.WriteTo(output);
      }

      // Write the new xml address book back to disk.
      using (Stream output = File.OpenWrite(args[1]))
      {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(Xml.AddressBook));
        xmlSerializer.Serialize(output, xmlAddressBook);
      }

      // Write the new json address book back to disk
      using (StreamWriter stream = new StreamWriter(args[2]))
      {
        JsonSerializer serializer = new JsonSerializer();
        JsonTextWriter writer = new JsonTextWriter(stream);
        serializer.Serialize(writer, jsonAddressBook, typeof(Json.AddressBook));
      }
      return 0;
    }
  }
}