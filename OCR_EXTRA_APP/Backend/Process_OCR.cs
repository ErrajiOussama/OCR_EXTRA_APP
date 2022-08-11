using Google.Cloud.Vision.V1;
using Microsoft.Extensions.Configuration;
using Npgsql;
using OCR_EXTRA_APP.models;
using System;
using System.Linq;
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
        public static Model Get_model(string[] acte_ocriser,int num_page)
        {
            string _connectbase = "";
            int comp3 = 0;
            int comp1 = 0;
            int num = 0;
            try
            {
                var builder = new ConfigurationBuilder().AddJsonFile($"DATA/config.json").Build();
                _connectbase = builder["ConnexionString"];
                var sql = (new StreamReader(@"SQL/Get_matrice.sql")).ReadToEnd();
                List<Pourssantage> pourssantages = new List<Pourssantage>(); 
                using (var npgadapter = new NpgsqlDataAdapter(sql, _connectbase))
                {
                    DataTable dt = new DataTable();
                    npgadapter.Fill(dt);
                    List<Matrice> matriceList = Hlp.converting_matrice(dt);
                    List<Delimitateur> delimitateurs = GetExtraction_delimitateur(acte_ocriser);
                    num = Hlp.number_of_matrice(dt);
                   
                    for(int i = 2; i <= num+1; i++)
                    {
                        foreach(Matrice matrice in matriceList)
                        {
                            foreach ( Delimitateur delimitateur in delimitateurs)
                            {
                                if(Int32.Parse(matrice.id_model) == i)
                                {
                                 
                                    comp3 = Hlp.number_delimitateur(num_page, Int32.Parse(matrice.id_model));
                                   
                                    if (Int32.Parse(matrice.num_page) == num_page )
                                    {                                        
                                        if (matrice.position == delimitateur.position && matrice.iddelimitateur1 == delimitateur.id.ToString())
                                        {
                                            comp1++;
                                        }
                                    }         
                                }
                            }
                        }
       
                        double taux_finaux2 = ((float)comp1 / comp3)*100;
                        Trace.WriteLine(taux_finaux2);
                        comp1 = 0;
                        Pourssantage pour = new Pourssantage(i, taux_finaux2);
                        pourssantages.Add(pour);                       
                    }
                    Model model = Hlp.detecter_le_model(pourssantages);
                    return model;
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
                var builder = new ConfigurationBuilder().AddJsonFile($"DATA/config.json").Build();
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
                            Regex regex = new Regex(delimitateur.mot_cles.Trim());
                            if (regex.IsMatch(acte_ocriser[i].Trim()))
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

        public static List<Values> Get_extraction_values(string[] acte_ocriser, int num_page)
        {
            List<Values> values = new List<Values>();
            int i=5;
            string[] names = Hlp.get_name_colums();
            Model model = Get_model(acte_ocriser, num_page);
            Values values1 = get_num_acte(acte_ocriser, num_page);
            values.Add(values1);
            List<Pourssantage> pourssantages = new List<Pourssantage>();
            List<Matrice> matrices = Hlp.remplissage_de_valeur_delimitateur(num_page, model.id, acte_ocriser);
            matrices.Where(m=>m.iddelimitateur1 != "1").ToList().ForEach(m =>
            {
                // Method 1
                var val = getExtractSameLine(m, acte_ocriser);

                // Method 2
                //if(string.IsNullOrWhiteSpace(val))
                //{
                //    val = getExtractionLineAfter(matrices, m, acte_ocriser);
                //}
                values.Add(new Values(names[i], val));
                i++;
            });        
            foreach (var val in values)
            {
                Trace.WriteLine($"{val.nom_de_champ} : {val.valeur}");
            }
            return values;
        }
        
        public static string getExtractSameLine(Matrice m, string[] acte_ocriser)
        {
            return acte_ocriser.Any(a => Regex.IsMatch(a, $"^{m.iddelimitateur1_val}")) ?
                acte_ocriser.FirstOrDefault(a => Regex.IsMatch(a, $"^{m.iddelimitateur1_val}"))
                .Replace($"^{m.iddelimitateur1_val}", "")
                : "";
        }

        public static string getExtractionLineAfter(List<Matrice> matrices,Matrice m, string[] acte_ocriser)
        {
            var delimit2_m2 = matrices.Any(m1 => m1.position > m.position) ? matrices.FirstOrDefault(m1 => m1.position > m.position).iddelimitateur1_val : "";
            var lignes = Regex.IsMatch(string.Join(" ", acte_ocriser), $"[{m.iddelimitateur1_val}]([aA-zZ]|[\u0621-\u064A\u0660-\u0669])+([{m.iddelimitateur2_val}]|[{delimit2_m2}])") ? "fouded" : "";
            return lignes;
        }

        public static Values get_num_acte(string[] acte_ocriser,int num_page)
        {
            Values values = new Values();
            var regex = new Regex("[0-9]");
            string[] names = Hlp.get_name_colums();
            if (num_page == 1)
            {
                for (int i = 0; i < 10; i++)
                {
                    if (regex.IsMatch(acte_ocriser[i]))
                    {
                        values = new Values(names[4], acte_ocriser[i]);                               
                    }
                }
                return values;
            }
            else
            {
                for (int i = 0; i < 20; i++)
                {
                    if (regex.IsMatch(acte_ocriser[i]))
                    {
                        Values values1 = new Values(names[4], acte_ocriser[i]);
                        return values1;
                    }
                }
            }
            return null;
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
