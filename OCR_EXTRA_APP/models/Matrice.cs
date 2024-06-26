﻿using System;
using System.Collections.Generic;
using System.Text;

namespace OCR_EXTRA_APP.models
{
    internal class Matrice
    {
        public int id { get; private set; }
        public string id_model { get; private set; }
        public string iddelimitateur1 { get; private set; }
        public string iddelimitateur1_val { get; private set; }
        public string iddelimitateur2 { get; private set; }
        public string iddelimitateur2_val { get; private set; }
        public int position { get; private set; }
        public string num_page { get; private set; }
        public Matrice(int id, string id_model, string iddelimitateur1, string iddelimitateur2, int position, string num_page)
        {
            this.id = id;
            this.id_model = id_model;
            this.iddelimitateur1 = iddelimitateur1;
            this.iddelimitateur2 = iddelimitateur2;
            this.position = position;
            this.num_page = num_page;
        }
        public Matrice(int id, string id_model, string iddelimitateur1,string iddelimitateur1_val, string iddelimitateur2, string iddelimitateur2_val ,int position, string num_page)
        {
            this.id = id;
            this.id_model = id_model;
            this.iddelimitateur1 = iddelimitateur1;
            this.iddelimitateur1_val=iddelimitateur1_val;
            this.iddelimitateur2 = iddelimitateur2;
            this.iddelimitateur2_val=iddelimitateur2_val;
            this.position = position;
            this.num_page = num_page;
        }
    }
}
