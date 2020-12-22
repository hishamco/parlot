﻿using System;

namespace Parlot
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// Calling a Read() method without a token type won't emit a token. This can be used to reuse Read() method without other 
    /// Read() methods without trigger a new Token for each sub-call.
    /// </remarks>
    public class Scanner<T>
    {
        public readonly string Buffer;
        public Cursor Cursor;       

        public Scanner(string buffer)
        {
            Buffer = buffer;
            Cursor = new Cursor(Buffer, TextPosition.Start);
        }

        public Func<Token<T>, Token<T>> OnToken { get; set; }

        /// <summary>
        /// Reads any whitespace without generating a token.
        /// </summary>
        /// <returns>Whether some white space was read.</returns>
        public bool SkipWhiteSpace()
        {
            if (!Character.IsWhiteSpace(Cursor.Peek()))
            {
                return false;
            }

            do
            {
                Cursor.Advance();
            } while (Character.IsWhiteSpace(Cursor.Peek()));

            return true;
        }

        public Token<T> EmitToken(T tokenType, TextPosition start, TextPosition end)
        {
            var token = new Token<T>(tokenType, Buffer, start, end);

            if (OnToken is not null)
            {
                return OnToken.Invoke(token);
            }
            else
            {
                return token;
            }
        }

        public bool ReadIdentifier(Func<char, bool> identifierStart, Func<char, bool> identifierPart, out Token<T> token, T tokenType = default)
        {
            token = Token<T>.Empty;

            var start = Cursor.Position;

            if (!identifierStart(Cursor.Peek()))
            {
                return false;
            }

            // At this point we have an identifier, read while it's an identifier part.

            Cursor.Advance();

            ReadWhile(x => identifierPart(x), out _);

            token = EmitToken(tokenType, start, Cursor.Position);

            return true;
        }

        public bool ReadIdentifier(out Token<T> token, T tokenType = default)
        {
            // perf: using Character.IsIdentifierStart instead of x => Character.IsIdentifierStart(x) induces some allocations

            return ReadIdentifier(x => Character.IsIdentifierStart(x), x => Character.IsIdentifierPart(x), out token, tokenType);
        }

        public bool ReadDecimal(out Token<T> token, T tokenType = default)
        {
            token = Token<T>.Empty;

            var start = Cursor.Position;

            Cursor.RecordPosition();

            if (!Char.IsDigit(Cursor.Peek()))
            {
                Cursor.RollbackPosition();
                return false;
            }

            do
            {
                Cursor.Advance();

            } while (!Cursor.Eof && Char.IsDigit(Cursor.Peek()));

            if (Cursor.Match('.'))
            {
                Cursor.Advance();

                if (!Char.IsDigit(Cursor.Peek()))
                {
                    Cursor.RollbackPosition();
                    return false;
                }

                do
                {
                    Cursor.Advance();

                } while (!Cursor.Eof && Char.IsDigit(Cursor.Peek()));
            }

            Cursor.CommitPosition();

            var length = Cursor.Position - start; 
            
            if (length == 0)
            {
                return false;
            }

            token = EmitToken(tokenType, start, Cursor.Position);
            return true;
        }

        /// <summary>
        /// Reads a token while the specific predicate is valid.
        /// </summary>
        public bool ReadWhile(Func<char, bool> predicate, out Token<T> token, T tokenType = default)
        {
            token = Token<T>.Empty;

            var start = Cursor.Position;

            while (!Cursor.Eof && predicate(Cursor.Peek()))
            {
                Cursor.Advance();
            }

            var length = Cursor.Position - start; 
            
            if (length == 0)
            {
                return false;
            }

            token = EmitToken(tokenType, start, Cursor.Position);
            return true;
        }

        public bool ReadNonWhiteSpace(out Token<T> token, T tokenType = default)
        {
            return ReadWhile(x => !Char.IsWhiteSpace(x), out token, tokenType);
        }

        /// <summary>
        /// Reads the specific expected text.
        /// </summary>
        public bool ReadText(string text, out Token<T> token, T tokenType = default)
        {
            token = Token<T>.Empty;

            var start = Cursor.Position;

            if (!Cursor.Match(text))
            {
                return false;
            }

            Cursor.Advance(text.Length);

            token = EmitToken(tokenType, start, Cursor.Position);
            return true;
        }

        public bool ReadSingleQuotedString(out Token<T> token, T tokenType = default)
        {
            return ReadQuotedString('\'', out token, tokenType);
        }

        public bool ReadDoubleQuotedString(out Token<T> token, T tokenType = default)
        {
            return ReadQuotedString('\"', out token, tokenType);
        }

        public bool ReadQuotedString(out Token<T> token, T tokenType = default)
        {
            token = Token<T>.Empty;

            var startChar = Cursor.Peek();

            if (startChar != '\'' && startChar != '\"')
            {
                return false;
            }

            return ReadQuotedString(startChar, out token, tokenType);
        }

        /// <summary>
        /// Reads a string token enclosed in single or double quotes.
        /// </summary>
        /// <remarks>
        /// This method doesn't escape the string, but only validates its content is syntactically correct.
        /// The resulting Span contains the original quotes.
        /// </remarks>
        private bool ReadQuotedString(char quoteChar, out Token<T> token, T tokenType = default)
        {
            token = Token<T>.Empty;

            var startChar = Cursor.Peek();

            if (startChar != quoteChar)
            {
                return false;
            }

            var start = Cursor.Position;

            Cursor.RecordPosition();

            Cursor.Advance();
            
            // Fast path if there aren't any escape char until next quote
            var buffer = Cursor.Buffer.AsSpan(start.Offset + 1);

            var nextQuote = buffer.IndexOf(startChar);

            // Is there an end quote?
            if (nextQuote != -1)
            {
                var nextEscape = buffer.IndexOf('\\');

                // If the next escape if not before the next quote, we can return the string as-is
                if (nextEscape == -1 || nextEscape > nextQuote)
                {
                    Cursor.Advance(nextQuote + 1);

                    Cursor.CommitPosition();

                    token = EmitToken(tokenType, start, Cursor.Position);
                    return true;
                }
            }
            else
            {
                // There is no end quote
                Cursor.RollbackPosition();

                return false;
            }

            // TODO: Can we reuse the ReadOnlySpan buffer?

            while (!Cursor.Match(startChar))
            {
                if (Cursor.Eof)
                {
                    return false;
                }

                if (Cursor.Match("\\"))
                {
                    Cursor.Advance();

                    switch (Cursor.Peek())
                    {
                        case '0':
                        case '\'':
                        case '"':
                        case '\\':
                        case 'b':
                        case 'f':
                        case 'n':
                        case 'r':
                        case 't':
                        case 'v':
                            break;
                        case 'u':
                            var isValidUnicode = false;

                            Cursor.Advance();

                            if (!Cursor.Eof && Character.IsHexDigit(Cursor.Peek()))
                            {
                                Cursor.Advance();
                                if (!Cursor.Eof && Character.IsHexDigit(Cursor.Peek()))
                                {
                                    Cursor.Advance();
                                    if (!Cursor.Eof && Character.IsHexDigit(Cursor.Peek()))
                                    {
                                        Cursor.Advance();
                                        isValidUnicode = true;
                                    }
                                }
                            }

                            if (!isValidUnicode)
                            {
                                Cursor.RollbackPosition();

                                return false;
                            }

                            break;
                        case 'x':
                            bool isValidHex = false;

                            Cursor.Advance();

                            if (!Cursor.Eof && Character.IsHexDigit(Cursor.Peek()))
                            {
                                Cursor.Advance();
                                if (!Cursor.Eof && Character.IsHexDigit(Cursor.Peek()))
                                {
                                    isValidHex = true;
                                }
                            }

                            if (!isValidHex)
                            {
                                Cursor.RollbackPosition();
                                
                                return false;
                            }

                            break;
                        default:
                            Cursor.RollbackPosition();

                            return false;
                    }
                }

                Cursor.Advance();
            }

            Cursor.Advance();

            Cursor.CommitPosition();

            token = EmitToken(tokenType, start, Cursor.Position);

            return true;
        }
    }
}