using System.Diagnostics;

namespace CloudyWing.MoneyTrack.Infrastructure.Application {
    public class HostEnvironment {
        private EnvironmentState state = EnvironmentState.Staging;

        public HostEnvironment() {
            InitializeIfDevelopment();
            InitializeIfProduction();
        }

        public string ApplicationName => "MoneyTrack";

        public int MajorVersion => 0;

        public int MinorVersion => 0;

        public int Revision => 0;

        public string Version => $@"{MajorVersion}.{MinorVersion}.{Revision}{GetVersionPostfix()}";

        public bool IsDevelopment => state == EnvironmentState.Development;

        public bool IsStaging => state == EnvironmentState.Staging;

        public bool IsProduction => state == EnvironmentState.Production;

        [Conditional("DEBUG")]
        private void InitializeIfDevelopment() {
            state = EnvironmentState.Development;
        }

        [Conditional("RELEASE")]
        private void InitializeIfProduction() {
            state = EnvironmentState.Production;
        }

        private string GetVersionPostfix() {
            return IsProduction ? "" : $".{state}";
        }

        private enum EnvironmentState {
            Development,
            Staging,
            Production
        }
    }
}
