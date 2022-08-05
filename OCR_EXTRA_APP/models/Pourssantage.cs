using System;
using System.Collections.Generic;
using System.Text;

namespace OCR_EXTRA_APP.models
{
    internal class Pourssantage
    {
        public int id_model;
        public double pourssantage;

        public Pourssantage()
        {
        }

        public Pourssantage(int id_model, double pourssantage)
        {
            this.id_model = id_model;
            this.pourssantage = pourssantage;
        }
    }
}
