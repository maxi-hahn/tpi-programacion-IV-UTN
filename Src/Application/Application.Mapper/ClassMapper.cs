using Application.Dtos.Request;
using Application.Dtos.Responses;
using Domain.Entity;

namespace Application.Mapper
{
    public static class ClassMapper
    {
        public static ClassResponse ToClassResponse(this Class gymClass, Guid? userId = null)
        {
            // Calcular próxima fecha para cada horario y contar cupos correctamente
            var totalEnrolledUsers = 0;
            var totalAvailableSpots = 0;
            var hasAnyAvailable = false;

            var schedules = gymClass.Schedules?
                .Select(s => {
                    var scheduleResponse = s.ToScheduleResponse(gymClass.Max_Users, userId);

                    // Acumular para el resumen de la clase
                    totalEnrolledUsers += scheduleResponse.EnrolledUsers;
                    totalAvailableSpots += scheduleResponse.AvailableSpots;

                    if (!scheduleResponse.IsFull && scheduleResponse.IsActive)
                    {
                        hasAnyAvailable = true;
                    }

                    return scheduleResponse;
                })
                .ToList() ?? new();

            var isFull = schedules.Count > 0 && !hasAnyAvailable;

            return new ClassResponse
            {
                Id = gymClass.Id,
                Name = gymClass.Name,
                Max_Users = gymClass.Max_Users,
                EnrolledUsers = totalEnrolledUsers,
                AvailableSpots = totalAvailableSpots,
                IsFull = isFull,
                IsActive = gymClass.IsActive,
                Schedules = schedules
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