using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace UnitTests;

/// <summary>
/// Test helpers for session-based testing
/// </summary>
public sealed class SessionFeature : ISessionFeature 
{ 
    public ISession Session { get; set; } = null!; 
}

/// <summary>
/// In-memory session implementation for testing
/// </summary>
public sealed class TestSession : ISession
{
    private readonly Dictionary<string, byte[]> _storage = new();
    
    public IEnumerable<string> Keys => _storage.Keys;
    public string Id { get; } = Guid.NewGuid().ToString();
    public bool IsAvailable => true;
    
    public void Clear() => _storage.Clear();
    public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public void Remove(string key) => _storage.Remove(key);
    public void Set(string key, byte[] value) => _storage[key] = value;
    public bool TryGetValue(string key, out byte[] value) => _storage.TryGetValue(key, out value!);
    
    public void SetString(string key, string value) 
        => _storage[key] = System.Text.Encoding.UTF8.GetBytes(value);
}
