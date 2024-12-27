namespace DotRecast.Recast
{
    public struct RcCompactSpanBuilder
    {
        public ushort y;
        public ushort reg;
        public int con;
        public int h;

        public static RcCompactSpanBuilder NewBuilder(ref RcCompactSpan span)
        {
            var builder = new RcCompactSpanBuilder
            {
                y = span.y,
                reg = span.reg,
                con = span.con,
                h = span.h
            };
            return builder;
        }

        public RcCompactSpanBuilder WithReg(ushort reg)
        {
            this.reg = reg;
            return this;
        }

        public RcCompactSpan Build()
        {
            return new RcCompactSpan(this);
        }
    }
}