using System;
using System.Collections.Generic;
using System.Text;

namespace OCR_EXTRA_APP.models
{
    internal class Model
    {
        private int id { get; set; }
        private string type_model { get; set; }
        private string version_type { get; set; }
        private string path_model { get; set; }

        public Model(int id, string type_model, string version_type, string path_model)
        {
            this.id = id;
            this.type_model = type_model;
            this.version_type = version_type;
            this.path_model = path_model;
        }
    }
}
