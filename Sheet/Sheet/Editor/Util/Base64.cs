// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.IO;

namespace Sheet.Editor
{
    public class Base64 : IBase64
    {
        public string ToBase64(byte[] bytes)
        {
            if (bytes != null)
            {
                return Convert.ToBase64String(bytes);
            }
            return null;
        }

        public MemoryStream ToStream(byte[] bytes)
        {
            if (bytes != null)
            {
                return new MemoryStream(bytes, 0, bytes.Length);
            }
            return null;
        }

        public byte[] ToBytes(string base64)
        {
            if (!string.IsNullOrEmpty(base64))
            {
                return Convert.FromBase64String(base64);
            }
            return null;
        }

        public MemoryStream ToStream(string base64)
        {
            if (!string.IsNullOrEmpty(base64))
            {
                byte[] bytes = ToBytes(base64);
                if (bytes != null)
                {
                    return new MemoryStream(bytes, 0, bytes.Length);
                }
                return null;
            }
            return null;
        }

        public byte[] ReadAllBytes(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                return System.IO.File.ReadAllBytes(path);
            }
            return null;
        }

        public string FromFileToBase64(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                byte[] bytes = ReadAllBytes(path);
                if (bytes != null)
                {
                    return ToBase64(bytes);
                }
                return null;
            }
            return null;
        }

        public MemoryStream FromFileToStream(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                byte[] bytes = ReadAllBytes(path);
                if (bytes != null)
                {
                    return new MemoryStream(bytes, 0, bytes.Length);
                }
                return null;
            }
            return null;
        }
    }
}
