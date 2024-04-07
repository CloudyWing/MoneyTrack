namespace CloudyWing.MoneyTrack.Models.Application {
    public class MenuViewModel {
        private static readonly Lazy<MenuViewModel> instance = new(Build);

        public string? Title { get; set; }

        public string? MenuKey { get; set; }

        public string? Url { get; set; }

        public static MenuViewModel Instance => instance.Value;

        public IList<MenuViewModel>? ChildMenus { get; set; }

        public bool IsMatch(string? menuKey) {
            return MenuKey?.Equals(menuKey, StringComparison.CurrentCultureIgnoreCase) == true;
        }

        private static MenuViewModel Build() {
            return new MenuViewModel {
                ChildMenus = [
                    new() {
                        Title = "新增交易",
                        MenuKey = "Records",
                        Url = "/Records/Index"
                    },
                    new() {
                        Title = "設定",
                        ChildMenus = [
                            new() {
                                Title = "分類管理",
                                MenuKey = "Categories",
                                Url = "/Categories/Index"
                            }
                        ]
                    },
                    new() {
                        Title = "登出",
                        MenuKey = "Home",
                        Url = "Home/Logout"
                    }
                ]
            };
        }
    }
}
