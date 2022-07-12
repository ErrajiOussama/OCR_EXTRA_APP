using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Diagnostics;
using System.Windows;

namespace OCR_EXTRA_APP
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        string _connexionString = "";
        public Login()
        {
            InitializeComponent();
            try
            {
                // Chargement de la chaine de connexion
                var builder = new ConfigurationBuilder().AddJsonFile($"./config.json").Build();
                _connexionString = builder["ConnexionString"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Login1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(_connexionString))
                {
                    conn.Open();
                    var sqlLogin = $"select * from Compte where login=@login AND password=@password;";
                    using (NpgsqlCommand npgsqlCommand = new NpgsqlCommand(sqlLogin, conn))
                    {
                        npgsqlCommand.Parameters.Add(new NpgsqlParameter("@login", UserName.Text.Trim()));
                        npgsqlCommand.Parameters.Add(new NpgsqlParameter("@password", Password.Password.Trim()));

                        var reader = npgsqlCommand.ExecuteReader();

                        if (reader.HasRows)
                        {
                            Menu menu = new Menu();
                            menu.Show();
                            this.Close();
                        }
                        else
                            MessageBox.Show("erreur login ou mdp");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Trace.WriteLine(ex);
            }
        }
    }
    }

