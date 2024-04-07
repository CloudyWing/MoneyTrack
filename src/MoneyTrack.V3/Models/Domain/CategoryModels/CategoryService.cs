using System.Data.Common;
using CloudyWing.MoneyTrack.Infrastructure.Util;
using CloudyWing.MoneyTrack.Models.DataAccess;
using CloudyWing.MoneyTrack.Models.DataAccess.Entities;
using FluentValidation;

namespace CloudyWing.MoneyTrack.Models.Domain.CategoryModels {
    public class CategoryService(UnitOfWorker? unitOfWorker, IServiceProvider? serviceProvider)
        : QueryableService<CategoryQueryEntity>(unitOfWorker, serviceProvider) {
        public override string SqlBody {
            get {
                return @"
                    FROM (
                        SELECT A.*, ISNULL(B.Count, 0) RecordCount
                        FROM Categories A WITH (NOLOCK)
                        LEFT JOIN (
                            SELECT CategoryId, COUNT(1) Count
                            FROM Records WITH (NOLOCK)
                            GROUP BY CategoryId
                        ) B ON A.Id = B.CategoryId
                    ) Categories";
            }
        }

        public async Task<bool> CreateAsync(CategoryEditor editor) {
            ExceptionUtils.ThrowIfInvalid(new CreateValidator(), () => editor);

            Category entity = Mapper.Map<Category>(editor);
            entity.DisplayOrder = await UnitOfWorker.Categories.QueryCountAsync() + 1;

            UnitOfWorker.Categories.Add(entity);

            bool isOk = await UnitOfWorker.SaveChangesAsync() == 1;

            if (isOk) {
                editor.SetId(entity.Id);
            }

            return isOk;
        }

        public async Task<bool> UpdateAsync(CategoryEditor editor) {
            ExceptionUtils.ThrowIfInvalid(new UpdateValidator(), () => editor);

            Category? entity = await UnitOfWorker.Categories.QuerySingleOrDefaultAsync(new CategoryCondition {
                Id = editor.Id!,
            });

            ExceptionUtils.ThrowIfItemNotFound(entity);

            Mapper.Map(editor, entity);

            UnitOfWorker.Categories.Update(entity);

            return await UnitOfWorker.SaveChangesAsync() == 1;
        }

        public async Task<bool> DeleteAsync(long id) {
            using DbTransaction tran = UnitOfWorker.DbContext.BeginTransaction();
            RecordCondition recordCondition = new() {
                CategoryId = id!
            };

            if (await UnitOfWorker.Records.QueryExistsAsync(recordCondition)) {
                throw new ValidationException("Records 已存在相關資料。");
            }

            UnitOfWorker.Categories.Delete(id);

            if (await UnitOfWorker.SaveChangesAsync() != 1 || !await RefreshDisplayOrderAsync()) {
                return false;
            }

            tran.Commit();
            return true;
        }

        private async Task<bool> RefreshDisplayOrderAsync() {
            IEnumerable<Category> categories = await UnitOfWorker.Categories.QueryListAsync(orderBy: "DisplayOrder");

            if (!categories.Any()) {
                return true;
            }

            int i = 1;
            foreach (Category category in categories) {
                category.DisplayOrder = i++;

                UnitOfWorker.Categories.Update(category);
            }

            return await UnitOfWorker.SaveChangesAsync() > 0;
        }

        public async Task<bool> MoveUpAsync(long id) {
            Category? entity = await UnitOfWorker.Categories.QuerySingleOrDefaultAsync(new CategoryCondition {
                Id = id!,
            });

            ExceptionUtils.ThrowIfItemNotFound(entity);

            if (entity.DisplayOrder == 1) {
                throw new ValidationException("已是第一筆資料。");
            }

            Category? prevEntity = await UnitOfWorker.Categories.QuerySingleOrDefaultAsync(new CategoryCondition {
                DisplayOrder = (entity.DisplayOrder - 1)!
            });

            ExceptionUtils.ThrowIfItemNotFound(prevEntity);

            prevEntity.DisplayOrder = entity.DisplayOrder;
            entity.DisplayOrder -= 1;

            UnitOfWorker.Categories.Update(entity);
            UnitOfWorker.Categories.Update(prevEntity);

            return await UnitOfWorker.SaveChangesAsync() == 2;
        }

        public async Task<bool> MoveDownAsync(long id) {
            Category? entity = await UnitOfWorker.Categories.QuerySingleOrDefaultAsync(new CategoryCondition {
                Id = id!,
            });

            ExceptionUtils.ThrowIfItemNotFound(entity);

            long max = await UnitOfWorker.Categories.QueryCountAsync();

            if (entity.DisplayOrder == max) {
                throw new ValidationException("已是最後一筆資料。");
            }

            Category? nextEntity = await UnitOfWorker.Categories.QuerySingleOrDefaultAsync(new CategoryCondition {
                DisplayOrder = (entity.DisplayOrder + 1)!
            });

            ExceptionUtils.ThrowIfItemNotFound(nextEntity);

            nextEntity.DisplayOrder = entity.DisplayOrder;
            entity.DisplayOrder += 1;

            UnitOfWorker.Categories.Update(entity);
            UnitOfWorker.Categories.Update(nextEntity);

            return await UnitOfWorker.SaveChangesAsync() == 2;
        }

        private class CreateValidator : AbstractValidator<CategoryEditor> {
            public CreateValidator() {
                RuleFor(x => x.Name).Must(x => x.HasValue);
            }
        }

        private class UpdateValidator : AbstractValidator<CategoryEditor> {
            public UpdateValidator() {
                RuleFor(x => x.Id).NotNull();
            }
        }
    }
}
