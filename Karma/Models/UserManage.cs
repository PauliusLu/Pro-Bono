using Karma.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Karma.Models
{
    public class UserManage : UserManager<User>
    {
        private readonly UserStore<User, IdentityRole, KarmaContext, string, IdentityUserClaim<string>,
        IdentityUserRole<string>, IdentityUserLogin<string>, IdentityUserToken<string>, IdentityRoleClaim<string>> _store;

        public UserManage(
            IUserStore<User> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<User> passwordHasher,
            IEnumerable<IUserValidator<User>> userValidators,
            IEnumerable<IPasswordValidator<User>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<User>> logger)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _store = (UserStore<User, IdentityRole, KarmaContext, string, IdentityUserClaim<string>,
            IdentityUserRole<string>, IdentityUserLogin<string>, IdentityUserToken<string>, IdentityRoleClaim<string>>) store;
        }

        public virtual async Task<IdentityResult> AddUserRoleWithCharityId(User user, string roleName, int charityId)
        {
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentNullException(nameof(roleName));

            var normalizedRoleName = roleName.ToUpper();
            var role = _store.Context.Role
                .Where(r => r.NormalizedName == normalizedRoleName)
                .FirstOrDefault();

            if (await _store.IsInRoleAsync(user, normalizedRoleName))
                return IdentityResult.Failed(ErrorDescriber.UserAlreadyInRole(roleName));

            _store.Context.Set<IdentityUserRole<string>>().Add(new UserRole { RoleId = role.Id, UserId = user.Id, CharityId = charityId });

            return await UpdateUserAsync(user);
        }

        public virtual User GetUserByCharityId(string roleName, int charityId)
        {
            if (charityId == 0)
                throw new ArgumentOutOfRangeException(nameof(charityId));

            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentNullException(nameof(roleName));

            var normalizedRoleName = roleName.ToUpper();
            var role = _store.Context.Role
                .Where(r => r.NormalizedName == normalizedRoleName)
                .FirstOrDefault();

            var userRole = _store.Context.UserRole
                .Where(r => r.RoleId == role.Id && r.CharityId == charityId)
                .FirstOrDefault();

            if (userRole == null)
                return null;

            var userId = userRole.UserId;
            var user = _store.Context.Find<User>(userId);

            return user;
        }

        public virtual Charity GetCharityByUserRole(string roleName, string userId)
        {
            var normalizedRoleName = roleName.ToUpper();
            var role = _store.Context.Role
                .Where(r => r.NormalizedName == normalizedRoleName)
                .FirstOrDefault();

            var userRole = _store.Context.UserRole
                .Where(r => r.RoleId == role.Id && r.UserId == userId)
                .FirstOrDefault();

            var charityId = userRole.CharityId;
            var charity = _store.Context.Charity.FindAsync(charityId).Result;

            return charity;
        }
    }
}
