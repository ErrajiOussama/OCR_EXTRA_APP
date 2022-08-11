using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using OCR_EXTRA_APP.models;
using Microsoft.Extensions.Configuration;
using System.IO;
using Npgsql;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace OCR_EXTRA_APP.CS
{
    internal class Hlp
    {

        public static List<Delimitateur> converting_delimitateur(DataTable dataTable)
        {
            List<Delimitateur> delimitateurList = new List<Delimitateur>();
            int i = 0;
            foreach (DataRow dr in dataTable.Rows)
            {
                delimitateurList.Add(new Delimitateur(Int32.Parse(dr["id"].ToString()), dr["mot_cles"].ToString(), dr["lang"].ToString(),i));
                i++;
            }
            return delimitateurList;
        }

        public static List<Matrice> converting_matrice(DataTable dataTable)
        {
            List<Matrice> matriceList = new List<Matrice>();
            int i = 0;
            foreach (DataRow dr in dataTable.Rows)
            {
                matriceList.Add(new Matrice(Int32.Parse(dr["id"].ToString()), dr["id_model"].ToString(), dr["iddelimitateur1"].ToString(), dr["iddelimitateur2"].ToString(), Int32.Parse(dr["_position"].ToString()), dr["num_page"].ToString()));
                i++;
            }
            return matriceList;
        }
        public static List<Matrice> remplissage_de_valeur_delimitateur(int num_page, int id_modele, string[] acte_ocriser)
        {
            string valeur_de_delimitateur1 = "";
            string valeur_de_delimitateur2 = "";
            string _connectbase = "";
            try
            {
                var builder = new ConfigurationBuilder().AddJsonFile($"DATA/config.json").Build();
                _connectbase = builder["ConnexionString"];
                var sql = (new StreamReader(@"SQL/Get_num_delimitateur_from_matrice.sql")).ReadToEnd().Replace("@num_page", num_page.ToString()).Replace("@id_modele", id_modele.ToString());
                using (var npgadapter = new NpgsqlDataAdapter(sql, _connectbase))
                {
                    DataTable data = new DataTable();
                    npgadapter.Fill(data);
                    List<Matrice> matriceList = Hlp.converting_matrice(data);
                    List<Matrice> matrices = new List<Matrice>();
                    List<Delimitateur> delimitateurs = Process_OCR.GetExtraction_delimitateur(acte_ocriser);
                    foreach (var mat in matriceList)
                    {
                        foreach (Delimitateur delimitateur in delimitateurs)
                        {
                            if (mat.iddelimitateur1 == delimitateur.id.ToString())
                            {
                                valeur_de_delimitateur1 = delimitateur.mot_cles;
                            }
                            if (mat.iddelimitateur2 == delimitateur.id.ToString())
                            {
                                valeur_de_delimitateur2 = delimitateur.mot_cles;
                            }
                        }
                        Matrice matrice = new Matrice(mat.id, mat.id_model, mat.iddelimitateur1, valeur_de_delimitateur1, mat.iddelimitateur2, valeur_de_delimitateur2, mat.position, mat.num_page);
                        matrices.Add(matrice);
                    }
                    return matrices;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static int number_delimitateur( int num_page,int id_modele)
        {
            string n = "";
            string _connectbase = "";
            try
            {
                var builder = new ConfigurationBuilder().AddJsonFile($"DATA/config.json").Build();
                _connectbase = builder["ConnexionString"];
                var sql = (new StreamReader(@"SQL/Get_num_delimitateur_from_matrice.sql")).ReadToEnd().Replace("@num_page", num_page.ToString()).Replace("@id_modele",id_modele.ToString());
                using (var npgadapter = new NpgsqlDataAdapter(sql, _connectbase))
                {
                    DataTable data = new DataTable();
                    npgadapter.Fill(data);
                    foreach (DataRow dr in data.Rows)
                    {
                        n = dr["_position"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Int32.Parse(n);
        }

        public static int number_of_matrice(DataTable dataTable)
        {
            string n = "";
            foreach(DataRow dr in dataTable.Rows)
            {
                n = dr["id_model"].ToString();
            }
            return Int32.Parse(n);
        }

        public static string[] get_name_colums()
        {
            
            string _connectbase = "";
            int i = 0;
            try
            {
                var builder = new ConfigurationBuilder().AddJsonFile($"DATA/config.json").Build();
                _connectbase = builder["ConnexionString"];
                var sql = (new StreamReader(@"SQL/get_colums_acte_ocr.sql")).ReadToEnd();
                using (var npgadapter = new NpgsqlDataAdapter(sql, _connectbase))
                {
                    DataTable data = new DataTable();
                    npgadapter.Fill(data);
                    string[] names = new string[data.Rows.Count];
                    foreach (DataRow dr in data.Rows)
                    {
                        names[i] = dr["column_name"].ToString();
                        i++;
                    }
                   return names;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    
        public static string[] delet_caracter(string[] acte_ocriser)
        {
            string[] resultat=new string[acte_ocriser.Length];
            Regex regex = new Regex(":");
            Regex regex2 = new Regex(".");

            for (int i = 0; i < acte_ocriser.Length; i++) 
            {
                    resultat[i] = acte_ocriser[i].Replace(":", "").Replace(".", "");                
            }
       
            return resultat;
        }

        public static Model detecter_le_model(List<Pourssantage> pourssantages )
        {
            string _connectbase = "";
            try
            {
                var builder = new ConfigurationBuilder().AddJsonFile($"DATA/config.json").Build();
                _connectbase = builder["ConnexionString"];
               
                double max = 0;
                foreach(Pourssantage pourssantage in pourssantages)
                {
                    if (pourssantage.pourssantage > max)
                    {
                        max = pourssantage.pourssantage;
                    }
                }
                foreach(Pourssantage pourssantage in pourssantages)
                {
                    if (pourssantage.pourssantage == max)
                    {
                        var sql2 = (new StreamReader(@"SQL/Get_modele.sql")).ReadToEnd().Replace("@id_modele",pourssantage.id_model.ToString())
;                       using (var dataAdapter = new NpgsqlDataAdapter(sql2, _connectbase))
                        {
                            DataTable dataTable = new DataTable();
                            dataAdapter.Fill(dataTable);
                            Model model = new Model(Int32.Parse(dataTable.Rows[0]["id"].ToString()), dataTable.Rows[0]["type_model"].ToString(), dataTable.Rows[0]["version_model"].ToString());
                            return model;
                        }
                        
                    }
                }
                return null;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
