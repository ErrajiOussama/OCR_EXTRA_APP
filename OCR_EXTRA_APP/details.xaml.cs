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
                var builder = new ConfigurationBuilder().AddJsonFile($"./config.json").Build();
                _connexionString = builder["ConnexionString2"];
                _pathImageRepository = builder.GetSection("ListPaths").GetChildren().AsEnumerable().Select(e=>e.Value).ToArray<string>();
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
                if(file != null)
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
                    string[] path_image1=path_image.Split(";;");
                    string cheminRacine = getPathLot(_id_lot, _Extra);
                    if (path_image1.Length == 1) {
                        string imagepath1 = Path.Combine(getPathLot(_id_lot, _Extra), path_image1[0]);
                        openImage(new string[] { imagepath1 });
                    }
                    else 
                    {
                        string imagepath1 = Path.Combine(getPathLot(_id_lot, _Extra), path_image1[0]);
                        string imagepath2 = Path.Combine(getPathLot(_id_lot, _Extra), path_image1[1]);
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


        public string getPathLot(string idLot, string cs)
        {
            try
            {
                // récupération du chemin du lot 
                // formatage des informations
                // exemple format -- '1 2012 001 2403 03'
                string typlot = (idLot[0].ToString() == "1") ? "NA" : (idLot[0].ToString() == "2") ? "DE" : (idLot[0].ToString() == "3") ? "JM" : (idLot[0].ToString() == "4") ? "TR" : "ER";
                string annee = idLot[1].ToString() + idLot[2] + idLot[3] + idLot[4];
                string tome = idLot[5].ToString() + idLot[6] + idLot[7];
                string idbec = idLot[8].ToString() + idLot[9] + idLot[10] + idLot[11];
                string indice = idLot[12].ToString();
                string idcom = "";
                string tome_indice = (indice == "0") ? Int32.Parse(tome).ToString() : Int32.Parse(tome) + "_" + indice;

                // récupération du com
                using (var con = new NpgsqlConnection(cs))
                {
                    con.Open();

                    var sql = (new StreamReader(@"SQL/recupIdComExtraDb.sql")).ReadToEnd().Replace("@id_bec", idbec);

                    using (var cmd = new NpgsqlCommand(sql, con))
                    {
                        var resCom = cmd.ExecuteReader();
                        while (resCom.Read())
                        {
                            idcom = resCom["id_com"].ToString();
                        }
                        resCom.Close();
                    }
                }                

                // formatage du chemin et retour
                string[] ListPathImages = _pathImageRepository.ToArray();

                for (int i = 0; i < ListPathImages.Length; i++)
                {
                    if (Directory.Exists(Path.Combine(ListPathImages[i], idcom, idbec, annee, typlot, tome_indice)))
                    {
                        ListPathImages[i] = Path.Combine(ListPathImages[i], idcom, idbec, annee, typlot, tome_indice);
                    }
                    else
                    {
                        ListPathImages[i] = "";
                    }
                }

                return (ListPathImages.FirstOrDefault(p => !string.IsNullOrWhiteSpace(p)) != null) ? ListPathImages.FirstOrDefault(p => !string.IsNullOrWhiteSpace(p)) : "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
