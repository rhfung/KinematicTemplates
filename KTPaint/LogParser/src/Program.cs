using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

using TemplateID = System.String;
using TemplateLog = System.Collections.Generic.Dictionary<string, string>;

namespace RenderStroke2
{

    class Program
    {
        
        const int SESSION = 0;
        const int TIME_FROM_EPOCH = 1;
        const int DATE_STAMP = 2;
        const int ACTION = 3;
        const int ITEM = 4;
        const int ITEM_ID = 5;
        const int OTHER = 6;
        const int OTHER_2 = 7;

        class SupportedString
        {
            public int LineNumber { get; set; }
            public string Line { get; set; }
        }

        struct AppState
        {
            public string ViewBox;
            public PenType PenType;
            public string LastUsedStrength;
            public Dictionary<TemplateID, TemplateLog> Templates;
            public int EraserSize;
        }

        static void Main(string[] args)
        {
            string filename ;
            // ensure arguments is correct
            if (args.Length != 1)
            {
                System.Console.WriteLine("RenderStroke2: <filename>");
                System.Console.WriteLine();
                System.Console.WriteLine("Instructions:");
                System.Console.WriteLine();
                System.Console.WriteLine("1. To locate Kinematic Templates log files, create and save a drawing.");
                System.Console.WriteLine("2. Open the File menu and select Developer Options.");
                System.Console.WriteLine("3. Copy the text in \"Log Directory\" and paste into the dialog box here.");
                System.Console.WriteLine("4. Select the log file of a previous drawing you created.");

                // show dialog
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                
                dlg.CheckFileExists = true;
                dlg.Filter = "Kinematic Template Log Files (*.log)|*.log";
                dlg.Title = "Open Log File";
                bool? ret = dlg.ShowDialog();
                if (ret.HasValue)
                {
                    if (ret.Value == true)
                    {
                        filename = dlg.FileName;
                    }
                    else
                        return;
                }
                else
                {
                    return;
                }


            }
            else
            {
                filename = args[0];
            }

            // load file into a queue
            int c = 0;
            Queue<SupportedString> lines = new Queue<SupportedString>();
            try
            {
                using (TextReader reader = new StreamReader(filename))
                {
                    string line = reader.ReadLine();
                    while (line != null)
                    {
                        c++;
                        lines.Enqueue(new SupportedString() { Line = line, LineNumber = c });
                        line = reader.ReadLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot read the log file: " + ex.Message);
                Environment.Exit(1);
            }

            // quit if empty
            if (lines.Count == 0)
            {
                System.Console.WriteLine("RenderStroke2: log file is empty");
                return;
            }

            ThreadStart threadParam = new ThreadStart(() =>
                {
                    // app state
                    AppState s = new AppState() 
                        { ViewBox = "", 
                        PenType = PenType.Ink,
                        Templates = new Dictionary<TemplateID,TemplateLog>(),
                        EraserSize = 16};

                    // split up lines
                    string[] split = null;

                    // place to put all the stuff
                    Queue<Stroke> strokes = new Queue<Stroke>();
                    Stack<Stroke> undoStack = new Stack<Stroke>();
                    Stack<Stroke> redoStack = new Stack<Stroke>();

                    while (lines.Count > 0)
                    {
                        split = lines.Peek().Line.Split('/');

                        // do some processing here
                        if ("viewbox".Equals(split[ACTION]))
                            s.ViewBox = split[OTHER];

                        else if ("pen".Equals(split[ACTION]) || "use_pen".Equals(split[ACTION])
                            || "goto_draw_mode".Equals(split[ACTION]))
                            s.PenType = PenType.Ink;

                        else if ("eraser".Equals(split[ACTION]) || "use_eraser".Equals(split[ACTION])
                            || "goto_eraser_mode".Equals(split[ACTION]))
                            s.PenType = PenType.Eraser;

                        else if ("eraser_size".Equals(split[ACTION]))
                            s.EraserSize = int.Parse(split[OTHER]);
                        
                        else if ("undo".Equals(split[ACTION]))
                        {
                            if (undoStack.Count > 0)
                            {
                                Stroke q = UndoStroke(lines, undoStack, StrokeCreation.Undo);
                                strokes.Enqueue(q);
                                redoStack.Equals(q);
                            }
                        }

                        else if ("redo".Equals(split[ACTION]))
                        {
                            if (redoStack.Count > 0)
                            {
                                Stroke q = UndoStroke(lines, redoStack, StrokeCreation.Redo);
                                strokes.Enqueue(q);
                                undoStack.Equals(q);
                            }
                        }

                        else if ("stroke_started".Equals(split[ACTION]))
                        {
                            Stroke q = ExtractStroke(lines, s);
                            strokes.Enqueue(q);
                            undoStack.Push(q);
                        }
                        else
                        {
                            ExtractTemplate(split, s);
                        }

                        if (lines.Count > 0)
                            lines.Dequeue();
                    }


                    Window1 window = new Window1();
                    window.Title = filename;
                    foreach (var i in strokes)
                    {
                        window.listBox1.Items.Add(i);
                    }
                    window.ShowDialog();
                }
                );
            Thread test = new Thread(threadParam);
            test.SetApartmentState(ApartmentState.STA);  //Many WPF UI elements need to be created inside STA
            test.Start();
        }

        static void ExtractTemplate(string [] curLine, AppState p)
        {
            if (curLine[ITEM] != "mouse" && curLine[ITEM] != "ndhand_pointer"
                && curLine[ITEM] != "drawing")
            {
                // record any global template strength (CDPaint5)
                if ("change_template_strength".Equals(curLine[ACTION]))
                {
                    p.LastUsedStrength = curLine[OTHER] + "/" + curLine[OTHER_2];
                }

                
                // find the template log
                TemplateID templateID = curLine[ITEM] + "[" + curLine[ITEM_ID] + "]";
                TemplateLog c;
                if (p.Templates.ContainsKey(templateID))
                {
                    c = p.Templates[templateID];
                }
                else
                {
                    c = new TemplateLog();
                    p.Templates.Add(templateID, c);
                }
                // assign dictionary entry
                if (!c.ContainsKey(curLine[ACTION]))
                {
                    c.Add(curLine[ACTION], "");
                }
                c[curLine[ACTION]] = curLine[OTHER] + "/" + curLine[OTHER_2];
            }
        }

        static Stroke ExtractStroke(Queue<SupportedString> lines, AppState p)
        {
            Stroke s = new Stroke();
            
            // assign a view box to this stroke
            if (p.ViewBox.Length > 0)
            {
                string [] parts = p.ViewBox.Split(new char[] {'{','=',',','}'});
                s.Viewbox = new System.Windows.Rect(
                    Double.Parse( parts[2]),
                    Double.Parse(parts[4]),
                    Double.Parse(parts[6]),
                    Double.Parse(parts[8]));
            }

            {
                SupportedString line = lines.Dequeue();
                s.LineNumber = line.LineNumber;
                string dateTimeSpecial = line.Line.Split(new char[] { '/' })[2];
                dateTimeSpecial = dateTimeSpecial.Substring(0, dateTimeSpecial.IndexOf(',') - 1);
                s.Timestamp = DateTime.Parse(dateTimeSpecial);
            }

            s.PenType = p.PenType;
            if (p.PenType == PenType.Ink || p.PenType == PenType.UndoInk)
                s.StrokeWidth = 2;
            else
                s.StrokeWidth = p.EraserSize;
            s.StrokeType = StrokeCreation.New;
            
            string[] split = lines.Peek().Line.Split('/');
            while (!"stroke_ended".Equals(split[ACTION]))
            {
                if ("_input_stroke".Equals(split[ACTION]))
                {
                    s.InputMouse =  ShiftPoints( GetPointsFromString(split[OTHER]), new System.Windows.Vector(-s.Viewbox.Left,-s.Viewbox.Top));
                }
                else if ("_template_used".Equals(split[ACTION]))
                {
                    // get some template info
                    string templateKey = split[ITEM] + "[" + split[ITEM_ID] + "]";
                    if (!p.Templates.ContainsKey(templateKey))
                    {
                        // force extraction of template if it didn't exist before
                        Console.WriteLine("template " + templateKey + " used without being defined");
                        ExtractTemplate(split, p);
                    }
                    TemplateLog tl =  p.Templates[templateKey];
                    string strength = "default";
                    if (tl.ContainsKey("change_template_strength"))
                        strength = tl["change_template_strength"];

                    s.Templates.Add(new TemplateInfo() 
                        {
                            TemplateName = split[ITEM].Replace("TemplateFilter",""),
                            TimesEntered = int.Parse( split[OTHER]),
                            UID = int.Parse(split[ITEM_ID]),
                            TemplateStrength = strength
                        });
                    
                }
                else if ("_simultaneous_templates_max_used".Equals(split[ACTION]))
                {
                    s.SimultaneousMaxTemplatesUsed = int.Parse(split[OTHER]);
                }
                else if ("_output_stroke".Equals(split[ACTION]))
                {
                    s.OutputStroke = ShiftPoints(GetPointsFromString(split[OTHER]), new System.Windows.Vector(-s.Viewbox.Left,-s.Viewbox.Top));
                }
                else if ("_stroke_path".Equals(split[ACTION]))
                {
                    if (s.OutputStroke.Count == 0)
                    {
                        s.OutputStroke = ShiftPoints(GetPointsFromString(split[OTHER]), new System.Windows.Vector(-s.Viewbox.Left,-s.Viewbox.Top));
                        if (s.OutputStroke.Count > 0)
                            System.Console.WriteLine("_stroke_path used because _output_stroke missing");
                    }
                        
                }
                else if ("replay_recorded_movement".Equals(split[ACTION]))
                {
                    s.Recorded = true;
                }
                else
                {
                    System.Console.WriteLine("Could not understand {0}", split[ACTION]);
                }

                lines.Dequeue();
                split = lines.Peek().Line.Split('/');
            }

            return s;
        }

        static System.Windows.Media.PointCollection GetPointsFromString(string input)
        {
            System.Windows.Media.PointCollection pts = new System.Windows.Media.PointCollection();
            string[] splitter = input.Split(' ');
            foreach (string point in splitter)
            {
                string[] parts = point.Split(',');
                if (parts.Length == 2)
                {
                    pts.Add(new System.Windows.Point(double.Parse(parts[0]), double.Parse(parts[1])));
                }
            }
            return pts;
        }

        static Stroke UndoStroke(Queue<SupportedString> lines, Stack<Stroke> undoStack, StrokeCreation strokeType)
        {
            Stroke toCopy = undoStack.Pop();
            Stroke newOne = new Stroke();
            newOne.LineNumber = lines.Peek().LineNumber;
            newOne.PenType = UndoOf(toCopy.PenType);
            newOne.Recorded = false;
            newOne.SimultaneousMaxTemplatesUsed = 0;
            newOne.Viewbox = toCopy.Viewbox;
            newOne.Templates = new List<TemplateInfo>();
            newOne.InputMouse = toCopy.InputMouse;
            newOne.OutputStroke = toCopy.OutputStroke;
            newOne.StrokeType = strokeType;

            return newOne;
        }

        static PenType UndoOf(PenType p)
        {
            switch (p)
            {
                case PenType.Eraser: return PenType.UndoEraser;
                case PenType.Ink: return PenType.UndoInk;
                case PenType.UndoEraser: return PenType.Eraser;
                case PenType.UndoInk: return PenType.Ink;
                default: return PenType.Ink;
            }

        }

        static System.Windows.Media.PointCollection ShiftPoints(System.Windows.Media.PointCollection input, System.Windows.Vector shift)
        {
            System.Windows.Media.PointCollection pc = new System.Windows.Media.PointCollection(input.Count);
            foreach (System.Windows.Point s in input)
            {
                pc.Add(s + shift );
            }
            return pc;
        }
    }
}