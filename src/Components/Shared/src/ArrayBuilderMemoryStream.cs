// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;

#if BLAZOR_WEBVIEW
namespace Microsoft.AspNetCore.Components.WebView;
#else
namespace Microsoft.AspNetCore.Components.Server.Circuits;
#endif

/// <summary>
/// Writeable memory stream backed by a an <see cref="ArrayBuilder{T}"/>.
/// </summary>
internal sealed class ArrayBuilderMemoryStream : Stream
{
    public ArrayBuilderMemoryStream(ArrayBuilder<byte> arrayBuilder)
    {
        ArrayBuilder = arrayBuilder;
    }

    /// <inheritdoc />
    public override bool CanRead => false;

    /// <inheritdoc />
    public override bool CanSeek => false;

    /// <inheritdoc />
    public override bool CanWrite => true;

    /// <inheritdoc />
    public override long Length => ArrayBuilder.Count;

    /// <inheritdoc />
    public override long Position
    {
        get => ArrayBuilder.Count;
        set => throw new NotSupportedException();
    }

    public ArrayBuilder<byte> ArrayBuilder { get; }

    /// <inheritdoc />
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

    /// <inheritdoc />
    public override int Read(byte[] buffer, int offset, int count)
        => throw new NotSupportedException();

    /// <inheritdoc />
    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        => throw new NotSupportedException();

    /// <inheritdoc />
    public override void Write(byte[] buffer, int offset, int count)
    {
        ValidateArguments(buffer, offset, count);

        ArrayBuilder.Append(buffer, offset, count);
    }

    /// <inheritdoc />
    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        ValidateArguments(buffer, offset, count);

        ArrayBuilder.Append(buffer, offset, count);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override void Flush()
    {
        // Do nothing.
    }

    /// <inheritdoc />
    public override Task FlushAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <inheritdoc />
    public override void SetLength(long value) => throw new NotSupportedException();

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        // Do nothing.
    }

    /// <inheritdoc />
    public override ValueTask DisposeAsync() => default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ValidateArguments(byte[] buffer, int offset, int count)
    {
        if (buffer == null)
        {
            ThrowHelper.ThrowArgumentNullException(nameof(buffer));
        }

        if (offset < 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(offset));
        }

        if (count < 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(count));
        }

        if (buffer.Length - offset < count)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(offset));
        }
    }

    private static class ThrowHelper
    {
        public static void ThrowArgumentNullException(string name)
            => throw new ArgumentNullException(name);

        public static void ThrowArgumentOutOfRangeException(string name)
            => throw new ArgumentOutOfRangeException(name);
    }
}
