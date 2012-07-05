using System;
using System.Collections.Generic;
using System.Text;

namespace RenderStroke2
{
    enum PenType
    {
        Ink,
        Eraser,
        UndoInk,
        UndoEraser
    }

    enum StrokeCreation
    {
        New,
        Undo, 
        Redo
    }

    class TemplateInfo
    {
        public int TimesEntered { get; set; }
        public int UID { get; set; }
        public string TemplateName { get; set; }
        public string TemplateStrength {get;set;}
    }

    class Stroke
    {
        public List<TemplateInfo> Templates = new List<TemplateInfo>();

        public StrokeCreation StrokeType { get; set; }
        public PenType PenType { get; set; }
        public int StrokeWidth { get; set; }
        public int LineNumber { get; set; }
        public DateTime Timestamp { get; set; }
        public int SimultaneousMaxTemplatesUsed { get; set; }
        public bool Recorded { get; set; }
        public System.Windows.Rect Viewbox { get; set; }

        public System.Windows.Media.PointCollection InputMouse = new System.Windows.Media.PointCollection();
        public System.Windows.Media.PointCollection OutputStroke = new System.Windows.Media.PointCollection();

        public override string ToString()
        {
            return "Line " + LineNumber.ToString();
        }
    }

    
}
