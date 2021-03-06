﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Formatting;
using System.IO;
using Xunit;

namespace System.Text.Formatting.Tests
{
    struct Age : IBufferFormattable
    {
        int _age;
        bool _inMonths;

        public Age(int age, bool inMonths = false)
        {
            _age = age;
            _inMonths = inMonths;
        }

        public bool TryFormat(Span<byte> buffer, Format.Parsed format, FormattingData formattingData, out int bytesWritten)
        {
            if (!PrimitiveFormatters.TryFormat(_age, buffer, format, formattingData, out bytesWritten)) return false;


            char symbol = _inMonths ? 'm' : 'y';
            int symbolBytes;
            if (!PrimitiveFormatters.TryFormat(symbol, buffer.Slice(bytesWritten), format, formattingData, out symbolBytes)) return false;

            bytesWritten += symbolBytes;
            return true;
        }

        public override string ToString()
        {
            return _age.ToString() + (_inMonths ? "m" : "y");
        }
    }

    public class CustomTypeFormatting
    {


        [Fact]
        public void CustomTypeToStreamUtf16()
        {
            byte[] buffer = new byte[1024];
            MemoryStream stream = new MemoryStream(buffer);
            var writer = new StreamFormatter(stream);
            writer.Append(new Age(56));
            writer.Append(new Age(14, inMonths: true));

            var writtenText = Encoding.Unicode.GetString(buffer, 0, (int)stream.Position);
            Assert.Equal(writtenText, "56y14m");
        }

        [Fact]
        public void CustomTypeToStreamUtf8()
        {
            byte[] buffer = new byte[1024];
            MemoryStream stream = new MemoryStream(buffer);
            var writer = new StreamFormatter(stream, FormattingData.InvariantUtf8);
            writer.Append(new Age(56));
            writer.Append(new Age(14, inMonths: true));
            var writtenText = Encoding.UTF8.GetString(buffer, 0, (int)stream.Position);
            Assert.Equal(writtenText, "56y14m");
        }

        [Fact]
        public void CustomTypeToString()
        {
            var sb = new StringFormatter();
            sb.Append(new Age(56));
            sb.Append(new Age(14, inMonths: true));
            Assert.Equal(sb.ToString(), "56y14m");
        }
    }
}
