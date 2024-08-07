// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Authentication.ExtendedProtection;

namespace System.Net.Security
{
    [StructLayout(LayoutKind.Sequential)]
    internal ref struct InputSecurityBuffers
    {
        internal int Count;
        internal InputSecurityBuffer _item0;
        internal InputSecurityBuffer _item1;
        internal InputSecurityBuffer _item2;

        internal void SetNextBuffer(InputSecurityBuffer buffer)
        {
            Debug.Assert(Count >= 0 && Count < 3);
            if (Count == 0)
            {
                _item0 = buffer;
            }
            else if (Count == 1)
            {
                _item1 = buffer;
            }
            else
            {
                _item2 = buffer;
            }

            Count++;
        }
    }

    [StructLayout(LayoutKind.Auto)]
    internal ref struct InputSecurityBuffer
    {
        public SecurityBufferType Type;
        public ReadOnlySpan<byte> Token;
        public SafeHandle? UnmanagedToken;

        public InputSecurityBuffer(ReadOnlySpan<byte> data, SecurityBufferType tokentype)
        {
            Token = data;
            Type = tokentype;
            UnmanagedToken = null;
        }

        public InputSecurityBuffer(ChannelBinding binding)
        {
            Type = SecurityBufferType.SECBUFFER_CHANNEL_BINDINGS;
            Token = default;
            UnmanagedToken = binding;
        }
    }

    [StructLayout(LayoutKind.Auto)]
    internal struct SecurityBuffer
    {
        public int offset;
        public int size;
        public SecurityBufferType type;
        public byte[]? token;
        public SafeHandle? unmanagedToken;

        public SecurityBuffer(byte[]? data, int offset, int size, SecurityBufferType tokentype)
        {
            Debug.Assert(offset >= 0 && offset <= (data == null ? 0 : data.Length), $"'offset' out of range.  [{offset}]");
            Debug.Assert(size >= 0 && size <= (data == null ? 0 : data.Length - offset), $"'size' out of range.  [{size}]");

            this.offset = data == null || offset < 0 ? 0 : Math.Min(offset, data.Length);
            this.size = data == null || size < 0 ? 0 : Math.Min(size, data.Length - this.offset);
            this.type = tokentype;
            this.token = size == 0 ? null : data;
            this.unmanagedToken = null;
        }

        public SecurityBuffer(byte[]? data, SecurityBufferType tokentype)
        {
            this.offset = 0;
            this.size = data == null ? 0 : data.Length;
            this.type = tokentype;
            this.token = size == 0 ? null : data;
            this.unmanagedToken = null;
        }

        public SecurityBuffer(int size, SecurityBufferType tokentype)
        {
            Debug.Assert(size >= 0, $"'size' out of range.  [{size}]");

            this.offset = 0;
            this.size = size;
            this.type = tokentype;
            this.token = size == 0 ? null : new byte[size];
            this.unmanagedToken = null;
        }

        public SecurityBuffer(ChannelBinding binding)
        {
            this.offset = 0;
            this.size = (binding == null ? 0 : binding.Size);
            this.type = SecurityBufferType.SECBUFFER_CHANNEL_BINDINGS;
            this.token = null;
            this.unmanagedToken = binding;
        }
    }
}
