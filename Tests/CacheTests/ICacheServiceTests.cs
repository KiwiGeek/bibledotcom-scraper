using LibBibleDotCom.Interfaces;

namespace LibBibleDotCom.Tests.CacheTests;

[TestFixture(typeof(CacheServices.InMemoryCache))]
[TestFixture(typeof(CacheServices.SqliteCache))]
[TestFixture(typeof(CacheServices.FileSystemCache))]
public class ICacheServiceTests<T> where T : ICacheService, new()
{
    private ICacheService _cacheService;

    [SetUp]
    public void CreateCacheService()
    {
        _cacheService = new T();
    }

    [TearDown]
    public void ClearCache()
    {
        _cacheService.ClearAll();
    }

    [Test]
    public void IsCached_ReturnsTrue_WhenItemExistsInCache()
    {
        // Arrange
        ICacheService cache = _cacheService;
        cache.SetCache("key", "value", null);

        // Act
        bool result = cache.IsCached<string>("key");

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void IsCached_ReturnsFalse_WhenItemDoesNotExistInCache()
    {
        // Arrange
        ICacheService cache = _cacheService;

        // Act
        bool result = cache.IsCached<string>("key");

        // Assert
        Assert.That(result, Is.False);
    }


    [Test]
    public void IsCached_ThrowsException_WhenKeyIsNullOrEmpty()
    {
        // Arrange
        ICacheService cache = _cacheService;

        // Act & Assert
        Assert.Throws<Exception>(() => cache.IsCached<string?>(null!));
        Assert.Throws<Exception>(() => cache.IsCached<string>(string.Empty));
    }

    [Test]
    public void IsCached_ReturnsFalse_WhenItemIsExpired()
    {
        // Arrange
        ICacheService cache = _cacheService;
        cache.SetCache("key", "value", TimeSpan.FromMinutes(-10));

        // Act
        bool result = cache.IsCached<string>("key");

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void IsCached_ReturnsTrue_WhenItemIsNotExpired()
    {
        // Arrange
        ICacheService cache = _cacheService;
        cache.SetCache("key", "value", TimeSpan.FromMinutes(10));

        // Act
        bool result = cache.IsCached<string>("key");

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void IsCached_ReturnsFalse_WhenAnItemWithTheSameKeyExistsInTheCacheButADifferentType()
    {
        // Arrange
        ICacheService cache = _cacheService;
        cache.SetCache("key", "value", null);

        // Act
        bool result = cache.IsCached<int>("key");

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void GetAllCachedOfType_ReturnsAllItemsOfGivenType()
    {
        // Arrange
        ICacheService cache = _cacheService;
        cache.SetCache("key1", "value1", null);
        cache.SetCache("key2", "value2", null);

        // Act
        IEnumerable<string> result = cache.GetAllCachedOfType<string>();

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
    }

    [Test]
    public void GetAllCachedOfType_ReturnsEmptyList_WhenNoItemsOfGivenTypeExist()
    {
        // Arrange
        ICacheService cache = _cacheService;

        // Act
        IEnumerable<string> result = cache.GetAllCachedOfType<string>();

        // Assert
        Assert.That(result.Count(), Is.Zero);
    }

    [Test]
    public void GetAllCachedOfType_ReturnsEmptyList_WhenAllItemsOfGivenTypeAreExpired()
    {
        // Arrange
        ICacheService cache = _cacheService;
        cache.SetCache("key1", "value1", TimeSpan.FromMinutes(-10));
        cache.SetCache("key2", "value2", TimeSpan.FromMinutes(-5));

        // Act
        IEnumerable<string> result = cache.GetAllCachedOfType<string>();

        // Assert
        Assert.That(result.Count(), Is.Zero);
    }

    [Test]
    public void GetAllCachedOfType_ReturnsOnlyNonExpiredItemsOfGivenType()
    {
        // Arrange
        ICacheService cache = _cacheService;
        cache.SetCache("key1", "value1", TimeSpan.FromMinutes(-10));
        cache.SetCache("key2", "value2", TimeSpan.FromMinutes(10));

        // Act
        IEnumerable<string> result = cache.GetAllCachedOfType<string>();

        // Assert
        Assert.That(result.Count(), Is.EqualTo(1));
    }

    [Test]
    public void GetAllCachedOfType_ReturnsOnlyItemsOfGivenType()
    {
        // Arrange
        ICacheService cache = _cacheService;
        cache.SetCache("key1", "value1", null);
        cache.SetCache("key2", 1, null);

        // Act
        IEnumerable<string> result = cache.GetAllCachedOfType<string>();

        // Assert
        Assert.That(result.Count(), Is.EqualTo(1));
    }


    [Test]
    public void SetCache_ThrowsException_WhenItemWithSameKeyAndTypeAlreadyExists()
    {
        // Arrange
        ICacheService cache = _cacheService;
        cache.SetCache("key", "value", null);

        // Act & Assert
        Assert.Throws<Exception>(() => cache.SetCache("key", "value", null));
    }

    [Test]
    public void SetCache_DoesNotThrowException_WhenItemWithSameKeyButDifferentTypeAlreadyExists()
    {
        // Arrange
        ICacheService cache = _cacheService;

        // Act
        cache.SetCache("key", "value", null);

        //Assert
        Assert.DoesNotThrow(() => cache.SetCache("key", 1, null));
    }

    [Test]
    public void SetCache_DoesNotThrowException_WhenTwoItemsOfTheSameTypeButDifferentNamesAreCached()
    {
        // Arrange
        ICacheService cache = _cacheService;

        // Act
        cache.SetCache("key", "value", null);

        //Assert
        Assert.DoesNotThrow(() => cache.SetCache("key2", "value", null));
    }

    [Test]
    public void SetCache_AddsAnItemOfTypeToCache()
    {
        // Arrange
        ICacheService cache = _cacheService;

        // Act & Assert
        cache.SetCache("key", "value", null);
        int stringsInCache = cache.GetAllCachedOfType<string>().Count();
        Assert.That(stringsInCache, Is.EqualTo(1));
        cache.SetCache("key2", "value", null);
        stringsInCache = cache.GetAllCachedOfType<string>().Count();
        Assert.That(stringsInCache, Is.EqualTo(2));
    }

    [Test]
    public void SetCache_ThrowsException_WhenValueIsNull()
    {
        // Arrange
        ICacheService cache = _cacheService;

        // Act & Assert
        Assert.Throws<Exception>(() => cache.SetCache<string?>("key", null, null));
    }

    [Test]
    public void SetCache_ThrowsException_WhenKeyIsNullOrEmpty()
    {
        // Arrange
        ICacheService cache = _cacheService;

        // Act & Assert
        Assert.Throws<Exception>(() => cache.SetCache<string?>(null!, "value", null));
        Assert.Throws<Exception>(() => cache.SetCache(string.Empty, "value", null));
    }

    [Test]
    public void SetCache_ThrowsException_WhenLifespanIsZero()
    {
        // Arrange
        ICacheService cache = _cacheService;

        // Act & Assert
        Assert.Throws<Exception>(() => cache.SetCache("key", "value", TimeSpan.Zero));
    }

    [Test]
    public void SetCache_AddsAnItemWithGivenKeyAndTypeToCache()
    {
        // Arrange
        ICacheService cache = _cacheService;

        // Act & Assert
        cache.SetCache("key", "value", null);
        Assert.That(cache.IsCached<string>("key"), Is.True);
    }

    [Test]
    public void SetCache_AddsAnItemWithGivenKeyAndTypeToCacheAndItHasTheRightValue()
    {
        // Arrange
        ICacheService cache = _cacheService;

        // Act
        cache.SetCache("key", "value", null);
        cache.SetCache("key2", 1, null);
        IEnumerable<string> stringInCache = cache.GetAllCachedOfType<string>();
        IEnumerable<int> intInCache = cache.GetAllCachedOfType<int>();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(stringInCache.Single(), Is.EqualTo("value"));
            Assert.That(intInCache.Single(), Is.EqualTo(1));
        });
    }

    [Test]
    public void GetCached_ReturnsItemWithGivenKeyAndType()
    {
        // Arrange
        ICacheService cache = _cacheService;
        cache.SetCache("key", "value", null);

        // Act
        string result = cache.GetCached<string>("key");

        // Assert
        Assert.That(result, Is.EqualTo("value"));
    }

    [Test]
    public void TryGetCached_ReturnsTrueAndValue_WhenItemExistsInCache()
    {
        // Arrange
        ICacheService cache = _cacheService;
        cache.SetCache("key", "value", null);

        // Act
        bool success = cache.TryGetCached<string>("key", out string? result);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(success, Is.True);
            Assert.That(result, Is.EqualTo("value"));
        });
    }

    [Test]
    public void TryGetCached_ReturnsFalseAndDefault_WhenItemDoesNotExistInCache()
    {
        // Arrange
        ICacheService cache = _cacheService;

        // Act
        bool success = cache.TryGetCached<string>("key", out string? result);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(success, Is.False);
            Assert.That(result, Is.EqualTo(default(string)));
        });
    }

    [Test]
    public void RemoveCache_RemovesItemWithGivenKeyAndTypeFromCache()
    {
        // Arrange
        ICacheService cache = _cacheService;
        cache.SetCache("key", "value", null);

        // Act
        cache.RemoveCache<string>("key");

        // Assert
        Assert.That(cache.IsCached<string>("key"), Is.False);
    }

    [Test]
    public void ClearAllOfType_RemovesAllItemsWithGivenTypeFromCache()
    {
        // Arrange
        ICacheService cache = _cacheService;
        cache.SetCache("key1", "value1", null);
        cache.SetCache("key2", "value2", null);

        // Act
        cache.ClearAllOfType<string>();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(cache.IsCached<string>("key1"), Is.False);
            Assert.That(cache.IsCached<string>("key2"), Is.False);
        });
    }

    [Test]
    public void ClearAll_RemovesAllItemsFromCache()
    {
        // Arrange
        ICacheService cache = _cacheService;
        cache.SetCache("key1", "value1", null);
        cache.SetCache("key2", "value2", null);

        // Act
        cache.ClearAll();

        // Assert
        Assert.That(cache.Count(), Is.Zero);
    }

    [Test]
    public void UpdateCache_RemovesItemWithGivenKeyAndTypeAndSetsNewValue()
    {
        // Arrange
        ICacheService cache = _cacheService;
        cache.SetCache("key", "value1", null);

        // Act
        cache.UpdateCache("key", "value2", null);

        // Assert
        Assert.That(cache.GetCached<string>("key"), Is.EqualTo("value2"));
    }

    [Test]
    public void Count_ReturnsNumberOfItemsInCache()
    {
        // Arrange
        ICacheService cache = _cacheService;
        cache.SetCache("key1", "value1", null);
        cache.SetCache("key2", "value2", null);

        // Act
        uint result = cache.Count();

        // Assert
        Assert.That(result, Is.EqualTo(2));
    }

    [Test]
    public void PurgeExpired_RemovesExpiredItemsFromCache()
    {
        // Arrange
        ICacheService cache = _cacheService;
        cache.SetCache("key1", "value1", TimeSpan.FromMinutes(-10));
        cache.SetCache("key2", "value2", TimeSpan.FromMinutes(-5));
        cache.SetCache("key3", "value3", TimeSpan.FromMinutes(10));

        // Act
        cache.PurgeExpired();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(cache.IsCached<string>("key1"), Is.False);
            Assert.That(cache.IsCached<string>("key2"), Is.False);
            Assert.That(cache.IsCached<string>("key3"), Is.True);
        });
    }
}
