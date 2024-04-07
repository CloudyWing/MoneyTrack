using System;
using CloudyWing.Enumeration.Abstractions;

namespace CloudyWing.MoneyTrack.Models.Enumerations {
    public class QuestionType : ShortEnumeration<QuestionType> {
        private static readonly Lazy<QuestionType> multiChoice = new Lazy<QuestionType>(() => new QuestionType(0, "單選題"));
        private static readonly Lazy<QuestionType> multiSelect = new Lazy<QuestionType>(() => new QuestionType(1, "多選題"));
        private static readonly Lazy<QuestionType> shortAnswer = new Lazy<QuestionType>(() => new QuestionType(2, "簡答題"));
        private static readonly Lazy<QuestionType> blankFilling = new Lazy<QuestionType>(() => new QuestionType(3, "填空題"));

        private QuestionType(short value, string name) : base(value, name) { }

        public QuestionType MultiChoice => multiChoice.Value;

        public QuestionType MultiSelect => multiSelect.Value;

        public QuestionType ShortAnswer => shortAnswer.Value;

        public QuestionType BlankFilling => blankFilling.Value;
    }
}
