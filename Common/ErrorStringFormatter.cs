using System.Text;
using System.Text.RegularExpressions;

namespace Common;

public static class ErrorStringFormatter
{
    public static StringBuilder Format(IEnumerable<Error> errors)
    {
        var bob = new StringBuilder();
        var groups = errors.GroupBy(e => e.ErrorName);
        foreach (var group in groups)
        {
            bob.AppendLine().Append(group.Key);
            foreach (var error in group)
            {
                bob.AppendLine().Append('\t').Append(Regex.Replace(error.Description, @"\t|\n|\r", ""));
            }
        }

        return bob;
    }
}