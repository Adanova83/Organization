using Ct = Library.Contract;
using Do = Library.Domain;
using En = Library.Entity;

namespace Library.Mapper;

public static class ActivitySession
{
    public static Ct.ActivitySession DomainToContract(this Do.ActivitySession source)
    {
        return new Ct.ActivitySession
        {
            Id = source.Id,
            ActivityId = source.ActivityId,
            SchoolYearId = source.SchoolYearId,
            Price = source.Price,
            StartDate = source.StartDate,
            EndDate = source.EndDate,
            DuringTermTime = source.DuringTermTime,
            DuringSchoolHolidays = source.DuringSchoolHolidays,
            SchoolYearCode = source.SchoolYearCode,
            EffectiveStartDate = source.EffectiveStartDate,
            EffectiveEndDate = source.EffectiveEndDate,
            Active = source.Active.DomainToContract()
        };
    }

    internal static En.ActivitySession DomainToEntity(this Do.ActivitySession source)
    {
        return new En.ActivitySession
        {
            Id = source.Id,
            ActivityId = source.ActivityId,
            SchoolYearId = source.SchoolYearId,
            Price = source.Price,
            StartDate = source.StartDate,
            EndDate = source.EndDate,
            DuringTermTime = source.DuringTermTime,
            DuringSchoolHolidays = source.DuringSchoolHolidays,
            Active = source.Active.DomainToEntity()
        };
    }

    /// <summary>Entité vers domaine ; les dates effectives sont complétées par l'année scolaire si elle a été chargée.</summary>
    internal static Do.ActivitySession EntityToDomain(this En.ActivitySession source)
    {
        return new Do.ActivitySession
        {
            Id = source.Id,
            ActivityId = source.ActivityId,
            SchoolYearId = source.SchoolYearId,
            Price = source.Price,
            StartDate = source.StartDate,
            EndDate = source.EndDate,
            DuringTermTime = source.DuringTermTime,
            DuringSchoolHolidays = source.DuringSchoolHolidays,
            SchoolYearCode = source.SchoolYear?.Code,
            EffectiveStartDate = source.StartDate ?? source.SchoolYear?.StartDate,
            EffectiveEndDate = source.EndDate ?? source.SchoolYear?.EndDate,
            Active = source.Active.EntityToDomain(),
            Audit = source.Audit.EntityToDomain()
        };
    }

    /// <summary>Modèle reçu de l'API vers le domaine. L'audit est géré par la base, jamais fourni par le client.</summary>
    public static Do.ActivitySession ContractToDomain(this Ct.ActivitySession source, Guid id)
    {
        return new Do.ActivitySession
        {
            Id = id,
            ActivityId = source.ActivityId,
            SchoolYearId = source.SchoolYearId,
            Price = source.Price,
            StartDate = source.StartDate,
            EndDate = source.EndDate,
            DuringTermTime = source.DuringTermTime,
            DuringSchoolHolidays = source.DuringSchoolHolidays,
            Active = source.Active.ContractToDomain() ?? new Do.Active(),
            Audit = new Do.Audit()
        };
    }
}
