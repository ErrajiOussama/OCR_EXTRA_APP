using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OCR_EXTRA_APP
{
    internal class Acces_Images
    {
                
        public string getPathLot(string idLot, string cs, string[] _pathImageRepository)
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
