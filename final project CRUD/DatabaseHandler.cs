using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace final_project_CRUD
{
    public class DatabaseHandler
    {
        public static DatabaseHandler Instance = new DatabaseHandler();

        string connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
        string createContactQuery = "INSERT INTO Contact([First Name], [Last Name], Age, Email, [Phone Number]) VALUES(@firstName, @lastName, @age, @email, @phoneNumber)";
        string viewContactQuery = "SELECT * FROM Contact";
        string updateContactQuery = "UPDATE Contact SET [First Name] = @firstName, [Last Name] = @lastName, Age = @age, Email = @email, [Phone Number] = @phoneNumber WHERE ID = @id";
        string deleteContactQuery = "DELETE FROM Contact WHERE ID = @id";

        public void CreateContact()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(createContactQuery, connection);
                    try
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@firstName", ContactCollection.contacts.ElementAt(ContactCollection.contacts.Count() - 1).FirstName);
                    command.Parameters.AddWithValue("@lastName", ContactCollection.contacts.ElementAt(ContactCollection.contacts.Count() - 1).LastName);
                    command.Parameters.AddWithValue("@age", ContactCollection.contacts.ElementAt(ContactCollection.contacts.Count() - 1).Age);
                    command.Parameters.AddWithValue("@email", ContactCollection.contacts.ElementAt(ContactCollection.contacts.Count() - 1).Email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@phoneNumber", ContactCollection.contacts.ElementAt(ContactCollection.contacts.Count() - 1).PhoneNumber ?? (object)DBNull.Value);

                    var rowsAffected = command.ExecuteNonQuery();

                    MessageBox.Show("You have successfully inserted the records", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (SqlException e)
                {
                    MessageBox.Show("Something went wrong" + e.ToString(), "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);


                }
            }
        }
        public void ViewContact()
        {
            SyncContacts(ContactCollection.contacts);
        }

        public void UpdateContact(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(updateContactQuery, connection);

                try
                {
                    connection.Open();

                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@firstName", ContactCollection.contacts.ElementAt(ContactCollection.contactID - 1).FirstName);
                    command.Parameters.AddWithValue("@lastName", ContactCollection.contacts.ElementAt(ContactCollection.contactID - 1).LastName);
                    command.Parameters.AddWithValue("@age", ContactCollection.contacts.ElementAt(ContactCollection.contactID - 1).Age);
                    command.Parameters.AddWithValue("@email", ContactCollection.contacts.ElementAt(ContactCollection.contactID - 1).Email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@phoneNumber", ContactCollection.contacts.ElementAt(ContactCollection.contactID - 1).PhoneNumber ?? (object)DBNull.Value);

                    var rowsAffected = command.ExecuteNonQuery();

                    MessageBox.Show($"You have successfully updated your contact's information!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (SqlException e)
                {
                    MessageBox.Show("Something went wrong" + e.ToString(), "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        public void DeleteContact(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(deleteContactQuery, connection);

                try
                {
                    connection.Open();

                    command.Parameters.AddWithValue("@id", id);

                    var rowsAffected = command.ExecuteNonQuery();

                    MessageBox.Show($"You have successfully deleted this contact!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (SqlException e)
                {
                    MessageBox.Show("Something went wrong" + e.ToString(), "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                SyncContacts(ContactCollection.contacts);
                RefreshedRecords();
            }
        }

        public void SyncContacts(List<Contact> contacts)
        {
            ContactCollection.contacts.Clear();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(viewContactQuery, connection);

                try
                {
                    connection.Open();

                    SqlDataReader dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {

                        contacts.Add(ContactCollection.AddContact(
                            dataReader["First Name"].ToString(),
                            dataReader["Last Name"].ToString(),
                            Convert.ToInt32(dataReader["Age"]),
                            dataReader["Email"].ToString(),
                            dataReader["Phone Number"].ToString()));

                    }
                }
                catch (SqlException e)
                {
                    MessageBox.Show("Something went wrong" + e.ToString(), "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        public void RefreshedRecords()
        {
            DeleteRecords();
            RefreshContact();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand($"INSERT INTO Contact([First Name], [Last Name], Age, Email, [Phone Number]) VALUES(@firstName, @lastName, @age, @email, @phoneNumber)", connection);

                try
                {
                    connection.Open();

                    command.Parameters.AddWithValue("@firstName", DbType.String);
                    command.Parameters.AddWithValue("@lastName", DbType.String);
                    command.Parameters.AddWithValue("@age", DbType.String);
                    command.Parameters.AddWithValue("@email", DbType.String);
                    command.Parameters.AddWithValue("@phoneNumber", DbType.String);

                    foreach (var contact in ContactCollection.contacts)
                    {
                        if (contact.Email == "")
                        {
                            command.Parameters[0].Value = contact.FirstName;
                            command.Parameters[1].Value = contact.LastName;
                            command.Parameters[2].Value = contact.Age;
                            command.Parameters[3].Value = DBNull.Value;
                            command.Parameters[4].Value = contact.PhoneNumber;

                            command.ExecuteNonQuery();
                            continue;
                        }
                        if (contact.PhoneNumber == "")
                        {
                            command.Parameters[0].Value = contact.FirstName;
                            command.Parameters[1].Value = contact.LastName;
                            command.Parameters[2].Value = contact.Age;
                            command.Parameters[3].Value = contact.Email;
                            command.Parameters[4].Value = DBNull.Value;

                            command.ExecuteNonQuery();
                            continue;
                        }
                        else
                        {
                            command.Parameters[0].Value = contact.FirstName;
                            command.Parameters[1].Value = contact.LastName;
                            command.Parameters[2].Value = contact.Age;
                            command.Parameters[3].Value = contact.Email;
                            command.Parameters[4].Value = contact.PhoneNumber;

                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch (SqlException e)
                {
                    MessageBox.Show("Something went wrong" + e.ToString(), "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        public void DeleteRecords()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("DELETE FROM Contact", connection);

                try
                {
                    connection.Open();

                    SqlDataReader dataReader = command.ExecuteReader();
                }
                catch (SqlException e)
                {
                    MessageBox.Show("Something went wrong" + e.ToString(), "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        public void RefreshContact()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("DBCC CHECKIDENT(Contact, RESEED, 0)", connection);

                try
                {
                    connection.Open();

                    SqlDataReader dataReader = command.ExecuteReader();
                }
                catch (SqlException e)
                {
                    MessageBox.Show("Something went wrong" + e.ToString(), "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void AddImportedCSV()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand($"INSERT INTO Contact([First Name], [Last Name], Age, Email, [Phone Number]) VALUES(@firstName, @lastName, @age, @email, @phoneNumber)", connection);

                try
                {
                    connection.Open();

                    command.Parameters.AddWithValue("@firstName", DbType.String);
                    command.Parameters.AddWithValue("@lastName", DbType.String);
                    command.Parameters.AddWithValue("@age", DbType.String);
                    command.Parameters.AddWithValue("@email", DbType.String);
                    command.Parameters.AddWithValue("@phoneNumber", DbType.String);

                    foreach (var contact in ContactCollection.importedCSVContacts)
                    {
                        if (contact.Email == null)
                        {
                            command.Parameters[0].Value = contact.FirstName;
                            command.Parameters[1].Value = contact.LastName;
                            command.Parameters[2].Value = contact.Age;
                            command.Parameters[3].Value = DBNull.Value;
                            command.Parameters[4].Value = contact.PhoneNumber;

                            command.ExecuteNonQuery();
                            continue;
                        }
                        if (contact.PhoneNumber == null)
                        {
                            command.Parameters[0].Value = contact.FirstName;
                            command.Parameters[1].Value = contact.LastName;
                            command.Parameters[2].Value = contact.Age;
                            command.Parameters[3].Value = contact.Email;
                            command.Parameters[4].Value = DBNull.Value;

                            command.ExecuteNonQuery();
                            continue;
                        }
                        else
                        {
                            command.Parameters[0].Value = contact.FirstName;
                            command.Parameters[1].Value = contact.LastName;
                            command.Parameters[2].Value = contact.Age;
                            command.Parameters[3].Value = contact.Email;
                            command.Parameters[4].Value = contact.PhoneNumber;

                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch (SqlException e)
                {
                    MessageBox.Show("Something went wrong" + e.ToString(), "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
        
