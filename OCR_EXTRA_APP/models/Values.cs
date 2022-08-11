using System;
using System.Collections.Generic;
using System.Text;

namespace OCR_EXTRA_APP.models
{
    internal class Values
    {
        public string nom_de_champ { private set; get; }
        public string valeur { private set; get; }

        public Values()
        {

        }
       public Values(string nom_de_champ, string valeur)
        {
            this.nom_de_champ = nom_de_champ;
            this.valeur = valeur;
        }
    }
}
