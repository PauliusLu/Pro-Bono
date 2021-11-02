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

        public virtual async Task<IdentityResult> AddUserRoleWithCharityId(User user, string roleId, int charityId)
        {
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(roleId))
                throw new ArgumentNullException(nameof(roleId));

            var role = _store.Context.Find<IdentityRole>(roleId);
            var normalizedRoleName = role.NormalizedName;

            if (await _store.IsInRoleAsync(user, normalizedRoleName))
                return IdentityResult.Failed(ErrorDescriber.UserAlreadyInRole(roleId));

            _store.Context.Set<IdentityUserRole<string>>().Add(new UserRole { RoleId = roleId, UserId = user.Id, CharityId = charityId });

            return await UpdateUserAsync(user);
        }
    }
}
