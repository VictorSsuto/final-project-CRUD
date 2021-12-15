using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace final_project_CRUD
{
    class DatabaseHandler
    {
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
        
