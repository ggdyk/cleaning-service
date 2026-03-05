namespace Application.DTOs.Admin;

public record PopularServiceDto(
    int ServiceId,
    string ServiceNameRu,
    string ServiceNameKk,
    string ServiceNameEn,
    int OrderCount
);
