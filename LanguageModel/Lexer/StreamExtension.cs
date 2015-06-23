﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LanguageModel
{
    public static class StreamExtension
    {

		public static char Peek(this Stream stream)
		{
			return stream.Peek(1);
		}

		public static char Peek(this Stream stream , int forward)
		{
			stream.Position += forward - 1;
			char c = stream.ReadChar();

			stream.Position -= forward;

			return c;
		}

		public static bool EndOfStream(this Stream stream)
		{
			return stream.Position != stream.Length - 1;
		}

		public static char ReadChar(this Stream stream)
		{
			return (char)stream.ReadByte();
		}

		public static int Read(this Stream stream, char[] buffer, int offset, int count)
		{
			int len = buffer.Length;
			byte[] _buffer = new byte[len];

			int c = stream.Read(_buffer, offset, count);

			for (int i = 0; i < len; ++i)
			{
				buffer[i] = (char)_buffer[i];
			}

			return c;

		}

    }
}
