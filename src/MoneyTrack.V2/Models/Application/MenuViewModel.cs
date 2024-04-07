using System;
using System.Collections.Generic;

namespace CloudyWing.MoneyTrack.Models.Application {
    public class MenuViewModel {
        private static readonly Lazy<MenuViewModel> instance = new Lazy<MenuViewModel>(() => Build());

        public string Title { get; set; }

        public string Area { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        public static MenuViewModel Instance => instance.Value;

        public IList<MenuViewModel> ChildMenus { get; set; }

        public bool IsMatch(string area, string controller, string action = null) {
            if (Area?.ToLower() != area?.ToLower()) {
                return false;
            }

            if (Controller?.ToLower() != controller?.ToLower()) {
                return false;
            }

            if (Action != null && action != null) {
                if (Action.ToLower() != action.ToLower()) {
                    return false;
                }
            }

            return true;
        }

        private static MenuViewModel Build() {
            return new MenuViewModel {
                ChildMenus = new List<MenuViewModel> {
                    new MenuViewModel {
                        Title = "新增交易",
                        Controller = "Record",
                        Area = "Administrators"
                    },
                    new MenuViewModel {
                        Title = "設定",
                        ChildMenus = new List<MenuViewModel> {
                            new MenuViewModel {
                                Title = "分類管理",
                                Controller = "Category",
                                Area = "Administrators"
                            }
                        }
                    },
                    new MenuViewModel {
                        Title = "登出",
                        Controller = "Home",
                        Action = "Logout"
                    }
                }
            };
        }
    }
}
