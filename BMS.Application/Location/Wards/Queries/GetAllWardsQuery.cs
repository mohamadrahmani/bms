using BMS.Application.Location.Wards.Dtos;
using MediatR;

namespace BMS.Application.Location.Wards.Queries;

public class GetAllWardsQuery : IRequest<List<WardDto>>
{
}
