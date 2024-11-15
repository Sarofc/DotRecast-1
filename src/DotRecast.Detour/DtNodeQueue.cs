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
    public class DtNodeQueue
    {
        private DtNode[] m_heap;
        private int m_size;

        public DtNodeQueue(int capacity)
        {
            System.Diagnostics.Debug.Assert(capacity > 0);
            m_heap = new DtNode[capacity];
            m_size = 0;
        }

        public void Clear() { m_size = 0; }

        public DtNode Top() { return m_heap[0]; }

        public DtNode Pop()
        {
            DtNode result = m_heap[0];
            m_size--;
            trickleDown(0, m_heap[m_size]);
            return result;
        }

        public void Push(DtNode node)
        {
            System.Diagnostics.Debug.Assert(node != null);
            m_size++;
            bubbleUp(m_size - 1, node);
        }

        public void Modify(DtNode node)
        {
            System.Diagnostics.Debug.Assert(node != null);
            for (int i = 0; i < m_size; ++i)
            {
                if (m_heap[i] == node)
                {
                    bubbleUp(i, node);
                    return;
                }
            }
        }

        public int Count() => m_size;

        public bool IsEmpty() { return m_size == 0; }

        public int GetCapacity() { return m_heap.Length; }

        void bubbleUp(int i, DtNode node)
        {
            int parent = (i - 1) / 2;
            // note: (index > 0) means there is a parent
            while ((i > 0) && (m_heap[parent].total > node.total))
            {
                m_heap[i] = m_heap[parent];
                i = parent;
                parent = (i - 1) / 2;
            }
            m_heap[i] = node;
        }

        void trickleDown(int i, DtNode node)
        {
            int child = (i * 2) + 1;
            while (child < m_size)
            {
                if (((child + 1) < m_size) &&
                    (m_heap[child].total > m_heap[child + 1].total))
                {
                    child++;
                }
                m_heap[i] = m_heap[child];
                i = child;
                child = (i * 2) + 1;
            }
            bubbleUp(i, node);
        }
    }
}