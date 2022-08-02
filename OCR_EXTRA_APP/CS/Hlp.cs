using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using OCR_EXTRA_APP.models;
using Microsoft.Extensions.Configuration;
using System.IO;
using Npgsql;

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
                matriceList.Add(new Matrice(Int32.Parse(dr["id"].ToString()), dr["id_model"].ToString(), dr["iddelimitateur1"].ToString(), dr["iddelimitateur2"].ToString(), dr["_position"].ToString(), dr["num_page"].ToString()));
                i++;
            }
            return matriceList;
        }

        public static int number_delimitateur( int test)
        {
            string n = "";
            string _connectbase = "";
            try
            {
                var builder = new ConfigurationBuilder().AddJsonFile($"./config.json").Build();
                _connectbase = builder["ConnexionString"];
                var sql = (new StreamReader(@"SQL/Get_num_delimitateur_from_matrice.sql")).ReadToEnd().Replace("@num_page", test.ToString());
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
    }
}
