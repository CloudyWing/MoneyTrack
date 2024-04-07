using CloudyWing.MoneyTrack.Infrastructure.Util;
using CloudyWing.MoneyTrack.Models.DataAccess;
using CloudyWing.MoneyTrack.Models.DataAccess.Entities;
using FluentValidation;

namespace CloudyWing.MoneyTrack.Models.Domain.RecordModels {
    public class RecordService(UnitOfWorker unitOfWorker, IServiceProvider? serviceProvider)
        : QueryableService<RecordQueryEntity>(unitOfWorker, serviceProvider) {
        public override string SqlBody {
            get {
                return @"
                    FROM Records Record
                    INNER JOIN Categories Category ON Record.CategoryId = Category.Id
                ";
            }
        }

        public async Task<bool> CreateAsync(RecordEditor editor) {
            ExceptionUtils.ThrowIfNull(() => editor);
            ExceptionUtils.ThrowIfInvalid(new CreateValidator(), () => editor);

            Record entity = Mapper.Map<Record>(editor);

            UnitOfWorker.Records.Add(entity);

            bool isOk = await UnitOfWorker.SaveChangesAsync() == 1;

            if (isOk) {
                editor.SetId(entity.Id);
            }

            return isOk;
        }

        public async Task<bool> UpdateAsync(RecordEditor editor) {
            ExceptionUtils.ThrowIfNull(() => editor);
            ExceptionUtils.ThrowIfInvalid(new UpdateValidator(), () => editor);

            Record? entity = await UnitOfWorker.Records.QuerySingleOrDefaultAsync(new RecordCondition {
                Id = editor.Id,
            });

            ExceptionUtils.ThrowIfItemNotFound(entity);

            Mapper.Map(editor, entity);

            UnitOfWorker.Records.Update(entity);

            return await UnitOfWorker.SaveChangesAsync() == 1;
        }

        public async Task<bool> DeleteAsync(long id) {
            UnitOfWorker.Records.Delete(id);

            return await UnitOfWorker.SaveChangesAsync() == 1;
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
