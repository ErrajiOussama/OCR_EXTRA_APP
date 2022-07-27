using System;
using System.Collections.Generic;
using System.Text;

namespace OCR_EXTRA_APP.CS
{
    internal class Matrice
    {
        private int id { get; set; }
        private string id_model { get; set; }
        private string iddelimitateur1 { get; set; }
        private string iddelimitateur2 { get; set; }
        private string _position { get; set; }

        public Matrice(int id, string id_model, string iddelimitateur1, string iddelimitateur2, string position)
        {
            this.id = id;
            this.id_model = id_model;
            this.iddelimitateur1 = iddelimitateur1;
            this.iddelimitateur2 = iddelimitateur2;
            _position = position;
        }
    }
}
