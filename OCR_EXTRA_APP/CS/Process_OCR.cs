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
            GetExtraction_delimitateur(acte_ocriser);
            string _connectbase = "";
            int comp1 = 0;
            int comp2 = 0;
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
                    foreach(Delimitateur delimitateur in delimitateurs)
                    {
                        foreach(Matrice matrice in matriceList)
                        {
                            if (matrice.num_page == num_page.ToString())
                            {
                                if (matrice.position == delimitateur.position.ToString())
                                {
                                    comp1++;
                                }
                                if (matrice.iddelimitateur1 == delimitateur.id.ToString())
                                {
                                    comp2++;
                                }
                            }
                        }
                        
                    }
                    if (num_page == 1)
                    {
                        double taux_id = (comp1 / 7.0) * 100;
                        double taux_position = (comp2 / 7.0) * 100;
                        Trace.WriteLine(taux_id);
                        Trace.WriteLine(taux_position);
                    }
                    else
                    {
                        double taux_id = (comp1 / 27.0) * 100;
                        double taux_position = (comp2 / 27.0) * 100;
                        Trace.WriteLine(taux_id);
                        Trace.WriteLine(taux_position);
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
