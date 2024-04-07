using CloudyWing.MoneyTrack.Infrastructure.Util;
using CloudyWing.MoneyTrack.Models.DataAccess;
using CloudyWing.MoneyTrack.Models.DataAccess.Entities;
using Dapper;
using FluentValidation;

namespace CloudyWing.MoneyTrack.Models.Domain.CategoryModels {
    public class RecordService : QueryableService<RecordQueryCondition, RecordQueryRecord> {
        public const string RecordAlias = "R";

        public const string CategoryAlias = "C";

        public RecordService(UnitOfWorker unitOfWorker) : base(unitOfWorker) { }

        protected override string DefaultColumnNamesOfQuery => $"{RecordAlias}.*, {CategoryAlias}.Name CategoryName";

        protected override string DefaultOrderBy => $"{RecordAlias}.RecordDate DESC, {RecordAlias}.Id";

        public bool Create(RecordEditor editor) {
            ExceptionUtils.ThrowIfNull(() => editor);
            ExceptionUtils.ThrowIfInvalid(new CreateValidator(), () => editor);

            Record entity = Mapper.Map<Record>(editor);

            UnitOfWorker.Records.Add(entity);

            bool isOk = UnitOfWorker.SaveChanges() == 1;

            if (isOk) {
                editor.SetId(entity.Id);
            }

            return isOk;
        }

        public bool Update(RecordEditor editor) {
            ExceptionUtils.ThrowIfNull(() => editor);
            ExceptionUtils.ThrowIfInvalid(new UpdateValidator(), () => editor);

            Record entity = UnitOfWorker.Records.QuerySingleOrDefault(new RecordCondition {
                Id = editor.Id,
            });

            ExceptionUtils.ThrowIfItemNotFound(entity);

            Mapper.Map(editor, entity);

            UnitOfWorker.Records.Update(entity);

            return UnitOfWorker.SaveChanges() == 1;
        }

        public bool Delete(long id) {
            UnitOfWorker.Records.Delete(id);

            return UnitOfWorker.SaveChanges() == 1;
        }

        protected override void CreateSqlInfoOfQuery(RecordQueryCondition condition, string columnNames, out string resultSql, out DynamicParameters resultParameters) {
            resultParameters = new DynamicParameters();
            WhereClauseBuilder clauseBuilder = new WhereClauseBuilder();
            ConditionParser<RecordQueryCondition> parser = new ConditionParser<RecordQueryCondition>(condition, clauseBuilder, resultParameters);

            parser.ParseColumnToWhereInfo(x => x.Id, $"{RecordAlias}.Id");
            parser.ParseColumnToWhereInfo(x => x.CategoryId, $"{RecordAlias}.CategoryId");
            parser.ParseColumnToWhereInfo(x => x.RecordDate, $"{RecordAlias}.RecordDate");
            parser.ParseColumnToWhereInfo(x => x.IsIncome, $"{RecordAlias}.IsIncome");
            parser.ParseColumnToWhereInfo(x => x.Amount, $"{RecordAlias}.Amount");
            parser.ParseColumnToWhereInfo(x => x.Description, $"{RecordAlias}.Description");

            resultSql = $@"
                SELECT {columnNames}
                FROM Records {RecordAlias} WITH (NOLOCK)
                INNER JOIN Categories {CategoryAlias} WITH (NOLOCK)
                ON {RecordAlias}.CategoryId = {CategoryAlias}.Id
                {clauseBuilder.StartWhereSeparatorAnd}";
        }

        private class CreateValidator : AbstractValidator<RecordEditor> {
            public CreateValidator() {
                RuleFor(x => x.CategoryId).Must(x => x.HasValue);
                RuleFor(x => x.RecordDate).Must(x => x.HasValue);
                RuleFor(x => x.IsIncome).Must(x => x.HasValue);
                RuleFor(x => x.Amount).Must(x => x.HasValue && x.Value > 0);
            }
        }

        private class UpdateValidator : AbstractValidator<RecordEditor> {
            public UpdateValidator() {
                RuleFor(x => x.Id).NotNull();
                RuleFor(x => x.Amount).Must(x => !x.HasValue || x.Value > 0);
            }
        }
    }
}
