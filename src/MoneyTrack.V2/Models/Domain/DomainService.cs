using System;
using AutoMapper;
using CloudyWing.MoneyTrack.Infrastructure.Util;
using CloudyWing.MoneyTrack.Models.DataAccess;

namespace CloudyWing.MoneyTrack.Models.Domain {
    public abstract class DomainService {
        private static readonly Lazy<IMapper> mapper = new Lazy<IMapper>(() => {
            MapperConfiguration config = new MapperConfiguration(cfg => {
                cfg.AddProfile(new DomainServiceProfile());
            });
            config.AssertConfigurationIsValid();
            return config.CreateMapper();
        });

        protected DomainService(UnitOfWorker unitOfWorker) {
            ExceptionUtils.ThrowIfNull(() => unitOfWorker);

            UnitOfWorker = unitOfWorker;
        }

        protected IMapper Mapper => mapper.Value;

        protected UnitOfWorker UnitOfWorker { get; }

        protected virtual IConfigurationProvider CreateMapperConfig() {
            return new MapperConfiguration(cfg => { });
        }
    }
}
