// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using KinTemplates;
using KinTemplates.Cursor;
using KinTemplates.Cursor.Logger;
using KinTemplates.Cursor.Position;
using Tools = KinTemplates.Cursor.Tools;

namespace CDPaint2
{
    public partial class Form1 : Form
    {
        // poll the timer every 16 ms
        // that is equivalent to 60 Hz

        // singleton
        private static Form1 m_Instance;

        // debug info
        private const bool m_ShowPhysical = false;
        private const int SIZE = 10;
        private IMouseLogger logger;

        // mouse state
        private enum State
        { 
            Drawing,
            PointSelection,
            LineSelectionStart,
            LineSelectionEnd,
            Reveal
        }
        private State m_State = State.Drawing;
        private bool m_MouseDown;

        // drawing state
        private VirtualBoundedMousePosition m_Pointer;

        // render
        private List<Canvas.Command> m_Command;
        private System.Drawing.Drawing2D.GraphicsPath m_RealPath = new System.Drawing.Drawing2D.GraphicsPath();

        private double m_LastSpeed = 0; // for displaying to the user

        // line def'n
        private PointD m_somePoint;

        public Form1()
        {
            m_Instance = this;
            InitializeComponent();
        }

        public static Form GetInstance()
        {
            return m_Instance;
        }

        private Canvas.Command LastCommand
        {
            get
            {
                return m_Command[m_Command.Count - 1];
            }
        }

        private FilterManager Manager;

        private Canvas.RenderPath Render
        {
            get
            {
                return LastCommand.Render;
            }
        }

        private System.Drawing.Drawing2D.GraphicsPath CurrentPath
        {
            get
            {
                return m_Command[m_Command.Count - 1].Render.Path;
            }
        }

        private Rectangle RectFromPoint(Point p)
        {
            return new Rectangle(new Point(p.X - SIZE /2 , p.Y - SIZE /2), new Size(SIZE,SIZE));
        }

        private Rectangle BoundingRectFromPoint(Point p)
        {
            return new Rectangle(new Point(p.X - SIZE / 2 - 2, p.Y - SIZE / 2 - 2), new Size(SIZE + 5, SIZE + 5));
        }

        private void PaintCursor()
        {
            //Invalidate(new Rectangle(0, 0, 100, 30));
            //Invalidate(BoundingRectFromPoint(m_Pointer.GetLastVirtualPoint()));
            //Invalidate(BoundingRectFromPoint(m_Pointer.GetVirtualPoint()));

            if (listBox1.Items.Count != Manager.Filters.Count)
            {
                listBox1.Items.Clear();
                foreach (Tools.IToolFilter filter in Manager.Filters)
                {
                    listBox1.Items.Add(filter.ToString());
                }
            }

            Invalidate();
        }

        private void SetStateDrawing()
        {
            Cursor = Cursors.Cross;
            m_State = State.Drawing;
            toolStrip1.Enabled = true;
            toolStrip2.Enabled = true;

            // code here to make the buttons appear depressed
            foreach (ToolStripItem item in toolStrip1.Items)
            {
                item.Enabled = false;
                item.Enabled = true;
            }

            toolStrip2.Refresh();
            PaintCursor();
        }

        private void SetStatePoint()
        {
            Cursor = Cursors.Arrow;
            m_State = State.PointSelection;
            toolStrip1.Enabled = false;
            toolStrip2.Enabled = false;
            PaintCursor();
        }

        private void SetStateLinePt1()
        {
            Cursor = Cursors.Arrow;
            m_State = State.LineSelectionStart;
            toolStrip1.Enabled = false;
            toolStrip2.Enabled = false; 
            PaintCursor();
        }

        private void SetStateLinePt2()
        {
            Cursor = Cursors.Arrow;
            m_State = State.LineSelectionEnd;
            toolStrip1.Enabled = false;
            toolStrip2.Enabled = false;
            PaintCursor();
        }

        private void SetStateReveal()
        {
            Cursor = Cursors.Arrow;
            m_State = State.Reveal;
            PaintCursor();
        }

        private void AddFilter(Tools.IToolFilter filter)
        {
            filter.Parameters.Path = new Tools.Model.VectorPath(new Rectangle(new Point(0, 0), new Size(2000,2000)));
            Manager.Filters.Add(filter);
            PaintCursor();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           logger = new MouseNoLogger();
           Manager = new FilterManager();
           m_Command = new List<CDPaint2.Canvas.Command>(100);
           m_Command.Add(new Canvas.Command());
           //AddFilter(new Tools.IceSheetFilter());
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            m_MouseDown = true;

            switch (m_State)
            {
                case State.Drawing:
                    m_Pointer = new VirtualBoundedMousePosition(this,e.Location);

                    logger.MouseDown(m_Pointer);

                    Cursor.Hide();
                    m_Command.Add(new Canvas.Command(new Canvas.RenderPath(Render)));
                    //Manager.StartStroke();
                    Manager.ResetState();
                    CurrentPath.StartFigure();
                    m_RealPath.StartFigure();
                    PaintCursor();
                    break;
            }

        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            // todo: figure out how to capture all the mouse events and time them properly
            // to make smooth curves
            switch (m_State)
            {
                // drawing state is refreshed by the timer
                case State.LineSelectionEnd:
                    PaintCursor();
                    break;
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            switch (m_State)
            {
                case State.Drawing:
                    if (m_MouseDown)
                    {
                        logger.MouseUp(m_Pointer);

                        Cursor = Cursors.Cross; 
                        Cursor.Position = PointToScreen(new Point(m_Pointer.GetVirtualPoint().X, m_Pointer.GetVirtualPoint().Y));
                        Cursor.Show();
                        Invalidate();

                        m_Pointer = null;
                    }
                    break;

                case State.PointSelection:
                    Manager.Filters[Manager.Filters.Count - 1].Parameters.Pt = new PointD(e.X, e.Y);
                //Manager.Filters[Manager.Filters.Count - 1].SetParameters(Tools.ToolParameter.Point(new PointD(e.X, e.Y)));
                    PaintCursor();
                    SetStateDrawing();
                    break;

                case State.LineSelectionStart:
                    m_somePoint = new PointD(e.X, e.Y);
                    SetStateLinePt2();
                    break;

                case State.LineSelectionEnd:
                    Tools.LineToolFilter filter = Manager.Filters[Manager.Filters.Count - 1] as Tools.LineToolFilter;
                    filter.Parameters.Path = new KinTemplates.Cursor.Tools.Model.VectorPath((PointF)m_somePoint, new PointF(e.X, e.Y));
                    filter.Parameters.PathThickness = 1000; // infinitely wide
                    filter.SubsamplePoints();
                    filter.Cache(MyDrawVisitor.DefaultRender());
                    SetStateDrawing();
                    break;
            }

            m_MouseDown = false;

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (m_State == State.Drawing && m_Pointer != null)// State.Drawing
            {
                // remove old cursor
                e.Graphics.FillEllipse(new SolidBrush(BackColor), RectFromPoint(m_Pointer.GetLastVirtualPoint()));
            }

            if (m_ShowPhysical)
                e.Graphics.DrawPath(Pens.Red, m_RealPath);


            foreach(Canvas.Command cmd in m_Command)
            {
                cmd.Render.Render(e.Graphics);
            }

            if (m_State == State.Drawing && m_Pointer != null) // State.Drawing
            {
                if (m_MouseDown)
                {
                    // draw new cursor
                    e.Graphics.FillEllipse(Brushes.White, RectFromPoint(m_Pointer.GetVirtualPoint()));
                    e.Graphics.DrawEllipse(Pens.Black, RectFromPoint(m_Pointer.GetVirtualPoint()));
                }

                if (m_ShowPhysical)
                {
                    //Manager.RenderDebug(e.Graphics);
                }

                //e.Graphics.DrawString("Speed: " + m_Pointer.GetSpeed() + "\nAcceleration: " + m_Pointer.GetAcceleration().Magnitude() , Font, Brushes.Black, new Point(0, 0));
                //e.Graphics.DrawString("Speed: " + m_LastSpeed + "\nAcceleration: " + m_LastAccel, Font, Brushes.Red, new Point(0, 50));
            }

            if (m_State != State.Drawing)
            {

                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(127, Color.Gray)), new Rectangle(new Point(0,0), Size));
                if (m_State == State.LineSelectionEnd)
                {
                    e.Graphics.DrawLine(Pens.Black, (PointF) m_somePoint, PointToClient(Cursor.Position));
                }

                if (m_State == State.Reveal)
                {
                    KinTemplates.Cursor.Tools.Render.RenderParameter rp = KinTemplates.Cursor.Tools.Render.RenderParameter.DetailedMode();
                    Manager.Draw(e.Graphics, rp, KinTemplates.Cursor.Tools.Render.RenderHint.Default,  MyDrawVisitor.DefaultRender(), new PointD(0, 0), false);
                }
            }

        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            if (m_State == State.Drawing && m_MouseDown)
            {
                if (m_Pointer.SetPhysicalPoint(PointToClient( Cursor.Position )))
                {
                    //int iParts = (int) (m_Pointer.GetDistance() / 10.0) + 1; // space interpolation every 10 px
                    //if (iParts > 10) iParts = 10; // and upper bound the sampling points
                    int iParts = 1;
                    double timeSlice = m_Pointer.GetTimeInterval() / iParts;

                    if (timeSlice == 0)
                    {
                        timeSlice = m_Pointer.GetTimeInterval();
                        iParts = 1;  // subsampling disabled
                    }

                    logger.MouseMoved(m_Pointer);
                    ParameterizedMousePosition fakePosition = new ParameterizedMousePosition(m_Pointer);

                    // subsampling
                    for (int i = 0; i < iParts; i++)
                    {
                        bool changed;
                        Manager.Compute(fakePosition, out changed);

                        // apply filters to acceleration and velocity
                        double xShift = timeSlice * (Manager.GetVelocity().X);
                        double yShift = timeSlice * (Manager.GetVelocity().Y);

                        m_LastSpeed = Manager.GetVelocity().Magnitude();

                        PointD newPoint = new PointD(fakePosition.GetVirtualPointD().X + xShift, fakePosition.GetVirtualPointD().Y + yShift);

                        if (fakePosition.GetVirtualPoint() != (Point)newPoint)
                        {
                            CurrentPath.AddLine(fakePosition.GetVirtualPoint(), (Point)newPoint);
                        }

                        fakePosition.SetVirtualPointD(newPoint);
                        fakePosition.SetDisplacement(fakePosition.GetDisplacement() + new PointD(xShift, yShift));
                        //fakePosition = new ParameterizedMousePosition(fakePosition, fakePosition.GetVelocity(), fakePosition.GetAcceleration());
                    }

                    m_Pointer.SetVirtualPoint(fakePosition.GetVirtualPointD());
                    if (m_Pointer.GetLastVirtualPoint() != m_Pointer.GetVirtualPoint())
                        m_RealPath.AddLine(m_Pointer.GetLastPhysicalPoint(), m_Pointer.GetPhysicalPoint());
                    PaintCursor();
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            logger.Stop();
        }

 
        private void btnSlick_Click(object sender, EventArgs e)
        {
            AddFilter(new Tools.IceSheetFilter());
        }

        private void btnSandpaper_Click(object sender, EventArgs e)
        {
            AddFilter(new Tools.SandpaperFilter());

        }

        private void btnThatch_Click(object sender, EventArgs e)
        {
            AddFilter(new Tools.GridFilter());
        }

        

        private void btnInertia_Click(object sender, EventArgs e)
        {
            AddFilter(new Tools.InertiaFilter());
        }

        private void btnProtractor_Click(object sender, EventArgs e)
        {
            AddFilter(new Tools.CompassFilter());
            SetStatePoint();
        }

        private void btnDimpleChad_Click(object sender, EventArgs e)
        {
            AddFilter(new Tools.DimpleChadFilter());
            SetStatePoint();
        }

        private void btnGravityMass_Click(object sender, EventArgs e)
        {
            AddFilter(new Tools.OrbitFilter());
            SetStatePoint();

        }

        private void btnPointAttract_Click(object sender, EventArgs e)
        {
            AddFilter(new Tools.MagneticPointAttractionFilter(true)); 
            SetStatePoint();
        }

        private void btnPointRepulsion_Click(object sender, EventArgs e)
        {
            AddFilter(new Tools.MagneticPointAttractionFilter(false));
            SetStatePoint();
        }

        private void btnRubberBand_Click(object sender, EventArgs e)
        {
            AddFilter(new Tools.RubberBandFilter());
            SetStatePoint();

        }

        private void btnLineAttraction_Click(object sender, EventArgs e)
        {
            AddFilter(new Tools.MagneticLineAttractionFilter());
            SetStateLinePt1();
        }

        private void btnLineRepulsion_Click(object sender, EventArgs e)
        {
            AddFilter(new Tools.MagneticLineRepulsionFilter());
            SetStateLinePt1();
        }


        private void btnBeltTop_Click(object sender, EventArgs e)
        {
            Tools.IToolFilter filter = new Tools.ConveyorBeltFilter();
            filter.Parameters.V = new PointD(0, -1);
            AddFilter(filter);
        }

        private void btnBeltLeft_Click(object sender, EventArgs e)
        {
            Tools.IToolFilter filter = new Tools.ConveyorBeltFilter();
            filter.Parameters.V = new PointD(-1, 0);
            AddFilter(filter);

        }

        private void btnBeltRight_Click(object sender, EventArgs e)
        {
            Tools.IToolFilter filter = new Tools.ConveyorBeltFilter();
            filter.Parameters.V = new PointD(1, 0);
            AddFilter(filter);

        }

        private void btnBeltBottom_Click(object sender, EventArgs e)
        {
            Tools.IToolFilter filter = new Tools.ConveyorBeltFilter();
            filter.Parameters.V = new PointD(0,1);
            AddFilter(filter);
        }

        private void btnFurTop_Click(object sender, EventArgs e)
        {
            Tools.IToolFilter filter = new Tools.FurFilter();
            filter.Parameters.V = new PointD(0, -1);
            AddFilter(filter);
        }

        private void btnFurLeft_Click(object sender, EventArgs e)
        {
            Tools.IToolFilter filter = new Tools.FurFilter();
            filter.Parameters.V = new PointD(-1,0);
            AddFilter(filter);

        }

        private void btnFurRight_Click(object sender, EventArgs e)
        {
            Tools.IToolFilter filter = new Tools.FurFilter();
            filter.Parameters.V = new PointD(1,0);
            AddFilter(filter);
        }

        private void btnFurBottom_Click(object sender, EventArgs e)
        {
            Tools.IToolFilter filter = new Tools.FurFilter();
            filter.Parameters.V = new PointD(0, 1);
            AddFilter(filter);
        }

        private void btnPaintMin_Click(object sender, EventArgs e)
        {
            AddFilter(new Tools.MinFilter());
        }

        private void btnPaintMax_Click(object sender, EventArgs e)
        {
            AddFilter(new Tools.MaxFilter());
        }

        private void btnThin_Click(object sender, EventArgs e)
        {
            m_Command.Add(new Canvas.Command( new Canvas.RenderPath(Render, 1)));
        }

        private void btnWidthSmall_Click(object sender, EventArgs e)
        {
            m_Command.Add(new Canvas.Command(new Canvas.RenderPath(Render, 2)));
        }

        private void btnWidthMedium_Click(object sender, EventArgs e)
        {
            m_Command.Add(new Canvas.Command(new Canvas.RenderPath(Render, 4)));
        }

        private void btnWidthLarge_Click(object sender, EventArgs e)
        {
            m_Command.Add(new Canvas.Command(new Canvas.RenderPath(Render, 6)));
        }

        private void btnColour1_Click(object sender, EventArgs e)
        {
            //Render.Add(new Canvas.RenderPath(Render[Render.Count - 1], ((ToolStripButton) sender).BackColor));
            m_Command.Add(new Canvas.Command(new Canvas.RenderPath(Render,((ToolStripButton) sender).BackColor )));

        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            foreach (Canvas.Command cmd in m_Command)
            {
                cmd.Render.Render(e.Graphics);
            }
        }

     
        private void btnCorduroyLeft_Click(object sender, EventArgs e)
        {
            Tools.IToolFilter filter = new Tools.CorduroyFilter();
            filter.Parameters.V = new PointD(1,0);
            AddFilter(filter);
        }

        private void btnCorduroyTop_Click(object sender, EventArgs e)
        {
            Tools.IToolFilter filter = new Tools.CorduroyFilter();
            filter.Parameters.V = new PointD(0, 1);
            AddFilter(filter);

        }

 

        private void btnReveal_MouseDown(object sender, MouseEventArgs e)
        {
            SetStateReveal();
        }

        private void btnReveal_MouseUp(object sender, MouseEventArgs e)
        {
            SetStateDrawing();
        }

   
        private void btnMakeSandpaper_Click(object sender, EventArgs e)
        {
            Canvas.Command cmd =  m_Command[m_Command.Count - 1];
            m_Command.RemoveAt(m_Command.Count - 1);
            Tools.IToolFilter filter =new Tools.SandpaperFilter();
            Manager.Filters.Add(filter);
            cmd.Render.Path.CloseFigure();
            filter.Parameters.Path = new KinTemplates.Cursor.Tools.Model.VectorPath(cmd.Render.Path);
            PaintCursor();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            printDocument1.DefaultPageSettings.Landscape = Size.Width > Size.Height;
            if (printDialog1.ShowDialog() == DialogResult.OK)
            {
                printDocument1.Print();
            }
            
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Bitmap bitmap = new Bitmap(Size.Width, Size.Height);
                Graphics context = Graphics.FromImage(bitmap);
                foreach (Canvas.Command cmd in m_Command)
                {
                    cmd.Render.Render(context);
                }
                bitmap.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Png);
                context.Dispose();
                bitmap.Dispose();
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Manager = new FilterManager();
            Canvas.Command lastCommand = LastCommand;
            m_Command = new List<CDPaint2.Canvas.Command>(100);
            m_Command.Add(new CDPaint2.Canvas.Command(new Canvas.RenderPath(lastCommand.Render)));
            m_RealPath = new System.Drawing.Drawing2D.GraphicsPath();
            PaintCursor();
        }

        private void undoAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Canvas.Command last = LastCommand;
            m_Command = new List<CDPaint2.Canvas.Command>(100);
            m_Command.Add(new CDPaint2.Canvas.Command(new Canvas.RenderPath(last.Render)));
            m_RealPath = new System.Drawing.Drawing2D.GraphicsPath();
            PaintCursor();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_Command.RemoveAt(m_Command.Count - 1);
            if (m_Command.Count == 0)
            {
                m_Command.Add(new CDPaint2.Canvas.Command());
            }
            PaintCursor();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // m_Command.Add(new Canvas.Command(new FilterManager()));
            Manager.ClearFilters();
            PaintCursor();
        }




    }
}