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
using System.Xml;

namespace KinTemplates.Cursor.Tools.Model
{
    public abstract class ToolParameter : ICloneable
    {
        // meta-properties
        public bool FilterEnabled;
        public bool FilterVisible;
        public string FilterName;

        public PointD V;            // Vector for directional effect
        public double C;            // scalar constant for strength -- can change to C1 and C2, gradient strength

        public bool InUse;          // feedback for the kinematic templates list
        public int UseCounter;      // count number of times used

        public PathShape Shape;     // shape of the path
        public double Rotation;     // rotation angle of the path, in radians

        public abstract Model.IPath Path { get; set; }      // path that defines the outline of the applicable region
        public double PathThickness { get; set; }           // Line thickness

        public abstract PointD Pt { get; set; }             // Point source point
        public abstract PointD PtSize { get; set; }         // Point source size


 
        public ToolParameter(PathShape shape)
        {
            C = 0.75;
            FilterEnabled = true;
            FilterVisible = true;
            InUse = false;
            Shape = shape;
        }

        /// <summary>
        /// Gets the unrotated bounding rectangle.
        /// </summary>
        /// <returns></returns>
        public abstract RectangleF GetBoundingRect();

        /// <summary>
        /// Sets the unrotated bounding rectangle.
        /// </summary>
        /// <param name="bounds"></param>
        public abstract void SetBoundingRect(RectangleF bounds);

        /// <summary>
        /// Gets the visible area occupied by the shape, including rotation.
        /// </summary>
        /// <returns></returns>
        public abstract RectangleF GetVisibleRect();

        [Obsolete()]
        public double PtRadius
        {
            get
            {
                return Math.Min(PtSize.X, PtSize.Y) / 2;
            }
        }

        #region ICloneable Members

        public abstract object Clone();

        protected static void CopyObject(ToolParameter oldParam, ToolParameter newParam)
        {
            newParam.C = oldParam.C;
            newParam.V = oldParam.V;

            newParam.FilterEnabled = oldParam.FilterEnabled;
            newParam.FilterVisible = oldParam.FilterVisible;
            newParam.FilterName = oldParam.FilterName;

            newParam.InUse = oldParam.InUse;
            newParam.UseCounter = oldParam.UseCounter;

            newParam.Shape = oldParam.Shape;
            newParam.Rotation = oldParam.Rotation;
            
        }

        #endregion
    }
}
