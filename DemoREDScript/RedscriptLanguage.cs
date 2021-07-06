using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Syncfusion.Windows.Edit;

namespace DemoREDScript
{
    public class RedscriptLanguage : ProceduralLanguageBase
    {
        public RedscriptLanguage(EditControl control) : base(control)
        {
            Name = "Redscript";
            FileExtension = "reds";
            ApplyColoring = true;
            SupportsIntellisense = false; // TODO
            SupportsOutlining = true;
        }
    }
}
