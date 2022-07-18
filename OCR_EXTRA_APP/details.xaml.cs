using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Linq;

namespace OCR_EXTRA_APP
{
    /// <summary>
    /// Interaction logic for details.xaml
    /// </summary>
    public partial class details : Window
    {
        DataTable _dataTableListLot;
        string _id_lot = "";
        string _connexionString;
        public details()
        {
            InitializeComponent();
            try
            {
                var builder = new ConfigurationBuilder().AddJsonFile($"./config.json").Build();
                _connexionString = builder["ConnexionString2"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public details(string id_lot):this()
        {
            _id_lot = id_lot;
            txtLotTitre.Text = id_lot;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var sql = (new StreamReader(@"SQL/Actes.sql")).ReadToEnd().Replace("@lots", _id_lot);
                using (var dataAdapter = new NpgsqlDataAdapter(sql, _connexionString))
                {
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);
                    Actes.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
