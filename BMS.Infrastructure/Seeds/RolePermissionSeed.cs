//using Bms.Infrastructure.Common;
using BMS.Domain.Entities;
using BMS.Domain.Entities;
using BMS.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Bms.Infrastructure.Seeds
{
    public static class RolePermissionSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            var permissions = PermissionSeed.AllPermissions;
            var rolePermissions = new List<RolePermission>();

            foreach (var permission in permissions)
            {
                rolePermissions.Add(
                    new RolePermission(
                        DeterministicGuid.Create(RoleSeed.SuperAdminId, permission.Id),
                        RoleSeed.SuperAdminId,
                        permission.Id
                    )
                );
            }

            //foreach (var permission in permissions
            //             .Where(p => PermissionKeys.OperatorPermissions.Contains(p.Key)))
            //{
            //    rolePermissions.Add(
            //        new RolePermission(
            //            DeterministicGuid.Create(RoleSeed.OperatorId, permission.Id),
            //            RoleSeed.OperatorId,
            //            permission.Id
            //        )
            //    );
            //}

            //foreach (var permission in permissions
            //             .Where(p => PermissionKeys.ViewerPermissions.Contains(p.Key)))
            //{
            //    rolePermissions.Add(
            //        new RolePermission(
            //            DeterministicGuid.Create(RoleSeed.ViewerId, permission.Id),
            //            RoleSeed.ViewerId,
            //            permission.Id
            //        )
            //    );
            //}

            modelBuilder.Entity<RolePermission>().HasData(rolePermissions);
        }
    }
}
