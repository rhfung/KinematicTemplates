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
    public interface IPath : IDisposable, ICloneable
    {
        void StartFigure();
        void AddLine(PointF point1, PointF point2);
        void AddPath(IPath path, bool connect);
        void AddLines(PointF[] points);
        void AddEllipse(RectangleF rect);
        void AddRectangle(RectangleF rect);
        void CloseFigure();
        PointF GetFirstPoint();
        PointF GetLastPoint();
        RectangleF GetBounds();
        void Translate(float shiftX, float shiftY);
        void Transform(Matrix m);
        bool IsVisible(PointF point);
        bool IsOutlineVisible(PointF point, Pen test);

        int PointCount
        {
            get;
        }

        GraphicsPath InternalPath
        {
            get;
        }
    }
}
