using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPad
{
    public class DocumentTab
    {
        public bool TextHasChanged { get; set; }
        public bool DocumentIsSaved { get; set; }
        public string? DocumentName { get; set; }
        // For in-app house-keeping
        public int LineNumber { get; set; }
        public string FindTextString { get; set; }
        public int FindLastIndexFound { get; set; }

        public DocumentTab()
        {
            this.TextHasChanged = false;
            this.DocumentIsSaved = false;
            this.DocumentName = "Untitled";
            this.LineNumber = 0;
            this.FindTextString = "";
            this.FindLastIndexFound = 0;
        }

        public DocumentTab(bool text, bool doc, string name)
        {
            this.TextHasChanged = text;
            this.DocumentIsSaved = doc;
            this.DocumentName = name;
            this.LineNumber = 0;
            this.FindTextString = "";
            this.FindLastIndexFound = 0;
        }
    }
}