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

namespace OCR_EXTRA_APP
{
    /// <summary>
    /// Interaction logic for details.xaml
    /// </summary>
    public partial class details : Window
    {
        #region declaration des variables
        string _id_lot = "";
        string _connexionString;
        string[] _pathImageRepository;
        string _Extra;
        #endregion
        public details()
        {
            InitializeComponent();
            #region cnx avec la base de donner 
            try
            {
                var builder = new ConfigurationBuilder().AddJsonFile($"DATA/config.json").Build();
                _connexionString = builder["ConnexionString2"];
                _pathImageRepository = builder.GetSection("ListPaths").GetChildren().AsEnumerable().Select(e => e.Value).ToArray<string>();
                _Extra = builder["ConnexionString3"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            #endregion
        }
        #region constructeur pour recuperer le id lot de la page etat civil
        public details(string id_lot):this()
        {
            var item = new TreeViewItem();
            _id_lot = id_lot;            
        }
        #endregion
        //every time you open this window you call this function 
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            #region recuperer les donners des actes de l'id lot   
            try
            {         
                
                var sql = (new StreamReader(@"SQL/Get_num_acte.sql")).ReadToEnd().Replace("@lots", _id_lot);                
                using (var dataAdapter = new NpgsqlDataAdapter(sql, _connexionString))
                {
                    
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);
                    

                    TreeViewItem LotTree = new TreeViewItem();
                    LotTree.Header = _id_lot;
                    foreach(DataRow row in dataTable.Rows)
                    {
                        TreeViewItem treeViewItem = new TreeViewItem();
                        treeViewItem.Header = row["num_acte"].ToString();
                        treeViewItem.Tag = row;
                        treeViewItem.MouseDoubleClick += TreeViewItem_MouseDoubleClick;                        
                        LotTree.Items.Add(treeViewItem);
                    }

                    TreeActesLots.Items.Add(LotTree);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            #endregion
        }

        private void openImage(string[] path)
        {
            FixedDocument fixedDocument = new FixedDocument();
            foreach (string file in path)
            {
                if(!string.IsNullOrEmpty(file) && File.Exists(file))
                {                  
                    System.Windows.Media.ImageSource imageSource = BitmapFrame.Create(new Uri(file), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                    Image image = new Image();
                    image.Source = imageSource;
                    FixedPage fixedPage = new FixedPage();
                    fixedPage.Width = imageSource.Width;
                    fixedPage.Height = imageSource.Height;
                    fixedPage.Children.Add(image);            
                    PageContent pageContent = new PageContent();
                    pageContent.Child = fixedPage;
                    fixedDocument.Pages.Add(pageContent);
                }
                else
                {
                    MessageBox.Show("Chemin introuvable");
                }
            }
            ouvrirImage.Document = fixedDocument;
        }
        private void TreeViewItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            #region recuperer les donner de l'id acte choisis par le utilisateur
            try
            {
                
                var id_acte = ((sender as TreeViewItem).Tag as DataRow)["id_acte"].ToString();
                var path_image = ((sender as TreeViewItem).Tag as DataRow)["imagepath"].ToString();
                var sql2 = (new StreamReader(@"SQL/Get_Actes.sql")).ReadToEnd().Replace("@acte", id_acte);
                using (var dataAdapter = new NpgsqlDataAdapter(sql2, _connexionString))
                {
                    DataTable dt = new DataTable();
                    dataAdapter.Fill(dt);
                    List<ActefieldSchemat> actefieldSchemats = new List<ActefieldSchemat>();
                    foreach(var cl in dt.Columns)
                    {
                        actefieldSchemats.Add(new ActefieldSchemat() { Champ = cl.ToString(), Valeur = dt.Rows[0][cl.ToString()].ToString() });
                    }
                    Champ.ItemsSource =  actefieldSchemats;
                    string[] path_image1=path_image.Split(";;").Where(e=> !string.IsNullOrWhiteSpace(e)).ToArray();
                    Acces_Images acces_images= new Acces_Images();
                    string cheminRacine = acces_images.getPathLot(_id_lot, _Extra, _pathImageRepository);
                    Trace.WriteLine(path_image);
                    Trace.WriteLine(cheminRacine);
                    if (path_image1.Length == 1) {
                        string imagepath1 = Path.Combine(acces_images.getPathLot(_id_lot, _Extra, _pathImageRepository), path_image1[0]);
                        openImage(new string[] { imagepath1 });
                    }
                    else 
                    {
                        string imagepath1 = Path.Combine(acces_images.getPathLot(_id_lot, _Extra, _pathImageRepository), path_image1[0]);
                        string imagepath2 = Path.Combine(acces_images.getPathLot(_id_lot, _Extra, _pathImageRepository), path_image1[1]);
                        openImage(new string[] { imagepath1,imagepath2 });
                    }
                }
              
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            #endregion
        }

        internal class ActefieldSchemat
        {
            public string Champ { get; set; }
            public string Valeur { get; set; }
        }


       
    }
}
