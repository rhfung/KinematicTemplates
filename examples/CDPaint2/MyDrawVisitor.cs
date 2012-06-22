// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

using KinTemplates;
using KinTemplates.Cursor.Tools;
using KinTemplates.Cursor.Tools.Render;


namespace CDPaint2
{
    class MyDrawVisitor : IDrawVisitor
    {
        private static IDrawVisitor render = new MyDrawVisitor();


        public static IDrawVisitor DefaultRender()
        {
            return render;
        }

        public void DrawArrow(System.Drawing.Graphics gc, RenderParameter r, PointD thePoint, PointD theVector)
        {
        }

        public void DrawScales(System.Drawing.Graphics gc, RenderParameter r, PointD thePoint, PointD theVector)
        {
        }

        public void DrawNegativeSpace(Graphics gc, GraphicsPath path, RenderParameter r)
        {

        }

        public void DrawPositiveSpace(Graphics gc, GraphicsPath path, RenderParameter r)
        {
        }

        public double Spacing(double C)
        {
            return (1.5 - C) * KinTemplates.Cursor.Tools.Render.DrawHelper.SPEED_AMPLIFIER;
        }


        public void DrawHandles(Graphics gc, IToolFilter filter, RenderParameter r)
        {

        }
    }
}
