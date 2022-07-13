using CMFDev.Toolkit.Data.Forms.Resources.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CMFDev.Toolkit.Data.Forms
{
    public record ValidationResult(bool Successful, IEnumerable<string> ErrorMessages)
    {
        public static ValidationResult Success { get; } = new(true, Enumerable.Empty<string>());

        public static ValidationResult Failure(string message) => new(false, new[] { message });
    }

    public delegate ValidationResult ValidatorFunc<T>(T? value);

    public static class Validators<T>
    {
        #region Pre-Defined Regexes
        private static readonly Regex _regexDirectory = new(@"^(([a-zA-Z]\:)|(\\))(\\{1}|((\\{1})[^\\]([^\/:*?<>""|]*))+)$", RegexOptions.Compiled);
        private static readonly Regex _regexEmail = new(@"(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9]))\.){3}(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9])|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])", RegexOptions.Compiled);
        private static readonly Regex _regexFileName = new(@"^[^</*?""\\>:|]+$");
        private static readonly Regex _regexUrl = new(@"(?i)\b((?:https?:(?:\/{1,3}|[a-z0-9%])|[a-z0-9.\-]+[.](?:com|net|org|edu|gov|mil|aero|asia|biz|cat|coop|info|int|jobs|mobi|museum|name|post|pro|tel|travel|xxx|ac|ad|ae|af|ag|ai|al|am|an|ao|aq|ar|as|at|au|aw|ax|az|ba|bb|bd|be|bf|bg|bh|bi|bj|bm|bn|bo|br|bs|bt|bv|bw|by|bz|ca|cc|cd|cf|cg|ch|ci|ck|cl|cm|cn|co|cr|cs|cu|cv|cx|cy|cz|dd|de|dj|dk|dm|do|dz|ec|ee|eg|eh|er|es|et|eu|fi|fj|fk|fm|fo|fr|ga|gb|gd|ge|gf|gg|gh|gi|gl|gm|gn|gp|gq|gr|gs|gt|gu|gw|gy|hk|hm|hn|hr|ht|hu|id|ie|il|im|in|io|iq|ir|is|it|je|jm|jo|jp|ke|kg|kh|ki|km|kn|kp|kr|kw|ky|kz|la|lb|lc|li|lk|lr|ls|lt|lu|lv|ly|ma|mc|md|me|mg|mh|mk|ml|mm|mn|mo|mp|mq|mr|ms|mt|mu|mv|mw|mx|my|mz|na|nc|ne|nf|ng|ni|nl|no|np|nr|nu|nz|om|pa|pe|pf|pg|ph|pk|pl|pm|pn|pr|ps|pt|pw|py|qa|re|ro|rs|ru|rw|sa|sb|sc|sd|se|sg|sh|si|sj|Ja|sk|sl|sm|sn|so|sr|ss|st|su|sv|sx|sy|sz|tc|td|tf|tg|th|tj|tk|tl|tm|tn|to|tp|tr|tt|tv|tw|tz|ua|ug|uk|us|uy|uz|va|vc|ve|vg|vi|vn|vu|wf|ws|ye|yt|yu|za|zm|zw)\/)(?:[^\s()<>{}\[\]]+|\([^\s()]*?\([^\s()]+\)[^\s()]*?\)|\([^\s]+?\))+(?:\([^\s()]*?\([^\s()]+\)[^\s()]*?\)|\([^\s]+?\)|[^\s`!()\[\]{};:'"".,<>?«»“”‘’])|(?:(?<!@)[a-z0-9]+(?:[.\-][a-z0-9]+)*[.](?:com|net|org|edu|gov|mil|aero|asia|biz|cat|coop|info|int|jobs|mobi|museum|name|post|pro|tel|travel|xxx|ac|ad|ae|af|ag|ai|al|am|an|ao|aq|ar|as|at|au|aw|ax|az|ba|bb|bd|be|bf|bg|bh|bi|bj|bm|bn|bo|br|bs|bt|bv|bw|by|bz|ca|cc|cd|cf|cg|ch|ci|ck|cl|cm|cn|co|cr|cs|cu|cv|cx|cy|cz|dd|de|dj|dk|dm|do|dz|ec|ee|eg|eh|er|es|et|eu|fi|fj|fk|fm|fo|fr|ga|gb|gd|ge|gf|gg|gh|gi|gl|gm|gn|gp|gq|gr|gs|gt|gu|gw|gy|hk|hm|hn|hr|ht|hu|id|ie|il|im|in|io|iq|ir|is|it|je|jm|jo|jp|ke|kg|kh|ki|km|kn|kp|kr|kw|ky|kz|la|lb|lc|li|lk|lr|ls|lt|lu|lv|ly|ma|mc|md|me|mg|mh|mk|ml|mm|mn|mo|mp|mq|mr|ms|mt|mu|mv|mw|mx|my|mz|na|nc|ne|nf|ng|ni|nl|no|np|nr|nu|nz|om|pa|pe|pf|pg|ph|pk|pl|pm|pn|pr|ps|pt|pw|py|qa|re|ro|rs|ru|rw|sa|sb|sc|sd|se|sg|sh|si|sj|Ja|sk|sl|sm|sn|so|sr|ss|st|su|sv|sx|sy|sz|tc|td|tf|tg|th|tj|tk|tl|tm|tn|to|tp|tr|tt|tv|tw|tz|ua|ug|uk|us|uy|uz|va|vc|ve|vg|vi|vn|vu|wf|ws|ye|yt|yu|za|zm|zw)\b\/?(?!@)))", RegexOptions.Compiled);
        #endregion

        public static ValidatorFunc<T> Directory { get; } = _Directory;
        public static ValidatorFunc<T> Email { get; } = Matches(_regexEmail, Strings.ValidationResult_Email);
        public static ValidatorFunc<T> FileName { get; } = Matches(_regexFileName, Strings.ValidationResult_FileName);
        public static ValidatorFunc<T> Required { get; } = _Required;
        public static ValidatorFunc<T> Url { get; } = Matches(_regexUrl, Strings.ValidationResult_Url);

        public static ValidatorFunc<T> Compose(params ValidatorFunc<T>[] validatorFuncs) => (value) =>
        {
            var errors = validatorFuncs.Select(x => x(value)).Where(x => !x.Successful).SelectMany(x => x.ErrorMessages);
            return new(!errors.Any(), errors);
        };

        public static ValidatorFunc<T> Matches(Regex pattern, string? message = null) => (value) => value is null || (value is string s && (string.IsNullOrWhiteSpace(s) || pattern.IsMatch(s))) ? ValidationResult.Success : ValidationResult.Failure(message ?? Strings.ValidationResult_Match);


        private static ValidationResult _Directory(T? value)
        {
            if (value is null || value is not string path) { return ValidationResult.Success; }
            bool isValid;
            try
            {
                var fullPath = Path.GetFullPath(path);
                var root = Path.GetPathRoot(path) ?? string.Empty;
                isValid = string.IsNullOrEmpty(root.Trim(new char[] { '\\', '/' })) == false;
                if (isValid && !_regexDirectory.IsMatch(path))
                {
                    isValid = false;
                }
            }
            catch
            {
                isValid = false;
            }
            return isValid ? ValidationResult.Success : ValidationResult.Failure(Strings.ValidationResult_Directory);
        }


        private static ValidationResult _Required(T? value) =>
           (value is null
               || (value is string s && string.IsNullOrWhiteSpace(s))
               || (value is int i && i == 0)
               || (value is long l && l == 0)
               || (value is float f && f == 0)
               || (value is double d && d == 0)
               )
               ? ValidationResult.Failure(Strings.ValidationResult_Required)
               : ValidationResult.Success;

    }
}
