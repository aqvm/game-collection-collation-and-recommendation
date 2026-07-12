using System.Runtime.InteropServices;
using System.Text.Json;
using GameLibrary.Application;

namespace GameLibrary.Infrastructure;

public sealed class RedactedFileLogger(string directory) : IStructuredLogger
{
    private static readonly HashSet<string> SafeFields = new(StringComparer.Ordinal) { "phase", "count", "durationMs", "outcome", "retryCategory", "correlationId" };
    private readonly string _path = Path.Combine(directory, "game-library.ndjson");

    public void Write(string eventName, IReadOnlyDictionary<string, object?> fields)
    {
        Directory.CreateDirectory(directory);
        var safe = fields.Where(field => SafeFields.Contains(field.Key)).ToDictionary(field => field.Key, field => field.Value);
        var line = JsonSerializer.Serialize(new { timestamp = DateTimeOffset.UtcNow, eventName, fields = safe });
        File.AppendAllText(_path, line + Environment.NewLine);
    }
}

public sealed class WindowsCredentialVault : ISecretVault
{
    public Task StoreAsync(string reference, string secret, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!OperatingSystem.IsWindows()) throw new PlatformNotSupportedException("Windows Credential Manager is required on this target.");
        var bytes = System.Text.Encoding.Unicode.GetBytes(secret);
        var credential = new NativeCredential { Type = 1, TargetName = reference, CredentialBlobSize = (uint)bytes.Length, Persist = 2, UserName = Environment.UserName };
        credential.CredentialBlob = Marshal.AllocCoTaskMem(bytes.Length);
        try { Marshal.Copy(bytes, 0, credential.CredentialBlob, bytes.Length); if (!CredWrite(ref credential, 0)) throw new InvalidOperationException("Unable to store secret in Windows Credential Manager."); }
        finally { Marshal.FreeCoTaskMem(credential.CredentialBlob); }
        return Task.CompletedTask;
    }

    public Task<string?> GetAsync(string reference, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!CredRead(reference, 1, 0, out var pointer)) return Task.FromResult<string?>(null);
        try { var credential = Marshal.PtrToStructure<NativeCredential>(pointer); var value = credential.CredentialBlobSize == 0 ? string.Empty : Marshal.PtrToStringUni(credential.CredentialBlob, (int)credential.CredentialBlobSize / 2); return Task.FromResult<string?>(value); }
        finally { CredFree(pointer); }
    }

    public Task RemoveAsync(string reference, CancellationToken cancellationToken)
    { cancellationToken.ThrowIfCancellationRequested(); if (!CredDelete(reference, 1, 0)) { var error = Marshal.GetLastWin32Error(); if (error != 1168) throw new InvalidOperationException("Unable to remove secret from Windows Credential Manager."); } return Task.CompletedTask; }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)] private struct NativeCredential { public uint Flags; public uint Type; public string TargetName; public string Comment; public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten; public uint CredentialBlobSize; public IntPtr CredentialBlob; public uint Persist; public uint AttributeCount; public IntPtr Attributes; public string TargetAlias; public string UserName; }
    [DllImport("Advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)] private static extern bool CredWrite([In] ref NativeCredential credential, uint flags);
    [DllImport("Advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)] private static extern bool CredRead(string target, uint type, uint flags, out IntPtr credential);
    [DllImport("Advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)] private static extern bool CredDelete(string target, uint type, uint flags);
    [DllImport("Advapi32.dll", SetLastError = true)] private static extern void CredFree(IntPtr credential);
}
