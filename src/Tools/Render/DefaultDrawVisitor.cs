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
    /// <summary>
    /// The default draw visitor does nothing.  Spacing is still computed.
    /// </summary>
    public class DefaultDrawVisitor : IDrawVisitor
    {
        public void DrawArrow(System.Drawing.Graphics gc, RenderParameter r, PointD thePoint, PointD theVector)
        {
            // do nothing here
        }

        public void DrawScales(System.Drawing.Graphics gc, RenderParameter r, PointD thePoint, PointD theVector)
        {
            // do nothing here
        }

        public void DrawNegativeSpace(Graphics gc, GraphicsPath path, RenderParameter r)
        {
            // do nothing here
        }

        public void DrawPositiveSpace(Graphics gc, GraphicsPath path, RenderParameter r)
        {
            // do nothing here
        }

        public double Spacing(double C)
        {
            return (1.5 - C) * Render.DrawHelper.SPEED_AMPLIFIER;
        }


        public void DrawHandles(Graphics gc, IToolFilter filter, RenderParameter r)
        {
            // do nothing here
        }

    }
}
