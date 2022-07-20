using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OCR_EXTRA_APP
{
    /// <summary>
    /// Interaction logic for OSRisation.xaml
    /// </summary>
    public partial class OSRisation : Page
    {
        DataTable _dataTableListLot;
        string _connexionString = "";
        public OSRisation()
        {

            InitializeComponent();
            Load_Lots();
        }
        private void Load_Lots()
        {
            try
            {
                var builder = new ConfigurationBuilder().AddJsonFile($"./config.json").Build();
                _connexionString = builder["ConnexionString2"];


                var sql = (new StreamReader(@"SQL/Get_Lots.sql")).ReadToEnd();
                using (var npgsqlDataAdapter = new NpgsqlDataAdapter(sql, _connexionString))
                {
                    _dataTableListLot = new DataTable();
                    npgsqlDataAdapter.Fill(_dataTableListLot);
                    LotsList.ItemsSource = _dataTableListLot.DefaultView;
                    //LotsList_ocr.ItemsSource = _dataTableListLot.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Rechercher_Click(object sender, RoutedEventArgs e)
        {

        }

        private void combo_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    var builder = new ConfigurationBuilder().AddJsonFile($"./config.json").Build();
                    _connexionString = builder["ConnexionString2"];


                    var sql = (new StreamReader(@"SQL/Get_Lots.sql")).ReadToEnd();
                    using (var npgsqlDataAdapter = new NpgsqlDataAdapter(sql, _connexionString))
                    {
                        _dataTableListLot = new DataTable();
                        npgsqlDataAdapter.Fill(_dataTableListLot);
                        LotsList.ItemsSource = _dataTableListLot.DefaultView;
                        //LotsList_ocr.ItemsSource = _dataTableListLot.DefaultView;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
