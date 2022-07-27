using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

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
                    
                    var sql = (new StreamReader(@"SQL/Get_User.sql")).ReadToEnd();
                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(new NpgsqlParameter("@login", UserName.Text.Trim()));
                        cmd.Parameters.Add(new NpgsqlParameter("@password", Password.Password.Trim()));

                        var reader = cmd.ExecuteReader();

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

        private void Password_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                try
                {
                    using (NpgsqlConnection conn = new NpgsqlConnection(_connexionString))
                    {
                        conn.Open();

                        var sql = (new StreamReader(@"SQL/Get_User.sql")).ReadToEnd();
                        using (var cmd = new NpgsqlCommand(sql, conn))
                        {
                            cmd.Parameters.Add(new NpgsqlParameter("@login", UserName.Text.Trim()));
                            cmd.Parameters.Add(new NpgsqlParameter("@password", Password.Password.Trim()));

                            var reader = cmd.ExecuteReader();

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
    }

