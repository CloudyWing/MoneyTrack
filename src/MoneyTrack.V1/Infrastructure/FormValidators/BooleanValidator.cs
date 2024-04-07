namespace CloudyWing.FormValidators {
    public class BooleanValidator : IFormValidator {
        public BooleanValidator(string column, string value) {
            Column = column;
            Value = value;
        }

        public string Column { get; }

        public string Value { get; }

        public string ErrorMessage { get; private set; }

        public bool IsValid { get; private set; }

        public bool Validate() {
            if (string.IsNullOrEmpty(Value)) {
                return true;
            }

            IsValid = bool.TryParse(Value, out _);

            if (!IsValid) {
                ErrorMessage = $"{Column} 格式錯誤。";
            }

            return IsValid;
        }
    }
}
