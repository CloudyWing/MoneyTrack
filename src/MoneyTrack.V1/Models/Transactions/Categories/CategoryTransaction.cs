using CloudyWing.MoneyTrack.Models.Transactions.Categories;

namespace CloudyWing.MoneyTrack.Models.Transactions {
    public class CategoryTransaction : DataTransaction {
        public CategoryInsertAction CreateInsertAction(string name) {
            return SetAction(new CategoryInsertAction(name));
        }

        public CategoryUpdateAction CreateUpdateAction(long id) {
            return SetAction(new CategoryUpdateAction(id));
        }

        public CategoryDeleteAction CreateDeleteAction(long id) {
            return SetAction(new CategoryDeleteAction(id));
        }

        public CategoryMoveUpAction CreateMoveUpAction(long id) {
            return SetAction(new CategoryMoveUpAction(id));
        }

        public CategoryMoveDownAction CreateMoveDownAction(long id) {
            return SetAction(new CategoryMoveDownAction(id));
        }
    }
}
