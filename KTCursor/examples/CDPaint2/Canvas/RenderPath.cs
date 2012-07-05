// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CDPaint2.Canvas
{
    /// <summary>
    /// Represents a path and a pen colour.
    /// </summary>
    class RenderPath
    {
        private GraphicsPath m_path;
        private Pen m_pen;

        public RenderPath(RenderPath cloneRender)
        {
            m_path = new GraphicsPath();
            m_pen = cloneRender.m_pen;
        }

        public RenderPath(RenderPath cloneRender, int newWidth)
        {
            m_path = new GraphicsPath();
            m_pen = new Pen(cloneRender.m_pen.Color, newWidth);
        }

        public RenderPath(RenderPath cloneRender, Color newColour)
        {
            m_path = new GraphicsPath();
            m_pen = new Pen(newColour, cloneRender.m_pen.Width);
        }

        public RenderPath()
        {
            m_path = new GraphicsPath();
            m_pen = new Pen(Color.Black);
            m_pen.EndCap = LineCap.NoAnchor;
            m_pen.StartCap = LineCap.NoAnchor;
            m_pen.LineJoin = LineJoin.MiterClipped;
        }

        public void Render(Graphics g)
        {
            g.DrawPath(m_pen, m_path);
        }

        public GraphicsPath Path
        {
            get
            {
                return m_path;
            }
        }
    }
}
