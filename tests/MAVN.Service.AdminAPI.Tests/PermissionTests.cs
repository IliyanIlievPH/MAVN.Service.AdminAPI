using System;
using System.Collections.Generic;
using System.Linq;
using MAVN.Service.AdminAPI.Domain.Enums;
using MAVN.Service.AdminAPI.Models.Admins;
using Xunit;

namespace MAVN.Service.AdminAPI.Tests
{
    public class PermissionTests
    {
        private readonly HashSet<string> _expectedPermissionLevels = new HashSet<string>
        {
            nameof(PermissionLevel.View),
            nameof(PermissionLevel.Edit),
            nameof(PermissionLevel.PartnerEdit),
        };

        /// <summary>
        ///     Ensures each permission level is unique and it's value has not changed.
        ///     Verifies that newly added permission levels has test cases.
        ///     Verifies if levels were removed their test cases is removed too.
        ///     If for some reasons you have modified permission levels contract,
        ///     please fix unit test cases too. This is needed to make sure you have changed permission levels knowingly.
        /// </summary>
        [Fact]
        public void PermissionLevels_WasNotModifiedAccidentally()
        {
            var currentPermissionLevels = Enum.GetNames(typeof(PermissionLevel)).ToList();

            foreach (var expectedLevel in _expectedPermissionLevels)
            {
                Assert.True(currentPermissionLevels.Contains(expectedLevel),
                    $"Permission Level: \"{expectedLevel}\" was removed! But it still have a test. If you removed it knowingly please remove it from {nameof(_expectedPermissionLevels)}.");
            }

            if (currentPermissionLevels.Count > _expectedPermissionLevels.Count)
            {
                var addedPermissionLevels = currentPermissionLevels.Except(_expectedPermissionLevels);

                foreach (var addedPermissionLevel in addedPermissionLevels)
                    Assert.True(false,
                        $"Level: \"{addedPermissionLevel}\" was added, but don't have a test. Please add it to {nameof(_expectedPermissionLevels)}.");
            }
        }
        
        private readonly HashSet<string> _expectedApiPermissionTypes = new HashSet<string>
        {
            nameof(PermissionType.AdminUsers),
            nameof(PermissionType.Settings),
            nameof(PermissionType.Customers),
            nameof(PermissionType.Dashboard),
            nameof(PermissionType.ActionRules),
            nameof(PermissionType.VoucherManager),
            nameof(PermissionType.Reports),
            nameof(PermissionType.ProgramPartners),
            nameof(PermissionType.BlockchainOperations),
            nameof(PermissionType.AuditLogs),
        };
        
        /// <summary>
        ///     Ensures each permission type is unique and it's value has not changed.
        ///     Verifies that newly added permission type has test cases.
        ///     Verifies if types were removed their test cases is removed too.
        ///     If for some reasons you have modified permission types contract,
        ///     please fix unit test cases too. This is needed to make sure you have changed permission types knowingly.
        /// </summary>
        [Fact]
        public void ApiPermissionTypes_WasNotModifiedAccidentally()
        {
            var currentPermissionTypes = Enum.GetNames(typeof(PermissionType)).ToList();

            foreach (var expectedType in _expectedApiPermissionTypes)
            {
                Assert.True(currentPermissionTypes.Contains(expectedType),
                    $"Permission Type: \"{expectedType}\" was removed! But it still have a test. If you removed it knowingly please remove it from {nameof(_expectedApiPermissionTypes)}.");
            }

            if (currentPermissionTypes.Count > _expectedApiPermissionTypes.Count)
            {
                var addedPermissionTypes = currentPermissionTypes.Except(_expectedApiPermissionTypes);

                foreach (var addedPermissionType in addedPermissionTypes)
                    Assert.True(false,
                        $"Type: \"{addedPermissionType}\" was added, but don't have a test. Please add it to {nameof(_expectedApiPermissionTypes)}.");
            }
        }
    }
}
