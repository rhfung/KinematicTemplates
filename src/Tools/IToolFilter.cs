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

namespace KinTemplates.Cursor.Tools
{
    public interface IToolFilter
    {
        /// <summary>
        /// gets the velocity to be added to the template
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        PointD GetVelocity(Position.IVirtualMousePosition m);

        /// <summary>
        /// draws an outline of the template
        /// </summary>
        /// <param name="gc"></param>
        /// <param name="r"></param>
        void DrawOutline(System.Drawing.Graphics gc, Render.RenderParameter r);

        /// <summary>
        /// draws the template with details
        /// </summary>
        /// <param name="gc">Graphics context to draw to</param>
        /// <param name="r">Some hints about how to show the template details</param>
        /// <param name="editState">Template can change appearance based on its current editing state</param>
        /// <param name="drawMethods">Methods to help draw the template</param>
        /// <param name="mousePosition">Position of the cursor when feedback is shown</param>
        void Draw(System.Drawing.Graphics gc, Render.RenderParameter r, Render.RenderHint editState, Render.IDrawVisitor drawMethods, PointD mousePosition);

        /// <summary>
        /// parameters for the filter
        /// </summary>
        Model.ToolParameter Parameters
        {
            get;
            set;
        }

        /// <summary>
        /// tests to see if point is influenced
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        bool HitTest(PointD point);

        /// <summary>
        /// resets the state of hit test upon mouse down/up
        /// </summary>
         void ResetState();

        /// <summary>
         /// get the position of the parameter handle
        /// </summary>
        /// <returns></returns>
        PointD GetParamHandlePos();

        /// <summary>
        /// template identification
        /// </summary>
        int UID
        {
            get;
        }

        /// <summary>
        /// Creates a cache of data used in the template, for example, to save in expensive computations.
        /// </summary>
        void Cache(Render.IDrawVisitor drawMethods);
    }

}
