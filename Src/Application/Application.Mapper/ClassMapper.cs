using Application.Dtos.Request;
using Application.Dtos.Responses;
using Domain.Entity;

namespace Application.Mapper
{
    public static class ClassMapper
    {
        public static ClassResponse ToClassResponse(this Class gymClass)
        {
            var enrolledUsers = gymClass.Schedules
          .SelectMany(s => s.Inscriptions)
          .Count(i => i.IsActive);

            return new ClassResponse
            {
                Id = gymClass.Id,
                Name = gymClass.Name,
                Max_Users = gymClass.Max_Users,
                EnrolledUsers = enrolledUsers,
                AvailableSpots = gymClass.Max_Users - enrolledUsers,
                IsFull = enrolledUsers >= gymClass.Max_Users,
                Schedules = gymClass.Schedules?
                .Select(s => s.ToScheduleResponse(gymClass.Max_Users))
                .ToList() ?? new()
            };
        }
        public static ClassDetailResponse ToClassDetailResponse(this Class gymClass, int currentInscriptions, List<ClientInfoResponse> clients)
        {
            return new ClassDetailResponse
            {
                Id = gymClass.Id,
                Name = gymClass.Name,
                Max_Users = gymClass.Max_Users,
                CurrentInscriptions = currentInscriptions,
                Schedules = gymClass.Schedules
                .Select(s => s.ToScheduleResponse(gymClass.Max_Users))
                .ToList(),
                Clients = clients
            };
        }

        public static Class ToClass(this CreateClassRequest request)
        {
            return new Class
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Max_Users = request.Max_Users
            };
        }
    }
}
