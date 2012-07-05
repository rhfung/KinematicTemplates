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
    public class HillFilter : PointToolFilter
    {
        public override string ToString()
        {
            return "Hill";
        }

        public override PointD GetParamHandlePos()
        {
            return PointD.Empty;
        }

        protected override RadialFactors GetInnerVelocity(Position.RadialMousePosition m)
        {
            if (m.Delta.R < 0)
            {
                return new RadialFactors((1 - m_Param.C) * (m.VirtualPoint.R / m_Param.PtRadius) , 1, 0, 0);
            }
            else
            {
                return new RadialFactors(1, 1, 0, 0);
            }
        }
    }
    
}
