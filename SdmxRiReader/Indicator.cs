using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdmxRiReader
{
    class Indicator
    {
        private string code                 = "";
        private string domain_id            = "";
        private string name                 = "";
        private string gradeValues          = "";
        private string gradeColors          = "";
        private string measure              = "";
        private string note                 = "";
        private string sourceWebLink        = "";
        private string colorScale           = "";

        public string Code
        {
            get { return code; }
            set { code = value; }
        }

        public string Domain_Id
        {
            get { return domain_id; }
            set { domain_id = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string GradeValues
        {
            get { return gradeValues; }
            set { gradeValues = value; }
        }

        public string GradeColors
        {
            get { return gradeColors; }
            set { gradeColors = value; }
        }

        public string Measure
        {
            get { return measure; }
            set { measure = value; }
        }

        public string Note
        {
            get { return note; }
            set { note = value; }
        }

        public string SourceWebLink
        {
            get { return sourceWebLink; }
            set { sourceWebLink = value; }
        }
        public string ColorScale
        {
            get { return colorScale; }
            set { colorScale = value; }
        }

    }
}
