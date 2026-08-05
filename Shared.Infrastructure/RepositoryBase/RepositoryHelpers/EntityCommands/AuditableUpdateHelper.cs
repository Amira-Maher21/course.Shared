using Shared.Application.API;
using Shared.Domain.Contracts.EntityCommonData;

namespace Shared.Infrastructure.RepositoryBase.RepositoryHelpers.EntityCommands
{
    internal class AuditableUpdateHelper
    {
        public void ForInsert(object entity, CommonUserData userData)
        {
            if (entity is IAuditable)
            {
                var auditableEntity = entity as IAuditable;
                if (auditableEntity != null)
                {
                    auditableEntity.In_User = userData.UserName;
                    auditableEntity.In_Date = DateTime.Now;

                    auditableEntity.Mod_User = null;
                    auditableEntity.Mod_Date = null;
                }

            }
        }

        public void ForUpdate(object entity, object originalEntity, CommonUserData userData)
        {
            if (entity is IAuditable)
            {
                var auditableEntity = entity as IAuditable;
                if (originalEntity is IAuditable)
                {
                    var originalAuditableEntity = originalEntity as IAuditable;
                    if (originalAuditableEntity != null)
                    {
                        if (auditableEntity != null)
                        {
                            auditableEntity.In_User = originalAuditableEntity.In_User;
                            auditableEntity.In_Date = originalAuditableEntity.In_Date;

                            auditableEntity.Mod_User = userData.UserName;
                            auditableEntity.Mod_Date = DateTime.Now;
                        }
                    }
                    else if (auditableEntity != null)
                    {
                        auditableEntity.Mod_User = userData.UserName;
                        auditableEntity.Mod_Date = DateTime.Now;
                    }
                }
            }
        }
    }
}
