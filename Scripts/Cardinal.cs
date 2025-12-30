/*
 * Copyright 2025 yiroth
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 * Purpose: Common helper functions
 */

using System.Collections.Generic;
using System.Linq;

namespace LibYiroth.Helper
{
    public static class Cardinal
    {
        public static bool IsValid(object check)
        {
            return !check.Equals(null);
        }

        public static bool IsNotValid(object check)
        {
            return check.Equals(null);
        }
        
        // Source - https://stackoverflow.com/a
        // Posted by classicSchmosby98
        // Retrieved 2025-12-30, Licence - CC BY-SA 4.0
        // Modified by yiroth
        public static bool TryGetElementByIndex<T>(List<T> list, int index, out T element)
        {
            var e = list.ElementAtOrDefault(index);

            if (IsValid(e))
            {
                element = e;
                return true;
            }

            element = default;
            return false;
        }
    }
}