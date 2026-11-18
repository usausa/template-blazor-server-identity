namespace Template.BlazorServer.Application.Identity;

using Microsoft.AspNetCore.Identity;

using Template.BlazorServer.Host.Application.Identity;

public sealed class PasskeyAuthenticatorsTests
{
    private static readonly Guid WindowsHello = new("08987058-cadc-4b81-b6e1-30de50dcbe96");

    [Fact]
    public void KnownAuthenticatorHasDefaultName()
    {
        // Arrange
        var passkey = CreatePasskey(null, WindowsHello);

        // Act
        var found = PasskeyAuthenticators.TryGetDefaultDisplayName(passkey, out var name);

        // Assert
        Assert.True(found);
        Assert.Equal("Windows Hello", name);
    }

    [Fact]
    public void UnknownAuthenticatorHasNoDefaultName()
    {
        // Arrange
        var passkey = CreatePasskey(null, Guid.Empty);

        // Act
        var found = PasskeyAuthenticators.TryGetDefaultDisplayName(passkey, out var name);

        // Assert
        Assert.False(found);
        Assert.Null(name);
    }

    [Fact]
    public void DisplayNamePrefersSavedName()
    {
        // Arrange
        var passkey = CreatePasskey("My key", WindowsHello);

        // Act
        var name = PasskeyAuthenticators.GetDisplayName(passkey);

        // Assert
        Assert.Equal("My key", name);
    }

    [Fact]
    public void DisplayNameFallsBackToAuthenticatorName()
    {
        // Arrange
        var passkey = CreatePasskey(null, WindowsHello);

        // Act
        var name = PasskeyAuthenticators.GetDisplayName(passkey);

        // Assert
        Assert.Equal("Windows Hello", name);
    }

    [Fact]
    public void DisplayNameWithoutAuthenticatorIsUnnamed()
    {
        // Arrange
        var passkey = CreatePasskey(null, null);

        // Act
        var name = PasskeyAuthenticators.GetDisplayName(passkey);

        // Assert
        Assert.Equal("名前未設定のパスキー", name);
    }

    private static UserPasskeyInfo CreatePasskey(string? name, Guid? aaguid) =>
        new([1], [2], DateTimeOffset.UnixEpoch, 0, null, true, false, false, [3], [4])
        {
            Name = name,
            Aaguid = aaguid?.ToByteArray(bigEndian: true)
        };
}
