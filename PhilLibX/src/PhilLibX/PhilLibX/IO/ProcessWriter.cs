// ------------------------------------------------------------------------
// PhilLibX - My Utility Library
// Copyright(c) 2018 Philip/Scobalula
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// ------------------------------------------------------------------------
// File: IO/ProcessWriter.cs
// Author: Philip/Scobalula
// Description: A class to help with writing to the memory of other processes.
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

// TODO: Extend on this

namespace PhilLibX.IO
{
    /// <summary>
    /// A class to help with writing to the memory of other processes.
    /// </summary>
    public class ProcessWriter
    {
        /// <summary>
        /// Internal Process Property
        /// </summary>
        private Process _Process { get; set; }

        /// <summary>
        /// Internal Handle Property
        /// </summary>
        private IntPtr _Handle { get; set; }

        /// <summary>
        /// Active Process
        /// </summary>
        public Process ActiveProcess
        {
            get { return _Process; }
            set
            {
                _Process = value;
                _Handle = NativeMethods.OpenProcess(MemoryUtil.ProcessVMOperation | MemoryUtil.ProcessVMWrite, false, _Process.Id);
            }
        }

        /// <summary>
        /// Active Process Handle
        /// </summary>
        public IntPtr Handle { get { return _Handle; } }

        /// <summary>
        /// Initalizes a Process Reader with a Process
        /// </summary>
        public ProcessWriter(Process process)
        {
            ActiveProcess = process;
        }

        /// <summary>
        /// Writes bytes to the processes' memory
        /// </summary>
        /// <param name="address"></param>
        /// <param name="buffer"></param>
        public void WriteBytes(long address, byte[] buffer)
        {
            if (!NativeMethods.WriteProcessMemory((int)Handle, address, buffer, buffer.Length, out int bytesRead))
            {
                throw new ArgumentException(new Win32Exception(Marshal.GetLastWin32Error()).Message);
            }
        }
    }
}
