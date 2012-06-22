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

namespace KinTemplates.Cursor.Tools.Render
{
    public class StaticFeedback
    {
        const int FORCE_GRID = 40;
        PointD[][] m_forces;
        
        public void AddFilter(KinTemplates.Cursor.Tools.IToolFilter filter, FeedbackParameter r)
        {
            // forces
            if (filter is Tools.ConveyorBeltFilter
                || filter is Tools.ConveyorBeltFilter2 
                || filter is Tools.OrbitFilter 
                || filter is Tools.OrbitFilter2
                || filter is Tools.OrbitFilter3
                || filter is Tools.MagneticLineAttractionFilter
                || filter is Tools.MagneticPointAttractionFilter
                || filter is Tools.RubberBandFilter
                // || filter is Tools.TunnelLineFilter 
                )
            {
                if (m_forces == null)
                {
                    Rectangle screenBounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
                    m_forces = new PointD[screenBounds.Width / FORCE_GRID][];
                    for (int i = 0; i < m_forces.Length; i++)
                    {
                        m_forces[i] = new PointD[screenBounds.Height / FORCE_GRID];
                    }

                    //m_renderedForces = new Bitmap(screenBounds.Width, screenBounds.Height);
                }

                for (int i = 0; i < m_forces.Length; i++)
                {
                    for (int j = 0; j < m_forces[i].Length; j++)
                    {
                        Point thePoint = new Point(i * FORCE_GRID, j * FORCE_GRID);
                        if (filter.HitTest((PointD)thePoint))
                        {
                            // test all points with the filter
                            Position.VirtualMousePosition mouse = new Position.VirtualMousePosition(thePoint);
                            Position.ParameterizedMousePosition fakeMouse = new Position.ParameterizedMousePosition(mouse, 20);// TODO: instrument from the computer
                            m_forces[i][j] += filter.GetVelocity(fakeMouse) * Render.DrawHelper.SPEED_AMPLIFIER;
                        }
                    }
                }
            }

            /*
            // we have rendered something, let's cache this picture
            if (m_forces != null)
            {
                Graphics fromBitmap = Graphics.FromImage(m_renderedForces);
                InternalRenderForces(fromBitmap, r);
                fromBitmap.Dispose();
            }
            */
        }

        public void RenderForces(System.Drawing.Graphics gc, FeedbackParameter r, Render.IDrawVisitor drawMethods)
        {
            InternalRenderForces(gc, r, drawMethods);
        }

        private void InternalRenderForces(System.Drawing.Graphics gc, FeedbackParameter r,  Render.IDrawVisitor drawMethods)
        {
            if (m_forces != null)
            {
                for (int i = 0; i < m_forces.Length; i++)
                {
                    for (int j = 0; j < m_forces[i].Length; j++)
                    {
                        drawMethods.DrawArrow(gc, r, new PointD(i * FORCE_GRID, j * FORCE_GRID), m_forces[i][j]);
                    }
                }
            }
        }

    }
}
