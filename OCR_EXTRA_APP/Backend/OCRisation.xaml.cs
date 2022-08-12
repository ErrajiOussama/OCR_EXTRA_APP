using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Windows.Documents;
using System.Diagnostics;
using System.Windows.Input;
using Google.Cloud.Vision.V1;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Image = Google.Cloud.Vision.V1.Image;
using OCR_EXTRA_APP.CS;
using OCR_EXTRA_APP.models;

namespace OCR_EXTRA_APP
{
    /// <summary>
    /// Interaction logic for OSRisation.xaml
    /// </summary>
    public partial class OCRisation : System.Windows.Controls.Page
    {
        DataTable _dataTableListLot;
        string _connexionString = "";
        string[] _pathTexte;
        string[] _pathImageRepository;
        string _Extra;
        public OCRisation()
        {
            InitializeComponent();
            Load_Lots();
        }
        private void Load_Lots()
        {
            try
            {
                var builder = new ConfigurationBuilder().AddJsonFile($"DATA/config.json").Build();
                _connexionString = builder["ConnexionString2"];
                _pathImageRepository = builder.GetSection("ListPaths").GetChildren().AsEnumerable().Select(e => e.Value).ToArray<string>();
                _Extra = builder["ConnexionString3"];
                _pathTexte = builder.GetSection("ListPathsTexte").GetChildren().AsEnumerable().Select(e => e.Value).ToArray<string>();
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
                    switch (combo2.SelectedIndex)
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

        private async void lunchOCR_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView dataRow = LotsList.SelectedItem as DataRowView;
                string _id_lot = dataRow["id_Lot"].ToString();
                string _path_image_acte = "";
                
                string id_acte = "";
                var builder = new ConfigurationBuilder().AddJsonFile($"DATA/config.json").Build();
                _connexionString = builder["ConnexionString2"];
                Random random = new Random();
                var sql = (new StreamReader(@"SQL/Get_random_acte.sql")).ReadToEnd().Replace("@lots", _id_lot);
                using (var npgsqlDataAdapter = new NpgsqlDataAdapter(sql, _connexionString))
                {
                    DataTable dataTable = new DataTable();
                    npgsqlDataAdapter.Fill(dataTable);
                    _path_image_acte = dataTable.Rows[0]["imagepath"].ToString();
                    id_acte = dataTable.Rows[0]["id_acte"].ToString();
                }

                if (!string.IsNullOrEmpty(_id_lot))
                {
                    Acces_Images acces_Images = new Acces_Images();
                    FlowDocument flowDocument = new FlowDocument();
                    string _path = acces_Images.getPathLot(_id_lot, _Extra, _pathImageRepository);
                    string[] path_image1 = _path_image_acte.Split(";;").Where(e => !string.IsNullOrWhiteSpace(e)).ToArray();
                    if (path_image1.Length == 1)
                    {
                        string[] resultat1 =  await Process_OCR.getOCRImage(Path.Combine(_path, path_image1[0]));
                        File.WriteAllText(Path.Combine(_pathTexte[0], $"{_id_lot}_{id_acte}_{path_image1[0].Replace("jpg","txt")}"), string.Join("\n", resultat1));
                    }
                    else
                    {
                        string imagepath1 = path_image1[0];
                        string imagepath2 = path_image1[1];
                        string[] resultat2 = await Process_OCR.getOCRImage(Path.Combine(_path, imagepath1));
                        string[] resultat3 = await Process_OCR.getOCRImage(Path.Combine(_path, imagepath2));
                        string Actes = _id_lot + imagepath1.Replace("jpg", "txt");
                        string first_path = Path.Combine(Path.Combine(_pathTexte[0],Actes ));
                        string second_path = Path.Combine(Path.Combine(_pathTexte[0], _id_lot + "_" + imagepath2.Replace("jpg", "txt")));

                        File.WriteAllText(first_path, string.Join("\n", resultat2));
                        File.WriteAllText(second_path, string.Join("\n", resultat3));
                        List<Delimitateur> list = Process_OCR.GetExtraction_delimitateur(resultat2);
                        List<Delimitateur> delimitateurs = Process_OCR.GetExtraction_delimitateur(resultat3);
                        string[] resultat1 = Hlp.delet_caracter(resultat2);
                        string[] res2 = Hlp.delet_caracter(resultat3);                                       
                        Process_OCR.Get_extraction_values(res2, 2, id_acte);
                    }
                }
                else
                {
                    MessageBox.Show("Charger une image", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                if (LotsList.SelectedItem != null)
                {
                    DataRowView datarow = LotsList.SelectedItem as DataRowView;
                    OCRiser  oCRiser= new OCRiser(datarow["id_lot"].ToString());
                    oCRiser.Show();
                }
            }
             

            catch (Exception ex)
            {
                    Trace.WriteLine(ex);
                    MessageBox.Show(ex.Message);
            }
        }
    }
}
