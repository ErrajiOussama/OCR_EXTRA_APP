using System;
using System.Collections.Generic;
using System.Text;

namespace OCR_EXTRA_APP.models
{
    internal class Delimitateur
    {
        private int id { get; set; }
        private string mot_cles { get; set; }
        private string lang { get; set; }

        public Delimitateur(int id, string mot_cles, string lang)
        {
            this.id = id;
            this.mot_cles = mot_cles;
            this.lang = lang;
        }
    }
}
