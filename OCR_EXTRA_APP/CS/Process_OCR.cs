using Google.Cloud.Vision.V1;
using Microsoft.Extensions.Configuration;
using Npgsql;
using OCR_EXTRA_APP.models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace OCR_EXTRA_APP.CS
{
    internal class Process_OCR
    {
        public static void Get_model(string[] acte_ocriser,int num_page)
        {
            string _connectbase = "";
            int comp3 = 0;
            int comp1 = 0;
            int num = 0;
            try
            {
                var builder = new ConfigurationBuilder().AddJsonFile($"./config.json").Build();
                _connectbase = builder["ConnexionString"];
                var sql = (new StreamReader(@"SQL/Get_matrice.sql")).ReadToEnd();
                using (var npgadapter = new NpgsqlDataAdapter(sql, _connectbase))
                {
                    DataTable dt = new DataTable();
                    npgadapter.Fill(dt);
                    List<Matrice> matriceList = Hlp.converting_matrice(dt);
                    List<Delimitateur> delimitateurs = GetExtraction_delimitateur(acte_ocriser);
                    comp3 = Hlp.number_delimitateur(num_page);
                    num = Hlp.number_of_matrice(dt);
                    for(int i = 2; i < num+1; i++)
                    {
                        foreach(Matrice matrice in matriceList)
                        {
                            foreach ( Delimitateur delimitateur in delimitateurs)
                            {
                                if(Int32.Parse(matrice.id_model) == i)
                                {
                                    if (Int32.Parse(matrice.num_page) == num_page )
                                    {                                        
                                        if (matrice.position == delimitateur.position.ToString())
                                        {
                                            comp1++;
                                        }
                                        if (matrice.iddelimitateur1 == delimitateur.id.ToString())
                                        {
                                            comp1++;
                                        }
                                    }         
                                }
                            }
                        }
                        Trace.WriteLine(comp1);
                        Trace.WriteLine(comp3);
                        double taux_finaux2 = ((float)comp1 / (comp3*2))*100 ;
                        Trace.WriteLine(taux_finaux2);
                        comp1 = 0;
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public static List<Delimitateur> GetExtraction_delimitateur(string[] acte_ocriser)
        {
            string _connexionbase = "";
            List<Delimitateur> list_delimitateur = new List<Delimitateur>();
            try
            {
                var builder = new ConfigurationBuilder().AddJsonFile($"./config.json").Build();
                _connexionbase = builder["ConnexionString"];
                var sql = (new StreamReader(@"SQL/Get_delimitateur.sql")).ReadToEnd();
                using (var npgadapter = new NpgsqlDataAdapter(sql, _connexionbase))
                {
                    DataTable dt = new DataTable();
                    npgadapter.Fill(dt);
                    List<Delimitateur> resultat = Hlp.converting_delimitateur(dt);
                    int comp = 0;
                    for (int i = 1; i < acte_ocriser.Length; i++)
                    {
                        foreach (Delimitateur delimitateur in resultat)
                        {
                            Regex regex = new Regex(delimitateur.mot_cles);
                            if (regex.IsMatch(acte_ocriser[i]))
                            {
                                comp++;
                                Delimitateur delimitateur_cles = new Delimitateur(delimitateur.id, delimitateur.mot_cles, delimitateur.lang,comp);
                                list_delimitateur.Add(delimitateur_cles);
                            }
                        }
                    }
                }
                return list_delimitateur;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static async Task<string[]> getOCRImage(string pathimage)
        {
            try
            {
                if (!string.IsNullOrEmpty(pathimage))
                {
                    FlowDocument flowDocument = new FlowDocument();
                    var client = ImageAnnotatorClient.Create();
                    Random random = new Random();
                    var image = Image.FromFile(pathimage);
                    var response = await Task.Run(() => client.DetectText(image));
                    System.Windows.Documents.Paragraph paragraph = new System.Windows.Documents.Paragraph();
                    string[] responseStr = new string[response.Count];
                    return response[0].Description.Split("\n");
                }
                else
                {
                    throw new Exception("Charger une image");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
