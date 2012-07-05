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

namespace KinTemplates.Cursor.Position
{
    public interface IVirtualMousePosition
    {
        long GetTimeInterval();
        long GetTimeStamp();

        System.Drawing.Point GetLastPhysicalPoint();
        System.Drawing.Point GetPhysicalPoint();

        System.Drawing.Point GetLastVirtualPoint();
        System.Drawing.Point GetVirtualPoint();

        PointD GetDisplacement();
        double GetDistance();
        PointD GetUnitDirection();

        PointD GetLastVirtualPointD();
        PointD GetVirtualPointD();

        double GetSpeed();
        PointD GetVelocity();

        PointD GetAcceleration();

    }
}
