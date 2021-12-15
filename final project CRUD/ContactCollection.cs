using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace final_project_CRUD
{
    public class ContactCollection
    {
        public static List<Contact> contacts = new List<Contact>();
        public static List<Contact> contactInfos = new List<Contact>();
        public static List<Contact> importedCSVContacts = new List<Contact>();

        public static int contactID;

        public static Contact AddContact(string firstName, string lastName, int age, string email = null, string phoneNumber = null)
        {
            Contact contact = new Contact()
            {
                FirstName = firstName,
                LastName = lastName,
                Age = age,
                Email = email,
                PhoneNumber = phoneNumber
            };

            return contact;
        }

        public static void ExportCSV()
        {
            SaveFileDialog exportContacts = new SaveFileDialog();
            exportContacts.Filter = "(*.csv) | *.csv";

            var stringBuilder = new StringBuilder();

            foreach (var contact in contacts)
            {
                stringBuilder.AppendLine(contact.FirstName + "," +
                        contact.LastName + "," +
                        contact.Age + "," +
                        contact.Email + "," +
                        contact.PhoneNumber);
            }

            if (exportContacts.ShowDialog() == true)
            {
                File.WriteAllText(exportContacts.FileName, stringBuilder.ToString());

            }
        }

        public static void ImportCSV()
        {
            var databaseInstance = DatabaseHandler.Instance;

            importedCSVContacts.Clear();

            OpenFileDialog importContacts = new OpenFileDialog();

            string[] importedContacts = null;

            if (importContacts.ShowDialog() == true)
            {
                importedContacts = File.ReadAllLines(importContacts.FileName);

                foreach (var lines in importedContacts)
                {
                    string[] contactInfo = lines.Split(',');

                    if (contactInfo.Length == 5)
                    {
                        if (contactInfo[3] == "")
                        {
                            Contact contact = new Contact()
                            {
                                FirstName = contactInfo[0],
                                LastName = contactInfo[1],
                                Age = Convert.ToInt32(contactInfo[2]),
                                Email = null,
                                PhoneNumber = contactInfo[4]
                            };
                            importedCSVContacts.Add(contact);
                            continue;
                        }
                        if (contactInfo[4] == "")
                        {
                            Contact contact = new Contact()
                            {
                                FirstName = contactInfo[0],
                                LastName = contactInfo[1],
                                Age = Convert.ToInt32(contactInfo[2]),
                                Email = contactInfo[3],
                                PhoneNumber = null
                            };
                            importedCSVContacts.Add(contact);
                            continue;
                        }
                        else
                        {
                            Contact contact = new Contact()
                            {
                                FirstName = contactInfo[0],
                                LastName = contactInfo[1],
                                Age = Convert.ToInt32(contactInfo[2]),
                                Email = contactInfo[3],
                                PhoneNumber = contactInfo[4]
                            };
                            importedCSVContacts.Add(contact);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Your CSV file isn't compatible to import", "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                databaseInstance.AddImportedCSV();
            }
        }
    }

    public class Contact
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}

    

