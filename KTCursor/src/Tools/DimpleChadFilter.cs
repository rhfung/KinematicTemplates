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
    public class DimpleChadFilter : PointToolFilter
    {
        public override string ToString()
        {
            return "Vanishing Point";
        }
        #region IToolFilter Members

        protected override RadialFactors GetInnerVelocity(Position.RadialMousePosition mp)
        {
            return new RadialFactors(1, (1 - m_Param.C), 0, 0);
        }

        public override PointD GetParamHandlePos()
        {
            return PointD.Empty;
        }


        #endregion
    }
}
