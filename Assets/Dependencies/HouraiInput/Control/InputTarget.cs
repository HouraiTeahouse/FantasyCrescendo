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

namespace HouraiTeahouse.HouraiInput {
    public enum InputTarget {
        None = 0,

        // Standardized.
        //
        LeftStickX,
        LeftStickY,
        LeftStickButton,

        RightStickX,
        RightStickY,
        RightStickButton,

        DPadUp,
        DPadDown,
        DPadLeft,
        DPadRight,

        Action1,
        Action2,
        Action3,
        Action4,

        LeftTrigger,
        RightTrigger,

        LeftBumper,
        RightBumper,


        // Not standardized, but provided for convenience.
        //
        Back,
        Start,
        Select,
        System,
        Pause,
        Menu,
        Share,
        View,
        Options,
        TiltX,
        TiltY,
        TiltZ,
        ScrollWheel,
        TouchPadTap,
        TouchPadXAxis,
        TouchPadYAxis,


        // Not standardized.
        //
        Analog0,
        Analog1,
        Analog2,
        Analog3,
        Analog4,
        Analog5,
        Analog6,
        Analog7,
        Analog8,
        Analog9,
        Analog10,
        Analog11,
        Analog12,
        Analog13,
        Analog14,
        Analog15,
        Analog16,
        Analog17,
        Analog18,
        Analog19,

        Button0,
        Button1,
        Button2,
        Button3,
        Button4,
        Button5,
        Button6,
        Button7,
        Button8,
        Button9,
        Button10,
        Button11,
        Button12,
        Button13,
        Button14,
        Button15,
        Button16,
        Button17,
        Button18,
        Button19
    }
}