using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace OCR_EXTRA_APP
{
    /// <summary>
    /// Interaction logic for Etat_Civil.xaml
    /// </summary>
    public partial class Etat_Civil : Page
    {
        string _connexionString = "";
        public Etat_Civil()
        {
            InitializeComponent();

            ContextMenu menuListLot = this.FindResource("cmListLot") as ContextMenu;
            EmployeesList.ContextMenu = menuListLot;

            try
            {
                // Chargement de la chaine de connexion
                var builder = new ConfigurationBuilder().AddJsonFile($"./config.json").Build();
                _connexionString = builder["ConnexionString"];
                var sqlcomptes = $"select id,login,password,role from Compte;";
                using (NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter(sqlcomptes, _connexionString))
                {
                    DataTable dataTable = new DataTable();
                    npgsqlDataAdapter.Fill(dataTable);
                    EmployeesList.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
