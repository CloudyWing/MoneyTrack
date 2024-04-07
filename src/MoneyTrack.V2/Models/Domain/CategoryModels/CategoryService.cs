using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using CloudyWing.MoneyTrack.Infrastructure.Util;
using CloudyWing.MoneyTrack.Models.DataAccess;
using CloudyWing.MoneyTrack.Models.DataAccess.Entities;
using CloudyWing.MoneyTrack.Models.Domain.CategoryModels;
using Dapper;
using FluentValidation;

namespace CloudyWing.MoneyTrack.Models.Domain.CategoryModels {
    public class CategoryService : QueryableService<CategoryQueryCondition, Category> {

        public CategoryService(UnitOfWorker unitOfWorker) : base(unitOfWorker) { }

        protected override string DefaultColumnNamesOfQuery => "A.*";

        protected override string DefaultOrderBy => "DisplayOrder";

        public bool Create(CategoryEditor editor) {
            Category entity = Mapper.Map<Category>(editor);
            entity.DisplayOrder = UnitOfWorker.Categories.QueryCount() + 1;

            UnitOfWorker.Categories.Add(entity);

            bool isOk = UnitOfWorker.SaveChanges() == 1;

            if (isOk) {
                editor.SetId(entity.Id);
            }

            return isOk;
        }

        public bool Update(CategoryEditor editor) {
            Category entity = UnitOfWorker.Categories.QuerySingleOrDefault(new CategoryCondition {
                Id = editor.Id,
            });

            ExceptionUtils.ThrowIfItemNotFound(entity);

            Mapper.Map(editor, entity);

            UnitOfWorker.Categories.Update(entity);

            return UnitOfWorker.SaveChanges() == 1;
        }

        public bool Delete(long id) {
            using (DbTransaction tran = UnitOfWorker.DbContext.BeginTransaction()) {
                RecordCondition recordCondition = new RecordCondition {
                    CategoryId = id
                };

                if (UnitOfWorker.Records.QueryExists(recordCondition)) {
                    throw new ValidationException("Records 已存在相關資料。");
                }

                UnitOfWorker.Categories.Delete(id);

                if (UnitOfWorker.SaveChanges() != 1 || !RefreshDisplayOrder()) {
                    return false;
                }

                tran.Commit();
                return true;
            }
        }

        private bool RefreshDisplayOrder() {
            IEnumerable<Category> categories = UnitOfWorker.Categories.QueryList(orderBy: "DisplayOrder");

            if (categories.Count() == 0) {
                return true;
            }

            int i = 1;
            foreach (Category category in categories) {
                category.DisplayOrder = i++;

                UnitOfWorker.Categories.Update(category);
            }

            return UnitOfWorker.SaveChanges() > 0;
        }

        public bool MoveUp(long id) {
            Category entity = UnitOfWorker.Categories.QuerySingleOrDefault(new CategoryCondition {
                Id = id,
            });

            ExceptionUtils.ThrowIfItemNotFound(entity);

            if (entity.DisplayOrder == 1) {
                throw new ValidationException("已是第一筆資料。");
            }

            Category prevEntity = UnitOfWorker.Categories.QuerySingleOrDefault(new CategoryCondition {
                DisplayOrder = entity.DisplayOrder - 1
            });

            prevEntity.DisplayOrder = entity.DisplayOrder;
            entity.DisplayOrder -= 1;

            UnitOfWorker.Categories.Update(entity);
            UnitOfWorker.Categories.Update(prevEntity);

            return UnitOfWorker.SaveChanges() == 2;
        }

        public bool MoveDown(long id) {
            Category entity = UnitOfWorker.Categories.QuerySingleOrDefault(new CategoryCondition {
                Id = id,
            });

            ExceptionUtils.ThrowIfItemNotFound(entity);

            long max = UnitOfWorker.Categories.QueryCount();

            if (entity.DisplayOrder == max) {
                throw new ValidationException("已是最後一筆資料。");
            }

            Category nextEntity = UnitOfWorker.Categories.QuerySingleOrDefault(new CategoryCondition {
                DisplayOrder = entity.DisplayOrder + 1
            });

            nextEntity.DisplayOrder = entity.DisplayOrder;
            entity.DisplayOrder += 1;

            UnitOfWorker.Categories.Update(entity);
            UnitOfWorker.Categories.Update(nextEntity);

            return UnitOfWorker.SaveChanges() == 2;
        }

        protected override void CreateSqlInfoOfQuery(CategoryQueryCondition condition, string columnNames, out string resultSql, out DynamicParameters resultParameters) {
            resultParameters = new DynamicParameters();
            WhereClauseBuilder clauseBuilder = new WhereClauseBuilder();
            ConditionParser<CategoryQueryCondition> parser = new ConditionParser<CategoryQueryCondition>(condition, clauseBuilder, resultParameters);

            parser.ParseColumnToWhereInfo(x => x.Id, "A.Id");
            parser.ParseColumnToWhereInfo(x => x.Name, "A.Name");
            parser.ParseColumnToWhereInfo(x => x.DisplayOrder, "A.DisplayOrder");

            resultSql = $@"
                SELECT {columnNames}, B.Count RecordCount
                FROM Categories A WITH (NOLOCK)
                LEFT JOIN (
                    SELECT CategoryId, COUNT(1) Count
                    FROM Records WITH (NOLOCK)
                    GROUP BY CategoryId
                ) B ON A.Id = B.CategoryId
                {clauseBuilder.StartWhereSeparatorAnd}";
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
