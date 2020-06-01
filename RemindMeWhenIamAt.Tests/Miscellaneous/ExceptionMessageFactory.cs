using System;

namespace RemindMeWhenIamAt.Tests.Miscellaneous
{
    // Syntactic shugar for making possible to pass both a string and a Func<string> to methods like Verify.That()
    internal sealed class ExceptionMessageFactory
    {
        private ExceptionMessageFactory(Func<string> produceMessage) => this._produceMessage = produceMessage;

        public string CreateMessage() => _produceMessage();

        public static implicit operator ExceptionMessageFactory(string message) => new ExceptionMessageFactory(() => message);

        public static implicit operator ExceptionMessageFactory(Func<string> produceMessage) => new ExceptionMessageFactory(produceMessage);

        private readonly Func<string> _produceMessage;
   }
}