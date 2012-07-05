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
    public abstract class RegionToolRegionFilter : RegionToolFilter
    {
        protected abstract Brush GetBrush(Render.RenderParameter r);

        public override Tools.Model.IPath GetNewPath()
        {
            return new Tools.Model.BitmapPath(GetBrush(Render.RenderParameter.DrawMode()), m_Param.PathThickness);
        }

        public override void DrawOutline(System.Drawing.Graphics gc, Render.RenderParameter r)
        {
            if (m_Param.Path != null)
            {
                if (m_Param.Path is Tools.Model.VectorPath)
                {
                    gc.DrawPath(r.RegionOutline, (Tools.Model.VectorPath)m_Param.Path);
                }
                else if (m_Param.Path is Tools.Model.BitmapPath)
                {
                    using (Pen dashPen = (Pen) r.RegionOutline.Clone())
                    {
                        dashPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                        gc.DrawPath(dashPen, ((Tools.Model.BitmapPath)m_Param.Path).LineTrace);
                    }
                }
            }
        }

        public override void Draw(System.Drawing.Graphics gc, Render.RenderParameter r, Render.RenderHint editState, Render.IDrawVisitor drawMethods, PointD mousePosition)
        {
            if (m_Param.Path != null)
            {
                /*if (editState.GetAttributes() == States.StateAttributes.Start)
                {
                    if (m_Param.Path is Tools.Model.VectorPath)
                    {
                        if (r.StrokeFill != null)
                            gc.FillPath(r.StrokeFill, (Tools.Model.VectorPath)m_Param.Path);
                        gc.DrawPath(r.StrokeOutline, (Tools.Model.VectorPath)m_Param.Path);
                    }
                    
                        
                }
                else*/ if (editState == Render.RenderHint.Start)
                {
                    if (m_Param.Path is Tools.Model.VectorPath)
                    {
                        Pen dashPen = (Pen)r.StrokeOutline.Clone();
                        dashPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                        if (m_Param.Path.PointCount > 0)
                        {
                            PointD firstPoint = (PointD)m_Param.Path.GetFirstPoint();
                            gc.DrawEllipse(dashPen, (float)firstPoint.X - Tools.Render.DrawHelper.TARGET_SIZE / 2.0f, (float)firstPoint.Y - Tools.Render.DrawHelper.TARGET_SIZE / 2.0f, (float)Tools.Render.DrawHelper.TARGET_SIZE, (float)Tools.Render.DrawHelper.TARGET_SIZE);
                           
                            if (r.StrokeFill != null)
                                gc.FillPath(r.StrokeFill, ( Tools.Model.VectorPath)m_Param.Path);
                            gc.DrawPath(r.StrokeOutline, ( Tools.Model.VectorPath)m_Param.Path);
                            
                            DrawRegionRepresentation(gc, r, drawMethods, mousePosition);
                        }
                    }
                }
                /*else if (editState.GetAttributes() == States.StateAttributes.Change)
                {
                    /*
                    drawMethods.DrawNegativeSpace(gc, m_Param, r);
                    drawMethods.DrawPositiveSpace(gc, m_Param, r);
                     *-/
                    DrawRegionRepresentation(gc, r, mousePosition);
                    if (editState is States.RegionChange)
                        ((States.RegionChange)editState).Handles.DrawHandles(gc, m_Param, r);
                     
                }*/
                else
                {
                    /*
                    drawMethods.DrawNegativeSpace(gc, m_Param, r);
                    drawMethods.DrawPositiveSpace(gc, m_Param, r);
                     */

                    if (m_Param.Path is Tools.Model.VectorPath)
                    {
                        if (r.StrokeFill != null)
                            gc.FillPath(r.StrokeFill, (Tools.Model.VectorPath)m_Param.Path);
                        gc.DrawPath(r.StrokeOutline, (Tools.Model.VectorPath)m_Param.Path);
                    }

                    DrawRegionRepresentation(gc, r,drawMethods, mousePosition);

                        //TODO ((States.IFilterHandles)editState).Handles.DrawHandles(gc, m_Param, r);

                    
                }

                drawMethods.DrawHandles(gc, this, r);

            }

                
        }

        public override void DrawRegionRepresentation(System.Drawing.Graphics gc, Render.RenderParameter r, Render.IDrawVisitor drawMethods, PointD mousePosition)
        {
            if (m_Param.Path is Tools.Model.BitmapPath)
            {
                gc.DrawImage((Tools.Model.BitmapPath)m_Param.Path, new Point(0, 0));
            }
            else if (m_Param.Path is Tools.Model.VectorPath)
            {
                gc.FillPath(GetBrush(r), (Tools.Model.VectorPath)m_Param.Path);
            }

            if (HitTest(mousePosition))
            {
                gc.DrawString(ToString(), r.FontType, new SolidBrush(r.RegionGuides.Color), new PointF((float)mousePosition.X, (float)mousePosition.Y - 15));
            }
        }

        // returns value between 0 and 1
        public double GetStrength(PointD point)
        {
            if (m_Param.Path is Model.VectorPath)
            {
                return HitTest(point) ? 1.0 : 0.0; 
            }
            else if (m_Param.Path is Model.BitmapPath)
            {
                Model.BitmapPath bp = (Model.BitmapPath)m_Param.Path;
                return (255 - bp.GetPixelValue((PointF) point)) / 255.0;
            }
            else
            {
                throw new Exception("Not handled");
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
