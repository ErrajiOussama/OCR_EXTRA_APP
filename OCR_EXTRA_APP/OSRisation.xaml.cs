﻿using Microsoft.Extensions.Configuration;
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
            ContextMenu menuListLot = this.FindResource("cmListLotS") as ContextMenu;
            LotsList.ContextMenu = menuListLot;
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

                    #region Methode search database
                    //var type_value = "";
                    //switch(combo.SelectedIndex)
                    //{
                    //    case 0:
                    //        type_value = "l.id_bureau";
                    //        break;
                    //    case 1:
                    //        type_value = "l.id_commune";
                    //        break;
                    //    case 2:
                    //        type_value = "l.id_lot";
                    //        break;
                    //    default :
                    //        type_value = "l.id_lot";
                    //        break;
                    //}

                    //var sql = (new StreamReader(@"SQL/search_from_type.sql")).ReadToEnd().Replace("@type",type_value).Replace("@valeur",$"'%{search.Text.Trim()}%'");
                    //using (var dataAdapter = new NpgsqlDataAdapter(sql, _connexionString))
                    //{                    
                    //    DataTable dataTable = new DataTable();
                    //    dataAdapter.Fill(dataTable);
                    //    EmployeesList.ItemsSource = dataTable.DefaultView;
                    //}
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

        private void search_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Rechercher_Click(sender, e);
            }
        }

        private void Actualiser_Click(object sender, RoutedEventArgs e)
        {
            Load_Lots();
        }

        private void Ocriser_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
