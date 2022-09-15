using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OCR_EXTRA_APP
{
    /// <summary>
    /// Interaction logic for Correction.xaml
    /// </summary>
    public partial class Correction : Page
    {
        DataTable _dataTableListLot;
        string _connexionString;
        public Correction()
        {
            InitializeComponent();
            Load_Lots_ocr();
        }

        private void Load_Lots_ocr()
        {
            try
            {
                var builder = new ConfigurationBuilder().AddJsonFile($"DATA/config.json").Build();
                _connexionString = builder["ConnexionString2"];


                var sql = (new StreamReader(@"SQL/Get_Lots.sql")).ReadToEnd();
                using (var npgsqlDataAdapter = new NpgsqlDataAdapter(sql, _connexionString))
                {
                    _dataTableListLot = new DataTable();
                    npgsqlDataAdapter.Fill(_dataTableListLot);
                    LotsList.ItemsSource = _dataTableListLot.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Rechercher_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if (!string.IsNullOrEmpty(search.Text))
                {
                    #region Methode Search
                    var type_value = "";
                    switch (combo.SelectedIndex)
                    {
                        case 0:
                            type_value = "id_bureau";
                            break;
                        case 1:
                            type_value = "id_commune";
                            break;
                        case 2:
                            type_value = "id_lot";
                            break;
                        default:
                            type_value = "id_lot";
                            break;
                    }
                    DataTable searchTable = _dataTableListLot.Copy();
                    searchTable.DefaultView.RowFilter = $"convert({type_value}, System.String) like '%{search.Text.Trim()}%'";
                    LotsList.ItemsSource = searchTable.DefaultView;
                    #endregion
                }
                else
                {
                    LotsList.ItemsSource = _dataTableListLot.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ContextMenu menuListLot = this.FindResource("cmListLot") as ContextMenu;
            LotsList.ContextMenu = menuListLot;
        }
        private void Coriger_Btn(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LotsList.SelectedItem != null)
                {
                    DataRowView dataRow = LotsList.SelectedItem as DataRowView;
                    Corriger corriger = new Corriger(dataRow["id_lot"].ToString());
                    corriger.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Actualiser_Btn(object sender, RoutedEventArgs e)
        {
            Load_Lots_ocr();
        }
        private void search_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Rechercher_Click(sender, e);
            }
        }
    }
}
