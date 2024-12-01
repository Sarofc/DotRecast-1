/*
Copyright (c) 2009-2010 Mikko Mononen memon@inside.org
recast4j copyright (c) 2015-2019 Piotr Piastucki piotr@jtilia.org
DotRecast Copyright (c) 2023-2024 Choi Ikpil ikpil@naver.com

This software is provided 'as-is', without any express or implied
warranty.  In no event will the authors be held liable for any damages
arising from the use of this software.
Permission is granted to anyone to use this software for any purpose,
including commercial applications, and to alter it and redistribute it
freely, subject to the following restrictions:
1. The origin of this software must not be misrepresented; you must not
 claim that you wrote the original software. If you use this software
 in a product, an acknowledgment in the product documentation would be
 appreciated but is not required.
2. Altered source versions must be plainly marked as such, and must not be
 misrepresented as being the original software.
3. This notice may not be removed or altered from any source distribution.
*/

namespace DotRecast.Detour
{
    public enum DtStatus : long
    {
        // High level status.
        DT_FAILURE = (1u << 31), // Operation failed. 
        DT_SUCCESS = (1u << 30), // Operation succeed. 
        DT_IN_PROGRESS = (1u << 29), // Operation still in progress. 

        // Detail information for status.
        DT_STATUS_DETAIL_MASK = (0x0ffffff),
        DT_STATUS_NOTHING = (0), // nothing
        DT_WRONG_MAGIC = (1 << 0), // Input data is not recognized.
        DT_WRONG_VERSION = (1 << 1), // Input data is in wrong version.
        DT_OUT_OF_MEMORY = (1 << 2), // Operation ran out of memory.
        DT_INVALID_PARAM = (1 << 3), // An input parameter was invalid.
        DT_BUFFER_TOO_SMALL = (1 << 4), // Result buffer for the query was too small to store all results.
        DT_OUT_OF_NODES = (1 << 5), // Query ran out of nodes during search.
        DT_PARTIAL_RESULT = (1 << 6), // Query did not reach the end location, returning best guess. 
        DT_ALREADY_OCCUPIED = (1 << 7), // A tile has already been assigned to the given x,y coordinate
    }

    public static class DtStatusEx
    {

        public static bool IsEmpty(this DtStatus Value)
        {
            return 0 == Value;
        }

        public static bool Succeeded(this DtStatus Value)
        {
            return 0 != (Value & (DtStatus.DT_SUCCESS | DtStatus.DT_PARTIAL_RESULT));
        }

        public static bool Failed(this DtStatus Value)
        {
            return 0 != (Value & (DtStatus.DT_FAILURE | DtStatus.DT_INVALID_PARAM));
        }

        public static bool InProgress(this DtStatus Value)
        {
            return 0 != (Value & DtStatus.DT_IN_PROGRESS);
        }

        public static bool IsPartial(this DtStatus Value)
        {
            return 0 != (Value & DtStatus.DT_PARTIAL_RESULT);
        }
    }
}