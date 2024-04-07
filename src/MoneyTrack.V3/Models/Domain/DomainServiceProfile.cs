using System.Net;
using AutoMapper;
using CloudyWing.Enumeration.Abstractions;
using CloudyWing.MoneyTrack.Models.DataAccess.Entities;
using CloudyWing.MoneyTrack.Models.Domain.CategoryModels;
using CloudyWing.MoneyTrack.Models.Domain.RecordModels;

namespace CloudyWing.MoneyTrack.Models.Domain {
    public class DomainServiceProfile : Profile {
        public DomainServiceProfile() {
            CreateMap<ValueWatcher<IConvertible>, IConvertible>()
                 .ConvertUsing<OptionalPropertyConverter<IConvertible>>();

            CreateMap<string?, string?>()
                .ConvertUsing(x => x == null ? x : x.Trim());

            CreateMap<ValueWatcher<string?>, string?>()
                .ConvertUsing<OptionalStringConverter>();

            CreateMap<ValueWatcher<byte[]>, byte[]>()
                .ConvertUsing<OptionalPropertyConverter<byte[]>>();

            CreateMap<ValueWatcher<Guid>, Guid>()
                .ConvertUsing<OptionalPropertyConverter<Guid>>();

            CreateMap<ValueWatcher<Guid?>, Guid?>()
                .ConvertUsing<OptionalPropertyConverter<Guid?>>();

            CreateMap<ValueWatcher<bool?>, bool?>()
                .ConvertUsing<OptionalPropertyConverter<bool?>>();

            CreateMap<ValueWatcher<byte?>, byte?>()
                .ConvertUsing<OptionalPropertyConverter<byte?>>();

            CreateMap<ValueWatcher<short?>, short?>()
                .ConvertUsing<OptionalPropertyConverter<short?>>();

            CreateMap<ValueWatcher<int?>, int?>()
                .ConvertUsing<OptionalPropertyConverter<int?>>();

            CreateMap<ValueWatcher<long?>, long?>()
                .ConvertUsing<OptionalPropertyConverter<long?>>();

            CreateMap<ValueWatcher<decimal?>, decimal?>()
                .ConvertUsing<OptionalPropertyConverter<decimal?>>();

            CreateMap<ValueWatcher<float?>, float?>()
                .ConvertUsing<OptionalPropertyConverter<float?>>();

            CreateMap<ValueWatcher<DateTime?>, DateTime?>()
                .ConvertUsing<OptionalPropertyConverter<DateTime?>>();

            CreateMap<HttpMethod, string>()
                .ConvertUsing(x => x.Method);

            CreateMap<HttpStatusCode, short>()
                .ConvertUsing(x => (short)(int)x);

            CreateRecordMap();
            CreateCategoryMap();
        }

        private void CreateRecordMap() {
            CreateMap<RecordEditor, Record>()
                .ForMember(desc => desc.Id, opt => opt.Ignore());
        }

        private void CreateCategoryMap() {
            CreateMap<CategoryEditor, Category>()
                .ForMember(desc => desc.Id, opt => opt.Ignore())
                .ForMember(desc => desc.DisplayOrder, opt => opt.Ignore());
        }

        private class OptionalPropertyConverter<T> : ITypeConverter<ValueWatcher<T>, T> {
            public T Convert(ValueWatcher<T> source, T destination, ResolutionContext context) {
                if (source.HasValue) {
                    destination = source.Value;
                }
                return destination;
            }
        }

        private class OptionalStringConverter : ITypeConverter<ValueWatcher<string?>, string?> {
            public string? Convert(ValueWatcher<string?> source, string? destination, ResolutionContext context) {
                if (source.HasValue) {
                    destination = source.Value?.Trim();
                }
                return destination;
            }
        }

        private class OptionalConstantPropertyConverter<TEnum, TValue> : ITypeConverter<ValueWatcher<TEnum>, TValue>
            where TEnum : EnumerationBase<TEnum, TValue>
            where TValue : IComparable, IConvertible {
            public TValue Convert(ValueWatcher<TEnum> source, TValue destination, ResolutionContext context) {
                if (source.HasValue) {
                    destination = source.Value;
                }
                return destination;
            }
        }
    }
}
