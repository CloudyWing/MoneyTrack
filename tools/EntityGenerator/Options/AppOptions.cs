namespace CloudyWing.MoneyTrack.Tools.EntityGenerator.Options {
    public class AppOptions {
        public const string Key = "App";

        public string ConnectionString { get; set; } = default!;

        public string DefaultNamespaceForSchema { get; set; } = default!;

        public string DefaultNamespaceForSql { get; set; } = default!;
    }
}
