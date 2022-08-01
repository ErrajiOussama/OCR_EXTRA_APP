using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using OCR_EXTRA_APP.models;

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

    }
}
