// Kinematic Templates Cursor Manipulation Library
// Copyright (C) 2009, Richard H. Fung
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace KinTemplates.Cursor.Tools.Model
{
    public class BitmapPath : IPath
    {
        protected Bitmap m_mask;
        protected Bitmap m_visual;
        protected Graphics m_editMask;
        protected Graphics m_editVisual;
        
        protected Brush m_visualFill;
        protected GraphicsPath m_lineTrace;

        protected bool m_blnStarted;
        protected PointF m_startPt;
        protected PointF m_endPt;

        protected double m_pathThickness;

        protected int m_left;
        protected int m_top;
        protected int m_width;
        protected int m_height;

        protected int m_count;

        // private constructor used for cloning object
        private BitmapPath()
        {
        }

        public BitmapPath(Brush visualFill, double pathThickness)
        {
            Rectangle bounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            
            m_mask = new Bitmap(bounds.Width, bounds.Height);
            m_editMask = Graphics.FromImage(m_mask);
            m_editMask.FillRectangle(Brushes.White, bounds);

            m_visual = new Bitmap(bounds.Width, bounds.Height);
            m_editVisual = Graphics.FromImage(m_visual);
            
            m_visualFill = visualFill;
            m_lineTrace = new GraphicsPath();

            m_blnStarted = false;
            m_startPt = PointF.Empty;
            m_endPt = PointF.Empty;

            m_pathThickness = pathThickness;

            m_left = int.MaxValue;
            m_top = int.MaxValue;
            m_width = 0;
            m_height = 0;
        }

        public Image VisualImage
        {
            get{
            return m_visual;
            }
        }    

        public Image MaskImage
        {
            get
            {
                return m_mask;
            }
        }

        public GraphicsPath LineTrace
        {
            get
            {
                return m_lineTrace;
            }
        }

        public static implicit operator Image(BitmapPath path)
        {
            return path.VisualImage;
        }

        #region IPath Members

        public void StartFigure()
        {
            m_blnStarted = false;
            m_startPt = PointF.Empty;
            m_endPt = PointF.Empty;
            // line trace is visible only
            m_lineTrace.StartFigure();
        }

        public void AddLine(PointF point1, PointF point2)
        {
            if (!m_blnStarted)
            {
                m_startPt = point1;
                m_blnStarted = true;
            }
            m_endPt = point2;

            // subsampling points
            PointF[] fill = LineToolFilter.FillPoints(new PointF[] { point1, point2 }, 2f);

            foreach (PointF pt in fill)
            {
                AddEllipse(new RectangleF(new PointF((float)(pt.X - m_pathThickness), (float)(pt.Y - m_pathThickness)),
                    new SizeF((float)(m_pathThickness * 2), (float)(m_pathThickness * 2))));
            }

            m_lineTrace.AddLine(point1, point2);
        }

        public void AddPath(IPath path, bool connect)
        {
            if (connect)
            {
                throw new Exception("The method or operation is not implemented.");
            }
            else
            {
                if (path is VectorPath)
                {
                    m_editMask.FillPath(Brushes.Black, (VectorPath)path);
                    m_editVisual.FillPath(m_visualFill, (VectorPath)path);

                    m_lineTrace.StartFigure();
                    m_lineTrace.AddPath((VectorPath)path, false);

                    // keep track of bounds
                    RectangleF rect = path.GetBounds();
                    if (rect.Left < m_left)
                        m_left = (int)rect.Left;
                    if (rect.Right > m_left + m_width)
                        m_width = (int)rect.Right - m_left;
                    if (rect.Top < m_top)
                        m_top = (int)rect.Top;
                    if (rect.Bottom > m_top + m_height)
                        m_height = (int)rect.Bottom - m_top;

                    m_count += path.PointCount;
                }
                else if (path is BitmapPath)
                {
                    if (m_count == 0)
                    {
                        BitmapPath o = (BitmapPath)path;
                        m_editMask.DrawImage(o.m_mask, new Point(0, 0));
                        m_editVisual.DrawImage(o.m_visual, new Point(0, 0));
                        m_count = o.m_count;
                        m_left = o.m_left;
                        m_top = o.m_top;
                        m_width = o.m_width;
                        m_height = o.m_height;

                        if (o.m_lineTrace.PointCount > 0)
                        {
                            m_lineTrace.StartFigure();
                            m_lineTrace.AddPath(o.m_lineTrace, false);
                        }
                        // m_lineTrace.AddRectangle(new Rectangle(m_left, m_top, m_width, m_height));

                    }
                    else
                    {
                        throw new Exception("not implemented");
                    }
                }
            }
            
        }

        public void AddLines(PointF[] points)
        {
            PointF firstPoint = points[0];
            for (int i = 1; i < points.Length; i++)
            {
                AddLine(firstPoint, points[i]);
                points[i] = firstPoint;
            }
        }

        public void AddEllipse(RectangleF rect)
        {
            m_editMask.FillEllipse( Brushes.Black , rect);
            m_editVisual.FillEllipse(m_visualFill, rect);

            // keep track of bounds
            if (rect.Left < m_left)
                m_left = (int)rect.Left;
            if (rect.Right > m_left + m_width)
                m_width = (int)rect.Right - m_left;
            if (rect.Top < m_top)
                m_top = (int)rect.Top;
            if (rect.Bottom > m_top + m_height)
                m_height = (int)rect.Bottom - m_top;

            m_count++;
       }

        public void AddRectangle(RectangleF rect)
        {
            StartFigure();
            AddLine(new PointF(rect.Left, rect.Top), new PointF(rect.Left + rect.Width, rect.Top));
            AddLine(new PointF(rect.Left + rect.Width, rect.Top), new PointF(rect.Left + rect.Width, rect.Top + rect.Height) );
            AddLine(new PointF(rect.Left + rect.Width, rect.Top + rect.Height), new PointF(rect.Left , rect.Top + rect.Height));
            m_lineTrace.StartFigure();
            m_lineTrace.AddRectangle(rect);
            CloseFigure();
        }

        public void DrawMask(Image image, Point point)
        {
            m_editMask.DrawImage(image, point.X,point.Y, image.Width, image.Height );
            m_editVisual.DrawImage(image, point.X, point.Y, image.Width, image.Height);
            if (point.X < m_left)
                m_left = point.X;
            if (point.Y < m_top)
                m_top = point.Y;
            if (point.X + image.Width > m_left + m_width)
                m_width = image.Width + m_left;
            if (point.Y + image.Height > m_top + m_height)
                m_height = image.Height + m_height;
            m_count++;
        }

        public void CloseFigure()
        {
            AddLine(m_startPt, m_endPt);
        }

        public PointF GetFirstPoint()
        {
            return m_startPt;
        }

        public PointF GetLastPoint()
        {
            return m_endPt;
        }

        public RectangleF GetBounds()
        {
            return new RectangleF(m_left, m_top, m_width, m_height);
        }

        public void Translate(float shiftX, float shiftY)
        {
            Rectangle bounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            Bitmap newMask = new Bitmap(bounds.Width, bounds.Height);
            Bitmap newVisual = new Bitmap(bounds.Width, bounds.Height);

            m_editMask.Dispose();
            m_editMask = Graphics.FromImage(newMask);
            m_editMask.FillRectangle(Brushes.White, bounds);
            m_editMask.DrawImage(m_mask, new PointF(shiftX, shiftY));

            m_editVisual.Dispose();
            m_editVisual = Graphics.FromImage(newVisual);
            m_editVisual.DrawImage(m_visual, new PointF(shiftX, shiftY));

            m_mask.Dispose();
            m_mask = newMask;

            m_visual.Dispose();
            m_visual = newVisual;

            m_left += (int)shiftX;
            m_top += (int)shiftY;

            Matrix t = new Matrix();
            t.Translate(shiftX, shiftY);
            m_lineTrace.Transform(t);
        }

        public void Transform(Matrix m)
        {
            // not tested!!
            Rectangle bounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            Bitmap newMask = new Bitmap(bounds.Width, bounds.Height);
            Bitmap newVisual = new Bitmap(bounds.Width, bounds.Height);

            m_editMask.Dispose();
            m_editMask = Graphics.FromImage(newMask);
            m_editMask.FillRectangle(Brushes.White, bounds);
            m_editMask.Transform = m;
            m_editMask.DrawImage(m_mask, PointF.Empty);

            m_editVisual.Dispose();
            m_editVisual = Graphics.FromImage(newVisual);
            m_editVisual.Transform = m;
            m_editVisual.DrawImage(m_visual, PointF.Empty);

            m_mask.Dispose();
            m_mask = newMask;

            m_visual.Dispose();
            m_visual = newVisual;

            m_lineTrace.Transform(m);
        }

        public bool IsVisible(PointF point)
        {
            return IsVisible(point, 255);
        }

        // this threshold at 255 means non-white
        public bool IsVisible(PointF point, byte threshold)
        {
            Size imgSize = m_mask.Size;
            if (point.X < 0 || point.Y < 0 ||
                point.X > imgSize.Width || point.Y > imgSize.Height)
            {
                return false;
            }
            else
            {
                return GetPixelValue(point) < threshold;
            }
        }

        public byte GetPixelValue(PointF point)
        {
            return m_mask.GetPixel((int)point.X, (int)point.Y).G;
        }

        public bool IsOutlineVisible(PointF point, Pen test)
        {
            return IsVisible(point);
        }

        public int PointCount
        {
            get { return m_count; }
        }

        #endregion

        public double PathThickness
        {
            get
            {
                return m_pathThickness;
            }
            set
            {
                m_pathThickness = value;
            }
        }

        public GraphicsPath InternalPath
        {
            get
            {
                System.Diagnostics.Debug.Assert(false, "not implemented");
                return m_lineTrace;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            BitmapPath bpath = new BitmapPath();
            bpath.m_mask = new Bitmap(m_mask);
            bpath.m_visual = new Bitmap(m_visual);
            bpath.m_editMask = Graphics.FromImage(bpath.m_mask);
            bpath.m_editVisual = Graphics.FromImage(bpath.m_visual);
            bpath.m_visualFill = m_visualFill; // not cloned!!
            bpath.m_lineTrace = new GraphicsPath();
            if (m_lineTrace.PointCount > 0)
                bpath.m_lineTrace.AddPath(m_lineTrace, false);
            bpath.m_startPt = m_startPt;
            bpath.m_endPt = m_endPt;
            bpath.m_pathThickness = m_pathThickness;
            bpath.m_left = m_left;
            bpath.m_top = m_top;
            bpath.m_width = m_width;
            bpath.m_height = m_height;
            bpath.m_count = m_count;

            return bpath;
        }

        #endregion
    }
}
