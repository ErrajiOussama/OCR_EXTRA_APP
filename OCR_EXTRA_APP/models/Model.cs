using System;
using System.Collections.Generic;
using System.Text;

namespace OCR_EXTRA_APP.models
{
    internal class Model
    {
        public int id { get;private set; }
        public string type_model { get;private set; }
        public string version_type { get;private set; }

        public Model(int id, string type_model, string version_type)
        {
            this.id = id;
            this.type_model = type_model;
            this.version_type = version_type;
        }
    }
}
