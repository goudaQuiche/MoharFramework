using Verse;

namespace Ubet
{
    public enum Operand
    {
        [Description("AND operand")]
        and,
        [Description("OR operand")]
        or,
        [Description("NOT operand")]
        not,

        [Description("empty operand")]
        empty
    }
}
