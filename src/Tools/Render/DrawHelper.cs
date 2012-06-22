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

namespace KinTemplates.Cursor.Tools.Render
{
    public static class DrawHelper
    {
        /// <summary>
        /// Columns of arrows for some templates
        /// </summary>
        public const int X_ARROWS = 3;

        /// <summary>
        /// Rows of arrows for some templates
        /// </summary>
        public const int Y_ARROWS = 3;

        /// <summary>
        /// Number of rings for point-based templates
        /// </summary>
        public const int PT_RINGS = 4;

        /// <summary>
        /// When visualizing speed sensitivity, we have to amplify it so the user can see it.
        /// </summary>
        public const double SPEED_AMPLIFIER = 20;
        
        /// <summary>
        /// Smaller value of SPEED_AMPLIFIER
        /// </summary>
        public const double SMALL_SPEED_AMPLIFIER = 10;
        
        /// <summary>
        /// Don't remember
        /// </summary>
        public const int TARGET_SIZE = 30;
        
        /// <summary>
        /// Only take effect if the speed of the cursor exceeds this value.
        /// </summary>
        public const double MIN_SPEED_SENSITIVITY = 0.001;

        /// <summary>
        /// The border that is used for tracing along the edges of templates.
        /// </summary>
        public const float EDGE_WIDTH = 8f;                     
    }
}
