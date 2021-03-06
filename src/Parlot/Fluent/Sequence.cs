﻿using System;
using System.Collections.Generic;

namespace Parlot.Fluent
{
    public sealed class Sequence<T1, T2> : Parser<ValueTuple<T1, T2>>
    {
        internal readonly IParser<T1> _parser1;
        internal readonly IParser<T2> _parser2;
        public Sequence(IParser<T1> parser1, IParser<T2> parser2)
        {
            _parser1 = parser1 ?? throw new ArgumentNullException(nameof(parser1));
            _parser2 = parser2 ?? throw new ArgumentNullException(nameof(parser2));
        }

        public override bool Parse(ParseContext context, ref ParseResult<ValueTuple<T1, T2>> result)
        {
            context.EnterParser(this);

            var parseResult1 = new ParseResult<T1>();

            if (_parser1.Parse(context, ref parseResult1))
            {
                var parseResult2 = new ParseResult<T2>();

                if (_parser2.Parse(context, ref parseResult2))
                {
                    result.Set(parseResult1.Buffer, parseResult1.Start, parseResult2.End, new ValueTuple<T1, T2>(parseResult1.Value, parseResult2.Value));
                    return true;
                }
            }

            return false;
        }
    }

    public sealed class Sequence<T1, T2, T3> : Parser<ValueTuple<T1, T2, T3>>
    {
        private readonly IParser<ValueTuple<T1, T2>> _parser;
        internal readonly IParser<T3> _lastParser;

        public Sequence(IParser<ValueTuple<T1, T2>> 
            parser,
            IParser<T3> lastParser
            )
        {
            _parser = parser;
            _lastParser = lastParser ?? throw new ArgumentNullException(nameof(lastParser));
        }

        public override bool Parse(ParseContext context, ref ParseResult<ValueTuple<T1, T2, T3>> result)
        {
            context.EnterParser(this);

            var tupleResult = new ParseResult<ValueTuple<T1, T2>>();

            if (_parser.Parse(context, ref tupleResult))
            {
                var lastResult = new ParseResult<T3>();

                if (_lastParser.Parse(context, ref lastResult))
                {
                    var tuple = new ValueTuple<T1, T2, T3>(
                        tupleResult.Value.Item1,
                        tupleResult.Value.Item2,
                        lastResult.Value
                        );
                        
                    result.Set(tupleResult.Buffer, tupleResult.Start, lastResult.End, tuple);
                    return true;
                }
            }

            return false;
        }
    }

    public sealed class Sequence<T1, T2, T3, T4> : Parser<ValueTuple<T1, T2, T3, T4>>
    {
        private readonly IParser<ValueTuple<T1, T2, T3>> _parser;
        internal readonly IParser<T4> _lastParser;

        public Sequence(IParser<ValueTuple<T1, T2, T3>> parser, IParser<T4> lastParser)
        {
            _parser = parser;
            _lastParser = lastParser ?? throw new ArgumentNullException(nameof(lastParser));
        }

        public override bool Parse(ParseContext context, ref ParseResult<ValueTuple<T1, T2, T3, T4>> result)
        {
            context.EnterParser(this);

            var tupleResult = new ParseResult<ValueTuple<T1, T2, T3>>();

            if (_parser.Parse(context, ref tupleResult))
            {
                var lastResult = new ParseResult<T4>();

                if (_lastParser.Parse(context, ref lastResult))
                {
                    var tuple = new ValueTuple<T1, T2, T3, T4>(
                        tupleResult.Value.Item1,
                        tupleResult.Value.Item2,
                        tupleResult.Value.Item3,
                        lastResult.Value
                        );

                    result.Set(tupleResult.Buffer, tupleResult.Start, lastResult.End, tuple);
                    return true;
                }
            }

            return false;
        }
    }
    
    public sealed class Sequence<T1, T2, T3, T4, T5> : Parser<ValueTuple<T1, T2, T3, T4, T5>>
    {
        private readonly IParser<ValueTuple<T1, T2, T3, T4>> _parser;
        internal readonly IParser<T5> _lastParser;
        
        public Sequence(IParser<ValueTuple<T1, T2, T3, T4>> parser, IParser<T5> lastParser)
        {
            _parser = parser;
            _lastParser = lastParser ?? throw new ArgumentNullException(nameof(lastParser));
        }

        public override bool Parse(ParseContext context, ref ParseResult<ValueTuple<T1, T2, T3, T4, T5>> result)
        {
            context.EnterParser(this);

            var tupleResult = new ParseResult<ValueTuple<T1, T2, T3, T4>>();

            if (_parser.Parse(context, ref tupleResult))
            {
                var lastResult = new ParseResult<T5>();

                if (_lastParser.Parse(context, ref lastResult))
                {
                    var tuple = new ValueTuple<T1, T2, T3, T4, T5>(
                        tupleResult.Value.Item1,
                        tupleResult.Value.Item2,
                        tupleResult.Value.Item3,
                        tupleResult.Value.Item4,
                        lastResult.Value
                        );

                    result.Set(tupleResult.Buffer, tupleResult.Start, lastResult.End, tuple);
                    return true;
                }
            }

            return false;
        }
    }

    public sealed class Sequence<T1, T2, T3, T4, T5, T6> : Parser<ValueTuple<T1, T2, T3, T4, T5, T6>>
    {
        private readonly IParser<ValueTuple<T1, T2, T3, T4, T5>> _parser;
        internal readonly IParser<T6> _lastParser;        

        public Sequence(IParser<ValueTuple<T1, T2, T3, T4, T5>> parser, IParser<T6> lastParser)
        {
            _parser = parser;
            _lastParser = lastParser ?? throw new ArgumentNullException(nameof(lastParser));
        }

        public override bool Parse(ParseContext context, ref ParseResult<ValueTuple<T1, T2, T3, T4, T5, T6>> result)
        {
            context.EnterParser(this);

            var tupleResult = new ParseResult<ValueTuple<T1, T2, T3, T4, T5>>();

            if (_parser.Parse(context, ref tupleResult))
            {
                var lastResult = new ParseResult<T6>();

                if (_lastParser.Parse(context, ref lastResult))
                {
                    var tuple = new ValueTuple<T1, T2, T3, T4, T5, T6>(
                        tupleResult.Value.Item1,
                        tupleResult.Value.Item2,
                        tupleResult.Value.Item3,
                        tupleResult.Value.Item4,
                        tupleResult.Value.Item5,
                        lastResult.Value
                        );

                    result.Set(tupleResult.Buffer, tupleResult.Start, lastResult.End, tuple);
                    return true;
                }
            }

            return false;
        }
    }

    public sealed class Sequence<T1, T2, T3, T4, T5, T6, T7> : Parser<ValueTuple<T1, T2, T3, T4, T5, T6, T7>>
    {
        private readonly IParser<ValueTuple<T1, T2, T3, T4, T5, T6>> _parser;
        internal readonly IParser<T7> _lastParser;

        public Sequence(
            IParser<ValueTuple<T1, T2, T3, T4, T5, T6>> parser, IParser<T7> lastParser)
        {
            _parser = parser;
            _lastParser = lastParser ?? throw new ArgumentNullException(nameof(lastParser));
        }

        public override bool Parse(ParseContext context, ref ParseResult<ValueTuple<T1, T2, T3, T4, T5, T6, T7>> result)
        {
            context.EnterParser(this);

            var tupleResult = new ParseResult<ValueTuple<T1, T2, T3, T4, T5, T6>>();

            if (_parser.Parse(context, ref tupleResult))
            {
                var lastResult = new ParseResult<T7>();

                if (_lastParser.Parse(context, ref lastResult))
                {
                    var tuple = new ValueTuple<T1, T2, T3, T4, T5, T6, T7>(
                        tupleResult.Value.Item1,
                        tupleResult.Value.Item2,
                        tupleResult.Value.Item3,
                        tupleResult.Value.Item4,
                        tupleResult.Value.Item5,
                        tupleResult.Value.Item6,
                        lastResult.Value
                        );

                    result.Set(tupleResult.Buffer, tupleResult.Start, lastResult.End, tuple);
                    return true;
                }
            }

            return false;
        }
    }

    public sealed class Sequence : Parser<IList<ParseResult<object>>>
    {
        internal readonly IParser[] _parsers;
        public Sequence(IParser[] parsers)
        {
            _parsers = parsers;
        }

        public override bool Parse(ParseContext context, ref ParseResult<IList<ParseResult<object>>> result)
        {
            context.EnterParser(this);

            if (_parsers.Length == 0)
            {
                return true;
            }

            var results = new List<ParseResult<object>>(_parsers.Length);

            var success = true;

            var parsed = new ParseResult<object>();

            for (var i = 0; i < _parsers.Length; i++)
            {
                if (!_parsers[i].Parse(context, ref parsed))
                {
                    success = false;
                    break;
                }

                results[i] = parsed;
            }

            if (success)
            {
                result.Set(results[0].Buffer, results[0].Start, results[^1].End, results);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
