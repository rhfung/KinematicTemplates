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

namespace KinTemplates.Cursor.Tools
{
    public abstract class RegionToolPathFilter : RegionToolFilter
    {

        public override Tools.Model.IPath GetNewPath()
        {
            return new Tools.Model.VectorPath();
        }

        public override void DrawOutline(System.Drawing.Graphics gc, Render.RenderParameter r)
        {
            if (m_Param.Path != null)
                gc.DrawPath(r.RegionOutline, (Tools.Model.VectorPath)m_Param.Path);
        }

        public override void Draw(System.Drawing.Graphics gc, Render.RenderParameter r, Render.RenderHint editState, Render.IDrawVisitor drawMethods, PointD mousePosition)
        {
            if (m_Param.Path != null)
            {
                /*if (editState.GetAttributes() == States.StateAttributes.Start)
                {
                    if (r.StrokeFill != null)
                        gc.FillPath(r.StrokeFill, (Tools.Model.VectorPath) m_Param.Path);
                    gc.DrawPath(r.StrokeOutline, (Tools.Model.VectorPath)m_Param.Path);
                }
                else*/ if (editState == Render.RenderHint.Start)
                {
                    Pen dashPen = (Pen)r.StrokeOutline.Clone();
                    dashPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                    if (m_Param.Path.PointCount > 0)
                    {
                        PointD firstPoint = (PointD)m_Param.Path.GetFirstPoint();
                        gc.DrawEllipse(dashPen, (float)firstPoint.X - Tools.Render.DrawHelper.TARGET_SIZE / 2.0f, (float)firstPoint.Y - Tools.Render.DrawHelper.TARGET_SIZE / 2.0f, (float)Tools.Render.DrawHelper.TARGET_SIZE, (float)Tools.Render.DrawHelper.TARGET_SIZE);
                        
                        if (r.StrokeFill != null)
                            gc.FillPath(r.StrokeFill, (Tools.Model.VectorPath)m_Param.Path);
                        gc.DrawPath(r.StrokeOutline, (Tools.Model.VectorPath)m_Param.Path);
                         
                        DrawRegionRepresentation(gc, r, drawMethods, mousePosition);
                    }
                }
                /*else if (editState.GetAttributes() == States.StateAttributes.Change)
                {
                    drawMethods.DrawNegativeSpace(gc, m_Param, r);
                    drawMethods.DrawPositiveSpace(gc, m_Param, r);
                    if (editState is States.RegionChange)
                    {
                        DrawRegionRepresentation(gc, r, mousePosition);

                        ((States.RegionChange)editState).Handles.DrawHandles(gc, m_Param, r);
                    }
                }*/
                else if (editState == Render.RenderHint.Feedback)
                {
                    drawMethods.DrawNegativeSpace(gc, m_Param.Path.InternalPath, r);
                    drawMethods.DrawPositiveSpace(gc, m_Param.Path.InternalPath, r);
                    if (!(this is ConveyorBeltFilter))
                        DrawRegionRepresentation(gc, r,drawMethods, mousePosition);

                }
                else
                {
                    drawMethods.DrawNegativeSpace(gc, m_Param.Path.InternalPath, r);
                    drawMethods.DrawPositiveSpace(gc, m_Param.Path.InternalPath, r);
                    DrawRegionRepresentation(gc, r, drawMethods, mousePosition);

                        // TODO ((States.IFilterHandles)editState).Handles.DrawHandles(gc, m_Param, r);

                }
                drawMethods.DrawHandles(gc, this, r);
            }
        }

        public override bool HitTest(PointD point)
        {
            if (m_Param.Path == null)
            {
                return false;
            }
            else
            {
                return m_Param.Path.IsVisible((Point)point);
            }
        }
    }
}
