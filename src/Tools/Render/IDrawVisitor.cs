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
namespace KinTemplates.Cursor.Tools.Render
{
    /// <summary>
    /// Interface for helping to draw the visible (static) feedback provided by templates on the drawing canvas.
    /// DefaultDrawVisitor does nothing.
    /// </summary>
    public interface IDrawVisitor
    {
        void DrawArrow(System.Drawing.Graphics gc, RenderParameter r, KinTemplates.PointD thePoint, KinTemplates.PointD theVector);
        void DrawScales(System.Drawing.Graphics gc, RenderParameter r, KinTemplates.PointD thePoint, KinTemplates.PointD theVector);

        void DrawNegativeSpace(System.Drawing.Graphics gc, System.Drawing.Drawing2D.GraphicsPath path, RenderParameter r);
        void DrawPositiveSpace(System.Drawing.Graphics gc, System.Drawing.Drawing2D.GraphicsPath path, RenderParameter r);

        void DrawHandles(System.Drawing.Graphics gc, IToolFilter filter, RenderParameter r);

        double Spacing(double C);
    }
}
