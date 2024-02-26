using Microsoft.Extensions.Caching.Memory;
using Oqtane.Extensions;
using Oqtane.Models;
using Oqtane.Shared;

namespace Oqtane.Infrastructure.EventSubscribers
{
    public class CacheInvalidationEventSubscriber : IEventSubscriber
    {
        private readonly IMemoryCache _cache;

        public CacheInvalidationEventSubscriber(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void EntityChanged(SyncEvent syncEvent)
        {
            // when site entities change (ie. site, pages, modules, etc...) a site refresh event is raised and the site cache item needs to be refreshed
            if (syncEvent.EntityName == EntityNames.Site && syncEvent.Action == SyncEventActions.Refresh)
            {
                _cache.Remove($"site:{syncEvent.TenantId}:{syncEvent.EntityId}*", true);
            }
            // when user is modified (ie. roles) a a site reload event is raised and the site cache item for the user needs to be refreshed
            if (syncEvent.EntityName == EntityNames.User && syncEvent.Action == SyncEventActions.Reload)
            {
                _cache.Remove($"site:{syncEvent.TenantId}:{syncEvent.SiteId}:{syncEvent.EntityId}", true);
            }

            // when a site entity is updated, the hosting model may have changed so the client assemblies cache items need to be refreshed
            if (syncEvent.EntityName == EntityNames.Site && syncEvent.Action == SyncEventActions.Update)
            {
                _cache.Remove($"assemblieslist:{syncEvent.TenantId}:{syncEvent.EntityId}");
                _cache.Remove($"assemblies:{syncEvent.TenantId}:{syncEvent.EntityId}");
            }
        }
    }
}
