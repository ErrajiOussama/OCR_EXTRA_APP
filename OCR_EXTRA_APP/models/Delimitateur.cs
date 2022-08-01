using System;
using System.Collections.Generic;
using System.Text;

namespace OCR_EXTRA_APP.models
{
    internal class Delimitateur
    {
        public int id { get; private set; }
        public string mot_cles { get; private set; }
        public string lang { get; private set; }

        public int position { get; private set; }
        public Delimitateur(int id, string mot_cles, string lang,int position )
        {
            this.id = id;
            this.mot_cles = mot_cles;
            this.lang = lang;
            this.position = position;
        }
    }
}
