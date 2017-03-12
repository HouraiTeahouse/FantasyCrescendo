// The MIT License (MIT)
// 
// Copyright (c) 2016 Hourai Teahouse
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Collections.Generic;
using UnityEngine;

public class BoxOutline : ModifiedShadow {
    const int maxHalfSampleCount = 20;

    [SerializeField]
    [Range(1, maxHalfSampleCount)]
    int m_halfSampleCountX = 1;

    [SerializeField]
    [Range(1, maxHalfSampleCount)]
    int m_halfSampleCountY = 1;

    public int halfSampleCountX {
        get { return m_halfSampleCountX; }

        set {
            m_halfSampleCountX = Mathf.Clamp(value, 1, maxHalfSampleCount);
            if (graphic != null)
                graphic.SetVerticesDirty();
        }
    }

    public int halfSampleCountY {
        get { return m_halfSampleCountY; }

        set {
            m_halfSampleCountY = Mathf.Clamp(value, 1, maxHalfSampleCount);
            if (graphic != null)
                graphic.SetVerticesDirty();
        }
    }

    public override void ModifyVertices(List<UIVertex> verts) {
        if (!IsActive())
            return;

        verts.Capacity = verts.Count * (m_halfSampleCountX * 2 + 1)
            * (m_halfSampleCountY * 2 + 1);
        int original = verts.Count;
        var count = 0;
        float dx = effectDistance.x / m_halfSampleCountX;
        float dy = effectDistance.y / m_halfSampleCountY;
        for (int x = -m_halfSampleCountX; x <= m_halfSampleCountX; x++) {
            for (int y = -m_halfSampleCountY; y <= m_halfSampleCountY; y++) {
                if (!(x == 0 && y == 0)) {
                    int next = count + original;
                    ApplyShadow(verts, effectColor, count, next, dx * x, dy * y);
                    count = next;
                }
            }
        }
    }
}